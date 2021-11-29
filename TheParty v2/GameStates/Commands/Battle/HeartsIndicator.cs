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
        int CurrentDisplayHP;
        Timer UpdateTimer;
        public int MaxHP { get; set; }
        int CenterX;
        int Y;
        int NumHearts;
        int CurrentBouncing;
        bool BeMeat;
        bool ShowMax;
        Timer BounceMoveTimer;

        public HeartsIndicator(int startHP, int centerX, int y, bool beMeat = false, bool showMax = false, int maxHP = 0)
        {
            CenterX = centerX;
            Y = y;
            BounceMoveTimer = new Timer(0.1f);
            CurrentBouncing = new Random().Next(NumHearts * 10);
            CurrentDisplayHP = 0;
            UpdateTimer = new Timer(0.1f);
            BeMeat = beMeat;
            ShowMax = showMax;
            MaxHP = maxHP;
            SetHP(startHP);

        }

        public void Update(float deltaTime)
        {
            BounceMoveTimer.Update(deltaTime);
            if (BounceMoveTimer.TicThisFrame)
                CurrentBouncing++;
            if (CurrentBouncing > NumHearts * 10)
                CurrentBouncing = 0;

            UpdateTimer.Update(deltaTime);
            if (UpdateTimer.TicThisFrame && CurrentDisplayHP != CurrentHP)
            {
                bool MovingUp = CurrentDisplayHP < CurrentHP;
                CurrentDisplayHP += (MovingUp) ? +1 : -1;
            }
        }

        public void SetShowMax(bool setting)
        {
            ShowMax = setting;
            SetHP(CurrentHP);
        }

        public void SetHP(int newHP)
        {
            CurrentHP = newHP;

            if (ShowMax)
                NumHearts = (MaxHP + 1) / 2;
            else
                NumHearts = (newHP + 1) / 2;
        }

        public void SetMax(int newMax)
        {
            MaxHP = newMax;
            NumHearts = (MaxHP + 1) / 2;
        }

        public void SetPos(Point newPos)
        {
            CenterX = newPos.X;
            Y = newPos.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int HeartWidth = 4;
            int NumDisplayHearts = (CurrentDisplayHP + 1) / 2;
            int NumHeartsAcross = NumHearts > 5 ? 5 : NumHearts;
            int TotalWidth = HeartWidth * NumHeartsAcross;
            int Left = CenterX - TotalWidth / 2;

            List<Point> DrawPoses = new List<Point>();
            for (int i = 0; i < NumHearts; i++)
            {
                int DrawX =
                    Left + ((i % 5) * HeartWidth);
                int DrawY = Y +
                    ((i / 5) * 5) +
                    ((CurrentBouncing == i) ? - 1 : 0);
                DrawPoses.Add(new Point(DrawX, DrawY));
            }

            int LastSourceX = (CurrentDisplayHP % 2 == 0) ? 0 : 5;

            for (int i = 0; i < NumHearts; i++)
            {
                Point MaxSourcePos = new Point(0, 5);

                Point SourcePos = (i == NumDisplayHearts - 1) ?  // *****
                    new Point(LastSourceX, 0) : new Point(0, 0);

                string SpriteName = (BeMeat) ? "Meats" : "Hearts";

                if (ShowMax)
                {
                    spriteBatch.Draw(
                        GameContent.Sprites[SpriteName],
                        new Rectangle(DrawPoses[i], new Point(5, 5)),
                        new Rectangle(MaxSourcePos, new Point(5, 5)),
                        Color.White);
                }

                if (i < NumDisplayHearts)
                {
                    spriteBatch.Draw(
                        GameContent.Sprites[SpriteName],
                        new Rectangle(DrawPoses[i], new Point(5, 5)),
                        new Rectangle(SourcePos, new Point(5, 5)),
                        Color.White);
                }
            }
        }
    }
}
