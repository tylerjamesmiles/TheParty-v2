using Microsoft.Xna.Framework;
using System;

namespace TheParty_v2
{
    static class MathUtility
    {
        public static int RolledIfAtLimit(int start, int limit) =>
            start >= limit ? start - limit : start;

        public static Vector2 Capped(Vector2 vec, float lengthLimit) =>
            vec.LengthSquared() > lengthLimit * lengthLimit ?
                Vector2.Normalize(vec) * lengthLimit : vec;

        public static bool LengthComparedTo(Vector2 vec, Func<Vector2, float, bool> op, float length) =>
           op(vec, length);

        public static Vector2 KeptInRect(Vector2 vec, Rectangle rect)
        {
            Vector2 Result = vec;
            if (Result.X < rect.Left) Result.X = rect.Left;
            if (Result.Y < rect.Top) Result.Y = rect.Top;
            if (Result.X > rect.Right) Result.X = rect.Right;
            if (Result.Y > rect.Bottom) Result.Y = rect.Bottom;
            return Result;
        }

        public static float Clamped(float num, float min, float max) => 
            (num < min) ? min :
            (num > max) ? max :
            num;

        public static int GeneralDirection(Vector2 vec)
        {
            return MathF.Abs(vec.Y) > MathF.Abs(vec.X) ?
                vec.Y < 0 ? 0 : 1 : // up, down
                vec.X < 0 ? 2 : 3;  // left, right
        }
    }
}
