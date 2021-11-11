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

        [JsonConstructor]
        public Party(List<Member> members) { Members = members; }
        public Party DeepCopy() => new Party(Members.ConvertAll(m => m.DeepCopy()));


        // ~ ~ ~ ~ GETTERS ~ ~ ~ ~ 

        public int NumMembers => Members.Count;
        public int NumAlertMembers => Members.FindAll(m => m.CanGo).Count;
        public bool IsKOd => Members.TrueForAll(m => m.KOd);
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

            if (BestTurnSoFar.Move != null)
                Debug.Write(BestTurnSoFar.Move.Name + "\n");

            return BestTurnSoFar;
        }

        private int RateState(int fromPartyIdx, Battle state)
        {
            int AllyHP = 0;
            int AllyKOd = 0;
            int AllyAlive = 0;
            int AllyDead = 0;
            int AllyCharged = 0;
            int AllyVulnerable = 0;
            int AllyPositiveStatus = 0;
            int AllyNegativeStatus = 0;

            int EnemyHP = 0;
            int EnemyKOd = 0;
            int EnemyAlive = 0;
            int EnemyDead = 0;
            int EnemyCharged = 0;
            int EnemyVulnerable = 0;
            int EnemyPositiveStatus = 0;
            int EnemyNegativeStatus = 0;

            for (int party2 = 0; party2 < state.NumParties; party2++)
            {
                for (int member = 0; member < state.Parties[party2].NumMembers; member++)
                {
                    Member MemberInQuestion = state.Member(party2, member);

                    // Looking at Ally
                    if (party2 == fromPartyIdx)
                    {
                        AllyHP += MemberInQuestion.HP;
                        AllyKOd += MemberInQuestion.KOd ? 1 : 0;
                        AllyCharged += MemberInQuestion.Charged ? 1 : 0;
                        AllyAlive += MemberInQuestion.HP > 0 ? 1 : 0;
                        AllyDead += MemberInQuestion.HP == 0 ? 1 : 0;

                        AllyPositiveStatus +=
                            MemberInQuestion.HasEffect("Evade") ||
                            MemberInQuestion.HasEffect("AttackUp") ||
                            MemberInQuestion.HasEffect("DefenseUp") ? 1 : 0;

                        AllyNegativeStatus +=
                            MemberInQuestion.HasEffect("Poison") ||
                            MemberInQuestion.HasEffect("CantCharge") ||
                            MemberInQuestion.HasEffect("Stunned") ||
                            MemberInQuestion.HasEffect("AttackDown") ||
                            MemberInQuestion.HasEffect("DefenseDown") ? 1 : 0;

                        // can this member ko any enemies?
                        for (int enemyMember = 0; enemyMember < state.Parties[fromPartyIdx].NumMembers; enemyMember++)
                        {
                            Member EnemyMember = state.Parties[fromPartyIdx].Members[enemyMember];
                            if (MemberInQuestion.Stance + EnemyMember.Stance >= 5)
                                EnemyVulnerable += 1;
                        }
                    }

                    // Opponent
                    else if (party2 != fromPartyIdx)
                    {
                        EnemyHP += MemberInQuestion.HP;
                        EnemyKOd += MemberInQuestion.KOd ? 1 : 0;
                        EnemyCharged += MemberInQuestion.Charged ? 1 : 0;
                        EnemyAlive += MemberInQuestion.HP > 0 ? 1 : 0;
                        EnemyDead += MemberInQuestion.HP == 0 ? 1 : 0;

                        EnemyPositiveStatus +=
                            MemberInQuestion.HasEffect("Evade") ||
                            MemberInQuestion.HasEffect("AttackUp") ||
                            MemberInQuestion.HasEffect("DefenseUp") ? 1 : 0;

                        EnemyNegativeStatus +=
                            MemberInQuestion.HasEffect("Poison") ||
                            MemberInQuestion.HasEffect("CantCharge") ||
                            MemberInQuestion.HasEffect("Stunned") ||
                            MemberInQuestion.HasEffect("AttackDown") ||
                            MemberInQuestion.HasEffect("DefenseDown") ? 1 : 0;

                        // can this member ko any of mine?
                        for (int myMember = 0; myMember < state.Parties[fromPartyIdx].NumMembers; myMember++)
                        {
                            Member MyMember = state.Parties[fromPartyIdx].Members[myMember];
                            if (MemberInQuestion.Stance + MyMember.Stance >= 5)
                                AllyVulnerable += 1;
                        }
                    }
                }
            }

            return
                + AllyHP
                - AllyKOd * 3
                + AllyAlive * 4
                - AllyDead * 5
                + AllyCharged
                - AllyVulnerable * 2
                + AllyPositiveStatus
                - AllyNegativeStatus

                - EnemyHP
                + EnemyKOd * 3
                - EnemyAlive * 4
                + EnemyDead * 5
                - EnemyCharged
                + EnemyVulnerable * 2
                - EnemyPositiveStatus
                + EnemyNegativeStatus
                ;
        }
       
        
    }


}
