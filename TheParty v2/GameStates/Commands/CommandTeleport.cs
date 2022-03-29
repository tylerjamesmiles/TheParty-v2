using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandTeleport : Command<TheParty>
    {
        string NewMap;
        Vector2 NewPlayerPos;

        public CommandTeleport(string newMap, int newX, int newY)
        {
            NewMap = newMap;
            NewPlayerPos = new Vector2(newX * 16 + 8, newY * 16 + 8);
        }

        public override void Update(TheParty client, float deltaTime)
        {
            OgmoTileMap Map = GameContent.Maps[NewMap];
            Map.SpawnRandomEncounters();
            client.CurrentMap = Map;
            client.Player.Transform.Position = NewPlayerPos;
            client.Player.Movement.Stop();
            client.Player.InitializeFieldMembers();
            client.EventsCanHappenTimer.Reset();
            GameContent.PlaySong(client.CurrentMap.values["Song"]);

            

            Done = true;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            // to accomodate fades
            spriteBatch.Draw(
                GameContent.Sprites["FadeIn"],
                new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                Color.White);
        }
    }
}
