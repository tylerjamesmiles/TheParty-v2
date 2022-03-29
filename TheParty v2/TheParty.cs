using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TheParty_v2
{
    class TheParty : Game
    {
        private GraphicsDeviceManager GraphicsManager;
        private SpriteBatch SpriteBatch;

        private FixedResolutionTarget Target;

        public StateMachine<TheParty> StateMachine;
        public CommandQueue<TheParty> CommandQueue;
        public OgmoTileMap CurrentMap;

        public Player Player;
        public Timer EventsCanHappenTimer;
        public Camera2D Camera;
        public Dictionary<Vector2, SpriteAnimation> CollectionAnimations;

        public bool BeFaded;

        public TheParty()
        {
            GraphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Title = "The Party 1/1/22 Demo";
            Window.AllowUserResizing = true;

            EventsCanHappenTimer = new Timer(0.5f);

            BeFaded = false;

            CollectionAnimations = new Dictionary<Vector2, SpriteAnimation>();
        }

        protected override void Initialize()
        {
            DrawPrimitives2D.Initialize(GraphicsDevice);
            Camera = new Camera2D();

            IsMouseVisible = true;
            GraphicsManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            GraphicsManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            GraphicsManager.IsFullScreen = false;
            GraphicsManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            GraphicsGlobals.Setup(GraphicsDevice);

            Target = new FixedResolutionTarget(
                GraphicsGlobals.ScreenSize.X, 
                GraphicsGlobals.ScreenSize.Y, 
                GraphicsDevice, true);

            Target.Enable();

            GameContent.LoadContent(Content);
            GameContent.LoadData(); // sets everything to blank slate values
                                    // on Load, more specific data will be drawn

            StateMachine = new StateMachine<TheParty>();
            StateMachine.SetNewCurrentState(this, new GameStateTitle());

            CommandQueue = new CommandQueue<TheParty>();

        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.SetNewState();

            float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GameContent.Update(DeltaTime);
            StateMachine.Update(this, DeltaTime);
            CommandQueue.Update(this, DeltaTime);

            var ToErase = new List<KeyValuePair<Vector2, SpriteAnimation>>();
            foreach (var animation in CollectionAnimations)
            {
                animation.Value.Update(DeltaTime);
                if (animation.Value.NumTimesLooped > 0)
                    ToErase.Add(animation);
            }
            ToErase.ForEach(a => CollectionAnimations.Remove(a.Key));

            InputManager.SetOldState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Target.Begin();

            SpriteBatch.Begin();

            // ~ ~ ~

            StateMachine.Draw(this, SpriteBatch);

            if (BeFaded)
                SpriteBatch.Draw(
                    GameContent.Sprites["FadeIn"],
                    new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                    new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                    Color.White);

            CommandQueue.Draw(this, SpriteBatch);

            foreach (var animation in CollectionAnimations)
            {
                Vector2 DrawPos = animation.Key + new Vector2(-8, -48) - Camera.Position;
                Rectangle DrawRect = new Rectangle(DrawPos.ToPoint(), new Point(16, 32));
                animation.Value.Draw("CollectAnimations", DrawRect, SpriteBatch);
            }

            // ~ ~ ~

            SpriteBatch.End();
            Target.End();

            Target.Draw(SpriteBatch);

            base.Draw(gameTime);
        }
    }
}
