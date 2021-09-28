using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Animation
    {
        int FrameRow;
        int NumFrames;
        int CurrentFrame;
        Timer Timer;

        public Animation(int row, int numFrames, float speed)
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
        Vector2 Pos;
        Vector2 Offset;
        public Vector2 DrawPos => Pos;
        bool Flip;
        Dictionary<string, Animation> Animations;

        public AnimatedSprite2D(string spriteName, Point frameSize, Vector2 drawPos, Vector2 offset, bool flip = false)
        {
            CurrentAnimationName = "";
            SpriteName = spriteName;
            FrameSize = frameSize;
            Animations = new Dictionary<string, Animation>();
            Pos = drawPos;
            Offset = offset;
            Flip = flip;
        }

        public Animation CurrentAnimation =>
            Animations.GetValueOrDefault(CurrentAnimationName);
                
        public void AddAnimation(string name, int row, int numFrames, float speed)
            => Animations.Add(name, new Animation(row, numFrames, speed));

        public void SetCurrentAnimation(string name) => CurrentAnimationName = name;

        public void Update(float deltaTime)
        {
            CurrentAnimation?.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Point DrawPosPoint = Pos.ToPoint() + Offset.ToPoint();
            Rectangle DrawRect = new Rectangle(DrawPosPoint, FrameSize);
            CurrentAnimation?.Draw(SpriteName, DrawRect, spriteBatch, Flip);
        }
    }
}
