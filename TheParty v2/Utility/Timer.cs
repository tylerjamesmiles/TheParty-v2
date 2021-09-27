using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Timer
    {
        float TimeElapsed;
        float TicRate;
        public bool TicThisFrame { get; private set; }
        public int TicsSoFar { get; private set; }

        public Timer(float ticRate)
        {
            TimeElapsed = 0f;
            TicRate = ticRate;
            TicThisFrame = false;
            TicsSoFar = 0;
        }

        public void Update(float deltaTime)
        {
            TimeElapsed += deltaTime;
            TicThisFrame = false;

            if (TimeElapsed > TicRate)
            {
                TimeElapsed -= TicRate;
                TicsSoFar += 1;
                TicThisFrame = true;
            }
        }
    }
}
