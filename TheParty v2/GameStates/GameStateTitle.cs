using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class GameStateTitle : State<TheParty>
    {
        GUIChoiceBox Choice;

        public override void Enter(TheParty client)
        {
            Choice = new GUIChoiceBox(new[] { "New", "Load", "Quit" }, GUIChoiceBox.Position.BottomRight);
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Choice.Update(deltaTime, true);
            if (Choice.Done)
            {
                switch (Choice.CurrentChoice)
                {
                    case 0:
                        //client.CurrentMap = GameContent.Maps["Camp"];
                        //client.Player = new Player(new Vector2(9, 6) * 16 + new Vector2(8, 8));

                        client.CurrentMap = GameContent.Maps["WorldMap"];
                        client.Player = new Player(new Vector2(90, 93) * 16 + new Vector2(8, 8));

                        client.CommandQueue.EnqueueCommand(new CommandFade(CommandFade.Direction.In));
                        client.StateMachine.SetNewCurrentState(client, new GameStateField());
                        break;

                    case 1:
                        client.CommandQueue.EnqueueCommand(new CommandLoad());
                        client.StateMachine.SetNewCurrentState(client, new GameStateField());
                        break;

                    case 2:
                        client.Exit();
                        break;
                }
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites["Title"],
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                Color.White);

            Choice.Draw(spriteBatch, true);
        }

        public override void Exit(TheParty client)
        {

        }
    }
}
