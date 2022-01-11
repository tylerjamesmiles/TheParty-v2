using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text.Json;
using Newtonsoft.Json;

namespace TheParty_v2
{
    class Move
    {
        public string Name;
        public string Description;
        public List<string> Effects;
        public List<string> Conditions;

        public Move(JsonElement move)
        {
            Name = move.GetProperty("Name").GetString();
            Description = move.GetProperty("Description").GetString();

            Effects = new List<string>();
            for (int i = 0; i < move.GetProperty("Effects").GetArrayLength(); i++)
                Effects.Add(move.GetProperty("Effects")[i].GetString());

            Conditions = new List<string>();
            for (int i = 0; i < move.GetProperty("Conditions").GetArrayLength(); i++)
                Conditions.Add(move.GetProperty("Conditions")[i].GetString());
        }

        [JsonConstructor]
        public Move(string name, string description, List<string> effects, List<string> conditions)
        {
            Name = name;
            Description = description;
            Effects = effects;
            Conditions = conditions;
        }

        public static string InvalidMoveName = "Invalid";

        public bool ValidOnMember(Battle state, Targeting targeting) =>
            state.From(targeting).HP > 0 &&
            state.To(targeting).HP > 0 &&
            Conditions.TrueForAll(c => Battle.CheckCondition(c, state, targeting));

        public bool ValidOnAnyone(Battle state, int fromPartyIdx, int fromMemberIdx) => 
            state.AllTargetingFor(fromPartyIdx, fromMemberIdx).Exists(t => ValidOnMember(state, t));
    }
}
