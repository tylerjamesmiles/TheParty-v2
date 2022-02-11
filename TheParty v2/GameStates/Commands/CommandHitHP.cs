using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandHitCommitment : Command<TheParty>
    {
        string MemberName;
        int Amt;

        public CommandHitCommitment(string member, int amt)
        {
            MemberName = member;
            Amt = amt;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Member M = client.Player.ActiveParty.Members.Find(m => m.Name == MemberName);
            M.HitStance(Amt);
            Done = true;
        }
    }

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

    class CommandInn : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            foreach (Member member in client.Player.ActiveParty.Members)
            {
                if (member.HP > 0)
                {
                    member.HP = member.MaxHP;
                    member.Hunger = member.MaxHunger;
                }
            }
            Done = true;
        }
    }

    class CommandHealDead : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            foreach (Member member in client.Player.ActiveParty.Members)
                if (member.HP == 0)
                    member.HP = 1;
            Done = true;
        }
    }
}
