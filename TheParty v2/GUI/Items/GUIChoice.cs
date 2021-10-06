using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TheParty_v2
{
    class GUIChoice
    {
        Vector2[] ChoicePositions;
        public int CurrentChoiceIdx { get; private set; }
        public int NumChoices => ChoicePositions.Length;
        public bool ChoiceUpdatedThisFrame { get; private set; }
        int[][] Edges;
        LerpV MovementLerp;
        Wobble HandWobble;

        public bool Done;
        static readonly Vector2 HandOffset = new Vector2(-20, 0);
        const float HandTravelTime = 0.05f;
        const int InvalidNode = -1;

        public GUIChoice(Vector2[] choicePositions)
        {
            ChoicePositions = choicePositions;
            CurrentChoiceIdx = 0;
            Edges = PositionEdges(choicePositions);
            HandWobble = new Wobble(4f, 2f);
            Done = false;
            MovementLerp = new LerpV(ChoicePositions[0], ChoicePositions[0], 0f);
            ChoiceUpdatedThisFrame = false;
        }

        public static int[][] PositionEdges(Vector2[] positions)
        {
            int[][] Result = new int[positions.Length][];

            for (int i = 0; i < Result.Length; i++)
                Result[i] = new int[] { InvalidNode, InvalidNode, InvalidNode, InvalidNode };

            Func<Vector2, Vector2, int, bool> IsVec2InDirection = (vec1, vec2, dir) =>
                dir == 0 ? vec2.Y < vec1.Y :
                dir == 1 ? vec2.Y > vec1.Y :
                dir == 2 ? vec2.X < vec1.X :
                dir == 3 ? vec2.X > vec1.X :
                false;

            Func<Vector2, Vector2, int, float> AmtVec2InDirection = (vec1, vec2, dir) =>
                dir == 0 ? Vector2.Dot(Vector2.Normalize(vec2 - vec1), new Vector2(0, -1)) :
                dir == 1 ? Vector2.Dot(Vector2.Normalize(vec2 - vec1), new Vector2(0, +1)) :
                dir == 2 ? Vector2.Dot(Vector2.Normalize(vec2 - vec1), new Vector2(-1, 0)) :
                dir == 3 ? Vector2.Dot(Vector2.Normalize(vec2 - vec1), new Vector2(+1, 0)) :
                0;

            Func<Vector2, Vector2, float> AmtVec2CloseTo = (vec1, vec2) =>
                1f - ((vec2 - vec1).Length() / 50f)  ;

            Func<Vector2, Vector2, int, float> ScoreVec2 = (vec1, vec2, dir) =>
                AmtVec2CloseTo(vec1, vec2) +
                AmtVec2InDirection(vec1, vec2, dir);

            for (int posIdx = 0; posIdx < positions.Length; posIdx++)
            {
                Vector2 FromPos = positions[posIdx];

                for (int dir = 0; dir < 4; dir++)
                {
                    for (int otherPosIdx = 0; otherPosIdx < positions.Length; otherPosIdx++)
                    {
                        if (otherPosIdx == posIdx)
                            continue;

                        int CurrentEdgeTo = Result[posIdx][dir];

                        if (CurrentEdgeTo == -1)
                        {
                            Result[posIdx][dir] = otherPosIdx;
                            continue;
                        }

                        Vector2 CurrentToPos = positions[CurrentEdgeTo];
                        Vector2 PotentialToPos = positions[otherPosIdx];

                        if (!IsVec2InDirection(FromPos, PotentialToPos, dir))
                            continue;

                        float CurrentToScore = ScoreVec2(FromPos, CurrentToPos, dir);
                        float PotentialToScore = ScoreVec2(FromPos, PotentialToPos, dir);

                        if (PotentialToScore > CurrentToScore)
                        {
                            Result[posIdx][dir] = otherPosIdx;
                        }
                    }
                }
            }

            return Result;
        }

        public void Update(float deltaTime, bool isInFocus, bool onLegalChoice = true)
        {
            int NoDirectionPressed = -1;
            int Direction =
                InputManager.JustPressed(Keys.W) ? 0 :
                InputManager.JustPressed(Keys.S) ? 1 :
                InputManager.JustPressed(Keys.A) ? 2 :
                InputManager.JustPressed(Keys.D) ? 3 :
                NoDirectionPressed;

            bool DirectionWasPressed = Direction != NoDirectionPressed;
            int NewChoiceIdx = DirectionWasPressed ? Edges[CurrentChoiceIdx][Direction] : -1;
            bool ChoiceIsValid = NewChoiceIdx != InvalidNode;
            ChoiceUpdatedThisFrame = DirectionWasPressed && ChoiceIsValid;
            Vector2 NewChoicePos = ChoiceUpdatedThisFrame ? ChoicePositions[NewChoiceIdx] : Vector2.Zero;

            if (ChoiceUpdatedThisFrame)
            {
                MovementLerp = new LerpV(MovementLerp.CurrentPosition, NewChoicePos, HandTravelTime);
                CurrentChoiceIdx = NewChoiceIdx;
            }

            MovementLerp.Update(deltaTime);
            HandWobble.Update(deltaTime);

            if (isInFocus && InputManager.JustPressed(Keys.Space) && onLegalChoice)
                Done = true;
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            Vector2 DrawPos =
                MovementLerp.CurrentPosition +
                HandOffset +
                new Vector2(HandWobble.CurrentPosition, 0);

            spriteBatch.Draw(
                GameContent.Sprites["Cursor"],
                new Rectangle(DrawPos.ToPoint(), new Point(16, 16)),
                new Rectangle(new Point(0, 0), new Point(16, 16)),
                Color.White);
        }

    }
}
