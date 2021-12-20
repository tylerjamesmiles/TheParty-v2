using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class AnimateMiss : State<CommandBattle>
    {
        LerpV MissLerp;
        LerpF MissTransparency;

        public override void Enter(CommandBattle client)
        {
            Vector2 MissStart = client.ToSprite.DrawPos + new Vector2(-8, 0);
            Vector2 MissEnd = MissStart + new Vector2(0, -32);
            MissLerp = new LerpV(MissStart, MissEnd, 0.5f);
            MissTransparency = new LerpF(1f, 0f, 0.5f);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            MissLerp.Update(deltaTime);
            MissTransparency.Update(deltaTime);

            if (MissLerp.Reached && MissTransparency.Reached)
            {
                client.FromMember.GoneThisTurn = true;
                client.StateMachine.SetNewCurrentState(client, new DoMoveBack());

            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Rectangle DrawRect = new Rectangle(MissLerp.CurrentPosition.ToPoint(), new Point(21, 10));
            spriteBatch.Draw(GameContent.Sprites["Miss"], DrawRect, Color.White);
        }

        public override void Exit(CommandBattle client)
        {

        }
    }
}
