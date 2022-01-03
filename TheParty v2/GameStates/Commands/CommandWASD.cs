using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandWASD : Command<TheParty>
    {
        LerpF FadeLerp;
        enum State { FadeIn, Stay, FadeOut };
        State CurrentState;

        public override void Enter(TheParty client)
        {
            FadeLerp = new LerpF(0f, 1f, 0.2f);
            CurrentState = State.FadeIn;
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if ((InputManager.Pressed(Keys.W) ||
                InputManager.Pressed(Keys.A) ||
                InputManager.Pressed(Keys.S) ||
                InputManager.Pressed(Keys.D)) &&
                CurrentState != State.FadeOut)
            {
                CurrentState = State.FadeOut;
                FadeLerp = new LerpF(FadeLerp.CurrentPosition, 0f, 0.1f);
            }

            switch (CurrentState)
            {
                case State.FadeIn:
                    FadeLerp.Update(deltaTime);
                    if (FadeLerp.Reached)
                        CurrentState = State.Stay;
                    break;

                case State.FadeOut:
                    FadeLerp.Update(deltaTime);
                    if (FadeLerp.Reached)
                        Done = true;
                    break;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            if (!Entered)
                return;

            float cp = FadeLerp.CurrentPosition;
            Color FadeColor = new Color(cp, cp, cp, FadeLerp.CurrentPosition);
            Rectangle DrawRect = new Rectangle(
                client.Player.Transform.Position.ToPoint() - client.Camera.Position.ToPoint() + new Point(-16, -48),
                new Point(32, 32));
            Rectangle SourceRect = new Rectangle(new Point(0, 0), new Point(32, 32));

            spriteBatch.Draw(
                GameContent.Sprites["WASD"],
                DrawRect,
                SourceRect,
                FadeColor);
        }
    }

    class CommandESC : Command<TheParty>
    {
        LerpF FadeLerp;
        enum State { FadeIn, Stay, FadeOut };
        State CurrentState;

        public override void Enter(TheParty client)
        {
            FadeLerp = new LerpF(0f, 1f, 0.2f);
            CurrentState = State.FadeIn;
            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if ((InputManager.Pressed(Keys.Escape)) &&
                CurrentState != State.FadeOut)
            {
                Done = true;
            }

            switch (CurrentState)
            {
                case State.FadeIn:
                    FadeLerp.Update(deltaTime);
                    if (FadeLerp.Reached)
                        CurrentState = State.Stay;
                    break;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            if (!Entered)
                return;

            float cp = FadeLerp.CurrentPosition;
            Color FadeColor = new Color(cp, cp, cp, FadeLerp.CurrentPosition);
            Rectangle DrawRect = new Rectangle(
                client.Player.Transform.Position.ToPoint() - client.Camera.Position.ToPoint() + new Point(-8, -48),
                new Point(16, 16));
            Rectangle SourceRect = new Rectangle(new Point(0, 0), new Point(16, 16));

            spriteBatch.Draw(
                GameContent.Sprites["ESC"],
                DrawRect,
                SourceRect,
                FadeColor);
        }
    }
}
