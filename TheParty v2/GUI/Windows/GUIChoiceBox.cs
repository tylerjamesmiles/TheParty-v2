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

        public GUIChoiceBox(string[] choices)
        {
            string ChoicesText = "";
            for (int i = 0; i < choices.Length; i++)
            {
                ChoicesText += choices[i];
                if (i < choices.Length - 1)
                    ChoicesText += "\n";
            }

            Vector2 TextSize = GameContent.Font.MeasureString(ChoicesText);
            Vector2 ScreenCenter = GraphicsGlobals.ScreenSize.ToVector2() / 2;
            Vector2 TextLoc = ScreenCenter - TextSize / 2;
            Text = new GUIText(ChoicesText, TextLoc, int.MaxValue, float.MinValue);

            Point BoxLoc = TextLoc.ToPoint() + new Point(-4, -4);
            Point BoxSize = TextSize.ToPoint() + new Point(8, 8);
            Rectangle BoxBounds = new Rectangle(BoxLoc, BoxSize);
            Box = new GUIBox(BoxBounds);

            Vector2[] ChoicePositions = new Vector2[choices.Length];
            for (int i = 0; i < choices.Length; i++)
                ChoicePositions[i] = TextLoc + new Vector2(0, i * 10 - 4);
            Choice = new GUIChoice(ChoicePositions);

        }

        public void Update(float deltaTime, bool isInFocus)
        {
            Box.Update(deltaTime, true);
            Choice.Update(deltaTime, true);
            Text.Update(deltaTime, true);
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            Box.Draw(spriteBatch, true);
            Choice.Draw(spriteBatch, true);
            Text.Draw(spriteBatch, true);
        }
    }
}
