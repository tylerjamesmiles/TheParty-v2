using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Victory : State<CommandBattle>
    {
        GUIDialogueBox Msg;

        public Victory()
        {
            string[] VictoryMsgs = new string[]
            {
                "Victory!"
            };
            Msg = new GUIDialogueBox(GUIDialogueBox.Position.SkinnyTop, VictoryMsgs);
        }

        public override void Enter(CommandBattle client)
        {
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            Msg.Update(deltaTime, true);

            if (Msg.Done)
                client.Done = true;
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Msg.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
