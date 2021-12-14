using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Equipment
    {
        public string Name { get; set; }
        public List<string> EveryTurnEffects { get; set; }
        public List<string> PassiveEffects { get; set; }

        [JsonConstructor]
        public Equipment(string name, List<string> everyTurnEffects, List<string> passiveEffects)
        {
            Name = name;
            EveryTurnEffects = everyTurnEffects;
            PassiveEffects = passiveEffects;
        }

        public List<string> BonusMoves()
        {
            List<string> Result = new List<string>();
            foreach (string effect in PassiveEffects)
            {
                string[] Keywords = effect.Split(' ');
                if (Keywords[0] == "+Move")
                    Result.Add(Keywords[1]);
            }
            return Result;
        }

        public int StatBonuses(string stat)
        {
            foreach (string effect in PassiveEffects)
            {
                string[] Keywords = effect.Split(' ');
                string Type = Keywords[0];

                if (Type == stat)   // e.g. "Attack", "Defense" . . .
                {
                    string Operation = Keywords[1];
                    string Amt = Keywords[2];
                    switch (Operation)
                    {
                        case "+": return int.Parse(Amt);
                        case "*": return int.Parse(Amt);    // Still haven't implemented
                        case "-": return -int.Parse(Amt);
                        default: throw new Exception("Invalid operation " + Operation);
                    }
                }
            }
            return 0;
        }

        public int StatWithBonus(string stat, int stance) => stance + StatBonuses(stat);
    }
}
