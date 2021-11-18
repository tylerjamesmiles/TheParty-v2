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
        public List<Action<Member, Member>> Effects;
        public List<Func<Battle, Targeting, bool>> MoveConditions;

        public static string InvalidMoveName = "Invalid";


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // N O R M A L   M O V E S
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static Move Hit =>
            new Move()
            {
                Name = "Hit",
                Description = 
                    "Hit % by your stance.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>> 
                { 
                    Battle.HitHPByStance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive,
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
                Effects = new List<Action<Member, Member>>
                {
                    Battle.HitStanceBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetIsSelf
                }
            };

        public static Move Give =>
            new Move()
            {
                Name = "Give",
                Description = 
                    "Give one pt. of Stance.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.Give1Stance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive
                }
            };

        public static Move Take =>
            new Move()
            {
                Name = "Take",
                Description =
                    "Take one pt. of Stance.\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.Take1Stance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive
                }
            };

        public static Move Calm =>
            new Move()
            {
                Name = "Calm",
                Description =
                    "Lower stance to 1\n" +
                    "(Must be #Charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.Take1Stance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetIsInSameParty,
                    Battle.TargetStanceGreaterThan1,
                    Battle.TargetAlertAndAlive
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
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.SuckHP
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Heal =>
            new Move()
            {
                Name = "Heal",
                Description = "Heal 2%",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.HealHPBy2
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
                Description = "Give enough % to heal",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.GiveEnoughHP
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.CasterCharged,
                    Battle.TargetAlive,
                }
            };

        public static Move Fireball =>
            new Move()
            {
                Name = "Fireball",
                Description = "Hit 1%",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.HitHPBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
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
                Effects = new List<Action<Member, Member>> 
                { 
                    Battle.HitStanceBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
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
                Effects = new List<Action<Member, Member>>
                {
                    Battle.TradeStance
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive
                }
            };

        public static Move TradeHP =>
            new Move()
            {
                Name = "Trade%",
                Description = "Trade %s with target.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.TradeHearts
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive
                }
            };

        public static Move Haunt =>
            new Move()
            {
                Name = "Haunt",
                Description = "Hit 2 %s,\n" +
                    "(Must be Dead.)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.HitHPBy1,
                    Battle.HitHPBy1
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlive,
                    Battle.CasterDead,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Darkness =>
            new Move()
            {
                Name = "Darkness",
                Description = "1/5 chance to kill",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.Kill20
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Kamikaze =>
            new Move()
            {
                Name = "Kamikaze",
                Description = "Kill target.\n" +
                    "Bring you to 1%.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.Kamikaze
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
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
                Effects = new List<Action<Member, Member>>
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
                Description = "Bring back to life,\n" +
                    "at the cost of yours.",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.GiveLife
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetDead
                }
            };

        public static Move Stun =>
        new Move()
        {
            Name = "Stun",
            Description = "KO an enemy\n",
            AnimationSheet = "HitAnimations",
            AnimationName = "Hit",
            PositiveEffect = false,
            Effects = new List<Action<Member, Member>>
            {
                Battle.KOTarget
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
                Effects = new List<Action<Member, Member>>
                {
                    Battle.AddStatus("Poisoned")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlive,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Restore =>
            new Move()
            {
                Name = "Restore",
                Description = "Heal 1/2 % each turn.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Charge",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>> 
                {
                    Battle.AddStatus("Restore")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlive,
                    Battle.TargetIsInSameParty
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
                Effects = new List<Action<Member, Member>>
                {
                    Battle.RemoveStatus("Poisoned")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlive,
                    Battle.HasPoison
                }
            };

        public static Move Strengthen =>
            new Move()
            {
                Name = "Strengthen",
                Description = "Increase attack.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.AddStatus("AttackUp")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive,
                    Battle.DoesntHaveAttackUp
                }
            };

        public static Move Weaken =>
            new Move()
            {
                Name = "Weaken",
                Description = "Weaken target's attack.\n",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.AddStatus("AttackDown")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlertAndAlive,
                    Battle.DoesntHaveAttackDown,
                    Battle.TargetIsInDifferentParty
                }
            };

        public static Move Bolster =>
            new Move()
            {
                Name = "Bolster",
                Description = "Increase defense.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.AddStatus("DefenseUp")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetAlive,
                    Battle.DoesntHaveDefenseUp
                }
            };

        public static Move Lower =>
            new Move()
            {
                Name = "Lower",
                Description = "Weaken defense.\n" +
                    "(Must be #charged#)",
                AnimationSheet = "HitAnimations",
                AnimationName = "Hit",
                PositiveEffect = false,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.AddStatus("DefenseDown")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
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
                AnimationName = "Charge",
                PositiveEffect = true,
                Effects = new List<Action<Member, Member>>
                {
                    Battle.AddStatus("Evade")
                },
                MoveConditions = new List<Func<Battle, Targeting, bool>>
                {
                    Battle.CasterAlertAndAlive,
                    Battle.TargetIsSelf,
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
