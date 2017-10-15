using Zadatak4.PongGame;

namespace Zadatak4.Extras
{
    public abstract class Pickup : Sprite
    {
        protected Pickup(int width, int height, float x, float y) : base(width, height, x, y)
        {
        }

        public abstract void Activate(Pong pong);
    }
}