using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Movement2D
    {
        public Vector2 Velocity;
        public Vector2 Heading;
        public float Mass;
        public float MaxSpeed;
        public const float Drag = 0.25f;

        public Movement2D(float mass, float maxSpeed)
        {
            Mass = mass;
            MaxSpeed = maxSpeed;
        }

        public void Update(Vector2 steeringForce, float deltaTime)
        {
            Vector2 Accel = steeringForce / Mass;
            Vector2 DeltaVel = Accel * deltaTime;
            Vector2 NewVel = Velocity + DeltaVel;
            Vector2 NewVelCapped = MathUtility.Capped(NewVel, MaxSpeed);
            Vector2 VelWithDrag = NewVelCapped - (NewVelCapped * Drag);

            Velocity = VelWithDrag;
            Heading = (Velocity.LengthSquared() > 0.001f) ?
                Vector2.Normalize(Velocity) : Heading;
        }
    }
}
