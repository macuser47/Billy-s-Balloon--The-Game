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
    class CameraFollowableObject : PhysicsObject
    {

        float scale;

        bool applyPlayerGravityXYMovement = true;

        Texture2D Texture;

        //float gravity;

        public Vector2 Position;

        Rectangle playerHitBox;

        private Vector2 nextPosition;

        public int rightVelocity { get; set; }

        public int leftVelocity { get; set; }

        public int Health { get; set; }

        public float textWidth { get { return Texture.Width * scale; } }

        public float textHeight { get { return Texture.Height * scale; } }

        //public struct CorrectionVector2
        //{
        //    public DirectionX DirectionX;
        //    public DirectionY DirectionY;
        //    public float X;
        //    public float Y;
        //}

        //public enum DirectionX
        //{
        //    Left = -1,
        //    None = 0,
        //    Right = 1
        //}

        //public enum DirectionY
        //{
        //    Up = -1,
        //    None = 0,
        //    Down = 1
        //}

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
           // if (MenuHandler.currentGameState == MenuHandler.GameState.InGameplay)
           // {
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

                while (/*Position.X > graphicsDevice.Viewport.Width - 48 || Position.X < 48*/ Position.X != graphicsDevice.Viewport.Width / 2)
                {
                    if ((leftVelocity == 0 && Position.X < graphicsDevice.Viewport.Width / 2) || (rightVelocity == 0 && Position.X > graphicsDevice.Viewport.Width / 2))
                        goto endWhile;

                    if (/*Position.X > graphicsDevice.Viewport.Width - 48*/ rightVelocity != 0)
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

                    if (/*Position.X < 48*/ leftVelocity != 0)
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

                //moveAndApplyCollision(graphicsDevice);

                List<Rectangle> collidableList = new List<Rectangle>();

                foreach (Block block in Map.blockList)
                {
                    if (block.hasCollision)
                        collidableList.Add(block.collide);
                }

                Position = base.Update(graphicsDevice, Position, true, 5, rightVelocity, leftVelocity, 0f, 0f, textWidth, textHeight, collidableList, applyPlayerGravityXYMovement);
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
            return new Vector2((int)Math.Round(Position.X / 24), (int)Math.Round(Position.Y / 24));
        }

        public void moveAndApplyCollision(GraphicsDevice graphicsDevice)
        {
            //float slope = (gravity) / (rightVelocity - leftVelocity);
            //float diffY = slope;
            //float diffX = 1f;

            //Vector2 nextPosition;
            Rectangle checkRectangle;
            gravity += 0.1f;
            nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity, Position.Y + gravity);
            checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
            #region Old Collision System
            //int loopCount = 0;

            //foreach (Block block in Map.blockList)
            //{
            //    if (block.hasCollision && checkRectangle.Intersects(block.collide))
            //    {

            //        while (checkRectangle.Intersects(block.collide))
            //        {
            //            if (gravity > 0)
            //                Position.Y -= 1;
            //            else if (gravity < 0)
            //                Position.Y += 1;

            //            if (rightVelocity - leftVelocity > 0)
            //                Position.X -= 1;
            //            else if (rightVelocity - leftVelocity < 0)
            //                Position.X += 1;

            //            nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity, Position.Y + gravity);
            //            checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
            //            loopCount++;
            //        }
            //        Console.WriteLine("Loop Count = {0}", loopCount);
            //        nextPosition = new Vector2(Position.X, Position.Y + gravity);
            //        gravity = 0;
            //    }
            //}
            //nextPosition = new Vector2(nextPosition.X, Position.Y);
            //Position = nextPosition;
            #endregion

            List<CorrectionVector2> corrections = new List<CorrectionVector2>();

            foreach (Block block in Map.blockList)
            {
                if (block.hasCollision && checkRectangle.Intersects(block.collide))
                    corrections.Add(GetCorrectionVector(checkRectangle, block));
            }

            #region Sum up directions
            int horizontalSum = SumHorizontal(corrections);
            int verticalSum = SumVertical(corrections);

            DirectionX directionX = DirectionX.None;
            DirectionY directionY = DirectionY.None;


            if (horizontalSum <= (int)DirectionX.Left)
                directionX = DirectionX.Left;
            else if (horizontalSum >= (int)DirectionX.Right)
                directionX = DirectionX.Right;
            else
                directionX = DirectionX.None; // if they cancel each other out, i.e 2 Left and 2 Right


            if (verticalSum <= (float)DirectionY.Up)
                directionY = DirectionY.Up;
            else if (verticalSum >= (float)DirectionY.Down)
                directionY = DirectionY.Down;
            else
                directionY = DirectionY.None; // if they cancel each other out, i.e 1 Up and 1 Down
            #endregion

            #region Correct Player Position
            CorrectionVector2 smallestCorrectionY = getSmallestCorrectionY(directionY, corrections);
            CorrectionVector2 smallestCorrectionX = getSmallestCorrectionX(directionX, corrections);

            if (Math.Abs(verticalSum) > Math.Abs(horizontalSum)) // start with Y, if collision = then try X
            {
                if (verticalSum < 0)
                    gravity = 0;
                else
                    gravity = 0.1f;
                correctCollision(smallestCorrectionY, false, verticalSum);
                checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
                if (IsCollidingWithBlocks(checkRectangle))
                    correctCollision(smallestCorrectionX, true, verticalSum);
                else
                    directionX = DirectionX.None;
            }
            else if (Math.Abs(horizontalSum) > Math.Abs(verticalSum)) // start with X, if collision = then try Y
            {
                correctCollision(smallestCorrectionX, true, verticalSum);
                checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
                if (IsCollidingWithBlocks(checkRectangle))
                    correctCollision(smallestCorrectionY, false, verticalSum);
                else
                    directionY = DirectionY.None;
            }
            #endregion
            else
            {
                #region Account for Zeros (Cancelling)
                if (smallestCorrectionX.X > smallestCorrectionY.Y) // start with Y
                {
                    if (verticalSum < 0)
                        gravity = 0;
                    else
                        gravity = 0.1f;
                    correctCollision(smallestCorrectionY, false, verticalSum);
                    checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
                    if (IsCollidingWithBlocks(checkRectangle))
                        correctCollision(smallestCorrectionX, true, verticalSum);
                    else
                        directionX = DirectionX.None;
                }
                else // start with X
                {
                    correctCollision(smallestCorrectionX, true, verticalSum);
                    checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
                    if (IsCollidingWithBlocks(checkRectangle))
                        correctCollision(smallestCorrectionY, false, verticalSum);
                    else
                        directionY = DirectionY.None;
                }
                #endregion
            }
            
            nextPosition = new Vector2(nextPosition.X, nextPosition.Y - gravity);
            Position = nextPosition;

            //Console.WriteLine(gravity);

            if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space)) && gravity == 0)
            {
                gravity = -5;
            }

            if ((Position.Y < graphicsDevice.Viewport.Height - 47 || gravity < 0) && applyPlayerGravityXYMovement)
                Position.Y += gravity;

        }
        #region Collision Functions
        private bool IsCollidingWithBlocks(Rectangle rect)
        {
            foreach(Block block in Map.blockList)
            {
                if (rect.Intersects(block.collide) && block.hasCollision)
                    return true;
            }

            return false;
        }

        public CorrectionVector2 GetCorrectionVector(Rectangle player, Block block)
        {
            CorrectionVector2 ret = new CorrectionVector2();

            float x1 = Math.Abs(player.Right - block.collide.Left);
            float x2 = Math.Abs(player.Left - block.collide.Right);
            float y1 = Math.Abs(player.Bottom - block.collide.Top);
            float y2 = Math.Abs(player.Top - block.collide.Bottom);

            // calculate displacement along X-axis
            if (x1 < x2)
            {
                ret.X = x1;
                ret.DirectionX = DirectionX.Left;
            }
            else if (x1 > x2)
            {
                ret.X = x2;
                ret.DirectionX = DirectionX.Right;
            }

            // calculate displacement along Y-axis
            if (y1 < y2)
            {
                ret.Y = y1;
                ret.DirectionY = DirectionY.Up;
            }
            else if (y1 > y2)
            {
                ret.Y = y2;
                ret.DirectionY = DirectionY.Down;
            }

            return ret;
        }

        private int SumHorizontal(List<CorrectionVector2> list)
        {
            int count = 0;
            foreach (CorrectionVector2 vector in list)
            {
                count += (int)vector.DirectionX;
            }

            return count;
        }

        private int SumVertical(List<CorrectionVector2> list)
        {
            int count = 0;
            foreach (CorrectionVector2 vector in list)
            {
                count += (int)vector.DirectionY;
            }

            return count;
        }

        private void correctCollision(CorrectionVector2 correction, bool correctHorizontal, int sumVert)
        {
            if (correctHorizontal) // horizontal
            {
                nextPosition.X += correction.X * (int)correction.DirectionX;
                //Console.WriteLine("X: {0}", (int)correction.DirectionX);
            }
            else // vertical
                nextPosition.Y += correction.Y * (int)correction.DirectionY;
       }

        private CorrectionVector2 getSmallestCorrectionX(DirectionX directionX, List<CorrectionVector2> corrections)
        {
            CorrectionVector2 smallest = new CorrectionVector2();
            smallest.X = int.MaxValue;

            foreach (CorrectionVector2 correction in corrections)
            {
                if (correction.DirectionX == directionX && correction.X < smallest.X)
                    smallest = correction;
            }

            return smallest;
        }

        private CorrectionVector2 getSmallestCorrectionY(DirectionY directionY, List<CorrectionVector2> corrections)
        {
            CorrectionVector2 smallest = new CorrectionVector2();
            smallest.Y = int.MaxValue;

            foreach (CorrectionVector2 correction in corrections)
            {
                if (correction.DirectionY == directionY && correction.Y < smallest.Y)
                    smallest = correction;
            }

            return smallest;
        }
        #endregion
        


    }
}
