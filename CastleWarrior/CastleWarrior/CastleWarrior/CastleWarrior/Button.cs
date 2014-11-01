﻿using System;
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
    class Button
    {
        Texture2D buttonUp;

        Texture2D buttonDown;

        Texture2D currentTexture;

        Vector2 Position;

        Rectangle collisionRectangle;

        public bool Active { get; set; }

        public void Initialize(Texture2D buttonUp, Texture2D buttonDown, Vector2 position, bool isActive)
        {
            this.buttonUp = buttonUp;
            this.buttonDown = buttonDown;
            Position = position;
            Active = isActive;

            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, this.buttonUp.Width, this.buttonUp.Height);
        }

        public bool isPressed()
        {
            if (Active)
            {
                MouseState mousePosition = Mouse.GetState();
                if (collisionRectangle.Intersects(new Rectangle(mousePosition.X, mousePosition.Y, 1, 1)) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    currentTexture = buttonDown;
                else
                    currentTexture = buttonUp;
                return (collisionRectangle.Intersects(new Rectangle(mousePosition.X, mousePosition.Y, 1, 1)) && Mouse.GetState().LeftButton == ButtonState.Pressed);
            }
            else
                return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(currentTexture, Position, Color.White);
                spriteBatch.End();
            }
        }

    }
}
