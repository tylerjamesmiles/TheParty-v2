using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    static class MoveCondition
    {
        public static bool ChargeAvailable(BattleStore state, Targeting targeting)
            => state.AvailableCharge > 0;

        public static bool CasterAlive(BattleStore state, Targeting targeting)
            => Targeting.FromMemberRef(targeting, state).HP > 0;
        public static bool CasterDead(BattleStore state, Targeting targeting)
            => Targeting.FromMemberRef(targeting, state).HP == 0;
        public static bool CasterAlert(BattleStore state, Targeting targeting)
            => Targeting.FromMemberRef(targeting, state).KOd == false;
        public static bool CasterKOd(BattleStore state, Targeting targeting)
            => Targeting.FromMemberRef(targeting, state).KOd == true;
        public static bool CasterNotCharged(BattleStore state, Targeting targeting)
            => Targeting.FromMemberRef(targeting, state).Charged == false;
        public static bool CasterCharged(BattleStore state, Targeting targeting)
            => Targeting.FromMemberRef(targeting, state).Charged == true;

        public static bool TargetAlive(BattleStore state, Targeting targeting)
            => Targeting.ToMemberRef(targeting, state).HP > 0;
        public static bool TargetDead(BattleStore state, Targeting targeting)
            => Targeting.ToMemberRef(targeting, state).HP == 0;
        public static bool TargetAlert(BattleStore state, Targeting targeting)
            => Targeting.ToMemberRef(targeting, state).KOd == false;
        public static bool TargetKOd(BattleStore state, Targeting targeting)
            => Targeting.ToMemberRef(targeting, state).KOd == true;
        public static bool TargetNotCharged(BattleStore state, Targeting targeting)
            => Targeting.ToMemberRef(targeting, state).Charged == false;
        public static bool TargetCharged(BattleStore state, Targeting targeting)
            => Targeting.ToMemberRef(targeting, state).Charged == true;

        public static bool TargetIsSelf(BattleStore state, Targeting targeting)
            => TargetIsInSameParty(state, targeting) && targeting.FromMemberIdx == targeting.ToMemberIdx;
        public static bool TargetIsOTher(BattleStore state, Targeting targeting)
            => TargetIsInDifferentParty(state, targeting) || targeting.FromMemberIdx != targeting.ToMemberIdx;
        public static bool TargetIsInSameParty(BattleStore state, Targeting targeting)
            => targeting.FromPartyIdx == targeting.ToPartyIdx;
        public static bool TargetIsInDifferentParty(BattleStore state, Targeting targeting)
            => targeting.FromPartyIdx != targeting.ToPartyIdx;

    }
}
