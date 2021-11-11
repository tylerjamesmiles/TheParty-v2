using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{

    class Whatever
    {
        int x = 4;
        int y = 5;
    }

    class CommandSave : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            string save = JsonSerializer.Serialize(client.Player);
            File.WriteAllText("SaveFile.json", save);
            Done = true;
        }
    }

    class CommandLoad : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            string save = File.ReadAllText("SaveFile.json");
            client.Player = JsonSerializer.Deserialize<Player>(save);
            Done = true;
        }
    }
}
