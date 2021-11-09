using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class StatusEffect
    {
        public string Name;
        public string SpriteAnimation;
        public string AnimationSheet;
        public string AnimationName;
        public List<Action<Member, Member>> EveryTurnEffects;
        public int NumTurnsRemaining;

        public bool Gone => NumTurnsRemaining == 0;
        public bool HasEveryTurnEffect => EveryTurnEffects.Count > 0;
        public void DecrementTurnsRemaining() => NumTurnsRemaining -= 1;
        public void Do(Battle state, Member member)
            => EveryTurnEffects.ForEach(e => e(member, member));

        public static StatusEffect Poison =>
            new StatusEffect()
            {
                Name = "Poison",
                SpriteAnimation = "NegativeHit",
                AnimationSheet = "StatusAnimations",
                AnimationName = "Poison",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                    Battle.HitHPBy1
                },
                NumTurnsRemaining = 3
            };

        public static StatusEffect CantCharge =>
            new StatusEffect()
            {
                Name = "CantCharge",
                SpriteAnimation = "",
                AnimationSheet = "",
                AnimationName = "",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 3
            };

        public static StatusEffect Evade =>
            new StatusEffect()
            {
                Name = "Evade",
                SpriteAnimation = "",
                AnimationSheet = "",
                AnimationName = "",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 3
            };

        public static StatusEffect Stunned =>
            new StatusEffect()
            {
                Name = "Stunned",
                SpriteAnimation = "",
                AnimationSheet = "StatusAnimations",
                AnimationName = "Stunned",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 2
            };

        public static StatusEffect AttackUp =>
            new StatusEffect()
            {
                Name = "AttackUp",
                SpriteAnimation = "",
                AnimationSheet = "",
                AnimationName = "",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 3
            };

        public static StatusEffect AttackDown =>
            new StatusEffect()
            {
                Name = "AttackDown",
                SpriteAnimation = "",
                AnimationSheet = "",
                AnimationName = "",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 3
            };

        public static StatusEffect DefenseUp =>
            new StatusEffect()
            {
                Name = "DefenseUp",
                SpriteAnimation = "",
                AnimationSheet = "",
                AnimationName = "",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 3
            };

        public static StatusEffect DefenseDown =>
            new StatusEffect()
            {
                Name = "DefenseDown",
                SpriteAnimation = "",
                AnimationSheet = "",
                AnimationName = "",
                EveryTurnEffects = new List<Action<Member, Member>>
                {
                },
                NumTurnsRemaining = 3
            };
    }
}
