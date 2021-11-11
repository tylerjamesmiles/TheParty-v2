using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class ControllerSteering2D
    {
        public Vector2 SteeringForce { get; private set; }
        public float MaxForce { get; private set; }

        public ControllerSteering2D(float maxForce)
        {
            MaxForce = maxForce;
        }

        public void Update()
        {
            SteeringForce = InputManager.WASDDirectionPressed() * MaxForce;
        }
    }
}
