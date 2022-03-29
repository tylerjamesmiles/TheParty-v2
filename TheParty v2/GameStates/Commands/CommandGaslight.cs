using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheParty_v2
{
    class CommandGaslight : Command<TheParty>
    {
        List<GaslightEntity> Entities;
        GaslightMap Map;
        Camera2D Camera;

        public GaslightEntity Player;
        public GaslightEntity Stairs;
        public List<GaslightEntity> Fires;
        public List<GaslightEntity> Oils;
        public List<GaslightEntity> Coins;
        public List<GaslightEntity> Spiders;
        public List<GaslightEntity> Refills;
        public List<GaslightEntity> Health;

        public List<Rectangle> CollisionBoxes => Map.Collision;
        public bool IsSpaceAvailable(Vector2 pos) => Map.IsSpaceAvailable(pos);
        public bool CanSeeBetweenSpaces(Point pos1, Point pos2) => Map.CanSeeBetweenPoints(pos1, pos2);
        int Level;

        public void GenerateNewMap(int hp = 5, int gold = 0, int oil = 0)
        {
            Level++;

            Point MapSize = new Point(16 + Level, 16 + Level);

            Map = new GaslightMap(MapSize);
            Map.Generate();

            Entities = new List<GaslightEntity>();
            Entities.Add(new GaslightEntityPlayer(Map.RandomOpenSpace(), oil, hp, gold));
            Entities.Add(new GaslightEntityStairs(Map.RandomOpenSpace()));

            Random Rand = new Random();

            int NumSpiders = Level / 2 - 1;
            int NumCoins = 1 + Level / 2;
            int NumRefills = Level / 2;

            for (int i = 0; i < NumSpiders; i++)
                Entities.Add(new GaslightEntitySpider(Map.RandomOpenSpace()));

            for (int i = 0; i < NumCoins; i++)
                Entities.Add(new GaslightEntityCoin(Map.RandomOpenSpace()));

            for (int i = 0; i < NumRefills; i++)
                Entities.Add(new GaslightEntityRefill(Map.RandomOpenSpace()));


            Camera = new Camera2D();
        }

        public override void Enter(TheParty client)
        {
            Level = 0;

            GenerateNewMap();

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            // Only do this search once
            Player = Entities.Find(e => e.EntityType == GaslightEntity.Type.Player);
            Stairs = Entities.Find(e => e.EntityType == GaslightEntity.Type.Stairs);
            Fires = Entities.FindAll(e => e.EntityType == GaslightEntity.Type.Fire);
            Oils = Entities.FindAll(e => e.EntityType == GaslightEntity.Type.Oil);
            Coins = Entities.FindAll(e => e.EntityType == GaslightEntity.Type.Coin);
            Spiders = Entities.FindAll(e => e.EntityType == GaslightEntity.Type.Spider);
            Refills = Entities.FindAll(e => e.EntityType == GaslightEntity.Type.Refill);
            Health = Entities.FindAll(e => e.EntityType == GaslightEntity.Type.Health);

            Map.ClearVisible();
            Fires.ForEach(f => Map.SetVisible(f));
            Map.SetVisible(Player);
            Map.SetHalfVisible();

            List<GaslightEntity> NewEntities = new List<GaslightEntity>();
            Entities.ForEach(e => NewEntities.AddRange(e.AddEntities(this, Camera.Position)));
            Entities.AddRange(NewEntities);

            Entities.ForEach(e => e.AI(this, deltaTime));
            Entities.ForEach(e => e.Update(this, deltaTime));

            if (Player.IsDead)
            {
                client.CommandQueue.EnqueueCommand(new CommandFadeOutMusic());
                client.CommandQueue.EnqueueCommand(new CommandGameOver());
                Done = true;
            }

            Entities.RemoveAll(e => e.IsDead);

            Camera.Update(Map.MapSizePixels.ToPoint(), Player.Position);

            if (InputManager.JustReleased(Keys.R))
                Enter(client);
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            if (!Entered)
                return;

            Map.Draw(spriteBatch, Camera.Position);

            var NonOils = Entities.FindAll(e => e.EntityType != GaslightEntity.Type.Oil);

            // Draw Oils first
            Oils.ForEach(e => e.Draw(spriteBatch, Camera.Position));

            // Draw others
            NonOils.Sort((e1, e2) => e1.Position.Y >= e2.Position.Y ? 1 : -1);

            NonOils.ForEach(e => e.Draw(spriteBatch, Camera.Position));

            //// Num fires
            //spriteBatch.DrawString(
            //    GameContent.Font,
            //    "Fires: " + Fires.Count.ToString(),
            //    new Vector2(0, 20),
            //    Color.White);

            //// Num oils
            //spriteBatch.DrawString(
            //    GameContent.Font,
            //    "Oils: " + Oils.Count.ToString(),
            //    new Vector2(0, 30),
            //    Color.White);
        }

        public override void Exit(TheParty client)
        {
            
        }
    }
}
