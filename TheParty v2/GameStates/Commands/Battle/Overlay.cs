using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Overlay
    {
        float HorizontalSpeed;
        float HorizontalOffset;
        string SpriteName;

        public Overlay(string spriteName)
        {
            HorizontalSpeed = 5f;
            HorizontalOffset = 0f;
            SpriteName = spriteName;
        }

        public void Update(float deltaTime)
        {
            HorizontalOffset += HorizontalSpeed * deltaTime;
            if (HorizontalOffset > GraphicsGlobals.ScreenSize.X)
                HorizontalOffset -= GraphicsGlobals.ScreenSize.X;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 mapSize, Vector2 cameraPos)
        {
            Texture2D Sprite = GameContent.Sprites[SpriteName];
            Point Size = GraphicsGlobals.ScreenSize;
            Point CameraPos = cameraPos.ToPoint();

            Point BRPos = new Point(-CameraPos.X % Size.X, -CameraPos.Y % Size.Y);
            BRPos -= Size;
            BRPos.X += (int)HorizontalOffset;

            List<Point> DrawPoses = new List<Point>();
            DrawPoses.Add(BRPos);
            DrawPoses.Add(BRPos + new Point(Size.X,     0));
            DrawPoses.Add(BRPos + new Point(Size.X * 2, 0));
            DrawPoses.Add(BRPos + new Point(0,           Size.Y));
            DrawPoses.Add(BRPos + new Point(Size.X,     Size.Y));
            DrawPoses.Add(BRPos + new Point(Size.X * 2, Size.Y));
            DrawPoses.Add(BRPos + new Point(0,           Size.Y * 2));
            DrawPoses.Add(BRPos + new Point(Size.X,     Size.Y * 2));
            DrawPoses.Add(BRPos + new Point(Size.X * 2, Size.Y * 2));

            DrawPoses.ForEach(dp => spriteBatch.Draw(Sprite, new Rectangle(dp, Size), Color.White)); 
        }
    }
}
