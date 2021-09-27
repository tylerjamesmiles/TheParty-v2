using System.Collections.Generic;
using System.Text.Json;

namespace TheParty_v2
{
    struct MemberMove
    {
        public Targeting Targeting;
        public Move Move;
    }

    struct Turn
    {
        public MemberMove[] MemberMoves;
    }

    struct Party
    {
        public Member[] Members;
        public bool AIControlled;

        public Party(string partyName, JsonDocument doc)
        {
            JsonElement party = doc.RootElement.GetProperty(partyName);
            JsonElement members = party.GetProperty("Members");
            Members = new Member[members.GetArrayLength()];
            for (int i = 0; i < members.GetArrayLength(); i++)
            {
                string MemberName = members[i].GetString();
                Members[i] = GameContent.Members[MemberName];
            }

            AIControlled = party.GetProperty("AIControlled").GetBoolean();
        }

        public static Party DeepCopyOf(Party party)
        {
            Party PartyCopy = new Party();
            PartyCopy.Members = new Member[party.Members.Length];
            for (int i = 0; i < party.Members.Length; i++)
                PartyCopy.Members[i] = Member.DeepCopyOf(party.Members[i]);
            PartyCopy.AIControlled = party.AIControlled;
            return PartyCopy;
        }

        public static bool IsDead(Party party)
        {
            bool Result = true;
            foreach (Member member in party.Members)
                if (member.HP > 0)
                    Result = false;
            return Result;
        }

        public static MemberMove[] AllPossibleMemberMoves(Party party, int fromPartyIdx, BattleStore state)
        {
            List<MemberMove> Result = new List<MemberMove>();
            for (int fromMember = 0; fromMember < party.Members.Length; fromMember++)
            {
                if (state.Parties[fromPartyIdx].Members[fromMember].HasGoneThisTurn)
                    continue;

                for (int toParty = 0; toParty < state.Parties.Length; toParty++)
                {
                    for (int toMember = 0; toMember < state.Parties[toParty].Members.Length; toMember++)
                    { 
                        Targeting PotentialTargeting = new Targeting()
                        {
                            FromPartyIdx = fromPartyIdx,
                            FromMemberIdx = fromMember,
                            ToPartyIdx = toParty,
                            ToMemberIdx = toMember
                        };

                        foreach (Move move in Move.AllValidMovesFor(fromPartyIdx, fromMember, state))
                        {
                            MemberMove MemberMove = new MemberMove()
                            {
                                Targeting = PotentialTargeting,
                                Move = move
                            };

                            Result.Add(MemberMove);
                        }
                    }
                }
            }

            return Result.ToArray();
        }

        public static Turn[] AllPossibleTurns(Party party, int fromPartyIdx, BattleStore state)
        {
            List<Turn> Result = new List<Turn>();

            // would rather this be recursive, but you can't have the whole world

            foreach (MemberMove move1 in AllPossibleMemberMoves(party, fromPartyIdx, state))
            {
                if (party.Members.Length == 1)
                {
                    Turn Turn = new Turn() { MemberMoves = new MemberMove[1] { move1 } };
                    Result.Add(Turn);
                    continue;
                }

                foreach (MemberMove move2 in AllPossibleMemberMoves(party, fromPartyIdx, state))
                {
                    if (party.Members.Length == 2)
                    {
                        // same member can't go twice
                        if (move1.Targeting.FromMemberIdx == move2.Targeting.FromMemberIdx)
                            continue;

                        Turn Turn = new Turn() { MemberMoves = new MemberMove[2] { move1, move2 } };
                        Result.Add(Turn);
                        continue;
                    }

                    foreach (MemberMove move3 in AllPossibleMemberMoves(party, fromPartyIdx, state))
                    {
                        if (party.Members.Length == 3)
                        {
                            // same member can't go twice
                            if (move1.Targeting.FromMemberIdx == move3.Targeting.FromMemberIdx ||
                                move2.Targeting.FromMemberIdx == move3.Targeting.FromMemberIdx)
                                continue;

                            Turn Turn = new Turn() { MemberMoves = new MemberMove[3] { move1, move2, move3 } };
                            Result.Add(Turn);
                            continue;
                        }
                    }

                }
            }

            return Result.ToArray();
        }

        public static Turn BestTurn(Party party, int fromPartyIdx, BattleStore state)
        {
            // Get all possible Turns
            Turn BestTurnSoFar = new Turn();
            int BestRatingSoFar = -int.MaxValue;

            foreach (Turn turn in AllPossibleTurns(party, fromPartyIdx, state))
            {
                // Get the resulting state, after the turn is done
                BattleStore ResultState = BattleStore.DeepCopyOf(state);
                foreach (MemberMove memberMove in turn.MemberMoves)
                {
                    ResultState = Move.WithEffectDone(ResultState, memberMove.Move, memberMove.Targeting);   
                }

                // Rate the resulting state
                int Rating = RateState(fromPartyIdx, ResultState);

                if (Rating > BestRatingSoFar)
                {
                    BestTurnSoFar = turn;
                    BestRatingSoFar = Rating;
                }
            }

            return BestTurnSoFar;
        }

        private static int RateState(int fromPartyIdx, BattleStore state)
        {
            int AllyHP = 0;
            int EnemyHP = 0;
            int AllyKOd = 0;
            int EnemyKOd = 0;
            int MeVulnerable = 0;

            for (int party2 = 0; party2 < state.Parties.Length; party2++)
            {
                for (int member = 0; member < state.Parties[party2].Members.Length; member++)
                {
                    Member MemberInQuestion = state.Parties[party2].Members[member];

                    if (party2 == fromPartyIdx)
                    {
                        AllyHP += MemberInQuestion.HP;
                        AllyKOd += MemberInQuestion.KOdFor;
                    }
                    else if (party2 != fromPartyIdx)
                    {
                        EnemyHP += MemberInQuestion.HP;
                        EnemyKOd += MemberInQuestion.KOdFor;

                        for (int myMember = 0; myMember < state.Parties[fromPartyIdx].Members.Length; myMember++)
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
