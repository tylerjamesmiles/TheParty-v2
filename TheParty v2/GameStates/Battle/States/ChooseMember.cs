using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TheParty_v2
{
    class ChooseMember : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            int CurrentTurn = client.CurrentStore.CurrentTurnPartyIdx;
            List<Vector2> MemberPositions = client.PartySprites(CurrentTurn).ConvertAll(s => s.DrawPos);
            client.MemberChoice = new GUIChoice(MemberPositions.ToArray());
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MemberChoice.Update(deltaTime, true);

            if (client.MemberChoice.Done)
                client.StateMachine.SetNewCurrentState(client, new ChooseMove());

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new FightOrFlee());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.MemberChoice.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {

        }
    }
}
