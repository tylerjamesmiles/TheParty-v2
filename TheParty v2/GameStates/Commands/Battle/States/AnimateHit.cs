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
        bool ShowFromBonus;
        bool ShowToBonus;
        string FromBonusText;
        string ToBonusText;
        LerpV FromStanceLerp;
        LerpV FromBonusLerp;
        LerpV ToStanceLerp;
        LerpV ToBonusLerp;
        Timer WaitTimer;
        LerpV ResultLerp;
        Vector2 FromBonusPos;
        Vector2 ToBonusPos;
        Vector2 PlusSignPos;
        Vector2 FromStartPos;
        Vector2 TargetStartPos;
        Vector2 TargetHeartsPos;
        Vector2 WindupSpot;

        enum State { Show, BonusCollide, AddBonus, Windup, Wait, Collide, ShowResult, ResultWindup, ResultCollide };
        State AnimationState;

        public override void Enter(CommandBattle client)
        {
            FromStance = new StanceIndicator(client.FromMember.Stance, client.StanceIndicators[client.FromSpriteIdx].DrawPos);
            int NewAttackAmt = client.FromMember.StatAmt("Attack");
            int NewDefenseAmt = client.ToMember.StatAmt("Defense");

            ShowFromBonus = NewAttackAmt != client.FromMember.Stance;
            ShowToBonus = NewDefenseAmt != client.ToMember.Stance;

            FromStance.HardSet(client.FromMember.Stance);
            FromStance.SetTarget(NewAttackAmt);  // sets target

            TargetStance = new StanceIndicator(client.ToMember.Stance, client.StanceIndicators[client.TargetSpriteIdx].DrawPos);
            TargetStance.HardSet(client.ToMember.Stance);
            TargetStance.SetTarget(NewDefenseAmt);

            FromStartPos = FromStance.DrawPos;
            TargetStartPos = TargetStance.DrawPos;

            bool TargetFacingRight = client.CurrentTargeting.FromPartyIdx == 0;
            int FromBonusXOffset = TargetFacingRight ? +10 : -10;
            int ToBonusXOffset = TargetFacingRight ? -10 : 10;

            int FromBonusAmt = client.FromMember.StatBonus("Attack");
            FromBonusText = "(+" + FromBonusAmt;
            FromBonusPos = FromStartPos + new Vector2(FromBonusXOffset, -20);
            FromBonusLerp = new LerpV(FromBonusPos, FromStartPos, 0.1f);

            int ToBonusAmt = client.ToMember.StatBonus("Defense");
            ToBonusText = ")" + ToBonusAmt.ToString();
            ToBonusPos = TargetStartPos + new Vector2(ToBonusXOffset, -20);
            ToBonusLerp = new LerpV(ToBonusPos, TargetStartPos, 0.1f);

            PlusSignPos = (FromStartPos + TargetStartPos) / 2;

            int NewStance = NewAttackAmt + NewDefenseAmt;
            if (NewStance > 10)
                NewStance = 10;

            ResultStance = new StanceIndicator(0, PlusSignPos);
            ResultStance.HardSet(NewStance);

            

            TargetHearts = client.HPIndicators[client.TargetSpriteIdx];
            TargetHeartsPos = client.Sprites[client.TargetSpriteIdx].DrawPos + new Vector2(0, 16);
            WindupSpot = PlusSignPos + ((PlusSignPos - TargetHeartsPos) * 0.3f);

            WaitTimer = new Timer(0.4f);

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
                        AnimationState = State.BonusCollide;
                    }
                    break;

                case State.BonusCollide:
                    FromBonusLerp.Update(deltaTime);
                    ToBonusLerp.Update(deltaTime);
                    FromBonusPos = FromBonusLerp.CurrentPosition;
                    ToBonusPos = ToBonusLerp.CurrentPosition;
                    if (FromBonusLerp.Reached)
                    {
                        AnimationState = State.AddBonus;
                    }
                    break;

                case State.AddBonus:
                    FromStance.Update(deltaTime);
                    TargetStance.Update(deltaTime);
                    if (FromStance.Reached && TargetStance.Reached)
                    {
                        bool FromFacingLeft = client.Sprites[client.FromSpriteIdx].Flip;
                        bool TargetFacingLeft = client.Sprites[client.TargetSpriteIdx].Flip;
                        float MoveBy = 30f;
                        float FromMoveX = (FromFacingLeft) ? -MoveBy : MoveBy;
                        float TargetMoveX = (TargetFacingLeft) ? -MoveBy : MoveBy;
                        float TravelTime = 0.3f;
                        FromStanceLerp = new LerpV(FromStance.DrawPos, FromStance.DrawPos + new Vector2(FromMoveX, 0), TravelTime);
                        ToStanceLerp = new LerpV(TargetStance.DrawPos, TargetStance.DrawPos + new Vector2(TargetMoveX, 0), TravelTime);
                        AnimationState = State.Windup;
                    }

                    break;

                case State.Windup:
                    FromStanceLerp.Update(deltaTime);
                    ToStanceLerp.Update(deltaTime);
                    FromStance.DrawPos = FromStanceLerp.CurrentPosition;
                    TargetStance.DrawPos = ToStanceLerp.CurrentPosition;

                    if (FromStanceLerp.Reached && ToStanceLerp.Reached)
                    {
                        WaitTimer = new Timer(0.1f);
                        AnimationState = State.Wait;
                    }
                    break;

                case State.Wait:
                    WaitTimer.Update(deltaTime);
                    FromStance.DrawPos = FromStanceLerp.CurrentPosition;
                    TargetStance.DrawPos = ToStanceLerp.CurrentPosition;

                    if (WaitTimer.TicsSoFar > 1)
                    {
                        FromStanceLerp = new LerpV(FromStance.DrawPos, FromBonusPos, 0.1f);
                        ToStanceLerp = new LerpV(TargetStance.DrawPos, FromBonusPos, 0.1f);
                        AnimationState = State.Collide;
                    }
                    break;

                case State.Collide:
                    FromStanceLerp.Update(deltaTime);
                    ToStanceLerp.Update(deltaTime);
                    FromStance.DrawPos = FromStanceLerp.CurrentPosition;
                    TargetStance.DrawPos = ToStanceLerp.CurrentPosition;

                    if (FromStanceLerp.Reached && ToStanceLerp.Reached)
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
            spriteBatch.Draw(
                GameContent.Sprites["Black"], 
                new Rectangle(new Point(0, 0), 
                new Point(160, 144)), 
                Color.White);

            if (AnimationState == State.Show ||
                AnimationState == State.BonusCollide)
            {
                if (ShowFromBonus)
                    spriteBatch.DrawString(
                        GameContent.Font,
                        FromBonusText,
                        FromBonusPos.ToPoint().ToVector2(),
                        Color.White);

                if (ShowToBonus)
                    spriteBatch.DrawString(
                        GameContent.Font,
                        ToBonusText,
                        ToBonusPos.ToPoint().ToVector2(),
                        Color.White);
            }

            if (AnimationState == State.Show ||
                AnimationState == State.BonusCollide ||
                AnimationState == State.AddBonus ||
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
