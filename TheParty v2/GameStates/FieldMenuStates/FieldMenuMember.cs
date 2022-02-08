using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuMember : State<GameStateFieldMenu>
    {
        enum State { ChooseMember, ChooseAction };
        State CurrentState;

        GUIChoice MemberChoice;
        GUIChoiceBox ActionChoice;

        Member Selected;

        public FieldMenuMember()
        {

        }

        public override void Enter(GameStateFieldMenu client)
        {
            List<Vector2> Positions = client.MemberSprites.ConvertAll(s => s.DrawPos + new Vector2(10, 10));
            MemberChoice = new GUIChoice(Positions.ToArray());

            for (int i = 0; i < client.ActiveMembers.Count; i++)
            {
                client.HPIndicators[i].SetMax(client.ActiveMembers[i].MaxHP);
                client.HungerIndicators[i].SetMax(client.ActiveMembers[i].MaxHunger);
                client.HPIndicators[i].SetShowMax(true);
                client.HungerIndicators[i].SetShowMax(true);
            }

            CurrentState = State.ChooseMember;
        }

        private void LoadActions(GameStateFieldMenu client)
        {
            Selected = client.ActiveMembers[MemberChoice.CurrentChoiceIdx];

            string[] Actions = new[] { "Heal%", "Feed\"", "Commit" };
            bool[] ChoiceValidity = new bool[3];
            ChoiceValidity[0] = Selected.HP < Selected.MaxHP && Selected.Hunger > 0;
            ChoiceValidity[1] = Selected.Hunger < Selected.MaxHunger && GameContent.Variables["FoodSupply"] > 0;
            ChoiceValidity[2] = Selected.Stance < 9;

            int CurrentChoice =
                ActionChoice != null ? ActionChoice.CurrentChoice : 0;
            ActionChoice = new GUIChoiceBox(Actions, GUIChoiceBox.Position.TopRight, 1, ChoiceValidity);
            ActionChoice.SetCurrentChoice(CurrentChoice);
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Member Selected = client.ActiveMembers[MemberChoice.CurrentChoiceIdx];
            HeartsIndicator Stance = client.StanceIndicators[MemberChoice.CurrentChoiceIdx];
            HeartsIndicator Hearts = client.HPIndicators[MemberChoice.CurrentChoiceIdx];
            HeartsIndicator Meats = client.HungerIndicators[MemberChoice.CurrentChoiceIdx];



            switch (CurrentState)
            {
                case State.ChooseMember:
                    MemberChoice.Update(deltaTime, true);
                    if (MemberChoice.Done && Selected.HP > 0)
                    {
                        LoadActions(client);
                        CurrentState = State.ChooseAction;
                    }
                    else if (InputManager.JustReleased(Keys.Escape))
                    {
                        GameContent.SoundEffects["MenuBack"].Play();
                        client.StateMachine.SetNewCurrentState(client, new FieldMenuMain());

                    }

                    MemberChoice.Done = false;
                    break;

                case State.ChooseAction:
                    if (InputManager.JustPressed(Keys.A))
                    {
                        int NewChoice = MemberChoice.CurrentChoiceIdx - 1;
                        if (NewChoice < 0)
                            NewChoice = client.ActiveMembers.Count - 1;
                        MemberChoice.SetChoice(NewChoice);

                        LoadActions(client);
                    }
                    else if (InputManager.JustPressed(Keys.D))
                    {
                        int NewChoice = MemberChoice.CurrentChoiceIdx + 1;
                        if (NewChoice >= client.ActiveMembers.Count)
                            NewChoice = 0;
                        MemberChoice.SetChoice(NewChoice);

                        LoadActions(client);
                    }
                    else
                    {
                        ActionChoice.Update(deltaTime, true);
                        MemberChoice.OnlyUpdateLerp(deltaTime);
                        if (ActionChoice.Done)
                        {
                            switch (ActionChoice.CurrentChoice)
                            {
                                case 0:     // heal
                                    Selected.HP += 1;
                                    Hearts.SetHP(Selected.HP);

                                    Selected.Hunger -= 1;
                                    Meats.SetHP(Selected.Hunger);

                                    break;

                                case 1:     // feed
                                    Selected.Hunger += 1;
                                    Meats.SetHP(Selected.Hunger);

                                    GameContent.Variables["FoodSupply"] -= 1;
                                    client.FoodMoneyDays.SetNewText(
                                        "\"" + GameContent.Variables["FoodSupply"].ToString() + "\n" +
                                        "$" + GameContent.Variables["Money"].ToString() + "\n" +
                                        "&" + GameContent.Variables["DaysRemaining"].ToString()
                                        );

                                    break;

                                case 2:     // commit
                                    Selected.Stance += 1;
                                    Stance.SetHP(Selected.Stance);
                                    break;
                            }

                            int CurrentAction = ActionChoice.CurrentChoice;
                            LoadActions(client);
                            ActionChoice.SetCurrentChoice(CurrentAction);
                        }
                    }
                    

                    if (InputManager.JustReleased(Keys.Escape))
                    {
                        GameContent.SoundEffects["MenuBack"].Play();

                        CurrentState = State.ChooseMember;
                    }

                    break;
            }
        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            MemberChoice.Draw(spriteBatch, true);

            if (CurrentState == State.ChooseAction)
                ActionChoice.Draw(spriteBatch, true);
        }

        public override void Exit(GameStateFieldMenu client)
        {
            //client.HPIndicators.ForEach(hp => hp.SetShowMax(false));
            //client.HungerIndicators.ForEach(h => h.SetShowMax(false));
        }

    }
}
