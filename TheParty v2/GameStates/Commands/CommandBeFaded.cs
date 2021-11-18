using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandBeFaded : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            client.BeFaded = true;
            Done = true;
        }
    }

    class CommandShowScreen : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            client.BeFaded = false;
            Done = true;
        }
    }
}
