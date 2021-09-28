using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheParty_v2
{
    class GUIChoiceBox
    {
        GUIBox Box;
        GUIChoice Choice;
        GUIText Text;

        public int CurrentChoice => Choice.CurrentChoiceIdx;
        public int NumChoices => Choice.NumChoices;
        public bool Done => Box.Done;

        public enum Position
        {
            Center,
            BottomLeft,
            BottomRight
        }

        public GUIChoiceBox(string[] choices, Position position, bool[] choiceValidity = null)
        {
            string ChoicesText = "";
            for (int i = 0; i < choices.Length; i++)
            {
                ChoicesText += choices[i];
                if (i < choices.Length - 1)
                    ChoicesText += "\n";
            }

            Vector2 TextSize = GameContent.Font.MeasureString(ChoicesText);
            Vector2 ScreenSize = GraphicsGlobals.ScreenSize.ToVector2();
            Vector2 ScreenCenter = ScreenSize / 2;
            Vector2 TextLoc =
                position == Position.Center ? ScreenCenter - TextSize / 2 :
                position == Position.BottomLeft ? new Vector2(4, ScreenSize.Y - TextSize.Y) :
                position == Position.BottomRight ? new Vector2(ScreenSize.X - TextSize.X - 8, ScreenSize.Y - TextSize.Y - 8) :
                Vector2.Zero;
            Text = new GUIText(ChoicesText, TextLoc, int.MaxValue, float.MinValue);

            Point BoxLoc = TextLoc.ToPoint() + new Point(-4, -4);
            Point BoxSize = TextSize.ToPoint() + new Point(8, 8);
            Rectangle BoxBounds = new Rectangle(BoxLoc, BoxSize);
            Box = new GUIBox(BoxBounds);

            Vector2[] ChoicePositions = new Vector2[choices.Length];
            for (int i = 0; i < choices.Length; i++)
                if (choiceValidity == null || 
                    (choiceValidity != null && choiceValidity[i]))
                    ChoicePositions[i] = TextLoc + new Vector2(0, i * 10 - 4);

            Choice = new GUIChoice(ChoicePositions);

        }

        public void Update(float deltaTime, bool isInFocus)
        {
            Box.Update(deltaTime, true);
            Choice.Update(deltaTime, Box.Grown);
            Text.Update(deltaTime, Box.Grown);

            if (!Box.Shrink && Choice.Done)
                Box.StartShrink();
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            Box.Draw(spriteBatch, true);

            if (Box.Grown && !Box.Shrink)
            {
                Choice.Draw(spriteBatch, true);
                Text.Draw(spriteBatch, true);
            }

        }
    }
}
