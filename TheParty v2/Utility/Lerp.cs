using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class LerpF
    {
        float Start;
        float End;
        float TimeElapsed;
        float TravelTime;
        public float CurrentPosition { get; private set; }
        public bool Reached => TimeElapsed >= TravelTime;

        public LerpF(float start, float end, float travelTime)
        {
            Start = start;
            End = end;
            TravelTime = travelTime;
            TimeElapsed = 0f;
            CurrentPosition = start;
        }

        public void Update(float deltaTime)
        {
            TimeElapsed += deltaTime;
            float T = TimeElapsed / TravelTime;

            if (T < 1)
                CurrentPosition = Start + (End - Start) * T;
            else
                CurrentPosition = End;
        }
    }

    class LerpV
    {
        Vector2 Start;
        Vector2 End;
        float TimeElapsed;
        float TravelTime;
        public Vector2 CurrentPosition { get; private set; }
        public bool Reached => TimeElapsed >= TravelTime;

        public LerpV(Vector2 start, Vector2 end, float travelTime)
        {
            Start = start;
            End = end;
            TravelTime = travelTime;
            TimeElapsed = 0f;
            CurrentPosition = start;
        }

        public void Update(float deltaTime)
        {
            TimeElapsed += deltaTime;
            float T = TimeElapsed / TravelTime;

            if (T < 1)
                CurrentPosition = Start + (End - Start) * T;
            else
                CurrentPosition = End;
        }
    }
}
