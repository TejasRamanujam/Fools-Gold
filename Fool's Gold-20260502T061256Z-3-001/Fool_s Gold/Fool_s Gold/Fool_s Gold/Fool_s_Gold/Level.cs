using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace Fool_s_Gold
{
    class Level : IDisposable //Interface IDisposal will be useful for unloading the level
    {

        private List<Enemy> enemies = new List<Enemy>();
        private List<Enemy> deadEnemies = new List<Enemy>();
        public List<Bullet> bullets = new List<Bullet>();

        private List<Collectible> collectibles = new List<Collectible>();
        private List<Collectible> collectedCollectibles = new List<Collectible>();

        private Texture2D heartTexture;


        //Constructor
        public Level(IServiceProvider _serviceProvider, string path)
        {
            //Create new Content Manager to load content used by this level
            content = new ContentManager(_serviceProvider, "Content");

            heartTexture = content.Load<Texture2D>("Sprites/White Square");

            //Load the textures
            TileSheets = new Dictionary<string, Texture2D>();
            //TileSheets.Add("Blocks", Content.Load<Texture2D>("Sprites/White Square"));

            TileSheets.Add("Grass Edge", Content.Load<Texture2D>("Sprites/Tiles/grass edge"));
            TileSheets.Add("Grass", Content.Load<Texture2D>("Sprites/Tiles/grass"));
            TileSheets.Add("Wood", Content.Load<Texture2D>("Sprites/Tiles/Wood"));

            //Create a collection of source rectangle
            TileSourceRecs = new Dictionary<int, Rectangle>();
            for (int i = 0; i < TilesPerRow * NumRowsPerSheet; i++)
            {
                Rectangle rectTile = new Rectangle((i % TilesPerRow) * TileWidth, (i / TilesPerRow) * TileHeight, TileWidth, TileHeight);
                TileSourceRecs.Add(i, rectTile);

            }

            LoadTiles(path);
        }

        private void LoadTiles(string path)
        {
            //Load level and ensure all of the lines are the same length
            int numOfTilesAcross = 0;
            List<string> lines = new List<string>();

            try
            {
                //Create and instance of Streamreader to read from a file
                //The using statement also closes the Streamreader
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = reader.ReadLine();
                    numOfTilesAcross = line.Length;
                    while (line != null)
                    {
                        lines.Add(line); //Saves the text file data in the list
                        int nextLineWidth = line.Length;
                        if (nextLineWidth != numOfTilesAcross) //Each row is the same length
                            throw new Exception(String.Format("The length of line {0} is different from the preceding lines.", lines.Count));
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            tiles = new Tile[numOfTilesAcross, lines.Count];

            //Loop over every tile position

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //To load each tile
                    string currentRow = lines[y];
                    char tileType = currentRow[x];

                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }
        }

        private Tile LoadTile(char _tileType, int _x, int _y)
        {
            switch (_tileType)
            {
                //Blank space
                case '.':
                    return new Tile(String.Empty, 0, TileCollision.Passable);

                //Impassible blocks
                case 'b':
                    return LoadVarietyTile("Grass Edge", 0, 0);
                case 'g':
                    return LoadVarietyTile("Grass", 0, 0);
                case 'w':
                    return LoadVarietyTile("Wood", 0, 0);

                // enemies spawns
                case 'e':
                    return LoadEnemyTile(_x, _y, "e");

                case 'E':
                    return LoadEnemyTile(_x, _y, "E");

                // Collectibles spawns
                case 'c':
                    return LoadCollectibleTile(_x, _y, "s");
                case 'C':
                    return LoadCollectibleTile(_x, _y, "S");


                //Player starting point
                case '+':
                    return LoadStartTile(_x, _y);


                //Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", _tileType, _x, _y));
            }

        }

        private Tile LoadVarietyTile(string _tileSheetName, int _colorRow, int _variationCount)
        {
            int index = random.Next(_variationCount);  //randomly choose random design variant

            //get index on tile to rect dictionary
            int tileSheetIndex = _colorRow + index;
            return new Tile(_tileSheetName, tileSheetIndex, TileCollision.Impassable);

        }

        // Instantiates an enemy and puts him in the level.
        private Tile LoadEnemyTile(int _x, int _y, string _enemy)
        {
            Vector2 position = new Vector2((_x * 48) + 48, (_y * 48) + 12);
            enemies.Add(new Enemy(this, position, _enemy));
            return new Tile(String.Empty, 0, TileCollision.Passable);
        }

        // Instantiates an collectible and puts it in the level.
        private Tile LoadCollectibleTile(int _x, int _y, string _collectible)
        {
            Vector2 position = new Vector2((_x * 48) + 48, (_y * 48) + 20);
            collectibles.Add(new Collectible(this, position, _collectible));
            return new Tile(String.Empty, 0, TileCollision.Passable);
        }


        //Instantiates a player, puts him in the level, and remembers where to put him when 
        //he is resurrected.
        private Tile LoadStartTile(int _x, int _y)
        {

            if (player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            //player is drawn based on the origin, so we need to compensate for that.
            //48 is half the size of the player.
            //(x * baseTileWidth) + halfofplayerwidth, (y * baseTileHeight) + 
            //    + (baseTileHeight - player Height) + halfofplayerheight
            start = new Vector2((_x * 48) + 24, (_y * 48) + 10);
            player = new Player(this, start);

            return new Tile(String.Empty, 0, TileCollision.Passable);
        }


        //This 2D array of Tile objects will keep track of the platform locations.
        private Tile[,] tiles;

        //These collections will let us reference the sprite sheets by name 
        //and get the correct source rectangle.
        private Dictionary<string, Texture2D> TileSheets;
        public Dictionary<int, Rectangle> TileSourceRecs;

        public List<Rectangle> TileDefinitions;

        //Entities in the Level
        public Player Player
        {
            get { return player; }
        }
        Player player;

        //holds the starting point for the level
        private Vector2 start;


        //Useful constants

        //Previously width = 64
        private const int TileWidth = 48;
        private const int TileHeight = 48;
        //Previously TilesPerRow = 10
        private const int TilesPerRow = 25;
        private const int NumRowsPerSheet = 5;

        private Random random = new Random(1337);

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public int Width
        {
            get { return tiles.GetLength(0); }
        }
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        public void Draw(GameTime _gameTime, SpriteBatch _spriteBatch)
        {
            DrawTiles(_spriteBatch);
            player.Draw(_gameTime, _spriteBatch);
            foreach (Collectible collectible in collectibles)
                collectible.Draw(_gameTime, _spriteBatch);
            foreach (Enemy enemy in enemies)
                enemy.Draw(_gameTime, _spriteBatch);
            foreach (Bullet bullet in bullets)
                bullet.Draw(_spriteBatch);

            for (int x = 0; x < Player.Hp; x++)
            {
                _spriteBatch.Draw(heartTexture, new Rectangle((x * 25) + 10, 10, 20, 20), Color.Red);
            }
        }



        private void DrawTiles(SpriteBatch spriteBatch)
        {
            //For each tile position
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //If there is a visible tile in that position
                    if (TileSheets.ContainsKey(tiles[x, y].TileSheetName))
                    {
                        //Draw it in the screen space
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(TileSheets[tiles[x, y].TileSheetName], position, TileSourceRecs[tiles[x, y].TileSheetIndex], Color.White);
                    }
                }
            }
        }

        public void Dispose()
        {
            Content.Unload();
        }

        //Gets the collision mode of the tile at a particular location.
        //This method handles tiles outside of the level boundaries by making it
        //impossible to escape past the left or right edges, but allowing things
        //to jump beyond the top of the level and fall off the bottom.
        public TileCollision GetCollision(int _x, int _y)
        {
            //Prevent escaping past the level ends
            if(enemiesClear())
            {
               if (_x < 0)
                    return TileCollision.Impassable;
            }

            else if (_x < 0 || _x >= Width - 1)
                return TileCollision.Impassable;
            if (_y < 0 || _y >= Height)
                return TileCollision.Passable;

            return tiles[_x, _y].Collision;
        }

        // Gets the bounding rectangle of a tile in world space.
        public Rectangle GetBounds(int _x, int _y)
        {
            if (_x < 0 || _y < 0 || _x >= Width || _y >= Height)
                return new Rectangle(_x * Tile.Width, _y * Tile.Height, Tile.Width, Tile.Height);

            return new Rectangle(_x * Tile.Width, _y * Tile.Height + 5, Tile.Width, Tile.Height - 5);
        }


        // Updates all objects in the world, performs collision between them,
        // and handles the time limit with scoring.
        public void Update(GameTime _gameTime)
        {
            Player.Update(_gameTime);
            if (Player.IsCompletelyDead)
                Player.Reset(start);
            UpdateEnemies(_gameTime);
            UpdateCollectibles(_gameTime);
            UpdateBullets(_gameTime);
        }


        // Animates each enemy and allow them to kill the player.
        private void UpdateCollectibles(GameTime _gameTime)
        {
            foreach (Collectible collectible in collectibles)
            {
                collectible.Update(_gameTime);
                if (player.IsAlive && collectible.IsAlive)
                {
                    // Touching an coin 
                    if (collectible.BoundingRectangle.Intersects(Player.BoundingRectangle))
                    {
                        OnCollectibleCollected(collectible);
                        score += 50;
                    }
                }
            }
            if (collectedCollectibles.Count > 0)
            {
                foreach (Collectible collectible in collectedCollectibles)
                {
                    collectibles.Remove(collectible);
                }
                collectedCollectibles.Clear();
            }
        }

        // Called when a collectible is collected.
        private void OnCollectibleCollected(Collectible _collectible)
        {
            _collectible.OnKilled();
            collectedCollectibles.Add(_collectible);
        }

        private int previousPlayerHitTime = 0;

        // Animates each enemy and allow them to kill the player.
        private void UpdateEnemies(GameTime _gameTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(_gameTime);
                if (player.IsAlive && enemy.IsAlive)
                {
                    if (enemy.BoundingRectangle.Intersects(Player.swordRect) && Player.isAttacking)
                    {
                        if (enemy.Hp > 0 && enemy.IFrames <= 0)
                        {
                            enemy.Hp--;
                            enemy.IFrames = 45;
                        }
                        else if(enemy.Hp <= 0)
                        {
                            OnEnemyKilled(enemy);
                            if (enemy.enemyType.Equals("Soldier"))
                                score += 100;
                            else
                                score += 500;
                        }
                    }
                    else if (enemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                    {
                        //Adding Player and enemy Collision
                        Rectangle bounds = Player.BoundingRectangle;
                        Vector2 depth = bounds.GetIntersectionDepth(enemy.BoundingRectangle);
                        float absDepthX = Math.Abs(depth.X);
                        float absDepthY = Math.Abs(depth.Y);
                        
                        if (Player.Velocity.X > 0)
                        {
                            Player.Position = new Vector2(Player.Position.X - absDepthX - 5, Player.Position.Y);
                        }
                        else
                        {
                            Player.Position = new Vector2(Player.Position.X + absDepthX + 5, Player.Position.Y);
                        }
                        
                        if(_gameTime.TotalGameTime.Seconds > previousPlayerHitTime + 1)
                        {
                            previousPlayerHitTime = _gameTime.TotalGameTime.Seconds;
                            Player.Hp -= 1;
                            if(Player.Hp <= 0)
                            {
                                OnPlayerKilled();
                            }
                        }
                    }
                }
                else
                {
                    if (enemy.IsCompletelyDead)
                        deadEnemies.Add(enemy);
                }
            }
            if (deadEnemies.Count > 0)
            {
                foreach (Enemy deadEnemy in deadEnemies)
                {
                    enemies.Remove(deadEnemy);
                }
                deadEnemies.Clear();
            }
        }

        private void UpdateBullets(GameTime _gameTime)
        {
            for (int x = bullets.Count - 1; x >= 0; x--)
            {
                bullets[x].Update();
                if(bullets[x].position.Intersects(player.BoundingRectangle))
                {
                    bullets.RemoveAt(x);
                    player.Hp -= 1;
                    if (Player.Hp <= 0)
                    {
                        OnPlayerKilled();
                    }
                }
                else if (bullets[x].position.X < 0 || bullets[x].position.X > 1270)
                    bullets.RemoveAt(x);
            }
        }

        // Called when the player is killed.
        private void OnPlayerKilled()
        {
            Player.OnKilled();
        }

        // Called when an enemy is killed.
        private void OnEnemyKilled(Enemy _enemy)
        {
            _enemy.OnKilled();
        }

        public bool enemiesClear()
        {
            return enemies.Count == 0;
        }

        public int score;

    }

}
