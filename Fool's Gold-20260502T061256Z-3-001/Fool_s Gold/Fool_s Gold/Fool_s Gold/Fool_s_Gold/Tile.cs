using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fool_s_Gold
{
    //Controls the collision detection and response behavior of a tile.
    public enum TileCollision
    {
        //A passable tile is one which does not hinder player motion at all.
        Passable = 0,

        //An impassable tile is one which does not allow the player to move through it at all. 
        //It is completely solid.
        Impassable = 1,

        //A platform tile is one which behaves like a passable tile except when the 
        //player is above it. A player can jump up through a platform as well as move 
        //past it to the left and right, but cannot fall down through the top of it.
        Platform = 2,
    }

    public class Tile
    {
        //Name of the sprite sheet
        public string TileSheetName;
        //the index of the rectangle for sourceRect for drawing
        public int TileSheetIndex;

        public TileCollision Collision;


        //Width and Height previously 64
        public const int Width = 48;
        public const int Height = 48;
        public static readonly Vector2 Size = new Vector2(Width, Height);

        //Constructs a new tile
        public Tile(string _tileSheetName, int _tileSheetIndex, TileCollision _collision)
        {
            TileSheetName = _tileSheetName;
            TileSheetIndex = _tileSheetIndex;
            Collision = _collision;
        }
    }
}
