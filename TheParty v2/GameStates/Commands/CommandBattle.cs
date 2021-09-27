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

        GUIChoiceBox Choice;

        public CommandBattle(string name)
        {
            BackgroundName = "TestBackground";
            CurrentStore = new BattleStore()
            {
                CurrentTurnPartyIdx = 0,
                AvailableCharge = 0,
                Parties = new Party[]
                {
                    new Party()
                    {
                        AIControlled = false,
                        Members = new Member[]
                        {
                            new Member()
                            {
                                HP = 6,
                                Stance = 2,
                                Charged = false,
                                KOdFor = 0,
                                HasGoneThisTurn = false
                            },
                            new Member()
                            {
                                HP = 6,
                                Stance = 2,
                                Charged = false,
                                KOdFor = 0,
                                HasGoneThisTurn = false
                            }
                        }
                    },
                    new Party()
                    {
                        AIControlled = false,
                        Members = new Member[]
                        {
                            new Member()
                            {
                                HP = 6,
                                Stance = 2,
                                Charged = false,
                                KOdFor = 0,
                                HasGoneThisTurn = false
                            },
                            new Member()
                            {
                                HP = 6,
                                Stance = 2,
                                Charged = false,
                                KOdFor = 0,
                                HasGoneThisTurn = false
                            },
                            new Member()
                            {
                                HP = 6,
                                Stance = 2,
                                Charged = false,
                                KOdFor = 0,
                                HasGoneThisTurn = false
                            }
                        }
                    }
                }
            };

            string[] Choices = new string[] { "Hit", "Hurt", "Charge" };
            Choice = new GUIChoiceBox(Choices);
        }

        public override void Enter(TheParty client)
        {
        }

        public override void Update(TheParty client, float deltaTime)
        {

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
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
