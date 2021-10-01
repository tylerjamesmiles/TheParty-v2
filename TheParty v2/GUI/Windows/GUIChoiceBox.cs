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
        List<GUIText> Texts;
        bool[] ChoiceValidity;

        public int CurrentChoice => Choice.CurrentChoiceIdx;
        public int NumChoices => Choice.NumChoices;
        public bool Done => Box.Done;

        public enum Position
        {
            Center,
            BottomLeft,
            BottomRight
        }

        public GUIChoiceBox(string[] choices, Position position, int numColumns = 1, bool[] choiceValidity = null)
        {
            Vector2 ScreenSize = GraphicsGlobals.ScreenSize.ToVector2();
            Vector2 ScreenCenter = ScreenSize / 2;

            ChoiceValidity = choiceValidity;

            // Create Collumn Strings
            int ChoicesPerColumn = (numColumns == 1) ? choices.Length : (choices.Length + 1) / numColumns;
            string[] Collumns = new string[numColumns];
            int CurrentChoiceIdx = 0;
            for (int c = 0; c < numColumns; c++)
            {
                Collumns[c] = "";
                for (int i = 0; i < ChoicesPerColumn; i++)
                {
                    if (CurrentChoiceIdx < choices.Length)
                    {
                        Collumns[c] += choices[CurrentChoiceIdx];
                        if (i < ChoicesPerColumn - 1)
                            Collumns[c] += " \n";
                        CurrentChoiceIdx++;
                    } 
                }
            }

            // Set total Text Size
            Vector2 TextSize = Vector2.Zero;
            List<float> CollumnXOffsets = new List<float>();
            foreach (string collumn in Collumns)
            {
                Vector2 CollumnSize = GameContent.Font.MeasureString(collumn);
                CollumnXOffsets.Add(TextSize.X + 3);
                TextSize.X += CollumnSize.X + 3;
                if (CollumnSize.Y > TextSize.Y)
                    TextSize.Y = CollumnSize.Y;
            }

            Vector2 TextTL =
                position == Position.Center ? ScreenCenter - TextSize / 2 :
                position == Position.BottomLeft ? new Vector2(4, ScreenSize.Y - TextSize.Y) :
                position == Position.BottomRight ? new Vector2(ScreenSize.X - TextSize.X - 8, ScreenSize.Y - TextSize.Y - 8) :
                Vector2.Zero;

            Texts = new List<GUIText>();
            for (int c = 0; c < Collumns.Length; c++)
            {
                Vector2 CollumnTL = TextTL + new Vector2(CollumnXOffsets[c], 0);
                Texts.Add(new GUIText(Collumns[c], CollumnTL, int.MaxValue, float.MinValue));
            }

            Point BoxLoc = TextTL.ToPoint() + new Point(-4, -4);
            Point BoxSize = TextSize.ToPoint() + new Point(8, 8);
            Rectangle BoxBounds = new Rectangle(BoxLoc, BoxSize);
            Box = new GUIBox(BoxBounds);

            List<Vector2> ChoicePositions = new List<Vector2>();
            int ChoiceIdx = 0;
            for (int c = 0; c < numColumns; c++)
                for (int i = 0; i < ChoicesPerColumn; i++)
                {
                    if (ChoiceIdx >= choices.Length)
                        break;
                    
                    ChoicePositions.Add(TextTL + new Vector2(CollumnXOffsets[c] + 3, i * 10 - 4));
                    
                    ChoiceIdx++;
                }

            Choice = new GUIChoice(ChoicePositions.ToArray());

        }

        public void Update(float deltaTime, bool isInFocus)
        {
            Box.Update(deltaTime, true);
            bool OnLegalChoice = ChoiceValidity == null || ChoiceValidity[Choice.CurrentChoiceIdx];
            Choice.Update(deltaTime, Box.Grown, OnLegalChoice);
            Texts.ForEach(t => t.Update(deltaTime, Box.Grown));

            if (!Box.Shrink && Choice.Done)
                Box.StartShrink();
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            Box.Draw(spriteBatch, true);

            if (Box.Grown && !Box.Shrink)
            {
                Texts.ForEach(t => t.Draw(spriteBatch, true));
                Choice.Draw(spriteBatch, true);
            }

        }
    }
}
