using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Fool_s_Gold
{
    class AnimatedSprite
    {
        // This holds our texture to the sprite.
        public List<Texture2D> SpriteTextures { get; set; }
        public int FrameWidth;
        public int FrameHeight;
        public int framesPerRow;
        public Dictionary<string, Animation> SpriteAnimations;

        public AnimatedSprite(int _frameWidth, int _frameHeight, int _framesPerRow)
        {
            FrameWidth = _frameWidth;
            FrameHeight = _frameHeight;
            framesPerRow = _framesPerRow;
            SpriteTextures = new List<Texture2D>();
            SpriteAnimations = new Dictionary<string, Animation>();
        }


        // Gets the Frame's origin
        public Vector2 Origin
        {
            get { return new Vector2(FrameWidth / 2.0f, FrameHeight / 2.0f); }
        }

        public Rectangle GetFrameRectangle(int _frameNumber)
        {
            return new Rectangle(
            (_frameNumber % framesPerRow) * FrameWidth,
            (_frameNumber / framesPerRow) * FrameHeight,
            FrameWidth,
            FrameHeight
            );
        }

    }
}
