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

        public GUIDialogueBox FoodMoneyDays;

        public List<Member> ActiveMembers;
        public List<Member> BackupMembers;
        public List<GUIBox> MemberBoxes;
        public List<AnimatedSprite2D> MemberSprites;
        public List<GUIText> MemberNames;
        public List<HeartsIndicator> HPIndicators;
        public List<HeartsIndicator> HungerIndicators;
        public List<HeartsIndicator> StanceIndicators;

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
            MemberBoxes = new List<GUIBox>();
            MemberSprites = new List<AnimatedSprite2D>();
            MemberNames = new List<GUIText>();
            HPIndicators = new List<HeartsIndicator>();
            HungerIndicators = new List<HeartsIndicator>();
            StanceIndicators = new List<HeartsIndicator>();

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                Member Member = ActiveMembers[i];

                int BoxWidth = 90;
                int BoxHeight = 36;
                int BoxL = 4 + (i / 3) * (BoxWidth- 1);
                int BoxR = 8 + (i % 3) * (BoxHeight - 1);
                Point BoxTL = new Point(BoxL, BoxR);
                Point BoxSize = new Point(BoxWidth - 4, BoxHeight);
                MemberBoxes.Add(new GUIBox(new Rectangle(BoxTL, BoxSize)));

                string Name = Member.Name;
                GUIText MemberName = new GUIText(Name, (BoxTL + new Point(32, 4)).ToVector2(), 160, 0.05f);
                MemberNames.Add(MemberName);

                Vector2 MemberDrawPos = BoxTL.ToVector2() + new Vector2(0, 2);
                Vector2 MemberDrawOffset = new Vector2();
                string SpriteName = Member.BattleSpriteName;
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
                    BoxTL.X + 32, 
                    BoxTL.Y + BoxSize.Y - 5 - 11,
                    HeartsIndicator.Type.Hearts, true, ActiveMembers[i].MaxHP, false);
                HPIndicators.Add(HP);

                HeartsIndicator Meats = new HeartsIndicator(
                    ActiveMembers[i].Hunger, 
                    BoxTL.X + 32, 
                    BoxTL.Y + BoxSize.Y - 5 - 5, 
                    HeartsIndicator.Type.Meats, true, ActiveMembers[i].MaxHunger, false);
                HungerIndicators.Add(Meats);

                HeartsIndicator Stance = new HeartsIndicator(
                    ActiveMembers[i].Stance,
                   BoxTL.X + 32,
                   BoxTL.Y + BoxSize.Y - 5 - 17,
                   HeartsIndicator.Type.Commitment, true, 5, false);
                StanceIndicators.Add(Stance);

                PreviousMainMenuChoice = 0;
            }
        }

        public override void Enter(TheParty client)
        {
            Rectangle BottomBoxBounds = new Rectangle(
                new Point(GraphicsGlobals.ScreenSize.X - 58, GraphicsGlobals.ScreenSize.Y - 48),
                new Point(54, 38));

            FoodMoneyDays = new GUIDialogueBox(BottomBoxBounds, 
                new[] { 
                    "\"" + GameContent.Variables["FoodSupply"].ToString() + "\n" +
                    "$" + GameContent.Variables["Money"].ToString() + "\n" +
                    "&" + GameContent.Variables["DaysRemaining"].ToString()
                });

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
            FoodMoneyDays.Update(deltaTime, true);
            MemberBoxes.ForEach(mb => mb.Update(deltaTime, true));
            MemberSprites.ForEach(s => s.Update(deltaTime));
            MemberNames.ForEach(mn => mn.Update(deltaTime, true));
            HPIndicators.ForEach(hp => hp.Update(deltaTime));
            HungerIndicators.ForEach(hi => hi.Update(deltaTime));
            StanceIndicators.ForEach(si => si.Update(deltaTime));


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
            FoodMoneyDays.Draw(spriteBatch, true);

            for (int i = 0; i < ActiveMembers.Count; i++)
            {
                MemberBoxes[i].Draw(spriteBatch, true);
                MemberSprites[i].Draw(spriteBatch);
                MemberNames[i].Draw(spriteBatch, true);

                if (ActiveMembers[i].HP > 0)
                {
                    HPIndicators[i].Draw(spriteBatch, Vector2.Zero);
                    HungerIndicators[i].Draw(spriteBatch, Vector2.Zero);
                    StanceIndicators[i].Draw(spriteBatch, Vector2.Zero);
                }
            }

            StateMachine.Draw(this, spriteBatch);
        }
    }
}
