namespace Zadatak4
{
    public class BallDirection
    {
        private float _x;
        private float _y;

        public BallDirection(BallDirection a)
        {
            if (a._x > 0) X = 1;
            else X = -1;

            if (a._y > 0) Y = 1;
            else Y = -1;
        }

        public BallDirection(float x, float y)
        {
            if (x > 0) X = 1;
            else X = -1;

            if (y > 0) Y = 1;
            else Y = -1;
        }

        public float X
        {
            get => _x;
            set
            {
                if (value > 0) _x = 1;
                else _x = -1;
            }
        }

        public float Y
        {
            get => _y;
            set
            {
                if (value > 0) _y = 1;
                else _y = -1;
            }
        }

        public void invertX()
        {
            X = -X;
        }

        public void invertY()
        {
            Y = -Y;
        }

        public static BallDirection operator *(BallDirection a, BallDirection b)
        {
            return new BallDirection(a.X * b.X, a.Y * b.Y);
        }

        public static BallDirection operator *(BallDirection a, float b)
        {
            var bDir = new BallDirection(a.X * b, a.Y * b);
            bDir._x = a.X * b;
            bDir._y = a.Y * b;
            return bDir;
        }
    }
}