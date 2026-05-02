using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fool_s_Gold
{
    class Collectible : AnimatedSprite
    {
        private string currentAnim = "Idle";
        private Rectangle localBounds;
        private Color colorOffset;

        // Constructs a new Collectible.
        public Collectible(Level _level, Vector2 _position, string _collectible) : base(64, 64, 1)
        {
            level = _level;
            position = _position;
            isAlive = true;
            LoadContent(_collectible);
        }

        // Loads the player sprite sheet.
        public void LoadContent(string _collectible)
        {
            string sheetString = string.Empty;
            switch (_collectible)
            {
                case "s":
                    sheetString = "Sprites/Collectables/Coin sprite";
                    colorOffset = Color.White;
                    break;
                case "S":
                    sheetString = "Sprites/Collectables/Coin sprite";
                    colorOffset = Color.White;
                    break;
                default:
                    sheetString = "Sprites/Collectables/Coin sprite";
                    colorOffset = Color.White;
                    break;
            }

            // Load animated textures.
            Texture2D tex = Level.Content.Load<Texture2D>("Sprites/Collectables/Coin sprite");
            if (!SpriteTextures.Contains(tex))
                SpriteTextures.Add(tex);

            Animation anim = new Animation();
            anim.LoadAnimation("Idle", 0, new List<int> { 0 }, 16, true);
            SpriteAnimations.Add("Idle", anim);

            // Calculate bounds within texture size.
            // subtract 4 from height to remove a 2px buffer
            // around the collectible.
            int width = FrameWidth;
            int left = (FrameWidth - width) / 2;
            int height = FrameHeight - 4;
            int top = FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
            SpriteAnimations[currentAnim].ResetPlay();
        }

        // Called when the player has collected us.
        public void OnKilled()
        {
            isAlive = false;
        }

        // Animate the collectible
        public void Update(GameTime _gameTime)
        {
            SpriteAnimations[currentAnim].Update(_gameTime);
        }

        // Draws the animated enemy.
        public void Draw(GameTime _gameTime, SpriteBatch _spriteBatch)
        {
            Rectangle source = GetFrameRectangle(SpriteAnimations[currentAnim].FrameToDraw);

            // Draw the enemy.
            _spriteBatch.Draw(SpriteTextures[0], position, source, colorOffset, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
        }


        // Is the collectible alive, or has been collected?
        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // What level are we on, used for collision and interaction
        // with the level's entities.
        public Level Level
        {
            get { return level; }
        }
        Level level;

        // What position the collectible is in the world
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        // Gets a rectangle which bounds this collectible in world space.
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - Origin.Y) + localBounds.Y;
                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }
    }

}
