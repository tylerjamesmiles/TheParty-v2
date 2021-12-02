using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandMoveEntity : Command<TheParty>
    {
        OgmoEntity Entity;
        string Path;

        public CommandMoveEntity(OgmoEntity entity, string path)
        {
            Entity = entity;
            Path = path;
        }

        public override void Enter(TheParty client)
        {
            Entity.SetPath(Path);
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if (Entity.PathFollower.Done)
                Done = true;
        }
    }

    class CommandMovePlayer : Command<TheParty>
    {
        string Path;

        public CommandMovePlayer(string path)
        {
            Path = path;
        }

        public override void Enter(TheParty client)
        {
            Player Player = client.Player;
            Player.PathFollower = new PathFollower2D(Path, Player.Transform.Position, false);
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if (client.Player.PathFollower.Done)
                Done = true;
        }
    }
}
