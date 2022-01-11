using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class CommandBattle : Command<TheParty>
    {
        public Battle CurrentStore;
        string BackgroundName;
        public List<AnimatedSprite2D> Sprites;
        public List<HeartsIndicator> HPIndicators;
        public List<StanceIndicator> StanceIndicators;
        public List<AnimatedSprite2D> StatusIndicators;
        public Timer StatusRotateTimer;
        public int StatusCounter;
        public StateMachine<CommandBattle> StateMachine;

        public GUIChoice MemberChoice;
        public GUIChoiceBox MoveChoice;
        public GUIChoice TargetChoice;

        public Move CurrentMove;
        public Targeting CurrentTargeting;

        public LerpV Movement;
        public Vector2 ReturnToPos;

        public bool GameOver;
        public bool ContinueAfter;
        public string SwitchToSet;

        public bool CanFlee;

        public LerpV BackgroundLerp1;
        public LerpV BackgroundLerp2;


        public CommandBattle(string name, string switchToSet)
        {
            BackgroundName = "TestBackground";
            CurrentStore = GameContent.Battles[name];
            ContinueAfter = true;
            SwitchToSet = switchToSet;

            CanFlee = SwitchToSet == "DidntFlee";

            Instantiate();
        }



        public List<int> PartyIdxs(int partyIdx)
        {
            List<int> Result = new List<int>();
            int Idx = 0;
            for (int p = 0; p < CurrentStore.NumParties; p++)
                for (int m = 0; m < CurrentStore.Parties[p].NumMembers; m++)
                {
                    if (p == partyIdx) 
                        Result.Add(Idx);
                    Idx++;
                }
            return Result;
        }

        public List<AnimatedSprite2D> PartySprites(int partyIdx)
            => PartyIdxs(partyIdx).ConvertAll(i => Sprites[i]);

        public int MemberSpriteIdx(int partyIdx, int memberIdx)
            => PartyIdxs(partyIdx)[memberIdx];


        public void SetAppropriateAnimations()
        {
            // Set Appropriate animations in aftermath
            for (int s = 0; s < Sprites.Count; s++)
            {
                List<Member> AllMembers = CurrentStore.AllMembers();
                string Animation = 
                    AllMembers[s].HP <= 0 ? "Dead" :
                    CurrentStore.CurrentTurnPartyIdx == 0 ?
                        AllMembers[s].CanGo ? "Move" : "Idle" :
                    "Idle";

                Sprites[s].SetCurrentAnimation(Animation);
            }
        }
        public Member FromMember
            => CurrentStore.Member(CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);

        public Member ToMember
            => CurrentStore.Member(CurrentTargeting.ToPartyIdx, CurrentTargeting.ToMemberIdx);
        
        public bool MoveValidOnAnyone(Move move)
            => move.ValidOnAnyone(CurrentStore, CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);

        public int FromSpriteIdx
            => MemberSpriteIdx(CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);
        public int TargetSpriteIdx
            => MemberSpriteIdx(CurrentTargeting.ToPartyIdx, CurrentTargeting.ToMemberIdx);

        public AnimatedSprite2D ToSprite
            => Sprites[TargetSpriteIdx];

        public AnimatedSprite2D FromSprite
            => Sprites[FromSpriteIdx];

        public Vector2 InFrontOfTarget
            => ToSprite.DrawPos + new Vector2(ToSprite.Flip ? 16 : -16, 0);

        public int PartyIdxOf(int memberIdx)
        {
            int i = 0;
            for (int party = 0; party < CurrentStore.NumParties; party++)
                for (int member = 0; member < CurrentStore.Parties[party].NumMembers; member++)
                {
                    if (i == memberIdx)
                        return party;
                    i++;
                }
            throw new Exception("Party index of member " + memberIdx + " not found.");
        }

        public int PartyMemberIdxOf(int memberIdx)
        {
            int i = 0;
            for (int party = 0; party < CurrentStore.NumParties; party++)
                for (int member = 0; member < CurrentStore.Parties[party].NumMembers; member++)
                {
                    if (i == memberIdx)
                        return member;
                    i++;
                }
            throw new Exception("Party index of member " + memberIdx + " not found.");
        }

        private void Instantiate()
        {
            StateMachine = new StateMachine<CommandBattle>();
            Sprites = new List<AnimatedSprite2D>();
            HPIndicators = new List<HeartsIndicator>();
            StanceIndicators = new List<StanceIndicator>();
            StatusIndicators = new List<AnimatedSprite2D>();

            foreach (Member member in CurrentStore.AllMembers())
            {
                member.StatusEffects = new List<StatusEffect>();
            }

            BackgroundLerp1 = new LerpV(new Vector2(0, 0), new Vector2(160, 144), 10f);
            BackgroundLerp2 = new LerpV(new Vector2(160, 144), new Vector2(0, 0), 10f);

            GameOver = false;
        }

        public override void Enter(TheParty client)
        {
            foreach (Member member in client.Player.ActiveParty.Members)
                member.GoneThisTurn = false;

            CurrentStore.Parties[0] = client.Player.ActiveParty;

            for (int party = 0; party < CurrentStore.NumParties; party++)
            {
                for (int member = 0; member < CurrentStore.Parties[party].NumMembers; member++)
                {
                    Vector2 MemberDrawOffset = new Vector2(-16, -16);
                    int MemberDrawStartX = (party == 0) ? 110 : 58;
                    int MemberXOffset = (party == 0) ? 16 : -16;
                    int MemberDrawX = MemberDrawStartX + member * MemberXOffset;
                    int MemberDrawStartY = 62;
                    int MemberDrawY = MemberDrawStartY + member * 16;
                    Vector2 MemberDrawPos = new Vector2(MemberDrawX, MemberDrawY);

                    Member ThisMember = CurrentStore.Parties[party].Members[member];
                    AnimatedSprite2D Sprite = new AnimatedSprite2D(ThisMember.SpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                    Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                    Sprite.AddAnimation("Move", 1, 4, 0.15f);
                    Sprite.AddAnimation("KOd", 2, 2, 0.15f);
                    Sprite.AddAnimation("PositiveHit", 3, 2, 0.15f);
                    Sprite.AddAnimation("NegativeHit", 4, 1, 0.15f);
                    Sprite.AddAnimation("Dead", 5, 1, 0.15f);
                    Sprite.SetCurrentAnimation("Idle");
                    Sprites.Add(Sprite);

                    int HP = CurrentStore.Parties[party].Members[member].HP;
                    HPIndicators.Add(new HeartsIndicator(HP, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 18));

                    int Stance = CurrentStore.Parties[party].Members[member].Stance;
                    StanceIndicators.Add(new StanceIndicator(0, Sprite.DrawPos + new Vector2(-4, -26)));

                    StatusIndicators.Add(GameContent.AnimationSheets["StatusAnimations"].DeepCopy());
                }
            }

            StatusCounter = 0;
            StatusRotateTimer = new Timer(0.8f);

            if (CurrentStore.CurrentTurnPartyIdx == 1)
                StateMachine.SetNewCurrentState(this, new PreBattleMessage("Suprise attack!"));
            else
                StateMachine.SetNewCurrentState(this, new ChooseMember());

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            List<Member> AllMembers = CurrentStore.AllMembers();
            HPIndicators.ForEach(h => h.SetHP(AllMembers[HPIndicators.IndexOf(h)].HP));
            StanceIndicators.ForEach(s => s.SetTarget(AllMembers[StanceIndicators.IndexOf(s)].Stance));

            Sprites.ForEach(s => s.Update(deltaTime));
            HPIndicators.ForEach(h => h.TopCenter = (Sprites[HPIndicators.IndexOf(h)].DrawPos.ToPoint() + new Point(0, 18)));
            HPIndicators.ForEach(h => h.Update(deltaTime));
            StanceIndicators.ForEach(s => s.DrawPos = Sprites[StanceIndicators.IndexOf(s)].DrawPos + new Vector2(-4, -26));
            StanceIndicators.ForEach(s => s.Update(deltaTime));
            StatusIndicators.ForEach(s => s.DrawPos = Sprites[StatusIndicators.IndexOf(s)].DrawPos + new Vector2(-4, -26));
            StatusIndicators.ForEach(s => s.Update(deltaTime));

            // Rotate through status effects every second or so
            StatusRotateTimer.Update(deltaTime);
            if (StatusRotateTimer.TicThisFrame)
            {
                StatusCounter++;
                foreach (var StatusIndicator in StatusIndicators)
                {
                    int Idx = StatusIndicators.IndexOf(StatusIndicator);
                    Member Member = CurrentStore.AllMembers()[Idx];
                    if (Member.StatusEffects.Count > 0)
                    {
                        int NewStatusIdx = StatusCounter % Member.StatusEffects.Count;
                        StatusEffect ToShow = Member.StatusEffects[NewStatusIdx];
                        StatusIndicator.SetCurrentAnimation(ToShow.Name);
                    }
                    else
                    {
                        StatusIndicator.SetCurrentAnimation("None");
                    }
                }
            }

            StateMachine.Update(this, deltaTime);

            if (GameOver)
            {
                if (ContinueAfter)
                {
                    GameContent.Switches[SwitchToSet] = false;
                }
                else
                {
                    client.CommandQueue.ClearCommands();
                    client.StateMachine.SetNewCurrentState(client, new GameStateGameOver());
                }

                Done = true;
                return;
            }

            BackgroundLerp1.Update(deltaTime);
            BackgroundLerp2.Update(deltaTime);

            if (BackgroundLerp1.Reached)
            {
                BackgroundLerp1 = new LerpV(new Vector2(0, 0), new Vector2(160, 144), 10f);
                BackgroundLerp2 = new LerpV(new Vector2(160, 144), new Vector2(0, 0), 10f);
            }

        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            // Background
            Rectangle BackgroundRect = new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize);
            spriteBatch.Draw(GameContent.Sprites["Black"], BackgroundRect, Color.White);
            Rectangle BackRect1 = new Rectangle(
                BackgroundLerp1.CurrentPosition.ToPoint() - new Point(160, 144), 
                new Point(320, 288));
            Rectangle BackRect2 = new Rectangle(
                BackgroundLerp2.CurrentPosition.ToPoint() - new Point(160, 144),
                new Point(320, 288));

            spriteBatch.Draw(GameContent.Sprites["BattleBackground"], BackRect1, Color.White);
            spriteBatch.Draw(GameContent.Sprites["BattleBackground"], BackRect2, Color.White);

            // Sort sprites by y position
            List<AnimatedSprite2D> Sorted = new List<AnimatedSprite2D>(Sprites);
            Sorted.Sort((s1, s2) => s1.DrawPos.Y > s2.DrawPos.Y ? 1 : -1);

            HPIndicators.ForEach(h => h.Draw(spriteBatch));
            StatusIndicators.ForEach(si => si.Draw(spriteBatch));
            StanceIndicators.ForEach(si => si.Draw(spriteBatch));

            if (Entered)
            {
                List<Member> AllMembers = CurrentStore.AllMembers();

                foreach (Member member in AllMembers)
                {
                    int Idx = AllMembers.IndexOf(member);
                    int PartyIdx = Idx >= CurrentStore.Parties[0].Members.Count ? 1 : 0;

                    if (member.HP > 0)
                    {
                        // Draw various effects
                        Vector2 EffectsStartPos = Sprites[Idx].DrawPos + new Vector2(16, 0);
                        List<string> Effects = new List<string>();
                        member.StatusEffects.ForEach(se => Effects.AddRange(se.PassiveEffects));
                        Effects.AddRange(member.Equipped().PassiveEffects);

                        // Naive solution. Clean up later.
                        List<string> Tokens = new List<string>();
                        foreach (string Effect in Effects)
                        {
                            string[] Keywords = Effect.Split(' ');
                            if (Keywords[0] == "Attack")
                            {
                                if (Keywords[1] == "+")
                                    for (int i = 0; i < int.Parse(Keywords[2]); i++)
                                        Tokens.Add("AttackUp");
                                if (Keywords[1] == "-")
                                    for (int i = 0; i < int.Parse(Keywords[2]); i++)
                                        Tokens.Add("AttackDown");
                            }
                            else if (Keywords[0] == "Defense")
                            {
                                if (Keywords[1] == "+")
                                    for (int i = 0; i < int.Parse(Keywords[2]); i++)
                                        Tokens.Add("DefenseUp");
                                if (Keywords[1] == "-")
                                    for (int i = 0; i < int.Parse(Keywords[2]); i++)
                                        Tokens.Add("DefenseDown");
                            }
                        }

                        for (int i = 0; i < Tokens.Count; i++)
                        {
                            string Token = Tokens[i];
                            int SourceX =
                                Token == "AttackUp" ? 0 * 8:
                                Token == "DefenseUp" ? 1 * 8:
                                Token == "AttackDown" ? 2 * 8:
                                Token == "DefenseDown" ? 3 * 8:
                                -1;
                            int DrawX = 16 + i * 8;
                            if (PartyIdx == 1)
                                DrawX = -DrawX;

                            Point DrawPos = Sprites[Idx].DrawPos.ToPoint() + 
                                new Point(DrawX, 0);
                            Point SourcePos = new Point(SourceX, 0);

                            spriteBatch.Draw(
                                GameContent.Sprites["Effects"],
                                new Rectangle(DrawPos, new Point(8, 8)),
                                new Rectangle(SourcePos, new Point(8, 8)),
                                Color.White);
                        }
                    }
                }

                Sorted.ForEach(s => s.Draw(spriteBatch));

            }


            // Any Extra State-Related Stuff
            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
            GameContent.Parties["PlayerParty"] = CurrentStore.Parties[0];
        }
    }
}
