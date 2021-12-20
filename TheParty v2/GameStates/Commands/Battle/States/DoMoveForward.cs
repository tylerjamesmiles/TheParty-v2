using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoMoveForward : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            client.Movement = new LerpV(client.FromSprite.DrawPos, client.InFrontOfTarget, 0.4f);
            client.ReturnToPos = client.FromSprite.DrawPos;
            client.FromSprite.SetCurrentAnimation("Move");
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.Movement.Update(deltaTime);
            client.FromSprite.DrawPos = client.Movement.CurrentPosition;
            if (client.Movement.Reached)
            {
                if (client.CurrentMove.Name == "Hit")
                {
                    bool HittingYou = client.CurrentTargeting.ToPartyIdx == 0;
                    int HitAmt = client.FromMember.StatAmt("Attack") + client.ToMember.StatAmt("Defense");
                    bool FatalHit = (client.ToMember.HP <= HitAmt);
                    int MissChance = HittingYou && FatalHit ? 50 : 10;
                    bool Miss = new Random().Next(100) < MissChance;

                    if (Miss)
                        client.StateMachine.SetNewCurrentState(client, new AnimateMiss());
                    else
                        client.StateMachine.SetNewCurrentState(client, new AnimateHit());
                }
                else
                    client.StateMachine.SetNewCurrentState(client, new DoDoMove());

            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {

        }

        public override void Exit(CommandBattle client)
        {

        }
    }
}
