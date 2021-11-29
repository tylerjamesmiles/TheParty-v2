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
        public bool KOd { get; set; }
        public int Hunger { get; set; }
        public int MaxHunger { get; set; }
        public List<string> Moves { get; private set; }
        public List<string> MovesToLearn { get; private set; }
        public List<StatusEffect> StatusEffects { get; set; }
        public string SpriteName { get; private set; }

        public bool GoneThisTurn { get; set; }

        private static readonly int StanceLimit = 10;

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
            Moves.ConvertAll(m => GameContent.Moves[m]);

        public List<Move> GetMovesToLearn() =>
            MovesToLearn.ConvertAll(m => GameContent.Moves[m]);

        public Member(JsonElement Mem)
        {
            Name = Mem.GetProperty("Name").GetString();
            HP = Mem.GetProperty("HP").GetInt32();
            MaxHP = Mem.GetProperty("MaxHP").GetInt32();
            Stance = Mem.GetProperty("Stance").GetInt32();
            Charged = Mem.GetProperty("Charged").GetBoolean();
            Hunger = Mem.GetProperty("Hunger").GetInt32();
            MaxHunger = Mem.GetProperty("MaxHunger").GetInt32();
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
            !GoneThisTurn &&
            !HasEffect("Stunned");

        public bool HasEffect(string name) => StatusEffects.Exists(s => s.Name == name);
        public List<MemberMove> AllValidMoves(int partyIdx, int memberIdx, Battle state)
        {
            List<MemberMove> Result = new List<MemberMove>();
            List<Move> Moves = GetMoves();
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
            if (Stance > StanceLimit)
                Stance = StanceLimit;
            if (Stance < 0)
                Stance = 0;
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
