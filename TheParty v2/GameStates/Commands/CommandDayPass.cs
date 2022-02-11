using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandDayPass : Command<TheParty>
    {
        enum State { Wait1, DayGoDown, Wait2, LowerCommitment, Wait3 }
        State CurrentState;

        Timer WaitTimer;

        GUIDialogueBox Days;

        List<Member> ActiveMembers;
        List<AnimatedSprite2D> Sprites;
        List<HeartsIndicator> Hearts;
        List<HeartsIndicator> Meats;
        List<StanceIndicator> Stances;


        public override void Enter(TheParty client)
        {
            CurrentState = State.Wait1;
            WaitTimer = new Timer(1f);

            Rectangle DaysBounds = new Rectangle(new Point(100, 4), new Point(36, 18));
            Days = new GUIDialogueBox(DaysBounds, new[] { "&" + GameContent.Variables["DaysRemaining"].ToString() });

            ActiveMembers = client.Player.ActiveParty.Members;
            Sprites = new List<AnimatedSprite2D>();
            Hearts = new List<HeartsIndicator>();
            Meats = new List<HeartsIndicator>();
            Stances = new List<StanceIndicator>();

            int party = 0;
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

                Member mem = ActiveMembers[member];
                AnimatedSprite2D Sprite = new AnimatedSprite2D(mem.BattleSpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("PositiveHit", 3, 1, 0.15f);
                Sprite.AddAnimation("Dead", 5, 1, 0.15f);

                if (mem.HP > 0)
                    Sprite.SetCurrentAnimation("Idle");
                else
                    Sprite.SetCurrentAnimation("Dead");

                Sprites.Add(Sprite);

                int HP = ActiveMembers[member].HP;
                Hearts.Add(new HeartsIndicator(
                    HP,
                    (int)Sprite.DrawPos.X,
                    (int)Sprite.DrawPos.Y + 18,
                    HeartsIndicator.Type.Hearts, true,
                    ActiveMembers[member].MaxHP));

                int Hunger = ActiveMembers[member].Hunger;
                Meats.Add(new HeartsIndicator(
                    Hunger,
                    (int)Sprite.DrawPos.X,
                    (int)Sprite.DrawPos.Y + 28,
                    HeartsIndicator.Type.Meats, true,
                    ActiveMembers[member].MaxHunger));

                int Stance = ActiveMembers[member].Stance;
                StanceIndicator SIndicator = new StanceIndicator(Stance, Sprite.DrawPos + new Vector2(-4, -28));
                SIndicator.HardSet(Stance);
                Stances.Add(SIndicator);
            }

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Days.Update(deltaTime, true);
            Sprites.ForEach(s => s.Update(deltaTime));
            Meats.ForEach(m => m.Update(deltaTime));
            Stances.ForEach(s => s.Update(deltaTime));
            Hearts.ForEach(h => h.Update(deltaTime));


            switch (CurrentState)
            {
                case State.Wait1:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        WaitTimer.Reset();
                        CurrentState = State.DayGoDown;
                    }
                    break;

                case State.DayGoDown:
                    GameContent.Variables["DaysRemaining"] -= 1;
                    Days.SetNewText("&" + GameContent.Variables["DaysRemaining"].ToString());
                    CurrentState = State.Wait2;
                    break;

                case State.Wait2:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        WaitTimer.Reset();
                        CurrentState = State.LowerCommitment;
                    }
                    break;

                case State.LowerCommitment:
                    for (int i = 0; i < ActiveMembers.Count; i++)
                    {
                        if (ActiveMembers[i].HP > 0 && ActiveMembers[i].Stance > 0)
                        {
                            ActiveMembers[i].Stance -= 1;
                            Stances[i].SetTarget(ActiveMembers[i].Stance);
                        }
                    }
                    WaitTimer = new Timer(2f);
                    CurrentState = State.Wait3;
                    break;

                case State.Wait3:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        WaitTimer.Reset();
                        Done = true;
                    }
                    break;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            if (!Entered)
                return;

            spriteBatch.Draw(
                GameContent.Sprites["Black"],
                new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                Color.White);

            Days.Draw(spriteBatch, true);

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                if (ActiveMembers[i].HP > 0)
                {
                    Hearts[i].Draw(spriteBatch);
                    Meats[i].Draw(spriteBatch);
                    Stances[i].Draw(spriteBatch);
                }

                Sprites[i].Draw(spriteBatch);
            }
        }
    }
}
