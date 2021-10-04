using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    struct BattleStore
    {
        public Party[] Parties;
        public int CurrentTurnPartyIdx;
        public int AvailableCharge;

        public static int NumDeepCopiesMade = 0;

        public BattleStore(string stateName, JsonDocument doc)
        {
            JsonElement parties = doc.RootElement.GetProperty(stateName).GetProperty("Parties");
            Parties = new Party[parties.GetArrayLength()];
            for (int i = 0; i < Parties.Length; i++)
                Parties[i] = GameContent.Parties[parties[i].GetString()];
            CurrentTurnPartyIdx = 0;
            AvailableCharge = Parties.Length;
        }

        public static BattleStore DeepCopyOf(BattleStore state)
        {
            NumDeepCopiesMade++;

            Party[] NewParties = new Party[state.Parties.Length];
            for (int i = 0; i < state.Parties.Length; i++)
                NewParties[i] = Party.DeepCopyOf(state.Parties[i]);

            BattleStore StateCopy = new BattleStore()
            {
                Parties = NewParties,
                CurrentTurnPartyIdx = state.CurrentTurnPartyIdx,
                AvailableCharge = state.AvailableCharge
            };

            return StateCopy;
        }

        public static Party CurrentTurnPartyOf(BattleStore state)
        {
            return state.Parties[state.CurrentTurnPartyIdx];
        }

        public static int TotalNumMembers(BattleStore state)
        {
            int Result = 0;
            foreach (Party party in state.Parties)
                Result += party.Members.Length;
            return Result;
        }

        public static BattleStore TimePassed(BattleStore state)
        {
            BattleStore ResultState = DeepCopyOf(state);

            // CurrentTurnPartyIdx
            int NextTurnPartyIdx = state.CurrentTurnPartyIdx + 1;
            if (NextTurnPartyIdx >= state.Parties.Length)
                NextTurnPartyIdx -= state.Parties.Length;
            ResultState.CurrentTurnPartyIdx = NextTurnPartyIdx;

            //// Turn off all has-moveds for CurrentTurnParty
            //for (int party = 0; party < ResultState.Parties.Length; party++)
            //    for (int member = 0; member < ResultState.Parties[party].Members.Length; member++)
            //    {
            //        ref Member Member = ref ResultState.Parties[party].Members[member];
            //        Member.HasGoneThisTurn = false;
            //    }

            // DecrementKOs
            //for (int party = 0; party < ResultState.Parties.Length; party++)
            //    for (int member = 0; member < ResultState.Parties[party].Members.Length; member++)
            //    {
            //        ref Member Member = ref ResultState.Parties[party].Members[member];

            //        // continues to decrement, even after recovery
            //        Member.KOdFor -= 1;
            //    }

            return ResultState;
        }

        public static int LargestNumMembers(BattleStore state)
        {
            int Result = -int.MaxValue;
            foreach (Party party in state.Parties)
                if (party.Members.Length > Result)
                    Result = party.Members.Length;
            return Result;
        }

        public static ref Member Member(BattleStore state, int partyIdx, int memberIdx) =>
            ref state.Parties[partyIdx].Members[memberIdx];


        public Member[] MembersOfParty(int partyIdx) =>
            Parties[partyIdx].Members;

        public Member[] AllMembers()
        {
            List<Member> Result = new List<Member>();
            foreach (Party party in Parties)
                foreach (Member member in party.Members)
                    Result.Add(member);
            return Result.ToArray();
        }

        public static bool IsTerminal(BattleStore state)
        {
            // Is all but one party dead?
            int NumDeadParties = 0;
            foreach (Party party in state.Parties)
                if (Party.IsDead(party))
                    NumDeadParties += 1;
            return (NumDeadParties == state.Parties.Length - 1);
        }
    }
}
