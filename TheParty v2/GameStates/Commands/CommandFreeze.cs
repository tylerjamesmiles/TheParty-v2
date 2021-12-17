using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandFreeze: Command<TheParty>
    {
        OgmoEntity Entity;

        public CommandFreeze(OgmoEntity entity)
        {
            Entity = entity;
        }

        public override void Enter(TheParty client)
        {
            Entity.Frozen = true;
            Done = true;
        }
    }
    class CommandUnFreeze : Command<TheParty>
    {
        OgmoEntity Entity;

        public CommandUnFreeze(OgmoEntity entity)
        {
            Entity = entity;
        }

        public override void Enter(TheParty client)
        {
            Entity.Frozen = false;
            Entered = true;
            Done = true;
        }
    }

    class CommandFreezeAll : Command<TheParty>
    {
        List<OgmoEntity> Entities;
        public CommandFreezeAll(List<OgmoEntity> entities)
        {
            Entities = entities;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Entities.ForEach(e => e.Frozen = true);
            Done = true;
        }
    }

    class CommandUnFreezeAll : Command<TheParty>
    {
        List<OgmoEntity> Entities;
        public CommandUnFreezeAll(List<OgmoEntity> entities)
        {
            Entities = entities;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Entities.ForEach(e => e.Frozen = false);
            Done = true;
        }
    }

    class CommandFreezePlayer : Command<TheParty>
    {
        public override void Enter(TheParty client)
        {
            client.Player.Frozen = true;
            Entered = true;
            Done = true;
        }
    }

    class CommandUnfreezePlayer : Command<TheParty>
    {
        public override void Enter(TheParty client)
        {
            client.Player.Frozen = false;
            Entered = true;
            Done = true;
        }
    }
}
