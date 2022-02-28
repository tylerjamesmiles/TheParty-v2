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
    class GaslightGame : State<CommandGaslight>
    {
        GaslightMap Map;
        List<GaslightEntity> Entities;
        Camera2D Camera;
        int MapNumber;

        enum State { Choose, Move, DoStuff }
        State CurrentState;

        LerpF FadeLerp;
        bool FadingIn;
        bool FadingOut;
        bool Flashing;
        public bool Flashed;
        bool MovingToNextMap;

        public int UntilSpiderSpawn = 0;
        public GaslightGame()
        {
            MapNumber = 3;
            MovingToNextMap = false;
        }

        public GaslightEntity GetPlayer() => Entities.Find(e => e.EntityType == GaslightEntity.Type.Player);

        public override void Enter(CommandGaslight client)
        {
            Instantiate();
        }

        public void StartMoveToNextMap()
        {
            FadeLerp = new LerpF(0f, 1f, 1f);
            MovingToNextMap = true;
            FadingOut = true;
        }

        public void StartRestart()
        {
            FadeLerp = new LerpF(0f, 1f, 1f);
            MovingToNextMap = false;
            FadingOut = true;
        }

        public void StartFlash()
        {
            Flashing = true;
            FadeLerp = new LerpF(1f, 0f, 1f);
        }

        public void MoveToNextMap()
        {
            MapNumber++;
            Instantiate();
        }

        private void Instantiate()
        {
            Map = new GaslightMap("GaslightCave" + MapNumber.ToString());
            Entities = new List<GaslightEntity>();

            var Player = new GaslightEntity(Map, GaslightEntity.Type.Player, Map.PlayerStartPos);
            Entities.Add(Player);

            if (Map.CellmateStartPos != new Point(0, 0))
            {
                var Cellmate = new GaslightEntity(Map, GaslightEntity.Type.Cellmate, Map.CellmateStartPos);
                Entities.Add(Cellmate);
            }

            if (MapNumber >= 4)
                Entities.Add(new GaslightEntity(Map, GaslightEntity.Type.Spider, Player.GetCantSeeSpot(Map)));

            CurrentState = State.Choose;

            Camera = new Camera2D();

            FadeLerp = new LerpF(0f, 1f, 1f);
            FadingIn = true;
            FadingOut = false;
            Flashing = false;
            Flashed = false;
        }

        public override void Update(CommandGaslight client, float deltaTime)
        {
            GaslightEntity Player = Entities.Find(e => e.EntityType == GaslightEntity.Type.Player);
            GaslightEntity Spider = Entities.Find(e => e.EntityType == GaslightEntity.Type.Spider);

            Entities.ForEach(e => e.FrameUpdate(deltaTime));

            Camera.Update(Map.MapPixelSize, Player.ScreenPos);

            if (FadingIn || FadingOut || Flashing)
            {
                FadeLerp.Update(deltaTime);
                if (FadeLerp.Reached)
                {
                    if (FadingIn)
                        FadingIn = false;
                    else if (FadingOut)
                    {
                        FadingOut = false;

                        if (MovingToNextMap)
                            MoveToNextMap();
                        else
                            Instantiate();
                    }
                    else if (Flashing)
                        Flashing = false;
                }
                return;
            }

            switch (CurrentState)
            {
                case State.Choose:
                    if (Entities.TrueForAll(e => e.ChooseMove(this, Map, Entities)))
                    {
                        if (Entities.Exists(e => e.Moving))
                            CurrentState = State.Move;
                    }

                    break;

                case State.Move:
                    if (Entities.TrueForAll(e => e.MovementUpdate(Map, deltaTime)))
                        CurrentState = State.DoStuff;

                    break;

                case State.DoStuff:

                    Map.UpdateFire();

                    // Restart Spider
                    if (Spider != null && Spider.TilePos == new Point(-1, -1))
                    {
                        UntilSpiderSpawn--;

                        if (UntilSpiderSpawn == 0)
                        {
                            Spider.TilePos = Player.GetCantSeeSpot(Map);
                            Spider.ScreenPos = Spider.TilePos.ToVector2() * 16 - Camera.Position;
                        }
                    }

                    CurrentState = State.Choose;

                    break;
            }

            if (InputManager.JustReleased(Keys.R))
                StartRestart();
        }

        public override void Draw(CommandGaslight client, SpriteBatch spriteBatch)
        {
            Map.Draw(Camera, spriteBatch);

            Entities.ForEach(e => e.Draw(Camera, Map, spriteBatch));
            Entities.ForEach(e => e.DrawDarkness(Camera, Map, spriteBatch));
            Entities.ForEach(e => e.DrawDebug(Camera, Map, spriteBatch));

            if (FadingIn || FadingOut || Flashing)
            {
                int FadeFrame = (int)MathF.Floor(FadeLerp.CurrentPosition * 4);
                int FadeX = FadeFrame * GraphicsGlobals.ScreenSize.X;
                Point SourcePos = new Point(FadeX, 0);

                spriteBatch.Draw(
                    (FadingIn) ? GameContent.Sprites["FadeIn"] : GameContent.Sprites["FadeOut"],
                    new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                    new Rectangle(SourcePos, GraphicsGlobals.ScreenSize),
                    Color.White);
            }
        }

        public override void Exit(CommandGaslight client)
        {

        }
    }
}
