using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    class Member
    {
        public string Name { get; private set; }
        public int HP { get; set; } // 0 - 10
        public int MaxHP { get; set; }
        public int Stance { get; set; }
        public bool Charged { get; set; }
        public bool KOd { get; private set; }
        public int Hunger { get; set; }
        public int MaxHunger { get; private set; }
        public List<string> Moves { get; private set; }
        public List<string> MovesToLearn { get; private set; }
        public List<StatusEffect> StatusEffects { get; private set; }
        public string SpriteName { get; private set; }

        private static readonly int StanceLimit = 5;

        public Member(string name, int hp, int maxHP, int stance, int hunger, int maxHunger, bool charged, bool kod, List<string> moves, List<string> movesToLearn, List<StatusEffect> effects, string spriteName)
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

        [JsonConstructor]
        public Member(string name, int hp, int maxHP, int stance, int hunger, int maxHunger, bool charged, bool kod, List<string> moves, List<string> movesToLearn, string spriteName)
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
            StatusEffects = new List<StatusEffect>();
            SpriteName = spriteName;
        }

        public Member DeepCopy() => new Member(Name, HP, MaxHP, Stance, Hunger, MaxHunger, Charged, KOd, Moves, MovesToLearn, StatusEffects, SpriteName);

        public List<Move> GetMoves() =>
            Moves.ConvertAll(m => (Move)Utility.CallMethod(typeof(Move), m));

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
            Moves = new List<string>();
            for (int i = 0; i < NumMoves; i++)
                Moves.Add(Mem.GetProperty("Moves")[i].GetString());

            int NumMovesToLearn = Mem.GetProperty("MovesToLearn").GetArrayLength();
            MovesToLearn = new List<string>();
            for (int i = 0; i < NumMovesToLearn; i++)
                MovesToLearn.Add(Mem.GetProperty("MovesToLearn")[i].GetString());

            StatusEffects = new List<StatusEffect>();

            SpriteName = Mem.GetProperty("SpriteName").GetString();
        }

        // ~ ~ ~ ~ GETTERS ~ ~ ~ ~

        public bool CanGo => 
            HP > 0 && 
            Stance > 0 &&
            !HasEffect("Stunned");

        public List<string> MoveNames => Moves;
        public bool HasEffect(string name) => StatusEffects.Exists(s => s.Name == name);
        public List<MemberMove> AllValidMoves(int partyIdx, int memberIdx, Battle state)
        {
            List<MemberMove> Result = new List<MemberMove>();
            foreach (Move move in GetMoves())
                foreach (Targeting targeting in state.AllTargetingFor(partyIdx, memberIdx))
                    if (move.ValidOnMember(state, targeting))
                        Result.Add(new MemberMove(targeting, move));
            return Result;
        }


        // ~ ~ ~ ~ SETTERS ~ ~ ~ ~
        public void HitStance(int by)
        {
            // charge
            if (Stance == 1 && by == -1)
            {
                Stance = 1;
                return;
            }

            Stance += by;
            if (Stance >= StanceLimit)
                Stance = 0;
            KOd = Stance == 0;

            if (KOd)
                StatusEffects.RemoveAll(s => s.Name == "Stunned");
        }

        public void HitHP(int by)
        {
            HP += by;
            if (HP < 0)
                HP = 0;
            if (HP > MaxHP)
                HP = MaxHP;
        }

        public void AddStatusEffect(string effectName)
        {
            StatusEffect Effect = (StatusEffect)Utility.CallMethod(typeof(StatusEffect), effectName);
            StatusEffects.Add(Effect);
        }

        public void RemoveStatusEffect(string effectName)
        {
            StatusEffect Effect = StatusEffects.Find(s => s.Name == effectName);
            StatusEffects.Remove(Effect);
        }

    }
}
