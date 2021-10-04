using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TheParty_v2
{
    struct Member
    {
        public int HP; // 0 - 10
        public int Stance; // 0 - 4
        public bool Charged;
        //public int KOdFor; // Down by 1 each turn
        public bool KOd;
        public Move[] Moves;

        //public bool HasGoneThisTurn;

        private static readonly int StanceLimit = 5;

        public Member(string memberName, JsonDocument doc)
        {
            JsonElement Mem = doc.RootElement.GetProperty(memberName);
            HP = Mem.GetProperty("HP").GetInt32();
            Stance = Mem.GetProperty("Stance").GetInt32();
            Charged = Mem.GetProperty("Charged").GetBoolean();
            //KOdFor = Mem.GetProperty("KOdFor").GetInt32();
            KOd = false;

            //HasGoneThisTurn = false;

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
            Moves = new Move[NumMoves];
            for (int i = 0; i < NumMoves; i++)
            {
                Moves[i] = StringToMove(Mem.GetProperty("Moves")[i].GetString());
            }
        }

        public static Member DeepCopyOf(Member m) =>
            new Member() 
            { 
                HP = m.HP, 
                Stance = m.Stance, 
                Charged = m.Charged, 
                //KOdFor = m.KOdFor,
                KOd = m.KOd,
                Moves = m.Moves,

                //HasGoneThisTurn = m.HasGoneThisTurn
            };

        public static int StanceAfterHit(int oldStance, int hitBy) =>
            MathUtility.RolledIfAtLimit(oldStance + hitBy, StanceLimit);

        public string[] MoveNames()
        {
            List<string> Result = new List<string>();
            foreach (Move move in Moves)
                Result.Add(move.Name);
            return Result.ToArray();
        }

        public bool CanGo()
        {
            return HP > 0 && Stance > 0;
        }
    }
}
