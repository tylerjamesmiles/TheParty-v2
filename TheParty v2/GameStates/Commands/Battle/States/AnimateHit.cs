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
        string FromBonusText;
        LerpV FromStanceLerp;
        LerpV BonusLerp;
        LerpV TargetStanceLerp;
        Timer WaitTimer;
        LerpV ResultLerp;
        Vector2 FromBonusPos;
        Vector2 FromStartPos;
        Vector2 TargetStartPos;
        Vector2 BonusPos;
        Vector2 TargetHeartsPos;
        Vector2 WindupSpot;

        enum State { Show, DoubleCollide, Double, Windup, Wait, Collide, ShowResult, ResultWindup, ResultCollide };
        State AnimationState;

        public override void Enter(CommandBattle client)
        {
            FromStance = new StanceIndicator(client.FromMember.Stance, client.StanceIndicators[client.FromSpriteIdx].DrawPos);
            int NewAttackAmt = client.FromMember.StanceWAttackBonus;
            FromStance.HardSet(client.FromMember.Stance);
            FromStance.SetTarget(NewAttackAmt);  // sets target

            TargetStance = client.StanceIndicators[client.TargetSpriteIdx];

            FromStartPos = FromStance.DrawPos;
            TargetStartPos = TargetStance.DrawPos;

            FromBonusText = "+" + client.FromMember.Equipped.Bonus("Attack").ToString();
            FromBonusPos = FromStartPos + new Vector2(-8, -16);
            BonusLerp = new LerpV(FromBonusPos, FromStartPos, 0.1f);

            BonusPos = (FromStartPos + TargetStartPos) / 2;

            int NewStance = NewAttackAmt + client.ToMember.Stance;
            if (NewStance > 10)
                NewStance = 10;

            ResultStance = new StanceIndicator(0, BonusPos);
            ResultStance.HardSet(NewStance);

            

            TargetHearts = client.HPIndicators[client.TargetSpriteIdx];
            TargetHeartsPos = client.Sprites[client.TargetSpriteIdx].DrawPos + new Vector2(0, 16);
            WindupSpot = BonusPos + ((BonusPos - TargetHeartsPos) * 0.3f);

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
                    BonusLerp.Update(deltaTime);
                    FromBonusPos = BonusLerp.CurrentPosition;
                    if (BonusLerp.Reached)
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
                        FromStanceLerp = new LerpV(FromStance.DrawPos, BonusPos, 0.1f);
                        TargetStanceLerp = new LerpV(TargetStance.DrawPos, BonusPos, 0.1f);
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
                spriteBatch.DrawString(
                    GameContent.Font,
                    FromBonusText,
                    FromBonusPos,
                    Color.White);
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
                spriteBatch.Draw(GameContent.Sprites["PlusSign"], new Rectangle(BonusPos.ToPoint(), new Point(6, 6)), Color.White);
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
