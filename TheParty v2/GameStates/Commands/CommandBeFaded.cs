using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandBeFaded : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            client.BeFaded = true;
            Done = true;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites["FadeIn"],
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                Color.White);
        }
    }

    class CommandShowScreen : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            client.BeFaded = false;
            Done = true;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites["FadeIn"],
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                Color.White);
        }
    }
}
