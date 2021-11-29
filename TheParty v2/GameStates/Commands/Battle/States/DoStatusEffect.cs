using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class DoStatusEffect : State<CommandBattle>
    {
        Timer WaitTimer;
        bool DoneShowingStances;
        Timer PostTimer;
        int HighestNumEffects;
        int CurrentEffectIdx;

        public override void Enter(CommandBattle client)
        {
            WaitTimer = new Timer(0.8f);
            PostTimer = new Timer(0.01f);
            DoneShowingStances = false;

            HighestNumEffects = int.MinValue;
            foreach (Member member in client.CurrentStore.AllMembers())
                if (member.StatusEffects.Count > HighestNumEffects)
                    HighestNumEffects = member.StatusEffects.Count;

            if (HighestNumEffects == 0)
                client.StateMachine.SetNewCurrentState(client, new PreMoveChecks());
            else
                UpdateStanceAnimations(client);
        }

        private void UpdateStanceAnimations(CommandBattle client)
        {
            List<Member> AllMembers = client.CurrentStore.AllMembers();
            foreach (Member member in AllMembers)
            {
                var Effects = member.StatusEffects;
                if (CurrentEffectIdx < Effects.Count)
                {
                    StatusEffect CurrentEffect = Effects[CurrentEffectIdx];
                    if (CurrentEffect.SpriteAnimation != "")
                    {
                        AnimatedSprite2D Sprite = client.Sprites[AllMembers.IndexOf(member)];
                        Sprite.SetCurrentAnimation(CurrentEffect.SpriteAnimation);
                        CurrentEffect.Do(client.CurrentStore, member);
                    }
                }
            }

            CurrentEffectIdx++;

            if (CurrentEffectIdx > HighestNumEffects)
            {
                client.SetAppropriateAnimations();
                DoneShowingStances = true;

            }
        }

        public override void Update(CommandBattle client, float deltaTime)
        {
            if (DoneShowingStances)
            {
                PostTimer.Update(deltaTime);
                if (PostTimer.TicThisFrame)
                {
                    // Move forward in time
                    foreach (Member member in client.CurrentStore.AllMembers())
                    {
                        member.StatusEffects.ForEach(se => se.DecrementTurnsRemaining());
                        member.StatusEffects.RemoveAll(se => se.NumTurnsRemaining == 0);
                    }

                    client.StateMachine.SetNewCurrentState(client, new PreMoveChecks());
                }
            }
            else
            {
                WaitTimer.Update(deltaTime);
                if (WaitTimer.TicThisFrame)
                {
                    UpdateStanceAnimations(client);
                }
            }
        }

        public override void Draw(CommandBattle client, SpriteBatch spriteBatch)
        {

        }
    }
}
