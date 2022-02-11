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

        public Timer PosRecorderTimer;
        public List<Vector2> RecordedPositions;
        public List<Vector2> MemberPositions;
        public List<Movement2D> MemberMovements;
        public List<FourDirSprite2D> MemberSprites;
        bool FieldMembersInitialized;
        

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

            FieldMembersInitialized = false;
        }

        public Player(Vector2 position)
        {
            Steering = new ControllerSteering2D(30f);
            Movement = new Movement2D(1f, 5f);
            Transform = new Transform2D(position, 8f);
            Sprite = new FourDirSprite2D("CharacterBase", new Point(-16, -24));
            Frozen = false;
            ActiveParty = GameContent.Parties["PlayerParty"];
            Inventory = new List<string>();
            CampMembers = GameContent.Parties["BackupParty"].Members;
            PathFollower = new PathFollower2D(new Queue<Vector2>(), false);

            FieldMembersInitialized = false;
        }

        public void InitializeFieldMembers()
        {
            PosRecorderTimer = new Timer(0.3f);
            RecordedPositions = new List<Vector2>();
            MemberSprites = new List<FourDirSprite2D>();
            MemberPositions = new List<Vector2>();
            MemberMovements = new List<Movement2D>();

            for (int i = 1; i < ActiveParty.Members.Count; i++)
            {
                MemberSprites.Add(new FourDirSprite2D(ActiveParty.Members[i].FieldSpriteName, new Point(-16, -24)));
                MemberPositions.Add(Transform.Position);
                MemberMovements.Add(new Movement2D(1f, 27f));
            }

            FieldMembersInitialized = true;
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

            if (!FieldMembersInitialized)
                InitializeFieldMembers();
            
            // If player is moving, record their position every half second or so
            if (Movement.Velocity.LengthSquared() > 1f)
            {
                PosRecorderTimer.Update(deltaTime);
            
                if (PosRecorderTimer.TicThisFrame)
                {
                    RecordedPositions.Add(Transform.Position);

                    if (RecordedPositions.Count > 5)
                        RecordedPositions.RemoveAt(0);
                }
            }

            // Move members to appropriate recorded position
            for (int i = 0; i < MemberSprites.Count; i++)
            {
                if (RecordedPositions.Count > i)
                {
                    Vector2 Pos = MemberPositions[i];
                    int Idx = RecordedPositions.Count - i;
                    Vector2 Target = RecordedPositions[RecordedPositions.Count - 1 - i];
                    Vector2 ToTarget = Target - Pos;
                    if (ToTarget.LengthSquared() > 3f)
                    {
                        Vector2 Steering = Vector2.Normalize(ToTarget - MemberMovements[i].Velocity) * 30f;
                        MemberMovements[i].Update(Steering, deltaTime);
                        MemberPositions[i] += MemberMovements[i].Velocity;
                        MemberSprites[i].Update(MemberMovements[i].Velocity, deltaTime);
                    }
                }

            }
        }

        public void Draw(Vector2 cameraPos, SpriteBatch spriteBatch)
        {
            //Sprite.Draw(Transform.Position, cameraPos, spriteBatch);
            
            if (FieldMembersInitialized)
            {
                // Y-sort sprites
                List<FourDirSprite2D> PartySprites = new List<FourDirSprite2D>(MemberSprites);
                PartySprites.Add(Sprite);

                List<Vector2> PartyPositions = new List<Vector2>(MemberPositions);
                PartyPositions.Add(Transform.Position);

                List<FourDirSprite2D> SpritesToDraw = new List<FourDirSprite2D>(PartySprites);
                SpritesToDraw.Add(Sprite);

                while (SpritesToDraw.Count > 0)
                {
                    Vector2 HighestPos = new Vector2(0, float.MaxValue);
                    FourDirSprite2D HighestSprite = null;

                    for (int i = 0; i < PartySprites.Count; i++)
                    {
                        FourDirSprite2D Sprite = PartySprites[i];
                        Vector2 Pos = PartyPositions[i];

                        if (SpritesToDraw.Contains(Sprite) && Pos.Y < HighestPos.Y)
                        {
                            HighestPos = Pos;
                            HighestSprite = Sprite;
                        }
                    }

                    HighestSprite.Draw(HighestPos, cameraPos, spriteBatch);
                    SpritesToDraw.Remove(HighestSprite);
                }

            }

        }
    }
}
