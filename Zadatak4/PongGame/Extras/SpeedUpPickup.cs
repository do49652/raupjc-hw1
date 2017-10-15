using System;
using System.Timers;
using Zadatak4.PongGame;

namespace Zadatak4.Extras
{
    public class SpeedUpPickup : Pickup
    {
        private Pong _pong;
        private int _repeat;
        private Timer _timer;

        public SpeedUpPickup(int width, int height, float x, float y) : base(width, height, x, y)
        {
            _repeat = 0;
        }

        public override void Activate(Pong pong)
        {
            _pong = pong;
            foreach (var ball in _pong.Ball)
                ball.Speed = 0.15f;
            _repeat = 10;

            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = 1;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _timer.Interval = 1000;
            if ((int.Parse(_pong.Timer) != 0 && int.Parse(_pong.Timer) < _repeat) || _pong.Timer.Equals("-1"))
            {
                _timer.Stop();
                if (int.Parse(_pong.Timer) == -1)
                    _pong.Timer = "0";
                return;
            }
            _pong.Timer = _repeat.ToString();
            if (_repeat == 0)
            {
                foreach (var ball in _pong.Ball)
                    ball.Speed = GameConstants.DefaultMaxBallSpeed;
                _timer.Stop();
            }
            _repeat--;
        }
    }
}