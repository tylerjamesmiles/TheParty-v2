using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace TheParty_v2
{
    struct MemberMove
    {
        public Targeting Targeting;
        public Move Move;

        public MemberMove(Targeting t, Move m) { Targeting = t; Move = m; }
    }

    class Party
    {
        public List<Member> Members;

        public Party(JsonElement party)
        {
            JsonElement members = party.GetProperty("Members");
            Members = new List<Member>();
            for (int i = 0; i < members.GetArrayLength(); i++)
            {
                string MemberName = members[i].GetString();
                Members.Add(GameContent.Members[MemberName].DeepCopy());
            }
        }

        [JsonConstructor]
        public Party(List<Member> members) { Members = members; }
        public Party DeepCopy() => new Party(Members.ConvertAll(m => m.DeepCopy()));


        // ~ ~ ~ ~ GETTERS ~ ~ ~ ~ 

        public int NumMembers => Members.Count;
        public int NumAlertMembers => Members.FindAll(m => m.CanGo).Count;
        public bool IsDead => Members.TrueForAll(m => m.HP == 0);


        public List<MemberMove> AllPossibleMemberMoves(int pIdx, Battle state)
        {
            List<MemberMove> Result = new List<MemberMove>();
            foreach (Member member in Members.FindAll(m => m.CanGo))
                Result.AddRange(member.AllValidMoves(pIdx, Members.IndexOf(member), state));
            return Result;
        }

        public MemberMove BestTurn(int pIdx, Battle state)
        {
            var Moves = AllPossibleMemberMoves(pIdx, state);
            int MoveIdx = new Random().Next(Moves.Count);
            return Moves[MoveIdx];
        }
      
    }


}
