using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Zadatak3;

namespace Zadatak4
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Pong : Game
    {
        private readonly GraphicsDeviceManager graphics;

        /// <summary>
        ///     Generic list that holds Sprites that should be drawn on screen
        /// </summary>
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

        /// <summary>
        ///     Bottom paddle object
        /// </summary>
        public Paddle PaddleBottom { get; private set; }

        /// <summary>
        ///     Top paddle object
        /// </summary>
        public Paddle PaddleTop { get; private set; }

        /// <summary>
        ///     Ball object
        /// </summary>
        public Ball Ball { get; private set; }

        /// <summary>
        ///     Background image
        /// </summary>
        public Background Background { get; private set; }

        /// <summary>
        ///     Sound when ball hits an obstacle .
        ///     SoundEffect is a type defined in Monogame framework
        /// </summary>
        public SoundEffect HitSound { get; private set; }

        /// <summary>
        ///     Background music.Song is a type defined in Monogame framework
        /// </summary>
        //public Song Music { get; private set; }

        public List<Wall> Walls { get; set; }

        public List<Wall> Goals { get; set; }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
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
            Ball.X = 100000 + screenBounds.Width / 2f - Ball.Width / 2f;
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
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize new SpriteBatch object which will be used to draw textures .
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Set textures
            var paddleTexture = Content.Load<Texture2D>("paddle");
            PaddleBottom.Texture = paddleTexture;
            PaddleTop.Texture = paddleTexture;
            Ball.Texture = Content.Load<Texture2D>("ball");
            Background.Texture = Content.Load<Texture2D>("background");

            // Load sounds
            // Start background music
            HitSound = Content.Load<SoundEffect>("hit");
            //Music = Content.Load<Song>("music");
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Music);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var screenBounds = GraphicsDevice.Viewport.Bounds;

            var touchState = Keyboard.GetState();
            if (touchState.IsKeyDown(Keys.Left))
                PaddleBottom.X -= (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (touchState.IsKeyDown(Keys.Right))
                PaddleBottom.X += (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);

            if (touchState.IsKeyDown(Keys.A))
                PaddleTop.X -= (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (touchState.IsKeyDown(Keys.D))
                PaddleTop.X += (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);

            PaddleBottom.X =
                MathHelper.Clamp(PaddleBottom.X, screenBounds.Left,
                    screenBounds.Right - PaddleBottom.Width);
            PaddleTop.X = MathHelper.Clamp(PaddleTop.X, screenBounds.Left,
                screenBounds.Right - PaddleTop.Width);

            Ball.Speed =
                MathHelper.Clamp(Ball.Speed, GameConstants.DefaultInitialBallSpeed, GameConstants.DefaultMaxBallSpeed);

            var ballPositionChange = Ball.Direction * (float)(gameTime.ElapsedGameTime.TotalMilliseconds * Ball.Speed);
            Ball.X += ballPositionChange.X;
            Ball.Y += ballPositionChange.Y;

            // Ball - side walls
            if (CollisionDetector.Overlaps(Ball, Walls.ElementAt(0)) ||
                CollisionDetector.Overlaps(Ball, Walls.ElementAt(1)))
            {
                Ball.Direction = new Vector2(-Ball.Direction.X, Ball.Direction.Y);
                Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
            }

            // Ball - goals
            if (CollisionDetector.Overlaps(Ball, Goals.ElementAt(0)) ||
                CollisionDetector.Overlaps(Ball, Goals.ElementAt(1)))
            {
                Ball.Speed = GameConstants.DefaultInitialBallSpeed;
                Ball.BumpSpeedIncreaseFactor = GameConstants.DefaultBallBumpSpeedIncreaseFactor;
                Ball.X = screenBounds.Width / 2f - Ball.Width / 2f;
                Ball.Y = screenBounds.Height / 2f - Ball.Height / 2f;
            }

            // Ball - paddles
            if (CollisionDetector.Overlaps(Ball, PaddleTop) || CollisionDetector.Overlaps(Ball, PaddleBottom))
            {
                Ball.Direction = new Vector2(Ball.Direction.X, -Ball.Direction.Y);
                Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
            }

            //Ball - outside of the screen
            if (!new Rectangle(-GameConstants.WallDefaultSize, -GameConstants.WallDefaultSize,
                screenBounds.Width + GameConstants.WallDefaultSize,
                screenBounds.Height + GameConstants.WallDefaultSize).Intersects(new Rectangle((int)Ball.X,
                (int)Ball.Y, Ball.Width, Ball.Height)))
            {
                Ball.Speed = GameConstants.DefaultInitialBallSpeed;
                Ball.BumpSpeedIncreaseFactor = GameConstants.DefaultBallBumpSpeedIncreaseFactor;
                Ball.X = screenBounds.Width / 2f - Ball.Width / 2f;
                Ball.Y = screenBounds.Height / 2f - Ball.Height / 2f;
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Start drawing .
            spriteBatch.Begin();
            for (var i = 0; i < SpritesForDrawList.Count; i++)
                SpritesForDrawList.GetElement(i).DrawSpriteOnScreen(spriteBatch);

            // End drawing .
            // Send all gathered details to the graphic card in one batch .
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}