using Microsoft.Xna.Framework;

namespace Zadatak4
{
    public class CollisionDetector
    {
        public static bool Overlaps(IPhysicalObject2D a, IPhysicalObject2D b)
        {
            Rectangle rA = new Rectangle((int)a.X, (int)a.Y, a.Width, a.Height);
            Rectangle rB = new Rectangle((int)b.X, (int)b.Y, b.Width, b.Height);

            if (rA.Intersects(rB))
                return true;

            return false;
        }
    }
}