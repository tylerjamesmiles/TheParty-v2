using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TheParty_v2
{
    class TheParty : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;

        private FixedResolutionTarget Target;

        public StateMachine<TheParty> StateMachine;
        public CommandQueue<TheParty> CommandQueue;
        public OgmoTileMap CurrentMap;

        public Player Player;
        public Timer EventsCanHappenTimer;


        int HP = 10;

        public TheParty()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            EventsCanHappenTimer = new Timer(0.5f);
        }

        protected override void Initialize()
        {
            DrawPrimitives2D.Initialize(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            GraphicsGlobals.Setup(GraphicsDevice);

            Target = new FixedResolutionTarget(160, 140, GraphicsDevice);
            Target.Enable();

            GameContent.Load(Content);



            StateMachine = new StateMachine<TheParty>();
            StateMachine.SetNewCurrentState(this, new GameStateTitle());

            CommandQueue = new CommandQueue<TheParty>();

            CurrentMap = GameContent.Maps["FarawayHeightsBase"];
            Player = new Player(new Vector2(16, 30) * 16 + new Vector2(8, 8));
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.SetNewState();

            float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            StateMachine.Update(this, DeltaTime);
            CommandQueue.Update(this, DeltaTime);

            InputManager.SetOldState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            Target.Begin();

            SpriteBatch.Begin();

            // ~ ~ ~

            StateMachine.Draw(this, SpriteBatch);
            CommandQueue.Draw(this, SpriteBatch);

            // ~ ~ ~

            SpriteBatch.End();
            Target.End();

            Target.Draw(SpriteBatch);

            base.Draw(gameTime);
        }
    }
}
