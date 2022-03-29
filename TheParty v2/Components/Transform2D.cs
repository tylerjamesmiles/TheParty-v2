using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Transform2D
    {
        public Vector2 Position { get; set; }
        public float BoundingRadius { get; set; }

        public Transform2D(Vector2 pos, float br)
        {
            Position = pos;
            BoundingRadius = br;
        }

        public void Update(Vector2 velocity, List<Rectangle> collisionRects, List<Transform2D> entityTransforms, bool Solid = true)
        {
            Vector2 PotentialPos = Position + velocity;



            if (Solid)
            {
                // only check nearby rects
                List<Rectangle> NearbyRects = new List<Rectangle>();
                foreach (Rectangle rect in collisionRects)
                {
                    Vector2 RectPos = rect.Location.ToVector2();
                    float DistSq = (Position - RectPos).LengthSquared();
                    float MaxDist = 64f;
                    if (DistSq < MaxDist * MaxDist)
                        NearbyRects.Add(rect);
                }

                foreach (Rectangle rect in NearbyRects)
                {
                    CollisionInfo Collision = Collisions2D.CircleRectangleIntersection(Position, BoundingRadius, rect);

                    if (Collision.Exists)
                        PotentialPos += Collision.Resolution;
                }

                foreach (Transform2D entityTransform in entityTransforms)
                {
                    if (entityTransform.Position == Position)
                        continue;

                    CollisionInfo Collision = Collisions2D.CircleCircleInterection(Position, BoundingRadius,
                        entityTransform.Position, entityTransform.BoundingRadius);

                    if (Collision.Exists)
                        PotentialPos += Collision.Resolution;
                }
            }


            Position = PotentialPos;
        }
    }
}
