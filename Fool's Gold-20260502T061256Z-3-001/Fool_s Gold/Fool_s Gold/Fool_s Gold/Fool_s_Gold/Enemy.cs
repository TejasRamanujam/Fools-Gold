using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Fool_s_Gold
{
    class Enemy : AnimatedSprite
    {
        private SpriteEffects flip = SpriteEffects.None;
        private string currentAnim = "Walk";
        private const float MoveSpeed = 128.0f;
        private Rectangle localBounds;
        private Rectangle lineOfSight;
        private Texture2D bullet;
        public String enemyType;

        private int reloadTime;

        // Constructs a new Enemy.
        public Enemy(Level _level, Vector2 _position, string _enemy) : base(15, 17, 6)
        {
            level = _level;
            position = _position;
            isAlive = true;
            isCompletelyDead = false;
            LoadContent(_enemy);
            lineOfSight = new Rectangle((int)position.X + FrameWidth, (int)position.Y, 300, FrameHeight);
            reloadTime = 0;
        }


        // Loads the player sprite sheet.
        public void LoadContent(string _enemy)
        {
            bullet = Level.Content.Load<Texture2D>("Sprites/White Square");

            string enemyString = string.Empty;
            switch (_enemy)
            {
                case "e":
                    enemyString = "Sprites/Enemy/Soldier";
                    hp = 1;
                    break;
                case "E":
                    enemyString = "Sprites/Enemy/Lieutenant 1";
                    hp = 15;
                    break;
                default:
                    enemyString = "Sprites/Enemy/Soldier";
                    break;
            }

            // Load animated textures.
            Texture2D tex = Level.Content.Load<Texture2D>(enemyString);
            if (!SpriteTextures.Contains(tex))
                SpriteTextures.Add(tex);
            if(enemyString.Equals("Sprites/Enemy/Soldier"))
            {
                FrameWidth = 15;
                FrameHeight = 17;
                framesPerRow = 6;

                Animation anim = new Animation();
                anim.LoadAnimation("Walk", 0, new List<int> { 6, 7, 8, 9, 10, 11 }, 3, true);
                SpriteAnimations.Add("Walk", anim);

                anim = new Animation();
                anim.LoadAnimation("Dead", 0, new List<int> { 4 }, 1, false);
                anim.AnimationCallBack(DeadAnimEnd);
                SpriteAnimations.Add("Dead", anim);

                anim = new Animation();
                anim.LoadAnimation("Shoot", 0, new List<int> { 12, 13 }, 2, true);
                SpriteAnimations.Add("Shoot", anim);

                enemyType = "Soldier";
            }
            else
            {
                FrameWidth = 50;
                FrameHeight = 51;
                framesPerRow = 8;
                Animation anim = new Animation();
                anim.LoadAnimation("Walk", 0, new List<int> { 8, 9, 10, 11, 12, 13, 14, 15, 14, 13, 12, 11, 10, 9, 8 }, 8, true);
                SpriteAnimations.Add("Walk", anim);

                anim = new Animation();
                anim.LoadAnimation("Dead", 0, new List<int> { 3 }, 1, false);
                anim.AnimationCallBack(DeadAnimEnd);
                SpriteAnimations.Add("Dead", anim);

                anim = new Animation();
                anim.LoadAnimation("Shoot", 0, new List<int> { 0 }, 1, true);
                SpriteAnimations.Add("Shoot", anim);

                anim = new Animation();
                anim.LoadAnimation("Dash", 0, new List<int> { 16, 16, 16, 16, 17, 17 }, 2, false);
                SpriteAnimations.Add("Dash", anim);

                enemyType = "Lieutenant";
            }

            // Calculate bounds within texture size.
            // subtract 10 from width and height to remove a 5px buffer
            // around the enemy.
            int width = FrameWidth - 10;
            int left = (FrameWidth - width) / 2;
            int height = FrameHeight - 10;
            int top = FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
            SpriteAnimations[currentAnim].ResetPlay();
        }

        // Called when the player has squished us.
        public void OnKilled()
        {
            isAlive = false;
            lineOfSight.Y = -200;
            SpriteAnimations[currentAnim].Stop();
            currentAnim = "Dead";
            SpriteAnimations[currentAnim].ResetPlay();
        }

        // Called when Dead animation is done, letting the level know to remove.
        public void DeadAnimEnd()
        {
            isCompletelyDead = true;
        }

        // Paces back and forth along a platform.
        public void Update(GameTime _gameTime)
        {
            SpriteAnimations[currentAnim].Update(_gameTime);
            if (isAlive)
            {
                if(level.Player.BoundingRectangle.Intersects(lineOfSight) && level.Player.IsAlive)
                {
                    
                    if(reloadTime <= 0)
                    {
                        if (currentAnim != "Shoot")
                        {
                            SpriteAnimations[currentAnim].Stop();
                            currentAnim = "Shoot";
                            SpriteAnimations[currentAnim].ResetPlay();
                        }

                        if (enemyType.Equals("Soldier"))
                        {
                            if (velocity.X < 0)
                            {
                                level.bullets.Add(new Bullet(new Rectangle((int)position.X - 20, (int)position.Y, 10, 10), -1, bullet));
                            }
                            else
                            {
                                level.bullets.Add(new Bullet(new Rectangle((int)position.X + FrameWidth, (int)position.Y, 10, 10), 1, bullet));
                            }
                            reloadTime = 60;
                        }
                        else if(enemyType.Equals("Lieutenant"))
                        {
                            if (velocity.X < 0)
                            {
                                level.bullets.Add(new Bullet(new Rectangle((int)position.X - 20, (int)position.Y - 30, 10, 10), -1, bullet));
                            }
                            else
                            {
                                level.bullets.Add(new Bullet(new Rectangle((int)position.X + FrameWidth, (int)position.Y - 30, 10, 10), 1, bullet));
                            }

                            reloadTime = 45;
                        }
                    }
                    else if(enemyType.Equals("Lieutenant") && position.X > 30 && position.X < 1250)
                    {
                        if (currentAnim != "Dash")
                        {
                            SpriteAnimations[currentAnim].Stop();
                            currentAnim = "Dash";
                            SpriteAnimations[currentAnim].ResetPlay();
                        }

                        if(velocity.X > 0)
                        {
                            velocity = new Vector2(MoveSpeed * 0.08f, 0.0f);
                            position.X = position.X + velocity.X;
                        }
                        else if(velocity.X < 0)
                        {
                            velocity = new Vector2(MoveSpeed * -0.08f, 0.0f);
                            position.X = position.X + velocity.X;
                        }

                        reloadTime = 45;
                    }

                    reloadTime--;
                    
                }
                else
                {
                    if(currentAnim != "Walk")
                    {
                        SpriteAnimations[currentAnim].Stop();
                        currentAnim = "Walk";
                        SpriteAnimations[currentAnim].ResetPlay();
                    }

                    float elapsed = (float)_gameTime.ElapsedGameTime.TotalSeconds;
                    // Calculate tile position based on the side we are walking towards.
                    int direction = 1;
                    if (Velocity.X < 0)
                        direction = -1;
                    float posX = Position.X + localBounds.Width / 2 * direction;
                    int tileX = (int)Math.Floor(posX / Tile.Width) - direction;
                    int tileY = (int)Math.Floor(Position.Y / Tile.Height);

                    // If we are about to run into a wall or off a cliff, reverse direction.
                    if (Level.GetCollision(tileX + direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + direction, tileY + 1) == TileCollision.Passable ||
                    Level.GetCollision(tileX + direction, tileY) == TileCollision.Impassable ||
                    position.X > 1230 - FrameWidth)
                    {
                        direction *= -1;
                    }
                    // Move in the current direction.
                    velocity = new Vector2(direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;

                    //Makes line of sight follow
                    if (direction == 1)
                    {
                        lineOfSight.X = BoundingRectangle.Right;
                    }
                    else
                    {
                        lineOfSight.X = BoundingRectangle.Left - 300;
                    }
                }

                iFrames--;
                
            }
        }


        // Draws the animated enemy.
        public void Draw(GameTime _gameTime, SpriteBatch _spriteBatch)
        {
            Rectangle source = GetFrameRectangle(SpriteAnimations[currentAnim].FrameToDraw);
            // Flip the sprite to face the way it is moving.
            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X > 0)
                flip = SpriteEffects.None;

            // Draw the enemy.
            if(enemyType.Equals("Soldier"))
            {
                _spriteBatch.Draw(SpriteTextures[0], position, source, Color.Gray, 0.0f, Origin, 5.0f, flip, 0.0f);
            }
            else if(enemyType.Equals("Lieutenant"))
            {
                if (currentAnim == "Dash")
                    FrameWidth = 70;
                else
                    FrameWidth = 50;

                _spriteBatch.Draw(SpriteTextures[0], new Vector2(position.X, position.Y - 25), source, new Color(255, (int)(255 * (hp / 15.0)), (int)(255 * (hp / 15.0))), 0.0f, Origin, 2.5f, flip, 0.0f);
            }

            //_spriteBatch.Draw(bullet, lineOfSight, Color.OrangeRed);
        }



        // Is the enemy alive, or dead?
        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Is enemy completely dead, if so, flag for removal from level
        public bool IsCompletelyDead
        {
            get { return isCompletelyDead; }
        }
        bool isCompletelyDead;

        // What level are we on, used for collision and interaction
        // with the level's entities.
        public Level Level
        {
            get { return level; }
        }
        Level level;

        // What position the enemy is in the world
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        // The enemy's movement velocity
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Gets a rectangle which bounds this enemy in world space.
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - Origin.Y) + localBounds.Y;
                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public int IFrames
        {
            get { return iFrames; }
            set { iFrames = value; }
        }
        int iFrames;

        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }
        int hp;
    }

}
