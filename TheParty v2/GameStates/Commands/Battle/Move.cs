using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

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


    //  C H A R A C T E R S
    //  Hero                    SCAN        TALK        SLANDER
    //  MLM Businessman         GIVE        TAKE        TRADE 
    //  Zombie                  SUCK        ATTACKDOWN  HAUNT
    //  Priest                  ATTACKUP    DEMORALIZE  ENCOURAGE
    //  Humanitarian            HEAL        SACRIFICE   LIFE
    //  Martial Artist          EVADE       STUN        PUNCH
    //  Apothecary              ANTIDOTE    POISON      ARSENIC
    //  Wizard                  TELEPORT    DEFENSEUP   DEFENSEDOWN
    //  Soldier                 SHOOT       DIVE        KAMIKAZE
                                
    //[ ]   TALK - Try to sway the enemy your way                                               
    //[ ]   TELEPORT - Brings party back to the world map                   
    //[ ]   SLANDER - Encourage one party to dislike the other              
    //[ ]   SCAN - Tells you info about the enemy                           


    class Move
    {
        public string Name;
        public string Description;
        public string AnimationSheet;
        public string AnimationName;
        public bool PositiveEffect;
        public List<Action<Member, Member>> UnChargedEffects;
        public List<Action<Member, Member>> ChargedEffects;
        public List<Func<Battle, Targeting, bool>> MoveConditions;

        public static string InvalidMoveName = "Invalid";

        public List<Action<Member, Member>> Effects(Member m) =>
            m.Charged ? ChargedEffects : UnChargedEffects;





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
                UnChargedEffects = new List<Action<Member, Member>> 
                { 
                    Battle.HitStanceByStance,
                    Battle.HitHPBy1
                },
                ChargedEffects = new List<Action<Member, Member>> 
                { 
                    Battle.HitStanceByStance,
                    Battle.HitHPBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive,
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
                UnChargedEffects = new List<Action<Member, Member>> 
                { 
                    Battle.HitHPByStance 
                },
                ChargedEffects = new List<Action<Member, Member>> 
                { 
                    Battle.HitHPByStance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
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
                UnChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Charge
                },
                ChargedEffects = new List<Action<Member, Member>> { },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetIsSelf,
                    Battle.CasterNotCharged
                }
            };

        public static Move Give =>
            new Move()
            {
                Name = "Give",
                Description = 
                    "Give one pt. of Stance.\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Give1Stance,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive
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
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.HitStanceByStance,
                    Battle.HealHPBy1,
                    Battle.TargetLoseCharge,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
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
                UnChargedEffects = new List<Action<Member, Member>> { },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.SuckHP,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Heal =>
            new Move()
            {
                Name = "Heal",
                Description = "Heal 2%\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>> { },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.HealHPBy2,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive
                }
            };

        public static Move Sacrifice =>
            new Move()
            {
                Name = "Heal",
                Description = "Give enough % to heal\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>> { },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.GiveEnoughHP,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                }
            };

        public static Move Shoot =>
            new Move()
            {
                Name = "Shoot",
                Description = "Hit 1% and keep charge.\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>> { },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.HitHPBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Encourage =>
            new Move()
            {
                Name = "Encourage",
                Description = "Charge someone else up.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>> 
                { 
                    Battle.Charge,
                    Battle.HealHPBy1
                },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Charge,
                    Battle.HealHPBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlertAndAlive
                }
            };

        public static Move Trade =>
            new Move()
            {
                Name = "Trade",
                Description = "Trade stance with target.\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.TradeStance,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlertAndAlive
                }
            };

        public static Move Punch =>
            new Move()
            {
                Name = "Punch",
                Description = "Hit % by your stance.\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.HitHPByStance,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Haunt =>
            new Move()
            {
                Name = "Haunt",
                Description = "Hit 1/2 a %,\n" +
                    "(Must be KO'd.)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.HitHPBy1
                },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.HitHPBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterKOd,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Arsenic =>
            new Move()
            {
                Name = "Arsenic",
                Description = "1/5 chance to kill,\n" +
                    "(Must be #Charged#.)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Kill20,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Kamikaze =>
            new Move()
            {
                Name = "Kamikaze",
                Description = "Kill target. Bring you,\n" +
                    "to 1%. (Req. #Charge#.)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Kamikaze,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Dive =>
            new Move()
            {
                Name = "Dive",
                Description = "KO you and target.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.KOCaster,
                    Battle.KOTarget
                },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.KOCaster,
                    Battle.KOTarget
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move GiveLife =>
            new Move()
            {
                Name = "GiveLife",
                Description = "Bring target back to life,\n" +
                    "at the cost of yours.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.GiveLife
                },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.GiveLife
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetDead
                }
            };

        public static Move Demoralize =>
            new Move()
            {
                Name = "Demoralize",
                Description = "Steal charge.\n" +
                    "Target can't charge next turn.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Charge,
                    Battle.TargetLoseCharge,
                    Battle.AddCantCharge
                },
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.Charge,
                    Battle.TargetLoseCharge,
                    Battle.AddCantCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive,
                    Battle.TargetCharged,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Stun =>
        new Move()
        {
            Name = "Stun",
            Description = "Stun for 1 turn.\n" +
                "(Must be #charged#)",
            AnimationSheet = "HitAnimations",
            AnimationName = "Hit",
            PositiveEffect = false,
            UnChargedEffects = new List<Action<Member, Member>>
            {
                Battle.AddStunned
            },
            ChargedEffects = new List<Action<Member, Member>>
            {
                Battle.AddStunned
            },
            MoveConditions = new List<Func<Battle, Targeting, bool>>
            {
                Battle.CasterAlertAndAlive,
                Battle.TargetAlertAndAlive,
                Battle.TargetIsInDifferentParty
            }
        };

        public static Move Poison =>
            new Move()
            {
                Name = "Poison",
                Description = "Poison a target.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.AddPoison,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Antidote =>
            new Move()
            {
                Name = "Antidote",
                Description = "Heal poison.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.RemovePoison,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.HasPoison
                }
            };

        public static Move Strengthen =>
            new Move()
            {
                Name = "Strengthen",
                Description = "Increase target's attack.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.AddAttackUp,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlertAndAlive,
                    Battle.DoesntHaveAttackUp
                }
            };

        public static Move Weaken =>
            new Move()
            {
                Name = "Weaken",
                Description = "Weaken target's attack.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.AddAttackDown,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlertAndAlive,
                    Battle.DoesntHaveAttackDown,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Bolster =>
            new Move()
            {
                Name = "Bolster",
                Description = "Increase target's defense.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.AddDefenseUp,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.DoesntHaveDefenseUp
                }
            };

        public static Move Lower =>
            new Move()
            {
                Name = "Lower",
                Description = "Weaken target's defense.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.AddDefenseDown,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                    Battle.DoesntHaveDefenseDown,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Evade =>
            new Move()
            {
                Name = "Evade",
                Description = "Will dodge next attack.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                UnChargedEffects = new List<Action<Member, Member>>{},
                ChargedEffects = new List<Action<Member, Member>>
                {
                    Battle.AddEvade,
                    Battle.CasterLoseCharge
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlertAndAlive,
                    Battle.DoesntHaveEvade
                }
            };

        public bool ValidOnMember(Battle state, Targeting targeting)
            => MoveConditions.TrueForAll(c => c(state, targeting));

        public bool ValidOnAnyone(Battle state, int fromPartyIdx, int fromMemberIdx)
            => state.AllTargetingFor(fromPartyIdx, fromMemberIdx).Exists(t => ValidOnMember(state, t));
    }
}
