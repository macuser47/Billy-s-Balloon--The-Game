using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CastleWarrior
{
    class Block
    {
        Texture2D Texture;

        public Vector2 Position;

        public int Width
        {
            get
            {
                return Texture.Width;
            }
        }

        public int Height
        {
            get
            {
                return Texture.Height;
            }
        }

        public bool hasCollision;

        public bool isLeftEdge;

        public bool isRightEdge;

        public bool isTopEdge;

        public bool isBottomEdge;

        public Rectangle collide;

        public void Initialize(Texture2D text, bool condition, Vector2 vect, bool right, bool left, bool top, bool bottom)
        {
            Texture = text;

            hasCollision = condition;

            Position = vect;

            isRightEdge = right;

            isLeftEdge = left;

            isTopEdge = top;

            isBottomEdge = bottom;

            collide = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public void Update()
        {
            Position.X += Map.scroll;
            Position.Y += Map.upscroll;

            collide = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Position, Color.White);
            spriteBatch.End();
        }
    }
}
