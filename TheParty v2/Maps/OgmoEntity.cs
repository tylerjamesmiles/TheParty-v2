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
        public bool Exists;

        public Timer CanInteractTimer;
        const float MinInteractDist = 20f;

        public void Initialize()
        {
            Transform = new Transform2D(new Vector2(x, y), 6f);
            Movement = new Movement2D(1f, 20f);
            Sprite = new FourDirSprite2D(values["SpriteName"], new Point(-16, -24));
            Frozen = false;
        }

        private bool IExist()   // <- Deep questions
        {
            if (!values.ContainsKey("ExistsIf"))
                return true;

            string Condition = values["ExistsIf"];

            if (Condition == "")
                return true;

            string[] Keywords = Condition.Split(' ');
            string VarName = Keywords[0];
            string Operator = Keywords[1];
            string RHValString = Keywords[2];

            if (GameContent.Switches.ContainsKey(VarName))
            {
                bool LHVal = GameContent.Switches[VarName];
                bool RHVal = bool.Parse(RHValString);
                switch (Operator)
                {
                    case "==": return LHVal == RHVal;
                    case "!=": return LHVal != RHVal;
                    default: throw new Exception("Invalid operator.");
                }
            }
            else if (GameContent.Variables.ContainsKey(VarName))
            {
                int LHVal = GameContent.Variables[VarName];
                int RHVal = int.Parse(RHValString);
                switch (Operator)
                {
                    case "==": return LHVal == RHVal;
                    case "!=": return LHVal != RHVal;
                    case "<": return LHVal < RHVal;
                    case ">": return LHVal > RHVal;
                    case "<=": return LHVal <= RHVal;
                    case ">=": return LHVal >= RHVal;
                    default: throw new Exception("Invalid operator.");
                }
            }
            else
                throw new Exception("Invalid variable name.");
        }

        public void Update(List<Rectangle> collisionBoxes, List<Transform2D> entityTransforms, Player player, float deltaTime)
        {
            Exists = IExist();

            if (!Exists)
                return;

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
                Sprite.CurrentFacing = Utility.GeneralDirection(ToPlayer);
            }

        }

        public void Draw( Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            if (Exists)
                Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
        }

        public bool PlayerCanInteract(Vector2 playerPos, Vector2 playerHeading)
        {
            Vector2 ToMe = Transform.Position - playerPos;
            bool CloseEnough = ToMe.LengthSquared() < MinInteractDist * MinInteractDist;
            float Dot = Vector2.Dot(ToMe, playerHeading);
            bool Facing = Dot >= 0f;
            return CloseEnough && Facing;
        }
    }
}
