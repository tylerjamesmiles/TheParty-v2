using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class AnimateHitOld : State<CommandBattle>
    {
        StanceIndicator FromStance;
        StanceIndicator TargetStance;
        StanceIndicator ResultStance;
        HeartsIndicator TargetHearts;
        LerpV FromLerp;
        LerpV TargetLerp;
        Timer WaitTimer;
        LerpV ResultLerp;
        Vector2 FromStartPos;
        Vector2 TargetStartPos;
        Vector2 PlusSignPos;
        Vector2 TargetHeartsPos;
        Vector2 WindupSpot;

        enum State { Windup, Wait, Collide, ShowResult, ResultWindup, ResultCollide };
        State AnimationState;

        public override void Enter(CommandBattle client)
        {
            FromStance = client.StanceIndicators[client.FromSpriteIdx];
            TargetStance = client.StanceIndicators[client.TargetSpriteIdx];

            FromStartPos = FromStance.DrawPos;
            TargetStartPos = TargetStance.DrawPos;

            PlusSignPos = (FromStartPos + TargetStartPos) / 2;

            int NewStance = client.FromMember.Stance + client.ToMember.Stance;
            if (NewStance > 10)
                NewStance = 10;

            ResultStance = new StanceIndicator(0, PlusSignPos);
            ResultStance.SetStance(NewStance);

            bool FromFacingLeft = client.Sprites[client.FromSpriteIdx].Flip;
            bool TargetFacingLeft = client.Sprites[client.TargetSpriteIdx].Flip;
            float MoveBy = 30f;
            float FromMoveX = (FromFacingLeft) ? -MoveBy : MoveBy;
            float TargetMoveX = (TargetFacingLeft) ? -MoveBy : MoveBy;
            float TravelTime = 0.5f;
            FromLerp = new LerpV(FromStance.DrawPos, FromStance.DrawPos + new Vector2(FromMoveX, 0), TravelTime);
            TargetLerp = new LerpV(TargetStance.DrawPos, TargetStance.DrawPos + new Vector2(TargetMoveX, 0), TravelTime);
            AnimationState = State.Windup;

            TargetHearts = client.HPIndicators[client.TargetSpriteIdx];
            TargetHeartsPos = client.Sprites[client.TargetSpriteIdx].DrawPos + new Vector2(0, 16);
            WindupSpot = PlusSignPos + ((PlusSignPos - TargetHeartsPos) * 0.3f);

            WaitTimer = new Timer(0.3f);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            FromLerp.Update(deltaTime);
            TargetLerp.Update(deltaTime);
            FromStance.DrawPos = FromLerp.CurrentPosition;
            TargetStance.DrawPos = TargetLerp.CurrentPosition;

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
                        FromLerp = new LerpV(FromStance.DrawPos, PlusSignPos, 0.1f);
                        TargetLerp = new LerpV(TargetStance.DrawPos, PlusSignPos, 0.1f);
                        AnimationState = State.Collide;
                    }
                    break;

                case State.Collide:
                    if (FromLerp.Reached && TargetLerp.Reached)
                    {
                        FromStance.DrawPos = FromStartPos;
                        TargetStance.DrawPos = TargetStartPos;
                        AnimationState = State.ShowResult;
                    }
                    break;
                case State.ShowResult:
                    ResultStance.Update(deltaTime);
                    if (ResultStance.Reached)
                    {
                        ResultLerp = new LerpV(ResultStance.DrawPos, WindupSpot, 1f);
                        AnimationState = State.ResultWindup;
                    }

                    break;

                case State.ResultWindup:
                    ResultLerp.Update(deltaTime);
                    ResultStance.DrawPos = ResultLerp.CurrentPosition;
                    if (ResultLerp.Reached)
                    {
                        ResultLerp = new LerpV(ResultStance.DrawPos, TargetHeartsPos, 0.1f);
                        AnimationState = State.ResultCollide;
                    }

                    break;

                case State.ResultCollide:
                    ResultLerp.Update(deltaTime);
                    ResultStance.DrawPos = ResultLerp.CurrentPosition;
                    if (ResultLerp.Reached)
                    {
                        client.StateMachine.SetNewCurrentState(client, new DoDoMove());
                    }

                    break;
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);

            if (AnimationState == State.Windup ||
                AnimationState == State.Wait ||
                AnimationState == State.Collide)
            {
                FromStance.Draw(spriteBatch);
                TargetStance.Draw(spriteBatch);
                spriteBatch.Draw(GameContent.Sprites["PlusSign"], new Rectangle(PlusSignPos.ToPoint(), new Point(6, 6)), Color.White);

            }
            else if (AnimationState == State.ShowResult ||
                AnimationState == State.ResultWindup ||
                AnimationState == State.ResultCollide)
            {
                ResultStance.Draw(spriteBatch);
                TargetHearts.Draw(spriteBatch);
            }

        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
