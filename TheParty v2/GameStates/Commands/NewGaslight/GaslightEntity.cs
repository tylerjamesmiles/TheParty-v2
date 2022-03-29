using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheParty_v2
{
    abstract class GaslightEntity
    {
        protected Transform2D Transform;
        protected Movement2D Movement;
        public FourDirSprite2D Sprite;
        public enum Type { Player, Spider, Oil, Fire, Coin, Refill, Health, Stairs }

        protected Vector2 SteeringForce { set; get; }
        public Type EntityType { get; private set; }
        public Vector2 Position => Transform.Position;
        public Vector2 Velocity => Movement.Velocity;
        public float DistToPointSq(Vector2 point) => (point - Transform.Position).LengthSquared();
        public bool PointInRadius(Vector2 point) => DistToPointSq(point) < Transform.BoundingRadius * Transform.BoundingRadius;
        public bool IsDead { get; protected set; }
        public void Kill() => IsDead = true;
        bool Solid;


        protected GaslightEntity(Vector2 pos, float radius, bool solid, float mass, float maxSpeed, string spriteName, Point spriteDrawOffset, Type entityType)
        {
            Transform = new Transform2D(pos, radius);
            Movement = new Movement2D(mass, maxSpeed);
            Sprite = new FourDirSprite2D(spriteName, spriteDrawOffset);
            EntityType = entityType;
            SteeringForce = Vector2.Zero;
            IsDead = false;
            Solid = solid;
        }

        public virtual void AI(CommandGaslight game, float deltaTime) { }
        public virtual List<GaslightEntity> AddEntities(CommandGaslight game, Vector2 cameraPos) { return new List<GaslightEntity>(); }

        public virtual void Update(CommandGaslight game, float deltaTime)
        {
            Movement.Update(SteeringForce, deltaTime);
            Transform.Update(Movement.Velocity, game.CollisionBoxes, new List<Transform2D>(), Solid);
            Sprite.Update(Movement.Velocity, deltaTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            //DrawPrimitives2D.FilledCircle(Transform.Position - cameraPos, Transform.BoundingRadius, Color.Red, spriteBatch);
            Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
        }
    }

    class GaslightHealth
    {
        public int HP { get; private set; }
        HeartsIndicator Hearts;
        Timer CanBeDamagedTimer;
        Timer FlickerTimer;

        public bool CanBeDamaged => CanBeDamagedTimer.TicsSoFar > 1;

        public GaslightHealth(int startingHealth)
        {
            HP = startingHealth;
            Hearts = new HeartsIndicator(startingHealth, 0, 0);
            Hearts.SetMax(10);
            CanBeDamagedTimer = new Timer(0.4f);
            FlickerTimer = new Timer(0.05f);
        }

        public void TakeDamage(int amt)
        {
            HP += amt;
            CanBeDamagedTimer.Reset();
        }

        public void Update(Vector2 entityPos, float deltaTime)
        {
            // If too close to fire, take damage
            CanBeDamagedTimer.Update(deltaTime);
            FlickerTimer.Update(deltaTime);

            // Update Hearts
            Hearts.Origin = entityPos.ToPoint() + new Point(0, -32);
            Hearts.SetHP(HP);
            Hearts.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            Hearts.Draw(spriteBatch, cameraPos);
        }
    }

    class GaslightEntityPlayer : GaslightEntity
    {
        GaslightHealth Health;
        ControllerSteering2D ControllerSteering;
        int AmtOil;
        int AmtMoney;
        Timer PlaceTimer;

        public GaslightEntityPlayer(Vector2 startPos, int startOil, int startHealth, int startMoney)
            : base(startPos, 5f, true, 1f, 27f, "CharacterSmallBase", new Point(-16, -23), Type.Player)
        {
            Health = new GaslightHealth(startHealth);
            ControllerSteering = new ControllerSteering2D(27f);
            AmtOil = startOil;
            AmtMoney = startMoney;
            PlaceTimer = new Timer(0.05f);
        }

        MouseState CurrentMouseState;
        MouseState OldMouseState;

        public Vector2 MousePos()
        {
            Vector2 PlacePos =
                CurrentMouseState.Position.ToVector2() / FixedResolutionTarget.PixelPerfectScale -
                FixedResolutionTarget.Offset / FixedResolutionTarget.PixelPerfectScale;
            return PlacePos;
        }

        public override List<GaslightEntity> AddEntities(CommandGaslight game, Vector2 cameraPos)
        {
            List<GaslightEntity> NewEntities = new List<GaslightEntity>();

            //Vector2 PlacePos = Transform.Position + Facing * 24f;
            CurrentMouseState = Mouse.GetState();
            Vector2 PlacePos = MousePos() + cameraPos;
            bool MousePressed = CurrentMouseState.LeftButton == ButtonState.Pressed;
            bool MouseJustPressed = OldMouseState != null && OldMouseState.LeftButton == ButtonState.Released && MousePressed;

            List<GaslightEntity> Oils = game.Oils;

            if (Oils.Exists(e => e.PointInRadius(PlacePos)))
            {
                if (MouseJustPressed)
                {
                    Oils.Sort((o1, o2) => o1.DistToPointSq(PlacePos) < o2.DistToPointSq(PlacePos) ? -1 : 1);
                    GaslightEntity ClosestOil = Oils[0];
                    Vector2 ClosestOilPos = ClosestOil.Position;

                    if (!game.Fires.Exists(f => f.PointInRadius(ClosestOilPos)))
                    {
                        ClosestOil.Kill();

                        NewEntities.Add(new GaslightEntityFire(ClosestOilPos));

                        GameContent.SoundEffects["Fire"].Play();
                    }
                }
            }
            else if (MousePressed && game.IsSpaceAvailable(PlacePos) && AmtOil > 0 && PlaceTimer.TicThisFrame &&
                !game.Fires.Exists(f => f.PointInRadius(PlacePos)))
            {
                NewEntities.Add(new GaslightEntityOil(PlacePos));
                AmtOil--;
            }
            

            OldMouseState = CurrentMouseState;

            return NewEntities;
        }

        public override void AI(CommandGaslight game, float deltaTime)
        {
            ControllerSteering.Update();
            SteeringForce = ControllerSteering.SteeringForce;
        }

        public override void Update(CommandGaslight game, float deltaTime)
        {
            // Take damage near fire
            if (Health.CanBeDamaged &&
                game.Fires.Exists(f => f.PointInRadius(Position)))
            {
                Health.TakeDamage(-1);
                GameContent.SoundEffects["Hit"].Play();
            }

            // Take damage near spiders
            if (Health.CanBeDamaged &&
                game.Spiders.Exists(s => s.PointInRadius(Position)))
            {
                Health.TakeDamage(-1);
                GameContent.SoundEffects["Hit"].Play();

            }

            Health.Update(Position, deltaTime);

            if (Health.HP == 0)
                IsDead = true;

            PlaceTimer.Update(deltaTime);

            // Gather Money
            if (game.Coins.Exists(c => c.PointInRadius(Position)))
            {
                GaslightEntity Coin = game.Coins.Find(c => c.PointInRadius(Position));
                Coin.Kill();
                AmtMoney++;
                GameContent.SoundEffects["Coin"].Play(0.5f, 0f, 0f);
            }

            // Gather Refill
            if (game.Refills.Exists(c => c.PointInRadius(Position)))
            {
                GaslightEntity Refill = game.Refills.Find(c => c.PointInRadius(Position));
                Refill.Kill();
                AmtOil += 5 + new Random().Next(15);
                GameContent.SoundEffects["MenuSelect"].Play();
            }

            // Health
            if (game.Health.Exists(c => c.PointInRadius(Position)))
            {
                GaslightEntity HealthPickup = game.Health.Find(c => c.PointInRadius(Position));
                HealthPickup.Kill();
                Health.TakeDamage(+1);
                GameContent.SoundEffects["MenuSelect"].Play();
            }

            // New level
            if (game.Stairs.PointInRadius(Position))
            {
                game.GenerateNewMap(Health.HP, AmtMoney, AmtOil);
                GameContent.SoundEffects["Stairs"].Play();
            }

            Sprite.LockFacing = InputManager.Pressed(Keys.LeftShift);

            //if (!Sprite.LockFacing)
            //    Facing = Movement.Heading;

            // Move
            base.Update(game, deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            Health.Draw(spriteBatch, cameraPos);

            if (AmtOil > 0)
            {
                Vector2 PlacePos = MousePos();
                spriteBatch.Draw(
                    GameContent.Sprites["GaslightingTiles"],
                    new Rectangle(PlacePos.ToPoint() + new Point(-8, -8), new Point(16, 16)),
                    new Rectangle(new Point(96, 32), new Point(16, 16)),
                    Color.White);
            }

            base.Draw(spriteBatch, cameraPos);

            spriteBatch.DrawString(
                GameContent.Font, "Oil: " + AmtOil.ToString(), Vector2.Zero, Color.White);

            spriteBatch.DrawString(
                GameContent.Font, "Money: " + AmtMoney.ToString(), new Vector2(0, 10), Color.White);
        }
    }

    class GaslightEntitySpider : GaslightEntity
    {
        GaslightHealth Health;
        enum SpiderState { Wander, Chase }
        SpiderState CurrentState;

        Timer MoveTimer;
        Vector2 WanderDir;

        public GaslightEntitySpider(Vector2 startPos)
            : base(startPos, 5f, true, 1f, 40f, "GaslightSpider", new Point(-16, -16), Type.Spider)
        {
            Health = new GaslightHealth(1 + new Random().Next(3));
            CurrentState = SpiderState.Wander;

            MoveTimer = new Timer(0.2f);
        }

        public override List<GaslightEntity> AddEntities(CommandGaslight game, Vector2 cameraPos)
        {
            List<GaslightEntity> Result = new List<GaslightEntity>();

            if (Health.HP == 0)
            {
                IsDead = true;
                GameContent.SoundEffects["SpiderScream"].Play(0.02f, 0f, 0f);

                int Drop = new Random().Next(3);
                switch (Drop)
                {
                    case 0: Result.Add(new GaslightEntityCoin(Transform.Position)); break;
                    case 1: Result.Add(new GaslightEntityRefill(Transform.Position)); break;
                    case 2: Result.Add(new GaslightEntityHealth(Transform.Position)); break;
                }
            }

            return Result;
        }

        public override void AI(CommandGaslight game, float deltaTime)
        {
            GaslightEntity Player = game.Player;
            Point PlayerTilePos = (Player.Position / 16).ToPoint();
            Point SpiderTilePos = (Position / 16f).ToPoint();

            switch (CurrentState)
            {
                case SpiderState.Chase:

                    // Steer towards
                    Vector2 ToTarget = Vector2.Normalize(Player.Position - Transform.Position) * Movement.MaxSpeed;

                    if (MoveTimer.TicsSoFar % 2 == 0)
                        SteeringForce = ToTarget - Movement.Velocity;
                    else
                        SteeringForce = Vector2.Zero;

                    if (!game.CanSeeBetweenSpaces(PlayerTilePos, SpiderTilePos))
                        CurrentState = SpiderState.Wander;

                    break;

                case SpiderState.Wander:
                    SteeringForce = Vector2.Zero;

                    if (MoveTimer.TicsSoFar % 2 == 0)
                    {
                        float DirX = -1 + (float)new Random().NextDouble() * 2;
                        float DirY = -1 + (float)new Random().NextDouble() * 2;
                        WanderDir = Vector2.Normalize(new Vector2(DirX, DirY));
                    }
                    else
                    {
                        SteeringForce = WanderDir * Movement.MaxSpeed;
                    }

                    if (game.CanSeeBetweenSpaces(PlayerTilePos, SpiderTilePos))
                    {
                        CurrentState = SpiderState.Chase;
                        //if (new Random().Next(5) == 0)
                        ///    GameContent.SoundEffects["SpiderNotice"].Play(0.05f, 0f, 0f);
                    }

                    break;
            }
        }

        public override void Update(CommandGaslight game, float deltaTime)
        {
            if (Health.CanBeDamaged &&
                game.Fires.Exists(f => f.PointInRadius(Position)))
            {
                Health.TakeDamage(-1);
                GameContent.SoundEffects["Hit"].Play(0.05f, 0f, 0f);
            }

            Health.Update(Position, deltaTime);



            MoveTimer.Update(deltaTime);

            base.Update(game, deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            base.Draw(spriteBatch, cameraPos);
            Health.Draw(spriteBatch, cameraPos);
        }
    }

    class GaslightEntityOil : GaslightEntity
    {
        public bool ClaimedByFire { get; private set; }

        public void Claim() => ClaimedByFire = true;

        public GaslightEntityOil(Vector2 startPos)
            : base(startPos, 8f, false, 1f, 1f, "GaslightOil", new Point(-16, -16), Type.Oil)
        {
        
        }
    }

    class GaslightEntityFire : GaslightEntity
    {
        Timer SpreadTimer;
        Timer ExistTimer;

        public GaslightEntityFire(Vector2 startPos)
            : base(startPos, 7f, false, 1f, 1f, "GaslightFire", new Point(-16, -16), Type.Fire)
        {
            SpreadTimer = new Timer(0.1f);
            Sprite.AnimateWhenStatic = true;
            ExistTimer = new Timer(3 + new Random().Next(3));
        }

        public override void Update(CommandGaslight game, float deltaTime)
        {
            SpreadTimer.Update(deltaTime);
            ExistTimer.Update(deltaTime);
            if (ExistTimer.TicThisFrame)
                IsDead = true;
            base.Update(game, deltaTime);
        }

        public override List<GaslightEntity> AddEntities(CommandGaslight game, Vector2 cameraPos)
        {
            List<GaslightEntity> NewEntities = new List<GaslightEntity>();
            if (SpreadTimer.TicThisFrame)
            {
                float SpreadRadius = 16f;

                var FiresInRange = game.Fires.FindAll(e => e.DistToPointSq(Position) < SpreadRadius * SpreadRadius);

                var UnlitOilsInRange = game.Oils.FindAll(e => e.DistToPointSq(Position) < SpreadRadius * SpreadRadius);

                if (UnlitOilsInRange.Count > 0)
                {
                    GaslightEntityOil FirstUnclaimedOil = null;
                    foreach (GaslightEntity oilentity in UnlitOilsInRange)
                    {
                        GaslightEntityOil oil = (GaslightEntityOil)oilentity;
                        if (!oil.ClaimedByFire)
                        {
                            FirstUnclaimedOil = oil;
                            break;
                        }
                    }

                    if (FirstUnclaimedOil != null)
                    {
                        Vector2 UnlitOilPos = FirstUnclaimedOil.Position;

                        float mindist = 1.5f;
                        if (!FiresInRange.Exists(e => (e.Position - UnlitOilPos).LengthSquared() < mindist * mindist))
                        {
                            FirstUnclaimedOil.Claim();
                            FirstUnclaimedOil.Kill();
                            NewEntities.Add(new GaslightEntityFire(UnlitOilPos));

                            if (new Random().Next(20) == 0)
                                GameContent.SoundEffects["FireSpread"].Play(0.2f, 0f, 0f);
                        }
                    }


                }
            }

            return NewEntities;
        }
    }

    class GaslightEntityCoin : GaslightEntity
    {
        AnimatedSprite2D ASprite;

        public GaslightEntityCoin(Vector2 startPos)
            : base(startPos, 5f, false, 1f, 64f, "CharacterBase", Point.Zero, Type.Coin)
        {
            ASprite = new AnimatedSprite2D("Coin", new Point(16, 16), startPos, new Vector2(-8, -8));
            ASprite.AddAnimation("Idle", 0, 8, 0.1f);
            ASprite.SetCurrentAnimation("Idle");
        }

        public override void Update(CommandGaslight game, float deltaTime)
        {
            ASprite.Update(deltaTime);

            Vector2 ToPlayer = Position - game.Player.Position;
            float SeekDist = 32;
            if (ToPlayer.LengthSquared() < SeekDist * SeekDist)
            {
                Vector2 ToTarget = Vector2.Normalize(game.Player.Position - Transform.Position) * Movement.MaxSpeed;
                SteeringForce = ToTarget - Movement.Velocity;
            }

            base.Update(game, deltaTime);

            ASprite.DrawPos = Transform.Position;
        }


        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            ASprite.Draw(spriteBatch, cameraPos);
        }
    }

    class GaslightEntityRefill: GaslightEntity
    {

        public GaslightEntityRefill(Vector2 startPos)
            : base(startPos, 5f, false, 1f, 64f, "GaslightRefill", new Point(-16, -16), Type.Refill)
        {}

        public override void Update(CommandGaslight game, float deltaTime)
        {
            Vector2 ToPlayer = Position - game.Player.Position;
            float SeekDist = 32;
            if (ToPlayer.LengthSquared() < SeekDist * SeekDist)
            {
                Vector2 ToTarget = Vector2.Normalize(game.Player.Position - Transform.Position) * Movement.MaxSpeed;
                SteeringForce = ToTarget - Movement.Velocity;
            }

            base.Update(game, deltaTime);
        }
    }

    class GaslightEntityHealth : GaslightEntity
    {

        public GaslightEntityHealth(Vector2 startPos)
            : base(startPos, 5f, false, 1f, 64f, "GaslightHeart", new Point(-16, -16), Type.Health)
        { }

        public override void Update(CommandGaslight game, float deltaTime)
        {
            Vector2 ToPlayer = Position - game.Player.Position;
            float SeekDist = 32;
            if (ToPlayer.LengthSquared() < SeekDist * SeekDist)
            {
                Vector2 ToTarget = Vector2.Normalize(game.Player.Position - Transform.Position) * Movement.MaxSpeed;
                SteeringForce = ToTarget - Movement.Velocity;
            }

            base.Update(game, deltaTime);
        }
    }

    class GaslightEntityStairs : GaslightEntity
    {

        public GaslightEntityStairs(Vector2 startPos)
            : base(startPos, 8f, false, 1f, 0f, "GaslightHeart", new Point(0, 0), Type.Stairs)
        { }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            Rectangle DrawRect = new Rectangle((Transform.Position - cameraPos).ToPoint() + new Point(-8, -8), new Point(16, 16));
            Rectangle SourceRect = new Rectangle(new Point(48, 48), new Point(16, 16));

            spriteBatch.Draw(
                GameContent.Sprites["GaslightingTiles"],
                DrawRect,
                SourceRect,
                Color.White);
        }
    }

}
