using Microsoft.Xna.Framework.Graphics;
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
        Player Player;

        public GameStateField()
        {
            Camera = new Camera2D();
            Player = new Player(new Vector2(200, 200));
        }

        public override void Enter(TheParty client)
        {

        }

        public override void Update(TheParty client, float deltaTime)
        {
            Player.Update(client.CurrentMap.CollisionBoxes, client.CurrentMap.EntityTransforms, deltaTime);
            client.CurrentMap.Update(Player, deltaTime);

            if (client.CommandQueue.Empty)
            {
                // Consider moving this logic to Entity class
                var Entities = client.CurrentMap.EntityLayer.entities;
                foreach (var entity in Entities)
                {
                    if (entity.PlayerCanInteract(Player.Transform.Position, Player.Movement.Heading) &&
                        (entity.values["TriggerOnTouch"] == "true" || InputManager.JustPressed(Keys.Space)))
                    {
                        var Commands = ScriptInterpreter.Interpret(client, entity, entity.values["Script"]);
                        client.CommandQueue.AddCommands(Commands);
                        break;
                    }
                }
            }

            Camera.Update(client.CurrentMap.Size, Player.Transform.Position);
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            client.CurrentMap.Draw(Camera.Position, spriteBatch);
            Player.Draw(Camera.Position, spriteBatch);
        }

        public override void Exit(TheParty client)
        {

        }
    }
}
