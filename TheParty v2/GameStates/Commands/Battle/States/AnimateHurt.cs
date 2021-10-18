using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class AnimateHurt : State<CommandBattle>
    {
        StanceIndicator FromStance;
        HeartsIndicator TargetHearts;
        Vector2 HeartsPos;
        LerpV StanceLerp;
        enum State { Windup, Wait, Collide };
        State AnimationState;
        Timer WaitTimer;

        public override void Enter(CommandBattle client)
        {
            FromStance = client.StanceIndicators[client.FromSpriteIdx];
            TargetHearts = client.HPIndicators[client.TargetSpriteIdx];

            HeartsPos = client.Sprites[client.TargetSpriteIdx].DrawPos + new Vector2(0, 16);
            Vector2 WindupSpot = FromStance.DrawPos + ((FromStance.DrawPos - HeartsPos) * 0.3f);
            StanceLerp = new LerpV(FromStance.DrawPos, WindupSpot, 0.5f);

            WaitTimer = new Timer(0.1f);
            AnimationState = State.Windup;  
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            StanceLerp.Update(deltaTime);
            FromStance.DrawPos = StanceLerp.CurrentPosition;

            switch (AnimationState)
            {
                case State.Windup:
                    if (StanceLerp.Reached)
                    {
                        AnimationState = State.Wait;
                    }
                    break;

                case State.Wait:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        StanceLerp = new LerpV(FromStance.DrawPos, HeartsPos, 0.1f);
                        AnimationState = State.Collide;
                    }
                    break;

                case State.Collide:
                    if (StanceLerp.Reached)
                    {
                        client.StateMachine.SetNewCurrentState(client, new DoDoMove());
                    }
                    break;
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);
            FromStance.Draw(spriteBatch);
            TargetHearts.Draw(spriteBatch);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
