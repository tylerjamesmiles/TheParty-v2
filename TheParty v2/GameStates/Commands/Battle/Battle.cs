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


            CurrentTurnPartyIdx = new Random().Next(2);
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


        // ~ ~ ~ ~ MOVE EFFECTS ~ ~ ~ ~\
        public static void DoEffect(string effect, Member from, Member to)
        {
            string Effect = effect.Remove(effect.IndexOf('('));
            string ParamsWithParen = effect.Remove(0, Effect.Length);
            string ParamsNoFirstParen = ParamsWithParen.Remove(0, 1);
            string ParamsNoLastParen = ParamsNoFirstParen.Remove(ParamsWithParen.Length - 2, 1);
            List<string> Arguments = new List<string>(ParamsNoLastParen.Split(','));
            Arguments.ForEach(a => a = a.Trim());

            // All Effect parameters are:
            //      name(?target, ?amt)

            Member Target = 
                Arguments.Count > 0 ? 
                    Arguments[0] == "Caster" ? from : to : 
                    null;
            int x;
            int Amt = 
                Arguments.Count > 1 && int.TryParse(Arguments[1], out x) ?
                    int.Parse(Arguments[1]) : 0;

            switch (Effect)
            {

                // ~ ~ ~ STANCE ~ ~ ~

                case "SetStance":
                    Target.Stance = 1;
                    break;

                case "HitStance":
                    Target.HitStance(Amt);
                    break;

                case "ResetStance":
                    Target.Stance = 0;
                    break;

                case "TradeStance":
                    int FromsStance = from.Stance;
                    from.Stance = to.Stance;
                    to.Stance = FromsStance;
                    break;

                // ~ ~ ~ HP ~ ~ ~

                case "HitHP":
                    to.HitHP(Amt);
                    break;

                case "GiveEnoughHP":
                    int GiveAmt = to.MaxHP - to.HP;
                    from.HitHP(-GiveAmt);
                    to.HitHP(+GiveAmt);
                    break;

                case "TradeHP":
                    int FromsHP = from.HP;
                    from.HP = to.HP;
                    to.HP = FromsHP;
                    break;

                case "HitHPByStance":
                    int HitAmt = from.StatAmt("Attack") + to.StatAmt("Defense");
                    to.HitHP(-HitAmt);
                    break;

                case "StealHP":
                    from.HitHP(Amt);
                    to.HitHP(-Amt);
                    break;

                // ~ ~ ~ STATUS ~ ~ ~

                case "Kill":
                    Target.HitHP(-Target.HP);
                    break;
                
                case "KillChance":
                    if (new Random().Next(100) < Amt)
                        Target.HitHP(-Target.HP);
                    break;

                case "AddStatus":
                    Target.AddStatusEffect(Arguments[1].Trim());
                    break;

                case "RemoveStatus":
                    Target.RemoveStatusEffect(Arguments[1].Trim());
                    break;

                // ~ ~ ~ MISC ~ ~ ~ 

                case "StealCoin":
                    GameContent.Variables["Money"] -= 1;
                    break;

                case "StealFood":
                    GameContent.Variables["FoodSupply"] -= 1;
                    break;

                default:
                    throw new Exception("Effect Keyword " + Effect + " not recognized.");
            }
        }

        public static bool CheckCondition(string condition, Battle state, Targeting targeting)
        {
            string Condition = condition.Remove(condition.IndexOf('('));
            string ParamsWithParen = condition.Remove(0, Condition.Length);
            string ParamsNoFirstParen = ParamsWithParen.Remove(0, 1);
            string ParamsNoLastParen = ParamsNoFirstParen.Remove(ParamsWithParen.Length - 2, 1);
            List<string> Arguments = new List<string>(ParamsNoLastParen.Split(','));

            Member From = state.From(targeting);
            Member To = state.To(targeting);
            Member Target =
                Arguments.Count > 0 ?
                    Arguments[0] == "Caster" ? From : To :
                    null;

            switch (Condition)
            {
                case "HP":
                    int HPAmt = int.Parse(Arguments[2]);
                    switch (Arguments[1].Trim())
                    {
                        case "<": return Target.HP < HPAmt; 
                        case ">": return Target.HP > HPAmt; 
                        case "<=": return Target.HP <= HPAmt; 
                        case ">=": return Target.HP >= HPAmt; 
                        case "==": return Target.HP == HPAmt; 
                        case "!=": return Target.HP != HPAmt;
                        default: throw new Exception("Sign " + Arguments[1] + " is not valid.");
                    }

                case "Commitment":
                    int StanceAmt = int.Parse(Arguments[2]);
                    switch (Arguments[1].Trim())
                    {
                        case "<": return Target.Stance < StanceAmt;
                        case ">": return Target.Stance > StanceAmt;
                        case "<=": return Target.Stance <= StanceAmt;
                        case ">=": return Target.Stance >= StanceAmt;
                        case "==": return Target.Stance == StanceAmt;
                        case "!=": return Target.Stance != StanceAmt;
                        default: throw new Exception("Sign " + Arguments[1] + " is not valid.");
                    }

                case "Is":
                    switch (Arguments[1].Trim())
                    {
                        case "Self":
                            return targeting.FromPartyIdx == targeting.ToPartyIdx &&
                                targeting.FromMemberIdx == targeting.ToMemberIdx;
                        case "Other":
                            return targeting.FromPartyIdx != targeting.ToPartyIdx ||
                                targeting.FromMemberIdx != targeting.ToMemberIdx;
                        case "InSameParty":
                            return targeting.FromPartyIdx == targeting.ToPartyIdx;
                        case "InDifferentParty":
                            return targeting.FromPartyIdx != targeting.ToPartyIdx;
                        default: throw new Exception("Condition " + Arguments[1] + " is not valid.");
                    }

                case "Status":
                    switch (Arguments[1].Trim())
                    {
                        case "Has": return Target.HasEffect(Arguments[2].Trim());
                        case "Doesn'tHave": return !Target.HasEffect(Arguments[2].Trim());
                        default: throw new Exception("Keyword " + Arguments[1].Trim() + " is not valid.");
                    }

                default:
                    throw new Exception("Keyword " + Condition + " is not valid.");
            }
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
