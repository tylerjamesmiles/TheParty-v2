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
        StanceIndicator TargetStance;
        StanceIndicator ResultStance;
        HeartsIndicator TargetHearts;
        LerpV FromStanceLerp;
        LerpV From2XLerp;
        LerpV TargetStanceLerp;
        Timer WaitTimer;
        LerpV ResultLerp;
        Vector2 From2xPos;
        Vector2 FromStartPos;
        Vector2 TargetStartPos;
        Vector2 PlusSignPos;
        Vector2 TargetHeartsPos;
        Vector2 WindupSpot;

        enum State { Show, DoubleCollide, Double, Windup, Wait, Collide, ShowResult, ResultWindup, ResultCollide };
        State AnimationState;

        public override void Enter(CommandBattle client)
        {
            FromStance = new StanceIndicator(client.FromMember.Stance, client.StanceIndicators[client.FromSpriteIdx].DrawPos);
            int Doubled = client.FromMember.Stance * 2;
            FromStance.HardSet(client.FromMember.Stance);
            FromStance.SetTarget(Doubled);  // sets target

            TargetStance = client.StanceIndicators[client.TargetSpriteIdx];

            FromStartPos = FromStance.DrawPos;
            TargetStartPos = TargetStance.DrawPos;

            From2xPos = FromStartPos + new Vector2(-8, -16);
            From2XLerp = new LerpV(From2xPos, FromStartPos, 0.1f);

            PlusSignPos = (FromStartPos + TargetStartPos) / 2;

            int NewStance = Doubled + client.ToMember.Stance;
            if (NewStance > 10)
                NewStance = 10;

            ResultStance = new StanceIndicator(0, PlusSignPos);
            ResultStance.HardSet(NewStance);

            

            TargetHearts = client.HPIndicators[client.TargetSpriteIdx];
            TargetHeartsPos = client.Sprites[client.TargetSpriteIdx].DrawPos + new Vector2(0, 16);
            WindupSpot = PlusSignPos + ((PlusSignPos - TargetHeartsPos) * 0.3f);

            WaitTimer = new Timer(0.3f);

            AnimationState = State.Show;
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            switch (AnimationState)
            {
                case State.Show:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                    {
                        AnimationState = State.DoubleCollide;
                    }
                    break;

                case State.DoubleCollide:
                    From2XLerp.Update(deltaTime);
                    From2xPos = From2XLerp.CurrentPosition;
                    if (From2XLerp.Reached)
                    {
                        AnimationState = State.Double;
                    }
                    break;

                case State.Double:
                    FromStance.Update(deltaTime);
                    if (FromStance.Reached)
                    {
                        bool FromFacingLeft = client.Sprites[client.FromSpriteIdx].Flip;
                        bool TargetFacingLeft = client.Sprites[client.TargetSpriteIdx].Flip;
                        float MoveBy = 30f;
                        float FromMoveX = (FromFacingLeft) ? -MoveBy : MoveBy;
                        float TargetMoveX = (TargetFacingLeft) ? -MoveBy : MoveBy;
                        float TravelTime = 0.5f;
                        FromStanceLerp = new LerpV(FromStance.DrawPos, FromStance.DrawPos + new Vector2(FromMoveX, 0), TravelTime);
                        TargetStanceLerp = new LerpV(TargetStance.DrawPos, TargetStance.DrawPos + new Vector2(TargetMoveX, 0), TravelTime);
                        AnimationState = State.Windup;
                    }

                    break;

                case State.Windup:
                    FromStanceLerp.Update(deltaTime);
                    TargetStanceLerp.Update(deltaTime);
                    FromStance.DrawPos = FromStanceLerp.CurrentPosition;
                    TargetStance.DrawPos = TargetStanceLerp.CurrentPosition;

                    if (FromStanceLerp.Reached && TargetStanceLerp.Reached)
                    {
                        WaitTimer = new Timer(0.1f);
                        AnimationState = State.Wait;
                    }
                    break;

                case State.Wait:
                    WaitTimer.Update(deltaTime);
                    FromStance.DrawPos = FromStanceLerp.CurrentPosition;
                    TargetStance.DrawPos = TargetStanceLerp.CurrentPosition;

                    if (WaitTimer.TicsSoFar > 1)
                    {
                        FromStanceLerp = new LerpV(FromStance.DrawPos, PlusSignPos, 0.1f);
                        TargetStanceLerp = new LerpV(TargetStance.DrawPos, PlusSignPos, 0.1f);
                        AnimationState = State.Collide;
                    }
                    break;

                case State.Collide:
                    FromStanceLerp.Update(deltaTime);
                    TargetStanceLerp.Update(deltaTime);
                    FromStance.DrawPos = FromStanceLerp.CurrentPosition;
                    TargetStance.DrawPos = TargetStanceLerp.CurrentPosition;

                    if (FromStanceLerp.Reached && TargetStanceLerp.Reached)
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

            if (AnimationState == State.Show ||
                AnimationState == State.DoubleCollide)
            {
                spriteBatch.Draw(GameContent.Sprites["2x"], new Rectangle(From2xPos.ToPoint(), new Point(16, 16)), Color.White);
            }

            if (AnimationState == State.Show ||
                AnimationState == State.DoubleCollide ||
                AnimationState == State.Double ||
                AnimationState == State.Windup ||
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
            FromStance.SetTarget(client.FromMember.Stance);
        }
    }
}
