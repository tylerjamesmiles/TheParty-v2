using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class StanceIndicator
    {
        int CurrentFrame;
        int CurrentStance;
        Timer AnimationTimer;
        Wobble Bob;
        public Vector2 DrawPos;
        bool Moving;
        bool Direction; // true = up, false = down
        public bool Reached { get; private set; }
    
        public StanceIndicator(int startStance, Vector2 drawPos)
        {
            CurrentStance = startStance;
            CurrentFrame = 0;
            AnimationTimer = new Timer(0.08f);
            Bob = new Wobble(2f, 2f);
            DrawPos = drawPos;
            Moving = true;
            Direction = true;
            Reached = false;
        }

        public void Update(float deltaTime)
        {
            Bob.Update(deltaTime);

            AnimationTimer.Update(deltaTime);

            if (Moving && AnimationTimer.TicThisFrame)
            {
                if (CurrentFrame == CurrentStance * 4)
                {
                    Moving = false;
                    Reached = true;
                }
                else
                {
                    CurrentFrame += Direction ? +1 : -1;

                    if (CurrentFrame > 10 * 4)
                        CurrentFrame = 0;
                }
            }
           
        }

        public void HardSet(int newStance)
        {
            Moving = false;
            Reached = true;
            CurrentStance = newStance;
            CurrentFrame = CurrentStance * 4;
        }

        public void SetTarget(int newStance)
        {
            if (newStance != CurrentStance)
            {
                Direction = newStance > CurrentStance;
                Moving = true;
                Reached = false;
                CurrentStance = newStance;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Point FrameSize = new Point(8, 10);

            Point DrawLoc = DrawPos.ToPoint() + new Point(0, (int)Bob.CurrentPosition);
            Rectangle DrawRect = new Rectangle(DrawLoc, FrameSize);

            Point SourceLoc = new Point(CurrentFrame * FrameSize.X, 0);
            Rectangle SourceRect = new Rectangle(SourceLoc, FrameSize);

            spriteBatch.Draw(GameContent.Sprites["Stances"], DrawRect, SourceRect, Color.White);
        }
    }

}
