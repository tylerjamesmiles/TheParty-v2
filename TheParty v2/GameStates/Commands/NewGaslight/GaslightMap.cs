using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TheParty_v2
{
    class GaslightMap
    {
        Point MapSizeTiles;
        public Vector2 MapSizePixels { get; private set; }
        Point TileSize;
        int TilesetWidthTiles;

        List<int> FloorTiles;
        List<int> DetailedTiles;
        public List<Rectangle> Collision { get; private set; }
        List<int> Visible;
        List<int> HalfVisible;

        int Floor = 9;
        int Wall = 24;

        public GaslightMap(Point mapSize)
        {
            TileSize = new Point(16, 16);
            MapSizeTiles = mapSize;
            MapSizePixels = new Vector2(MapSizeTiles.X * TileSize.X, MapSizeTiles.Y * TileSize.Y);
            TilesetWidthTiles = 8;
        }

        public bool IsTileVisible(Point pos)
        {
            if (pos.X < 0 || pos.X > MapSizeTiles.X - 1|| pos.Y < 0 || pos.Y > MapSizeTiles.Y - 1)
                return false;

            int Idx = pos.Y * MapSizeTiles.X + pos.X;

            if (FloorTiles[Idx] == Floor)
                return true;

            return false;
           
        }

        public bool IsSpaceAvailable(Vector2 pos)
        {
            foreach (Rectangle box in Collision)
                if (box.Contains(pos))
                    return false;
            return true;
        }

        public bool HasFloorNeighbor(Point pos, List<int> container)
        {
            int Idx = pos.Y * MapSizeTiles.X + pos.X;
            bool NorthFloor = (pos.Y > 0 && container[Idx - MapSizeTiles.X] == Floor);
            bool SouthFloor = (pos.Y < MapSizeTiles.Y - 1 && container[Idx + MapSizeTiles.X] == Floor);
            bool WestFloor = (pos.X > 0 && container[Idx - 1] == Floor);
            bool EastFloor = (pos.X < MapSizeTiles.X - 1 && container[Idx + 1] == Floor);
            bool NeighborFloor = NorthFloor || SouthFloor || WestFloor || EastFloor;
            return NeighborFloor;
        }

        public void ClearVisible()
        {
            HalfVisible = new List<int>();
            Visible = new List<int>();
        }

        public bool CanSeeBetweenPoints(Point pos1, Point pos2)
        {
            List<int> Spaces = EmptySpacesBetweenTiles(pos1, pos2);
            int Pos2Idx = pos2.Y * MapSizeTiles.X + pos2.X;
            int LastSpace = Spaces[Spaces.Count - 1];

            return 
                Pos2Idx == LastSpace ||
                Pos2Idx == LastSpace - MapSizeTiles.X ||
                Pos2Idx == LastSpace + MapSizeTiles.X ||
                Pos2Idx == LastSpace + 1 ||
                Pos2Idx == LastSpace - 1;
        }

        public List<int> EmptySpacesBetweenTiles(Point pos1, Point pos2)
        {
            List<int> Result = new List<int>();

            Point RayStartPos = pos1;
            Point ToPos2 = pos2 - pos1;
            Vector2 Dir = Vector2.Normalize(ToPos2.ToVector2());
            float XStepSize = MathF.Sqrt(1 + (Dir.Y / Dir.X) * (Dir.Y / Dir.X));
            float YStepSize = MathF.Sqrt(1 + (Dir.X / Dir.Y) * (Dir.X / Dir.Y));
            float XRayLength;
            float YRayLength;
            int XStep;
            int YStep;
            Point MapTile = RayStartPos;

            if (Dir.X < 0)
            {
                XStep = -1;
                XRayLength = (RayStartPos.X - (float)MapTile.X) * XStepSize;
            }
            else
            {
                XStep = +1;
                XRayLength = ((float)(MapTile.X + 1) - RayStartPos.X) * XStepSize;
            }

            if (Dir.Y < 0)
            {
                YStep = -1;
                YRayLength = (RayStartPos.Y - (float)MapTile.Y) * YStepSize;
            }
            else
            {
                YStep = +1;
                YRayLength = ((float)(MapTile.Y + 1) - RayStartPos.Y) * YStepSize;
            }

            // Ray march
            bool WallHit = false;
            float MaxDistance = 10f;
            float Distance = 0f;
            bool TargetFound = false;
            while (!WallHit && !TargetFound && Distance < MaxDistance)
            {
                if (XRayLength < YRayLength)
                {
                    MapTile.X += XStep;
                    Distance = XRayLength;
                    XRayLength += XStepSize;
                }
                else
                {
                    MapTile.Y += YStep;
                    Distance = YRayLength;
                    YRayLength += YStepSize;
                }

                int MapTileIdx = MapTile.Y * MapSizeTiles.X + MapTile.X;

                Result.Add(MapTileIdx);

                if (!IsTileVisible(MapTile))
                    WallHit = true;

                if (MapTile == pos2)
                    TargetFound = true;
            }

            return Result;
        }

        public void SetVisible(GaslightEntity entity)
        {
            Vector2 RayStartPos = entity.Position / 16;
            float Theta = 0f;
            float TwoPi = MathF.PI * 2;
            float Rot = TwoPi / 64f;

            Point StartPosTile = RayStartPos.ToPoint();
            int PosIdx = StartPosTile.Y * MapSizeTiles.X + StartPosTile.X;
            if (entity.EntityType == GaslightEntity.Type.Player &&
                !Visible.Contains(PosIdx) && !HalfVisible.Contains(PosIdx))
                HalfVisible.Add(PosIdx);

            else if (entity.EntityType == GaslightEntity.Type.Fire &&
                !Visible.Contains(PosIdx))
                    Visible.Add(PosIdx);

            while (Theta < TwoPi)
            {
                Vector2 Dir = new Vector2(MathF.Cos(Theta), MathF.Sin(Theta));
                Vector2 Pos2 = RayStartPos + Dir * 5;
                List<int> Lit = EmptySpacesBetweenTiles(RayStartPos.ToPoint(), Pos2.ToPoint());

                foreach (int Idx in Lit)
                {
                    if (entity.EntityType == GaslightEntity.Type.Player &&
                        !Visible.Contains(Idx) && !HalfVisible.Contains(Idx))
                        HalfVisible.Add(Idx);

                    else if (entity.EntityType == GaslightEntity.Type.Fire &&
                        !Visible.Contains(Idx))
                        Visible.Add(Idx);
                }

                Theta += Rot;
            }
        }

        public void SetHalfVisible()
        {
            foreach (int visible in Visible)
            {
                Point pos = new Point(visible % MapSizeTiles.X, visible / MapSizeTiles.X);
                int Up = visible - MapSizeTiles.Y;
                int Down = visible + MapSizeTiles.Y;
                int Left = visible - 1;
                int Right = visible + 1;

                if (pos.Y > 0 && !Visible.Contains(Up) && !HalfVisible.Contains(Up)) HalfVisible.Add(Up);
                if (pos.Y < MapSizeTiles.Y - 1 && !Visible.Contains(Down) && !HalfVisible.Contains(Down)) HalfVisible.Add(Down);
                if (pos.X > 0 && !Visible.Contains(Left) && !HalfVisible.Contains(Left)) HalfVisible.Add(Left);
                if (pos.X < MapSizeTiles.X - 1 && !Visible.Contains(Right) && !HalfVisible.Contains(Right)) HalfVisible.Add(Right);
            }
        }

        public Vector2 RandomOpenSpace()
        {
            int TimesTried = 0;
            int MaxTimesTried = 1000;
            Random Rand = new Random();

            while (TimesTried < MaxTimesTried)
            {
                Point RandTile = new Point(Rand.Next(MapSizeTiles.X), Rand.Next(MapSizeTiles.Y));

                if (DetailedTiles[RandTile.Y * MapSizeTiles.Y + RandTile.X] == Floor)
                    return RandTile.ToVector2() * 16 + new Vector2(8, 8);
                else
                    TimesTried++;
            }

            // Random search unsuccessful. Return first open spot.
            for (int x = 0; x < MapSizeTiles.X; x++)
                for (int y = 0; y < MapSizeTiles.Y; y++)
                    if (DetailedTiles[y * MapSizeTiles.X + x] == Floor)
                        return new Vector2(x, y) * 16 + new Vector2(8, 8);

            throw new Exception("No open spots available?");
        }

        private void SetFloorTile(int x, int y, int setTo)
        {
            int Idx = y * MapSizeTiles.X + x;
            if (FloorTiles[Idx] != setTo)
                FloorTiles[Idx] = setTo;
        }

        private void AddRoom(Rectangle room)
        {
            for (int x = room.Left; x < room.Left + room.Width; x++)
                for (int y = room.Top; y < room.Top + room.Height; y++)
                    SetFloorTile(x, y, Floor);
        }

        private void HorizontalTunnel(int startX, int endX, int y)
        {
            int Dir = startX < endX ? +1 : -1;
            for (int x = startX; Dir == +1 ? x < endX : x > endX; x += Dir)
                SetFloorTile(x, y, Floor);
        }

        private void VerticalTunnel(int startY, int endY, int x)
        {
            int Dir = startY < endY ? +1 : -1;
            for (int y = startY; Dir == +1 ? y < endY : y > endY; y += Dir)
                SetFloorTile(x, y, Floor);
        }

        public void AddTunnel(Point start, Point end)
        {
            bool HorizontalFirst = new Random().Next(2) == 0;

            if (HorizontalFirst)
            {
                HorizontalTunnel(start.X, end.X, start.Y);
                VerticalTunnel(start.Y, end.Y, end.X);
            }
            else
            {
                VerticalTunnel(start.Y, end.Y, start.X);
                HorizontalTunnel(start.X, end.X, end.Y);
            }
        }

        public void Generate()
        {
            // Fill with empty space
            FloorTiles = new List<int>();
            for (int y = 0; y < MapSizeTiles.Y; y++)
                for (int x = 0; x < MapSizeTiles.X; x++)
                    FloorTiles.Add(Wall);

            // Rooms

            int MaxNumRooms = 10;
            int TimesTried = 0;
            int MaxTimesTried = 100;

            int MinRoomWidth = 4;
            int MinRoomHeight = 4;
            int MaxRoomWidth = 8;
            int MaxRoomHeight = 8;

            Random Rand = new Random();

            List<Rectangle> Rooms = new List<Rectangle>();

            Func<int, int, int> RandBetween = (min, max) => min + Rand.Next(max - min);

            while (Rooms.Count < MaxNumRooms && TimesTried < MaxTimesTried)
            {
                int Width = RandBetween(MinRoomWidth, MaxRoomWidth);
                int Height = RandBetween(MinRoomHeight, MaxRoomHeight);
                int Top = RandBetween(2, MapSizeTiles.Y - Height - 2);
                int Left = RandBetween(2, MapSizeTiles.X - Width - 2);

                bool RoomLegal = true;
                for (int x = Left - 1; x < Left + Width + 1 && x >= 0 && x < MapSizeTiles.X; x++)
                    for (int y = Top - 1; y < Top + Height + 1 && y >= 0 && y < MapSizeTiles.Y; y++)
                    {
                        int ID = FloorTiles[y * MapSizeTiles.X + x];
                        if (ID == Floor)
                            RoomLegal = false;
                    }

                if (RoomLegal)
                {
                    Rectangle Room = new Rectangle(new Point(Left, Top), new Point(Width, Height));
                    AddRoom(Room);
                    Rooms.Add(Room);

                }
                else
                    TimesTried++;
            }

            // Tunnels

            // Sort rooms left to right
            Rooms.Sort((r1, r2) => r1.Left < r2.Left ? -1 : +1);

            // Add tunnels from each room to the next
            for (int roomIdx = 0; roomIdx < Rooms.Count; roomIdx++)
            {
                Rectangle Room1 = Rooms[roomIdx];
                Rectangle Room2 = roomIdx == Rooms.Count - 1 ? Rooms[0] : Rooms[roomIdx + 1];

                Point Room1Center = new Point(Room1.Left + Room1.Width / 2, Room1.Top + Room1.Height / 2);
                Point Room2Center = new Point(Room2.Left + Room2.Width / 2, Room2.Top + Room2.Height / 2);

                AddTunnel(Room1Center, Room2Center);
            }

            // Add collision boxes
            Collision = new List<Rectangle>();
            for (int x = 0; x < MapSizeTiles.X; x++)
                for (int y = 0; y < MapSizeTiles.Y; y++)
                {
                    int Idx = y * MapSizeTiles.X + x;

                    if (FloorTiles[Idx] == Wall)
                    {
                        if (HasFloorNeighbor(new Point(x, y), FloorTiles))
                        {
                            Rectangle Rect = new Rectangle(new Point(x * 16, y * 16), new Point(16, 16));
                            Collision.Add(Rect);
                        }
                    }
                }


            // Details

            DetailedTiles = new List<int>(FloorTiles);

            int MWidth = MapSizeTiles.X;
            int MHeight = MapSizeTiles.Y;

            for (int x = 0; x < MapSizeTiles.X; x++)
                for (int y = 0; y < MapSizeTiles.Y; y++)
                {
                    int Idx = y * MapSizeTiles.X + x;

                    int Tile =                                              FloorTiles[Idx];
                    int N =     (y > 0) ?                               FloorTiles[Idx - MWidth] : -1;
                    int S =     (y < MHeight - 1) ?                     FloorTiles[Idx + MWidth] : -1;
                    int W =      (x > 0) ?                               FloorTiles[Idx - 1] : -1;
                    int E =      (x < MWidth - 1) ?                      FloorTiles[Idx + 1] : -1;
                    int NW = (y > 0 && x > 0) ?                      FloorTiles[Idx - MWidth - 1] : -1;
                    int NE = (y > 0 && x < MWidth - 1) ?             FloorTiles[Idx - MWidth + 1] : -1;
                    int SW = (y < MHeight - 1 && x > 0) ?            FloorTiles[Idx + MWidth - 1] : -1;
                    int SE = (y < MHeight - 1 && x < MWidth - 1) ?   FloorTiles[Idx + MWidth + 1] : -1;

                    int InsideTLSource = 0;
                    int InsideTMSource = 1;
                    int InsideTRSource = 2;
                    int InsideBLSource = 16;
                    int InsideBMSource = 17;
                    int InsideBRSource = 18;
                    int InsideLSource = 8;
                    int InsideRSource = 10;
                    int InsideBSource = 5;

                    int BackInsideRCorner = 11;
                    int BackInsideLCorner = 12;
                    int BackInsideBCorner = 19;
                    int BackInsideRCornerWallL = 14;
                    int BackInsideLCornerWallR = 15;

                    int FrontInsideRCorner = 3;
                    int FrontInsideLCorner = 4;
                    int FrontInsideBCorner = 20;
                    int FrontInsideRCornerWallL = 6;
                    int FrontInsideLCornerWallR = 7;

                    int Fl = Floor;
                    int Wl = Wall;

                    // Inside Back walls
                    if (Tile == Wl && S == Fl) DetailedTiles[Idx] = InsideTMSource;
                    if (W == Wl && SW == Wl && S == Fl && SE == Fl) DetailedTiles[Idx] = InsideTLSource;
                    if (E == Wl && SW == Fl && S == Fl && SE == Wl) DetailedTiles[Idx] = InsideTRSource;

                    // L/R Walls
                    if (Tile == Fl && W == Wl && E == Fl) DetailedTiles[Idx] = InsideLSource;
                    if (Tile == Fl && W == Fl && E == Wl) DetailedTiles[Idx] = InsideRSource;
                    if (Tile == Fl && W == Wl && E == Wl) DetailedTiles[Idx] = InsideBSource;

                    // Inside Front walls
                    if (Tile == Fl && S == Wl) DetailedTiles[Idx] = InsideBMSource;
                    if (Tile == Fl && W == Wl && S == Wl) DetailedTiles[Idx] = InsideBLSource;
                    if (Tile == Fl && E == Wl && S == Wl) DetailedTiles[Idx] = InsideBRSource;

                    // BackCorners
                    if (Tile == Fl && E == Wl && S == Fl && SE == Fl) DetailedTiles[Idx] = BackInsideRCorner;
                    if (Tile == Fl && W == Wl && S == Fl && SW == Fl) DetailedTiles[Idx] = BackInsideLCorner;
                    if (Tile == Fl && E == Wl && S == Fl && SE == Fl && W == Wl) DetailedTiles[Idx] = BackInsideRCornerWallL;
                    if (Tile == Fl && W == Wl && S == Fl && SW == Fl && E == Wl) DetailedTiles[Idx] = BackInsideLCornerWallR;
                    if (Tile == Fl && N == Fl && E == Wl && W == Wl && S == Fl && SE == Fl && SW == Fl) DetailedTiles[Idx] = BackInsideBCorner;
                    
                    // FrontCorners
                    if (Tile == Fl && W == Fl && E == Fl && S == Fl && SE == Wl) DetailedTiles[Idx] = FrontInsideRCorner;
                    if (Tile == Fl && W == Fl && E == Fl && S == Fl && SW == Wl) DetailedTiles[Idx] = FrontInsideLCorner;
                    if (Tile == Fl && W == Wl && E == Fl && S == Fl && SE == Wl && SW == Wl) DetailedTiles[Idx] = FrontInsideRCornerWallL;
                    if (Tile == Fl && E == Wl && W == Fl && S == Fl && SW == Wl && SE == Wl) DetailedTiles[Idx] = FrontInsideLCornerWallR;
                    if (Tile == Fl && W == Fl && E == Fl && S == Fl && SE == Wl && SW == Wl) DetailedTiles[Idx] = FrontInsideBCorner;
                }

            ClearVisible();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            // DrawBackground
            Rectangle BackgroundRect = new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize);
            Rectangle BackgroundSource = new Rectangle(new Point(0, 48), new Point(15, 16));
            spriteBatch.Draw(
                GameContent.Sprites["GaslightingTiles"],
                BackgroundRect,
                BackgroundSource,
                Color.White);

            foreach (int visible in Visible)
            {
                int x = visible % MapSizeTiles.X;
                int y = visible / MapSizeTiles.Y;

                Vector2 DrawTilePos = new Vector2(x, y);
                Vector2 DrawPixelPos = DrawTilePos * TileSize.ToVector2();

                int SourceID = DetailedTiles[y * MapSizeTiles.X + x];
                Vector2 SourceTilePos = new Vector2(SourceID % TilesetWidthTiles, SourceID / TilesetWidthTiles);
                Vector2 SourcePixelPos = SourceTilePos * TileSize.ToVector2();

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle((DrawPixelPos - cameraPos).ToPoint(), TileSize),
                    new Rectangle(SourcePixelPos.ToPoint(), TileSize),
                    Color.White);
            }

            foreach (int visible in HalfVisible)
            {
                int x = visible % MapSizeTiles.X;
                int y = visible / MapSizeTiles.Y;

                Vector2 DrawTilePos = new Vector2(x, y);
                Vector2 DrawPixelPos = DrawTilePos * TileSize.ToVector2();

                int SourceID = DetailedTiles[y * MapSizeTiles.X + x];
                Vector2 SourceTilePos = new Vector2(SourceID % TilesetWidthTiles, SourceID / TilesetWidthTiles);
                Vector2 SourcePixelPos = SourceTilePos * TileSize.ToVector2();

                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle((DrawPixelPos - cameraPos).ToPoint(), TileSize),
                    new Rectangle(SourcePixelPos.ToPoint(), TileSize),
                    Color.White);

                // transparent shroud
                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle((DrawPixelPos - cameraPos).ToPoint(), TileSize),
                    new Rectangle(new Point(0, 64), TileSize),
                    Color.White);
            }

            // Draw Collision Rects
            //foreach (Rectangle box in Collision)
            //{
            //    Rectangle transformed = new Rectangle(box.Location - cameraPos.ToPoint(), box.Size);
            //    DrawPrimitives2D.Rectangle(transformed, Color.Green, spriteBatch);

            //}

            //// Draw Visible Tiles
            //foreach (int visible in Visible)
            //{
            //    Point pos = new Point(visible % MapSizeTiles.X, visible / MapSizeTiles.X);
            //    Vector2 ScreenPos = pos.ToVector2() * 16f - cameraPos;
            //    Rectangle Rect = new Rectangle(ScreenPos.ToPoint(), new Point(16, 16));
            //    DrawPrimitives2D.Rectangle(Rect, Color.Red, spriteBatch);
            //}
        }
        
    }
}
