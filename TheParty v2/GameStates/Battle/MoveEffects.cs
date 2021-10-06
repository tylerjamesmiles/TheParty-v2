using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    static class Effects
    {
        public static BattleStore HitStanceBy1(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            To.Stance = MathUtility.RolledIfAtLimit(To.Stance + 1, 5);
            To.KOd = (To.Stance == 0);

            return NewState;
        }
        public static BattleStore HitStanceByStance(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            To.Stance = MathUtility.RolledIfAtLimit(To.Stance + From.Stance, 5);
            //To.KOdFor = (To.Stance == 0) ? 3 : To.KOdFor;
            To.KOd = (To.Stance == 0);

            return NewState;
        }

        public static BattleStore HitHPByStance(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            To.HP -= From.Stance * 2;
            if (To.HP < 0)
                To.HP = 0;

            //if (To.KOd)
            //    To.HP -= 1;

            return NewState;
        }

        public static BattleStore Give1Stance(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            From.Stance -= 1;
            To.Stance += 1;

            return NewState;
        }

        public static BattleStore HealHPBy1(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            To.HP += 2;

            return NewState;
        }


        public static BattleStore HitHPBy1(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            To.HP -= 1;

            return NewState;
        }


        public static BattleStore Charge(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            To.Charged = true;
            NewState.AvailableCharge -= 1;

            return NewState;
        }

        public static BattleStore CasterLoseCharge(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            From.Charged = false;
            NewState.AvailableCharge += 1;

            return NewState;
        }

        public static BattleStore KO(BattleStore state, Targeting targeting)
        {
            BattleStore NewState = BattleStore.DeepCopyOf(state);

            ref Member To = ref Targeting.ToMemberRef(targeting, NewState);
            ref Member From = ref Targeting.FromMemberRef(targeting, NewState);

            //To.KOdFor = 2;
            To.KOd = true;

            return NewState;
        }
    }
}
