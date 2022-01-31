using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class GameStateFieldMenu : State<TheParty>
    {
        public Player Player;

        public GUIDialogueBox Food;
        public GUIDialogueBox Money;
        public GUIDialogueBox Days;

        public List<Member> ActiveMembers;
        public List<Member> BackupMembers;
        public List<AnimatedSprite2D> MemberSprites;
        public List<HeartsIndicator> HPIndicators;
        public List<HeartsIndicator> HungerIndicators;
        public List<StanceIndicator> StanceIndicators;

        public StateMachine<GameStateFieldMenu> StateMachine;
        public bool Done;
        public bool Save;
        public bool Quit;

        public bool ReloadSprites;

        public int PreviousMainMenuChoice;

        public void LoadSprites(TheParty client)
        {
            ActiveMembers = client.Player.ActiveParty.Members;
            BackupMembers = client.Player.CampMembers;
            MemberSprites = new List<AnimatedSprite2D>();
            HPIndicators = new List<HeartsIndicator>();
            HungerIndicators = new List<HeartsIndicator>();
            StanceIndicators = new List<StanceIndicator>();

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                Member Member = ActiveMembers[i];

                Vector2 MemberDrawPos = new Vector2(16 + i * 48, 48);
                Vector2 MemberDrawOffset = new Vector2();
                string SpriteName = Member.SpriteName;
                AnimatedSprite2D Sprite = new AnimatedSprite2D(SpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.AddAnimation("Selected", 0, 1, 0.15f);
                Sprite.AddAnimation("Dead", 5, 1, 0f);

                if (Member.HP == 0)
                    Sprite.SetCurrentAnimation("Dead");
                else
                    Sprite.SetCurrentAnimation("Idle");

                MemberSprites.Add(Sprite);

                HeartsIndicator HP = new HeartsIndicator(
                    ActiveMembers[i].HP, 
                    (int)MemberDrawPos.X + 16, 
                    (int)MemberDrawPos.Y + 32 + 4);
                HPIndicators.Add(HP);

                HeartsIndicator Meats = new HeartsIndicator(
                    ActiveMembers[i].Hunger, 
                    (int)MemberDrawPos.X + 16, 
                    (int)MemberDrawPos.Y + 32 + 15, 
                    true);
                HungerIndicators.Add(Meats);

                StanceIndicator Stance = new StanceIndicator(
                    ActiveMembers[i].Stance,
                    MemberDrawPos + new Vector2(+14, -10));
                StanceIndicators.Add(Stance);

                PreviousMainMenuChoice = 0;
            }
        }

        public override void Enter(TheParty client)
        {
            Rectangle FoodBounds = new Rectangle(new Point(28, 4), new Point(36, 18));
            Food = new GUIDialogueBox(FoodBounds, new[] { "\"" + GameContent.Variables["FoodSupply"].ToString() });

            Rectangle MoneyBounds = new Rectangle(new Point(64, 4), new Point(36, 18));
            Money = new GUIDialogueBox(MoneyBounds, new[] { "$" + GameContent.Variables["Money"].ToString() });

            Rectangle DaysBounds = new Rectangle(new Point(100, 4), new Point(36, 18));
            Days = new GUIDialogueBox(DaysBounds, new[] { "&" + GameContent.Variables["DaysRemaining"].ToString() });

            LoadSprites(client);

            StateMachine = new StateMachine<GameStateFieldMenu>();
            StateMachine.SetNewCurrentState(this, new FieldMenuMain());

            Done = false;
            Quit = false;

            ReloadSprites = false;

            Player = client.Player;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            Food.Update(deltaTime, true);
            Money.Update(deltaTime, true);
            Days.Update(deltaTime, true);

            foreach (AnimatedSprite2D sprite in MemberSprites)
                sprite.Update(deltaTime);
            foreach (HeartsIndicator hp in HPIndicators)
                hp.Update(deltaTime);
            foreach (HeartsIndicator hunger in HungerIndicators)
                hunger.Update(deltaTime);
            foreach (StanceIndicator stance in StanceIndicators)
                stance.Update(deltaTime);

            StateMachine.Update(this, deltaTime);

            if (Done)
            {
                client.StateMachine.SetNewCurrentState(client, new GameStateField());
            }

            if (Save)
            {
                client.CommandQueue.EnqueueCommand(new CommandSave());
                Done = true;
            }

            if (Quit)
                client.StateMachine.SetNewCurrentState(client, new GameStateTitle());

            if (ReloadSprites)
            {
                LoadSprites(client);
                ReloadSprites = false;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize), Color.White);
            Food.Draw(spriteBatch, true);
            Money.Draw(spriteBatch, true);
            Days.Draw(spriteBatch, true);

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                MemberSprites[i].Draw(spriteBatch);

                if (ActiveMembers[i].HP > 0)
                {
                    HPIndicators[i].Draw(spriteBatch);
                    HungerIndicators[i].Draw(spriteBatch);
                    StanceIndicators[i].Draw(spriteBatch);
                }
            }


            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
