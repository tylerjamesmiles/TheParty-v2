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

        public Party ActiveParty;
        public List<Member> CampMembers;

        public bool Frozen;

        public Player(Vector2 position)
        {
            Steering = new ControllerSteering2D(30f);
            Movement = new Movement2D(1f, 5f);
            Transform = new Transform2D(position, 8f);
            Sprite = new FourDirSprite2D("CharacterBase", new Point(-16, -24));
            Frozen = false;

            ActiveParty = GameContent.Parties["PlayerParty"];

            CampMembers = new List<Member>(ActiveParty.Members);
            


        }

        public void Update(List<Rectangle> collisionBoxes, List<Transform2D> entityTransforms, float deltaTime)
        {
            if (!Frozen)
            {
                Steering.Update();
                Movement.Update(Steering.SteeringForce, deltaTime);
            }
            else
            {
                Movement.Velocity = new Vector2(0, 0);
            }

            Transform.Update(Movement.Velocity, collisionBoxes, entityTransforms);
            Sprite.Update(Movement.Velocity, deltaTime);
        }

        public void Draw(Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
        }
    }
}
