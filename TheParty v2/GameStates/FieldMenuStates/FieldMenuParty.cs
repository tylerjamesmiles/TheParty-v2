using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuParty : State<GameStateFieldMenu>
    {
        List<AnimatedSprite2D> Sprites;

        public FieldMenuParty()
        {
        }

        public override void Enter(GameStateFieldMenu client)
        {
            Sprites = new List<AnimatedSprite2D>();
            for (int i = 0; i < client.BackupMembers.Count; i++)
            {
                string SpriteName = client.BackupMembers[i].SpriteName;
                Point FrameSize = new Point(32, 32);
                Vector2 DrawPos = new Vector2(i * 32, 96);
                Vector2 Offset = new Vector2();
                AnimatedSprite2D Sprite = new AnimatedSprite2D(SpriteName, FrameSize, DrawPos, Offset);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("Selected", 0, 1, 0.15f);
                Sprite.SetCurrentAnimation("Idle");
                Sprites.Add(Sprite);
            }
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Sprites.ForEach(s => s.Update(deltaTime));

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new FieldMenuMain());
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Sprites.ForEach(s => s.Draw(spriteBatch));
        }

        public override void Exit(GameStateFieldMenu client)
        {
        }
    }
}
