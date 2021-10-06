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

        public void Initialize()
        {
            CollisionBoxes = GenerateCollisionBoxes();
            layers.ForEach(l => l.Initialize());
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

        public void Draw(Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            layers.ForEach(l => l.Draw(cameraPos, spriteBatch));  
        }
    }


}
