using System.Collections.Generic;
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

        public Party(string partyName, JsonDocument doc)
        {
            JsonElement party = doc.RootElement.GetProperty(partyName);
            JsonElement members = party.GetProperty("Members");
            Members = new List<Member>();
            for (int i = 0; i < members.GetArrayLength(); i++)
            {
                string MemberName = members[i].GetString();
                Members.Add(GameContent.Members[MemberName].DeepCopy());
            }
        }

        public Party(List<Member> members) { Members = members; }
        public Party DeepCopy() => new Party(Members.ConvertAll(m => m.DeepCopy()));


        // ~ ~ ~ ~ GETTERS ~ ~ ~ ~ 

        public int NumMembers => Members.Count;
        public bool IsKOd => Members.TrueForAll(m => m.KOd);
        public bool IsDead => Members.TrueForAll(m => m.HP == 0);


        public List<MemberMove> AllPossibleMemberMoves(int pIdx, Battle state)
        {
            List<MemberMove> Result = new List<MemberMove>();
            foreach (Member member in Members)
                foreach (MemberMove mm in member.AllValidMoves(pIdx, Members.IndexOf(member), state))
                    Result.Add(mm);
            return Result;
        }

        public MemberMove BestTurn(int pIdx, Battle state)
        {
            // Get all possible Turns
            MemberMove BestTurnSoFar = new MemberMove();
            int BestRatingSoFar = -int.MaxValue;

            foreach (MemberMove turn in AllPossibleMemberMoves(pIdx, state))
            {
                Battle copy = state.DeepCopy();
                copy.DoMove(turn.Move, turn.Targeting);

                // Rate the resulting state
                int Rating = RateState(pIdx, copy);

                if (Rating > BestRatingSoFar)
                {
                    BestTurnSoFar = turn;
                    BestRatingSoFar = Rating;
                }
            }

            return BestTurnSoFar;
        }

        private int RateState(int fromPartyIdx, Battle state)
        {
            int AllyHP = 0;
            int EnemyHP = 0;
            int AllyKOd = 0;
            int EnemyKOd = 0;
            int MeVulnerable = 0;

            for (int party2 = 0; party2 < state.NumParties; party2++)
            {
                for (int member = 0; member < state.Parties[party2].NumMembers; member++)
                {
                    Member MemberInQuestion = state.Member(party2, member);

                    // Ally
                    if (party2 == fromPartyIdx)
                    {
                        AllyHP += MemberInQuestion.HP;
                        AllyKOd += MemberInQuestion.KOd ? 1 : 0;
                    }

                    // Opponent
                    else if (party2 != fromPartyIdx)
                    {
                        EnemyHP += MemberInQuestion.HP;
                        EnemyKOd += MemberInQuestion.KOd ? 1 : 0;

                        for (int myMember = 0; myMember < state.Parties[fromPartyIdx].NumMembers; myMember++)
                        {
                            Member MyMember = state.Parties[fromPartyIdx].Members[myMember];
                            if (MemberInQuestion.Stance + MyMember.Stance == 5)
                                MeVulnerable += 2;
                        }
                    }
                }
            }

            return 
                + AllyHP
                - AllyKOd

                + EnemyKOd
                - EnemyHP 

                - MeVulnerable;
        }
       
        
    }


}
