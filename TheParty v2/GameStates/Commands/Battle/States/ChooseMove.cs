using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TheParty_v2
{
    class ChooseMove : State<CommandBattle>
    {
        GUIDialogueBox Description;

        public override void Enter(CommandBattle client)
        {
            Member Selected = client.FromMember;
            List<Move> Moves = Selected.GetMoves();
            bool[] ChoiceValidity = new bool[Moves.Count];
            for (int i = 0; i < Moves.Count; i++)
                ChoiceValidity[i] = client.MoveValidOnAnyone(Selected.GetMoves()[i]);
            int NumMoves = Moves.Count;
            int NumCollumns = NumMoves == 2 ? 1 : 2;
            List<string> MoveNames = Moves.ConvertAll(m => m.Name);

            if (MoveNames.Count == 0)
                throw new Exception("Trying to choose between no moves.");

            client.MoveChoice = new GUIChoiceBox(MoveNames.ToArray(), GUIChoiceBox.Position.BottomRight, NumCollumns, ChoiceValidity);

            int CurrentChoice = client.MoveChoice.CurrentChoice;
            string DescrTxt = client.FromMember.GetMoves()[CurrentChoice].Description;
            Description = new GUIDialogueBox(GUIDialogueBox.Position.SkinnyTop, new string[] { DescrTxt }, 0.01f, false);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.MoveChoice.Update(deltaTime, true);

            if (client.MoveChoice.ChoiceUpdatedThisFrame)
            {
                int CurrentChoice = client.MoveChoice.CurrentChoice;
                string DescrTxt = client.FromMember.GetMoves()[CurrentChoice].Description;
                Description = new GUIDialogueBox(GUIDialogueBox.Position.SkinnyTop, new string[] { DescrTxt }, 0.01f);
            }

            Description.Update(deltaTime, true);

            if (client.MoveChoice.Done)
            {
                client.CurrentMove = client.FromMember.GetMoves()[client.MoveChoice.CurrentChoice];

                if (client.CurrentMove.Conditions.Contains("Is(Target, Self)"))
                {
                    client.CurrentTargeting.ToPartyIdx = client.CurrentTargeting.FromPartyIdx;
                    client.CurrentTargeting.ToMemberIdx = client.CurrentTargeting.FromMemberIdx;
                    client.StateMachine.SetNewCurrentState(client, new DoMoveForward());
                }
                else
                    client.StateMachine.SetNewCurrentState(client, new ChooseTarget());
            }

            if (InputManager.JustReleased(Keys.Escape))
            {
                int OldChoice = client.MemberChoice.CurrentChoiceIdx;
                client.StateMachine.SetNewCurrentState(client, new ChooseMember());
                client.MemberChoice.SetChoice(OldChoice);
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Point DeadHandPos = client.FromSprite.DrawPos.ToPoint() + new Point(-24, -8);
            spriteBatch.Draw(
                GameContent.Sprites["Cursor"],
                new Rectangle(DeadHandPos, new Point(16, 16)),
                new Rectangle(new Point(16, 0), new Point(16, 16)),
                Color.White);

            client.MoveChoice.Draw(spriteBatch, true);
            Description.Draw(spriteBatch, true);


        }
    }
}
