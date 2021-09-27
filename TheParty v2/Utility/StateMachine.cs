using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TheParty_v2
{
    abstract class State<T>
    {
        public abstract void Enter(T client);
        public abstract void Update(T client, float deltaTime);
        public abstract void Draw(T client, SpriteBatch spriteBatch);
        public abstract void Exit(T client);
    }

    class StateMachine<T>
    {
        State<T> CurrentState;
        State<T> PreviousState;
        State<T> GlobalState;

        public StateMachine()
        {
        }

        public void SetNewCurrentState(T client, State<T> newState)
        {
            if (CurrentState != null)
            {
                PreviousState = CurrentState;
                CurrentState.Exit(client);
            }

            CurrentState = newState;
            CurrentState.Enter(client);
        }

        public void SetNewGlobalState(T client, State<T> newGlobalState)
        {
            GlobalState?.Exit(client);
            GlobalState = newGlobalState;
            GlobalState.Enter(client);
        }

        public void Update(T client, float deltaTime)
        {
            CurrentState?.Update(client, deltaTime);
            GlobalState?.Update(client, deltaTime);
        }

        public void Draw(T client, SpriteBatch spriteBatch)
        {
            CurrentState?.Draw(client, spriteBatch);
            GlobalState?.Draw(client, spriteBatch);
        }
    }
}
