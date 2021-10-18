using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TheParty_v2
{
    class ChooseMove : State<CommandBattle>
    {
        GUIDialogueBox Description;

        public override void Enter(CommandBattle client)
        {
            Member Selected = client.FromMember;
            bool[] ChoiceValidity = new bool[Selected.Moves.Count];
            for (int i = 0; i < Selected.Moves.Count; i++)
                ChoiceValidity[i] = client.MoveValidOnAnyone(Selected.Moves[i]);
                    
            client.MoveChoice = new GUIChoiceBox(Selected.MoveNames.ToArray(), GUIChoiceBox.Position.BottomRight, 2, ChoiceValidity);

            int CurrentChoice = client.MoveChoice.CurrentChoice;
            string DescrTxt = client.FromMember.Moves[CurrentChoice].Description;
            Description = new GUIDialogueBox(GUIDialogueBox.Position.SkinnyTop, new string[] { DescrTxt }, 0.01f);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MoveChoice.Update(deltaTime, true);

            if (client.MoveChoice.ChoiceUpdatedThisFrame)
            {
                int CurrentChoice = client.MoveChoice.CurrentChoice;
                string DescrTxt = client.FromMember.Moves[CurrentChoice].Description;
                Description = new GUIDialogueBox(GUIDialogueBox.Position.SkinnyTop, new string[] { DescrTxt }, 0.01f);
            }

            Description.Update(deltaTime, true);

            if (client.MoveChoice.Done)
            {
                client.CurrentMove = client.FromMember.Moves[client.MoveChoice.CurrentChoice];
                client.StateMachine.SetNewCurrentState(client, new ChooseTarget());
            }

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.MoveChoice.Draw(spriteBatch, true);
            Description.Draw(spriteBatch, true);
        }
    }
}
