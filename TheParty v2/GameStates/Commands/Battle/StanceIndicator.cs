using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class StanceParticle
    {
        public Vector2 Pos;
        public Vector2 Vel;
        float Mass;
        float MaxSpeed;
        float MaxForce;
        public Vector2 Target;
        public bool FastMode;

        public StanceParticle(Vector2 startPos, Vector2 startVel)
        {
            Vector2 Dir = new Vector2(
                -1 + (float)new Random().NextDouble() * 2, 
                -1 + (float)new Random().NextDouble() * 2);
            Pos = startPos + Dir * (new Random().Next(3) + 1);

            if (startVel == Vector2.Zero)
                Vel = Dir * MaxSpeed;
            else
                Vel = startVel;

            Target = startPos;
        }

        public void Update(float deltaTime)
        {
            if (FastMode)
            {
                MaxSpeed = 200f;
                MaxForce = 200f;
                Mass = 0.05f;
            }
            else
            {
                MaxSpeed = 40f;
                MaxForce = 40f;
                Mass = 0.2f;
            }


            Vector2 ToTarget = Target - Pos;
            Vector2 Steering = Vector2.Normalize(ToTarget) * MaxForce;
            Vector2 Accel = Steering / Mass;
            Vel += Accel * deltaTime;
            if (Vel.LengthSquared() > MaxSpeed * MaxSpeed)
                Vel = Vector2.Normalize(Vel) * MaxSpeed;
            Pos += Vel * deltaTime;
            if (Vel.LengthSquared() < 0.001f)
                Vel = Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(
                GameContent.Sprites["Commitment"],
                new Rectangle(Pos.ToPoint() - new Point(2, 2), new Point(5, 5)),
                new Rectangle(new Point(0, 0), new Point(5, 5)),
                Color.White);
        }
    }

    class StanceIndicator
    {
        public int CurrentCommitment;
        public Vector2 DrawPos;
        bool DissappearOnHitTarget;
        public bool ParticleHitThisFrame;
        public bool AllParticlesHit;

        public List<StanceParticle> CommitmentParticles;
    
        public StanceIndicator(int startStance, Vector2 targetPos)
        {
            CurrentCommitment = startStance;

            CommitmentParticles = new List<StanceParticle>();
            for (int i = 0; i < startStance; i++)
                CommitmentParticles.Add(new StanceParticle(targetPos, Vector2.Zero));

            SetAllTarget(targetPos);

            DissappearOnHitTarget = false;
            AllParticlesHit = false;
        }

        public StanceIndicator(Vector2 targetPos, params StanceIndicator[] sis)
        {
            CommitmentParticles = new List<StanceParticle>();
            foreach (StanceIndicator si in sis)
            {
                CurrentCommitment += si.CurrentCommitment;
                foreach (StanceParticle p in si.CommitmentParticles)
                {
                    CommitmentParticles.Add(new StanceParticle(p.Pos, p.Vel / 2));
                }
            }

            SetAllTarget(targetPos);
            DissappearOnHitTarget = true;
            AllParticlesHit = false;
        }

        public void SetAllTarget(Vector2 newTarget)
        {
            DrawPos = newTarget;
            CommitmentParticles.ForEach(p => p.Target = DrawPos);
        }

        public void SetOneTarget(Vector2 newTarget)
        {
            foreach (StanceParticle p in CommitmentParticles)
            {
                if (p.Target != newTarget)
                {
                    p.Target = newTarget;
                    p.FastMode = true;
                    break;
                }
            }
        }

        public void Update(float deltaTime)
        {
            CommitmentParticles.ForEach(p => p.Update(deltaTime));

            if (DissappearOnHitTarget)
            {
                ParticleHitThisFrame = false;

                for (int i = 0; i < CommitmentParticles.Count; i++)
                {
                    StanceParticle p = CommitmentParticles[i];
                    if (p.FastMode &&
                        (p.Pos - p.Target).LengthSquared() < 20f)
                    {
                        CommitmentParticles.RemoveAt(i);
                        ParticleHitThisFrame = true;
                    }
                }

                if (CommitmentParticles.Count == 0)
                    AllParticlesHit = true;
            }
        }

        public void SetCommitment(int newStance)
        {
            if (newStance < CurrentCommitment)
                CommitmentParticles.Clear();
            else
                for (int i = 0; i < newStance - CurrentCommitment; i++)
                    CommitmentParticles.Add(new StanceParticle(DrawPos, Vector2.Zero));

            CurrentCommitment = newStance;
        }

        public void ToggleFastMode(bool setting) => CommitmentParticles.ForEach(p => p.FastMode = setting);

        public void Draw(SpriteBatch spriteBatch)
        {
            CommitmentParticles.ForEach(p => p.Draw(spriteBatch));
        }
    }

}
