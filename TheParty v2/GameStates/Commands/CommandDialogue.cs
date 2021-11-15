using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandDialogue : Command<TheParty>
    {
        string[] InputDialogues;
        GUIDialogueBox Box;
        GUIDialogueBox.Position Pos;

        public CommandDialogue(params string[] dialogues)
        {
            InputDialogues = dialogues;
            Pos = GUIDialogueBox.Position.Bottom;
        }

        public CommandDialogue(GUIDialogueBox.Position pos, params string[] dialogues)
        {
            InputDialogues = dialogues;
            Pos = pos;
        }

        public override void Enter(TheParty client)
        {
            string[] CorrectedDialogues = new string[InputDialogues.Length];
            for (int i = 0; i < InputDialogues.Length; i++)
            {
                string str = InputDialogues[i];
                string[] words = str.Split(' ');
                foreach (string word in words)
                {
                    if (word[0] == '*')
                    {
                        string VarName = word.Substring(1);
                        if (GameContent.Variables.ContainsKey(VarName))
                            str = str.Replace(word, GameContent.Variables[VarName].ToString());
                    }
                }
                CorrectedDialogues[i] = str;
            }

            Box = new GUIDialogueBox(Pos, CorrectedDialogues);
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Box.Update(deltaTime, true);

            Done = Box.Done;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            Box.Draw(spriteBatch, true);
        }
    }
}
