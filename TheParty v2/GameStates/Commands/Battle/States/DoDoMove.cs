﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoDoMove : State<CommandBattle>
    {
        Timer TempWait;
        bool MoveDone;
        AnimatedSprite2D Sheet;

        public override void Enter(CommandBattle client)
        {
            client.FromSprite.SetCurrentAnimation("Idle");

            if (client.CurrentMove.PositiveEffect)
                client.TargetSprite.SetCurrentAnimation("PositiveHit");
            else
                client.TargetSprite.SetCurrentAnimation("NegativeHit");

            TempWait = new Timer(0.8f);
            MoveDone = false;

            Sheet = GameContent.AnimationSheets[client.CurrentMove.AnimationSheet];
            Sheet.SetCurrentAnimation(client.CurrentMove.AnimationName);
            Sheet.DrawPos = client.TargetSprite.DrawPos;

        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            if (!MoveDone)
            {
                client.CurrentStore.DoMove(client.CurrentMove, client.CurrentTargeting);
                client.CurrentStore.TimePass();
                
                MoveDone = true;
            }

            Sheet.Update(deltaTime);
            
            TempWait.Update(deltaTime);
            if (TempWait.TicThisFrame)
            {
                client.SetAppropriateAnimations();
                client.StateMachine.SetNewCurrentState(client, new DoMoveBack());
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Sheet.Draw(spriteBatch);
        }
    }
}