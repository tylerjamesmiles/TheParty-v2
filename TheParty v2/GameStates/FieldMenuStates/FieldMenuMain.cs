using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuMain : State<GameStateFieldMenu>
    {
        GUIChoiceBox Choices;

        public override void Enter(GameStateFieldMenu client)
        {
            bool[] ChoiceValidity = new bool[5];
            for (int i = 0; i < ChoiceValidity.Length; i++)
                ChoiceValidity[i] = true;

            ChoiceValidity[2] = client.BackupMembers.Count > 0;

            Choices = new GUIChoiceBox(
                new[] { "Feed", "Heal", "Party", "Back", "Title" }, 
                GUIChoiceBox.Position.BottomRight, 
                2, ChoiceValidity);
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Choices.Update(deltaTime, true);

            if (Choices.Done)
            {
                switch(Choices.CurrentChoice)
                {
                    case 0: client.StateMachine.SetNewCurrentState(client, new FieldMenuFeed()); break;
                    case 1: client.StateMachine.SetNewCurrentState(client, new FieldMenuHeal()); break;
                    case 2: client.StateMachine.SetNewCurrentState(client, new FieldMenuParty()); break;
                    case 3: client.Done = true; break;
                    case 4: client.Quit = true; break;
                }
            }

            if (InputManager.JustReleased(Keys.Escape))
                client.Done = true;
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Choices.Draw(spriteBatch, true);
        }
    }
}
