using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheParty_v2
{
    // From Pandepic's Monogame Engine

    public class GraphicsGlobals
    {
        public static readonly Point ScreenSize = new Point(240, 135);
        public static readonly Point TileSize = new Point(16, 16);

        public static RenderTarget2D DefaultRenderTarget = null;
        public static int TargetResolutionWidth;
        public static int TargetResolutionHeight;
        public static FixedResolutionTarget CurrentFixedResolutionTarget = null;
        public static GraphicsDevice GraphicsDevice = null;

        public static void Setup(GraphicsDevice graphics)
        {
            GraphicsDevice = graphics;
            //TargetResolutionWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            //TargetResolutionHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }
    }
}
