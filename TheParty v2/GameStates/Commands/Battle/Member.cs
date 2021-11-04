using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    class Member
    {
        public string Name;
        public int HP; // 0 - 10
        public int MaxHP;
        public int Stance;
        public bool Charged;
        public bool KOd;
        public int Hunger;
        public int MaxHunger;
        public List<Move> Moves;
        public List<Move> MovesToLearn;
        public List<StatusEffect> StatusEffects;
        public string SpriteName;

        private static readonly int StanceLimit = 5;

        public Member(string name, int hp, int maxHP, int stance, int hunger, int maxHunger, bool charged, bool kod, List<Move> moves, List<Move> movesToLearn, List<StatusEffect> effects, string spriteName)
        {
            Name = name;
            HP = hp;
            MaxHP = maxHP;
            Stance = stance;
            Hunger = hunger;
            MaxHunger = maxHunger;
            Charged = charged;
            KOd = kod;
            Moves = moves;
            MovesToLearn = movesToLearn;
            StatusEffects = new List<StatusEffect>(effects);
            SpriteName = spriteName;
        }
        public Member DeepCopy() => new Member(Name, HP, MaxHP, Stance, Hunger, MaxHunger, Charged, KOd, Moves, MovesToLearn, StatusEffects, SpriteName);

        public Member(string memberName, JsonDocument doc)
        {
            JsonElement Mem = doc.RootElement.GetProperty(memberName);
            Name = Mem.GetProperty("Name").GetString();
            HP = Mem.GetProperty("HP").GetInt32();
            MaxHP = 10;
            Stance = Mem.GetProperty("Stance").GetInt32();
            Charged = Mem.GetProperty("Charged").GetBoolean();
            Hunger = 7;
            MaxHunger = 10;
            KOd = false;

            int NumMoves = Mem.GetProperty("Moves").GetArrayLength();
            Moves = new List<Move>();
            for (int i = 0; i < NumMoves; i++)
                Moves.Add((Move)Utility.CallMethod(typeof(Move), Mem.GetProperty("Moves")[i].GetString()));

            int NumMovesToLearn = Mem.GetProperty("MovesToLearn").GetArrayLength();
            MovesToLearn = new List<Move>();
            for (int i = 0; i < NumMovesToLearn; i++)
                MovesToLearn.Add((Move)Utility.CallMethod(typeof(Move), Mem.GetProperty("MovesToLearn")[i].GetString()));

            StatusEffects = new List<StatusEffect>();

            SpriteName = Mem.GetProperty("SpriteName").GetString();
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
                Stance = 0;
            KOd = Stance == 0;
        }

        public void HitHP(int by)
        {
            HP += by;
            if (HP < 0)
                HP = 0;
            if (HP > MaxHP)
                HP = MaxHP;
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
