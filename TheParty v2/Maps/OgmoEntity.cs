using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TheParty_v2
{
    class PathFollower2D
    {
        Queue<Vector2> PointsBackStore;
        Queue<Vector2> Points;
        public Vector2 SteeringForce { get; private set; }
        static float MinPointDistance = 5f;
        private bool Repeat;
        public bool Done { get; private set; }

        public PathFollower2D()
        {
            PointsBackStore = new Queue<Vector2>();
            Points = new Queue<Vector2>();
            SteeringForce = new Vector2();
            Repeat = false;
            Done = false;
        }

        public PathFollower2D(string pathScript, Vector2 startingPos, bool repeat)
        {
            string Path = pathScript;
            string[] PathCommands = Path.Split('\n');
            Queue<Vector2> PathPoints = new Queue<Vector2>();

            for (int i = 0; i < PathCommands.Length; i++)
            {
                Vector2 LastPoint = i > 0 ? PathPoints.Peek() : startingPos;

                string Line = PathCommands[i];

                if (Line == "")
                    continue;

                string[] Keywords = Line.Split(' ');
                string Command = Keywords[0];
                string[] Arguments = new string[Keywords.Length - 1];
                for (int j = 1; j < Keywords.Length; j++)
                    Arguments[j - 1] = Keywords[j];

                float Amt = float.Parse(Arguments[0]) * 16;

                switch (Command.ToLower())
                {
                    case "up": PathPoints.Enqueue(LastPoint + new Vector2(0, -Amt)); break;
                    case "down": PathPoints.Enqueue(LastPoint + new Vector2(0, +Amt)); break;
                    case "left": PathPoints.Enqueue(LastPoint + new Vector2(-Amt, 0)); break;
                    case "right": PathPoints.Enqueue(LastPoint + new Vector2(+Amt, 0)); break;
                    case "to":
                        PathPoints.Enqueue(
                            new Vector2(
                                float.Parse(Arguments[0]) * 16 + 8,
                                float.Parse(Arguments[1]) * 16 + 8));
                        break;
                }
            }

            Points = new Queue<Vector2>(PathPoints);
            PointsBackStore = new Queue<Vector2>(PathPoints); // used when looping the path
            Repeat = repeat;
            Done = false;
        }

        public PathFollower2D(Queue<Vector2> points, bool repeat)
        {
            Points = new Queue<Vector2>(points);
            PointsBackStore = new Queue<Vector2>(points); // used when looping the path
            Repeat = repeat;
            Done = false;
        }

        public void Update(Vector2 currentPos, float maxSpeed, float deltaTime)
        {
            if (Points.Count > 0)
            {
                Vector2 ToNextPoint = Points.Peek() - currentPos;
                if (ToNextPoint.LengthSquared() < MinPointDistance * MinPointDistance)
                {
                    Points.Dequeue();
                }
            }

            if (Points.Count == 0)
            {
                if (Repeat)
                    Points = new Queue<Vector2>(PointsBackStore);
                else
                    Done = true;
            }

            SteeringForce = new Vector2();
            if (Points.Count > 0)
            {
                Vector2 ToNextPoint = Points.Peek() - currentPos;
                SteeringForce = Vector2.Normalize(ToNextPoint) * maxSpeed;
            }
        }
    }


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
        public PathFollower2D PathFollower;
        public FourDirSprite2D Sprite;      // for characters
        public AnimatedSprite2D AnimatedSprite; // for coins/meats
        public bool Frozen;
        public bool FacePlayer;
        public bool ChasePlayer;
        public bool FollowPath;
        public bool TemporaryFollowPath;
        public float VisionRange = 60f;
        public bool ManualExists;
        public bool Exists => IExist();

        float MinInteractDist;

        public int EntityId;
        static int NextEID = 0;

        public void Initialize()
        {
            Vector2 Pos = new Vector2(x + 8, y + 8);
            Transform = new Transform2D(Pos, 5f);

            EntityId = NextEID;
            NextEID++;

            if (name == "Coin" || name == "Food")
            {
                Frozen = false;

                if (name == "Coin")
                    AnimatedSprite = new AnimatedSprite2D("Coin", new Point(16, 16), Pos, new Vector2(-8, -8));
                else if (name == "Food")
                    AnimatedSprite = new AnimatedSprite2D("MeatField", new Point(16, 16), Pos, new Vector2(-8, -8));

                AnimatedSprite.AddAnimation("Loop", 0, 8, 0.15f);
                AnimatedSprite.SetCurrentAnimation("Loop");
                AnimatedSprite.GoToRandomFrame();

                Movement = new Movement2D(1f, 26f);

                values = new Dictionary<string, string>();
                values.Add("Solid", "false");
                values.Add("TriggerOnTouch", "true");
                values.Add("Path", "");
                values.Add("RepeatPath", "false");

                if (name == "Coin")
                {
                    string Script =
                        "Incr(Money, 1)\n" +
                        "CollectAnimation(Coin)\n" +
                        "Erase(Me)\n"
                        ;
                    values.Add("Script", Script);
                    values.Add("Name", "Coin" + EntityId.ToString());

                    //EntityId++;
                }
                else if (name == "Food")
                {
                    string Script =
                        "Incr(FoodSupply, 1)\n" +
                        "CollectAnimation(Meat)\n" +
                        "Erase(Me)\n"
                        ;
                    values.Add("Script", Script);
                    values.Add("Name", "Food" + EntityId.ToString());
                    //EntityId++;
                }

                FollowPath = true;
            }
            else
            {
                Movement = new Movement2D(1f, 26f);
                Sprite = new FourDirSprite2D(values["SpriteName"], new Point(-16, -24), values["AnimateWhenStatic"] == "true");
                Frozen = false;

                if (values.ContainsKey("PassiveBehavior"))
                {
                    if (values["PassiveBehavior"] == "FaceUp") Sprite.CurrentFacing = 0;
                    if (values["PassiveBehavior"] == "FaceDown") Sprite.CurrentFacing = 1;
                    if (values["PassiveBehavior"] == "FaceLeft") Sprite.CurrentFacing = 2;
                    if (values["PassiveBehavior"] == "FaceRight") Sprite.CurrentFacing = 3;
                    if (values["PassiveBehavior"] == "FacePlayer") FacePlayer = true;
                    if (values["PassiveBehavior"] == "ChaseIfPlayerInRange") ChasePlayer = true;
                    if (values["PassiveBehavior"] == "FollowPath") FollowPath = true;


                }
            }

            if (FollowPath)
            {
                PathFollower = new PathFollower2D(values["Path"], Pos, values["RepeatPath"] == "true");
            }

            if (values["Solid"] == "true")
                MinInteractDist = 25f;
            else
                MinInteractDist = 18f;

            ManualExists = true;
        }

        public void SetPath(string path)
        {
            TemporaryFollowPath = true;
            PathFollower = new PathFollower2D(path, Transform.Position, false);
        }

        private bool IExist()   // <- Deep questions
        {
            if (ManualExists == false)
            {
                return false;
            }

            if (GameContent.ErasedEntities.Contains(EntityId))
            {
                return false;
            }

            //if (name == "Coin" || name == "Food")
            //    return true;

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
            if (!Exists)
                return;

            Vector2 ToPlayer = player.Transform.Position - Transform.Position;

            Vector2 Steering = new Vector2(0f, 0f);
            if (ChasePlayer)
            {
                if (ToPlayer.LengthSquared() < VisionRange * VisionRange)
                    Steering = Vector2.Normalize(ToPlayer) * 26f;
            }
            else if (FollowPath || TemporaryFollowPath)
            {
                PathFollower.Update(Transform.Position, Movement.MaxSpeed, deltaTime);
                Steering = PathFollower.SteeringForce;

                if (PathFollower.Done)
                    TemporaryFollowPath = false;
            }
            
            if (Frozen)
            {
                Movement.Stop();

                if (Sprite != null)
                    Sprite.CurrentFrame = 0;
            }
            else
            {
                Movement.Update(Steering, deltaTime);
            }

            Transform.Update(Movement.Velocity, collisionBoxes, entityTransforms, values["Solid"] == "true");

            if (Sprite != null)
                Sprite.Update(Movement.Velocity, deltaTime);
            else if (AnimatedSprite != null)
                AnimatedSprite.Update(deltaTime);

            if (FacePlayer)
            {
                Sprite.CurrentFacing = Utility.GeneralDirection(ToPlayer);
            }
        }

        public void Draw( Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            if (Exists)
            {
                if (name == "Coin" || name == "Food")
                {
                    AnimatedSprite.Draw(spriteBatch, cameraPos);
                }
                else
                    Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
            }
        }

        public bool PlayerCanInteract(Vector2 playerPos, Vector2 playerHeading)
        {
            Vector2 ToMe = Transform.Position - playerPos;

            bool CloseEnough = ToMe.LengthSquared() < MinInteractDist * MinInteractDist;

            if (values["Solid"] == "false")
                return CloseEnough;

            float Dot = Vector2.Dot(ToMe, playerHeading);
            bool Facing = Dot >= 0f;

            return CloseEnough && Facing;
        }
    }
}
