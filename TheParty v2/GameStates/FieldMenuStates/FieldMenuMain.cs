﻿using Microsoft.Xna.Framework.Graphics;
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
            Choices = new GUIChoiceBox(new[] { "Feed", "Heal", "Save", "Quit" }, GUIChoiceBox.Position.BottomRight, 2);
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
                    case 2: break;
                    case 3: break;
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
