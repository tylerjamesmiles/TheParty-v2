using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    static class InputManager
    {
        static KeyboardState OldKeystate;
        static KeyboardState NewKeyState;

        public static void SetNewState() => NewKeyState = Keyboard.GetState();
        public static void SetOldState() => OldKeystate = NewKeyState;

        public static bool Pressed(Keys key)
            => NewKeyState.IsKeyDown(key);
        public static bool Released(Keys key)
            => NewKeyState.IsKeyUp(key);
        public static bool JustPressed(Keys key)
            => OldKeystate.IsKeyUp(key) && NewKeyState.IsKeyDown(key);
        public static bool JustReleased(Keys key)
            => OldKeystate.IsKeyDown(key) && NewKeyState.IsKeyUp(key);

        public static Vector2 WASDDirectionPressed()
        {
            Vector2 DirPressed = Vector2.Zero;
            if (Pressed(Keys.W)) DirPressed += new Vector2(0, -1);
            if (Pressed(Keys.A)) DirPressed += new Vector2(-1, 0);
            if (Pressed(Keys.S)) DirPressed += new Vector2(0, +1);
            if (Pressed(Keys.D)) DirPressed += new Vector2(+1, 0);
            return DirPressed;
        }
    }
}
