using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class HeartsIndicator
    {
        public int CurrentHP { get; set; }
        int CenterX;
        int Y;
        int NumHearts;
        int CurrentBouncing;
        Timer BounceMoveTimer;

        public HeartsIndicator(int startHP, int centerX, int y)
        {
            SetHP(startHP);
            CenterX = centerX;
            Y = y;
            BounceMoveTimer = new Timer(0.1f);
            CurrentBouncing = new Random().Next(NumHearts * 10);
        }

        public void Update(float deltaTime)
        {
            BounceMoveTimer.Update(deltaTime);
            if (BounceMoveTimer.TicThisFrame)
                CurrentBouncing++;
            if (CurrentBouncing > NumHearts * 10)
                CurrentBouncing = 0;
        }

        public void SetHP(int newHP)
        {
            CurrentHP = newHP;
            NumHearts = (newHP + 1) / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int HeartWidth = 4;
            int TotalWidth = HeartWidth * NumHearts;
            int Left = CenterX - TotalWidth / 2;

            List<Point> DrawPoses = new List<Point>();
            for (int i = 0; i < NumHearts; i++)
            {
                int DrawY = (CurrentBouncing == i) ? Y - 1 : Y;
                DrawPoses.Add(new Point(Left + i * HeartWidth, DrawY));
            }

            int LastSourceX = (CurrentHP % 2 == 0) ? 0 : 5;

            for (int i = 0; i < NumHearts; i++)
            {
                Point SourcePos = (i == NumHearts - 1) ?
                    new Point(LastSourceX, 0) : new Point(0, 0);

                spriteBatch.Draw(
                    GameContent.Sprites["Hearts"],
                    new Rectangle(DrawPoses[i], new Point(5, 5)),
                    new Rectangle(SourcePos, new Point(5, 5)),
                    Color.White);
            }

        }
    }
}
