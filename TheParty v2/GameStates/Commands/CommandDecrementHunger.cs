using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandDecrementHunger : Command<TheParty>
    {
        List<Member> ActiveMembers;
        List<AnimatedSprite2D> Sprites;
        List<HeartsIndicator> Hearts;
        List<HeartsIndicator> Meats;
        List<StanceIndicator> Stances;
        
        List<Vector2> WindupSpots;
        List<LerpV> StanceLerps;

        GUIDialogueBox Food;
        GUIChoiceBox Choice;

        bool LowerCommitment;

        enum State { 
            Wait, 
            Windup, 
            Collide, 
            DecHunger, 
            Wait2, 
            Hit, 
            Wait3, 
            Kill, 
            Wait4, 
            LowerCommitment, 
            Wait5
        }
        State CurrentState;

        Timer WaitTimer;

        public CommandDecrementHunger(bool lowerCommitment = false)
        {
            Sprites = new List<AnimatedSprite2D>();
            Hearts = new List<HeartsIndicator>();
            Meats = new List<HeartsIndicator>();
            Stances = new List<StanceIndicator>();
            WindupSpots = new List<Vector2>();
            StanceLerps = new List<LerpV>();

            LowerCommitment = lowerCommitment;

            Rectangle FoodBounds = new Rectangle(new Point(28, 4), new Point(36, 18));
            Food = new GUIDialogueBox(FoodBounds, new[] { "\"" + GameContent.Variables["FoodSupply"].ToString() });
        }

        public override void Enter(TheParty client)
        {
            int party = 0;
            ActiveMembers = client.Player.ActiveParty.Members;
            int NumMembers = ActiveMembers.Count;

            for (int member = 0; member < ActiveMembers.Count; member++)
            {
                Vector2 MemberDrawOffset = new Vector2(-16, -16);
                int ScreenCenter = GraphicsGlobals.ScreenSize.X / 2;
                int MemberDrawStartX =
                    (party == 0) ?
                        NumMembers > 2 ?
                            ScreenCenter + 24 :
                            ScreenCenter + 24 + 45 :
                        NumMembers > 2 ?
                            ScreenCenter - 24 :
                            ScreenCenter - 24 - 45;

                int MemberXOffset = (party == 0) ? 18 : -18;
                int MemberDrawX = MemberDrawStartX + member * MemberXOffset - (member > 2 ? MemberXOffset / 2 : 0);
                int MemberDrawStartY = 54;
                int MemberDrawY = MemberDrawStartY + (member % 3) * 18;
                Vector2 MemberDrawPos = new Vector2(MemberDrawX, MemberDrawY);

                AnimatedSprite2D Sprite = new AnimatedSprite2D(ActiveMembers[member].BattleSpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("PositiveHit", 3, 4, 0.15f);
                Sprite.AddAnimation("NegativeHit", 4, 1, 0.15f);
                Sprite.AddAnimation("Dead", 5, 1, 0.15f);

                if (ActiveMembers[member].HP == 0)
                    Sprite.SetCurrentAnimation("Dead");
                else
                    Sprite.SetCurrentAnimation("Idle");

                Sprites.Add(Sprite);

                int HP = ActiveMembers[member].HP;
                Hearts.Add(new HeartsIndicator(HP, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 18));

                int Hunger = ActiveMembers[member].Hunger;
                HeartsIndicator HIndicator = new HeartsIndicator(Hunger, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 26, HeartsIndicator.Type.Meats);
                Meats.Add(HIndicator);

                int Stance = ActiveMembers[member].Stance;
                StanceIndicator SIndicator = new StanceIndicator(Stance, Sprite.DrawPos + new Vector2(-4, -28));
                SIndicator.HardSet(Stance);
                Stances.Add(SIndicator);

                Vector2 WindupSpot = HIndicator.Origin.ToVector2() + new Vector2(-32, 0);
                WindupSpots.Add(WindupSpot);

                LerpV StanceLerp = new LerpV(SIndicator.DrawPos, WindupSpot, 0.8f);
                StanceLerps.Add(StanceLerp);
            }

            CurrentState = State.Wait;

            WaitTimer = new Timer(0.8f);

            Food.SetNewText("\"" + GameContent.Variables["FoodSupply"].ToString());

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            foreach (AnimatedSprite2D sprite in Sprites)
                sprite.Update(deltaTime);

            foreach (HeartsIndicator health in Hearts)
                health.Update(deltaTime);

            foreach (HeartsIndicator meat in Meats)
                meat.Update(deltaTime);

            foreach (StanceIndicator stance in Stances)
                stance.Update(deltaTime);

            Food.Update(deltaTime, true);

            switch (CurrentState)
            {
                case State.Wait:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame && Stances.TrueForAll(s => s.Reached))
                        CurrentState = State.Windup;
                    break;

                case State.Windup:
                    StanceLerps.ForEach(sl => sl.Update(deltaTime));
                    Stances.ForEach(s => s.DrawPos = StanceLerps[Stances.IndexOf(s)].CurrentPosition);
                    if (StanceLerps.TrueForAll(sl => sl.Reached))
                    {
                        List<LerpV> NewLerps = new List<LerpV>();
                        foreach (LerpV lerp in StanceLerps)
                        {
                            Vector2 Start = lerp.CurrentPosition;
                            Vector2 End = Meats[StanceLerps.IndexOf(lerp)].Origin.ToVector2();
                            NewLerps.Add(new LerpV(Start, End, 0.1f));
                        }
                        StanceLerps = NewLerps;
                        CurrentState = State.Collide;
                    }

                    break;

                case State.Collide:
                    StanceLerps.ForEach(sl => sl.Update(deltaTime));
                    Stances.ForEach(s => s.DrawPos = StanceLerps[Stances.IndexOf(s)].CurrentPosition);
                    if (StanceLerps.TrueForAll(sl => sl.Reached))
                    {
                        CurrentState = State.DecHunger;
                    }
                    break;

                case State.DecHunger:

                    for (int member = 0; member < ActiveMembers.Count; member++)
                    {
                        Member Member = ActiveMembers[member];
                        if (Member.Hunger > 0 && Member.HP > 0)
                        {
                            Member.Hunger -= Member.Stance;
                            if (Member.Hunger < 0)
                                Member.Hunger = 0;

                            Meats[member].SetHP(Member.Hunger);
                        }
                    }

                    WaitTimer.Reset();
                    CurrentState = State.Wait2;
                    break;

                case State.Wait2:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        CurrentState = State.Hit;
                    }
                    break;

                case State.Hit:
                    for (int member = 0; member < ActiveMembers.Count; member++)
                    {
                        if (ActiveMembers[member].Hunger == 0 && ActiveMembers[member].HP > 0)
                        {
                            ActiveMembers[member].HitHP(-1);
                            Sprites[member].SetCurrentAnimation("NegativeHit");
                        }
                        Hearts[member].SetHP(ActiveMembers[member].HP);
                    }

                    WaitTimer.Reset();
                    CurrentState = State.Wait3;
                    break;

                case State.Wait3:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                        CurrentState = State.Kill;
                    break;

                case State.Kill:
                    for (int member = 0; member < ActiveMembers.Count; member++)
                    {
                        if (ActiveMembers[member].HP == 0)
                        {
                            Sprites[member].SetCurrentAnimation("Dead");
                        }
                        else
                        {
                            Sprites[member].SetCurrentAnimation("Idle");
                        }
                    }
                    WaitTimer.Reset();

                    foreach (StanceIndicator si in Stances)
                    {
                        si.DrawPos = Sprites[Stances.IndexOf(si)].DrawPos + new Vector2(-4, -28);
                    }

                    if (client.Player.ActiveParty.Members.TrueForAll(m => m.HP == 0))
                        client.CommandQueue.PushCommand(new CommandGameOver());

                    CurrentState = State.Wait4;
                    break;

                case State.Wait4:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        if (LowerCommitment)
                            CurrentState = State.LowerCommitment;
                        else
                        {
                            Done = true;
                        }
                    }
                    break;

                case State.LowerCommitment:
                    for (int i = 0; i < ActiveMembers.Count; i++)
                    {
                        Member Member = ActiveMembers[i];
                        if (Member.HP > 0)
                        {
                            Member.HitStance(-1);
                            Stances[i].SetTarget(Member.Stance);
                        }
                    }
                    WaitTimer = new Timer(1.5f);
                    CurrentState = State.Wait5;
                    break;

                case State.Wait5:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame && Stances.TrueForAll(s => s.Reached))
                    {
                        Done = true;
                    }
                    break;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize), Color.White);

            bool DrawHearts = (CurrentState != State.Wait &&
                CurrentState != State.Windup &&
                CurrentState != State.Collide);

            bool DrawStance = (CurrentState == State.Wait ||
                CurrentState == State.Windup ||
                CurrentState == State.Collide ||
                CurrentState == State.Wait4 ||
                CurrentState == State.LowerCommitment ||
                CurrentState == State.Wait5);

            if (!Entered)
                return;

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                Sprites[i].Draw(spriteBatch);

                if (ActiveMembers[i].HP > 0)
                {
                    if (DrawHearts) Hearts[i].Draw(spriteBatch, Vector2.Zero);
                    if (DrawStance) Stances[i].Draw(spriteBatch);
                    Meats[i].Draw(spriteBatch, Vector2.Zero);
                }


            }
        }
    }
}
