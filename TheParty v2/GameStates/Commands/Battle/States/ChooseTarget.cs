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
        List<int> MemberIdxs;
        public override void Enter(CommandBattle client)
        {
            List<Targeting> PotentialTargets = new List<Targeting>();
            for (int targetParty = 0; targetParty < client.CurrentStore.NumParties; targetParty++)
            {
                for (int targetMem = 0; targetMem < client.CurrentStore.Parties[targetParty].NumMembers; targetMem++)
                {
                    client.CurrentTargeting.ToPartyIdx = targetParty;
                    client.CurrentTargeting.ToMemberIdx = targetMem;

                    if (client.CurrentMove.ValidOnMember(client.CurrentStore, client.CurrentTargeting))
                    {
                        PotentialTargets.Add(client.CurrentTargeting);
                    }
                }
            }

            MemberIdxs = PotentialTargets.ConvertAll(t => client.MemberSpriteIdx(t.ToPartyIdx, t.ToMemberIdx));
            List<Vector2> TargetPos = MemberIdxs.ConvertAll(si => client.Sprites[si].DrawPos);

            client.TargetChoice = new GUIChoice(TargetPos.ToArray());
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            client.TargetChoice.Update(deltaTime, true);

            if (client.TargetChoice.Done)
            {
                int MemberIdx = MemberIdxs[client.TargetChoice.CurrentChoiceIdx];
                client.CurrentTargeting.ToPartyIdx = client.PartyIdxOf(MemberIdx);
                client.CurrentTargeting.ToMemberIdx = client.PartyMemberIdxOf(MemberIdx);

                client.StateMachine.SetNewCurrentState(client, new DoMoveName());

            }

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new ChooseMove());
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            client.TargetChoice.Draw(spriteBatch, true);
        }
    }
}
