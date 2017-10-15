using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Zadatak3;

namespace Zadatak4
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Pong : Game
    {
        private readonly GraphicsDeviceManager graphics;

        private readonly IGenericList<Sprite> SpritesForDrawList = new GenericList<Sprite>();
        private SpriteBatch spriteBatch;

        public Pong()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 900,
                PreferredBackBufferWidth = 500
            };
            Content.RootDirectory = "Content";
        }

        public Paddle PaddleBottom { get; private set; }
        public Paddle PaddleTop { get; private set; }
        public Ball Ball { get; private set; }
        public Background Background { get; private set; }

        public SoundEffect HitSound { get; private set; }
        public Song Music { get; private set; }

        public List<Wall> Walls { get; set; }
        public List<Wall> Goals { get; set; }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        /// </summary>
        protected override void Initialize()
        {
            var screenBounds = GraphicsDevice.Viewport.Bounds;

            PaddleBottom = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight,
                GameConstants.PaddleDefaulSpeed);
            PaddleBottom.X = screenBounds.Width / 2f - PaddleBottom.Width / 2f;
            PaddleBottom.Y = screenBounds.Bottom - PaddleBottom.Height;

            PaddleTop = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight,
                GameConstants.PaddleDefaulSpeed);
            PaddleTop.X = screenBounds.Width / 2f - PaddleTop.Width / 2f;
            PaddleTop.Y = screenBounds.Top;

            Ball = new Ball(GameConstants.DefaultBallSize, GameConstants.DefaultInitialBallSpeed,
                GameConstants.DefaultBallBumpSpeedIncreaseFactor);
            Ball.X = screenBounds.Width / 2f - Ball.Width / 2f;
            Ball.Y = screenBounds.Height / 2f - Ball.Height / 2f;

            Background = new Background(screenBounds.Width, screenBounds.Height);

            SpritesForDrawList.Add(Background);
            SpritesForDrawList.Add(PaddleBottom);
            SpritesForDrawList.Add(PaddleTop);
            SpritesForDrawList.Add(Ball);

            Walls = new List<Wall>
            {
                new Wall(-GameConstants.WallDefaultSize, 0, GameConstants.WallDefaultSize, screenBounds.Height),
                new Wall(screenBounds.Right, 0, GameConstants.WallDefaultSize, screenBounds.Height)
            };

            Goals = new List<Wall>
            {
                new Wall(0, screenBounds.Height, screenBounds.Width, GameConstants.WallDefaultSize),
                new Wall(screenBounds.Top, -GameConstants.WallDefaultSize, screenBounds.Width,
                    GameConstants.WallDefaultSize)
            };

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var paddleTexture = Content.Load<Texture2D>("paddle");
            PaddleBottom.Texture = paddleTexture;
            PaddleTop.Texture = paddleTexture;
            Ball.Texture = Content.Load<Texture2D>("ball");
            Background.Texture = Content.Load<Texture2D>("background");

            HitSound = Content.Load<SoundEffect>("hit");
            Music = Content.Load<Song>("music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Music);
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
                PaddleTop.X -= (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (touchState.IsKeyDown(Keys.D))
                PaddleTop.X += (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);

            //Limit paddle position
            PaddleBottom.X =
                MathHelper.Clamp(PaddleBottom.X, screenBounds.Left, screenBounds.Right - PaddleBottom.Width);
            PaddleTop.X = MathHelper.Clamp(PaddleTop.X, screenBounds.Left, screenBounds.Right - PaddleTop.Width);

            //Limit ball speed
            Ball.Speed = MathHelper.Clamp(Ball.Speed, GameConstants.DefaultInitialBallSpeed,
                GameConstants.DefaultMaxBallSpeed);

            //Ball movement
            var ballPositionChange = Ball.Direction * (float)(gameTime.ElapsedGameTime.TotalMilliseconds * Ball.Speed);
            Ball.X += ballPositionChange.X;
            Ball.Y += ballPositionChange.Y;

            // Ball hitting side walls
            if (CollisionDetector.Overlaps(Ball, Walls.ElementAt(0)) ||
                CollisionDetector.Overlaps(Ball, Walls.ElementAt(1)))
            {
                HitSound.Play(0.2f, 0, 0);
                Ball.Direction.invertX();
                Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
            }

            // Ball hitting goals
            if (CollisionDetector.Overlaps(Ball, Goals.ElementAt(0)) ||
                CollisionDetector.Overlaps(Ball, Goals.ElementAt(1)))
            {
                Ball.Speed = GameConstants.DefaultInitialBallSpeed;
                Ball.X = screenBounds.Width / 2f - Ball.Width / 2f;
                Ball.Y = screenBounds.Height / 2f - Ball.Height / 2f;
            }

            // Ball hitting paddles
            if (CollisionDetector.Overlaps(Ball, PaddleTop) || CollisionDetector.Overlaps(Ball, PaddleBottom))
            {
                HitSound.Play(0.2f, 0, 0);
                Ball.Direction.invertY();
                Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
            }

            //Ball outside of the screen
            if (!new Rectangle(-GameConstants.WallDefaultSize, -GameConstants.WallDefaultSize,
                    screenBounds.Width + GameConstants.WallDefaultSize,
                    screenBounds.Height + GameConstants.WallDefaultSize)
                .Intersects(new Rectangle((int)Ball.X, (int)Ball.Y, Ball.Width, Ball.Height)))
            {
                Ball.Speed = GameConstants.DefaultInitialBallSpeed;
                Ball.X = screenBounds.Width / 2f - Ball.Width / 2f;
                Ball.Y = screenBounds.Height / 2f - Ball.Height / 2f;
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            for (var i = 0; i < SpritesForDrawList.Count; i++)
                SpritesForDrawList.GetElement(i).DrawSpriteOnScreen(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}