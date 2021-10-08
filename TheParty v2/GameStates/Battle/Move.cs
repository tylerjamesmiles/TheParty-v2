using System;
using System.Collections.Generic;

namespace TheParty_v2
{
    // E N V I R O N M E N T A L   E F F E C T S
    // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ 
    // ( you can tell 1 turn ahead of time who will be affected? )
    //
    // Wind occasionally blows and modifies one stance by 1
    // Heat occasionally hurts everybody by 1/2 heart
    // Bright light occasionally stuns
    //

    // S T A T U S   E F F E C T   I D E A S
    // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
    // [ ] POISON
    // [ ] ATTACK UP
    // [ ] ATTACK DOWN
    // [ ] DEFENSE UP
    // [ ] DEFENSE DOWN
    // [ ] EVADE
    // [ ] (CAN'T CHARGE)
    // [ ] STUNNED
    // [ ] 
    // [ ] 
    // [ ]


    //  M O V E S   S O   F A R
    // ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~
    //
    //  N O R M A L   M O V E S
    //
    //[x]   HIT - Hit stance by yours
    //[x]   HURT - Hurt KOd target's HP by your stance
    //[x]   CHARGE - Charge up energy
    //[x]   ITEM - Use an item
    //
    //  S P E C I A L   M O V E S   (Need to be charged)                    C L A S S   I D E A S
    //
    //[x]   HELP - Give KOd member your stance                              Fireman
    //[x]   GIVE - Give target 1 stance point of yours                      Communist
    //[x]   TAKE - Take one stance point from target                        Communist
    //[ ]   SUCK - Steal 1 Heart from target                                Vampire
    //[ ]   DEMORALIZE - Steal charge. Target can't charge next turn.       (someone who's a jerk)
    //[ ]   HEAL - Restore 2 hearts                                         Doctor
    //[ ]   SACRIFICE - Give as many hearts as completely heals a target    Monk/Nun
    //[ ]   STUN - Stun target for 1 turn                                   Martial Artist
    //[ ]   POKE - Deal 1 heart of damage and keep charge                   Fencer
    //[ ]   ENCOURAGE - Give target charge                                  Motivational Speaker
    //[ ]   TALK - Try to sway the enemy your way                           Cult Leader
    //[ ]   TRADE - Trade stance with the target                            Merchant
    //[ ]   PUNCH - Hit enemy HP by your stance                             Martial Artist
    //[ ]   ??? - Hit enemy by 1/2 HP, even if you're KO'd                  Zombie
    //[ ]   DEATH - 1/5 chance to instantly kill
    //[ ]   DOUBLE - Inflicts Double
    //[ ]   TELEPORT - Brings party back to the world map
    //[ ]   SLANDER - Encourage one party to dislike the other
    //[ ]   KAMIKAZE - Kills target. Leaves 1/2 heart.
    //[ ]   DIVE - KOs target and self.
    //[ ]   LIFE - Brings target back to life, at the expense of your own.
    //[ ]   SCAN - Tells you info about the enemy
    //[ ]   
    //[ ]
    //[ ]
    //[ ]

    //      O N   T H E   F E N C E   A B O U T
    //[ ]   POISON - Target takes 1/2 heart of damage every turn
    //[ ]   ANTIDOTE - Heals poison
    //
    //
    //
    //
    //

    //

    class Move
    {
        public string Name;
        public string Description;
        public string AnimationSheet;
        public string AnimationName;
        public bool PositiveEffect;
        public List<Action<Battle, Targeting>> UnChargedEffects;
        public List<Action<Battle, Targeting>> ChargedEffects;
        public List<Func<Battle, Targeting, bool>> MoveConditions;

        public static string InvalidMoveName = "Invalid";

        public List<Action<Battle, Targeting>> Effects(Battle b, Targeting t) =>
            b.From(t).Charged ? ChargedEffects : UnChargedEffects;

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // N O R M A L   M O V E S
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static Move Hit =>
            new Move()
            {
                Name = "Hit",
                Description = 
                    "Hit stance by yours.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Battle, Targeting>> 
                { 
                    Battle.HitStanceByStance,
                    Battle.HitHPBy1
                },
                ChargedEffects = new List<Action<Battle, Targeting>> 
                { 
                    Battle.HitStanceByStance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                    Battle.TargetAlive,
                    Battle.TargetAlert,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Hurt =>
            new Move()
            {
                Name = "Hurt",
                Description = 
                    "Hit % by your stance. \n" +
                    "(Target must be KOd)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,

                UnChargedEffects = new List<Action<Battle, Targeting>> 
                { 
                    Battle.HitHPByStance 
                },
                ChargedEffects = new List<Action<Battle, Targeting>> 
                { 
                    Battle.HitHPByStance, 
                    Battle.HitHPBy1,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                    Battle.TargetAlive,
                    Battle.TargetKOd,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Charge =>
            new Move()
            {
                Name = "#Charge#",
                Description = "#Charge up energy!#",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,

                UnChargedEffects = new List<Action<Battle, Targeting>>
                {
                            Battle.Charge,
                },
                ChargedEffects = new List<Action<Battle, Targeting>> { },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                            Battle.CasterAlive,
                            Battle.CasterAlert,
                            Battle.TargetIsSelf,
                            Battle.CasterNotCharged
                }
            };

        public static Move Item =>
            new Move()
            {
                Name = "Item",
                Description = "Use an item.",
                PositiveEffect = true,

                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                }
            };

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // S P E C I A L  M O V E S
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static Move Give =>
            new Move()
            {
                Name = "Give",
                PositiveEffect = true,
                Description = 
                    "Give one pt. of Stance.\n" +
                    "(Must be #Charged#)",

                UnChargedEffects = new List<Action<Battle, Targeting>>
                {},
                ChargedEffects = new List<Action<Battle, Targeting>>
                {
                    Battle.Give1Stance,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                    Battle.TargetAlive,
                    Battle.TargetAlert,
                    Battle.TargetIsInSameParty,
                    Battle.TargetIsOTher
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

                UnChargedEffects = new List<Action<Battle, Targeting>>
                {
                },
                ChargedEffects = new List<Action<Battle, Targeting>>
                {
                    Battle.HitStanceByStance,
                    Battle.HealHPBy1,
                    Battle.TargetLoseCharge,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetKOd
                }
            };


        public static Move Suck =>
            new Move()
            {
                Name = "Suck",
                Description = "Steal 1%\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Battle, Targeting>> { },
                ChargedEffects = new List<Action<Battle, Targeting>>
                {

                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }

            };

        public static Move Talk =>
            new Move()
            {
                Name = "Talk",
                Description = "Sway an enemy's feelings about you.",
                PositiveEffect = true,

                MoveConditions = new List<Func<Battle, Targeting, bool>> 
                {
                    Battle.CasterAlive,
                    Battle.CasterAlert,
                }
            };

        public bool ValidOnMember(Battle state, Targeting targeting)
            => MoveConditions.TrueForAll(c => c(state, targeting));

        public bool ValidOnAnyone(Battle state, int fromPartyIdx, int fromMemberIdx)
            => state.AllTargetingFor(fromPartyIdx, fromMemberIdx).Exists(t => ValidOnMember(state, t));

        public static Move[] AllValidMovesFor(int fromPartyIdx, int fromMemberIdx, Battle state)
        {
            List<Move> Result = new List<Move>();
            Member From = state.Member(state, fromPartyIdx, fromMemberIdx);
            foreach (Move move in From.Moves)
                if (ValidOnAnyone(move, state, fromPartyIdx, fromMemberIdx))
                    Result.Add(move);
            return Result.ToArray();
        }
    }
}
