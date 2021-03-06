using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Particle
    {
        Vector2 Position;
        Vector2 Velocity;
        Vector2 Acceleration;
        Vector2 Steering;
        float MaxSpeed;
        float Mass;
        float RotationSpeed;
        float CurrentRotation;
        float Bright;
        float BrightnessSpeed;

        Wobble FloatWobble;

        string SpriteName;

        public enum Type { Falling, Floating };
        Type ParticleType;

        public bool Offscreen { get; private set; }

        public Particle(Vector2 startingPos, string sprite = "Hearts")
        {
            ParticleType = Type.Floating;
            Position = startingPos;
            MaxSpeed = 10f;
            Velocity = new Vector2(0, -1f) * MaxSpeed;
            RotationSpeed = 0f;
            CurrentRotation = 0f;
            Mass = 1f;
            Bright = 1f;
            BrightnessSpeed = 0.5f;
            FloatWobble = new Wobble(3f, 4f);
            SpriteName = sprite;
        }

        public Particle(Vector2 startingPos, Vector2 startingDir, float startingSpeed, string sprite = "Hearts")
        {
            ParticleType = Type.Falling;
            Position = startingPos;;
            MaxSpeed = startingSpeed;
            Velocity = startingDir * MaxSpeed;
            int RotationDir = new Random().Next(2) == 0 ? -1 : 1;
            RotationSpeed = 5.0f * RotationDir;
            CurrentRotation = 0f;
            Mass = 1f;
            Bright = 1f;
            BrightnessSpeed = 0.5f;
            FloatWobble = new Wobble(0f, 0f);
            SpriteName = sprite;
        }

        public void Update(float deltaTime)
        {
            Vector2 SteeringForce = new Vector2();
            if (ParticleType == Type.Falling)
            {
                float GravityAmt = 150f;
                SteeringForce = new Vector2(0, GravityAmt);
            }
            else if (ParticleType == Type.Floating)
            {
                float FloatAmt = 20f;
                SteeringForce = new Vector2(0, -FloatAmt);
            }

            Acceleration = SteeringForce * Mass;
            Velocity += Acceleration * deltaTime;
            Position += Velocity * deltaTime;

            CurrentRotation += RotationSpeed * deltaTime;

            Bright -= BrightnessSpeed * deltaTime;

            Offscreen = 
                Position.X < 0 || Position.X > 160 || 
                Position.Y < 0 || Position.Y > 144 ||
                Bright == 0f;

            FloatWobble.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            Vector2 FloatOffset = new Vector2(FloatWobble.CurrentPosition, 0);
            Color DrawColor = new Color();
            Rectangle SourceRect = new Rectangle();

            if (ParticleType == Type.Falling)
            {
                DrawColor = new Color(Bright, Bright, Bright, 1f);
                SourceRect = new Rectangle(new Point(5, 0), new Point(5, 5));
            }
            else if (ParticleType == Type.Floating)
            {
                DrawColor = new Color(Bright, Bright, Bright, Bright);
                SourceRect = new Rectangle(new Point(0, 0), new Point(5, 5));
            }

            spriteBatch.Draw(
                GameContent.Sprites[SpriteName],
                new Rectangle(Position.ToPoint() + FloatOffset.ToPoint() - cameraPos.ToPoint(), new Point(5, 5)),
                SourceRect,
                DrawColor,
                CurrentRotation,
                Vector2.Zero,
                SpriteEffects.None,
                0f);
        }
    }

    class HeartsIndicator
    {
        public int CurrentHP { get; set; }
        int CurrentDisplayHP;
        Timer UpdateTimer;
        public int MaxHP { get; set; }
        public Point Origin;
        int NumTotalHearts;
        int CurrentBouncing;
        bool ShowMax;
        bool Centered;
        Timer BounceMoveTimer;

        public enum Type { Hearts, Meats, Commitment };
        Type IconType;

        List<Particle> Particles;

        public HeartsIndicator(int startHP, int centerX, int y, Type iconType = Type.Hearts, bool showMax = false, int maxHP = 0, bool centered = true)
        {
            Origin = new Point(centerX, y);
            BounceMoveTimer = new Timer(0.1f);
            CurrentBouncing = new Random().Next(NumTotalHearts * 10);
            CurrentDisplayHP = 0;
            UpdateTimer = new Timer(0.3f);
            IconType = iconType;
            ShowMax = showMax;
            MaxHP = maxHP;
            Centered = centered;
            SetHP(startHP);

            Particles = new List<Particle>();
        }

        public void Update(float deltaTime)
        {
            BounceMoveTimer.Update(deltaTime);
            if (BounceMoveTimer.TicThisFrame)
                CurrentBouncing++;
            if (CurrentBouncing > NumTotalHearts * 10 ||
                (NumTotalHearts <= 1 && CurrentBouncing > 2))
                CurrentBouncing = 0;

            UpdateTimer.Update(deltaTime);
            if (UpdateTimer.TicThisFrame && CurrentDisplayHP != CurrentHP)
            {
                bool MovingUp = CurrentDisplayHP < CurrentHP;
                CurrentDisplayHP += (MovingUp) ? +1 : -1;
                string SpriteName = 
                    (IconType == Type.Meats) ? "Meats" : 
                    (IconType == Type.Hearts) ? "Hearts" :
                    "Commitment";

                if (MovingUp == true)
                {
                    // Spawn floating heart
                    Particles.Add(new Particle(Origin.ToVector2(), SpriteName));
                }
                else if (MovingUp == false)
                {
                    // spawn two broken heart halves
                    Random Rand = new Random();
                    float RandX = -0.5f + (float)Rand.NextDouble() * 1.0f;
                    float RandY = (float)Rand.NextDouble() * -1.0f;
                    Vector2 Dir = new Vector2(RandX, RandY);
                    Dir.Normalize();

                    Particles.Add(new Particle(Origin.ToVector2(), Dir, 80f, SpriteName));

                    Vector2 NewDir = Dir + new Vector2(0.1f, 0.1f);
                    Particles.Add(new Particle(Origin.ToVector2(), NewDir, 80f, SpriteName));

                    GameContent.SoundEffects["LoseHeart"].Play();
                }
            }

            Particles.ForEach(p => p.Update(deltaTime));

            Particles.RemoveAll(p => p.Offscreen);
        }

        public void SetShowMax(bool setting)
        {
            ShowMax = setting;
            SetHP(CurrentHP);
        }

        public void SetHP(int newHP)
        {
            CurrentHP = newHP;

            if (ShowMax)
                NumTotalHearts = (MaxHP + 1) / 2;
            else
                NumTotalHearts = (newHP + 1) / 2;
        }

        public void SetMax(int newMax)
        {
            MaxHP = newMax;
            NumTotalHearts = (MaxHP + 1) / 2;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            int HeartPixelWidth = 4;
            int NumLivingHearts = CurrentDisplayHP;
            int NumHeartsToDisplay = 
                (ShowMax) ? 
                    MaxHP :
                    NumLivingHearts;    // Num hearts of current HP
            int NumHeartsAcross = NumHeartsToDisplay > 5 ? 5 : NumHeartsToDisplay;    // used for centering
            int TotalWidth = HeartPixelWidth * NumHeartsAcross;
            int Left = 
                (Centered) ? Origin.X - TotalWidth / 2 :
                Origin.X;

            List<Point> DrawPoses = new List<Point>();
            for (int i = 0; i < NumHeartsToDisplay; i++)
            {
                int DrawX =
                    (Centered) ? Left + ((i % 5) * HeartPixelWidth) :
                    Left + i * HeartPixelWidth;
                int DrawY =
                    (Centered) ? Origin.Y + ((i / 5) * 5) :
                    Origin.Y;
                if (i < NumLivingHearts)
                {
                    if (IconType == Type.Commitment && CurrentBouncing == i)
                        DrawX -= 1;

                    if (CurrentBouncing == i)
                        DrawY -= 1;
                }
                DrawPoses.Add(new Point(DrawX, DrawY));

            }

            int LastSourceX = 0;

            for (int i = 0; i < NumHeartsToDisplay; i++)
            {
                Point MaxSourcePos = new Point(0, 5);

                Point SourcePos = (i == NumHeartsToDisplay - 1) ?  // *****
                    new Point(LastSourceX, 0) : new Point(0, 0);

                string SpriteName =
                    (IconType == Type.Meats) ? "Meats" :
                    (IconType == Type.Hearts) ? "Hearts" :
                    "Commitment";

                if (ShowMax)
                {
                    spriteBatch.Draw(
                        GameContent.Sprites[SpriteName],
                        new Rectangle(DrawPoses[i] - cameraPos.ToPoint(), new Point(5, 5)),
                        new Rectangle(MaxSourcePos, new Point(5, 5)),
                        Color.White);
                }

                if (i < NumLivingHearts)
                {
                    Point SourceSize = new Point(5, 5);
                    if (IconType == Type.Commitment && CurrentBouncing == i)
                    {
                        SourcePos = new Point(5, 0);
                        SourceSize = new Point(7, 7);
                    }
                    spriteBatch.Draw(
                        GameContent.Sprites[SpriteName],
                        new Rectangle(DrawPoses[i] - cameraPos.ToPoint(), SourceSize),
                        new Rectangle(SourcePos, SourceSize),
                        Color.White);
                }
            }

            Particles.ForEach(p => p.Draw(spriteBatch, cameraPos));
        }
    }
}
