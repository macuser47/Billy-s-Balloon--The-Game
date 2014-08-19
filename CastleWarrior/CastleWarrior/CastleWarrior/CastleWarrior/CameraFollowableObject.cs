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
    class CameraFollowableObject
    {

        float scale;

        bool applyPlayerGravityXYMovement = true;

        Texture2D Texture;

        float gravity;

        public Vector2 Position;

        Rectangle playerHitBox;

        public int rightVelocity { get; set; }

        public int leftVelocity { get; set; }

        public int Health { get; set; }

        public float textWidth { get { return Texture.Width * scale; } }

        public float textHeight { get { return Texture.Height * scale; } }

        public void Initialize(Texture2D texture, Vector2 pos, int mapWidth, int mapHeight, GraphicsDevice graphicsDevice, float scale)
        {
            #region Set Variables / Objects

            this.scale = scale;

            Texture = texture;

            gravity = 0;

            Position = pos;

            Health = 100;

            playerHitBox = new Rectangle((int)Position.X, (int)Position.Y, (int)textWidth, (int)textHeight);

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
                        Map.upscroll -= 1;
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

            applyPlayerGravityXYMovement = true;

            playerHitBox = new Rectangle((int)Position.X, (int)Position.Y, (int)textWidth, (int)textHeight);

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

            moveAndApplyCollision(graphicsDevice);

            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public Vector2 getXYinMap()
        {
            return new Vector2((int)Math.Round(Position.X / 24), (int)Math.Round(Position.Y / 24));
        }

        public void moveAndApplyCollision(GraphicsDevice graphicsDevice)
        {
            float slope = (gravity) / (rightVelocity - leftVelocity);
            float diffY = slope;
            float diffX = 1f;

            Vector2 nextPosition;
            Rectangle checkRectangle;
            gravity += 0.1f;
            nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity, Position.Y + gravity);
            checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
            int loopCount = 0;
            foreach (Block block in Map.blockList)
            {
                if (block.hasCollision && checkRectangle.Intersects(block.collide))
                {
                    while (checkRectangle.Intersects(block.collide))
                    {
                        if (gravity > 0)
                            Position.Y -= 1;
                        else if (gravity < 0)
                            Position.Y += 1;

                        if (rightVelocity - leftVelocity > 0)
                            Position.X -= 1;
                        else if (rightVelocity - leftVelocity < 0)
                            Position.X += 1;

                        nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity, Position.Y + gravity);
                        checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
                        loopCount++;
                    }
                    Console.WriteLine("Loop Count = {0}", loopCount);
                    nextPosition = new Vector2(Position.X, Position.Y + gravity);
                    gravity = 0;
                }
            }
            nextPosition = new Vector2(nextPosition.X, Position.Y);
            Position = nextPosition;

            if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space)) && gravity == 0 && loopCount == 1)
            {
                gravity = -5;
            }

            if ((Position.Y < graphicsDevice.Viewport.Height - 47 || gravity < 0) && applyPlayerGravityXYMovement)
                Position.Y += gravity;

        }

    }
}
