using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheParty_v2
{
    class CommandAddPartyMember : Command<TheParty>
    {
        string Name;
        enum State { InitialChoice, MemberChoice, Done };
        State CurrentState;
        GUIChoiceBox InitialChoice;
        GUIChoiceBox MemberChoice;

        public CommandAddPartyMember(string name)
        {
            Name = name;
        }

        public override void Enter(TheParty client)
        {
            if (client.Player.ActiveParty.NumMembers == 3)
            {
                InitialChoice = new GUIChoiceBox(new[] { "Send " + Name + " to camp.", "Send someone else to camp." }, GUIChoiceBox.Position.Center);
                string[] MemberNames = client.Player.ActiveParty.Members.ConvertAll(m => "Send " + m.Name + " to camp.").ToArray();
                MemberChoice = new GUIChoiceBox(MemberNames, GUIChoiceBox.Position.Center);
                CurrentState = State.InitialChoice;
            }
            else
                CurrentState = State.Done;

            Entered = true;
        }

        public override void Update(TheParty client, float deltaTime)
        {
            switch (CurrentState)
            {
                case State.InitialChoice:
                    InitialChoice.Update(deltaTime, true);
                    if (InitialChoice.Done)
                    {
                        switch (InitialChoice.CurrentChoice)
                        {
                            case 0:
                                client.Player.CampMembers.Add(GameContent.Members[Name].DeepCopy());
                                Done = true;
                                break;

                            case 1:
                                CurrentState = State.MemberChoice;
                                break;
                        }
                    }
                    break;

                case State.MemberChoice:
                    MemberChoice.Update(deltaTime, true);
                    if (MemberChoice.Done)
                    {
                        List<Member> ActiveMembers = client.Player.ActiveParty.Members;
                        Member Chosen = ActiveMembers[MemberChoice.CurrentChoice];
                        ActiveMembers.Remove(Chosen);
                        client.Player.CampMembers.Add(Chosen);
                        ActiveMembers.Add(GameContent.Members[Name].DeepCopy());
                        Done = true;
                    }
                    break;
                case State.Done:
                    client.Player.ActiveParty.Members.Add(GameContent.Members[Name].DeepCopy());
                    Done = true;
                    break;
            }
        }

        public override void Draw(TheParty client, SpriteBatch spriteBatch)
        {
            if (CurrentState != State.Done)
            {
                spriteBatch.Draw(GameContent.Sprites["Black"], new Rectangle(new Point(0, 0), new Point(160, 144)), Color.White);
                InitialChoice.Draw(spriteBatch, true);
                MemberChoice.Draw(spriteBatch, true);
            }

        }
    }

    class CommandRemovePartyMember : Command<TheParty>
    {
        string Name;
        public CommandRemovePartyMember(string name)
        {
            Name = name;
        }
        public override void Update(TheParty client, float deltaTime)
        {
            List<Member> Members = client.Player.ActiveParty.Members;
            Member ToRemove = Members.Find(m => m.Name == Name);
            Members.Remove(ToRemove);
            Done = true;
        }
    }
}
