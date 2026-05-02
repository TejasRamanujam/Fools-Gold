using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fool_s_Gold
{
    class Player : AnimatedSprite
    {
        //Constructs a new player

        public Player(Level _level, Vector2 _position) : base(13, 21, 8)
        {
            level = _level;
            LoadContent();
            Reset(_position);

            hp = 3;
            dashTimer = 0;
        }

        private string currentAnim = "Idle";
        private SpriteEffects flip = SpriteEffects.None;

        private Texture2D swordTexture;
        public Rectangle swordRect;

        // Loads the player sprite sheet and sounds.
        public void LoadContent()
        {

            swordTexture = level.Content.Load<Texture2D>("Sprites/White Square");

            // Load animated textures.
            SpriteTextures.Add(Level.Content.Load<Texture2D>("Sprites/Player/idle_side"));
            SpriteTextures.Add(Level.Content.Load<Texture2D>("Sprites/Player/attack_side"));
            SpriteTextures.Add(Level.Content.Load<Texture2D>("Sprites/Player/death_side"));
            SpriteTextures.Add(Level.Content.Load<Texture2D>("Sprites/Player/run_side"));

            Animation anim = new Animation();
            anim.LoadAnimation("Idle", 0, new List<int> { 0, 1, 2, 3, 0, 1, 2, 3, 0}, 3, true);
            SpriteAnimations.Add("Idle", anim);

            anim = new Animation();
            anim.LoadAnimation("Attacking", 1, new List<int> { 0, 0, 0, 0, 1, 1, 2, 2, 2 }, 14, false);
            SpriteAnimations.Add("Attacking", anim);           

            anim = new Animation();
            anim.LoadAnimation("Dead", 2, new List<int> { 0, 1, 2, 3, 4}, 2, false);
            anim.AnimationCallBack(DeadAnimEnd);
            SpriteAnimations.Add("Dead", anim);

            anim = new Animation();
            anim.LoadAnimation("Walking", 3, new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 }, 8, true);
            SpriteAnimations.Add("Walking", anim);


            // Calculate bounds within texture size.
            // subtract 4 from width and height to remove a 2px buffer
            // around the player.

            /*int width = FrameWidth - 4;
            int left = (FrameWidth - width) / 2;
            int height = FrameHeight - 4;
            int top = FrameHeight - height;*/

            int width = 48;
            int left = (FrameWidth - width) / 2;
            int height = 72;
            int top = 0;


            localBounds = new Rectangle(left, top, width, height);

        }


        // Resets the player to life.
        // parameter: The position to come to life at.
        public void Reset(Vector2 _position)
        {
            hp = 3;
            dashTimer = 0;
            Position = _position;
            Velocity = Vector2.Zero;
            isAlive = true;
            isCompletelyDead = false;
            SpriteAnimations[currentAnim].Stop();
            currentAnim = "Idle";
            SpriteAnimations[currentAnim].ResetPlay();

            swordRect = new Rectangle((int)_position.X + 60, (int)_position.Y - 30, 130, 72);
        }



        //What level are we on, used for collision and interaction with the level's entities.
        public Level Level
        {
            get { return level; }
        }
        Level level;

        public int Hp
        {
            get { return hp;  }
            set { hp = value; }
        }
        int hp;

        private int dashTimer;

        //Is the player alive or dead?
        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;  

        //What position the player is in the world
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        //the player's movement velocity
        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
            }
        }
        Vector2 velocity;

        //Constants for controlling horizontal movement
        private const float MoveAcceleration = 14000.0f;
        private const float MaxMoveSpeed = 2000.0f;
        private const float GroundDragFactor = 0.58f;
        private const float AirDragFactor = 0.65f;

        //Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -4000.0f;
        private const float GravityAcceleration = 3500.0f;
        private const float MaxFallSpeed = 600.0f;
        private const float JumpControlPower = 0.14f;

        //Input configuration
        private const float MoveStickScale = 1.0f;
        private const Buttons JumpButton = Buttons.A;

        //Gets whether or not the player is on the ground.
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        //Current user movement input
        private float movement;

        //Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        //Attacking state
        private double attackTime = 0;
        public bool isAttacking;

        private Rectangle localBounds;
        //Gets a rectangle which bounds the player in world space.
        public Rectangle BoundingRectangle
        {
            get
            {
                FrameWidth = 13;
                Vector2 origin = Origin;

                if(currentAnim == "Attacking")
                {
                    FrameWidth = 49;
                }

                int left = (int)Math.Round(Position.X - origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        // Gets player horizontal movement and jump commands from input.
        private void GetInput()
        {
            if (!isAlive)
                return;

            // Get input state.
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Get analog horizontal movement.
            movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movement = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                keyboardState.IsKeyDown(Keys.Right) ||
                keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
            }

            if(position.X > 30 && position.X < 1210 && keyboardState.IsKeyDown(Keys.LeftShift) && dashTimer < 0)
            {
                position.X = position.X + new Vector2(movement * 80f, 0f).X;
                dashTimer = 60;
            }

            
            // Check if the player wants to jump.
            isJumping =
                gamePadState.IsButtonDown(JumpButton) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);

            isAttacking = keyboardState.IsKeyDown(Keys.Space);

            if (isAttacking && currentAnim != "Attacking")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Attacking";
                SpriteAnimations[currentAnim].ResetPlay();
            }
            else if (movement != 0  && currentAnim != "Walking" && currentAnim != "Attacking")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Walking";
                SpriteAnimations[currentAnim].ResetPlay();
            }
            else if (movement == 0 && currentAnim == "Walking" && currentAnim != "Attacking")
            {
                SpriteAnimations[currentAnim].Stop();
                currentAnim = "Idle";
                SpriteAnimations[currentAnim].ResetPlay();
            }
        }


        // Handles input, performs physics, and animates the player sprite.
        public void Update(GameTime _gameTime)
        {
            if (isAlive)
            {
                GetInput();
            }
            ApplyPhysics(_gameTime);

            //Making sword follow player
            if(velocity.X > 0)
                swordRect.X = (int)position.X + 60;
            else if(velocity.X < 0)
                swordRect.X = (int)position.X - 194;

            swordRect.Y = (int)position.Y - 30;

            SpriteAnimations[currentAnim].Update(_gameTime);
            // Clear input.
            movement = 0.0f;
            isJumping = false;

            dashTimer--;
        }



        //Updates the player's velocity and position based on input, gravity, etc.
        public void ApplyPhysics(GameTime _gameTime)
        {
            if (!isAlive)
                return;
            float elapsed = (float)_gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            //Base velocity is a combination of horizontal movement control and 
            //acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, _gameTime);

            //Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            //Prevent the player from running faster that his top speed.
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            //Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            //If the player is now colliding with the level, separator them.
            HandleCollisions();

            //If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;
            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;

            //Attack
            DoAttack(_gameTime);
        }

        //Calculate the Y velocity account for jumping and animates accordingly
        //During the ascent of a jump, the Y velocity is completely overridden
        //by the power curve. During the descent, gravity takes over.
        //The jump velocity is controlled by the jumpTime field which measures
        //time into the ascent of the current jump.
        //Returns a new Y velocity if beginning or continuing a jump; otherwise, 
        //the existing Y velocity.
        private float DoJump(float _velocityY, GameTime _gameTime)
        {
            //If the player wants to jump
            if (isJumping)
            {
                //Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    jumpTime += (float)_gameTime.ElapsedGameTime.TotalSeconds;
                }

                //If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    //Fully override the vertical velocity with a power curve that gives players 
                    //more control over the top if the jump
                    _velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    //Reach the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                //Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return _velocityY;
        }

        public void JumpAnimEnd()
        {
            currentAnim = "Idle";
            SpriteAnimations[currentAnim].Play();
        }

        public void DoAttack(GameTime _gameTime)
        {
            if (isAttacking || attackTime > 0)
            {
                attackTime++;

                if (attackTime > 30)
                {
                    attackTime = 0;
                    isAttacking = false;
                    SpriteAnimations[currentAnim].Stop();
                    currentAnim = "Idle";
                    SpriteAnimations[currentAnim].ResetPlay();
                }

            }
        }


        //Detects and resolves all collisions between the player and his neighboring tiles.
        //When collision is detected, the player is pushed away along one axis to prevent overlapping.
        //There is some special logic for the Y axis to handle platform for the Y axis to handle 
        //platforms which behave differently depending on direction of movement.
        private void HandleCollisions()
        {
            //Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            int previousBottom = 0;

            //Reset flag to search for ground collision.
            isOnGround = false;

            //For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = 0; x <= rightTile; ++x)
                {
                    //If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable || position.X <= -1)
                    {
                        //Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = bounds.GetIntersectionDepth(tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            //Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                //If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                //Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    //Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    //Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }

                            }
                            else if(velocity.X > 0)
                            {
                                velocity.X = 0;
                                position.X -= absDepthX;
                            }
                            else if(velocity.X < 0)
                            {
                                velocity.X = 0;
                                position.X += absDepthX;
                            }

                        }
                    }
                }
            }

            //Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        // Is player completely dead?
        public bool IsCompletelyDead
        {
            get { return isCompletelyDead; }
        }
        bool isCompletelyDead;

        // Called when the player has been killed.
        public void OnKilled()
        {
            isAttacking = false;
            isAlive = false;
            SpriteAnimations[currentAnim].Stop();
            currentAnim = "Dead";
            SpriteAnimations[currentAnim].ResetPlay();
        }

        // Called when the player has been killed.
        public void DeadAnimEnd()
        {
            isCompletelyDead = true;
            level.score -= 250;
        }


        // Draws the animated player.
        public void Draw(GameTime _gameTime, SpriteBatch _spriteBatch)
        {
            Rectangle source = GetFrameRectangle(SpriteAnimations[currentAnim].FrameToDraw);

            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            switch (currentAnim)
            {
                case "Idle":
                    base.FrameWidth = 13;
                    base.FrameHeight = 21;
                    _spriteBatch.Draw(SpriteTextures[0], position, source, Color.White, 0.0f,
                  Origin, 5.0f, flip, 0.0f);
                    break;

                case "Attacking":
                    base.FrameWidth = 49;
                    base.FrameHeight = 21;
                    if(flip == SpriteEffects.FlipHorizontally)
                    {
                        _spriteBatch.Draw(SpriteTextures[1], new Vector2(position.X + 80, position.Y), source, Color.White, 0.0f,
                  Origin, 5.0f, flip, 0.0f);
                    }
                    else
                    {
                        _spriteBatch.Draw(SpriteTextures[1], new Vector2(position.X - 80, position.Y), source, Color.White, 0.0f,
                  Origin, 5.0f, flip, 0.0f);
                    }
                    break;

                case "Dead":
                    base.FrameWidth = 16;
                    base.FrameHeight = 21;
                    _spriteBatch.Draw(SpriteTextures[2], position, source, Color.White, 0.0f,
                  Origin, 5.0f, flip, 0.0f);
                    break;

                case "Walking":
                    base.FrameWidth = 13;
                    base.FrameHeight = 21;
                    _spriteBatch.Draw(SpriteTextures[3], position, source, Color.White, 0.0f,
                  Origin, 5.0f, flip, 0.0f);
                    break;
            }

           //_spriteBatch.Draw(swordTexture, swordRect, Color.White);
        }

    }

}
