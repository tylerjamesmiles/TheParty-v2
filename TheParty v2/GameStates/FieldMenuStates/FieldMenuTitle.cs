using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuTitle : State<GameStateFieldMenu>
    {
        GUIDialogueBox Description;
        GUIChoiceBox Choice;

        public FieldMenuTitle()
        {
        }

        public override void Enter(GameStateFieldMenu client)
        {
            Description = new GUIDialogueBox(
                GUIDialogueBox.Position.SkinnyTop,
                new[] { "Are you sure? You will lose any unsaved progress." },
                0.05f, false);

            Choice = new GUIChoiceBox(new[] { "Back", "Quit" }, GUIChoiceBox.Position.Center);
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Description.Update(deltaTime, true);
            Choice.Update(deltaTime, true);

            if (Choice.Done)
            {
                switch(Choice.CurrentChoice)
                {
                    case 0: client.StateMachine.SetNewCurrentState(client, new FieldMenuMain()); break;
                    case 1: client.Quit = true; break;
                }
            }
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Description.Draw(spriteBatch, true);
            Choice.Draw(spriteBatch, true);
        }
    }
}
