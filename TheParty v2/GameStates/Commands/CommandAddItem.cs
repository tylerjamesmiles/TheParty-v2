using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandAddItem : Command<TheParty>
    {
        string ItemName;
        public CommandAddItem(string itemName)
        {
            ItemName = itemName;
        }
        public override void Update(TheParty client, float deltaTime)
        {
            client.Player.Inventory.Add(ItemName);
            Done = true;
        }
    }

}
