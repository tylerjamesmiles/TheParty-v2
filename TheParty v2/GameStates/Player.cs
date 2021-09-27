using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Player
    {
        public ControllerSteering2D Steering;
        public Movement2D Movement;
        public Transform2D Transform;
        public FourDirSprite2D Sprite;

        public Player(Vector2 position)
        {
            Steering = new ControllerSteering2D(30f);
            Movement = new Movement2D(1f, 15f);
            Transform = new Transform2D(position, 8f);
            Sprite = new FourDirSprite2D("CharacterBase", new Point(-16, -24));
        }

        public void Update(List<Rectangle> collisionBoxes, List<Transform2D> entityTransforms, float deltaTime)
        {
            Steering.Update();
            Movement.Update(Steering.SteeringForce, deltaTime);
            Transform.Update(Movement.Velocity, collisionBoxes, entityTransforms);
            Sprite.Update(Movement.Velocity, deltaTime);
        }

        public void Draw(Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
        }
    }
}
