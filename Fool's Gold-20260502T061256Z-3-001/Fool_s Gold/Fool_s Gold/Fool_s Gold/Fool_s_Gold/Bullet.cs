using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fool_s_Gold
{

    class Bullet
    {
        private int direction;
        public Rectangle position;
        private Texture2D texture;
        private int velocity;

        public Bullet(Rectangle position, int direction, Texture2D texture)
        {
            this.position = position;
            this.direction = direction;
            this.texture = texture;
            velocity = 20 * direction;
        }

        public void Update()
        {
            position.X += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.LightYellow);
        }
    }
}
