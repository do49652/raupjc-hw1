using Microsoft.Xna.Framework;
using Zadatak3;

namespace Zadatak4.Extras
{
    public class PaddleAI
    {
        public static int Move(Paddle paddle, GenericList<Ball> balls)
        {
            var clostestBall = balls.GetElement(0);
            var distance = Vector2.Distance(new Vector2(paddle.X, paddle.Y),
                new Vector2(clostestBall.X, clostestBall.Y));

            foreach (var ball in balls)
            {
                var d = Vector2.Distance(new Vector2(paddle.X, paddle.Y), new Vector2(ball.X, ball.Y));
                if (!(distance > d)) continue;
                distance = d;
                clostestBall = ball;
            }

            return (paddle.X + paddle.Width / 2 > clostestBall.X) ? -1 : 1;
        }
    }
}