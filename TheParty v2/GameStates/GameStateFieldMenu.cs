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
        public List<GUIBox> MemberBoxes;
        public List<AnimatedSprite2D> MemberSprites;
        public List<HeartsIndicator> HPIndicators;
        public List<HeartsIndicator> HungerIndicators;
        public List<HeartsIndicator> StanceIndicators;
        public List<GUIText> NameText;

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
            StanceIndicators = new List<HeartsIndicator>();
            MemberBoxes = new List<GUIBox>();
            NameText = new List<GUIText>();

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                Member Member = ActiveMembers[i];

                Vector2 MemberDrawPos = new Vector2(8, 5 + i * 33);
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

                Point BoxPos = new Point(8, 4 + i * 33);
                Point BoxSize = new Point(160, 35);
                GUIBox Box = new GUIBox(new Rectangle(BoxPos, BoxSize));
                MemberBoxes.Add(Box);

                GUIText NText = new GUIText(ActiveMembers[i].Name, new Vector2(BoxPos.X + 32, BoxPos.Y + 4), 160, 0.05f);
                NameText.Add(NText);

                HeartsIndicator St = new HeartsIndicator(
                    ActiveMembers[i].Stance,
                    BoxPos.X + 32, 
                    BoxPos.Y + 13,
                    HeartsIndicator.Type.Commitment,
                    true, 5, false);
                StanceIndicators.Add(St);

                HeartsIndicator HP = new HeartsIndicator(
                    ActiveMembers[i].HP, 
                    BoxPos.X + 32, 
                    BoxPos.Y + 19,
                    HeartsIndicator.Type.Hearts, false, 0, false);
                HPIndicators.Add(HP);

                HeartsIndicator Meats = new HeartsIndicator(
                    ActiveMembers[i].Hunger, 
                    BoxPos.X + 32, 
                    BoxPos.Y + 25, 
                    HeartsIndicator.Type.Meat, false, 0, false);
                HungerIndicators.Add(Meats);



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

            MemberBoxes.ForEach(b => b.Update(deltaTime, true));
            NameText.ForEach(n => n.Update(deltaTime, true));

            foreach (AnimatedSprite2D sprite in MemberSprites)
                sprite.Update(deltaTime);
            foreach (HeartsIndicator hp in HPIndicators)
                hp.Update(deltaTime);
            foreach (HeartsIndicator hunger in HungerIndicators)
                hunger.Update(deltaTime);
            foreach (HeartsIndicator stance in StanceIndicators)
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
            spriteBatch.Draw(
                GameContent.Sprites["Black"], 
                new Rectangle(new Point(0, 0), GraphicsGlobals.ScreenSize), 
                Color.White);

            //Food.Draw(spriteBatch, true);
            //Money.Draw(spriteBatch, true);
            //Days.Draw(spriteBatch, true);

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                MemberBoxes[i].Draw(spriteBatch, true);
                MemberSprites[i].Draw(spriteBatch);

                if (ActiveMembers[i].HP > 0)
                {
                    NameText[i].Draw(spriteBatch, true);

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
