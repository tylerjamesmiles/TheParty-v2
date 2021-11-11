using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandFollowPath : Command<TheParty>
    {
        OgmoEntity Entity;
        string Path;

        public CommandFollowPath(OgmoEntity entity, string path)
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
}
