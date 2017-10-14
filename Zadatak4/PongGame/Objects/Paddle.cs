using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zadatak4
{
    /// <summary>
    ///     Represents player paddle .
    /// </summary>
    public class Paddle : Sprite
    {
        public Paddle(int width, int height, float initialSpeed) : base(width, height)
        {
            Speed = initialSpeed;
        }

        /// <summary>
        ///     Current paddle speed in time
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Overriding draw method . Masking paddle texture with black color .
        /// </summary>
        public override void DrawSpriteOnScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Vector2(X, Y), new Rectangle(0, 0, Width, Height), Color.GhostWhite);
        }
    }
}