using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class PreMoveChecks : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            // Are there any moves for current party?
            Party CurrentTurnPty = BattleStore.CurrentTurnPartyOf(client.CurrentStore);
            int CurrentTurnIdx = client.CurrentStore.CurrentTurnPartyIdx;
            MemberMove[] PossibleMoves = Party.AllPossibleMemberMoves(CurrentTurnPty, CurrentTurnIdx, client.CurrentStore);
            if (PossibleMoves.Length == 0)
            {
                client.CurrentStore = BattleStore.TimePassed(client.CurrentStore);
            }

            // Current Turn is player
            if (client.CurrentStore.CurrentTurnPartyIdx == 0)
            {
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
            }
            // Current Turn is AI
            else
            {
                int Idx = client.CurrentStore.CurrentTurnPartyIdx;
                MemberMove BestTurn = Party.BestTurn(CurrentTurnPty, Idx, client.CurrentStore);

                client.CurrentMove = BestTurn.Move;
                client.CurrentTargeting = BestTurn.Targeting;
                client.StateMachine.SetNewCurrentState(client, new DoMoveForward());
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
