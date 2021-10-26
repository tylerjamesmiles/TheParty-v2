﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TheParty_v2
{
    class ChooseMember : State<CommandBattle>
    {
        List<int> MemberIdxs;

        public override void Enter(CommandBattle client)
        {
            int CurrentTurn = client.CurrentStore.CurrentTurnPartyIdx;

            List<Vector2> MemberPositions = client.PartySprites(CurrentTurn).ConvertAll(s => s.DrawPos + new Vector2(-3, 0));

            List<Vector2> LegalMemberPositions = new List<Vector2>();
            List<Member> MembersInThisParty = client.CurrentStore.MembersOfParty(CurrentTurn);
            MemberIdxs = new List<int>();
            for (int i = 0; i < MembersInThisParty.Count; i++)
            { 
                List<MemberMove> ValidMoves = MembersInThisParty[i].AllValidMoves(CurrentTurn, i, client.CurrentStore);
                if (ValidMoves.Count > 0)
                {
                    MemberIdxs.Add(i);
                    LegalMemberPositions.Add(MemberPositions[i]);
                }
            }

            client.MemberChoice = new GUIChoice(LegalMemberPositions.ToArray());
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MemberChoice.Update(deltaTime, true);
            client.CurrentTargeting = new Targeting()
            {
                FromPartyIdx = client.CurrentStore.CurrentTurnPartyIdx,
                FromMemberIdx = MemberIdxs[client.MemberChoice.CurrentChoiceIdx]
            };

            client.SetAppropriateAnimations();
            if (!client.FromMember.Charged)
                client.FromSprite.SetCurrentAnimation("Move");

            if (client.MemberChoice.Done)
                client.StateMachine.SetNewCurrentState(client, new ChooseMove());

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new FightOrFlee());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.MemberChoice.Draw(spriteBatch, true);
        }
    }
}