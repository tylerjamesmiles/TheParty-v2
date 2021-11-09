﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class OgmoLayer
    {
        public string name;
        public string _eid;
        public int offsetX;
        public int offsetY;
        public int gridCellWidth;
        public int gridCellHeight;
        public int gridCellsX;
        public int gridCellsY;
        public string tileset;
        public int[] data;
        public string[] grid;
        public List<OgmoEntity> entities;
        public int exportMode;
        public int arrayMode;

        public enum LayerType { Tile, Entity, Collision, Undefined };
        public LayerType Type;

        public OgmoEntity EntityWithName(string name) => entities.Find(e => e.name == name);

        public void Initialize()
        {
            Type =
                (data != null) ? LayerType.Tile :
                (entities != null) ? LayerType.Entity :
                (grid != null) ? LayerType.Collision :
                LayerType.Undefined;

            if (Type == LayerType.Entity)
                entities.ForEach(e => e.Initialize());

        }

        public void Update(List<Rectangle> collisionRects, List<Transform2D> entityTransforms, Player player, float deltaTime)
        {
            if (Type == LayerType.Entity)
                entities.ForEach(e => e.Update(collisionRects, entityTransforms, player , deltaTime));
        }

        public void Draw(Vector2 cameraPos, Player player, SpriteBatch spriteBatch)
        {
            if (data != null)
            {
                Texture2D TileSet = GameContent.Sprites[tileset];
                Point TileSize = new Point(gridCellWidth, gridCellHeight);
                int TileSetTileWidth = TileSet.Width / TileSize.X;
                int MapTileWidth = gridCellsX;

                Func<int, int, Rectangle> GetRect = (id, width) =>
                    new Rectangle(new Point(id % width, id / width) * TileSize, TileSize);

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == -1) continue;

                    Rectangle SourceRect = GetRect(data[i], TileSetTileWidth);
                    Rectangle DrawRect = GetRect(i, MapTileWidth);
                    DrawRect.Location -= cameraPos.ToPoint();

                    spriteBatch.Draw(TileSet, DrawRect, SourceRect, Color.White);
                }
            }
            else if (entities != null)
            {
                player.Draw(cameraPos, spriteBatch);

                // lazy y-sorting
                foreach (var entity in entities)
                {
                    if (player.Transform.Position.Y < entity.Transform.Position.Y)
                    {
                        player.Draw(cameraPos, spriteBatch);
                        entity.Draw(cameraPos, spriteBatch);
                    }
                    else
                    {
                        entity.Draw(cameraPos, spriteBatch);
                        player.Draw(cameraPos, spriteBatch);
                    }
                }
                
            }
        }
    }
}
