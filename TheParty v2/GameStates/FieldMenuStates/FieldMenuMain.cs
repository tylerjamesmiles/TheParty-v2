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
            string[] ChoiceStrings = new[] { "Feed", "Heal", "Commit", "Equip", "Party", "Back", "Title" };
            
            bool[] ChoiceValidity = new bool[ChoiceStrings.Length];
            for (int i = 0; i < ChoiceValidity.Length; i++)
                ChoiceValidity[i] = true;

            List<Member> Members = client.ActiveMembers;

            ChoiceValidity[0] =
                !Members.TrueForAll(m => m.Hunger == m.MaxHunger) &&
                GameContent.Variables["FoodSupply"] > 0;
            ChoiceValidity[1] =
                !Members.TrueForAll(m => m.HP == m.MaxHP) &&
                !Members.TrueForAll(m => m.Hunger == 0);
            ChoiceValidity[4] = client.BackupMembers.Count > 0;

            Choices = new GUIChoiceBox(
                ChoiceStrings, 
                GUIChoiceBox.Position.BottomRight, 
                3, ChoiceValidity);
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
                    case 2: client.StateMachine.SetNewCurrentState(client, new FieldMenuCommit()); break;
                    case 3: client.StateMachine.SetNewCurrentState(client, new FieldMenuEquip(client.Player)); break;
                    case 4: client.StateMachine.SetNewCurrentState(client, new FieldMenuParty()); break;
                    case 5: client.Done = true; break;
                    case 6: client.Quit = true; break;
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
