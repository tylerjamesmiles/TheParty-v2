using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class GUIDialogueBox
    {
        private const int Height = 47;
        public static readonly Rectangle Top = new Rectangle(new Point(0, 0), new Point(GraphicsGlobals.ScreenSize.X, Height));
        public static readonly Rectangle SkinnyTop = new Rectangle(new Point(0, 0), new Point(GraphicsGlobals.ScreenSize.X, Height / 2 + 4));
        public static readonly Rectangle ReallySkinnyTop = new Rectangle(new Point(0, 0), new Point(GraphicsGlobals.ScreenSize.X, 17));
        public static readonly Rectangle Middle = new Rectangle(new Point(0, Height), new Point(GraphicsGlobals.ScreenSize.X, Height));
        public static readonly Rectangle Bottom = new Rectangle(new Point(0, Height * 2), new Point(GraphicsGlobals.ScreenSize.X, Height));
        public enum Position { Top, SkinnyTop, ReallySkinnyTop, Middle, Bottom };

        GUIBox Box;
        GUIText Text;

        string[] Dialogues;
        int CurrentDialgoue;
        public bool Done { get; private set; }
        public bool TextShown => Text.Done;
        public void SetNewText(string newText) => Text.ChangeToNewText(newText);

        public GUIDialogueBox(Rectangle bounds, string[] dialogues, float popInRate = 0.1f)
        {
            Rectangle BoxBounds = bounds;
            Box = new GUIBox(BoxBounds);
            Dialogues = dialogues;
            CurrentDialgoue = 0;
            Done = false;
            Text = new GUIText(Dialogues[0], (BoxBounds.Location + new Point(4, 4)).ToVector2(), BoxBounds.Size.X - 8, popInRate);
        }

        public GUIDialogueBox(Position pos, string[] dialogues, float popInRate = 0.1f)
        {
            Rectangle BoxBounds =
                pos == Position.Top ? Top :
                pos == Position.SkinnyTop ? SkinnyTop :
                pos == Position.ReallySkinnyTop ? ReallySkinnyTop :
                pos == Position.Middle ? Middle :
                pos == Position.Bottom ? Bottom :
                new Rectangle();

            Box = new GUIBox(BoxBounds);

            Dialogues = dialogues;
            CurrentDialgoue = 0;
            Done = false;

            Text = new GUIText(Dialogues[0], (BoxBounds.Location + new Point(4, 4)).ToVector2(), BoxBounds.Size.X - 8, popInRate);
        }

        public void Update(float deltaTime, bool isInFocus)
        {
            Box.Update(deltaTime, true);

            if (Box.Grown)
                Text.Update(deltaTime, true);

            if (InputManager.JustReleased(Keys.Space))
            {
                if (Text.Done)
                {
                    if (CurrentDialgoue == Dialogues.Length - 1)
                    {
                        Done = true;
                    }
                    else
                    {
                        CurrentDialgoue++;
                        Text.ChangeToNewText(Dialogues[CurrentDialgoue]);
                    }
                }
                else
                {
                    Text.FullyDisplay();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            Box.Draw(spriteBatch, true);
            Text.Draw(spriteBatch, true);
        }
    }
}
