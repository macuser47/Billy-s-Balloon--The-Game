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
    class Player
    {
        Texture2D Texture;

        float gravity;

        public Vector2 Position;

        Rectangle playerHitBox;

        public int rightVelocity { get; set; }

        public int leftVelocity { get; set; }

        public int Health { get; set; }

        public void Initialize(Texture2D texture, Vector2 pos, int mapWidth, int mapHeight, GraphicsDevice graphicsDevice)
        {
            #region Set Variables / Objects
            Texture = texture;

            gravity = 0;

            Position = pos;

            Health = 100;

            playerHitBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            Position.X *= 24;
            Position.Y *= 24;

            Map.scroll = 0;
            Map.upscroll = 0;
            #endregion

            #region Initial Scroll to player position
            while (Position.X > graphicsDevice.Viewport.Width - 50 || Position.X < 50 || Position.Y > graphicsDevice.Viewport.Height - 50 || Position.Y < 50)
            {
                if (Position.X > graphicsDevice.Viewport.Width - 50)
                {
                    Position.X -= 1;
                    Map.scroll -= 1;
                }

                if (Position.X < 50)
                {
                    Position.X += 1;
                    Map.scroll += 1;
                }

                if (Position.Y > graphicsDevice.Viewport.Height - 50)
                {
                    Position.Y -= 1;
                    Map.upscroll -= 1;
                }

                if (Position.Y < 50)
                {
                    Position.Y += 1;
                    Map.upscroll += 1;
                }
            }
            #endregion

            #region Scroll map up in case of map being too far down
            foreach (Block block in Map.blockList)
            {
                if (block.Position.Y > -3 && block.isTopEdge)
                {
                    float tempPosY = block.Position.Y;
                    while (tempPosY > -3)
                    {
                        Map.upscroll-= 1;
                        tempPosY -= 1;
                        Position.Y -= 1;
                    }
                    return;
                }
            }
            #endregion

        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            #region Input Handler
            #region Check for left/right keyboard input
            //Check for Keyboard Input
            //if (Keyboard.GetState().IsKeyUp(Keys.Right) || Keyboard.GetState().IsKeyUp(Keys.Left) || Keyboard.GetState().IsKeyUp(Keys.Up))
            //{
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rightVelocity = 5;
            }
            else
                rightVelocity = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                leftVelocity = 5;
            }
            else
                leftVelocity = 0;
            //}
            #endregion
            #endregion

            #region Map and Gravity Manipulation
            Map.scroll = 0;
            Map.upscroll = 0;

            bool applyPlayerGravityXYMovement = true;

            playerHitBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            #region Scroll map down in case of map being too far up
            foreach (Block block in Map.blockList)
            {
                if (block.Position.Y < graphicsDevice.Viewport.Height - 24 && block.isBottomEdge)
                {
                    float tempPosY = block.Position.Y;
                    while (tempPosY < graphicsDevice.Viewport.Height - 24)
                    {
                        Map.upscroll += 1;
                        tempPosY += 1;
                        Position.Y += 1;
                    }
                    goto endDownScrollWhile;
                }
            }
            endDownScrollWhile:
            #endregion

            #region Scroll map left/right and stop if at edge

            while (Position.X > graphicsDevice.Viewport.Width - 48 || Position.X < 48)
            {
                if (Position.X > graphicsDevice.Viewport.Width - 48)
                {
                    bool willScrollRight = true;
                    foreach (Block block in Map.blockList)
                    {
                        if (block.isRightEdge)
                        {
                            if (block.Position.X > graphicsDevice.Viewport.Width)
                            {
                                willScrollRight = true;
                            }
                            else
                            {
                                willScrollRight = false;
                            }
                        }
                    }
                    if (willScrollRight)
                    {
                        Position.X -= 1;
                        Map.scroll -= 1;
                    }
                    else
                    {
                        if (Position.X > graphicsDevice.Viewport.Width - 25)
                        {
                            while (Position.X > graphicsDevice.Viewport.Width - 25)
                                Position.X -= 1;
                        }
                        goto endWhile;
                    }
                }

                if (Position.X < 48)
                {
                    bool willScrollLeft = true;
                    foreach (Block block in Map.blockList)
                    {
                        if (block.isLeftEdge)
                        {
                            if (block.Position.X < -3)
                            {
                                willScrollLeft = true;
                            }
                            else
                            {
                                willScrollLeft = false;
                            }
                        }
                    }
                    if (willScrollLeft)
                    {
                        Map.scroll += 1;
                        Position.X += 1;
                    }
                    else
                    {
                        if (Position.X < 0)
                        {
                            while (Position.X < 0)
                                Position.X += 1;
                        }
                        goto endWhile;
                    }

                }
            }
            endWhile:
            #endregion

            #region Scroll up/down and stop scrolling at top and bottom
            if (Math.Abs(Position.Y - graphicsDevice.Viewport.Height) < 47)
            {
                bool willScrollDown = true;
                foreach (Block block in Map.blockList)
                {
                    if (block.isBottomEdge)
                    {
                        if (block.Position.Y > graphicsDevice.Viewport.Height - 24)
                        {
                            willScrollDown = true;
                        }
                        else
                        {
                            willScrollDown = false;
                        }
                    }
                }
                if (willScrollDown)
                {
                    Map.upscroll = -(int)gravity;
                }
                else
                {
                    if (Position.Y > graphicsDevice.Viewport.Height + 3)
                    {
                        while (Position.Y > graphicsDevice.Viewport.Height + 3)
                        {
                            Position.Y -= 1;
                            gravity = 0f;
                        }
                    }
                    goto endUpScroll;
                }


            }
            else if (Position.Y < 200)
            {

                bool willScrollUp = true;
                foreach (Block block in Map.blockList)
                {
                    if (block.isTopEdge)
                    {
                        if (block.Position.Y < -3)
                        {
                            willScrollUp = true;
                        }
                        else
                        {
                            willScrollUp = false;
                        }
                    }
                }
                if (willScrollUp)
                {
                    if (gravity < 0)
                    {
                        Map.upscroll = -(int)gravity;
                        applyPlayerGravityXYMovement = false;
                    }
                }
                else
                {
                    if (Position.Y < 3)
                    {
                        while (Position.Y < 3)
                        {
                            Position.Y += 1;
                            gravity = 0f;
                        }
                    }
                    goto endUpScroll;
                }
            }
            else
            {
                Map.upscroll = 0;
            }
        endUpScroll:
            #endregion

            moveAndApplyCollision();

            //#region Gravity Control and up input and Collision
            //gravity += 0.1f;

            //foreach (Block block in Map.blockList)
            //{
            //    if (block.hasCollision && playerHitBox.Intersects(block.collide))
            //    {
            //        if (gravity > 0)
            //        {
            //            gravity = 0f;
            //        }
            //        else
            //        {
            //            gravity = 0f;
            //            while (block.hasCollision && playerHitBox.Intersects(block.collide))
            //            {
            //                Position.Y += 1;
            //                playerHitBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            //            }
            //            return;
            //        }

            //        while (block.hasCollision && playerHitBox.Intersects(block.collide))
            //        {
            //            Position.Y -= 1;
            //            playerHitBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            //        }
            //        Position.Y += 1;

            //        if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space))
            //        {
            //            gravity = -5;
            //        }
            //    }
            //}

            ////for scrolling down (falling)
            //if ((Position.Y < graphicsDevice.Viewport.Height - 47 || gravity < 0) && applyPlayerGravityXYMovement)
            //    Position.Y += gravity;
            //#endregion
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Position, Color.White);
            spriteBatch.End();
        }

        public Vector2 getXYinMap()
        {
           return new Vector2((int)Math.Round(Position.X / 24), (int)Math.Round(Position.Y / 24));
        }
        
        public void moveAndApplyCollision()
        {
            float slope = (gravity) / (rightVelocity - leftVelocity);
            float diffY = slope;
            float diffX = 1f;
            Vector2 nextPosition;
            Rectangle checkRectangle;
            bool endWhile;
            int counter = 0;
            do
            {
                endWhile = false;
                nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity + (counter * diffX), Position.Y + gravity + (counter * diffX) );
                checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, Texture.Width, Texture.Height);
                foreach (Block block in Map.blockList)
                { 
                    if (block.hasCollision && checkRectangle.Intersects(block.collide))
                        endWhile = true;
                }
                counter++;
            } while (endWhile);
            Position = nextPosition;

            if ()

        }
    }
}
