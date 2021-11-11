using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuHeal : State<GameStateFieldMenu>
    {
        GUIChoice Choice;

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
                Member ToHeal = client.ActiveMembers[Choice.CurrentChoiceIdx];
                if (ToHeal.HP < ToHeal.MaxHP && ToHeal.Hunger > 0)
                {
                    ToHeal.Hunger -= 1;
                    ToHeal.HitHP(+1);
                    client.HungerIndicators[Choice.CurrentChoiceIdx].SetHP(ToHeal.Hunger);
                    client.HPIndicators[Choice.CurrentChoiceIdx].SetHP(ToHeal.HP);
                }

                Choice.Done = false;
            }

            if (InputManager.JustReleased(Keys.Escape))
                client.StateMachine.SetNewCurrentState(client, new FieldMenuMain());
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Choice.Draw(spriteBatch, true);
        }
    }
}
