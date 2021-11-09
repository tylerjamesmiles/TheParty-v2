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

        public Party PartyOf(Member member)
            => Parties.Find(p => p.Members.Contains(member));

        public int PartyIdxOf(Member member)
            => Parties.IndexOf(PartyOf(member));

        public int MemberIdxOf(Member member)
            => PartyOf(member).Members.IndexOf(member);

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

        public void DoMove(Move move, Targeting t)
            => move.Effects(From(t)).ForEach(e => e(From(t), To(t)));



        // ~ ~ ~ ~ MOVE EFFECTS ~ ~ ~ ~


        // STANCE - - -
        public static void HitStanceBy1(Member from, Member to) => to.HitStance(1);
        public static void HitStanceByMinus1(Member from, Member to) => to.HitStance(-1);
        public static void HitStanceByStance(Member from, Member to) => to.HitStance(from.Stance);
        public static void HitHPByStance(Member from, Member to) => to.HitHP(-from.Stance * 2);
        public static void KOCaster(Member from, Member to) => from.HitStance(5 - from.Stance);
        public static void KOTarget(Member from, Member to) => to.HitStance(5 - to.Stance);
        public static void Give1Stance(Member from, Member to) { from.HitStance(-1); to.HitStance(+1); }
        public static void Take1Stance(Member from, Member to) { from.HitStance(+1); to.HitStance(-1); }
        public static void TradeStance(Member from, Member to)
        {
            int Temp = to.Stance;
            to.Stance = from.Stance;
            from.Stance = Temp;
        }

        // STATUS - - -
        public static void AddPoison(Member from, Member to) => to.AddEffect("Poison");
        public static void AddAttackUp(Member from, Member to) => to.AddEffect("AttackUp");
        public static void AddAttackDown(Member from, Member to) => to.AddEffect("AttackDown");
        public static void AddDefenseUp(Member from, Member to) => to.AddEffect("DefenseUp");
        public static void AddDefenseDown(Member from, Member to) => to.AddEffect("DefenseDown");
        public static void AddEvade(Member from, Member to) => to.AddEffect("Evade");
        public static void AddCantCharge(Member from, Member to) => to.AddEffect("CantCharge");
        public static void AddStunned(Member from, Member to) => to.AddEffect("Stunned");

        public static void RemovePoison(Member from, Member to) => to.RemoveEffect("Poison");
        public static void RemoveAttackUp(Member from, Member to) => to.RemoveEffect("AttackUp");
        public static void RemoveAttackDown(Member from, Member to) => to.RemoveEffect("AttackDown");
        public static void RemoveDefenseUp(Member from, Member to) => to.RemoveEffect("DefenseUp");
        public static void RemoveDefenseDown(Member from, Member to) => to.RemoveEffect("DefenseDown");
        public static void RemoveEvade(Member from, Member to) => to.RemoveEffect("Evade");
        public static void RemoveCantCharge(Member from, Member to) => to.RemoveEffect("CantCharge");
        public static void RemoveStunned(Member from, Member to) => to.RemoveEffect("Stunned");


        // HP - - -
        public static void HealHPByHalf(Member from, Member to) => to.HitHP(+1);
        public static void HealHPBy1(Member from, Member to) => to.HitHP(+2);
        public static void HealHPBy2(Member from, Member to) => to.HitHP(+4);
        public static void HitHPBy1(Member from, Member to) => to.HitHP(-1);
        public static void GiveEnoughHP(Member from, Member to)
        {
            int Amt = 10 - to.HP;
            from.HitHP(-Amt);
            to.HitHP(+Amt);
        }
        public static void SuckHP(Member from, Member to)
        {
            from.HP += 2;
            to.HitHP(-2);
        }
        public static void GiveLife(Member from, Member to)
        {
            from.HitHP(-from.HP);
            to.HitHP(+10);
        }

        // KILL - - -
        public static void Kill20(Member from, Member to)
        {
            if (new Random().NextDouble() > 1 - 0.2)
                to.HitHP(-to.HP);
        }
        public static void Kamikaze(Member from, Member to)
        {
            to.HitHP(-to.HP);
            if (from.HP > 2)
                from.HP = 2;
        }

        // CHARGE - - -
        public static void Charge(Member from, Member to) => to.Charged = true;
        public static void TargetLoseCharge(Member from, Member to) => to.Charged = false;
        public static void CasterLoseCharge(Member from, Member to) => from.Charged = false;


        // ~ ~ ~ ~ MOVE CONDITIONS ~ ~ ~ ~
        public static bool CasterAlive(Battle b, Targeting t) => b.From(t).HP > 0;
        public static bool CasterDead(Battle b, Targeting t) => b.From(t).HP == 0;
        public static bool CasterAlert(Battle b, Targeting t) => b.From(t).KOd == false;
        public static bool CasterKOd(Battle b, Targeting t) => b.From(t).KOd == true;
        public static bool CasterAlertAndAlive(Battle b, Targeting t) => CasterAlive(b, t) && CasterAlert(b, t);
        public static bool CasterNotCharged(Battle b, Targeting t) => b.From(t).Charged == false;
        public static bool CasterCharged(Battle b, Targeting t) => b.From(t).Charged == true;

        public static bool TargetAlive(Battle b, Targeting t) => b.To(t).HP > 0;
        public static bool TargetDead(Battle b, Targeting t) => b.To(t).HP == 0;
        public static bool TargetAlert(Battle b, Targeting t) => b.To(t).KOd == false;
        public static bool TargetKOd(Battle b, Targeting t) => b.To(t).KOd == true;
        public static bool TargetAlertAndAlive(Battle b, Targeting t) => TargetAlive(b, t) && TargetAlert(b, t);
        public static bool TargetNotCharged(Battle b, Targeting t) => b.To(t).Charged == false;
        public static bool TargetCharged(Battle b, Targeting t) => b.To(t).Charged == true;

        public static bool TargetIsSelf(Battle b, Targeting t) => TargetIsInSameParty(b, t) && t.FromMemberIdx == t.ToMemberIdx;
        public static bool TargetIsOTher(Battle b, Targeting t) => TargetIsInDifferentParty(b, t) || t.FromMemberIdx != t.ToMemberIdx;
        public static bool TargetIsInSameParty(Battle b, Targeting t) => t.FromPartyIdx == t.ToPartyIdx;
        public static bool TargetIsInDifferentParty(Battle b, Targeting t) => t.FromPartyIdx != t.ToPartyIdx;

        // STATUS - - -
        public static bool HasPoison(Battle b, Targeting t) => b.To(t).HasEffect("Poison");
        public static bool HasAttackUp(Battle b, Targeting t) => b.To(t).HasEffect("AttackUp");
        public static bool HasAttackDown(Battle b, Targeting t) => b.To(t).HasEffect("AttackDown");
        public static bool HasDefenseUp(Battle b, Targeting t) => b.To(t).HasEffect("DefenseUp");
        public static bool HasDefenseDown(Battle b, Targeting t) => b.To(t).HasEffect("DefenseDown");
        public static bool HasEvade(Battle b, Targeting t) => b.To(t).HasEffect("Evade");
        public static bool HasCantCharge(Battle b, Targeting t) => b.To(t).HasEffect("CantCharge");
        public static bool HasStunned(Battle b, Targeting t) => b.To(t).HasEffect("Stunned");

        public static bool DoesntHavePoison(Battle b, Targeting t) => !b.To(t).HasEffect("Poison");
        public static bool DoesntHaveAttackUp(Battle b, Targeting t) => !b.To(t).HasEffect("AttackUp");
        public static bool DoesntHaveAttackDown(Battle b, Targeting t) => !b.To(t).HasEffect("AttackDown");
        public static bool DoesntHaveDefenseUp(Battle b, Targeting t) => !b.To(t).HasEffect("DefenseUp");
        public static bool DoesntHaveDefenseDown(Battle b, Targeting t) => !b.To(t).HasEffect("DefenseDown");
        public static bool DoesntHaveEvade(Battle b, Targeting t) => !b.To(t).HasEffect("Evade");
        public static bool DoesntHaveCantCharge(Battle b, Targeting t) => !b.To(t).HasEffect("CantCharge");
        public static bool DoesntHaveStunned(Battle b, Targeting t) => !b.To(t).HasEffect("Stunned");
    }
}
