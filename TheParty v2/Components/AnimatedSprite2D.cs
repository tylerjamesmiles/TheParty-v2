using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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

        public SpriteAnimation(JsonElement elem)
        {
            FrameRow = elem.GetProperty("FrameRow").GetInt32();
            NumFrames = elem.GetProperty("NumFrames").GetInt32();
            float FrameRate = (float)elem.GetProperty("FrameRate").GetDouble();
            Timer = new Timer(FrameRate);
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

        public AnimatedSprite2D(string sheetName, JsonDocument doc)
        {
            JsonElement root = doc.RootElement.GetProperty(sheetName);
            SpriteName = root.GetProperty("SpriteName").GetString();
            int X = root.GetProperty("FrameSizeX").GetInt32();
            int Y = root.GetProperty("FrameSizeY").GetInt32();
            FrameSize = new Point(X, Y);

            int oX = root.GetProperty("OffsetX").GetInt32();
            int oY = root.GetProperty("OffsetY").GetInt32();
            Offset = new Vector2(oX, oY);

            Animations = new Dictionary<string, SpriteAnimation>();
            int NumAnimations = root.GetProperty("Animations").GetArrayLength();
            for (int i = 0; i < NumAnimations; i++)
            {
                JsonElement anim = root.GetProperty("Animations")[i];
                string name = anim.GetProperty("Name").GetString();
                Animations.Add(name, new SpriteAnimation(anim));
            }
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
