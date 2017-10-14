using Microsoft.Xna.Framework;

namespace Zadatak4
{
    /// <summary>
    ///     Game ball object representation
    /// </summary>
    public class Ball : Sprite
    {
        public Ball(int size, float speed, float defaultBallBumpSpeedIncreaseFactor) : base(size, size)
        {
            Speed = speed;
            BumpSpeedIncreaseFactor = defaultBallBumpSpeedIncreaseFactor;
            // Initial direction
            Direction = new Vector2(1, 1);
        }

        /// <summary>
        ///     Defines current ball speed in time .
        /// </summary>
        public float Speed { get; set; }

        public float BumpSpeedIncreaseFactor { get; set; }

        /// <summary>
        ///     Defines ball direction .
        ///     Valid values ( -1 , -1) , (1 ,1) , (1 , -1) , ( -1 ,1).
        ///     Using Vector2 to simplify game calculation . Potentially
        ///     dangerous because vector 2 can swallow other values as well .
        ///     OPTIONAL TODO : create your own , more suitable type
        /// </summary>
        public Vector2 Direction { get; set; }
    }
}