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

        enum State { Wait, DecHunger, Wait2, Hit, Wait3, Kill, Wait4 }
        State CurrentState;

        Timer WaitTimer1;
        Timer WaitTimer2;
        Timer WaitTimer3;
        Timer WaitTimer4;

        public override void Enter(TheParty client)
        {
            int party = 0;
            ActiveMembers = client.Player.ActiveParty.Members;
            Sprites = new List<AnimatedSprite2D>();
            Hearts = new List<HeartsIndicator>();
            Meats = new List<HeartsIndicator>();
            for (int member = 0; member < ActiveMembers.Count; member++)
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
                Sprite.AddAnimation("PositiveHit", 4, 4, 0.15f);
                Sprite.AddAnimation("NegativeHit", 5, 1, 0.15f);
                Sprite.AddAnimation("Dead", 6, 1, 0.15f);
                Sprite.SetCurrentAnimation("Idle");
                Sprites.Add(Sprite);

                int HP = ActiveMembers[member].HP;
                Hearts.Add(new HeartsIndicator(HP, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 18));

                int Hunger = ActiveMembers[member].Hunger;
                Meats.Add(new HeartsIndicator(Hunger, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 26, true));
            }

            CurrentState = State.Wait;

            WaitTimer1 = new Timer(1.0f);
            WaitTimer2 = new Timer(1.5f);
            WaitTimer3 = new Timer(1.0f);
            WaitTimer4 = new Timer(1.0f);

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

            switch (CurrentState)
            {
                case State.Wait:
                    WaitTimer1.Update(deltaTime);
                    if (WaitTimer1.TicThisFrame)
                        CurrentState = State.DecHunger;
                    break;

                case State.DecHunger:
                    for (int member = 0; member < ActiveMembers.Count; member++)
                        if (ActiveMembers[member].Hunger > 0)
                        {
                            ActiveMembers[member].Hunger -= 1;
                            Meats[member].SetHP(ActiveMembers[member].Hunger);
                        }
                    CurrentState = State.Wait2;
                    break;

                case State.Wait2:
                    WaitTimer2.Update(deltaTime);
                    if (WaitTimer2.TicThisFrame)
                    {
                        CurrentState = State.Hit;
                    }
                    break;

                case State.Hit:
                    for (int member = 0; member < ActiveMembers.Count; member++)
                    {
                        if (ActiveMembers[member].Hunger == 0)
                        {
                            ActiveMembers[member].HitHP(-2);
                            Sprites[member].SetCurrentAnimation("NegativeHit");
                        }
                        else
                        {
                            ActiveMembers[member].HitHP(+1);
                            Sprites[member].SetCurrentAnimation("PositiveHit");
                        }
                        Hearts[member].SetHP(ActiveMembers[member].HP);

                    }
                    CurrentState = State.Wait3;
                    break;

                case State.Wait3:
                    WaitTimer3.Update(deltaTime);
                    if (WaitTimer3.TicThisFrame)
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
                    CurrentState = State.Wait4;
                    break;

                case State.Wait4:
                    WaitTimer4.Update(deltaTime);
                    if (WaitTimer4.TicThisFrame)
                        Done = true;
                    break;

            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);

            foreach (AnimatedSprite2D sprite in Sprites)
                sprite.Draw(spriteBatch);

            foreach (HeartsIndicator health in Hearts)
                health.Draw(spriteBatch);

            foreach (HeartsIndicator meat in Meats)
                meat.Draw(spriteBatch);

        }
    }
}
