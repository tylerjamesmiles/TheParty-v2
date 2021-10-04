using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandAddMember : Command<TheParty>
    {
        string MemberName;

        public CommandAddMember(string memberName)
        {
            MemberName = memberName;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            client.Player.AllMembers.Add(GameContent.Members[MemberName]);
        }
    }
}
