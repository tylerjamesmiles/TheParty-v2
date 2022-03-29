using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{

    // From Pandepic's Monogame Engine

    public class FixedResolutionTarget
    {
        protected int _targetWidth;             // Width of the target
        protected int _targetHeight;            // Height of the target
        protected GraphicsDevice _graphics;     // Local pointer to Graphics
        protected RenderTarget2D _renderTarget; // Local pointer
        protected Rectangle _targetRect;        // Rectangle created for target
        protected bool _pixelPerfect = false;   // Is the target pixel-perfect?

        public static float PixelPerfectScale { get; private set; }
        public static Vector2 Offset { get; private set; }

        public Rectangle TargetRect { get => _targetRect; }

        public FixedResolutionTarget(int width, int height, GraphicsDevice graphics, bool pixelPerfect = false)
        {
            _graphics = graphics;           // cache pointer to Graphics
            _pixelPerfect = pixelPerfect;   // set isPixelPerfect pointer

            SetSize(width, height);
        }

        // Destructor!
        ~FixedResolutionTarget()
        {
            _renderTarget?.Dispose();
        }

        public void SetSize(int width, int height)
        {
            _targetWidth = width;
            _targetHeight = height;

            _renderTarget?.Dispose();       // frees resources in renderTarget - look at later
            _renderTarget = new RenderTarget2D(_graphics, _targetWidth, _targetHeight);

            // why set to the render target, only to set it to null later?
            _graphics.SetRenderTarget(_renderTarget);   // assign rendertarget to _graphics
            _graphics.Clear(Color.Transparent);         // clear _graphics - i wonder why?
            _graphics.SetRenderTarget(null);

            // 
            var windowWidth = _graphics.PresentationParameters.BackBufferWidth;
            var windowHeight = _graphics.PresentationParameters.BackBufferHeight;

            _targetRect.Width = _targetWidth;
            _targetRect.Height = _targetHeight;

            if (!_pixelPerfect)
            {
                var resolutionScaleX = (float)windowWidth / (float)_targetWidth;
                var resolutionScaleY = (float)windowHeight / (float)_targetHeight;
                var scale = Math.Min(resolutionScaleX, resolutionScaleY);

                _targetRect.Width = (int)((float)_targetWidth * scale);
                _targetRect.Height = (int)((float)_targetHeight * scale);
            }
            else
            {
                // Ratio of window width to window height
                float windowAspectRatio = (float)windowWidth / (float)windowHeight;
                float targetAspectRatio = (float)_targetWidth / (float)_targetHeight;

                // Used to scale as big as we're able to
                float pixelPerfectScale = 1;

                if (targetAspectRatio > windowAspectRatio)
                    pixelPerfectScale = (float)windowWidth / (float)_targetWidth;
                else
                    pixelPerfectScale = (float)windowHeight / (float)_targetHeight;

                if (pixelPerfectScale == 0)
                    pixelPerfectScale = 1;

                PixelPerfectScale = pixelPerfectScale;

                _targetRect.Width = (int)((float)_targetWidth * (float)pixelPerfectScale);
                _targetRect.Height = (int)((float)_targetHeight * (float)pixelPerfectScale);
            }

            var offset = new Vector2();

            if (_targetRect.Width < windowWidth)
                offset.X = windowWidth / 2 - _targetRect.Width / 2;
            if (_targetRect.Height < windowHeight)
                offset.Y = windowHeight / 2 - _targetRect.Height / 2;

            Offset = offset;

            _targetRect.Location = offset.ToPoint();
        }

        public void Enable()
        {
            GraphicsGlobals.TargetResolutionWidth = _targetWidth;
            GraphicsGlobals.TargetResolutionHeight = _targetHeight;
            GraphicsGlobals.CurrentFixedResolutionTarget = this;
        }

        public void Disable()
        {
            GraphicsGlobals.TargetResolutionWidth = _graphics.PresentationParameters.BackBufferWidth;
            GraphicsGlobals.TargetResolutionHeight = _graphics.PresentationParameters.BackBufferHeight;
            GraphicsGlobals.CurrentFixedResolutionTarget = null;
        }

        public void Begin()
        {
            _graphics.SetRenderTarget(null);
            _graphics.Clear(Color.Black);
            GraphicsGlobals.DefaultRenderTarget = _renderTarget;
            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(Color.Black);
        }

        public void End()
        {
            GraphicsGlobals.DefaultRenderTarget = null;
            _graphics.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Draw(_renderTarget, _targetRect, null, Color.White);
            spriteBatch.End();

        }
    }
}
