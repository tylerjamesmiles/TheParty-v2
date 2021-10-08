using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class StatusEffect
    {
        string Name;
        string SpriteAnimation;
        string AnimationSheet;
        string AnimationName;
        List<Func<Battle, Targeting, Battle>> EveryTurnEffect;
        int NumTurnsRemaining;

        public bool Gone => NumTurnsRemaining == 0;
        public void DecrementTurnsRemaining() => NumTurnsRemaining -= 1;

        public Battle StateWithTurnEffectsDone(int partyIdx, int memberIdx, Battle state)
        {
            Targeting statusTargeting = new Targeting()
            {
                FromPartyIdx = partyIdx,
                FromMemberIdx = memberIdx,
                ToPartyIdx = partyIdx,
                ToMemberIdx = memberIdx
            };

            Battle Result = Battle.DeepCopyOf(state);
            foreach (var effect in EveryTurnEffect)
            {
                Result = effect(Result, statusTargeting);
            }

            return Result;
        }
    }
}
