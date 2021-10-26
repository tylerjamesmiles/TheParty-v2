using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandWait : Command<TheParty>
    {
        Timer WaitTimer;

        public CommandWait(float amtTime)
        {
            WaitTimer = new Timer(amtTime);
        }

        public override void Update(TheParty client, float deltaTime)
        {
            WaitTimer.Update(deltaTime);
            if (WaitTimer.TicThisFrame)
                Done = true;
        }
    }
}
