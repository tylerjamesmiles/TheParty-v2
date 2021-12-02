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
        public PathFollower2D PathFollower { get; set; }

        public Party ActiveParty { get; set; }
        public List<Member> CampMembers { get; set; }

        public List<string> Inventory { get; set; }

        public bool Frozen { get; set; }

        [JsonConstructor]
        public Player(
            ControllerSteering2D steering, 
            Movement2D movement, 
            Transform2D transform, 
            FourDirSprite2D sprite,
            Party activeParty,
            List<Member> campMembers,
            List<string> inventory)
        {
            Steering = steering;
            Movement = movement;
            Transform = transform;
            Sprite = sprite;
            ActiveParty = activeParty;
            CampMembers = campMembers;
            Inventory = inventory;
            PathFollower = new PathFollower2D(new Queue<Vector2>(), false);
        }

        public Player(Vector2 position)
        {
            Steering = new ControllerSteering2D(30f);
            Movement = new Movement2D(1f, 5f);
            Transform = new Transform2D(position, 7f);
            Sprite = new FourDirSprite2D("CharacterBase", new Point(-16, -24));
            Frozen = false;

            ActiveParty = GameContent.Parties["PlayerParty"];

            Inventory = new List<string>();

            CampMembers = GameContent.Parties["BackupParty"].Members;

            PathFollower = new PathFollower2D(new Queue<Vector2>(), false);

        }

        public void Update(List<Rectangle> collisionBoxes, List<Transform2D> entityTransforms, float deltaTime)
        {

 
            Vector2 SteeringForce = new Vector2();
            if (PathFollower.Done)
            {
                if (!Frozen)
                {
                    Steering.Update();
                    SteeringForce = Steering.SteeringForce;
                }
            }
            else
            {
                PathFollower.Update(Transform.Position, 27f, deltaTime);
                SteeringForce = PathFollower.SteeringForce;
            }
                    
            Movement.Update(SteeringForce, deltaTime);
            

            Transform.Update(Movement.Velocity, collisionBoxes, entityTransforms);
            Sprite.Update(Movement.Velocity, deltaTime);
        }

        public void Draw(Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
        }
    }
}
