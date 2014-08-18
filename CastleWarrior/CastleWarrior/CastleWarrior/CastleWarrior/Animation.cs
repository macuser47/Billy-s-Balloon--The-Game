using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Animation
    {
        Texture2D spriteStrip;

        float scale;

        int elapsedTime;

        public int frameTime;

        public int frameCount;

        int currentFrame;

        Color color;

        Rectangle sourceRect = new Rectangle();

        Rectangle destinationRect = new Rectangle();

        public int frameWidth;

        public int frameHeight;

        public bool active;

        public bool Looping;

        public Vector2 Position;

        public void Initailize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount,
                                    int frameTime, Color color, float scale, bool looping)
        {
            this.color = color;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frameTime;
            this.scale = scale;

            Looping = looping;

            Position = position;

            spriteStrip = texture;

            elapsedTime = 0;
            currentFrame = 0;

            active = true;

        }

        public void Update(GameTime gameTime)
        {
            if (active == false)
                return;

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                currentFrame++;

                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    if (Looping == false)
                        active = false;
                }

                elapsedTime = 0;

            }

            sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            destinationRect = new Rectangle((int)Position.X - (int)(frameWidth * scale) / 2, 
            (int)Position.Y - (int)(frameHeight * scale) / 2,
            (int)(frameWidth * scale), 
            (int)(frameHeight * scale));



        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }

    }
}
