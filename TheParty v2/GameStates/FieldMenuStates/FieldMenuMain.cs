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
        bool Quitting;

        public override void Enter(GameStateFieldMenu client)
        {
            string[] ChoiceStrings = new[] { 
                "Member", 
                "Equip", 
                "Party", 
                "Save", 
                "Options",
                "Title" };
            
            bool[] ChoiceValidity = new bool[ChoiceStrings.Length];
            for (int i = 0; i < ChoiceValidity.Length; i++)
                ChoiceValidity[i] = true;

            ChoiceValidity[2] = client.BackupMembers.Count > 0;

            List<Member> Members = client.ActiveMembers;

            Choices = new GUIChoiceBox(
                ChoiceStrings, 
                GUIChoiceBox.Position.BottomRight, 
                2, ChoiceValidity);

            Choices.SetCurrentChoice(client.PreviousMainMenuChoice);

            Quitting = false;
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Choices.Update(deltaTime, true);

            if (Choices.Done)
            {
                switch(Choices.CurrentChoice)
                {
                    case 0: client.StateMachine.SetNewCurrentState(client, new FieldMenuMember()); break;
                    case 1: client.StateMachine.SetNewCurrentState(client, new FieldMenuEquip(client.Player)); break;
                    case 2: client.StateMachine.SetNewCurrentState(client, new FieldMenuParty()); break;
                    case 3: client.Save = true; break;
                    case 4: Enter(client); break;       // TODO: Settings
                    case 5: client.Done = true; break;
                }

                client.PreviousMainMenuChoice = Choices.CurrentChoice;
            }

            if (InputManager.JustReleased(Keys.Escape))
            {
                Choices.StartShrink();
                Quitting = true;
            }

            if (Choices.Done && Quitting)
                client.Done = true;
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Choices.Draw(spriteBatch, true);
        }
    }
}
