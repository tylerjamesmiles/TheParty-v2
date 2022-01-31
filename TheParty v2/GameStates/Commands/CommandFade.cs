using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandFade : Command<TheParty>
    {
        public enum Direction { In, Out };
        Direction Dir;
        Timer Frames;
        int CurrentFrame;

        public CommandFade(Direction dir)
        {
            Dir = dir;
        }

        public override void Enter(TheParty client)
        {
            Frames = new Timer(0.2f);
            CurrentFrame = 0;
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Frames.Update(deltaTime);
            if (Frames.TicThisFrame)
            {
                if (CurrentFrame < 3)
                {
                    CurrentFrame++;
                }
                else
                {
                    Done = true;
                }
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            string SpriteName = Dir == Direction.In ? "FadeIn" : "FadeOut";
            spriteBatch.Draw(
                GameContent.Sprites[SpriteName],
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                new Rectangle(new Point(CurrentFrame * 160, 0), new Point(160, 144)),
                Color.White);
        }
    }
}
