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
        public int Hunger { get; set; }
        public int MaxHunger { get; set; }
        public List<string> Moves { get; private set; }
        public List<string> MovesToLearn { get; private set; }
        public string EquippedName { get; private set; }
        public List<StatusEffect> StatusEffects { get; set; }
        public string SpriteName { get; private set; }
        public bool GoneThisTurn { get; set; }

        private static readonly int StanceLimit = 10;


        public Member(string name, int hp, int maxHP, int stance, int hunger, int maxHunger, List<string> moves, List<string> movesToLearn, string equipped, List<StatusEffect> effects, string spriteName)
        {
            Name = name;
            HP = hp;
            MaxHP = maxHP;
            Stance = stance;
            Hunger = hunger;
            MaxHunger = maxHunger;
            Moves = moves;
            MovesToLearn = movesToLearn;
            EquippedName = equipped;
            StatusEffects = new List<StatusEffect>(effects);
            SpriteName = spriteName;
        }

        [JsonConstructor]
        public Member(string name, int hp, int maxHP, int stance, int hunger, int maxHunger, List<string> moves, List<string> movesToLearn, string equipped, string spriteName)
        {
            Name = name;
            HP = hp;
            MaxHP = maxHP;
            Stance = stance;
            Hunger = hunger;
            MaxHunger = maxHunger;
            Moves = moves;
            MovesToLearn = movesToLearn;
            EquippedName = equipped;
            StatusEffects = new List<StatusEffect>();
            SpriteName = spriteName;
        }

        public Member DeepCopy() => new Member(Name, HP, MaxHP, Stance, Hunger, MaxHunger, Moves, MovesToLearn, EquippedName, StatusEffects, SpriteName);

        public List<Move> GetMoves() =>
            Moves.ConvertAll(m => GameContent.Moves[m]);

        public List<Move> GetMovesToLearn() =>
            MovesToLearn.ConvertAll(m => GameContent.Moves[m]);

        public Member(JsonElement Mem)
        {
            Name = Mem.GetProperty("Name").GetString();
            HP = Mem.GetProperty("HP").GetInt32();
            MaxHP = Mem.GetProperty("MaxHP").GetInt32();
            Stance = 1;
            Hunger = Mem.GetProperty("Hunger").GetInt32();
            MaxHunger = Mem.GetProperty("MaxHunger").GetInt32();
            EquippedName = Mem.GetProperty("Equipped").GetString();

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
        public Equipment Equipped => GameContent.Equipment[EquippedName];

        public int StanceWAttackBonus => Equipped.StatWithBonus("Attack", Stance);
        public int StanceWDefenseBonus => Equipped.StatWithBonus("Defense", Stance);

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
            StatusEffect Effect = GameContent.StatusEffects[effectName].DeepCopy();
            StatusEffects.Add(Effect);
        }

        public void RemoveStatusEffect(string effectName)
        {
            StatusEffect Effect = StatusEffects.Find(s => s.Name == effectName);
            StatusEffects.Remove(Effect);
        }

    }
}
