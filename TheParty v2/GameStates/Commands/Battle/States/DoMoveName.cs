using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoMoveName : State<CommandBattle>
    {
        GUIDialogueBox MoveName;
        Timer ShowNameTimer;

        public override void Enter(CommandBattle client)
        {
            MoveName = new GUIDialogueBox(GUIDialogueBox.Position.ReallySkinnyTop, new[] { client.CurrentMove.Name });
            ShowNameTimer = new Timer(0.4f);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            MoveName.Update(deltaTime, true);

            if (MoveName.TextShown)
                ShowNameTimer.Update(deltaTime);
            if (ShowNameTimer.TicThisFrame)
                client.StateMachine.SetNewCurrentState(client, new DoMoveForward());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            MoveName.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
