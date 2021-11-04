using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace TheParty_v2.GameStates
{
    class SaveState
    {
        public string CurrentMap;
        public Player Player;
        public Dictionary<string, int> Variables;
        public Dictionary<string, bool> Switches;
    }

    class CommandSave : Command<TheParty>
    {

        public override void Update(TheParty client, float deltaTime)
        {
            SaveState State = new SaveState()
            {
                CurrentMap = client.CurrentMap.Name,
                Player = client.Player,
                Variables = GameContent.Variables,
                Switches = GameContent.Switches
            };

            
        }
    }
}
