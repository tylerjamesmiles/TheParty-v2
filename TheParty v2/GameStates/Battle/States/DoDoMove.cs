using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoDoMove : State<CommandBattle>
    {
        Timer TempWait;
        bool MoveDone;

        public override void Enter(CommandBattle client)
        {
            client.FromSprite.SetCurrentAnimation("Idle");
            TempWait = new Timer(0.8f);
            MoveDone = false;
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            if (!MoveDone)
            {
                client.CurrentStore = Move.WithEffectDone(client.CurrentStore, client.CurrentMove, client.CurrentTargeting);
                client.CurrentStore = BattleStore.TimePassed(client.CurrentStore);
                MoveDone = true;
            }


            TempWait.Update(deltaTime);
            if (TempWait.TicThisFrame)
                client.StateMachine.SetNewCurrentState(client, new DoMoveBack());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
