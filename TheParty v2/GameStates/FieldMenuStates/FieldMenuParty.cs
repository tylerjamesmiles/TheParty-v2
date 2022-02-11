using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuParty : State<GameStateFieldMenu>
    {
        List<AnimatedSprite2D> Sprites;
        List<HeartsIndicator> HPIndicators;
        List<HeartsIndicator> HungerIndicators;
        GUIChoice Member1Choice;
        GUIChoice Member2Choice;

        enum State { PickFirstMember, PickSecondMember };
        State CurrentState;

        public override void Enter(GameStateFieldMenu client)
        {
            List<Vector2> MemberPositions = new List<Vector2>();

            // top row
            foreach (AnimatedSprite2D sprite in client.MemberSprites)
                MemberPositions.Add(sprite.DrawPos + new Vector2(12, 12));

            if (client.MemberSprites.Count < 3)
            {
                int i = client.MemberSprites.Count;
                MemberPositions.Add(new Vector2(16 + i * 48, 48) + new Vector2(12, 12));
            }

            // bottom row
            Sprites = new List<AnimatedSprite2D>();
            HPIndicators = new List<HeartsIndicator>();
            HungerIndicators = new List<HeartsIndicator>();
            for (int i = 0; i < client.BackupMembers.Count; i++)
            {
                Member Member = client.BackupMembers[i];
                string SpriteName = Member.BattleSpriteName;
                Point FrameSize = new Point(32, 32);
                Vector2 DrawPos = new Vector2(i * 32, 96);
                Vector2 Offset = new Vector2();
                AnimatedSprite2D Sprite = new AnimatedSprite2D(SpriteName, FrameSize, DrawPos, Offset);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("Selected", 0, 1, 0.15f);
                Sprite.SetCurrentAnimation("Idle");
                Sprites.Add(Sprite);

                MemberPositions.Add(DrawPos + new Vector2(12, 12));

                HPIndicators.Add(new HeartsIndicator(Member.HP, (int)DrawPos.X + 16, (int)DrawPos.Y + 32));
                HungerIndicators.Add(new HeartsIndicator(Member.Hunger, (int)DrawPos.X + 16, (int)DrawPos.Y + 38, HeartsIndicator.Type.Meats));
            }
            
            Member1Choice = new GUIChoice(MemberPositions.ToArray());
            Member2Choice = new GUIChoice(MemberPositions.ToArray());

            CurrentState = State.PickFirstMember;
        }

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            Sprites.ForEach(s => s.Update(deltaTime));

            HPIndicators.ForEach(i => i.Update(deltaTime));
            HungerIndicators.ForEach(i => i.Update(deltaTime));

            switch (CurrentState)
            {
                case State.PickFirstMember:
                    Member1Choice.Update(deltaTime, true);
                    if (Member1Choice.Done)
                    {
                        CurrentState = State.PickSecondMember;
                        Member2Choice.SetChoice(Member1Choice.CurrentChoiceIdx);
                    }
                    else if (InputManager.JustReleased(Keys.Escape))
                    {
                        GameContent.SoundEffects["MenuBack"].Play();
                        client.StateMachine.SetNewCurrentState(client, new FieldMenuMain());
                    }

                    break;

                case State.PickSecondMember:
                    Member2Choice.Update(deltaTime, true);
                    if (Member2Choice.Done)
                    {
                        List<Member> BigList = new List<Member>();
                        BigList.AddRange(client.ActiveMembers);
                        BigList.AddRange(client.BackupMembers);

                        int Choice1 = Member1Choice.CurrentChoiceIdx;
                        int Choice2 = Member2Choice.CurrentChoiceIdx;

                        // Swap
                        Member Choice1Copy = BigList[Choice1].DeepCopy();
                        BigList[Choice1] = BigList[Choice2];
                        BigList[Choice2] = Choice1Copy;

                        int NumActiveMembers = client.ActiveMembers.Count;
                        int NumBackupMembers = client.BackupMembers.Count;

                        client.ActiveMembers.Clear();
                        client.ActiveMembers.AddRange(BigList.GetRange(0, NumActiveMembers));

                        client.BackupMembers.Clear();
                        client.BackupMembers.AddRange(BigList.GetRange(NumActiveMembers, NumBackupMembers));

                        Enter(client);
                        client.ReloadSprites = true;

                        Member1Choice.SetChoice(Choice2);
                    }
                    else if (InputManager.JustReleased(Keys.Escape))
                    {
                        GameContent.SoundEffects["MenuBack"].Play();
                        CurrentState = State.PickFirstMember;

                    }

                    break;
            }


        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            Sprites.ForEach(s => s.Draw(spriteBatch));
            HPIndicators.ForEach(i => i.Draw(spriteBatch));
            HungerIndicators.ForEach(i => i.Draw(spriteBatch));

            Member1Choice.Draw(spriteBatch, true);

            if (CurrentState == State.PickSecondMember)
                Member2Choice.Draw(spriteBatch, true);
        }

        public override void Exit(GameStateFieldMenu client)
        {
        }
    }
}
