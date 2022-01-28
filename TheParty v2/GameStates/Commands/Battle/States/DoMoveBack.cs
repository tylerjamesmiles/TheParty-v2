using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoMoveBack : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            client.FromSprite.SetCurrentAnimation("Move");
            client.Movement = new LerpV(client.FromSprite.DrawPos, client.ReturnToPos, 0.4f);

            client.StanceIndicators[client.FromSpriteIdx].SetAllTarget(client.ReturnToPos + new Vector2(0, -20));
            client.StanceIndicators[client.TargetSpriteIdx].SetAllTarget(client.ToSprite.DrawPos + new Vector2(0, -20));

            client.StanceIndicators[client.FromSpriteIdx].ToggleFastMode(false);
            client.StanceIndicators[client.TargetSpriteIdx].ToggleFastMode(false);

        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.Movement.Update(deltaTime);
            client.FromSprite.DrawPos = client.Movement.CurrentPosition;
            if (client.Movement.Reached)
            {
                client.StateMachine.SetNewCurrentState(client, new PostMoveCheck());
            }
        }
    }
}
