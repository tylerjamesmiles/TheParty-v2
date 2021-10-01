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
            NewPlayerPos = new Vector2(newX, newY);
        }

        public override void Enter(TheParty client)
        {
        }

        public override void Update(TheParty client, float deltaTime)
        {
            client.CurrentMap = GameContent.Maps[NewMap];
            client.Player.Transform.Position = NewPlayerPos;
            client.Player.Movement.Velocity = new Vector2(0, 0);
            client.EventsCanHappenTimer.Reset();
            Done = true;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
