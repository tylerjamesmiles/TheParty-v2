using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheParty_v2
{
    class GameStateTitle : State<TheParty>
    {
        GUIChoiceBox Choice;

        List<Particle> Particles;

        public override void Enter(TheParty client)
        {
            GameContent.PlaySong("TitleScreen");
            GameContent.FadeInMusic();

            GameContent.LoadData(); // sets everything to blank slate values
                                    // on Load, more specific data will be drawn

            bool[] ChoiceValidity = new bool[3];
            ChoiceValidity[0] = true;
            ChoiceValidity[1] = File.Exists("SaveFile.json");
            ChoiceValidity[2] = true;

            Choice = new GUIChoiceBox(
                new[] { "New", "Load", "Quit" }, 
                GUIChoiceBox.Position.BottomRight,
                1,
                ChoiceValidity);



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

                        //client.CommandQueue.EnqueueCommand(new CommandFade(CommandFade.Direction.Out));
                        //client.CommandQueue.EnqueueCommand(new CommandBeFaded());
                        //client.CommandQueue.EnqueueCommand(new CommandFadeOutMusic());
                        //client.CommandQueue.EnqueueCommand(new CommandWait(2f));
                        //client.CommandQueue.EnqueueCommand(new CommandTeleport("WorldMap", 98, 59));
                        //client.CommandQueue.EnqueueCommand(new CommandBringMusicVolumeBackUp());
                        //client.CommandQueue.EnqueueCommand(new CommandShowScreen());
                        //client.CommandQueue.EnqueueCommand(new CommandFade(CommandFade.Direction.In));
                        //client.CommandQueue.EnqueueCommand(new CommandUnfreezePlayer());
                        //client.CommandQueue.EnqueueCommand(new CommandWASD());
                        //client.StateMachine.SetNewCurrentState(client, new GameStateField());

                        client.CommandQueue.EnqueueCommand(new CommandBattle("WorldMapBattle1", "TestSwitch1"));
                        break;

                    case 1:
                        client.CommandQueue.EnqueueCommand(new CommandFade(CommandFade.Direction.Out));
                        client.CommandQueue.EnqueueCommand(new CommandBeFaded());
                        client.CommandQueue.EnqueueCommand(new CommandFadeOutMusic());
                        client.CommandQueue.EnqueueCommand(new CommandWait(2f));
                        client.CommandQueue.EnqueueCommand(new CommandLoad());
                        client.CommandQueue.EnqueueCommand(new CommandBringMusicVolumeBackUp());
                        client.CommandQueue.EnqueueCommand(new CommandShowScreen());
                        client.CommandQueue.EnqueueCommand(new CommandFade(CommandFade.Direction.In));
                        client.CommandQueue.EnqueueCommand(new CommandUnfreezePlayer());

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
                new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize),
                Color.White);

            Particles.ForEach(p => p.Draw(spriteBatch));

            Choice.Draw(spriteBatch, true);
        }

        public override void Exit(TheParty client)
        {

        }
    }
}
