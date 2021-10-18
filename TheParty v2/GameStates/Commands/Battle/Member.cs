using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    class Member
    {
        public int HP; // 0 - 10
        public int Stance;
        public bool Charged;
        public bool KOd;
        public int Hunger;
        public List<Move> Moves;
        public List<StatusEffect> StatusEffects;

        private static readonly int StanceLimit = 5;

        public Member(int hp, int stance, int hunger, bool charged, bool kod, List<Move> moves, List<StatusEffect> effects)
        {
            HP = hp;
            Stance = stance;
            Hunger = hunger;
            Charged = charged;
            KOd = kod;
            Moves = moves;
            StatusEffects = new List<StatusEffect>(effects);
        }
        public Member DeepCopy() => new Member(HP, Stance, Hunger, Charged, KOd, Moves, StatusEffects);

        public Member(string memberName, JsonDocument doc)
        {
            JsonElement Mem = doc.RootElement.GetProperty(memberName);
            HP = Mem.GetProperty("HP").GetInt32();
            Stance = Mem.GetProperty("Stance").GetInt32();
            Charged = Mem.GetProperty("Charged").GetBoolean();
            Hunger = 7;
            KOd = false;

            int NumMoves = Mem.GetProperty("Moves").GetArrayLength();
            Moves = new List<Move>();
            for (int i = 0; i < NumMoves; i++)
                Moves.Add(
                    (Move)Utility.CallMethod(
                        typeof(Move), 
                        (Mem.GetProperty("Moves")[i].GetString())));

            StatusEffects = new List<StatusEffect>();
        }

        // ~ ~ ~ ~ GETTERS ~ ~ ~ ~

        public bool CanGo => HP > 0 && Stance > 0;
        public List<string> MoveNames => Moves.ConvertAll(m => m.Name);
        public bool HasEffect(string name) => StatusEffects.Exists(s => s.Name == name);
        public List<MemberMove> AllValidMoves(int partyIdx, int memberIdx, Battle state)
        {
            List<MemberMove> Result = new List<MemberMove>();
            foreach (Move move in Moves)
                foreach (Targeting targeting in state.AllTargetingFor(partyIdx, memberIdx))
                    if (move.ValidOnMember(state, targeting))
                        Result.Add(new MemberMove(targeting, move));
            return Result;
        }


        // ~ ~ ~ ~ SETTERS ~ ~ ~ ~
        public void HitStance(int by)
        {
            Stance += by;
            if (Stance >= StanceLimit)
                Stance -= StanceLimit;
            KOd = Stance == 0;
        }

        public void HitHP(int by)
        {
            HP += by;
            if (HP < 0)
                HP = 0;
        }

        public void AddEffect(string effectName)
        {
            StatusEffect Effect = (StatusEffect)Utility.CallMethod(typeof(StatusEffect), effectName);
            StatusEffects.Add(Effect);
        }

        public void RemoveEffect(string effectName)
        {
            StatusEffect Effect = StatusEffects.Find(s => s.Name == effectName);
            StatusEffects.Remove(Effect);
        }

    }
}
