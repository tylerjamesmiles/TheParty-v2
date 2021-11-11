using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FourDirSprite2D
    {
        public string SpriteName { get; private set; }
        public Point DrawOffset { get; private set; }
        public Point SpriteSize { get; private set; }

        public int CurrentFrame { get; set; }
        public int CurrentFacing { get; set; }
        public Timer FrameTimer { get; private set; }

        public bool AnimateWhenStatic { get; private set; }

        public FourDirSprite2D(string spriteName, Point drawOffset, bool animateWhenStatic = false)
        {
            SpriteName = spriteName;
            DrawOffset = drawOffset;
            SpriteSize = new Point(32, 32);
            CurrentFrame = 0;
            CurrentFacing = 0;
            FrameTimer = new Timer(0.15f);
            AnimateWhenStatic = animateWhenStatic;
        }

        public void Update(Vector2 velocity, float deltaTime)
        {
            bool Moving = velocity.LengthSquared() > 0.05f;


            if (Moving || AnimateWhenStatic)
            {
                FrameTimer.Update(deltaTime);

                if (FrameTimer.TicThisFrame)
                    CurrentFrame = Utility.RolledIfAtLimit(CurrentFrame + 1, 4);
            }
            else
                CurrentFrame = 0;

            if (Moving)
            {
                CurrentFacing = (MathF.Abs(velocity.Y) > MathF.Abs(velocity.X)) ?
                    velocity.Y < 0 ? 0 : 1 :
                    velocity.X < 0 ? 2 : 3;
            }
        }

        public void Draw(Vector2 mapPos, Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            if (SpriteName == "")
                return;

            Point DrawPos = (mapPos - cameraPos).ToPoint() + DrawOffset;
            Point DrawSize = SpriteSize;
            Rectangle DrawRect = new Rectangle(DrawPos, DrawSize);

            int DrawFrame = CurrentFrame;
            if (CurrentFrame == 2)
                DrawFrame = 0;
            if (CurrentFrame == 3)
                DrawFrame = 2;
            int DrawFacing = CurrentFacing;
            if (CurrentFacing == 3)
                DrawFacing = 2;

            Point SourcePos = new Point(DrawFrame * SpriteSize.X, DrawFacing * SpriteSize.Y);
            Rectangle SourceRect = new Rectangle(SourcePos, SpriteSize);

            SpriteEffects Flip = CurrentFacing == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D Sprite = GameContent.Sprites[SpriteName];

            spriteBatch.Draw(Sprite, DrawRect, SourceRect, Color.White, 0f, Vector2.Zero, Flip, 0f);
        }
    }
}
