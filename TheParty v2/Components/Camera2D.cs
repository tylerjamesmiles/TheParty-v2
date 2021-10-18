using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class Camera2D
    {
        public Vector2 Position;

        public void Update(Point mapSize, Vector2 targetPos)
        {
            Vector2 ScreenSize = GraphicsGlobals.ScreenSize.ToVector2();
            Vector2 HalfScreenSize = ScreenSize / 2f;
            Point CameraLimitTL = new Point(0, 0);
            Point CameraLimitSize = mapSize - ScreenSize.ToPoint();
            Rectangle CameraLimitRect = new Rectangle(CameraLimitTL, CameraLimitSize);

            Vector2 UnlimitedCameraPos = targetPos - HalfScreenSize;
            Vector2 LimitedCameraPos = Utility.KeptInRect(UnlimitedCameraPos, CameraLimitRect);

            Position = LimitedCameraPos;
        }
    }
}
