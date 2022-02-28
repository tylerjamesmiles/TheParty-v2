using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheParty_v2
{
    class GaslightEntity
    {
        public Point TilePos;
        public Vector2 ScreenPos;
        LerpV MoveLerp;
        Point DrawOffset = new Point(-8, -12);
        Point Facing;
        bool LeftFoot;
        public bool Moving;
        float Speed = 0.1f;
        bool MoveChosen;
        int AmtOil;

        Point AffectedSpace;

        Queue<Point> Path;
        Point Target;

        public List<Point> CanSee;

        string Message;
        LerpF MessageTransparencyLerp;

        public enum Type { Player, Cellmate, Spider }
        public Type EntityType;

        public enum CState { SeekOilReserve, SeekPlayer, SeekPlacedOil, MoveOutOfWay, SeekNearPlayer, FleeSpider, Wander }
        public CState CellmateState;
        public CState PrevCellmateState;

        public enum SState { Wander, SeekPlayer }
        public SState SpiderState;

        string SpriteName;

        public GaslightEntity(GaslightMap map, Type type, Point startTilePos)
        {
            TilePos = startTilePos;
            Facing = new Point(0, 1);
            AffectedSpace = TilePos + Facing;
            ScreenPos = TilePos.ToVector2() * 16;
            LeftFoot = true;
            EntityType = type;
            MoveLerp = new LerpV(ScreenPos, ScreenPos, 0f);
            CanSee = new List<Point>();

            Message = "";
            MessageTransparencyLerp = new LerpF(0f, 0f, 0f);

            Path = new Queue<Point>();
            if (EntityType == Type.Cellmate)
            {
                Target = new Point(13, 2);
                SwitchCellmateState(CState.SeekOilReserve);
                AmtOil = 0;
               
                SpriteName = "GaslightCellmate";
            }
            else if (EntityType == Type.Player)
            {
                AmtOil = 5;
                SpriteName = "CharacterSmallBase";
            }
            else if (EntityType == Type.Spider)
            {
                SpriteName = "GaslightSpider";
                SpiderState = SState.Wander;
            }
        }

        private void SetCanSee(GaslightMap map)
        {
            CanSee = new List<Point>();

            int ViewRadius = 6;

            Point LeftDir = new Point(Facing.Y, -Facing.X);
            Point RightDir = new Point(-Facing.Y, Facing.X);

            List<Point> LineStarts = new List<Point>();
            List<Point> LineEnds = new List<Point>();

            LineStarts.Add(TilePos - Facing - Facing);

            LineStarts.Add(TilePos - Facing);
            LineStarts.Add(TilePos + LeftDir - Facing);
            LineStarts.Add(TilePos + RightDir - Facing);

            LineStarts.Add(TilePos);
            LineStarts.Add(TilePos + LeftDir);
            LineStarts.Add(TilePos + RightDir);

            LineStarts.Add(TilePos);
            LineStarts.Add(TilePos + LeftDir + LeftDir);
            LineStarts.Add(TilePos + RightDir + RightDir);

            LineStarts.Add(TilePos + Facing);
            LineStarts.Add(TilePos + LeftDir + Facing);
            LineStarts.Add(TilePos + RightDir + Facing);

            foreach (Point start in LineStarts)
            {
                for (int d = 0; d < ViewRadius; d++)
                {
                    Point Spot = start + new Point(Facing.X * d, Facing.Y * d);
                    CanSee.Add(Spot);

                    if (!map.IsPositionAvailable(Spot))
                        break;
                }
            }

            if (EntityType == Type.Player)
            {
                foreach (Point fire in map.Fire.Keys)
                {
                    CanSee.Add(fire);
                    CanSee.Add(fire + new Point(-1, 0));
                    CanSee.Add(fire + new Point(+1, 0));
                    CanSee.Add(fire + new Point(0, -1));
                    CanSee.Add(fire + new Point(0, +1));
                }
            }


        }

        private void SetAffectedSpace(GaslightMap map)
        {
            //if (map.IsPositionAvailable(TilePos + Facing))
                AffectedSpace = TilePos + Facing;
            //else
            //    AffectedSpace = new Point(-1, -1);
        }

        public Point GetCantSeeSpot(GaslightMap map)
        {
            int TimesTried = 0;

            while (TimesTried < 100)
            {
                Point Spot = new Point(new Random().Next(16), new Random().Next(8));
                float DistanceSq = (TilePos.ToVector2() - Spot.ToVector2()).LengthSquared();
                float MinDistance = 6;

                if (map.IsPositionAvailable(Spot) && !CanSee.Contains(Spot) && DistanceSq >= MinDistance * MinDistance)
                    return Spot;

                TimesTried++;
            }

            return Point.Zero;
        }

        public void SwitchCellmateState(CState newState)
        {
            PrevCellmateState = CellmateState;
            CellmateState = newState;
        }

        public void SetMessage(params string[] messageOptions)
        {
            int RandOption = new Random().Next(messageOptions.Length);
            Message = messageOptions[RandOption];
            MessageTransparencyLerp = new LerpF(1f, 0f, 3f);
        }

        public bool ChooseMove(GaslightGame game, GaslightMap map, List<GaslightEntity> entities)
        {
            SetCanSee(map);


            // Stops ai from making a decision every frame while the
            // player is still deciding
            if (MoveChosen)
                return true;

            Point OldTilePos = TilePos;
            Point Steering = new Point(0, 0);

            GaslightEntity Player = entities.Find(e => e.EntityType == Type.Player);
            GaslightEntity Cellmate = entities.Find(e => e.EntityType == Type.Cellmate);
            GaslightEntity Spider = entities.Find(e => e.EntityType == Type.Spider);

            switch (EntityType)
            {
                case Type.Player:

                    // Am I dead?
                    if (map.Fire.Keys.Contains(TilePos))
                    {
                        game.StartRestart();
                        return true;
                    }

                    if (Spider != null && Spider.TilePos == TilePos)
                    {
                        game.StartRestart();
                        return true;
                    }

                    if (TilePos == map.Exit && map.ExitOpened)
                    {
                        game.StartMoveToNextMap();
                    }

                    if (map.PointsAreNeighbors(Player.TilePos, map.OilSource))
                    {
                        Player.AmtOil += 8;
                        if (Player.AmtOil < 8)
                            Player.AmtOil = 8;
                    }

                    if (map.PointsAreNeighbors(Player.TilePos, Cellmate.TilePos))
                    {
                        Player.AmtOil += Cellmate.AmtOil;
                        Cellmate.AmtOil = 0;
                        if (Player.AmtOil < 8)
                            Player.AmtOil = 8;
                    }

                    // Player Steering
                    if (InputManager.Pressed(Keys.W)) Steering.Y -= 1;
                    else if (InputManager.Pressed(Keys.A)) Steering.X -= 1;
                    else if (InputManager.Pressed(Keys.S)) Steering.Y += 1;
                    else if (InputManager.Pressed(Keys.D)) Steering.X += 1;
                    else if (InputManager.JustPressed(Keys.Space))
                    {
                        switch (map.GetTileType(AffectedSpace))
                        {
                            case GaslightMap.TileType.Oil:
                                int Numlit = map.AddFire(AffectedSpace);

                                if (Numlit > 8 && !map.ExitOpened)
                                    Cellmate.SetMessage("Did you skip a spot?", "Are you missing a spot?", "Did it not go all the way?", "Are you ok?", "Do you want me to do it?", "How are you feeling?", "Did you double check?", "You're making a lot of mistakes.");
                                else if (map.ExitOpened && !game.Flashed)
                                    game.StartFlash();

                                if (Cellmate.CellmateState == CState.SeekNearPlayer)
                                    Cellmate.CellmateState = CState.SeekOilReserve;

                                return true;

                            case GaslightMap.TileType.Exit:
                                if (map.ExitOpened)
                                    game.StartMoveToNextMap();
                                else if (!map.DynamitePlaced)
                                {
                                    map.DynamitePlaced = true;
                                    return true;
                                }
                                break;
                        }
                    }
                    else if (InputManager.Pressed(Keys.Space))
                    {
                        switch (map.GetTileType(AffectedSpace))
                        {
                            case GaslightMap.TileType.Floor:
                                if (AmtOil > 0 && !map.Oil.Contains(AffectedSpace))
                                {
                                    map.AddOil(AffectedSpace);
                                    AmtOil -= 1;
                                    return true;
                                }
                                break;

                            case GaslightMap.TileType.Wall:

                                break;
                        }
                    }
            
                    else if (InputManager.JustReleased(Keys.Q))
                        return true;
                    else if (InputManager.JustReleased(Keys.E) && Cellmate.CellmateState != CState.FleeSpider)
                    {
                        if (Cellmate.CellmateState != CState.SeekNearPlayer)
                        {
                            Cellmate.SwitchCellmateState(CState.SeekNearPlayer);
                            Cellmate.SetMessage("Coming!", "On my way!", "Be right there!");
                        }
                        else
                        {
                            Cellmate.SwitchCellmateState(CState.SeekOilReserve);
                            Cellmate.SetMessage("Where was that oil?");
                        }
                    }

                    break;

                case Type.Cellmate:

                    // Cellmate Steering
                    if (map.ExitOpened && Cellmate.CellmateState != CState.SeekNearPlayer)
                    {
                        Cellmate.SwitchCellmateState(CState.SeekNearPlayer);
                        Cellmate.SetMessage("Oh! I guess we have to go.");
                    }

                    if (Spider != null && CanSee.Contains(Spider.TilePos) && CellmateState != CState.FleeSpider)
                    {
                        // Find a spot that's far from spider
                        Point FurthestTile = Point.Zero;
                        float FurthestDistanceSq = -float.MaxValue;
                        for (int tile = 0; tile < map.Tiles.Count; tile++)
                        {
                            Point ThisTilePos = new Point(tile % map.MapTileSize.X, tile / map.MapTileSize.X);

                            if (map.IsPositionAvailable(ThisTilePos))
                            {
                                float DistanceSq = (Cellmate.TilePos - ThisTilePos).ToVector2().Length();
                                if (DistanceSq > FurthestDistanceSq)
                                {
                                    FurthestDistanceSq = DistanceSq;
                                    FurthestTile = ThisTilePos;
                                }
                            }
                        }
                        Cellmate.Target = FurthestTile;
                        Cellmate.SwitchCellmateState(CState.FleeSpider);
                        Cellmate.SetMessage("Ah!", "Spider!", "Ew ew ew!");
                    }

                    if (map.Fire.ContainsKey(TilePos))
                    {
                        game.StartRestart();
                    }

                    switch (CellmateState)
                    {
                        case CState.SeekOilReserve:
                            Path = map.GetPath(TilePos, map.OilSource, entities);
                            if (map.OilSource != AffectedSpace &&
                                Path.Count > 0)
                                Steering = Path.Peek() - TilePos;
                            else
                            {
                                AmtOil += 8;
                                Cellmate.SwitchCellmateState(CState.SeekPlayer);
                                Cellmate.SetMessage("Got some oil!", "More oil coming!", "Refill on the way!");
                                return true;
                            }

                            break;

                        case CState.SeekPlayer:

                            if (map.PointsAreNeighbors(TilePos, Player.TilePos) ||
                                map.PointsAreNeighbors(TilePos, Player.AffectedSpace))
                            {
                                bool SeekPlacedOil = map.Oil.Count > 5 && new Random().Next(3) <= 1;

                                if (SeekPlacedOil)
                                {
                                    // Find a random tile to kick dirt on
                                    int NumTimesTried = 0;
                                    while (NumTimesTried < map.Oil.Count * 2)
                                    {
                                        int RandOil = new Random().Next(map.Oil.Count / 2);
                                        Point OilPoint = map.Oil[RandOil];
                                        if (!Player.CanSee.Contains(OilPoint))
                                        {
                                            Target = map.Oil[RandOil];
                                            Cellmate.SetMessage("There you go!", "Here you go.");
                                            Cellmate.SwitchCellmateState(CState.SeekPlacedOil);
                                            return true;
                                        }

                                        NumTimesTried++;
                                    }
                                }

                                float PDistToOilSourceSq = (Player.TilePos - map.OilSource).ToVector2().LengthSquared();
                                float WanderDist = 5f;
                                if (PDistToOilSourceSq < WanderDist * WanderDist ||
                                    new Random().Next(3) == 0)
                                {
                                    List<Point> FarEnoughAway = new List<Point>();
                                    float MinDistance = 8f;
                                    for (int tile = 0; tile < map.Tiles.Count; tile++)
                                    {
                                        Point Spot = new Point(tile % map.MapTileSize.X, tile / map.MapTileSize.X);
                                        if ((Spot - Player.TilePos).ToVector2().LengthSquared() > MinDistance * MinDistance &&
                                            map.IsPositionAvailable(Spot))
                                            FarEnoughAway.Add(Spot);
                                    }

                                    Cellmate.SetMessage("I'm gonna wander a bit.");
                                    Cellmate.SwitchCellmateState(CState.Wander);
                                    Target = FarEnoughAway[new Random().Next(FarEnoughAway.Count)];
                                }
                                else
                                {
                                    Cellmate.SetMessage("There you go!", "Hope this helps!", "Here you go, partner.");
                                    Cellmate.SwitchCellmateState(CState.SeekOilReserve);
                                }


                                return true;
                            }
                            else
                            {
                                Path = map.GetPath(TilePos, Player.TilePos, entities);
                                if (Path.Count > 0)
                                    Steering = Path.Peek() - TilePos;
                                else
                                    return true;

                            }

                            break;

                        case CState.SeekPlacedOil:

                            if (map.Fire.ContainsKey(Target) ||
                                Player.CanSee.Contains(Target))
                            {
                                Cellmate.SwitchCellmateState(CState.SeekOilReserve);
                                Cellmate.SetMessage("Oh hello!", "Didn't see you there!", "Oh! You startled me.");
                                return true;
                            }

                            Path = map.GetPath(TilePos, Target, entities);

                            if (Path.Count > 0)
                            {
                                Steering = Path.Peek() - TilePos;
                            }
                            else
                            {
                                map.Oil.Remove(Target);
                                Cellmate.SwitchCellmateState(CState.SeekOilReserve);
                                return true;
                            }

                            break;

                        case CState.MoveOutOfWay:
                            if (Path.Count > 0)
                            {
                                Point OutOfWayPos = Path.Dequeue();
                                Steering = OutOfWayPos - TilePos;
                                if (Facing != Steering)
                                    Facing = Steering;
                            }

                            Cellmate.SwitchCellmateState(Cellmate.PrevCellmateState);
                            break;

                        case CState.SeekNearPlayer:
                            List<Point> SafeSpots = new List<Point>();
                            float MaxDistance = 2;
                            for (int tile = 0; tile < map.Tiles.Count; tile++)
                            {
                                Point Spot = new Point(tile % map.MapTileSize.X, tile / map.MapTileSize.X);
                                if ((Player.TilePos - Spot).ToVector2().LengthSquared() < MaxDistance * MaxDistance &&
                                    !map.Oil.Contains(Spot) &&
                                    map.IsPositionAvailable(Spot))
                                    SafeSpots.Add(Spot);
                            }

                            if (SafeSpots.Count == 0)
                                SafeSpots.Add(Player.TilePos);

                            Path = map.GetPath(TilePos, SafeSpots[0], entities);

                            if (Path.Count > 0)
                                Steering = Path.Dequeue() - TilePos;
                            else
                                return true;
                            break;

                        case CState.FleeSpider:
                            Path = map.GetPath(TilePos, Target, entities);

                            float DistSqToSpider = (Cellmate.TilePos - Spider.TilePos).ToVector2().LengthSquared();
                            float SafeDistSq = 6 * 6;

                            if (DistSqToSpider > SafeDistSq || Spider.TilePos == new Point(-1, -1))
                            {
                                Cellmate.SwitchCellmateState(CState.Wander);
                                Cellmate.SetMessage("*Phew*", "That was scary.", "I hate spiders.", "Sorry about that.", "Is it safe?");
                                return true;
                            }
                            else if (Path.Count > 0)
                                Steering = Path.Dequeue() - TilePos;
                            else
                                return true;

                            break;

                        case CState.Wander:
                            Path = map.GetPath(TilePos, Target, entities);

                            if (Path.Count > 0)
                                Steering = Path.Dequeue() - TilePos;
                            else
                            {
                                SwitchCellmateState(CState.SeekOilReserve);
                                SetMessage("I'll get you some oil.", "Need any oil?", "Nothing interesting here.");
                            }

                            break;
                    }

                    if (Cellmate.MessageTransparencyLerp.CurrentPosition == 0f &&
                        new Random().Next(100) == 0 &&
                        (Cellmate.CellmateState != CState.FleeSpider))
                        Cellmate.SetMessage(
                            "This is so fun",
                            "You're my best friend.",
                            "I feel so close to you.",
                            "I'm sad you want to leave.",
                            "Prison wasn't so bad with you.",
                            "I like being around you.",
                            "I'll miss you when you're gone.",
                            "Do we have to go? Sorry.",
                            "I like you.",
                            "We're two friends on an adventure."
                            );

                    break;

                case Type.Spider:

                    if (TilePos == new Point(-1, -1))
                        return true;

                    if (map.Fire.Keys.Contains(TilePos))
                    {
                        TilePos = new Point(-1, -1);
                        ScreenPos = TilePos.ToVector2() * 16 + map.DrawOffset.ToVector2();
                        MoveLerp = new LerpV(ScreenPos, ScreenPos, 0f);
                        game.UntilSpiderSpawn = 100;

                        return true;
                    }

                    if (SpiderState == SState.Wander && Spider.CanSee.Contains(Player.TilePos))
                    {
                        SpiderState = SState.SeekPlayer;
                        Spider.SetMessage("*Skree!!*");
                    }
                    else if (SpiderState == SState.SeekPlayer && !CanSee.Contains(Player.TilePos))
                        SpiderState = SState.Wander;

                    if (SpiderState == SState.SeekPlayer)
                        Path = map.GetPath(TilePos, Player.TilePos, entities);
                    else
                    {
                        int Rand = new Random().Next(4);
                        Point NeighborOffset = new Point(0, 0);
                        if (Rand == 0) NeighborOffset = new Point(-1, 0);
                        else if (Rand == 1) NeighborOffset = new Point(+1, 0);
                        else if (Rand == 2) NeighborOffset = new Point(0, -1);
                        else if (Rand == 3) NeighborOffset = new Point(0, +1);
                        Path = new Queue<Point>();
                        Path.Enqueue(TilePos + NeighborOffset);
                    }

                    if (Path.Count > 0)
                        Steering = Path.Dequeue() - TilePos;
                    else
                        return true;

                    break;
            }

            TilePos += Steering;

            // If input recieved
            if (TilePos != OldTilePos)
            {
                Point Direction = TilePos - OldTilePos;

                // If not facing this way, just turn
                if (Direction != Facing &&
                    !(EntityType == Type.Player && InputManager.Pressed(Keys.LeftShift)))
                {
                    TilePos = OldTilePos;
                    Facing = Direction;
                }

                // Otherwise, if position is available, move to it
                if (map.IsPositionAvailable(TilePos))
                {
                    // If player is trying to move into cellmate space,
                    // Move cellmate out of the way
                    if (EntityType == Type.Player && TilePos == Cellmate.TilePos)
                    {
                        // Find a spot out of the way
                        List<Point> NeighborOffsets = new List<Point>();
                        NeighborOffsets.Add(new Point(-Facing.Y, Facing.X));
                        NeighborOffsets.Add(new Point(Facing.Y, -Facing.X));

                        List<Point> SideNeighbors = new List<Point>();
                        foreach (Point neighborOffset in NeighborOffsets)
                        {
                            Point CellmateNeighbor = Cellmate.TilePos + neighborOffset;

                            if (map.IsPositionAvailable(CellmateNeighbor) && CellmateNeighbor != OldTilePos)
                                SideNeighbors.Add(CellmateNeighbor);
                        }

                        Queue<Point> NewPath = new Queue<Point>();
                        Point ChosenNeighbor = Point.Zero;
                        if (SideNeighbors.Count > 0)
                        {
                            int RandNeighborIdx = new Random().Next(SideNeighbors.Count);
                            ChosenNeighbor = SideNeighbors[RandNeighborIdx];
                        }
                        else
                        {
                            ChosenNeighbor = Cellmate.TilePos + Player.Facing;
                        }

                        NewPath.Enqueue(ChosenNeighbor);
                        Cellmate.Path = NewPath;
                        Cellmate.SwitchCellmateState(CState.MoveOutOfWay);
                    }

                    Vector2 TargetPixelPos = TilePos.ToVector2() * 16;
                    MoveLerp = new LerpV(ScreenPos, TargetPixelPos, Speed);
                    LeftFoot = !LeftFoot;
                    Moving = true;
                    MoveChosen = true;

                    if (Path.Count > 0 && TilePos == Path.Peek())
                        Path.Dequeue();
                }
                // Can't turn? Just reset tilepos
                else
                {
                    TilePos = OldTilePos;
                }

                SetAffectedSpace(map);

                return true;
            }

            return false;
        }

        public bool MovementUpdate(GaslightMap map, float deltaTime)
        {
            MoveLerp.Update(deltaTime);
            ScreenPos = MoveLerp.CurrentPosition;
            if (MoveLerp.Reached)
            {
                Moving = false;
                MoveChosen = false;
                if (EntityType == Type.Player)
                    SetCanSee(map);
                return true;
            }
            return false;
        }

        public void FrameUpdate(float deltaTime)
        {
            MessageTransparencyLerp.Update(deltaTime);
        }

        public void Draw(Camera2D camera, GaslightMap map, SpriteBatch spriteBatch)
        {
            // Sprite
            Point SourcePos =
                (Facing == new Point(0, -1)) ? new Point(0, 0) :
                (Facing == new Point(0, +1)) ? new Point(0, 32) :
                new Point(0, 64);
            bool Flip = Facing == new Point(+1, 0);

            if (Moving)
                SourcePos.X += (LeftFoot) ? 32 : 64;

            Point SpriteOffset = new Point(-8, -12);

            spriteBatch.Draw(
                GameContent.Sprites[SpriteName],
                new Rectangle(ScreenPos.ToPoint() + SpriteOffset - camera.Position.ToPoint(), new Point(32, 32)),
                new Rectangle(SourcePos, new Point(32, 32)),
                Color.White,
                0f, Vector2.Zero,
                (Flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);


        }

        public void DrawDarkness(Camera2D camera, GaslightMap map, SpriteBatch spriteBatch)
        {
            if (EntityType != Type.Player)
                return;

            for (int x = 0; x < map.MapTileSize.X; x++)
                for (int y = 0; y < map.MapTileSize.Y; y++)
                {
                    Point Spot = new Point(x, y);

                    Func<Point, bool> isNeighbor = (Spot) =>
                        CanSee.Contains(Spot + new Point(-1, 0)) ||
                        CanSee.Contains(Spot + new Point(+1, 0)) ||
                        CanSee.Contains(Spot + new Point(0, -1)) ||
                        CanSee.Contains(Spot + new Point(0, +1));

                    if (!CanSee.Contains(Spot))
                    {
                        Point PixelPos = new Point(Spot.X * 16, Spot.Y * 16);
                        Rectangle DrawRect = new Rectangle(
                            PixelPos - camera.Position.ToPoint(), 
                            new Point(16, 16));
                        Rectangle SourceRect =
                            (isNeighbor(Spot)) ? 
                                new Rectangle(new Point(0, 64), new Point(16, 16)) :
                                new Rectangle(new Point(0, 48), new Point(16, 16));
                        
                        spriteBatch.Draw(GameContent.Sprites["GaslightingTiles"], DrawRect, SourceRect, Color.White);
                    }
                }
        }

        public void DrawDebug(Camera2D camera, GaslightMap map, SpriteBatch spriteBatch)
        {
            // Debug VisibleSpace
            //if (EntityType == Type.Player)
            //{
            //    foreach (Point point in CanSee)
            //    {
            //        DrawPrimitives2D.Rectangle(
            //            new Rectangle(new Point(point.X * 16, point.Y * 16) + map.DrawOffset, new Point(16, 16)),
            //            Color.White, spriteBatch);
            //    }
            //}


            ////Debug CurrentSpace
            //DrawPrimitives2D.Rectangle(
            //    new Rectangle(new Point(TilePos.X * 16, TilePos.Y * 16)- camera.Position.ToPoint(), new Point(16, 16)),
            //    Color.Red, spriteBatch);

            //// Debug AffectedSpace
            //DrawPrimitives2D.Rectangle(
            //    new Rectangle(new Point(AffectedSpace.X * 16, AffectedSpace.Y * 16) - camera.Position.ToPoint(), new Point(16, 16)),
            //    Color.Green, spriteBatch);

            //// Debug Path
            //foreach (Point point in Path)
            //{
            //    DrawPrimitives2D.FilledCircle(
            //        new Point(point.X * 16, point.Y * 16) + new Point(8, 8) - camera.Position.ToPoint(), 4, Color.Blue, spriteBatch);
            //}

            // Debug Text
            //if (EntityType == Type.Player)
            //{
            //    string TileType = map.GetTileType(AffectedSpace).ToString();
            //    string AmtOilStr = AmtOil.ToString();
            //    string DebugText = TileType + '\n' + AmtOilStr;
            //    spriteBatch.DrawString(
            //        GameContent.Font, DebugText, new Vector2(0, 0), Color.White);
            //}
            //else
            //{
            //    string State =
            //        (EntityType == Type.Cellmate) ? CellmateState.ToString() : SpiderState.ToString();
            //    spriteBatch.DrawString(
            //        GameContent.Font, State, ScreenPos + new Vector2(-12, -16) - camera.Position, Color.White);
            //}

            // Message

            if (TilePos == new Point(-1, -1))   // dont draw dead spider text
                return;

            Color MessageColor = new Color(MessageTransparencyLerp.CurrentPosition, MessageTransparencyLerp.CurrentPosition, MessageTransparencyLerp.CurrentPosition, MessageTransparencyLerp.CurrentPosition);
            Vector2 MessageSize = GameContent.Font.MeasureString(Message);
            Vector2 MessageDrawOffset = new Vector2(0, -12);
            Vector2 MessageDrawPos = ScreenPos + MessageDrawOffset - new Vector2(MathF.Floor(MessageSize.X / 2f), 0) - camera.Position;
            if (MessageDrawPos.X < 0) 
                MessageDrawPos.X = 0;
            if (MessageDrawPos.Y < 0) 
                MessageDrawPos.Y = 0;
            if (MessageDrawPos.X + MessageSize.X > GraphicsGlobals.ScreenSize.X)
                MessageDrawPos.X = GraphicsGlobals.ScreenSize.X - MessageSize.X;
            if (MessageDrawPos.Y + MessageSize.Y > GraphicsGlobals.ScreenSize.Y)
                MessageDrawPos.Y = GraphicsGlobals.ScreenSize.Y - MessageSize.Y;
            MessageDrawPos.X = (int)MessageDrawPos.X;
            MessageDrawPos.Y = (int)MessageDrawPos.Y;
            spriteBatch.DrawString(
                GameContent.Font,
                Message,
                MessageDrawPos,
                MessageColor);

        }
    }
}
