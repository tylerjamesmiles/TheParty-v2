using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class StatusEffect
    {
        public string Name;
        public string SpriteAnimation;
        public string AnimationSheet;
        public string AnimationName;
        public List<string> EveryTurnEffects;
        public int NumTurnsRemaining;

        public StatusEffect() { }

        public StatusEffect(JsonElement status)
        {
            Name = status.GetProperty("Name").GetString();
            SpriteAnimation = status.GetProperty("SpriteAnimation").GetString();
            AnimationSheet = status.GetProperty("AnimationSheet").GetString();
            AnimationName = status.GetProperty("AnimationName").GetString();

            var Effects = status.GetProperty("EveryTurnEffects");
            int NumEffects = Effects.GetArrayLength();
            for (int i = 0; i < NumEffects; i++)
                EveryTurnEffects.Add(Effects[i].GetString());

            NumTurnsRemaining = status.GetProperty("NumTurnsRemaining").GetInt32();
        }

        public StatusEffect DeepCopy()
        {
            return new StatusEffect()
            {
                Name = Name,
                AnimationName = AnimationName,
                AnimationSheet = AnimationSheet,
                EveryTurnEffects = EveryTurnEffects,
                NumTurnsRemaining = NumTurnsRemaining,
                SpriteAnimation = SpriteAnimation
            };
        }

        public bool Gone => NumTurnsRemaining == 0;
        public bool HasEveryTurnEffect => EveryTurnEffects.Count > 0;
        public void DecrementTurnsRemaining() => NumTurnsRemaining -= 1;
        public void Do(Battle state, Member member)
            => EveryTurnEffects.ForEach(e => Battle.DoEffect(e, member, member));

        
    }
}
