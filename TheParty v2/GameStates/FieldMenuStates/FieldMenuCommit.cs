using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuCommit : State<GameStateFieldMenu>
    {
        GUIChoice Choice;

        public FieldMenuCommit()
        {

        }

        public override void Enter(GameStateFieldMenu client)
        {
            List<Vector2> Positions = client.MemberSprites.ConvertAll(s => s.DrawPos + new Vector2(10, 10));
            Choice = new GUIChoice(Positions.ToArray());
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Choice.Update(deltaTime, true);
            if (Choice.Done)
            {
                Member Selected = client.ActiveMembers[Choice.CurrentChoiceIdx];
                StanceIndicator Indicator = client.StanceIndicators[Choice.CurrentChoiceIdx];
                HeartsIndicator Hunger = client.HungerIndicators[Choice.CurrentChoiceIdx];
                if (Selected.HP > 0 && Selected.Stance < 9 && Selected.Hunger > 0)
                {
                    Selected.Hunger -= 1;

                    if (Selected.Hunger < 0)
                        Selected.Hunger = 0;

                    Hunger.SetHP(Selected.Hunger);

                    Selected.Stance += 1;
                    Indicator.SetTarget(Selected.Stance);
                }

                Choice.Done = false;
            }

            if (InputManager.JustReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new FieldMenuMain());
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Choice.Draw(spriteBatch, true);
        }
    }
}
