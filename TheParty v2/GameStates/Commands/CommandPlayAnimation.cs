using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandPlayAnimation : Command<TheParty>
    {
        OgmoEntity Entity;
        AnimatedSprite2D AnimatedSprite;
        string Animation;

        Timer Timer;

        public CommandPlayAnimation(OgmoEntity entity, string sheet, string animation)
        {
            Entity = entity;
            AnimatedSprite = GameContent.AnimationSheets[sheet];
            Animation = animation;

            float Time = AnimatedSprite.Animations[Animation].AmtOfTime;
            Timer = new Timer(Time);
        }

        public override void Enter(TheParty client)
        {
            AnimatedSprite.SetCurrentAnimation(Animation);
            AnimatedSprite.DrawPos = client.Player.Transform.Position;

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            AnimatedSprite.DrawPos = client.Player.Transform.Position;

            AnimatedSprite.Update(deltaTime);
            Timer.Update(deltaTime);

            if (Timer.TicThisFrame)
                Done = true;
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            AnimatedSprite.Draw(spriteBatch, client.Camera.Position);
        }
    }
}
