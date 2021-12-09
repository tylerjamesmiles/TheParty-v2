using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FightOrFlee : State<CommandBattle>
    {
        GUIChoiceBox Choice;
        GUIDialogueBox Description;

        public override void Enter(CommandBattle client)
        {
            int MyStance = 0;
            foreach (Member member in client.CurrentStore.Parties[0].Members)
                MyStance += member.Stance;

            int TheirStance = 0;
            foreach (Member member in client.CurrentStore.Parties[1].Members)
                TheirStance += member.Stance;

            bool[] ChoiceValidity = new bool[2];
            ChoiceValidity[0] = true;
            ChoiceValidity[1] = MyStance > TheirStance && client.CanFlee;

            Choice = new GUIChoiceBox(
                new[] { "Fight", "Flee" }, 
                GUIChoiceBox.Position.Center, 
                1, ChoiceValidity);

            string DescriptionText =
                (client.CanFlee) ? 
                    MyStance > TheirStance ? 
                        "You can flee!" : 
                        "You need more commitment to flee!" : 
                    "You cannot run from this fight.";

            Description = new GUIDialogueBox(
                GUIDialogueBox.Position.SkinnyTop,
                new[] { DescriptionText },
                0.01f);
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            Choice.Update(deltaTime, true);
            Description.Update(deltaTime, true);
            if (Choice.Done)
            {
                switch (Choice.CurrentChoice)
                {
                    case 0: client.StateMachine.SetNewCurrentState(client, new ChooseMember());  break;
                    case 1: client.Done = true; break;
                }
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {
            Choice.Draw(spriteBatch, true);
            Description.Draw(spriteBatch, true);
        }
    }
}
