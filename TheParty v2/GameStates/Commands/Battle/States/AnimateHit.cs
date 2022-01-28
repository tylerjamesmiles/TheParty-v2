using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class AnimateHit : State<CommandBattle>
    {
        StanceIndicator FromStance;
        StanceIndicator ToStance;
        StanceIndicator AttackStance;

        Vector2 CombineSpot;

        Timer WaitTimer;
        enum State { Combine, AddBonus, Hit };
        State CurrentState;

        public override void Enter(CommandBattle client)
        {
            FromStance = client.StanceIndicators[client.FromSpriteIdx];
            ToStance = client.StanceIndicators[client.TargetSpriteIdx];

            Vector2 FromSpritePos = client.FromSprite.DrawPos;
            Vector2 ToSpritePos = client.ToSprite.DrawPos;
            Vector2 InBetween = FromSpritePos + (ToSpritePos - FromSpritePos) / 2;
            CombineSpot = InBetween + new Vector2(0, -40);
            FromStance.SetAllTarget(CombineSpot);
            ToStance.SetAllTarget(CombineSpot);
            FromStance.ToggleFastMode(true);
            ToStance.ToggleFastMode(true);

            WaitTimer = new Timer(1f);
            CurrentState = State.Combine;
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            switch (CurrentState)
            {
                case State.Combine:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        AttackStance = new StanceIndicator(CombineSpot, FromStance, ToStance);

                        WaitTimer = new Timer(0.2f);
                        CurrentState = State.Hit;
                    }


                    break;

                case State.AddBonus:

                    break;

                case State.Hit:
                    AttackStance.Update(deltaTime);
                    WaitTimer.Update(deltaTime);

                    if (WaitTimer.TicThisFrame)
                    {
                        Vector2 TargetPos = client.HPIndicators[client.TargetSpriteIdx].Origin.ToVector2();
                        AttackStance.SetOneTarget(TargetPos);
                    }

                    if (AttackStance.ParticleHitThisFrame)
                    {
                        client.HPIndicators[client.TargetSpriteIdx].RemoveHeart();
                    }

                    if (AttackStance.AllParticlesHit)
                    {
                        client.StateMachine.SetNewCurrentState(client, new DoDoMove());

                    }
                    break;
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites["Black"],
                new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                Color.White);

            client.HPIndicators[client.TargetSpriteIdx].Draw(spriteBatch);

            switch (CurrentState)
            {
                case State.Combine:
                    FromStance.Draw(spriteBatch);
                    ToStance.Draw(spriteBatch);
                    break;

                case State.Hit:
                    AttackStance.Draw(spriteBatch);

                    DrawPrimitives2D.Circle(client.HPIndicators[client.TargetSpriteIdx].Origin, 2, Color.Red, spriteBatch);
                    break;
            }
        }

        public override void Exit(CommandBattle client)
        {
           
        }
    }
}
