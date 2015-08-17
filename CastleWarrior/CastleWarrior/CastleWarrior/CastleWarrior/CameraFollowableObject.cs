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
    class CameraFollowableObject : PhyscisObject
    {

        float scale;

        bool applyPlayerGravityXYMovement = true;

        Texture2D Texture;

        //float gravity;

        public Vector2 Position;

        Rectangle playerHitBox;

        private Vector2 nextPosition;

        float previousTime;

        float currentTime;

        float deltaTime;

        public int rightVelocity { get; set; }

        public int leftVelocity { get; set; }

        public int upVelocity { get; set; }

        public int downVelocity { get; set; }

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

            Position.X *= GameProperties.TILESIZE;
            Position.Y *= GameProperties.TILESIZE;

            Map.scroll = 0;
            Map.upscroll = 0;

            previousTime = 0f;
            currentTime = 0f;

            #endregion

            #region Initial Scroll to player position

            //phase 1
            while (Position.X != (int)(graphicsDevice.Viewport.Width / 2) || Position.Y != (int)(graphicsDevice.Viewport.Height / 2))
            {
                if (Position.X > (int)(graphicsDevice.Viewport.Width / 2))
                {
                    Position.X -= 1;
                    Map.scroll -= 1;
                }
                else if (Position.X < (int)(graphicsDevice.Viewport.Width / 2))
                {
                    Position.X += 1;
                    Map.scroll += 1;
                }

                if (Position.Y > (int)(graphicsDevice.Viewport.Height / 2))
                {
                    Position.Y -= 1;
                    Map.upscroll -= 1;
                }
                else if (Position.Y < (int)(graphicsDevice.Viewport.Height / 2))
                {
                    Position.Y += 1;
                    Map.upscroll += 1;
                }
            }


            //phase 2

            foreach (Block block in Map.blockList)
            {
                if (block.isBottomEdge && block.Position.Y + Map.upscroll > -3 && block.Position.Y  + Map.upscroll < graphicsDevice.Viewport.Height)
                {
                    int tempPosY = (int)block.Position.Y + Map.upscroll;
                    if (tempPosY < graphicsDevice.Viewport.Height - GameProperties.TILESIZE)
                    {
                        Position.Y += graphicsDevice.Viewport.Height - GameProperties.TILESIZE - tempPosY;
                        Map.upscroll += graphicsDevice.Viewport.Height - GameProperties.TILESIZE - tempPosY;
                        tempPosY += graphicsDevice.Viewport.Height - GameProperties.TILESIZE - tempPosY;
                    }
                }

                if (block.isLeftEdge && block.Position.X + Map.scroll > 0 && block.Position.X + Map.scroll < graphicsDevice.Viewport.Height - (GameProperties.TILESIZE))
                {
                    int tempPosX = (int)block.Position.X + Map.scroll;
                    if (tempPosX > (2 * GameProperties.TILESIZE))
                    {
                        Position.X -= tempPosX;
                        Map.scroll -= tempPosX;
                        tempPosX -= tempPosX;
                    }
                }

                if (block.isRightEdge && block.Position.X + Map.scroll > 0 && block.Position.X + Map.scroll < graphicsDevice.Viewport.Height - (GameProperties.TILESIZE))
                {
                    int tempPosX = (int)block.Position.X + Map.scroll;
                    if (tempPosX < graphicsDevice.Viewport.Width - (GameProperties.TILESIZE))
                    {
                        Position.X += graphicsDevice.Viewport.Width - (GameProperties.TILESIZE) - tempPosX;
                        Map.scroll += graphicsDevice.Viewport.Width - (GameProperties.TILESIZE) - tempPosX;
                        tempPosX += graphicsDevice.Viewport.Width - (GameProperties.TILESIZE) - tempPosX;
                    }
                }

                if (block.isTopEdge && block.Position.Y + Map.upscroll > -3 && block.Position.Y + Map.upscroll < graphicsDevice.Viewport.Height)
                {
                    int tempPosY = (int)block.Position.Y + Map.upscroll;
                    if (tempPosY > -3)
                    {
                        Position.Y -= tempPosY + 3;
                        Map.upscroll -= tempPosY + 3;
                        tempPosY -= tempPosY + 3;
                    }
                }
            }
            #endregion

        }

        public void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
           // if (MenuHandler.currentGameState == MenuHandler.GameState.InGameplay)
            // {
                #region Set Up Time
                previousTime = currentTime;
                currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
                deltaTime = currentTime - previousTime;
                #endregion

                #region Input Handler
            #region Check for left/right keyboard input
            //Check for Keyboard Input
                if (!GameProperties.GRADUALMOVE)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        rightVelocity = GameProperties.PLAYERMOVEVELOCITY;
                    else
                        rightVelocity = 0;

                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        leftVelocity = GameProperties.PLAYERMOVEVELOCITY;
                    else
                        leftVelocity = 0;

                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                        upVelocity = GameProperties.PLAYERMOVEVELOCITY;
                    else
                        upVelocity = 0;

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                        downVelocity = GameProperties.PLAYERMOVEVELOCITY;
                    else
                        downVelocity = 0;

                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        if (rightVelocity < GameProperties.PLAYERMOVEVELOCITY)
                            rightVelocity += GameProperties.ACCELERATION;
                    }
                    else if (rightVelocity > 0)
                        rightVelocity -= GameProperties.DECELERATION;
                    else
                        rightVelocity = 0;

                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        if (leftVelocity < GameProperties.PLAYERMOVEVELOCITY)
                            leftVelocity += GameProperties.ACCELERATION;
                    }
                    else if (leftVelocity > 0)
                        leftVelocity -= GameProperties.DECELERATION;
                    else
                        leftVelocity = 0;

                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        if (upVelocity < GameProperties.PLAYERMOVEVELOCITY)
                            upVelocity += GameProperties.ACCELERATION;
                    }
                    else if (upVelocity > 0)
                        upVelocity -= GameProperties.DECELERATION;
                    else
                        upVelocity = 0;

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        if (downVelocity < GameProperties.PLAYERMOVEVELOCITY)
                            downVelocity += GameProperties.ACCELERATION;
                    }
                    else if (downVelocity > 0)
                        downVelocity -= GameProperties.DECELERATION;
                    else
                        downVelocity = 0;
                }
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
                    if (block.Position.Y < graphicsDevice.Viewport.Height - GameProperties.TILESIZE && block.isBottomEdge)
                    {
                        float tempPosY = block.Position.Y;
                        while (tempPosY < graphicsDevice.Viewport.Height - GameProperties.TILESIZE)
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

                while (/*Position.X > graphicsDevice.Viewport.Width - 48 || Position.X < 48*/ (int)Position.X != (int)(graphicsDevice.Viewport.Width / 2))
                {
                    if (Position.X > /*graphicsDevice.Viewport.Width - 48*/ graphicsDevice.Viewport.Width / 2)
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

                    if (Position.X < /*48*/ graphicsDevice.Viewport.Width / 2)
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
                if (/*Math.Abs(Position.Y - graphicsDevice.Viewport.Height) < 47*/ (int)Position.Y > graphicsDevice.Viewport.Height / 2)
                {
                    bool willScrollDown = true;
                    foreach (Block block in Map.blockList)
                    {
                        if (block.isBottomEdge)
                        {
                            if (block.Position.Y > graphicsDevice.Viewport.Height - GameProperties.TILESIZE)
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
                        Position.Y -= gravity;
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
                else if (/*Position.Y < 200*/ Position.Y < graphicsDevice.Viewport.Height / 2)
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

            List<Rectangle> list = new List<Rectangle>();
            foreach (Block block in Map.blockList)
            {
                if (block.hasCollision)
                    list.Add(block.collide);
            }

            //adjust position based on physics engine
            if (GameProperties.GAMETYPE == "platformer/sidescroller")
                Position = base.Update(Position, rightVelocity * deltaTime, leftVelocity * deltaTime, 0f, 0f, true, textWidth, textHeight, list, graphicsDevice);
            else if (GameProperties.GAMETYPE == "RPG")
                Position = base.Update(Position, rightVelocity * deltaTime, leftVelocity * deltaTime, upVelocity * deltaTime, downVelocity * deltaTime, false, textWidth, textHeight, list, graphicsDevice);
            else
                throw new Exception("Value GAMETYPE not one of the expected values. Check your GameProperties file.");


            if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space)) && gravity == 0 /*&& loopCount == 1*/)
            {
                gravity = -5;
            }

            if ((Position.Y < graphicsDevice.Viewport.Height - 47 || gravity < 0) && applyPlayerGravityXYMovement)
                Position.Y += gravity;

                #endregion
            //}
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public Vector2 getXYinMap()
        {
            return new Vector2((int)Math.Round(Position.X / GameProperties.TILESIZE), (int)Math.Round(Position.Y / GameProperties.TILESIZE));
        }

    }
}
