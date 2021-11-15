using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class CommandAddPartyMember : Command<TheParty>
    {
        string Name;

        public CommandAddPartyMember(string name)
        {
            Name = name;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if (client.Player.ActiveParty.Members.Count < 3)
                client.Player.ActiveParty.Members.Add(GameContent.Members[Name].DeepCopy());
            else
                client.Player.CampMembers.Add(GameContent.Members[Name].DeepCopy());
            Done = true;
        }
    }

    class CommandRemovePartyMember : Command<TheParty>
    {
        string Name;
        public CommandRemovePartyMember(string name)
        {
            Name = name;
        }
        public override void Update(TheParty client, float deltaTime)
        {
            List<Member> ActiveMembers = client.Player.ActiveParty.Members;
            List<Member> BackupMembers = client.Player.CampMembers;

            Member ToRemove = ActiveMembers.Find(m => m.Name == Name);
            ActiveMembers.Remove(ToRemove);

            if (client.Player.CampMembers.Count > 0)
            {
                ActiveMembers.Add(BackupMembers[0]);
                BackupMembers.RemoveAt(0);
            }

            Done = true;
        }
    }
}
