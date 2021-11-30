using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text.Json;

namespace TheParty_v2
{
    class Move
    {
        public string Name;
        public string Description;
        public string AnimationSheet;
        public string AnimationName;
        public bool PositiveEffect;
        public List<string> Effects;
        public List<string> MoveConditions;

        public Move(JsonElement move)
        {
            Name = move.GetProperty("Name").GetString();
            Description = move.GetProperty("Description").GetString();
            AnimationSheet = move.GetProperty("AnimationSheet").GetString();
            AnimationName = move.GetProperty("AnimationName").GetString();
            PositiveEffect = move.GetProperty("PositiveEffect").GetBoolean();

            Effects = new List<string>();
            for (int i = 0; i < move.GetProperty("Effects").GetArrayLength(); i++)
                Effects.Add(move.GetProperty("Effects")[i].GetString());

            MoveConditions = new List<string>();
            for (int i = 0; i < move.GetProperty("MoveConditions").GetArrayLength(); i++)
                MoveConditions.Add(move.GetProperty("MoveConditions")[i].GetString());
        }

        public static string InvalidMoveName = "Invalid";

        public bool ValidOnMember(Battle state, Targeting targeting)
            => MoveConditions.TrueForAll(c => Battle.CheckCondition(c, state, targeting));

        public bool ValidOnAnyone(Battle state, int fromPartyIdx, int fromMemberIdx)
            => state.AllTargetingFor(fromPartyIdx, fromMemberIdx).Exists(t => ValidOnMember(state, t));
    }
}
