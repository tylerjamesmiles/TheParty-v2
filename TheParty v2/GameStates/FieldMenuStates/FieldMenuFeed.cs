using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuFeed : State<GameStateFieldMenu>
    {
        GUIChoice Choice;

        public override void Enter(GameStateFieldMenu client)
        {
            List<Vector2> Positions = client.MemberSprites.ConvertAll(s => s.DrawPos + new Vector2(10, 10));
            Choice = new GUIChoice(Positions.ToArray());

            
            client.HungerIndicators.ForEach(hp => hp.SetShowMax(true));
            client.HungerIndicators.ForEach(hp => hp.SetMax(client.ActiveMembers[client.HungerIndicators.IndexOf(hp)].MaxHunger));
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Choice.Update(deltaTime, true);

            if (Choice.Done)
            {
                Member ToFeed = client.ActiveMembers[Choice.CurrentChoiceIdx];
                if (GameContent.Variables["FoodSupply"] > 0 && 
                    ToFeed.Hunger < ToFeed.MaxHunger)
                {
                    ToFeed.Hunger += 1;
                    GameContent.Variables["FoodSupply"] -= 1;
                    client.HungerIndicators[Choice.CurrentChoiceIdx].SetHP(ToFeed.Hunger);
                    Rectangle FoodBounds = new Rectangle(new Point(32, 4), new Point(32, 18));
                    client.Food = new GUIDialogueBox(FoodBounds, new[] { "\"" + GameContent.Variables["FoodSupply"].ToString() });


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

        public override void Exit(GameStateFieldMenu client)
        {
            client.HungerIndicators.ForEach(hp => hp.SetShowMax(false));
        }
    }
}
