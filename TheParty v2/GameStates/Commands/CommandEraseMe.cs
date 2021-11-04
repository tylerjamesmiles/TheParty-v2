﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandEraseMe : Command<TheParty>
    {
        string EventName;

        public CommandEraseMe(string eventName)
        {
            EventName = eventName;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            client.CurrentMap.Entities.Find(e => e.values["Name"] == EventName).ManualExists = false;
            Done = true;
        }
    }
}
