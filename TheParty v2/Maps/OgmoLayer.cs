using Microsoft.Xna.Framework;
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

        public OgmoEntity EntityWithName(string name) => entities.Find(e => e.values["Name"] == name);

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
                entities.Sort(delegate (OgmoEntity e1, OgmoEntity e2)
                {
                    if (e1 == null && e2 == null) return 0;
                    else if (e1 == null) return -1;
                    else if (e2 == null) return 1;
                    else return e1.Transform.Position.Y.CompareTo(e2.Transform.Position.Y);
                });

                bool PlayerDrawn = false;
                for (int i = 0; i < entities.Count; i++)
                {
                    entities[i].Draw(cameraPos, spriteBatch);

                    if (player.Transform.Position.Y >= entities[i].Transform.Position.Y)
                    {
                        player.Draw(cameraPos, spriteBatch);
                        PlayerDrawn = true;
                    }
                }

                if (!PlayerDrawn)
                    player.Draw(cameraPos, spriteBatch);

            }
        }
    }
}
