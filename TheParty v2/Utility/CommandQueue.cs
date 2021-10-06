using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Command<T>
    {
        public bool Entered { get; protected set; }
        public bool Done;
        public virtual void Enter(T client) { Entered = true; }
        public virtual void Update(T client, float deltaTime) { Done = true; }
        public virtual void Draw(T client, SpriteBatch spriteBatch) { }
        public virtual void Exit(T client) { }
    }

    class CommandQueue<T>
    {
        Queue<Command<T>> Commands;

        public CommandQueue()
        {
            Commands = new Queue<Command<T>>();
        }

        public void AddCommand(Command<T> command) => Commands.Enqueue(command);
        public void AddCommands(List<Command<T>> commands) => commands.ForEach(c => Commands.Enqueue(c));
        public bool Empty => Commands.Count == 0;
        private Command<T> Top => Commands.Peek();

        public void Update(T client, float deltaTime)
        {
            if (!Empty)
            {
                if (!Top.Entered)
                    Top.Enter(client);

                Top.Update(client, deltaTime);

                if (Top.Done)
                {
                    Commands.Dequeue().Exit(client);

                    if (!Empty)
                        Top.Enter(client);
                }
            }
        }

        public void Draw(T client, SpriteBatch spriteBatch)
        {
            if (!Empty)
                Top.Draw(client, spriteBatch);
        }
    }
}
