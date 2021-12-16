using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandErase : Command<TheParty>
    {
        int EventId;

        public CommandErase(int id)
        {
            EventId = id;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            OgmoEntity Entity = client.CurrentMap.Entities.Find(e => e.EntityId == EventId);

            GameContent.ErasedEntities.Add(Entity.EntityId);

            Done = true;
        }
    }
}
