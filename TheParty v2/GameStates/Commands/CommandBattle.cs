using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class CommandBattle : Command<TheParty>
    {
        BattleStore CurrentStore;
        string BackgroundName;
        public Vector2[] MemberPositions;
        public StateMachine<CommandBattle> StateMachine;

        public GUIChoiceBox FightOrFlee;
        public GUIChoice MemberChoice;

        public CommandBattle(string name)
        {
            BackgroundName = "TestBackground";
            CurrentStore = GameContent.Battles["TestBattle"];

            StateMachine = new StateMachine<CommandBattle>();
            StateMachine.SetNewCurrentState(this, new FightOrFlee());

            MemberPositions = new Vector2[BattleStore.TotalNumMembers(CurrentStore)];
            int MemberPosIdx = 0;
            for (int party = 0; party < CurrentStore.Parties.Length; party++)
            {
                for (int member = 0; member < CurrentStore.Parties[party].Members.Length; member++)
                {
                    Point MemberDrawOffset = new Point(-16, -24);
                    int MemberDrawStartX = (party == 0) ? 110 : 48;
                    int MemberXOffset = (party == 0) ? 16 : -16;
                    int MemberDrawX = MemberDrawStartX + member * MemberXOffset;
                    int MemberDrawStartY = 62;
                    int MemberDrawY = MemberDrawStartY + member * 16;
                    Point MemberDrawPos = new Point(MemberDrawX, MemberDrawY) + MemberDrawOffset;
                    MemberPositions[MemberPosIdx] = MemberDrawPos.ToVector2();
                    MemberPosIdx++;
                }
            }
        }

        public override void Enter(TheParty client)
        {
        }

        public override void Update(TheParty client, float deltaTime)
        {
            StateMachine.Update(this, deltaTime);
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            // Background
            Rectangle BackgroundRect = new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize);
            spriteBatch.Draw(GameContent.Sprites[BackgroundName], BackgroundRect, Color.White);
        
            // Fighters
            for (int party = 0; party < CurrentStore.Parties.Length; party++)
            {
                for (int member = 0; member < CurrentStore.Parties[party].Members.Length; member++)
                {
                    Point MemberDrawOffset = new Point(-16, -24);
                    int MemberDrawStartX = (party == 0) ? 110 : 48;
                    int MemberXOffset = (party == 0) ? 16 : -16;
                    int MemberDrawX = MemberDrawStartX + member * MemberXOffset;
                    int MemberDrawStartY = 62;
                    int MemberDrawY = MemberDrawStartY + member * 16;
                    Point MemberDrawPos = new Point(MemberDrawX, MemberDrawY) + MemberDrawOffset;
                    Point MemberDrawSize = new Point(32, 32);
                    Rectangle MemberDrawRect = new Rectangle(MemberDrawPos, MemberDrawSize);

                    int MemberSourceY = (party == 0) ? 64 : 96;
                    Point MemberSourcePos = new Point(0, MemberSourceY);
                    Point MemberSourceSize = new Point(32, 32);
                    Rectangle MemberSourceRect = new Rectangle(MemberSourcePos, MemberSourceSize);

                    spriteBatch.Draw(GameContent.Sprites["CharacterBase"], MemberDrawRect, MemberSourceRect, Color.White);
                }
            }

            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
