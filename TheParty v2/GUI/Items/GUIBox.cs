using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class GUIBox
    {
        private const string SpriteName = "Box";
        private const float GrowTime = 0.1f;
        private static readonly Point TileSize = new Point(16, 16);

        Rectangle Bounds;

        Vector2 LocSmall;
        Vector2 LocGrown;
        Vector2 SizeSmall;
        Vector2 SizeGrown;
        LerpV GrowLocationLerp;
        LerpV GrowSizeLerp;
        public bool Grown => GrowLocationLerp.Reached && GrowSizeLerp.Reached;
        public bool Done => Grown && Shrink;
        public bool Shrink;

        public GUIBox(Rectangle bounds)
        {
            Bounds = bounds;

            LocSmall = bounds.Center.ToVector2() + new Vector2(-8, -8);
            LocGrown = bounds.Location.ToVector2();
            GrowLocationLerp = new LerpV(LocSmall, LocGrown, GrowTime);

            SizeSmall = new Vector2(16, 16);
            SizeGrown = bounds.Size.ToVector2();
            GrowSizeLerp = new LerpV(SizeSmall, SizeGrown, GrowTime);

            Shrink = false;
        }

        public void StartShrink()
        {
            Shrink = true;
            GrowLocationLerp = new LerpV(LocGrown, LocSmall, GrowTime);
            GrowSizeLerp = new LerpV(SizeGrown, SizeSmall, GrowTime);
        }

        public void Update(float deltaTime, bool isInFocus)
        {
            GrowLocationLerp.Update(deltaTime);
            GrowSizeLerp.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch, bool isInFocus)
        {
            Rectangle DrawBounds = new Rectangle(
                GrowLocationLerp.CurrentPosition.ToPoint(),
                GrowSizeLerp.CurrentPosition.ToPoint());

            // Only draw if box is bigger than tilesize
            if (DrawBounds.Width <= TileSize.X || DrawBounds.Height <= TileSize.Y)
                return;

            // Middle

            Rectangle MSource = new Rectangle(TileSize, TileSize);
            Rectangle MDraw = new Rectangle(DrawBounds.Location + TileSize, DrawBounds.Size - TileSize - TileSize);

            spriteBatch.Draw(GameContent.Sprites[SpriteName], MDraw, MSource, Color.White);
           
            // Sides
            Rectangle TSource = new Rectangle(new Point(TileSize.X, 0), TileSize);
            Rectangle LSource = new Rectangle(new Point(0, TileSize.Y), TileSize);
            Rectangle BSource = new Rectangle(new Point(TileSize.X, TileSize.Y * 2), TileSize);
            Rectangle RSource = new Rectangle(new Point(TileSize.X * 2, TileSize.Y), TileSize);

            Rectangle TDraw = new Rectangle(
                new Point(DrawBounds.Left + TileSize.X, DrawBounds.Top),
                new Point(DrawBounds.Right - TileSize.X - DrawBounds.Left - TileSize.X, TileSize.Y));
            Rectangle LDraw = new Rectangle(
                new Point(DrawBounds.Left, DrawBounds.Top + TileSize.Y),
                new Point(TileSize.X, DrawBounds.Bottom - TileSize.Y - DrawBounds.Top - TileSize.Y));
            Rectangle BDraw = new Rectangle(
                new Point(DrawBounds.Left + TileSize.X, DrawBounds.Bottom - TileSize.Y),
                new Point(DrawBounds.Right - TileSize.X - DrawBounds.Left - TileSize.X, TileSize.Y));
            Rectangle RDraw = new Rectangle(
                new Point(DrawBounds.Right - TileSize.X, DrawBounds.Top + TileSize.Y),
                new Point(TileSize.X, DrawBounds.Bottom - TileSize.Y - DrawBounds.Top - TileSize.Y));

            spriteBatch.Draw(GameContent.Sprites[SpriteName], TDraw, TSource, Color.White);
            spriteBatch.Draw(GameContent.Sprites[SpriteName], LDraw, LSource, Color.White);
            spriteBatch.Draw(GameContent.Sprites[SpriteName], BDraw, BSource, Color.White);
            spriteBatch.Draw(GameContent.Sprites[SpriteName], RDraw, RSource, Color.White);

            // Corners
            Rectangle TLSource = new Rectangle(new Point(0, 0), TileSize);
            Rectangle TRSource = new Rectangle(new Point(TileSize.X * 2, 0), TileSize);
            Rectangle BLSource = new Rectangle(new Point(0, TileSize.Y * 2), TileSize);
            Rectangle BRSource = new Rectangle(new Point(TileSize.X * 2, TileSize.Y * 2), TileSize);

            Rectangle TLDraw = new Rectangle(DrawBounds.Location, TileSize);
            Rectangle TRDraw = new Rectangle(new Point(DrawBounds.Right - TileSize.X, DrawBounds.Top), TileSize);
            Rectangle BLDraw = new Rectangle(new Point(DrawBounds.Left, DrawBounds.Bottom - TileSize.Y), TileSize);
            Rectangle BRDraw = new Rectangle(new Point(DrawBounds.Right - TileSize.X, DrawBounds.Bottom - TileSize.Y), TileSize);

            spriteBatch.Draw(GameContent.Sprites[SpriteName], TLDraw, TLSource, Color.White);
            spriteBatch.Draw(GameContent.Sprites[SpriteName], TRDraw, TRSource, Color.White);
            spriteBatch.Draw(GameContent.Sprites[SpriteName], BLDraw, BLSource, Color.White);
            spriteBatch.Draw(GameContent.Sprites[SpriteName], BRDraw, BRSource, Color.White); 
        }
    }
}
