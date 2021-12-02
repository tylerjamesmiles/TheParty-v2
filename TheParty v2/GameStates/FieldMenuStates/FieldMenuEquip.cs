using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class FieldMenuEquip : State<GameStateFieldMenu>
    {
        Player Player;
        GUIChoice MemberChoice;
        GUIChoiceBox InventoryBox;
        List<GUIBox> EquippedItemBoxes;
        List<GUIText> EquippedItemNames;
        GUIDialogueBox ItemDescription;

        enum State { ChooseMember, ChooseItem };
        State CurrentState;

        public FieldMenuEquip(Player player)
        {
            Player = player;
        }

        public override void Enter(GameStateFieldMenu client)
        {
            GenerateMemberChoice(client);

            GenerateEquippedItemNames(client);

            
            ItemDescription = new GUIDialogueBox(
                GUIDialogueBox.ReallySkinnyTop, 
                new[] { SelectedMemberItemDescr() }, 
                0.01f);

            CurrentState = State.ChooseMember;
        }

        private string SelectedMemberItemDescr()
        {
            Equipment Item = SelectedMember.Equipped;
            string Description = Item.Type + " " + Item.Detail;
            return Description;
        }

        private string SelectedItemDescr()
        {
            Equipment Item = SelectedItem;
            string Description = Item.Type + " " + Item.Detail;
            return Description;
        }

        private void GenerateEquippedItemNames(GameStateFieldMenu client)
        {
            List<Member> ActiveParty = Player.ActiveParty.Members;
            EquippedItemBoxes = new List<GUIBox>();
            EquippedItemNames = new List<GUIText>();
            for (int m = 0; m < ActiveParty.Count; m++)
            {
                Vector2 MemberPos = client.MemberSprites[m].DrawPos;
                Vector2 BoxPos = MemberPos + new Vector2(-4, -18);
                Vector2 BoxSize = new Vector2(44, 20);
                EquippedItemBoxes.Add(new GUIBox(new Rectangle(BoxPos.ToPoint(), BoxSize.ToPoint())));
                Equipment Equipped = ActiveParty[m].Equipped;
                EquippedItemNames.Add(new GUIText(Equipped.Name, BoxPos + new Vector2(5, 5), 40, 0.01f));
            }
        }

        private void GenerateMemberChoice(GameStateFieldMenu client)
        {
            int LastChosen = -1;
            if (MemberChoice != null)
                LastChosen = MemberChoice.CurrentChoiceIdx;
            List<Vector2> MemberSpots = client.MemberSprites.ConvertAll(s => s.DrawPos + new Vector2(8, -16));
            MemberChoice = new GUIChoice(MemberSpots.ToArray());
            if (LastChosen != -1)
                MemberChoice.SetChoice(LastChosen);
        }

        private void GenerateItemChoice(GameStateFieldMenu client)
        {
            List<string> Items = new List<string>(Player.Inventory);
            Items.Add("");
            InventoryBox = new GUIChoiceBox(Items.ToArray(), GUIChoiceBox.Position.BottomRight, 1);
        }

        private Member SelectedMember => Player.ActiveParty.Members[MemberChoice.CurrentChoiceIdx];
        private string SelectedItemName => 
            InventoryBox.CurrentChoice < Player.Inventory.Count ?
                Player.Inventory[InventoryBox.CurrentChoice] : "";
        private Equipment SelectedItem => GameContent.Equipment[SelectedItemName];

        public override void Update(GameStateFieldMenu client, float deltaTime)
        {
            EquippedItemBoxes.ForEach(b => b.Update(deltaTime, true));
            EquippedItemNames.ForEach(n => n.Update(deltaTime, true));
            ItemDescription.Update(deltaTime, true);

            switch (CurrentState)
            {
                case State.ChooseMember:
                    MemberChoice.Update(deltaTime, true);
                    if (MemberChoice.ChoiceUpdatedThisFrame)
                        ItemDescription.SetNewText(SelectedMemberItemDescr());

                    if (MemberChoice.Done)
                    {
                        GenerateEquippedItemNames(client);
                        GenerateItemChoice(client);
                        ItemDescription.SetNewText(SelectedItemDescr());
                        CurrentState = State.ChooseItem;
                    }

                    if (InputManager.JustReleased(Keys.Escape))
                        client.StateMachine.SetNewCurrentState(client, new FieldMenuMain());

                    break;

                case State.ChooseItem:
                    InventoryBox.Update(deltaTime, true);
                    if (InventoryBox.ChoiceUpdatedThisFrame)
                        ItemDescription.SetNewText(SelectedItemDescr());

                    if (InventoryBox.Done)
                    {
                        string OldEquippedItem = SelectedMember.EquippedName;
                        string SelectedItem =
                            InventoryBox.CurrentChoice < Player.Inventory.Count ?
                                Player.Inventory[InventoryBox.CurrentChoice] : "";
                        
                        SelectedMember.EquippedName = SelectedItem;

                        if (SelectedItem != "")
                            Player.Inventory.Remove(SelectedItem);

                        if (OldEquippedItem != "")
                            Player.Inventory.Add(OldEquippedItem);

                        // for some reason need to flush any empty items from inventory anyway
                        Player.Inventory.RemoveAll(i => i == "");

                        GenerateMemberChoice(client);
                        GenerateEquippedItemNames(client);
                        CurrentState = State.ChooseMember;
                    }

                    if (InputManager.JustReleased(Keys.Escape))
                    {
                        GenerateMemberChoice(client);
                        GenerateEquippedItemNames(client);
                        CurrentState = State.ChooseMember;
                    }

                    break;
            }



        }

        public override void Draw(GameStateFieldMenu client, SpriteBatch spriteBatch)
        {
            EquippedItemBoxes.ForEach(b => b.Draw(spriteBatch, true));
            EquippedItemNames.ForEach(n => n.Draw(spriteBatch, true));

            MemberChoice.Draw(spriteBatch, true);

            ItemDescription.Draw(spriteBatch, true);

            if (CurrentState == State.ChooseItem)
                InventoryBox.Draw(spriteBatch, true);
        }


    }
}
