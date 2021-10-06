using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    struct Targeting
    {
        public int FromPartyIdx;
        public int FromMemberIdx;
        public int ToPartyIdx;
        public int ToMemberIdx;

        public static ref Party FromPartyRef(Targeting targeting, BattleStore state) =>
            ref state.Parties[targeting.FromPartyIdx];
        public static ref Party ToPartyRef(Targeting targeting, BattleStore state) =>
            ref state.Parties[targeting.ToPartyIdx];
        public static ref Member FromMemberRef(Targeting targeting, BattleStore state) =>
            ref state.Parties[targeting.FromPartyIdx].Members[targeting.FromMemberIdx];
        public static ref Member ToMemberRef(Targeting targeting, BattleStore state) =>
            ref state.Parties[targeting.ToPartyIdx].Members[targeting.ToMemberIdx];
    }
}
