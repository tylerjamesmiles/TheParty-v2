using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class ChooseMember : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            client.MemberChoice = new GUIChoice(client.MemberPositions);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MemberChoice.Update(deltaTime, true);
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.MemberChoice.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
            throw new NotImplementedException();
        }
    }
}
