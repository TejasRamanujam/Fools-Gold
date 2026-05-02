using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Fool_s_Gold
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level[] levels = new Level[11];
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


            //Frame rate differs between platforms
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            backgroundRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            oceanBackground = Content.Load<Texture2D>("Sprites/Background/Ocean background");
            plainsBackground = Content.Load<Texture2D>("Sprites/Background/Plains backgroud");
            forestBackground = Content.Load<Texture2D>("Sprites/Background/forest-background");
            scoreFont = Content.Load<SpriteFont>("Fonts/ScoreFont");

            levels[0] = new Level(Services, @"Content/Levels/Level" + 2 + ".txt");
            levels[1] = new Level(Services, @"Content/Levels/Level" + 2 + ".txt");
            levels[2] = new Level(Services, @"Content/Levels/Level" + 2 + ".txt");

            for (int x = 2; x < levels.Length; x++)
            {
                levels[x] = new Level(Services, @"Content/Levels/Level" + 2 + ".txt");
                
            }

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            levels[currentLevel].Update(gameTime);

            if(currentLevel < levels.Length && levels[currentLevel].Player.Position.X >= 1280 && levels[currentLevel].enemiesClear())
            {
                currentLevel++;
                if (currentLevel > 0)
                {
                    levels[currentLevel].Player.Hp = levels[currentLevel - 1].Player.Hp;
                }
            }

            base.Update(gameTime);
        }

        private int getScore()
        {
            int score = 0;
            foreach (Level level in levels)
            {
                score += level.score;
            }

            return score;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (currentLevel == 4 || currentLevel == 8 || currentLevel == 11)
            {
                GraphicsDevice.Clear(Color.DimGray);
                
            }
            else if(currentLevel < 4)
            {
                //GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Draw(oceanBackground, backgroundRect, Color.White);

            }
            else if(currentLevel < 8)
            {
                //GraphicsDevice.Clear(Color.LightGreen);
                spriteBatch.Draw(plainsBackground, backgroundRect, Color.White);
            }
            else
            {
                //GraphicsDevice.Clear(Color.OrangeRed);
                spriteBatch.Draw(forestBackground, backgroundRect, Color.White);
            }
            
            levels[currentLevel].Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(scoreFont, "Score: "+ getScore().ToString(), new Vector2(1100, 20), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
