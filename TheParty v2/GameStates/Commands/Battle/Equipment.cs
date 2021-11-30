using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Equipment
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Detail { get; set; }

        [JsonConstructor]
        public Equipment(string name, string type, string detail)
        {
            Name = name;
            Type = type;
            Detail = detail;
        }

        public int Bonus(string stat)
        {
            if (Type == stat)   // e.g. "Attack", "Defense" . . .
            {
                string Operation = Detail.Split(' ')[0];
                string Amt = Detail.Split(' ')[1];
                switch (Operation)
                {
                    case "+": return int.Parse(Amt);
                    case "*": return int.Parse(Amt);
                    default: throw new Exception("Invalid operation " + Operation);
                }
            }
            else
                return 0;
        }

        public int StatWithBonus(string stat, int stance) => stance + Bonus(stat);
    }
}
