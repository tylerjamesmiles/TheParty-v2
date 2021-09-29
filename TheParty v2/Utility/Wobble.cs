using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Wobble
    {
        float TimeElapsed;
        float Speed;
        float Range;
        public float CurrentPosition { get; private set; }

        public Wobble(float speed, float range)
        {
            TimeElapsed = (float)new Random().NextDouble() * speed;
            Speed = speed;
            Range = range;
            CurrentPosition = 0f;
        }

        public void Update(float deltaTime)
        {
            TimeElapsed += deltaTime;
            CurrentPosition = MathF.Sin(TimeElapsed * Speed) * Range;
        }
    }
}
