using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class AnimateHit : State<CommandBattle>
    {
        StanceIndicator FakeFrom;
        StanceIndicator FakeTarget;
        LerpV FromLerp;
        LerpV TargetLerp;
        Timer WaitTimer;
        Vector2 FromStartPos;
        Vector2 TargetStartPos;
        Vector2 PlusSignPos;
        enum State { Windup, Wait, Collide };
        State AnimationState;

        public override void Enter(CommandBattle client)
        {
            FakeFrom = client.StanceIndicators[client.FromSpriteIdx];
            FakeTarget = client.StanceIndicators[client.TargetSpriteIdx];

            FromStartPos = FakeFrom.DrawPos;
            TargetStartPos = FakeTarget.DrawPos;

            PlusSignPos = (FromStartPos + TargetStartPos) / 2;

            bool FromFacingLeft = client.Sprites[client.FromSpriteIdx].Flip;
            bool TargetFacingLeft = client.Sprites[client.TargetSpriteIdx].Flip;
            float MoveBy = 30f;
            float FromMoveX = (FromFacingLeft) ? -MoveBy : MoveBy;
            float TargetMoveX = (TargetFacingLeft) ? -MoveBy : MoveBy;
            float TravelTime = 0.5f;
            FromLerp = new LerpV(FakeFrom.DrawPos, FakeFrom.DrawPos + new Vector2(FromMoveX, 0), TravelTime);
            TargetLerp = new LerpV(FakeTarget.DrawPos, FakeTarget.DrawPos + new Vector2(TargetMoveX, 0), TravelTime);
            AnimationState = State.Windup;

            WaitTimer = new Timer(0.3f);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            FromLerp.Update(deltaTime);
            TargetLerp.Update(deltaTime);
            FakeFrom.DrawPos = FromLerp.CurrentPosition;
            FakeTarget.DrawPos = TargetLerp.CurrentPosition;

            switch (AnimationState)
            {
                case State.Windup:
                    if (FromLerp.Reached && TargetLerp.Reached)
                    {

                        AnimationState = State.Wait;
                    }
                    break;

                case State.Wait:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicsSoFar > 1)
                    {
                        FromLerp = new LerpV(FakeFrom.DrawPos, PlusSignPos, 0.1f);
                        TargetLerp = new LerpV(FakeTarget.DrawPos, PlusSignPos, 0.1f);
                        AnimationState = State.Collide;
                    }
                    break;

                case State.Collide:
                    if (FromLerp.Reached && TargetLerp.Reached)
                    {
                        FakeFrom.DrawPos = FromStartPos;
                        FakeTarget.DrawPos = TargetStartPos;
                        client.StateMachine.SetNewCurrentState(client, new DoDoMove());
                    }
                    break;
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);
            spriteBatch.Draw(GameContent.Sprites["PlusSign"], new Rectangle(PlusSignPos.ToPoint(), new Point(6, 6)), Color.White);
            FakeFrom.Draw(spriteBatch);
            FakeTarget.Draw(spriteBatch);

        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
