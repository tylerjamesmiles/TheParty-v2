using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandFacePlayer : Command<TheParty>
    {
        OgmoEntity Entity;

        public CommandFacePlayer(OgmoEntity entity)
        {
            Entity = entity;
        }

        public override void Enter(TheParty client)
        {
            Entity.FacePlayer = true;
            Entered = true;
            Done = true;
        }
    }

    class CommandStopFacingPlayer : Command<TheParty>
    {
        OgmoEntity Entity;

        public CommandStopFacingPlayer(OgmoEntity entity)
        {
            Entity = entity;
        }

        public override void Enter(TheParty client)
        {
            Entity.FacePlayer = false;
            Entered = true;
            Done = true;
        }
    }
}
