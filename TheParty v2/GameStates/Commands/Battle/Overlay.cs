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
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 mapSize, Vector2 cameraPos)
        {
            Texture2D Sprite = GameContent.Sprites[SpriteName];
            Point Size = GraphicsGlobals.ScreenSize;

            Point TL = new Point((int)-cameraPos.X % Size.X + (int)HorizontalOffset, (int)-cameraPos.Y % Size.Y);
            TL -= Size;
            Point TM = TL + new Point(Size.X, 0);
            Point TR = TL + new Point(Size.X * 2, 0);
            Point ML = TL + new Point(0, Size.Y);
            Point MM = TL + new Point(Size.X, Size.Y);
            Point MR = TL + new Point(Size.X * 2, Size.Y);
            Point BL = TL + new Point(0, Size.Y * 2);
            Point BM = TL + new Point(Size.X, Size.Y * 2);
            Point BR = TL + new Point(Size.X * 2, Size.Y * 2);

            spriteBatch.Draw(Sprite, new Rectangle(TL, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(TM, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(TR, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(ML, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(MM, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(MR, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(BL, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(BM, Size), Color.White);
            spriteBatch.Draw(Sprite, new Rectangle(BR, Size), Color.White);

        }
    }
}
