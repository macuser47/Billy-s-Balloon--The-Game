using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharedContent;

namespace CastleWarrior
{
    class Map
    {
        MapContent mapContent = new MapContent();

        public static int scroll;

        public static int upscroll;

        public static List<Block> blockList = new List<Block>();

        public int Height
        {
            get
            {
                return mapContent.height;
            }
        }

        public int Width
        {
            get
            {
                return mapContent.width;
            }
        }

        public void LoadMap(string mapString, ContentManager Content)
        {
            mapContent = Content.Load<MapContent>("Xml/" + mapString);

        }

        public void Generate(Texture2D brick, Texture2D fadedBrick)
        {
            Vector2 tempPos = new Vector2 (1, 1);
            bool right;
            bool left;
            bool top;
            bool bottom;
            foreach (int mapNum in mapContent.map)
            {
                #region Far Right/Left/Top/Bottom Definer (So scrolling stops at end of map)
                if (tempPos.X == 1)
                {
                    left = true;
                    right = false;
                }
                else if (tempPos.X == mapContent.width)
                {
                    left = false;
                    right = true;
                }
                else
                {
                    left = false;
                    right = false;
                }

                if (tempPos.Y == 1)
                {
                    top = true;
                    bottom = false;
                }
                else if (tempPos.Y == mapContent.height)
                {
                    top = false;
                    bottom = true;
                }
                else
                {
                    top = false;
                    bottom = false;
                }
                #endregion

                //map creation scripts based on XML file
                Block block = new Block();
                switch (mapNum)
                {
                    case 0:
                        block.Initialize(fadedBrick, false, new Vector2(24 * tempPos.X, 24 * tempPos.Y), right, left, top, bottom);
                        break;
                    case 1:
                        block.Initialize(brick, true, new Vector2(24 * tempPos.X, 24 * tempPos.Y), right, left, top, bottom);
                        break;
                }
                if (tempPos.X == mapContent.width)
                {
                    tempPos.X = 1;
                    tempPos.Y +=1;
                }
                else
                {
                    tempPos.X +=1;
                }
                blockList.Add(block);

            }
        }

        public void Update()
        {
            foreach (Block block in blockList)
            {
                block.Update();
            }
            //portal scripts go here
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Block block in blockList)
            {
                block.Draw(spriteBatch);
            }
        }

        public bool isBlockSolidAtPosition(int x, int y)
        {
            int blockType = mapContent.map[((y - 1) * mapContent.width) + x];
            if (blockType == 1)
                return true;
            else
                return false;
        }


        //need to complete

    }
}
