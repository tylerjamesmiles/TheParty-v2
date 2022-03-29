using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace TheParty_v2
{
    class OgmoTileMap
    {
        public string Name;
        public string ogmoVersion;
        public int width;
        public int height;
        public int offsetX;
        public int offsetY;
        public Dictionary<string, string> values;
        public List<OgmoLayer> layers;

        public List<Rectangle> CollisionBoxes;

        public Point Size => new Point(width, height);
        public OgmoLayer EntityLayer => layers.Find(l => l.entities != null);
        public OgmoLayer CollisionLayer => layers.Find(l => l.grid != null);
        public List<OgmoEntity> Entities => EntityLayer.entities;
        public List<Transform2D> EntityTransforms => 
            Entities.FindAll(e => e.Exists && e.values["Solid"] == "true").ConvertAll(e => e.Transform);

        public void Initialize(string name)
        {
            Name = name;
            CollisionBoxes = GenerateCollisionBoxes();
            layers.ForEach(l => l.Initialize());
        }

        public void SpawnRandomEncounters()
        {
            // Spawn random encounters

            // find appropriate battle
            bool Easy = bool.Parse(values["EasyBattles"]);
            bool Hard = bool.Parse(values["HardBattles"]);
            bool CoinFlip = new Random().Next(2) == 0;
            bool IsEasy =
                Easy && !Hard ? true :
                !Easy && Hard ? false :
                Easy && Hard ? CoinFlip :
                false;

            List<string> AvailableBattles = new List<string>();
            foreach (var battle in GameContent.Battles)
            {
                string[] Keywords = battle.Key.Split(' ');
                if (Keywords[0] != "Day")
                    continue;

                int DayNo = int.Parse(Keywords[1]);
                int DaysPassed = 100 - GameContent.Variables["DaysRemaining"];
                if (DayNo < DaysPassed)
                    continue;

                string Type = Keywords[2];
                if (Type == "Easy" && !IsEasy)
                    continue;
                if (Type == "Hard" && IsEasy)
                    continue;

                AvailableBattles.Add(battle.Key);
            }

            if (AvailableBattles.Count == 0)
                return;

            for (int i = 0; i < int.Parse(values["NumBattles"]); i++)
            {
                // find open position
                int RandX = new Random().Next(width);
                int RandY = new Random().Next(height);
                int TimesTried = 0;
                while (CollisionBoxes.Exists(cb => cb.Contains(RandX, RandY)) && TimesTried < 1000)
                {
                    RandX = new Random().Next(width);
                    RandY = new Random().Next(height);
                    TimesTried++;
                }

                if (TimesTried == 1000)
                    continue;

                Point RandPoint = new Point(RandX, RandY);

                // find available battle
                int RandBattleIdx = new Random().Next(AvailableBattles.Count);
                string RandBattle = AvailableBattles[RandBattleIdx];
                
                // create entity 
                OgmoEntity E = new OgmoEntity();
                E.name = "Monster " + RandBattle;
                E.Initialize();
                E.Transform.Position = RandPoint.ToVector2();

                Entities.Add(E);
            }
        }

        public List<Rectangle> GenerateCollisionBoxes()
        {
            List<Rectangle> Result = new List<Rectangle>();

            if (CollisionLayer.grid == null)
                return Result;

            Point TileSize = new Point(16, 16);
            int MapTileWidth = CollisionLayer.gridCellsX;
            for (int i = 0; i < CollisionLayer.grid.Length; i++)
            {
                if (CollisionLayer.grid[i] != "1")
                    continue;

                int PosX = i % MapTileWidth;
                int PosY = i / MapTileWidth;
                Point Pos = new Point(PosX, PosY) * TileSize;
                Point Size = TileSize;
                Result.Add(new Rectangle(Pos, Size));
            }

            // outside rects
            Point TopPos = new Point(0, -TileSize.Y);
            Point TopSize = new Point(width, TileSize.Y);
            Result.Add(new Rectangle(TopPos, TopSize));

            Point BottomPos = new Point(0, height);
            Point BottomSize = new Point(width, TileSize.Y);
            Result.Add(new Rectangle(BottomPos, BottomSize));

            Point LeftPos = new Point(-TileSize.X, 0);
            Point LeftSize = new Point(TileSize.X, height);
            Result.Add(new Rectangle(LeftPos, LeftSize));

            Point RightPos = new Point(width, 0);
            Point RightSize = new Point(TileSize.X, height);
            Result.Add(new Rectangle(RightPos, RightSize));

            return Result;
        }

        public void Update(Player player, float deltaTime)
        {
            layers.ForEach(l => l.Update(CollisionBoxes, EntityTransforms, player, deltaTime));
        }

        public void Draw(Vector2 cameraPos, Player player, SpriteBatch spriteBatch)
        {
            for (int layer = 0; layer < layers.Count; layer++)
            {
                layers[layer].Draw(cameraPos, player, spriteBatch);
            }
        }
    }


}
