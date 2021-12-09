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
        public List<string> EveryTurnEffects;
        public List<string> PassiveEffects;
        public int NumTurnsRemaining;

        public StatusEffect() { }

        // TODO: Change this constructor to something simpler
        public StatusEffect(JsonElement status)
        {
            Name = status.GetProperty("Name").GetString();

            var EveryTurnEffects = status.GetProperty("EveryTurnEffects");
            int NumEffects = EveryTurnEffects.GetArrayLength();
            for (int i = 0; i < NumEffects; i++)
                this.EveryTurnEffects.Add(EveryTurnEffects[i].GetString());

            var PassiveEffects = status.GetProperty("PassiveEffects");
            int NumPEffects = PassiveEffects.GetArrayLength();
            for (int i = 0; i < NumPEffects; i++)
                this.PassiveEffects.Add(PassiveEffects[i].GetString());
            
            NumTurnsRemaining = status.GetProperty("NumTurnsRemaining").GetInt32();
        }

        public StatusEffect DeepCopy()
        {
            return new StatusEffect()
            {
                Name = Name,
                EveryTurnEffects = EveryTurnEffects,
                PassiveEffects = PassiveEffects,
                NumTurnsRemaining = NumTurnsRemaining
            };
        }

        public bool Gone => NumTurnsRemaining == 0;
        public bool HasEveryTurnEffect => EveryTurnEffects.Count > 0;
        public void DecrementTurnsRemaining() => NumTurnsRemaining -= 1;

        public int StatBonus(string stat, int stance)
        {
            foreach (string effect in PassiveEffects)
            {
                string[] Keywords = effect.Split(' ');
                string Type = Keywords[0];

                if (Type == stat)   // e.g. "Attack", "Defense" . . .
                {
                    string Operation = Keywords[1];
                    int Amt = int.Parse(Keywords[2]);
                    switch (Operation)
                    {
                        case "+": return Amt;
                        case "*": return stance * Amt;
                        case "-": return -Amt;
                        default: throw new Exception("Invalid operation " + Operation);
                    }
                }
            }
            return 0;
        }
        public void Do(Member member)
            => EveryTurnEffects.ForEach(e => Battle.DoEffect(e, member, member));

        
    }
}
