using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fool_s_Gold
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level[] levels = new Level[3];
        int currentLevel = 0;

        Rectangle backgroundRect;
        Texture2D oceanBackground;
        Texture2D plainsBackground;
        Texture2D forestBackground;
        SpriteFont scoreFont;

        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 1248;
        private const int BackBufferHeight = 720;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
        }

        protected override void Initialize()
        {
            backgroundRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            oceanBackground = Content.Load<Texture2D>("Sprites/Background/Ocean background");
            plainsBackground = Content.Load<Texture2D>("Sprites/Background/Plains backgroud");
            forestBackground = Content.Load<Texture2D>("Sprites/Background/forest-background");
            scoreFont = Content.Load<SpriteFont>("Fonts/ScoreFont");

            levels[0] = new Level(Services, @"Content/Levels/Level0.txt");
            levels[1] = new Level(Services, @"Content/Levels/Level1.txt");
            levels[2] = new Level(Services, @"Content/Levels/Level2.txt");
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            levels[currentLevel].Update(gameTime);

            if (currentLevel < levels.Length - 1 && levels[currentLevel].Player.Position.X >= 1280 && levels[currentLevel].enemiesClear())
            {
                currentLevel++;
                levels[currentLevel].Player.Hp = levels[currentLevel - 1].Player.Hp;
            }

            base.Update(gameTime);
        }

        private int getScore()
        {
            int score = 0;
            foreach (Level level in levels)
                score += level.score;
            return score;
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (currentLevel == 2)
    spriteBatch.Draw(oceanBackground, backgroundRect, Color.White);
else if (currentLevel == 1)
    spriteBatch.Draw(plainsBackground, backgroundRect, Color.White);
else
    spriteBatch.Draw(forestBackground, backgroundRect, Color.White);

            levels[currentLevel].Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(scoreFont, "Score: " + getScore().ToString(), new Vector2(1100, 20), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}