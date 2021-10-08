using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    class Battle
    {
        public List<Party> Parties;
        public int CurrentTurnPartyIdx;

        public Battle(string stateName, JsonDocument doc)
        {
            JsonElement parties = doc.RootElement.GetProperty(stateName).GetProperty("Parties");
            Parties = new List<Party>();
            for (int i = 0; i < parties.GetArrayLength(); i++)
                Parties.Add(GameContent.Parties[parties[i].GetString()]);
            CurrentTurnPartyIdx = 0;
        }

        public Battle(List<Party> parties, int currentTurn)
        {
            Parties = parties;
            CurrentTurnPartyIdx = currentTurn;
        }
        public Battle DeepCopy() => new Battle(Parties.ConvertAll(p => p.DeepCopy()), CurrentTurnPartyIdx);



        // GETTERS

        public int NumParties => Parties.Count;
        public Member To(Targeting targeting) => Parties[targeting.ToPartyIdx].Members[targeting.ToMemberIdx];
        public Member From(Targeting targeting) => Parties[targeting.FromPartyIdx].Members[targeting.FromMemberIdx];
        public Party CurrentTurnParty => Parties[CurrentTurnPartyIdx];
        public int TotalNumMembers => AllMembers().Count;
        public int LargestNumMembers()
        {
            int Result = -int.MaxValue;
            Parties.ForEach(p => Result = (p.Members.Count > Result) ? p.Members.Count : Result); 
            return Result;
        }

        public Member Member(int partyIdx, int memberIdx) => Parties[partyIdx].Members[memberIdx];
        public List<Member> MembersOfParty(int partyIdx) => Parties[partyIdx].Members;

        public List<Member> AllMembers()
        {
            List<Member> Result = new List<Member>();
            Parties.ForEach(p => Result.AddRange(p.Members));
            return Result;
        }

        public bool IsTerminal() => Parties.FindAll(p => p.IsDead).Count == Parties.Count - 1;

        public List<Targeting> AllTargetingFor(int partyIdx, int memberIdx)
        {
            List<Targeting> Result = new List<Targeting>();
            for (int toP = 0; toP < Parties.Count; toP++)
                for (int toM = 0; toM < Parties[toP].Members.Count; toM++)
                    Result.Add(new Targeting(partyIdx, memberIdx, toP, toM));
            return Result;
        }

        // ~ ~ ~ ~ SETTERS ~ ~ ~ ~

        public void TimePass()
        {
            // CurrentTurnPartyIdx
            int NextTurnPartyIdx = CurrentTurnPartyIdx + 1;
            if (NextTurnPartyIdx >= Parties.Count)
                NextTurnPartyIdx -= Parties.Count;
            CurrentTurnPartyIdx = NextTurnPartyIdx;

            // Time pass on Status Effects

        }

        public void DoMove(Move move, Targeting t) => move.Effects(this, t).ForEach(e => e(this, t));


        // ~ ~ ~ ~ MOVE EFFECTS ~ ~ ~ ~


        // - - - STANCE - - -
        public static void HitStanceBy1(Battle b, Targeting t) => b.To(t).HitStance(1);
        public static void HitStanceByStance(Battle b, Targeting t) => b.To(t).HitStance(b.From(t).Stance);
        public static void HitHPByStance(Battle b, Targeting t) => b.To(t).HitHP(-b.From(t).Stance * 2);
        public static void KO(Battle b, Targeting t) => b.To(t).HitStance(5 - b.To(t).Stance);
        public static void Give1Stance(Battle b, Targeting t)
        {
            b.From(t).HitStance(+1);
            b.To(t).HitStance(-1);
        }


        // - - - HP - - -
        public static void HealHPBy1(Battle b, Targeting t) => b.To(t).HitHP(+2);
        public static void HitHPBy1(Battle b, Targeting t) => b.To(t).HitHP(-1);


        // - - - CHARGE - - -
        public static void Charge(Battle b, Targeting t) => b.To(t).Charged = true;
        public static void TargetLoseCharge(Battle b, Targeting t) => b.To(t).Charged = false;
        public static void CasterLoseCharge(Battle b, Targeting t) => b.From(t).Charged = false;


        // ~ ~ ~ ~ MOVE CONDITIONS ~ ~ ~ ~
        public static bool CasterAlive(Battle b, Targeting t) => b.From(t).HP > 0;
        public static bool CasterDead(Battle b, Targeting t) => b.From(t).HP == 0;
        public static bool CasterAlert(Battle b, Targeting t) => b.From(t).KOd == false;
        public static bool CasterKOd(Battle b, Targeting t) => b.From(t).KOd == true;
        public static bool CasterNotCharged(Battle b, Targeting t) => b.From(t).Charged == false;
        public static bool CasterCharged(Battle b, Targeting t) => b.From(t).Charged == true;

        public static bool TargetAlive(Battle b, Targeting t) => b.To(t).HP > 0;
        public static bool TargetDead(Battle b, Targeting t) => b.To(t).HP == 0;
        public static bool TargetAlert(Battle b, Targeting t) => b.To(t).KOd == false;
        public static bool TargetKOd(Battle b, Targeting t) => b.To(t).KOd == true;
        public static bool TargetNotCharged(Battle b, Targeting t) => b.To(t).Charged == false;
        public static bool TargetCharged(Battle b, Targeting t) => b.To(t).Charged == true;

        public static bool TargetIsSelf(Battle b, Targeting t) => TargetIsInSameParty(b, t) && t.FromMemberIdx == t.ToMemberIdx;
        public static bool TargetIsOTher(Battle b, Targeting t) => TargetIsInDifferentParty(b, t) || t.FromMemberIdx != t.ToMemberIdx;
        public static bool TargetIsInSameParty(Battle b, Targeting t) => t.FromPartyIdx == t.ToPartyIdx;
        public static bool TargetIsInDifferentParty(Battle b, Targeting t) => t.FromPartyIdx != t.ToPartyIdx;
    }
}
