using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Player
    {
        public ControllerSteering2D Steering { get; set; }
        public Movement2D Movement { get; set; }
        public Transform2D Transform { get; set; }
        public FourDirSprite2D Sprite { get; set; }

        public Party ActiveParty { get; set; }
        public List<Member> CampMembers { get; set; }

        public bool Frozen { get; set; }

        [JsonConstructor]
        public Player(
            ControllerSteering2D steering, 
            Movement2D movement, 
            Transform2D transform, 
            FourDirSprite2D sprite,
            Party activeParty,
            List<Member> campMembers)
        {
            Steering = steering;
            Movement = movement;
            Transform = transform;
            Sprite = sprite;
            ActiveParty = activeParty;
            CampMembers = campMembers;
        }

        public Player(Vector2 position)
        {
            Steering = new ControllerSteering2D(30f);
            Movement = new Movement2D(1f, 5f);
            Transform = new Transform2D(position, 8f);
            Sprite = new FourDirSprite2D("CharacterBase", new Point(-16, -24));
            Frozen = false;

            ActiveParty = GameContent.Parties["PlayerParty"];

            CampMembers = new List<Member>();
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
                Movement.Stop();
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
