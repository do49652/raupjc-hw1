using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Zadatak3;
using Zadatak4.Extras;

namespace Zadatak4.PongGame
{
    public class Pong : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private Texture2D _multiBallPickupTexture;
        private Texture2D _speedPickupTexture;
        private SpriteBatch _spriteBatch;
        private bool _paddleAi;

        public Pong()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 900,
                PreferredBackBufferWidth = 500
            };
            Content.RootDirectory = "Content";
        }

        public Texture2D BallTexture { get; set; }
        public Score Score { get; set; }
        public string Timer { get; set; }

        public Paddle PaddleBottom { get; private set; }
        public Paddle PaddleTop { get; private set; }
        public GenericList<Ball> Ball { get; set; }
        public Background Background { get; private set; }

        public SoundEffect HitSound { get; private set; }
        public Song Music { get; private set; }

        public GenericList<Wall> Walls { get; set; }
        public GenericList<Wall> Goals { get; set; }

        public SpriteFont Font { get; set; }

        public GenericList<Pickup> Pickups { get; set; }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        /// </summary>
        protected override void Initialize()
        {
            var screenBounds = GraphicsDevice.Viewport.Bounds;

            PaddleBottom = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight,
                GameConstants.PaddleDefaulSpeed)
            {
                X = screenBounds.Width / 2f - GameConstants.PaddleDefaultWidth / 2f,
                Y = screenBounds.Bottom - GameConstants.PaddleDefaulHeight
            };
            _paddleAi = true;

            PaddleTop = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight,
                GameConstants.PaddleDefaulSpeed)
            {
                X = screenBounds.Width / 2f - GameConstants.PaddleDefaultWidth / 2f,
                Y = screenBounds.Top
            };

            Ball = new GenericList<Ball>
            {
                new Ball(GameConstants.DefaultBallSize, GameConstants.DefaultInitialBallSpeed,
                    GameConstants.DefaultBallBumpSpeedIncreaseFactor)
                {
                    X = screenBounds.Width / 2f - GameConstants.DefaultBallSize / 2f,
                    Y = screenBounds.Height / 2f - GameConstants.DefaultBallSize / 2f
                }
            };

            Background = new Background(screenBounds.Width, screenBounds.Height);

            Walls = new GenericList<Wall>
            {
                new Wall(-GameConstants.WallDefaultSize, 0, GameConstants.WallDefaultSize, screenBounds.Height),
                new Wall(screenBounds.Right, 0, GameConstants.WallDefaultSize, screenBounds.Height)
            };

            Goals = new GenericList<Wall>
            {
                new Wall(0, screenBounds.Height, screenBounds.Width, GameConstants.WallDefaultSize),
                new Wall(screenBounds.Top, -GameConstants.WallDefaultSize, screenBounds.Width,
                    GameConstants.WallDefaultSize)
            };

            Score = new Score();
            Timer = "0";
            Pickups = new GenericList<Pickup>();

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var paddleTexture = Content.Load<Texture2D>("paddle");
            PaddleBottom.Texture = paddleTexture;
            PaddleTop.Texture = paddleTexture;

            BallTexture = Content.Load<Texture2D>("ball");
            Ball.GetElement(0).Texture = BallTexture;

            Background.Texture = Content.Load<Texture2D>("background");

            _speedPickupTexture = Content.Load<Texture2D>("speedPickup");
            _multiBallPickupTexture = Content.Load<Texture2D>("multiBallPickup");

            HitSound = Content.Load<SoundEffect>("hit");

            Music = Content.Load<Song>("music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Music);

            Font = Content.Load<SpriteFont>("font");
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var screenBounds = GraphicsDevice.Viewport.Bounds;
            var touchState = Keyboard.GetState();

            //Down paddle control
            if (touchState.IsKeyDown(Keys.Left))
                PaddleBottom.X -= (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (touchState.IsKeyDown(Keys.Right))
                PaddleBottom.X += (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);

            //Top paddle control
            if (touchState.IsKeyDown(Keys.A))
            {
                PaddleTop.X -= (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
                _paddleAi = false;
            }
            if (touchState.IsKeyDown(Keys.D))
            {
                PaddleTop.X += (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
                _paddleAi = false;
            }

            if (_paddleAi)
                PaddleTop.X = MathHelper.Lerp(PaddleTop.X, PaddleTop.X + PaddleAI.Move(PaddleTop, Ball) * (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds), Ball.GetElement(0).Speed > 1 ? 1 : (Ball.GetElement(0).Speed / 1.2f));

            //Limit paddle position
            PaddleBottom.X =
                MathHelper.Clamp(PaddleBottom.X, screenBounds.Left, screenBounds.Right - PaddleBottom.Width);
            PaddleTop.X = MathHelper.Clamp(PaddleTop.X, screenBounds.Left, screenBounds.Right - PaddleTop.Width);

            foreach (var ball in Ball)
            {
                //Ball movement
                var ballPositionChange =
                    ball.Direction * (float)(gameTime.ElapsedGameTime.TotalMilliseconds * ball.Speed);
                ball.X += ballPositionChange.X;
                ball.Y += ballPositionChange.Y;

                // Ball hitting side walls
                if (CollisionDetector.Overlaps(ball, Walls.GetElement(0)) ||
                    CollisionDetector.Overlaps(ball, Walls.GetElement(1)))
                {
                    HitSound.Play(0.2f, 0, 0);
                    ball.Direction.invertX();
                    if (ball.Speed < GameConstants.DefaultMaxBallSpeed)
                        ball.Speed *= ball.BumpSpeedIncreaseFactor;
                }

                // Ball hitting goals
                if (CollisionDetector.Overlaps(ball, Goals.GetElement(0)))
                {
                    Score.Player1++;
                    if (Ball.Count == 1) ResetBall();
                    else Ball.Remove(ball);
                }
                if (CollisionDetector.Overlaps(ball, Goals.GetElement(1)))
                {
                    Score.Player2++;
                    if (Ball.Count == 1) ResetBall();
                    else Ball.Remove(ball);
                }

                // Ball hitting paddles
                if (CollisionDetector.Overlaps(ball, PaddleTop) || CollisionDetector.Overlaps(ball, PaddleBottom))
                {
                    HitSound.Play(0.2f, 0, 0);
                    ball.Direction.invertY();
                    if (ball.Speed < GameConstants.DefaultMaxBallSpeed)
                        ball.Speed *= ball.BumpSpeedIncreaseFactor;

                    if (ball == Ball.GetElement(0))
                        SpawnPickups();
                }

                if (Timer.Equals("0"))
                    foreach (var pickup in Pickups)
                    {
                        if (!CollisionDetector.Overlaps(ball, pickup)) continue;
                        pickup.Activate(this);
                        Pickups.Remove(pickup);
                    }
            }

            //Ball outside of the screen
            if (!new Rectangle(-GameConstants.WallDefaultSize, -GameConstants.WallDefaultSize,
                    screenBounds.Width + GameConstants.WallDefaultSize,
                    screenBounds.Height + GameConstants.WallDefaultSize)
                .Intersects(new Rectangle((int)Ball.GetElement(0).X, (int)Ball.GetElement(0).Y,
                    Ball.GetElement(0).Width, Ball.GetElement(0).Height)))
                ResetBall();

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            var screenBounds = GraphicsDevice.Viewport.Bounds;

            _spriteBatch.Begin();

            Background.DrawSpriteOnScreen(_spriteBatch);

            for (var i = 0; i < Pickups.Count; i++)
                Pickups.GetElement(i).DrawSpriteOnScreen(_spriteBatch);

            PaddleTop.DrawSpriteOnScreen(_spriteBatch);
            PaddleBottom.DrawSpriteOnScreen(_spriteBatch);

            for (var i = 0; i < Ball.Count; i++)
                Ball.GetElement(i).DrawSpriteOnScreen(_spriteBatch);

            if (int.Parse(Timer) > 0)
                _spriteBatch.DrawString(Font, Timer, new Vector2(0, Font.MeasureString(Timer).Y), Color.Black);

            _spriteBatch.DrawString(Font, Score.Player1.ToString(),
                new Vector2(screenBounds.Width - Font.MeasureString(Score.Player1.ToString()).X,
                    screenBounds.Height / 2f - Font.MeasureString(Score.Player1.ToString()).Y), Color.Black);
            _spriteBatch.DrawString(Font, Score.Player2.ToString(),
                new Vector2(screenBounds.Width - Font.MeasureString(Score.Player2.ToString()).X,
                    screenBounds.Height / 2f), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ResetBall()
        {
            var screenBounds = GraphicsDevice.Viewport.Bounds;
            Ball.GetElement(0).Speed = GameConstants.DefaultInitialBallSpeed;
            Ball.GetElement(0).X = screenBounds.Width / 2f - Ball.GetElement(0).Width / 2f;
            Ball.GetElement(0).Y = screenBounds.Height / 2f - Ball.GetElement(0).Height / 2f;
            Timer = "-1";
        }

        private void SpawnPickups()
        {
            if (Timer == "-1")
                Timer = "0";

            var screenBounds = GraphicsDevice.Viewport.Bounds;
            var rnd = new Random();
            if (Pickups.Count > 5 || rnd.Next(0, 10) > 3) return;

            Pickup[] pickups =
            {
                new SpeedUpPickup(GameConstants.DefaultBallSize,
                    GameConstants.DefaultBallSize,
                    rnd.Next(GameConstants.WallDefaultSize, screenBounds.Width - GameConstants.WallDefaultSize),
                    rnd.Next(GameConstants.WallDefaultSize, screenBounds.Height - GameConstants.WallDefaultSize))
                {
                    Texture = _speedPickupTexture
                },
                new MultiBallPickup(GameConstants.DefaultBallSize,
                    GameConstants.DefaultBallSize,
                    rnd.Next(GameConstants.WallDefaultSize, screenBounds.Width - GameConstants.WallDefaultSize),
                    rnd.Next(GameConstants.WallDefaultSize, screenBounds.Height - GameConstants.WallDefaultSize))
                {
                    Texture = _multiBallPickupTexture
                }
            };

            Pickups.Add(pickups[rnd.Next(0, pickups.Length)]);
        }
    }
}