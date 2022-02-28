using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheParty_v2
{
    class CommandGaslight : Command<TheParty>
    {
        StateMachine<CommandGaslight> StateMachine;

        public override void Enter(TheParty client)
        {
            StateMachine = new StateMachine<CommandGaslight>();
            StateMachine.SetNewCurrentState(this, new GaslightGame());
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            StateMachine.Update(this, deltaTime);
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
            
        }
    }
}
