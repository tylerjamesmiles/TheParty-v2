using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Victory : State<CommandBattle>
    {
        GUIDialogueBox Msg;

        public Victory()
        {

        }

        public override void Enter(CommandBattle client)
        {
            List<string> VictoryMsgs = new List<string>();
            VictoryMsgs.Add("Victory!");
            string[] Rewards = client.CurrentStore.Rewards;
            for (int i = 0; i < Rewards.Length; i++)
            {
                // Give rewards
                string Reward = Rewards[i];
                string[] Keywords = Reward.Split(' ');
                int Num = int.Parse(Keywords[0]);
                string Type = Keywords[1];

                string Msg = "";

                switch (Type)
                {
                    case "$":
                        GameContent.Variables["Money"] += Num;
                        Msg = "The Party found \n" + Num.ToString() + " coins.";
                        break;

                    case "Food":
                        GameContent.Variables["FoodSupply"] += Num;
                        Msg = "The Party found \n" + Num.ToString() + " \" s";
                        break;
                }

                // Add to victory messages
                VictoryMsgs.Add(Msg);
            }

            Msg = new GUIDialogueBox(GUIDialogueBox.Position.SkinnyTop, VictoryMsgs.ToArray());
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            Msg.Update(deltaTime, true);

            if (Msg.Done)
            {
                if (client.ContinueAfter)
                {
                    GameContent.Switches[client.SwitchToSet] = true;
                }

                client.Done = true;
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Msg.Draw(spriteBatch, true);
        }

        public override void Exit(CommandBattle client)
        {
        }
    }
}
