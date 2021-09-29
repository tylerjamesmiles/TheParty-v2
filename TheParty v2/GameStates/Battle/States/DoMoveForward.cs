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
                client.StateMachine.SetNewCurrentState(client, new DoDoMove());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {

        }

        public override void Exit(CommandBattle client)
        {

        }
    }
}
