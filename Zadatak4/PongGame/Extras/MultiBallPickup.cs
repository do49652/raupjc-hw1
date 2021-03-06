﻿using System.Diagnostics;
using System.Timers;
using Zadatak4.PongGame;

namespace Zadatak4.Extras
{
    public class MultiBallPickup : Pickup
    {
        private Pong _pong;

        public MultiBallPickup(int width, int height, float x, float y) : base(width, height, x, y)
        {
        }

        public override void Activate(Pong pong)
        {
            _pong = pong;

            var newBall1 = new Ball(GameConstants.DefaultBallSize, GameConstants.DefaultInitialBallSpeed,
                GameConstants.DefaultBallBumpSpeedIncreaseFactor)
            {
                X = _pong.Ball.GetElement(0).X,
                Y = _pong.Ball.GetElement(0).Y,
                Texture = _pong.BallTexture,
                Direction = new BallDirection(_pong.Ball.GetElement(0).Direction)
            };
            newBall1.Direction.invertY();

            _pong.Ball.Add(newBall1);
        }
    }
}