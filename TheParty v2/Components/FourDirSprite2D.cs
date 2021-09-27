using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FourDirSprite2D
    {
        public string SpriteName;
        public Point DrawOffset;
        public Point SpriteSize;

        public int CurrentFrame;
        public int CurrentFacing;
        public Timer FrameTimer;

        public FourDirSprite2D(string spriteName, Point drawOffset)
        {
            SpriteName = spriteName;
            DrawOffset = drawOffset;
            SpriteSize = new Point(32, 32);
            CurrentFrame = 0;
            CurrentFacing = 0;
            FrameTimer = new Timer(0.15f);
        }

        public void Update(Vector2 velocity, float deltaTime)
        {
            bool Moving = velocity.LengthSquared() > 0.05f;

            if (Moving)
            {
                FrameTimer.Update(deltaTime);

                if (FrameTimer.TicThisFrame)
                    CurrentFrame = MathUtility.RolledIfAtLimit(CurrentFrame + 1, 4);

                CurrentFacing = (MathF.Abs(velocity.Y) > MathF.Abs(velocity.X)) ?
                    velocity.Y < 0 ? 0 : 1 :
                    velocity.X < 0 ? 2 : 3;
            }
            else
                CurrentFrame = 0;
        }

        public void Draw(Vector2 mapPos, Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            if (SpriteName == "")
                return;

            Point DrawPos = (mapPos - cameraPos).ToPoint() + DrawOffset;
            Point DrawSize = SpriteSize;
            Rectangle DrawRect = new Rectangle(DrawPos, DrawSize);

            int SourceX = CurrentFrame * SpriteSize.X;
            int SourceY = CurrentFacing * SpriteSize.Y;
            Point SourcePos = new Point(SourceX, SourceY);
            Point SourceSize = SpriteSize;
            Rectangle SourceRect = new Rectangle(SourcePos, SourceSize);

            Texture2D Sprite = GameContent.Sprites[SpriteName];

            spriteBatch.Draw(Sprite, DrawRect, SourceRect, Color.White);
        }
    }
}
