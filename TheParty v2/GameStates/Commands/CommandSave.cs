using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Text.Json.Serialization;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;

namespace TheParty_v2
{
    class SaveState
    {
        public string CurrentMap { get; set; }
        public Dictionary<string, int> Variables { get; set; }
        public Dictionary<string, bool> Switches { get; set; }
        public Player Player { get; set; }

    [JsonConstructor]
        public SaveState(string currentMap, Dictionary<string, int> variables, Dictionary<string, bool> switches, Player player)
        {
            CurrentMap = currentMap;
            Variables = variables;
            Switches = switches;
            Player = player;
        }
    }

    class CommandSave : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            SaveState State = new SaveState(
                client.CurrentMap.Name,
                GameContent.Variables,
                GameContent.Switches,
                client.Player);

            string save = JsonConvert.SerializeObject(
                State, Formatting.Indented,
                new Vector2JsonConverter());
            
            File.WriteAllText("SaveFile.json", save);
            Done = true;
        }
    }

    class CommandLoad : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            string save = File.ReadAllText("SaveFile.json");
            SaveState State = JsonConvert.DeserializeObject<SaveState>(
                save, new Vector2JsonConverter());

            client.CurrentMap = GameContent.Maps[State.CurrentMap];
            GameContent.Variables = State.Variables;
            GameContent.Switches = State.Switches;
            client.Player = State.Player;

            Done = true;
        }
    }
}
