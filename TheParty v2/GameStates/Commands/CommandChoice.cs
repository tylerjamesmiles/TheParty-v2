using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandChoice : Command<TheParty>
    {
        GUIChoiceBox Choice;
        List<Command<TheParty>>[] Outcomes;

        public CommandChoice(string[] choices, List<Command<TheParty>>[] outcomes)
        {
            Choice = new GUIChoiceBox(choices, GUIChoiceBox.Position.Center);
            Outcomes = outcomes;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Choice.Update(deltaTime, true);

            if (Choice.Done)
            {
                client.CommandQueue.AddCommands(Outcomes[Choice.CurrentChoice]);
                Done = true;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            Choice.Draw(spriteBatch, true);
        }
    }
}
