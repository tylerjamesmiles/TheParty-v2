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
        public StateMachine<CommandBattle> StateMachine;

        public GUIChoiceBox FightOrFlee;
        public GUIChoice MemberChoice;
        public GUIChoiceBox MoveChoice;
        public GUIChoice TargetChoice;

        public CommandBattle(string name)
        {
            BackgroundName = "TestBackground";
            CurrentStore = GameContent.Battles["TestBattle"];

            StateMachine = new StateMachine<CommandBattle>();
            StateMachine.SetNewCurrentState(this, new FightOrFlee());

            Sprites = new List<AnimatedSprite2D>();
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
                    Sprite.AddAnimation("Active", 0, 4, 0.2f);
                    Sprite.SetCurrentAnimation("Active");
                    Sprites.Add(Sprite);
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

        //public List<AnimatedSprite2D> MemberSprites(Predicate<Member> pred)
        //    => PartySprites[]

        public override void Enter(TheParty client)
        {
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Sprites.ForEach(s => s.Update(deltaTime));

            StateMachine.Update(this, deltaTime);
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            // Background
            Rectangle BackgroundRect = new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize);
            spriteBatch.Draw(GameContent.Sprites[BackgroundName], BackgroundRect, Color.White);

            Sprites.ForEach(s => s.Draw(spriteBatch));

            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
