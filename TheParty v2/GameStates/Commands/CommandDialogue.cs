using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandDialogue : Command<TheParty>
    {
        GUIDialogueBox Box;

        public CommandDialogue(params string[] dialogues)
        {
            Box = new GUIDialogueBox(GUIDialogueBox.Position.Bottom, dialogues);
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Box.Updated(deltaTime, true);

            Done = Box.Done;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            Box.Draw(spriteBatch, true);
        }
    }
}
