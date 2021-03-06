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

        public CommandLevelUp()
        {
            WaitTimer = new Timer(0.1f);
            Sprites = new List<AnimatedSprite2D>();
            Hearts = new List<HeartsIndicator>();
            Meats = new List<HeartsIndicator>();
        }

        public override void Enter(TheParty client)
        {
            int party = 0;
            ActiveMembers = client.Player.ActiveParty.Members;
            int NumMembers = client.Player.ActiveParty.NumMembers;

            for (int member = 0; member < ActiveMembers.Count; member++)
            {
                Vector2 MemberDrawOffset = new Vector2(-16, -16);
                int ScreenCenter = GraphicsGlobals.ScreenSize.X / 2;
                int MemberDrawStartX =
                    (party == 0) ?
                        NumMembers > 2 ?
                            ScreenCenter + 24 :
                            ScreenCenter + 24 + 45 :
                        NumMembers > 2 ?
                            ScreenCenter - 24 :
                            ScreenCenter - 24 - 45;

                int MemberXOffset = (party == 0) ? 18 : -18;
                int MemberDrawX = MemberDrawStartX + member * MemberXOffset - (member > 2 ? MemberXOffset / 2 : 0);
                int MemberDrawStartY = 54;
                int MemberDrawY = MemberDrawStartY + (member % 3) * 18;
                Vector2 MemberDrawPos = new Vector2(MemberDrawX, MemberDrawY);

                Member mem = ActiveMembers[member];
                AnimatedSprite2D Sprite = new AnimatedSprite2D(mem.BattleSpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset, party > 0);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("PositiveHit", 3, 1, 0.15f);
                Sprite.AddAnimation("Dead", 5, 1, 0.15f);

                if (mem.HP > 0)
                    Sprite.SetCurrentAnimation("Idle");
                else
                    Sprite.SetCurrentAnimation("Dead");

                Sprites.Add(Sprite);

                int HP = ActiveMembers[member].HP;
                Hearts.Add(new HeartsIndicator(
                    HP, 
                    (int)Sprite.DrawPos.X, 
                    (int)Sprite.DrawPos.Y + 18, 
                    HeartsIndicator.Type.Hearts, true, 
                    ActiveMembers[member].MaxHP));

                int Hunger = ActiveMembers[member].Hunger;
                Meats.Add(new HeartsIndicator(
                    Hunger, 
                    (int)Sprite.DrawPos.X, 
                    (int)Sprite.DrawPos.Y + 28, 
                    HeartsIndicator.Type.Meats, true,
                    ActiveMembers[member].MaxHunger));
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
            Member Member = ActiveMembers[MemberChoice.CurrentChoiceIdx];
            bool[] ChoiceValidity = new bool[5];
            ChoiceValidity[0] = Member.HP < Member.MaxHP;
            ChoiceValidity[1] = Member.Hunger < Member.MaxHunger;
            ChoiceValidity[2] = Member.MaxHP < 10;
            ChoiceValidity[3] = Member.MaxHunger < 10;
            ChoiceValidity[4] = Member.MovesToLearn.Count > 0;

            LevelUpTypeChoice = new GUIChoiceBox(
                new[] { "Heal all %'s", "Heal all \"'s", "+1 Max%'s", "+1 Max\"'s", "New Move" },
                GUIChoiceBox.Position.BottomLeft,
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
                            case 0: // Heal HP
                                Selected.HP = Selected.MaxHP;
                                Hearts[MemberChoice.CurrentChoiceIdx].SetHP(Selected.HP);
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;

                                break;

                            case 1: // Heal Hunger
                                Selected.Hunger = Selected.MaxHunger;
                                Meats[MemberChoice.CurrentChoiceIdx].SetHP(Selected.Hunger);
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;
                                break;

                            case 2:     // +1 max HP
                                Selected.MaxHP += 1;
                                Hearts[MemberChoice.CurrentChoiceIdx].SetMax(Selected.MaxHP);
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;
                                break;

                            case 3:     // +1 max Hunger
                                Selected.MaxHunger += 1;
                                Meats[MemberChoice.CurrentChoiceIdx].SetMax(Selected.MaxHunger);
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;
                                break;

                            case 4:     // new move
                                List<string> MoveNames = Selected.MovesToLearn;
                                MoveChoice = new GUIChoiceBox(MoveNames.ToArray(), GUIChoiceBox.Position.BottomLeft);

                                Move CurrentMove = Selected.GetMovesToLearn()[MoveChoice.CurrentChoice];
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
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize), Color.White);

            for (int i = 0; i < Hearts.Count; i++)
            {
                if (Living[i])
                {
                    Hearts[i].Draw(spriteBatch, Vector2.Zero);
                    Meats[i].Draw(spriteBatch, Vector2.Zero);
                }
            }

            Sprites.ForEach(s => s.Draw(spriteBatch));

            if (CurrentState == State.ChooseMember)
                MemberChoice.Draw(spriteBatch, true);

            if (CurrentState == State.ChooseType)
            {
                LevelUpTypeChoice.Draw(spriteBatch, true);

                Vector2 HandPos = Sprites[MemberChoice.CurrentChoiceIdx].DrawPos + new Vector2(-20, 0);
                spriteBatch.Draw(
                    GameContent.Sprites["Cursor"],
                    new Rectangle(HandPos.ToPoint(), new Point(16, 16)),
                    new Rectangle(new Point(16, 0), new Point(16, 16)),
                    Color.White);
            }

            if (CurrentState == State.ChooseMove)
            {
                MoveChoice.Draw(spriteBatch, true);

                Description.Draw(spriteBatch, true);
            }
        }
    }
}
