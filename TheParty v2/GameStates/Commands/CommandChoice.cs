using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandChoice : Command<TheParty>
    {
        string[] Choices;

        GUIChoiceBox Choice;
        List<Command<TheParty>>[] Outcomes;

        enum Show { Food, Money, Days };
        List<Show> ToShow;
        GUIDialogueBox Food;
        GUIDialogueBox Money;
        GUIDialogueBox Days;

        public CommandChoice(string[] choices, string show, List<Command<TheParty>>[] outcomes)
        {
            Choices = choices;
            Outcomes = outcomes;
            ToShow = new List<Show>();

            if (show != null)
            {
                string[] Shows = show.Split(' ');

                for (int i = 1; i < Shows.Length; i++)
                {
                    switch (Shows[i].ToLower())
                    {
                        case "": break;
                        case "food": ToShow.Add(Show.Food); break;
                        case "money": ToShow.Add(Show.Money); break;
                        case "days": ToShow.Add(Show.Days); break;
                    }
                }
            }
        }

        public override void Enter(TheParty client)
        {
            Choice = new GUIChoiceBox(Choices, GUIChoiceBox.Position.Center);

            Rectangle FoodBounds = new Rectangle(new Point(28, 4), new Point(36, 18));
            Food = new GUIDialogueBox(FoodBounds, new[] { "\"" + GameContent.Variables["FoodSupply"].ToString() });

            Rectangle MoneyBounds = new Rectangle(new Point(64, 4), new Point(36, 18));
            Money = new GUIDialogueBox(MoneyBounds, new[] { "$" + GameContent.Variables["Money"].ToString() });

            Rectangle DaysBounds = new Rectangle(new Point(100, 4), new Point(36, 18));
            Days = new GUIDialogueBox(DaysBounds, new[] { "&" + GameContent.Variables["DaysRemaining"].ToString() });

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Choice.Update(deltaTime, true);

            foreach (var show in ToShow)
            {
                switch (show)
                {
                    case Show.Food: Food.Update(deltaTime, true); break;
                    case Show.Money: Money.Update(deltaTime, true); break;
                    case Show.Days: Days.Update(deltaTime, true); break;
                }
            }

            if (Choice.Done)
            {
                Outcomes[Choice.CurrentChoice].ForEach(o => o.Done = false);
                Outcomes[Choice.CurrentChoice].ForEach(o => o.Entered = false);
                for (int i = Outcomes.Length - 1; i >= 0; i--)
                {
                    client.CommandQueue.PushCommand(Outcomes[Choice.CurrentChoice][i]);

                }
                Done = true;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            Choice.Draw(spriteBatch, true);
        
            foreach (var show in ToShow)
            {
                switch (show)
                {
                    case Show.Food: Food.Draw(spriteBatch, true); break;
                    case Show.Money: Money.Draw(spriteBatch, true); break;
                    case Show.Days: Days.Draw(spriteBatch, true); break;
                }
            }
        }
    }
}
