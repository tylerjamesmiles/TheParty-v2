using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandLevelUp : Command<TheParty>
    {
        List<Member> ActiveMembers;
        List<AnimatedSprite2D> Sprites;
        List<HeartsIndicator> Hearts;
        List<HeartsIndicator> Meats;

        Timer WaitTimer;
        GUIChoice MemberChoice;
        GUIChoiceBox LevelUpTypeChoice;
        GUIChoiceBox MoveChoice;
        Timer WaitTimer2;

        GUIDialogueBox Description;

        bool[] Living;
        enum State { WaitForMoment, ChooseMember, ChooseType, ChooseMove, WaitAnotherMoment };
        State CurrentState;

        public override void Enter(TheParty client)
        {
            WaitTimer = new Timer(0.1f);

            int party = 0;
            ActiveMembers = client.Player.ActiveParty.Members;
            Sprites = new List<AnimatedSprite2D>();
            Hearts = new List<HeartsIndicator>();
            Meats = new List<HeartsIndicator>();
            for (int member = 0; member < ActiveMembers.Count; member++)
            {
                Vector2 MemberDrawOffset = new Vector2(-16, -16);
                int MemberDrawStartX = (party == 0) ? 110 : 48;
                int MemberXOffset = (party == 0) ? 16 : -16;
                int MemberDrawX = MemberDrawStartX + member * MemberXOffset;
                int MemberDrawStartY = 62;
                int MemberDrawY = MemberDrawStartY + member * 16;
                Vector2 MemberDrawPos = new Vector2(MemberDrawX, MemberDrawY);

                Member mem = ActiveMembers[member];
                AnimatedSprite2D Sprite = new AnimatedSprite2D(mem.SpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("PositiveHit", 4, 1, 0.15f);
                Sprite.AddAnimation("Dead", 6, 1, 0.15f);

                if (mem.HP > 0)
                    Sprite.SetCurrentAnimation("Idle");
                else
                    Sprite.SetCurrentAnimation("Dead");

                Sprites.Add(Sprite);

                int HP = ActiveMembers[member].HP;
                Hearts.Add(new HeartsIndicator(HP, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 18, false, true, ActiveMembers[member].MaxHP));

                int Hunger = ActiveMembers[member].Hunger;
                Meats.Add(new HeartsIndicator(Hunger, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 26, true));
            }

            List<Vector2> Positions = Sprites.ConvertAll(s => s.DrawPos + new Vector2(0, 0));

            Living = new bool[ActiveMembers.Count];
            for (int i = 0; i < ActiveMembers.Count; i++)
                Living[i] = ActiveMembers[i].HP > 0;

            MemberChoice = new GUIChoice(Positions.ToArray());

            

            WaitTimer2 = new Timer(1f);

            CurrentState = State.WaitForMoment;

            Entered = true;
        }

        private void SetupTypeChoice()
        {
            bool[] ChoiceValidity = new bool[3];
            for (int i = 0; i < 3; i++)
            {
                ChoiceValidity[i] = true;
                if (i == 2)
                    ChoiceValidity[i] = ActiveMembers[MemberChoice.CurrentChoiceIdx].MovesToLearn.Count > 0;
            }

            LevelUpTypeChoice = new GUIChoiceBox(
                new[] { "+1 Max%'s", "+1 Max\"'s", "New Move" },
                GUIChoiceBox.Position.Center,
                1, ChoiceValidity);
        }

        public override void Update(TheParty client, float deltaTime)
        {
            foreach (AnimatedSprite2D sprite in Sprites)
                sprite.Update(deltaTime);

            foreach (HeartsIndicator hp in Hearts)
                hp.Update(deltaTime);

            foreach (HeartsIndicator meat in Meats)
                meat.Update(deltaTime);

            Member Selected = ActiveMembers[MemberChoice.CurrentChoiceIdx];


            switch (CurrentState)
            {
                case State.WaitForMoment:
                    WaitTimer.Update(deltaTime);
                    if (WaitTimer.TicThisFrame)
                        CurrentState = State.ChooseMember;
                    break;

                case State.ChooseMember:
                    MemberChoice.Update(deltaTime, true);
                    if (MemberChoice.Done)
                    {
                        if (Living[MemberChoice.CurrentChoiceIdx])
                        {
                            SetupTypeChoice();
                            CurrentState = State.ChooseType;
                        }
                        else
                            MemberChoice.Done = false;
                    }
                    break;

                case State.ChooseType:
                    LevelUpTypeChoice.Update(deltaTime, true);
                    if (LevelUpTypeChoice.Done)
                    {
                        switch (LevelUpTypeChoice.CurrentChoice)
                        {
                            case 0:     // +1 max HP
                                Selected.MaxHP += 2;
                                Hearts[MemberChoice.CurrentChoiceIdx].MaxHP = Selected.MaxHP;
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;
                                break;

                            case 1:     // +1 max Hunger
                                Selected.MaxHunger += 2;
                                Meats[MemberChoice.CurrentChoiceIdx].MaxHP = Selected.MaxHunger;
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;
                                break;

                            case 2:     // new move
                                List<string> MoveNames = Selected.MovesToLearn;
                                MoveChoice = new GUIChoiceBox(MoveNames.ToArray(), GUIChoiceBox.Position.Center);

                                Move CurrentMove = Selected.GetMoves()[MoveChoice.CurrentChoice];
                                Description = new GUIDialogueBox(
                                    GUIDialogueBox.Position.SkinnyTop,
                                    new[] { CurrentMove.Description }, 0.01f);

                                CurrentState = State.ChooseMove;
                                break;
                        }
                    }
                    else if (InputManager.JustReleased(Keys.Escape))
                    {
                        MemberChoice.Done = false;
                        CurrentState = State.ChooseMember;
                    }

                    break;

                case State.ChooseMove:
                    MoveChoice.Update(deltaTime, true);

                    if (MoveChoice.ChoiceUpdatedThisFrame)
                    {
                        Move CurrentMove = Selected.GetMovesToLearn()[MoveChoice.CurrentChoice];
                        Description.SetNewText(CurrentMove.Description);
                    }

                    if (Description != null)
                        Description.Update(deltaTime, true);

                    if (MoveChoice.Done)
                    {
                        Selected.Moves.Add(Selected.MovesToLearn[MoveChoice.CurrentChoice]);
                        Selected.MovesToLearn.RemoveAt(MoveChoice.CurrentChoice);
                        Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                        CurrentState = State.WaitAnotherMoment;
                    }
                    else if (InputManager.JustReleased(Keys.Escape))
                    {
                        SetupTypeChoice();
                        CurrentState = State.ChooseType;
                    }
                    break;

                case State.WaitAnotherMoment:
                    WaitTimer2.Update(deltaTime);
                    if (WaitTimer2.TicThisFrame)
                    {
                        Done = true;
                    }
                    break;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);
            Sprites.ForEach(s => s.Draw(spriteBatch));

            for (int i = 0; i < Hearts.Count; i++)
            {
                if (Living[i])
                {
                    Hearts[i].Draw(spriteBatch);
                    Meats[i].Draw(spriteBatch);
                }
            }

            if (CurrentState == State.ChooseMember)
                MemberChoice.Draw(spriteBatch, true);

            if (CurrentState == State.ChooseType)
                LevelUpTypeChoice.Draw(spriteBatch, true);

            if (CurrentState == State.ChooseMove)
            {
                MoveChoice.Draw(spriteBatch, true);


                Description.Draw(spriteBatch, true);
            }
        }
    }
}
