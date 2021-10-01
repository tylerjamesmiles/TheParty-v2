using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class CommandBattle : Command<TheParty>
    {
        public BattleStore CurrentStore;
        string BackgroundName;
        public List<AnimatedSprite2D> Sprites;
        public List<HeartsIndicator> HPIndicators;
        public List<StanceIndicator> StanceIndicators;
        public StateMachine<CommandBattle> StateMachine;

        public GUIChoiceBox FightOrFlee;
        public GUIChoice MemberChoice;
        public GUIChoiceBox MoveChoice;
        public GUIChoice TargetChoice;

        public Move CurrentMove;
        public Targeting CurrentTargeting;

        public LerpV Movement;
        public Vector2 ReturnToPos;

        public CommandBattle(string name)
        {
            BackgroundName = "TestBackground";
            CurrentStore = GameContent.Battles["TestBattle"];

            StateMachine = new StateMachine<CommandBattle>();
            StateMachine.SetNewCurrentState(this, new FightOrFlee());

            Sprites = new List<AnimatedSprite2D>();
            HPIndicators = new List<HeartsIndicator>();
            StanceIndicators = new List<StanceIndicator>();
            for (int party = 0; party < CurrentStore.Parties.Length; party++)
            {
                for (int member = 0; member < CurrentStore.Parties[party].Members.Length; member++)
                {
                    Vector2 MemberDrawOffset = new Vector2(-16, -16);
                    int MemberDrawStartX = (party == 0) ? 110 : 48;
                    int MemberXOffset = (party == 0) ? 16 : -16;
                    int MemberDrawX = MemberDrawStartX + member * MemberXOffset;
                    int MemberDrawStartY = 62;
                    int MemberDrawY = MemberDrawStartY + member * 16;
                    Vector2 MemberDrawPos = new Vector2(MemberDrawX, MemberDrawY);

                    AnimatedSprite2D Sprite = new AnimatedSprite2D("TestFighter", new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                    Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                    Sprite.AddAnimation("Move", 1, 4, 0.15f);
                    Sprite.AddAnimation("KOd", 2, 4, 0.15f);
                    Sprite.AddAnimation("Hit", 3, 1, 0.15f);
                    Sprite.AddAnimation("Dead", 4, 1, 0.15f);
                    Sprite.SetCurrentAnimation("Idle");
                    Sprites.Add(Sprite);

                    int HP = CurrentStore.Parties[party].Members[member].HP;
                    HPIndicators.Add(new HeartsIndicator(HP, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 18));

                    int Stance = CurrentStore.Parties[party].Members[member].Stance;
                    StanceIndicators.Add(new StanceIndicator(0, Sprite.DrawPos + new Vector2(-4, -26)));
                }
            }
        }

        public List<int> PartyIdxs(int partyIdx)
        {
            List<int> Result = new List<int>();
            int Idx = 0;
            for (int p = 0; p < CurrentStore.Parties.Length; p++)
                for (int m = 0; m < CurrentStore.Parties[p].Members.Length; m++)
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
                Member[] AllMembers = CurrentStore.AllMembers();
                string Animation = 
                    AllMembers[s].HP <= 0 ? "Dead" :
                    AllMembers[s].KOdFor > 0 ? "KOd" : 
                    "Idle";

                Sprites[s].SetCurrentAnimation(Animation);
            }
        }
        public Member FromMember
            => BattleStore.Member(CurrentStore, CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);
        

        public bool MoveValidOnAnyone(Move move)
            => Move.ValidOnAnyone(move, CurrentStore, CurrentTargeting.FromPartyIdx, CurrentTargeting.FromMemberIdx);

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
            for (int party = 0; party < CurrentStore.Parties.Length; party++)
                for (int member = 0; member < CurrentStore.Parties[party].Members.Length; member++)
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
            for (int party = 0; party < CurrentStore.Parties.Length; party++)
                for (int member = 0; member < CurrentStore.Parties[party].Members.Length; member++)
                {
                    if (i == memberIdx)
                        return member;
                    i++;
                }
            throw new Exception("Party index of member " + memberIdx + " not found.");
        }
                

        public override void Enter(TheParty client)
        {
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Member[] AllMembers = CurrentStore.AllMembers();
            HPIndicators.ForEach(h => h.SetHP(AllMembers[HPIndicators.IndexOf(h)].HP));
            StanceIndicators.ForEach(s => s.SetStance(AllMembers[StanceIndicators.IndexOf(s)].Stance));

            Sprites.ForEach(s => s.Update(deltaTime));
            HPIndicators.ForEach(h => h.SetPos(Sprites[HPIndicators.IndexOf(h)].DrawPos.ToPoint() + new Point(0, 18)));
            HPIndicators.ForEach(h => h.Update(deltaTime));
            StanceIndicators.ForEach(s => s.DrawPos = Sprites[StanceIndicators.IndexOf(s)].DrawPos + new Vector2(-4, -26));
            StanceIndicators.ForEach(s => s.Update(deltaTime));

            StateMachine.Update(this, deltaTime);
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

            StanceIndicators.ForEach(s => s.Draw(spriteBatch));

            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
