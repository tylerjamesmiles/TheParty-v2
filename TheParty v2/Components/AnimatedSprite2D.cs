using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class SpriteAnimation
    {
        int FrameRow;
        int NumFrames;
        int CurrentFrame;
        Timer Timer;

        public SpriteAnimation(int row, int numFrames, float speed)
        {
            FrameRow = row;
            NumFrames = numFrames;
            Timer = new Timer(speed);
            CurrentFrame = 0;
        }

        public void Update(float deltaTime)
        {
            Timer.Update(deltaTime);
            if (Timer.TicThisFrame)
            {
                CurrentFrame += 1;
                if (CurrentFrame >= NumFrames)
                    CurrentFrame -= NumFrames;
            }
        }

        public void Draw(string spriteName, Rectangle drawRect, SpriteBatch spriteBatch, bool flip = false)
        {
            Texture2D Sprite = GameContent.Sprites[spriteName];
            Point SourcePos = new Point(CurrentFrame * drawRect.Size.X, FrameRow * drawRect.Size.Y);
            Rectangle SourceRect = new Rectangle(SourcePos, drawRect.Size);

            SpriteEffects Flip = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(Sprite, drawRect, SourceRect, Color.White, 0f, Vector2.Zero, Flip, 0f);
        }
    }

    class AnimatedSprite2D
    {
        string SpriteName;
        string CurrentAnimationName;
        Point FrameSize;
        public Vector2 DrawPos { get; set; }
        Vector2 Offset;
        public bool Flip { get; private set; }
        Dictionary<string, SpriteAnimation> Animations;

        public AnimatedSprite2D(string spriteName, Point frameSize, Vector2 drawPos, Vector2 offset, bool flip = false)
        {
            CurrentAnimationName = "";
            SpriteName = spriteName;
            FrameSize = frameSize;
            Animations = new Dictionary<string, SpriteAnimation>();
            DrawPos = drawPos;
            Offset = offset;
            Flip = flip;
        }

        public SpriteAnimation CurrentAnimation =>
            Animations.GetValueOrDefault(CurrentAnimationName);
                
        public void AddAnimation(string name, int row, int numFrames, float speed)
            => Animations.Add(name, new SpriteAnimation(row, numFrames, speed));

        public void SetCurrentAnimation(string name) => CurrentAnimationName = name;

        public void Update(float deltaTime)
        {
            CurrentAnimation?.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Point DrawPosPoint = DrawPos.ToPoint() + Offset.ToPoint();
            Rectangle DrawRect = new Rectangle(DrawPosPoint, FrameSize);
            CurrentAnimation?.Draw(SpriteName, DrawRect, spriteBatch, Flip);
        }
    }
}
