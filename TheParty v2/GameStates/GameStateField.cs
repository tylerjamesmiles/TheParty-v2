﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace TheParty_v2
{ 
    class GameStateField : State<TheParty>
    {
        Camera2D Camera;



        public GameStateField()
        {
            Camera = new Camera2D();
        }

        public override void Update(TheParty client, float deltaTime)
        {
            client.CurrentMap.Update(client.Player, deltaTime);
            client.Player.Update(client.CurrentMap.CollisionBoxes, client.CurrentMap.EntityTransforms, deltaTime);

            if (client.CommandQueue.Empty)
            {
                client.EventsCanHappenTimer.Update(deltaTime);

                var Entities = client.CurrentMap.EntityLayer.entities;
                foreach (var entity in Entities)
                {
                    if (client.EventsCanHappenTimer.TicsSoFar >= 1 &&
                        entity.Exists &&
                        entity.PlayerCanInteract(client.Player.Transform.Position, client.Player.Movement.Heading) &&
                        (entity.values["TriggerOnTouch"] == "true" || InputManager.JustReleased(Keys.Space)))
                    {
                        var Commands = ScriptInterpreter.Interpret(client, entity, entity.values["Script"]);
                        client.CommandQueue.EnqueueCommands(Commands);
                        break;
                    }
                }

                if (InputManager.JustReleased(Keys.Escape))
                    client.StateMachine.SetNewCurrentState(client, new GameStateFieldMenu());
            }

            Camera.Update(client.CurrentMap.Size, client.Player.Transform.Position);
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            client.CurrentMap.Draw(Camera.Position, client.Player, spriteBatch);
        }
    }
}
