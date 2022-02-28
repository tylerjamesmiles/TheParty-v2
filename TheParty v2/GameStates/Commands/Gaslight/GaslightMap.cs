using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheParty_v2
{
    class GaslightMap
    {
        public List<int> Tiles { get; set; }
        public List<bool> Collision { get; set; }
        public List<Point> Oil { get; set; }
        public Dictionary<Point, int> Fire { get; set; }
        public Point OilSource;
        public Point Exit;
        public bool DynamitePlaced;
        public bool ExitOpened;

        public Point DrawOffset = new Point(-8, -8);
        public Point MapTileSize;
        public Point MapPixelSize;
        int TilesetTileWidth = 8;
        public List<Point> NeighborOffsets =
            new List<Point>(new[] { new Point(0, -1), new Point(0, +1), new Point(-1, 0), new Point(+1, 0) });

        public Point PlayerStartPos;
        public Point CellmateStartPos;

        public enum TileType { Floor, Wall, Oil, OilSource, Fire, Exit, Dynamite }


        public GaslightMap(string mapName)
        {
            OgmoTileMap OgmoMap = GameContent.Maps[mapName];
            Tiles = new List<int>(OgmoMap.layers[0].data);
            Collision = new List<string>(OgmoMap.CollisionLayer.grid).ConvertAll(cs => cs == "1");
            Oil = new List<Point>();
            Fire = new Dictionary<Point, int>();

            MapTileSize = new Point(OgmoMap.layers[0].gridCellsX, OgmoMap.layers[0].gridCellsY);
            MapPixelSize = new Point(OgmoMap.width, OgmoMap.height);

            DynamitePlaced = false;
            ExitOpened = false;
            //ExitOpened = true;

            // Set game pieces

            OgmoLayer EventLayer = OgmoMap.layers[1];
            Point PlayerStartSource = new Point(16, 48);
            Point CellmateStartSource = new Point(32, 48);
            Point OilSourceSource = new Point(96, 48);
            Point ClosedExitSource = new Point(112, 48);
            Point OpenExitSource = new Point(112, 64);
            Point OilSource = new Point(96, 32);

            PlayerStartPos = Point.Zero;
            CellmateStartPos = Point.Zero;
            OilSource = Point.Zero;
            Exit = Point.Zero;

            for (int tile = 0; tile < EventLayer.data.Length; tile++)
            {
                int TileID = EventLayer.data[tile];
                Point SourceTilePos = new Point(TileID % TilesetTileWidth, TileID / TilesetTileWidth);
                Point SourcePixelPos = new Point(SourceTilePos.X * 16, SourceTilePos.Y * 16);
                Point MapTilePos = new Point(tile % MapTileSize.X, tile / MapTileSize.X);

                if (SourcePixelPos == PlayerStartSource) PlayerStartPos = MapTilePos;
                else if (SourcePixelPos == CellmateStartSource) CellmateStartPos = MapTilePos;
                else if (SourcePixelPos == OilSourceSource) OilSource = MapTilePos;
                else if (SourcePixelPos == ClosedExitSource) { Exit = MapTilePos; ExitOpened = false; }
                else if (SourcePixelPos == OpenExitSource) { Exit = MapTilePos; ExitOpened = true; }
                else if (SourcePixelPos == OilSource) 
                { 
                    Oil.Add(MapTilePos); 
                }
            }

            if (PlayerStartPos == Point.Zero) throw new Exception("Didn't find PlayerStartPos.");
            //if (CellmateStartPos == Point.Zero) throw new Exception("Didn't find CellmateStartpos.");
            //if (OilSource == Point.Zero) throw new Exception("Didn't find OilSource");
            if (Exit == Point.Zero) throw new Exception("Didn't find Exit.");
        }

        public TileType GetTileType(Point pos)
        {
            int TilesIdx = pos.Y * MapTileSize.X + pos.X;

            if (pos == Exit)
                return TileType.Exit;

            if (pos == OilSource)
                return TileType.OilSource;

            if (TilesIdx < 0 || TilesIdx >= Tiles.Count)
                return TileType.Wall;

            int SourceIdx = Tiles[TilesIdx];
            if (SourceIdx == -1 ||
                SourceIdx == 0 ||
                SourceIdx == 1 ||
                SourceIdx == 2 ||
                SourceIdx == 13)
                return TileType.Wall;
            else if (Oil.Contains(pos))
                if (Fire.ContainsKey(pos))
                    return TileType.Fire;
                else
                    return TileType.Oil;
            else
                return TileType.Floor;
        }

        public bool IsPositionAvailable(Point pos)
        {
            if (ExitOpened && pos == Exit)
                return true;

            int Idx = pos.Y * MapTileSize.X + pos.X;

            return Idx >= 0 && Idx < Collision.Count &&
            !Fire.ContainsKey(pos) &&
            !Collision[pos.Y * MapTileSize.X + pos.X];
        }

        public bool IsPathObstructed(Queue<Point> path) =>
            new List<Point>(path).TrueForAll(p => IsPositionAvailable(p)) == false;

        public void AddOil(Point pos)
        {
            Oil.Add(pos);

            foreach (Point neighbor in NeighborOffsets)
            {
                Point neighborPoint = pos + neighbor;
                if (Fire.ContainsKey(neighborPoint))
                    AddFire(pos);
            }

        }
        public int AddFire(Point pos)
        {
            int NumLit = 0;

            // BFS Flood Fill
            Queue<Point> Queue = new Queue<Point>();
            Queue.Enqueue(pos);

            List<Point> Discovered = new List<Point>();

            while (Queue.Count > 0)
            {
                Point ThisPos = Queue.Dequeue();

                if (Fire.ContainsKey(ThisPos))
                    break;

                Fire.Add(ThisPos, 10 + new Random().Next(10));
                Discovered.Add(ThisPos);
                NumLit++;

                foreach (Point neighbor in NeighborOffsets)
                {
                    Point NeighborPos = ThisPos + neighbor;
                    if (Oil.Contains(NeighborPos) && !Fire.Keys.Contains(NeighborPos))
                    {
                        Queue.Enqueue(NeighborPos);
                    }
                    else if (DynamitePlaced && NeighborPos == Exit)
                    {
                        ExitOpened = true;
                        
                        // add a bunch of fire around exit
                        for (int tile = 0; tile < Tiles.Count; tile++)
                        {
                            Point TilePos = new Point(tile % MapTileSize.X, tile / MapTileSize.X);
                            int FireRange = 15;

                            if ((Exit - TilePos).ToVector2().LengthSquared() < FireRange * FireRange &&
                                new Random().Next(3) < 2 &&
                                IsPositionAvailable(TilePos) &&
                                !Fire.Keys.Contains(TilePos))
                            {
                                if (!Oil.Contains(TilePos))
                                    Oil.Add(TilePos);

                                if (!Fire.ContainsKey(TilePos))
                                {
                                    Fire.Add(TilePos, 5 + new Random().Next(5));
                                    NumLit++;
                                }
                            }
                        }
                    }
                }
            }

            return NumLit;
        }

        public void UpdateFire()
        {
            foreach (Point fire in Fire.Keys)
            {
                Fire[fire] -= 1;
                if (Fire[fire] == 0)
                    Fire.Remove(fire);
            }
        }

        public bool PointsAreNeighbors(Point p1, Point p2)
        {
            return (p1 - p2).ToVector2().LengthSquared() == 1;
        }

        public List<Point> LegalNeighborsOf(Point point)
        {
            List<Point> Result = new List<Point>();
            foreach (Point offset in NeighborOffsets)
                if (IsPositionAvailable(point + offset))
                    Result.Add(point + offset);
            return Result;
        }

        public Queue<Point> GetPath(Point startPos, Point targetPos, List<GaslightEntity> entities)
        {
            // Breadth-First Search

            Queue<Point> Queue = new Queue<Point>();
            Queue.Enqueue(startPos);

            List<Point> Discovered = new List<Point>();

            Dictionary<Point, Point> ChildParent = new Dictionary<Point, Point>();
            ChildParent.Add(startPos, startPos);

            bool TargetFound = false;

            while (Queue.Count > 0)
            {
                Point ThisPoint = Queue.Dequeue();

                if (ThisPoint == targetPos)
                {
                    TargetFound = true;
                    break;
                }

                Discovered.Add(ThisPoint);

                foreach (Point point in NeighborOffsets)
                {
                    Point neighborPoint = ThisPoint + point;
                    if (IsPositionAvailable(neighborPoint) && 
                        !entities.Exists(e => e.TilePos != targetPos && e.TilePos == neighborPoint) &&
                        !Discovered.Contains(neighborPoint))
                    {
                        if (!ChildParent.ContainsKey(neighborPoint))
                            ChildParent.Add(neighborPoint, ThisPoint);
                        Queue.Enqueue(neighborPoint);
                    }
                }
            }

            if (!TargetFound)
                return new Queue<Point>();

            // Turn ParentChild into Path
            Point LookingAt = targetPos;
            Stack<Point> Path = new Stack<Point>();
            while (LookingAt != startPos)
            {
                Path.Push(LookingAt);
                LookingAt = ChildParent[LookingAt];
            }

            return new Queue<Point>(Path);
        }

        public void Draw(Camera2D camera, SpriteBatch spriteBatch)
        {
            // Draw Tiles
            int TilePixelWidth = 16;
            Point TileSize = new Point(TilePixelWidth, TilePixelWidth);

            for (int tileIdx = 0; tileIdx < Tiles.Count; tileIdx++)
            {
                Vector2 ScreenPlayerTilePos = new Vector2(tileIdx % MapTileSize.X, tileIdx / MapTileSize.X);
                Vector2 ScreenDrawPos = ScreenPlayerTilePos * TilePixelWidth;

                Point SourceTilePos = new Point(Tiles[tileIdx] % TilesetTileWidth, Tiles[tileIdx] / TilesetTileWidth);
                Point SourcePos = new Point(SourceTilePos.X * 16, SourceTilePos.Y * 16);

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle((ScreenDrawPos - camera.Position).ToPoint(), TileSize),
                    new Rectangle(SourcePos, TileSize),
                    Color.White);
            }

            // Draw Oil
            foreach (Point oil in Oil)
            {
                Point DrawPos = new Point(oil.X * 16, oil.Y * 16);
                Point SourcePos = new Point(96, 32);

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle(DrawPos - camera.Position.ToPoint(), TileSize),
                    new Rectangle(SourcePos, TileSize),
                    Color.White);
            }

            {
                // Draw OilSource
                Point DrawPos = new Point(OilSource.X * 16, OilSource.Y * 16);
                Point SourcePos = new Point(96, 48);

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle(DrawPos  - camera.Position.ToPoint(), TileSize),
                    new Rectangle(SourcePos, TileSize),
                    Color.White);
            }

            {
                // Draw Exit
                Point DrawPos = new Point(Exit.X * 16, Exit.Y * 16);

                Point SourcePos =
                    (ExitOpened) ? new Point(112, 64) :
                    (DynamitePlaced) ? new Point(96, 64) :
                    new Point(112, 48);

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle(DrawPos  - camera.Position.ToPoint(), TileSize),
                    new Rectangle(SourcePos, TileSize),
                    Color.White);
            }

            // Draw Fire
            foreach (Point fire in Fire.Keys)
            {
                Point DrawPos = new Point(fire.X * 16, fire.Y * 16);
                Point SourcePos = new Point(112, 32);

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle(DrawPos  - camera.Position.ToPoint(), TileSize),
                    new Rectangle(SourcePos, TileSize),
                    Color.White);

                spriteBatch.DrawString(
                    GameContent.Font,
                    Fire[fire].ToString(),
                    DrawPos.ToVector2() + new Vector2(6, -0) - camera.Position.ToPoint().ToVector2(),
                    Color.White);
            }
        }
    }
}
