﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TheParty_v2
{
    class ChooseMove : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            Member Selected = client.FromMember;
            bool[] ChoiceValidity = new bool[Selected.Moves.Length];
            for (int i = 0; i < Selected.Moves.Length; i++)
                ChoiceValidity[i] = client.MoveValidOnAnyone(Selected.Moves[i]);
                    
            client.MoveChoice = new GUIChoiceBox(Selected.MoveNames(), GUIChoiceBox.Position.BottomRight, ChoiceValidity);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MoveChoice.Update(deltaTime, true);

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
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
