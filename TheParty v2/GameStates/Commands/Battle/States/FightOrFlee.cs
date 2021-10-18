using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FightOrFlee : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            client.FightOrFlee = new GUIChoiceBox(new[] { "Fight", "Flee" }, GUIChoiceBox.Position.Center);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.FightOrFlee.Update(deltaTime, true);

            if (client.FightOrFlee.Done)
            {
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.FightOrFlee.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
