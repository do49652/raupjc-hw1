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
            Direction = new BallDirection(1, 1);
        }

        /// <summary>
        ///     Defines current ball speed in time.
        /// </summary>
        public float Speed { get; set; }

        public float BumpSpeedIncreaseFactor { get; set; }

        /// <summary>
        ///     Defines ball direction.
        /// </summary>
        public BallDirection Direction { get; set; }
    }
}