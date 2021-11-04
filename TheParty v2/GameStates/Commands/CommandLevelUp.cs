using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                Sprite.SetCurrentAnimation("Idle");
                Sprites.Add(Sprite);

                int HP = ActiveMembers[member].HP;
                Hearts.Add(new HeartsIndicator(HP, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 18, false, true, ActiveMembers[member].MaxHP));

                int Hunger = ActiveMembers[member].Hunger;
                Meats.Add(new HeartsIndicator(Hunger, (int)Sprite.DrawPos.X, (int)Sprite.DrawPos.Y + 26, true));
            }

            List<Vector2> Positions = Sprites.ConvertAll(s => s.DrawPos + new Vector2(0, 0));
            MemberChoice = new GUIChoice(Positions.ToArray());

            string[] LevelUpChoices = new[]
            {
                "Recover all %'s",
                "+1 Max%'s",
                "#New Move#"
            };

            LevelUpTypeChoice = new GUIChoiceBox(new[] { "+1 Max%'s", "New Move" }, GUIChoiceBox.Position.Center);

            WaitTimer2 = new Timer(1f);

            CurrentState = State.WaitForMoment;

            Entered = true;
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
                        CurrentState = State.ChooseType;
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
                                Selected.HP = Selected.MaxHP;
                                Hearts[MemberChoice.CurrentChoiceIdx].MaxHP = Selected.MaxHP;
                                Hearts[MemberChoice.CurrentChoiceIdx].SetHP(Selected.HP);
                                Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                                CurrentState = State.WaitAnotherMoment;
                                break;

                            case 1:     // new move
                                List<string> MoveNames = Selected.MovesToLearn.ConvertAll(m => m.Name);
                                MoveChoice = new GUIChoiceBox(MoveNames.ToArray(), GUIChoiceBox.Position.Center);
                                CurrentState = State.ChooseMove;
                                break;
                        }
                    }

                    break;

                case State.ChooseMove:
                    MoveChoice.Update(deltaTime, true);
                    if (MoveChoice.Done)
                    {
                        Selected.Moves.Add(Selected.MovesToLearn[MoveChoice.CurrentChoice]);
                        Selected.MovesToLearn.RemoveAt(MoveChoice.CurrentChoice);
                        Sprites[MemberChoice.CurrentChoiceIdx].SetCurrentAnimation("PositiveHit");
                        CurrentState = State.WaitAnotherMoment;
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
            Hearts.ForEach(h => h.Draw(spriteBatch));
            Meats.ForEach(m => m.Draw(spriteBatch));

            if (CurrentState == State.ChooseMember)
                MemberChoice.Draw(spriteBatch, true);

            if (CurrentState == State.ChooseType)
                LevelUpTypeChoice.Draw(spriteBatch, true);

            if (MoveChoice != null)
                MoveChoice.Draw(spriteBatch, true);
        }
    }
}
