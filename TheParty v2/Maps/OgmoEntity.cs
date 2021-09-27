using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class OgmoEntity
    {
        public string name;
        public int id;
        public string _eid;
        public int x;
        public int y;
        public int originX;
        public int originY;
        public Dictionary<string, string> values;

        public Transform2D Transform;
        public Movement2D Movement;
        public FourDirSprite2D Sprite;
        public bool Frozen;
        public bool FacePlayer;

        const float MinInteractDist = 20f;

        public void Initialize()
        {
            Transform = new Transform2D(new Vector2(x, y), 6f);
            Movement = new Movement2D(1f, 20f);
            Sprite = new FourDirSprite2D(values["SpriteName"], new Point(-16, -24));
            Frozen = false;
        }

        public void Update(List<Rectangle> collisionBoxes, List<Transform2D> entityTransforms, Player player, float deltaTime)
        {
            Vector2 Steering = new Vector2(5f, 0f);

            if (Frozen)
            {
                Movement.Velocity = new Vector2(0, 0);
                Sprite.CurrentFrame = 0;
            }
            else
            {
                Movement.Update(Steering, deltaTime);
                Transform.Update(Movement.Velocity, collisionBoxes, entityTransforms, values["Solid"] == "true");
                Sprite.Update(Movement.Velocity, deltaTime);
            }
            
            if (FacePlayer)
            {
                Vector2 ToPlayer = player.Transform.Position - Transform.Position;
                Sprite.CurrentFacing = MathUtility.GeneralDirection(ToPlayer);
            }

        }

        public void Draw( Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
        }

        public bool PlayerCanInteract(Vector2 playerPos, Vector2 playerHeading)
        {
            Vector2 ToMe = Transform.Position - playerPos;
            bool CloseEnough = ToMe.LengthSquared() < MinInteractDist * MinInteractDist;
            float Dot = Vector2.Dot(Vector2.Normalize(ToMe), playerHeading);
            bool Facing = Dot > 0f;
            return CloseEnough && Facing;
        }
    }
}
