using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    class Member
    {
        public int HP; // 0 - 10
        public int Stance; // 0 - 4
        public bool Charged;
        public bool KOd;
        public List<Move> Moves;
        public List<StatusEffect> StatusEffects;

        private static readonly int StanceLimit = 5;

        public Member(int hp, int stance, bool charged, bool kod, List<Move> moves, List<StatusEffect> effects)
        {
            HP = hp;
            Stance = stance;
            Charged = charged;
            KOd = kod;
            Moves = moves;
            StatusEffects = effects;
        }
        public Member DeepCopy() => new Member(HP, Stance, Charged, KOd, Moves, StatusEffects);

        public Member(string memberName, JsonDocument doc)
        {
            JsonElement Mem = doc.RootElement.GetProperty(memberName);
            HP = Mem.GetProperty("HP").GetInt32();
            Stance = Mem.GetProperty("Stance").GetInt32();
            Charged = Mem.GetProperty("Charged").GetBoolean();
            KOd = false;

            Func<string, Move> StringToMove = (s) =>
                s == "Hit" ? Move.Hit :
                s == "Hurt" ? Move.Hurt :
                s == "Charge" ? Move.Charge :
                s == "Help" ? Move.Help :
                s == "Item" ? Move.Item :
                s == "Give" ? Move.Give :
                s == "Talk" ? Move.Talk :
                Move.Hit;

            int NumMoves = Mem.GetProperty("Moves").GetArrayLength();
            Moves = new List<Move>();
            for (int i = 0; i < NumMoves; i++)
                Moves.Add(StringToMove(Mem.GetProperty("Moves")[i].GetString()));

            StatusEffects = new List<StatusEffect>();
        }

        // ~ ~ ~ ~ GETTERS ~ ~ ~ ~

        public bool CanGo => HP > 0 && Stance > 0;
        public List<string> MoveNames => Moves.ConvertAll(m => m.Name);
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
            Stance = MathUtility.RolledIfAtLimit(Stance + by, StanceLimit);
            if (Stance == 0)
                KOd = true;
        }

        public void HitHP(int by)
        {
            HP += by;
            if (HP < 0)
                HP = 0;
        }


    }
}
