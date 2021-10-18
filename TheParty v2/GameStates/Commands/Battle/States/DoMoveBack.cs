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
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.Movement.Update(deltaTime);
            client.FromSprite.DrawPos = client.Movement.CurrentPosition;
            if (client.Movement.Reached)
            {
                client.StateMachine.SetNewCurrentState(client, new DoStatusEffect());
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
