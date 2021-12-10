using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class GUIText
    {
        string Text;
        string CurrentDisplayText;
        int CurrentChar;
        Timer NewCharTimer;
        Vector2 Location;
        bool DrawLight;
        public bool Done => CurrentChar == Text.Length - 1;

        public GUIText(string text, Vector2 location, int lineWidth, float popInRate, bool drawLight = false)
        {
            Text = WrappedByLineWidth(text, lineWidth);
            CurrentDisplayText = "";
            CurrentChar = 0;
            NewCharTimer = new Timer(popInRate);
            Location = location.ToPoint().ToVector2();  // had issues w shaky text
            DrawLight = drawLight;
        }

        public void FullyDisplay()
        {
            CurrentDisplayText = Text;
            CurrentChar = Text.Length - 1;
        }

        public void ChangeToNewText(string newText)
        {
            CurrentDisplayText = "";
            CurrentChar = 0;
            Text = newText;
        }

        public static string WrappedByLineWidth(string text, int maxLineWidth)
        {
            string ResultText = "";

            string[] Words = text.Split(' ');

            List<string> Lines = new List<string>();
            Lines.Add(Words[0]);
            int CurrentWordIdx = 1;


            while (CurrentWordIdx < Words.Length)
            {
                string CurrentWord = Words[CurrentWordIdx];
                string CurrentLine = Lines[Lines.Count - 1];
                string LineWithWord = CurrentLine + ' ' + CurrentWord;

                if (GameContent.Font.MeasureString(LineWithWord).X < maxLineWidth)
                    Lines[Lines.Count - 1] += ' ' + CurrentWord;
                else
                    Lines.Add(CurrentWord);

                CurrentWordIdx += 1;
            }

            foreach (string Line in Lines)
                ResultText += Line + '\n';

            return ResultText;
        }

        public void Update(float deltaTime, bool isInFocus)
        {
            if (!isInFocus)
                return;

            NewCharTimer.Update(deltaTime);

            if (NewCharTimer.TicThisFrame && CurrentChar < Text.Length - 1)
                CurrentChar += 1;

            if (CurrentChar == Text.Length - 1)
                CurrentDisplayText = Text;
            else if (Text.Length > 0)
                CurrentDisplayText = Text.Remove(CurrentChar);
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            SpriteFont Font = (DrawLight) ? GameContent.FontLight : GameContent.Font;
            spriteBatch.DrawString(Font, CurrentDisplayText, Location, Color.White);
        }
    }
}
