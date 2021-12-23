using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoMoveForward : State<CommandBattle>
    {
        static bool PlayerMissedLastTime;

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
                    bool EnemysTurn = client.CurrentTargeting.ToPartyIdx == 0;
                    int HitAmt = client.FromMember.StatAmt("Attack") + client.ToMember.StatAmt("Defense");
                    bool FatalHit = (client.ToMember.HP <= HitAmt);
                    int MissChance = EnemysTurn && FatalHit ? 50 : 8;

                    bool Miss = 
                        !EnemysTurn && PlayerMissedLastTime ? false :
                        new Random().Next(100) < MissChance;

                    // Static bool to prevent player from missing twice in a row
                    if (PlayerMissedLastTime == true)
                        PlayerMissedLastTime = false;   // reset

                    if (Miss && !EnemysTurn)
                        PlayerMissedLastTime = true;

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
