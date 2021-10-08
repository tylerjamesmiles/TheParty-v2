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

        public Targeting(int fpi, int fmi, int tpi, int tmi)
        {
            FromPartyIdx = fpi;
            FromMemberIdx = fmi;
            ToPartyIdx = tpi;
            ToMemberIdx = tmi;
        }
    }
}
