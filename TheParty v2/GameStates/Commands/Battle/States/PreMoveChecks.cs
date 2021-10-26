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
            // Is player dead?
            Party Player = client.CurrentStore.Parties[0];
            if (Player.IsDead)
                client.GameOver = true;

            // Are the other parties dead?
            bool OtherPartiesDead = true;
            for (int i = 1; i < client.CurrentStore.NumParties; i++)
                if (client.CurrentStore.Parties[i].IsDead == false)
                    OtherPartiesDead = false;
            if (OtherPartiesDead)
            {
                client.StateMachine.SetNewCurrentState(client, new Victory());
                return;
            }

            // Are there any moves for current party?
            Party CurrentTurnPty = client.CurrentStore.CurrentTurnParty;
            int CurrentTurnIdx = client.CurrentStore.CurrentTurnPartyIdx;
            List<MemberMove> PossibleMoves = CurrentTurnPty.AllPossibleMemberMoves(CurrentTurnIdx, client.CurrentStore);
            if (PossibleMoves.Count == 0)
            {
                client.CurrentStore.TimePass();
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
                MemberMove BestTurn = CurrentTurnPty.BestTurn(Idx, client.CurrentStore);

                client.CurrentMove = BestTurn.Move;
                client.CurrentTargeting = BestTurn.Targeting;
                client.StateMachine.SetNewCurrentState(client, new DoMoveForward());
            }
        }

        public override void Exit(CommandBattle client)
        {
            client.SetAppropriateAnimations();
        }
    }
}
