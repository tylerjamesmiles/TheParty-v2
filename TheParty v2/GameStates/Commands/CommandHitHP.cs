using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandHitPartyHP : Command<TheParty>
    {
        int Amt;
        public CommandHitPartyHP(int amt)
        {
            Amt = amt;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            foreach (Member member in client.Player.ActiveParty.Members)
                member.HitHP(Amt);
            Done = true;
        }
    }
}
