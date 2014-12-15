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
    class PhyscisObject
    {
        public float gravity = 0f;

        Vector2 nextPosition;

        List<Rectangle> rectList;

        public struct CorrectionVector2
        {
            public DirectionX DirectionX;
            public DirectionY DirectionY;
            public float X;
            public float Y;
        }

        public enum DirectionX
        {
            Left = -1,
            None = 0,
            Right = 1
        }

        public enum DirectionY
        {
            Up = -1,
            None = 0,
            Down = 1
        }

        public Vector2 Update(Vector2 Position, float rightVelocity, float leftVelocity, float upVelocity, float downVelocity, bool isGravity, float textWidth, float textHeight, List<Rectangle> rectList, GraphicsDevice graphicsDevice)
        {
            /*float slope = (gravity) / (rightVelocity - leftVelocity);
            float diffY = slope;
            float diffX = 1f;*/
            this.rectList = rectList;
            //Vector2 nextPosition;
            Rectangle checkRectangle;
            gravity += 0.1f;
            if (isGravity)
                nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity, Position.Y + gravity - upVelocity + downVelocity);
            else
                nextPosition = new Vector2(Position.X + rightVelocity - leftVelocity, Position.Y - upVelocity + downVelocity);

            checkRectangle = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)textWidth, (int)textHeight);
            #region Old Collision System
            /*int loopCount = 0;

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
            Position = nextPosition;*/
            #endregion

            List<CorrectionVector2> corrections = new List<CorrectionVector2>();

            foreach (Rectangle obj in rectList)
            {
                if (checkRectangle.Intersects(obj))
                    corrections.Add(GetCorrectionVector(checkRectangle, obj));
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

            return Position;

        }
        #region Collision Functions
        private bool IsCollidingWithBlocks(Rectangle rect)
        {
            foreach (Rectangle obj in rectList)
            {
                if (rect.Intersects(obj))
                    return true;
            }

            return false;
        }

        public CorrectionVector2 GetCorrectionVector(Rectangle player, Rectangle obj)
        {
            CorrectionVector2 ret = new CorrectionVector2();

            float x1 = Math.Abs(player.Right - obj.Left);
            float x2 = Math.Abs(player.Left - obj.Right);
            float y1 = Math.Abs(player.Bottom - obj.Top);
            float y2 = Math.Abs(player.Top - obj.Bottom);

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
