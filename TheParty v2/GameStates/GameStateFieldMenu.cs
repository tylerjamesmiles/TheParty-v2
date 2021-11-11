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
        public GUIDialogueBox Food;
        public GUIDialogueBox Money;
        public GUIDialogueBox Days;

        public List<Member> ActiveMembers;
        public List<AnimatedSprite2D> MemberSprites;
        public List<HeartsIndicator> HPIndicators;
        public List<HeartsIndicator> HungerIndicators;

        public StateMachine<GameStateFieldMenu> StateMachine;
        public bool Done;
        public bool Quit;

        public override void Enter(TheParty client)
        {
            Rectangle FoodBounds = new Rectangle(new Point(28, 4), new Point(36, 18));
            Food = new GUIDialogueBox(FoodBounds, new[] { "\"" + GameContent.Variables["FoodSupply"].ToString() });

            Rectangle MoneyBounds = new Rectangle(new Point(64, 4), new Point(36, 18));
            Money = new GUIDialogueBox(MoneyBounds, new[] { "$" + GameContent.Variables["Money"].ToString() });

            Rectangle DaysBounds = new Rectangle(new Point(100, 4), new Point(36, 18));
            Days = new GUIDialogueBox(DaysBounds, new[] { "&" + GameContent.Variables["DaysRemaining"].ToString() });

            ActiveMembers = client.Player.ActiveParty.Members;
            MemberSprites = new List<AnimatedSprite2D>();
            HPIndicators = new List<HeartsIndicator>();
            HungerIndicators = new List<HeartsIndicator>();
            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                Vector2 MemberDrawPos = new Vector2(16 + i * 48, 48);
                Vector2 MemberDrawOffset = new Vector2();
                string SpriteName = ActiveMembers[i].SpriteName;
                AnimatedSprite2D Sprite = new AnimatedSprite2D(SpriteName, new Point(32, 32), MemberDrawPos, MemberDrawOffset);
                Sprite.AddAnimation("Idle", 0, 4, 0.15f);
                Sprite.SetCurrentAnimation("Idle");
                MemberSprites.Add(Sprite);

                HeartsIndicator HP = new HeartsIndicator(ActiveMembers[i].HP, (int)MemberDrawPos.X + 16, (int)MemberDrawPos.Y + 32 + 4);
                HPIndicators.Add(HP);

                HeartsIndicator Meats = new HeartsIndicator(ActiveMembers[i].Hunger, (int)MemberDrawPos.X + 16, (int)MemberDrawPos.Y + 32 + 10, true);
                HungerIndicators.Add(Meats);
            }

            StateMachine = new StateMachine<GameStateFieldMenu>();
            StateMachine.SetNewCurrentState(this, new FieldMenuMain());

            Done = false;
            Quit = false;
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

            StateMachine.Update(this, deltaTime);
            if (Done)
                client.StateMachine.SetNewCurrentState(client, new GameStateField());

            if (Quit)
                client.StateMachine.SetNewCurrentState(client, new GameStateTitle());

        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);
            Food.Draw(spriteBatch, true);
            Money.Draw(spriteBatch, true);
            Days.Draw(spriteBatch, true);

            foreach (AnimatedSprite2D sprite in MemberSprites)
                sprite.Draw(spriteBatch);
            foreach (HeartsIndicator hp in HPIndicators)
                hp.Draw(spriteBatch);
            foreach (HeartsIndicator hunger in HungerIndicators)
                hunger.Draw(spriteBatch);

            StateMachine.Draw(this, spriteBatch);
        }

        public override void Exit(TheParty client)
        {
        }
    }
}
