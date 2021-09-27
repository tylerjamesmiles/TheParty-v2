using System;
using System.Collections.Generic;
using System.Text;

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
