using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TheParty_v2
{
    class ChooseMove : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            int FromParty = client.CurrentStore.CurrentTurnPartyIdx;
            int FromMem = client.MemberChoice.CurrentChoiceIdx;
            Member Selected = BattleStore.Member(client.CurrentStore, FromParty, FromMem);
            Move[] Moves = Selected.Moves;
            string[] MoveNames = Selected.MoveNames();
            bool[] ChoiceValidity = new bool[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
                ChoiceValidity[i] = Move.ValidOnAnyone(Moves[i], client.CurrentStore, FromParty, FromMem);
                    
            client.MoveChoice = new GUIChoiceBox(MoveNames, GUIChoiceBox.Position.BottomRight, ChoiceValidity);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MoveChoice.Update(deltaTime, true);
            
            if (client.MoveChoice.Done)
                client.StateMachine.SetNewCurrentState(client, new ChooseTarget());

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.MoveChoice.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
