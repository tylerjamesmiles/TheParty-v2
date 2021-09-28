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
        public int KOdFor; // Down by 1 each turn
        public Move[] Moves;

        public bool HasGoneThisTurn;

        private static readonly int StanceLimit = 5;

        public Member(string memberName, JsonDocument doc)
        {
            JsonElement Mem = doc.RootElement.GetProperty(memberName);
            HP = Mem.GetProperty("HP").GetInt32();
            Stance = Mem.GetProperty("Stance").GetInt32();
            Charged = Mem.GetProperty("Charged").GetBoolean();
            KOdFor = Mem.GetProperty("KOdFor").GetInt32();

            HasGoneThisTurn = false;
            Moves = new Move[] { Move.Hit, Move.Hurt, Move.Charge };
        }

        public static Member DeepCopyOf(Member m) =>
            new Member() 
            { 
                HP = m.HP, 
                Stance = m.Stance, 
                Charged = m.Charged, 
                KOdFor = m.KOdFor,
                Moves = m.Moves,

                HasGoneThisTurn = m.HasGoneThisTurn
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

        public static string StringRepresentation(Member member)
        {
            string Result = "";

            if (member.HasGoneThisTurn)
                Result += ".";

            if (member.Charged)
                Result += "*";

            for (int i = 0; i < member.KOdFor; i++)
                Result += '>';

            // stance
            if (member.KOdFor <= 0) 
                Result += "[" + member.Stance + "]";

            // hp
            Result += "(" + member.HP + ")";

            for (int i = 0; i < member.KOdFor; i++)
                Result += '<';

            if (member.Charged)
                Result += "*";

            return Result;
        }
    }
}
