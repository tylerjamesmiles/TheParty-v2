using System;
using System.Collections.Generic;

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

    struct Move
    {
        public string Name;
        public string AnimationSheet;
        public string AnimationName;
        public bool PositiveEffect;
        public Func<BattleStore, Targeting, BattleStore>[] UnChargedEffects;
        public Func<BattleStore, Targeting, BattleStore>[] ChargedEffects;
        public Func<BattleStore, Targeting, bool>[] MoveConditions;

        public static string InvalidMoveName = "Invalid";

        public static Move Hit =>
            new Move()
            {
                Name = "Hit",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] 
                { 
                    Effects.HitStanceByStance,
                    Effects.HitHPBy1
                },
                ChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] 
                { 
                    Effects.HitStanceByStance,
                    Effects.HitHPBy1,
                    Effects.CasterLoseCharge
                },
                MoveConditions = new Func<BattleStore, Targeting, bool>[]
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                    MoveCondition.TargetAlive,
                    MoveCondition.TargetAlert,
                    MoveCondition.TargetIsInDifferentParty
                }
            };

        public static Move Hurt =>
            new Move()
            {
                Name = "Hurt",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,

                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] 
                { 
                    Effects.HitHPByStance 
                },
                ChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] 
                { 
                    Effects.HitHPByStance, 
                    Effects.HitHPBy1,
                    Effects.CasterLoseCharge
                },
                MoveConditions = new Func<BattleStore, Targeting, bool>[]
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                    MoveCondition.TargetAlive,
                    MoveCondition.TargetKOd,
                    MoveCondition.TargetIsInDifferentParty
                }
            };

        public static Move Give =>
            new Move()
            {
                Name = "Give",
                PositiveEffect = true,

                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[]
                {
                    Effects.Give1Stance
                },
                ChargedEffects = new Func<BattleStore, Targeting, BattleStore>[]
                {
                    Effects.Give1Stance,
                    Effects.HealHPBy1,
                    Effects.CasterLoseCharge

                },
                MoveConditions = new Func<BattleStore, Targeting, bool>[]
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                    MoveCondition.TargetAlive,
                    MoveCondition.TargetAlert,
                    MoveCondition.TargetIsInSameParty,
                    MoveCondition.TargetIsOTher
                }
            };

        public static Move Help =>
            new Move()
            {
                Name = "Help",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,

                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[]
                {
                    Effects.HitStanceByStance
                },
                ChargedEffects = new Func<BattleStore, Targeting, BattleStore>[]
                {
                    Effects.HitStanceByStance,
                    Effects.CasterLoseCharge
                },
                MoveConditions = new Func<BattleStore, Targeting, bool>[]
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                    MoveCondition.CasterCharged,
                    MoveCondition.TargetAlive,
                    MoveCondition.TargetKOd
                }
            };

        public static Move Charge =>
            new Move()
            {
                Name = "Charge",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,

                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] 
                { 
                    Effects.Charge 
                },
                ChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] { },
                MoveConditions = new Func<BattleStore, Targeting, bool>[]
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                    MoveCondition.ChargeAvailable,
                    MoveCondition.TargetIsSelf,
                    MoveCondition.CasterNotCharged
                }
            };

        public static Move Item =>
            new Move()
            {
                Name = "Item",
                PositiveEffect = true,

                MoveConditions = new Func<BattleStore, Targeting, bool>[] 
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                }
            };

        public static Move Talk =>
            new Move()
            {
                Name = "Talk",
                PositiveEffect = true,

                MoveConditions = new Func<BattleStore, Targeting, bool>[] 
                {
                    MoveCondition.CasterAlive,
                    MoveCondition.CasterAlert,
                }
            };

        public static BattleStore WithEffectDone(BattleStore state, Move move, Targeting targeting)
        {
            BattleStore Result = BattleStore.DeepCopyOf(state);

            if (Targeting.FromMemberRef(targeting, state).Charged)
            {
                foreach (var effect in move.ChargedEffects)
                    Result = effect(Result, targeting);
            }
            else
            {
                foreach (var effect in move.UnChargedEffects)
                    Result = effect(Result, targeting);
                
            }

            //BattleStore.Member(Result, targeting.FromPartyIdx, targeting.FromMemberIdx).HasGoneThisTurn = true;

            return Result;
        }

        public static bool ValidOnMember(Move move, BattleStore state, Targeting targeting)
        {
            bool SatisfiesAllConditions = true;
            foreach (var condition in move.MoveConditions)
            {
                if (condition(state, targeting) == false)
                    SatisfiesAllConditions = false;
            }
            return SatisfiesAllConditions;
        }
        public static bool ValidOnAnyone(Move move, BattleStore state, int fromPartyIdx, int fromMemberIdx)
        {
            bool ValidOnAnyone = false;

            for (int party = 0; party < state.Parties.Length; party++)
                for (int member = 0; member < state.Parties[party].Members.Length; member++)
                {
                    Targeting Targeting = new Targeting()
                    {
                        FromPartyIdx = fromPartyIdx,
                        FromMemberIdx = fromMemberIdx,
                        ToPartyIdx = party,
                        ToMemberIdx = member
                    };

                    if (ValidOnMember(move, state, Targeting))
                        ValidOnAnyone = true;
                }

            return ValidOnAnyone;
        }
        public static Move[] AllValidMovesFor(int fromPartyIdx, int fromMemberIdx, BattleStore state)
        {
            List<Move> Result = new List<Move>();
            Member From = BattleStore.Member(state, fromPartyIdx, fromMemberIdx);
            foreach (Move move in From.Moves)
                if (ValidOnAnyone(move, state, fromPartyIdx, fromMemberIdx))
                    Result.Add(move);
            return Result.ToArray();
        }
        public static Move MoveWithName(string name, Move[] moves)
        {
            foreach (Move move in moves)
                if (move.Name == name)
                    return move;
            return new Move() { Name = InvalidMoveName };
        }
    }

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

    static class Effects
    {
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

            To.HP -= From.Stance;
            if (To.HP < 0)
                To.HP = 0;

            //if (To.KOdFor > 0)
            if (To.KOd)
                To.HP -= 1;

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

            To.HP += 1;

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
