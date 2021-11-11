using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandLeaveDead : Command<TheParty>
    {
        CommandQueue<TheParty> Commands;

        public override void Enter(TheParty client)
        {
            Commands = new CommandQueue<TheParty>();
            foreach (Member member in client.Player.ActiveParty.Members)
                if (member.HP == 0)
                {
                    Commands.EnqueueCommand(new CommandDialogue(member.Name + " is gone."));
                    Commands.EnqueueCommand(new CommandDialogue("The party mourns, buries the body and moves on."));
                    Commands.EnqueueCommand(new CommandDialogue("There is nothing more that anyone can do."));
                    Commands.EnqueueCommand(new CommandRemovePartyMember(member.Name));
                }
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Commands.Update(client, deltaTime);
            if (Commands.Empty)
                Done = true;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);
            Commands.Draw(client, spriteBatch);
        }
    }
}
