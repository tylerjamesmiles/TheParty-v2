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

        public GUIChoiceBox FightOrFlee;
        public GUIChoice MemberChoice;
        public GUIChoiceBox MoveChoice;
        public GUIChoice TargetChoice;

        public Move CurrentMove;
        public Targeting CurrentTargeting;

        public LerpV Movement;
        public Vector2 ReturnToPos;

        public bool GameOver;

        public CommandBattle(string name)
        {
            BackgroundName = "TestBackground";
            CurrentStore = GameContent.Battles["TestBattle"];

            StateMachine = new StateMachine<CommandBattle>();
            StateMachine.SetNewCurrentState(this, new FightOrFlee());

            GameOver = false;
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
                    AllMembers[s].KOd ? "KOd" : 
                    AllMembers[s].Charged ? "Charged" :
                    "Idle";

                Sprites[s].SetCurrentAnimation(Animation);
            }
        }
        public Member FromMember
            => CurrentStore.Member(CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);
        
        public bool MoveValidOnAnyone(Move move)
            => move.ValidOnAnyone(CurrentStore, CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);

        public int FromSpriteIdx
            => MemberSpriteIdx(CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);
        public int TargetSpriteIdx
            => MemberSpriteIdx(CurrentTargeting.ToPartyIdx, CurrentTargeting.ToMemberIdx);

        public AnimatedSprite2D TargetSprite
            => Sprites[TargetSpriteIdx];

        public AnimatedSprite2D FromSprite
            => Sprites[FromSpriteIdx];

        public Vector2 InFrontOfTarget
            => TargetSprite.DrawPos + new Vector2(TargetSprite.Flip ? 16 : -16, 0);

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

        public override void Enter(TheParty client)
        {
            CurrentStore.Parties[0] = client.Player.ActiveParty;

            Sprites = new List<AnimatedSprite2D>();
            HPIndicators = new List<HeartsIndicator>();
            StanceIndicators = new List<StanceIndicator>();
            StatusIndicators = new List<AnimatedSprite2D>();

            for (int party = 0; party < CurrentStore.NumParties; party++)
            {
                for (int member = 0; member < CurrentStore.Parties[party].NumMembers; member++)
                {
                    Vector2 MemberDrawOffset = new Vector2(-16, -16);
                    int MemberDrawStartX = (party == 0) ? 110 : 48;
                    int MemberXOffset = (party == 0) ? 16 : -16;
                    int MemberDrawX = MemberDrawStartX + member * MemberXOffset;
                    int MemberDrawStartY = 62;
                    int MemberDrawY = MemberDrawStartY + member * 16;
                    Vector2 MemberDrawPos = new Vector2(MemberDrawX, MemberDrawY);

                    Member ThisMember = CurrentStore.Parties[party].Members[member];
                    AnimatedSprite2D Sprite = new AnimatedSprite2D(ThisMember.SpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                    Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                    Sprite.AddAnimation("Move", 1, 4, 0.15f);
                    Sprite.AddAnimation("Charged", 2, 4, 0.15f);
                    Sprite.AddAnimation("KOd", 3, 2, 0.15f);
                    Sprite.AddAnimation("PositiveHit", 4, 4, 0.15f);
                    Sprite.AddAnimation("NegativeHit", 5, 1, 0.15f);
                    Sprite.AddAnimation("Dead", 6, 1, 0.15f);
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

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            List<Member> AllMembers = CurrentStore.AllMembers();
            HPIndicators.ForEach(h => h.SetHP(AllMembers[HPIndicators.IndexOf(h)].HP));
            StanceIndicators.ForEach(s => s.SetStance(AllMembers[StanceIndicators.IndexOf(s)].Stance));

            Sprites.ForEach(s => s.Update(deltaTime));
            HPIndicators.ForEach(h => h.SetPos(Sprites[HPIndicators.IndexOf(h)].DrawPos.ToPoint() + new Point(0, 18)));
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
                        StatusIndicator.SetCurrentAnimation(ToShow.AnimationName);
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
                client.CommandQueue.ClearCommands();
                client.StateMachine.SetNewCurrentState(client, new GameStateGameOver());
                Done = true;
                return;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            // Background
            Rectangle BackgroundRect = new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize);
            spriteBatch.Draw(GameContent.Sprites[BackgroundName], BackgroundRect, Color.White);

            List<AnimatedSprite2D> Sorted = new List<AnimatedSprite2D>(Sprites);
            Sorted.Sort((s1, s2) => s1.DrawPos.Y > s2.DrawPos.Y ? 1 : -1);

            HPIndicators.ForEach(h => h.Draw(spriteBatch));

            Sorted.ForEach(s => s.Draw(spriteBatch));

            List<Member> AllMembers = CurrentStore.AllMembers();
            foreach (Member member in AllMembers)
            {
                int Idx = AllMembers.IndexOf(member);
                if (member.HP > 0)
                {
                    StanceIndicators[Idx].Draw(spriteBatch);
                    StatusIndicators[Idx].Draw(spriteBatch);
                }
            }

            // Exshtra Shtuff
            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
            GameContent.Parties["PlayerParty"] = CurrentStore.Parties[0];
        }
    }
}
