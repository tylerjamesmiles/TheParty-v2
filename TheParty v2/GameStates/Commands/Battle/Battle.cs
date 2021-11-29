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
        public string[] Rewards;

        public Battle(JsonElement doc)
        {
            JsonElement parties = doc.GetProperty("Parties");
            Parties = new List<Party>();
            for (int i = 0; i < parties.GetArrayLength(); i++)
                Parties.Add(GameContent.Parties[parties[i].GetString()]);

            JsonElement rewards = doc.GetProperty("Rewards");
            int NumRewards = rewards.GetArrayLength();
            Rewards = new string[NumRewards];
            for (int i = 0; i < NumRewards; i++)
                Rewards[i] = rewards[i].GetString();

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
            // Reset everyone's GoneThisTurn
            foreach (Member member in AllMembers())
                member.GoneThisTurn = false;

            // CurrentTurnPartyIdx
            int NextTurnPartyIdx = CurrentTurnPartyIdx + 1;
            if (NextTurnPartyIdx >= Parties.Count)
                NextTurnPartyIdx -= Parties.Count;
            CurrentTurnPartyIdx = NextTurnPartyIdx;

        }

        public void DoMove(Move move, Targeting t)
            => move.Effects.ForEach(e => DoEffect(e, From(t), To(t)));


        // ~ ~ ~ ~ MOVE EFFECTS ~ ~ ~ ~

        private static void DoEffect(string effect, Member from, Member to)
        {

        }


        // STANCE - - -

        // for convenience
        private static void HitTargetStance(Member target, int amt)
        {
            if (!target.HasEffect("Evade"))
                target.HitStance(amt);
            else
                target.RemoveStatusEffect("Evade");
        }
        public static Action<Member, Member> HitStance(string member, int amt)
        {
            if (member == "Caster") 
                return (from, to) => HitTargetStance(from, amt);
            else 
                return (from, to) => HitTargetStance(to, amt);
        }
        public static void HitHPByStance(Member from, Member to) => HitHP(from, to, -(from.Stance * 2 + to.Stance));
        public static void Give1Stance(Member from, Member to) { from.HitStance(-1); to.HitStance(+1); }
        public static void Take1Stance(Member from, Member to) { from.HitStance(+1); to.HitStance(-1); }
        public static void ResetStance(Member from, Member to) { to.Stance = 1; }
        public static void TradeStance(Member from, Member to)
        {
            int Temp = to.Stance;
            to.Stance = from.Stance;
            from.Stance = Temp;
        }

        // STATUS - - -
        public static Action<Member, Member> AddStatus(string name) =>
            new Action<Member, Member>((from, to) => to.AddStatusEffect(name));
        public static Action<Member, Member> RemoveStatus(string name) =>
            new Action<Member, Member>((from, to) => to.RemoveStatusEffect(name));
       

        // HP - - -
        private static void HitHP(Member from, Member to, int amt)
        {
            int Amt = amt;

            if (to.HasEffect("Evade"))
            {
                to.RemoveStatusEffect("Evade");
                return;
            }
            if (from.HasEffect("AttackUp"))
            {
                Amt *= 2;
                //from.RemoveStatusEffect("AttackUp");
            }
            if (from.HasEffect("AttackDown"))
            {
                Amt /= 2;
                //from.RemoveStatusEffect("AttackDown");
            }
            if (to.HasEffect("DefenseUp"))
            {
                Amt /= 2;

                if (to.HP == 3)
                    Amt = 1;

                if (to.HP == 2)
                    Amt = 0;

                //to.RemoveStatusEffect("DefenseUp");
            }
            if (to.HasEffect("DefenseDown"))
            {
                Amt *= 2;
                //to.RemoveStatusEffect("DefenseDown");
            }

            to.HitHP(Amt);

        }

        public static void TradeHearts(Member from, Member to)
        {
            int FromHP = from.HP;
            from.HP = to.HP;
            to.HP = FromHP;
        }

        public static Action<Member, Member> HitHP(int amt) => (from, to) => HitHP(from, to, amt);

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

        // ~ ~ ~ ~ MOVE CONDITIONS ~ ~ ~ ~

        public static Func<Battle, Targeting, bool> HP(string member, string op, int amt)
        { 
            if (member == "Caster")
            {
                switch (op)
                {
                    case "==": return (b, t) => b.From(t).HP == amt;
                    case "!=": return (b, t) => b.From(t).HP != amt;
                    case ">": return (b, t) => b.From(t).HP > amt;
                    case "<": return (b, t) => b.From(t).HP < amt;
                    case ">=": return (b, t) => b.From(t).HP >= amt;
                    case "<=": return (b, t) => b.From(t).HP <= amt;
                }
            }
            else if (member == "Target")
            {
                switch (op)
                {
                    case "==": return (b, t) => b.To(t).HP == amt;
                    case "!=": return (b, t) => b.To(t).HP != amt;
                    case ">": return (b, t) => b.To(t).HP > amt;
                    case "<": return (b, t) => b.To(t).HP < amt;
                    case ">=": return (b, t) => b.To(t).HP >= amt;
                    case "<=": return (b, t) => b.To(t).HP <= amt;
                }
            }

            return (b, t) => false;
        }

        public static Func<Battle, Targeting, bool> Stance(string member, string op, int amt)
        {
            if (member == "Caster")
            {
                switch (op)
                {
                    case "==": return (b, t) => b.From(t).Stance == amt;
                    case "!=": return (b, t) => b.From(t).Stance != amt;
                    case ">": return (b, t) => b.From(t).Stance > amt;
                    case "<": return (b, t) => b.From(t).Stance < amt;
                    case ">=": return (b, t) => b.From(t).Stance >= amt;
                    case "<=": return (b, t) => b.From(t).Stance <= amt;
                }
            }
            else if (member == "Target")
            {
                switch (op)
                {
                    case "==": return (b, t) => b.To(t).Stance == amt;
                    case "!=": return (b, t) => b.To(t).Stance != amt;
                    case ">": return (b, t) => b.To(t).Stance > amt;
                    case "<": return (b, t) => b.To(t).Stance < amt;
                    case ">=": return (b, t) => b.To(t).Stance >= amt;
                    case "<=": return (b, t) => b.To(t).Stance <= amt;
                }
            }

            return (b, t) => false;
        }


        public static bool TargetIsSelf(Battle b, Targeting t) => TargetIsInSameParty(b, t) && t.FromMemberIdx == t.ToMemberIdx;
        public static bool TargetIsOTher(Battle b, Targeting t) => TargetIsInDifferentParty(b, t) || t.FromMemberIdx != t.ToMemberIdx;
        public static bool TargetIsInSameParty(Battle b, Targeting t) => t.FromPartyIdx == t.ToPartyIdx;
        public static bool TargetIsInDifferentParty(Battle b, Targeting t) => t.FromPartyIdx != t.ToPartyIdx;

        // STATUS - - -
        public static Func<Battle, Targeting, bool> StatusEffect(string member, string hasOrNot, string effect)
        {
            if (hasOrNot == "Has")
            {
                if (member == "Caster")
                    return (b, t) => b.From(t).HasEffect(effect);
                else
                    return (b, t) => b.To(t).HasEffect(effect);
            }
            else
            {
                if (member == "Caster")
                    return (b, t) => !b.From(t).HasEffect(effect);
                else
                    return (b, t) => !b.To(t).HasEffect(effect);
            }
        }
    }
}
