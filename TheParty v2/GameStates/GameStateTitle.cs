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

        List<Particle> Particles;

        public override void Enter(TheParty client)
        {
            Choice = new GUIChoiceBox(new[] { "New", "Load", "Quit" }, GUIChoiceBox.Position.BottomRight);

            Particles = new List<Particle>();
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Choice.Update(deltaTime, true);
            if (Choice.Done)
            {
                switch (Choice.CurrentChoice)
                {
                    case 0:
                        client.Player = new Player(new Vector2());

                        client.CommandQueue.EnqueueCommand(new CommandTeleport("WorldMap", 99, 101));
                        client.CommandQueue.EnqueueCommand(new CommandFade(CommandFade.Direction.In));
                        client.CommandQueue.EnqueueCommand(new CommandFreezePlayer());
                        client.CommandQueue.EnqueueCommand(new CommandDialogue(GUIDialogueBox.Position.SkinnyTop, "100 days until the world ends."));
                        client.CommandQueue.EnqueueCommand(new CommandUnfreezePlayer());
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

            if (InputManager.JustReleased(Keys.P))
            {


            }


            Particles.ForEach(p => p.Update(deltaTime));
            Particles.RemoveAll(p => p.Offscreen);

        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites["Title"],
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                Color.White);

            Particles.ForEach(p => p.Draw(spriteBatch));

            Choice.Draw(spriteBatch, true);
        }

        public override void Exit(TheParty client)
        {

        }
    }
}
