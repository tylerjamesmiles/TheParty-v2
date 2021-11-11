using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandFace : Command<TheParty>
    {
        OgmoEntity Entity;
        string Direction;

        public CommandFace(OgmoEntity entity, string dir)
        {
            Entity = entity;
            Direction = dir;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Entity.Sprite.CurrentFacing =
                Direction.ToLower() == "up" ? 0 :
                Direction.ToLower() == "down" ? 1 :
                Direction.ToLower() == "left" ? 2 :
                Direction.ToLower() == "right" ? 3 : 0;
            Done = true;
        }
    }
}
