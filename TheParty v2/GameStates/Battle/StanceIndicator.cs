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
        Point DrawPos;
    
        public StanceIndicator(int startStance, Point drawPos)
        {
            CurrentStance = startStance;
            CurrentFrame = 0;
            AnimationTimer = new Timer(0.08f);
            Bob = new Wobble(2f, 3f);
            DrawPos = drawPos;
        }

        public void Update(float deltaTime)
        {
            Bob.Update(deltaTime);

            AnimationTimer.Update(deltaTime);

            if (CurrentFrame != CurrentStance * 4 && AnimationTimer.TicThisFrame)
                CurrentFrame++;

            if (CurrentFrame > 5 * 4)
                CurrentFrame = 0;
        }

        public void SetStance(int newStance)
        {
            CurrentStance = newStance;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Point FrameSize = new Point(7, 10);

            Point DrawLoc = DrawPos + new Point(0, (int)Bob.CurrentPosition);
            Rectangle DrawRect = new Rectangle(DrawLoc, FrameSize);

            Point SourceLoc = new Point(CurrentFrame * 7, 0);
            Rectangle SourceRect = new Rectangle(SourceLoc, FrameSize);

            spriteBatch.Draw(GameContent.Sprites["Stances"], DrawRect, SourceRect, Color.White);
        }
    }

}
