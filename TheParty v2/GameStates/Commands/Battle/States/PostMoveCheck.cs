using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class PostMoveCheck : State<CommandBattle>
    {
        public override void Update(CommandBattle client, float deltaTime)
        {
            // Is player dead?
            Party Player = client.CurrentStore.Parties[0];
            if (Player.IsDead)
            {
                client.GameOver = true;
                return;
            }

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

            // Has everybody in the current party gone?
            if (client.CurrentStore.CurrentTurnParty.Members.TrueForAll(m => !m.CanGo))
            {
                client.StateMachine.SetNewCurrentState(client, new TimePass());
            }
            else
            {
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
            }
        }

        public override void Exit(CommandBattle client)
        {
            client.SetAppropriateAnimations();
        }
    }
}
