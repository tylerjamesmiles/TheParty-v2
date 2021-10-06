using System;
using System.Collections.Generic;

namespace TheParty_v2
{
    struct Move
    {
        public string Name;
        public string Description;
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
                Description = 
                    "Hit stance by yours.",
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
                    Effects.HitStanceByStance
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
                Description = 
                    "Hit HP by your stance. \n" +
                    "(Target must be KOd)",
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
                Description = 
                    "Target St. +1. \n" +
                    "#: Also heal their HP by 1.",

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
                Description = 
                    "Revive a KOd ally.\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,

                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[]
                {
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
                Name = "#Charge",
                Description = "#Charge up energy!#",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,

                UnChargedEffects = new Func<BattleStore, Targeting, BattleStore>[] 
                { 
                    Effects.Charge,
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
                Description = "Use an item.",
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
                Description = "Sway an enemy's feelings about you.",
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

}
