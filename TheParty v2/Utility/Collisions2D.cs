using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    struct CollisionInfo
    {
        public bool Exists;
        public Vector2 IntersectionPoint;
        public Vector2 Normal;
        public Vector2 Resolution;
        public float T;
    }

    static class Collisions2D
    {
        public static CollisionInfo CircleCircleInterection(
            Vector2 circle1Pos, float circle1Radius,
            Vector2 circle2Pos, float circle2Radius)
        {
            Vector2 ToCircle2 = circle2Pos - circle1Pos;
            float Distance = ToCircle2.Length();
            float SumOfRadi = circle1Radius + circle2Radius;
            if (Distance < SumOfRadi)
            {
                Vector2 ToCir2Dir = Vector2.Normalize(ToCircle2);
                float HalfDistance = Distance / 2f;
                Vector2 IP = circle1Pos + ToCir2Dir * HalfDistance;
                Vector2 Normal = -ToCir2Dir;
                float Overlap = SumOfRadi - Distance;
                Vector2 Resolution = Normal * Overlap;

                return new CollisionInfo()
                {
                    Exists = true,
                    IntersectionPoint = IP,
                    Normal = Normal,
                    Resolution = Resolution,
                    T = 0.0f
                };
            }
            else
                return new CollisionInfo()
                {
                    Exists = false
                };

        }

        public static CollisionInfo CircleRectangleIntersection(
            Vector2 circlePos, float circleRadius, Rectangle rect)
        {
            Vector2 rectBR = (rect.Location + rect.Size).ToVector2();
            float NearestX = MathUtility.Clamped(circlePos.X, rect.Location.X, rectBR.X);
            float NearestY = MathUtility.Clamped(circlePos.Y, rect.Location.Y, rectBR.Y);
            Vector2 NearestPointToCircle = new Vector2(NearestX, NearestY);
            Vector2 ToCircle = circlePos - NearestPointToCircle;
            float DistToNearestPoint = ToCircle.Length();

            if (DistToNearestPoint < circleRadius)
            {
                Vector2 Normal = Vector2.Normalize(ToCircle);
                float T = 0.0f;
                float Overlap = circleRadius - DistToNearestPoint;
                Vector2 Resolution = Normal * Overlap;

                return new CollisionInfo()
                {
                    Exists = true,
                    IntersectionPoint = NearestPointToCircle,
                    Normal = Normal,
                    Resolution = Resolution,
                    T = T
                };
            }
            else
                return new CollisionInfo()
                {
                    Exists = false
                };

        }
    }
}
