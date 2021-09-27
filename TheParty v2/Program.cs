using System;
using System.Collections.Generic;

namespace TheParty_v2
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TheParty())
                game.Run();
        }
    }
}
