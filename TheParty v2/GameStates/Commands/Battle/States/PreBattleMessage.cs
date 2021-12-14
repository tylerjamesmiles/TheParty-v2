using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class PreBattleMessage : State<CommandBattle>
    {
        GUIDialogueBox Message;

        public PreBattleMessage(string message)
        {
            Message = new GUIDialogueBox(GUIDialogueBox.Position.ReallySkinnyTop, new[] { message });
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            Message.Update(deltaTime, true);

            if (Message.Done)
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Message.Draw(spriteBatch, true);
        }
    }
}
