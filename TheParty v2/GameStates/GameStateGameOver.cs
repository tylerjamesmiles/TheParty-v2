using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class GameStateGameOver : State<TheParty>
    {
        public override void Enter(TheParty client)
        {
        }

        public override void Update(TheParty client, float deltaTime)
        {
            if (InputManager.JustReleased(Keys.Space))
                client.StateMachine.SetNewCurrentState(client, new GameStateTitle());
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameContent.Sprites["GameOver"],
                new Rectangle(new Point(0, 0), new Point(160, 144)),
                Color.White);
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
