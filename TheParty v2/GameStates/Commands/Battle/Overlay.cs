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
            Point Size = new Point(160, 144);

            for (int x = 0; x < mapSize.X; x++)
                for (int y = 0; y < mapSize.Y; y++)
                {
                    //Point DrawPos
                }

            spriteBatch.Draw(Sprite, new Rectangle(new Point((int)HorizontalOffset, 0), Size), Color.White);
        }
    }
}
