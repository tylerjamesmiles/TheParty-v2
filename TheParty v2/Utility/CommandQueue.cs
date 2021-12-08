using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Command<T>
    {
        public bool Entered;
        public bool Done;
        public virtual void Enter(T client) { Entered = true; }
        public virtual void Update(T client, float deltaTime) { Done = true; }
        public virtual void Draw(T client, SpriteBatch spriteBatch) { }
        public virtual void Exit(T client) { }
    }

    class CommandQueue<T>
    {
        LinkedList<Command<T>> Commands;

        public CommandQueue()
        {
            Commands = new LinkedList<Command<T>>();
        }

        public void PushCommand(Command<T> command) => Commands.AddFirst(command);
        public void PushCommands(List<Command<T>> commands)
        {
            for (int i = commands.Count - 1; i >= 0; i--)
                Commands.AddFirst(commands[i]);
        }
        public void EnqueueCommand(Command<T> command)
        {
            Commands.AddLast(command);
        }
        public void EnqueueCommands(List<Command<T>> commands)
        {
            commands.ForEach(c => Commands.AddLast(c));
        }

        public bool Empty => Commands.Count == 0;
        private Command<T> Top => Commands.First.Value;

        public void ClearCommands() => Commands.Clear();

        public void Update(T client, float deltaTime)
        {
            if (!Empty)
            {
                if (!Top.Entered)
                    Top.Enter(client);

                Top.Update(client, deltaTime);

                // Are any commands 'Done'?
                bool AtLeastOneIsDone = false;
                foreach (var item in Commands)
                    if (item.Done)
                        AtLeastOneIsDone = true;

                // If so, remove all of them (manually...)
                if (AtLeastOneIsDone)
                {
                    LinkedList<Command<T>> Copy = new LinkedList<Command<T>>(Commands);
                    Commands.Clear();

                    foreach (var item in Copy)
                    {
                        if (item.Done)
                            item.Exit(client);  // Call its exit command first
                        else
                            Commands.AddLast(item);
                    }
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
