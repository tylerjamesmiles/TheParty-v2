using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoDoMove : State<CommandBattle>
    {
        bool MoveDone;
        AnimatedSprite2D Sheet;

        public override void Enter(CommandBattle client)
        {
            client.FromSprite.SetCurrentAnimation("Idle");

            //if (client.CurrentMove.PositiveEffect)
            //    client.TargetSprite.SetCurrentAnimation("PositiveHit");
            //else
            //    client.TargetSprite.SetCurrentAnimation("NegativeHit");

            MoveDone = false;

            Sheet = GameContent.AnimationSheets["HitAnimations"];
            if (Sheet.Animations.ContainsKey(client.CurrentMove.Name))
            {
                Sheet.SetCurrentAnimation(client.CurrentMove.Name);
                Sheet.CurrentAnimation.NumTimesLooped = 0;
                Sheet.DrawPos = client.TargetSprite.DrawPos;
            }
            else
                Sheet = null;
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            if (!MoveDone)
            {
                client.CurrentStore.DoMove(client.CurrentMove, client.CurrentTargeting);
                client.FromMember.GoneThisTurn = true;
                MoveDone = true;
            }

            if (Sheet != null)
                Sheet.Update(deltaTime);

            if ((Sheet != null && Sheet.CurrentAnimation.NumTimesLooped > 0) || Sheet == null)
            {
                //client.SetAppropriateAnimations();
                client.StateMachine.SetNewCurrentState(client, new DoMoveBack());
            }

        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            if (Sheet != null)
                Sheet.Draw(spriteBatch);
        }
    }
}
