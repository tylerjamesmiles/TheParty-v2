using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class ChooseTarget : State<CommandBattle>
    {
        public override void Enter(CommandBattle client)
        {
            int FromIdx = client.MemberChoice.CurrentChoiceIdx;
            Member From = client.CurrentStore.MemberFromIdx(FromIdx);
            Move ChosenMove = From.Moves[client.MoveChoice.CurrentChoice];
            List<Targeting> PotentialTargets = new List<Targeting>();

            for (int targetParty = 0; targetParty < client.CurrentStore.Parties.Length; targetParty++)
            {
                for (int targetMem = 0; targetMem < client.CurrentStore.Parties[targetParty].Members.Length; targetMem++)
                {
                    Targeting PotTargeting = new Targeting()
                    {
                        FromPartyIdx = client.CurrentStore.CurrentTurnPartyIdx,
                        FromMemberIdx = FromIdx,
                        ToPartyIdx = targetParty,
                        ToMemberIdx = targetMem
                    };

                    if (Move.ValidOnMember(ChosenMove, client.CurrentStore, PotTargeting))
                    {
                        PotentialTargets.Add(PotTargeting);
                    }
                }
            }

            List<int> SpriteIdxs = PotentialTargets.ConvertAll(t => client.MemberSpriteIdx(t.ToPartyIdx, t.ToMemberIdx));
            List<Vector2> TargetPos = SpriteIdxs.ConvertAll(si => client.Sprites[si].DrawPos);

            client.TargetChoice = new GUIChoice(TargetPos.ToArray());
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            if (client.TargetChoice.NumChoices == 0)
            {
                client.StateMachine.SetNewCurrentState(client, new ChooseMove());
                return;
            }

            client.TargetChoice.Update(deltaTime, true);

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new ChooseMove());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.TargetChoice.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
