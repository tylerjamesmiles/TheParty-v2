using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheParty_v2
{
    static class DrawPrimitives2D
    {
        private static Texture2D SimpleTexture;
        private static readonly string CircleSpriteName = "Circle";
        private static readonly string FilledCircleSpriteName = "FilledCircle";

        public static void Initialize(GraphicsDevice graphics)
        {
            SimpleTexture = new Texture2D(graphics, 1, 1);
            SimpleTexture.SetData(new Color[] { Color.White });
        }

        // PIXEL ~ ~ ~

        public static void Pixel(
            Point position,
            Color color,
            SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                SimpleTexture,
                new Rectangle(position, new Point(1, 1)),
                new Rectangle(new Point(0, 0), new Point(1, 1)),
                color);
        }


        // LINE ~ ~ ~
        public static void Line(
            Vector2 startPos,
            float magnitude,
            float rotation,
            Color color,
            SpriteBatch spriteBatch,
            float thickness = 1f)
        {
            Vector2 Origin = new Vector2(0f, 0.5f);
            Vector2 Scale = new Vector2(magnitude, thickness);

            spriteBatch.Draw(
                SimpleTexture,
                startPos,
                null,
                color,
                rotation,
                Origin,
                Scale,
                SpriteEffects.None,
                0);
        }

        public static void Line(
            Vector2 startPos,
            Vector2 endPos,
            Color color,
            SpriteBatch spriteBatch,
            float thickness = 1f)
        {
            float Magnitude = Vector2.Distance(startPos, endPos);
            float Rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
            Line(startPos, Magnitude, Rotation, color, spriteBatch, thickness);
        }

        public static void Line(Point startPos, Point endPos, Color color, SpriteBatch spriteBatch, float thickness = 1f)
            => Line(startPos.ToVector2(), endPos.ToVector2(), color, spriteBatch, thickness);


        // CIRCLE ~ ~ ~

        public static void Circle(
            Point position,
            int radius,
            Color color,
            SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites[CircleSpriteName],
                new Rectangle(
                    position - new Point((radius / 2), (radius / 2)),
                    new Point(radius, radius)),
                new Rectangle(
                    new Point(0, 0),
                    new Point(GameContent.Sprites[CircleSpriteName].Width, GameContent.Sprites[CircleSpriteName].Height)),
                color);
        }
    
        public static void Circle(Vector2 position, float radius, Color color, SpriteBatch spriteBatch)
            => Circle(position.ToPoint(), (int)radius, color, spriteBatch);


        // FILLED CIRCLE ~ ~ ~
        public static void FilledCircle(
            Point position,
            int radius,
            Color color,
            SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites[FilledCircleSpriteName],
                new Rectangle(
                    position - new Point(radius / 2, radius / 2),
                    new Point(radius, radius)),
                new Rectangle(
                    new Point(0, 0),
                    new Point(GameContent.Sprites[FilledCircleSpriteName].Width, GameContent.Sprites[FilledCircleSpriteName].Height)),
                color);
        }

        public static void FilledCircle( Vector2 position, float radius, Color color, SpriteBatch spriteBatch)
            => FilledCircle(position.ToPoint(), (int)radius, color, spriteBatch);


        // RECTANGLE ~ ~ ~

        public static void Rectangle(
            Point position,
            Point size,
            Color color,
            SpriteBatch spriteBatch,
            float thickness = 1f)
        {
            Point TL = position;
            Point TR = new Point(position.X + size.X, position.Y);
            Point BL = new Point(position.X, position.Y + size.Y);
            Point BR = position + size;

            Line(TL, TR, color, spriteBatch, thickness);
            Line(TR, BR, color, spriteBatch, thickness);
            Line(BR, BL, color, spriteBatch, thickness);
            Line(BL, TL, color, spriteBatch, thickness);
        }

        public static void Rectangle(Vector2 position, Vector2 size, Color color, SpriteBatch spriteBatch, float thickness = 1f)
            => Rectangle(position.ToPoint(), size.ToPoint(), color, spriteBatch, thickness);

        public static void Rectangle( Rectangle rect, Color color, SpriteBatch spriteBatch, float thickness = 1f)
            => Rectangle(rect.Location, rect.Size, color, spriteBatch, thickness);

        // FILLED RECTANGLE ~ ~ ~

        public static void FilledRectangle(
            Rectangle rect,
            Color color, 
            SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                SimpleTexture,
                rect,
                new Rectangle(new Point(0, 0), new Point(1, 1)),
                color);
        }

        public static void FilledRectangle(Point position, Point size, Color color, SpriteBatch spriteBatch)
            => FilledRectangle(new Rectangle(position, size), color, spriteBatch);
        public static void FilledRectangle(Vector2 position, Vector2 size, Color color, SpriteBatch spriteBatch)
            => FilledRectangle(new Rectangle(position.ToPoint(), size.ToPoint()), color, spriteBatch);
            
    }
}
