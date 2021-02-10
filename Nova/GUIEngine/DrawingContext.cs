using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.GUIEngine.Base;
using Nova.GUIEngine.Units;

namespace Nova.GUIEngine
{
    public class DrawingContext
    {
        public GraphicsDevice GraphicsDevice { get; set; }

        public SpriteBatch SpriteBatch { get; set; }
        
        public int ViewportWidth => GraphicsDevice.Viewport.Width;
        public int ViewportHeight => GraphicsDevice.Viewport.Height;
        public float Scale { get; set; }

        /// <summary>
        /// Called before every visual draw, the context updates the Visual being drawn. If we can't move anything, in the future it might be
        ///  handy TODO: only calculate it once and only do it again if the elements update/viewport changes.
        ///
        /// Method calculates based on USize, UVector and most importantly UDim. Everything is based off of the parent if any, otherwise screen.
        /// </summary>
        /// <param name="visual">Visual being drawn.</param>
        public void InitializeVisualDraw(Visual visual)
        {
            Visual parent = visual.Parent;

            // Get screen bounds
            var visualPosition = visual.Position;
            UVector2 actualPosition;
            var visualSize = visual.Size;
            USize actualSize;

            float screenWidth = ViewportWidth;
            float screenHeight = ViewportHeight;

            if (parent == null)
            {

                // Sizing based on parent null which means screen resolution
                if (visualSize.IsAbsolute())
                {
                    actualSize = visualSize;
                }
                else
                {
                    var width = visualSize.Width.Scale * screenWidth + visualSize.Width.Offset;
                    var height = visualSize.Height.Scale * screenHeight + visualSize.Height.Offset;
                    
                    actualSize = new USize(UDim.Absolute(width), UDim.Absolute(height));
                }

                // Positioning based on parent null which means screen resolution
                if (visualPosition.IsAbsolute())
                {
                    // This is easy, it's a top level absolute
                    actualPosition = visualPosition;
                }
                else
                {
                    var xDim = visualPosition.X;
                    var yDim = visualPosition.Y;

                    var x = xDim.Scale * screenWidth + xDim.Offset;
                    var y = yDim.Scale * screenHeight + yDim.Offset;
                    
                    actualPosition = new UVector2(UDim.Absolute(x), UDim.Absolute(y));
                }
            }
            else
            {
                // Positioning based on parent
                var parentActualPosition = parent.ActualPosition;
                var parentActualSize = parent.ActualSize;

                if (visualPosition.IsAbsolute())
                {
                    // Child is using absolute positioning _under_ the parents area.
                    actualPosition = parentActualPosition + visualPosition;
                }
                else
                {
                    var xDim = visualPosition.X;
                    var yDim = visualPosition.Y;

                    var x = parentActualPosition.X.Absolute() + xDim.Scale * parentActualSize.Width.Absolute() + xDim.Offset;
                    var y = parentActualPosition.Y.Absolute() + yDim.Scale * parentActualSize.Height.Absolute() + yDim.Offset;

                    actualPosition = new UVector2(UDim.Absolute(x), UDim.Absolute(y));
                }

                // Sizing based on parent
                if (visualSize.IsAbsolute())
                {
                    // Child using absolute sizing, in this case it doesn't really matter..children should never do this though because 
                    //  if a child is 200 pixels wide but the parent is 0.2 (relative) wide the child could be larger than the parent.
                    actualSize = visualSize;
                }
                else
                {
                    var width = visualSize.Width.Scale * parentActualSize.Width.Absolute() + visualSize.Width.Offset;
                    var height = visualSize.Height.Scale * parentActualSize.Height.Absolute() + visualSize.Height.Offset;

                    actualSize = new USize(UDim.Absolute(width), UDim.Absolute(height));
                }
            }

            visual.ActualPosition = actualPosition;
            visual.ActualSize = actualSize;
        }

        /// <summary>
        /// Helper methods to make drawing uniform. If it ever changes, we only have to do it here.
        /// </summary>
        public void Draw(Sprite sprite, UVector2 position)
        {
            sprite.Draw(SpriteBatch, position.ToVector2());
        }

        /// <summary>
        /// Helper methods to make drawing uniform. If it ever changes, we only have to do it here.
        /// </summary>
        public void DrawString(SpriteFont font, string text, UVector2 position)
        {
            SpriteBatch.DrawString(font, text, position.ToVector2(), Color.White);
        }

        /// <summary>
        /// Helper methods to make drawing uniform. If it ever changes, we only have to do it here.
        /// Note: The color parameter doesn't actually specify it's color. It does some tinting
        ///  because of the way SpriteFonts work.
        /// </summary>
        public void DrawString(SpriteFont font, string text, UVector2 position, Color color)
        {
            SpriteBatch.DrawString(font, text, position.ToVector2(), color);
        }
    }
}
