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
    class Textbox
    {
        SpriteFont font;

        public string String { get; set; }

        int lineNumbers;

        Vector2 Position;

        string[] wordList;

        List<string> stringList = new List<string>();

        public Textbox(SpriteFont font, string String, int numberOfLines, Vector2 position)
        {
            this.font = font;
            this.String = String;
            lineNumbers = numberOfLines;
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            wordList = String.Split(' ');
            int wordsPerLine = wordList.Length / lineNumbers;

            stringList = new List<string>();

            for (int k = 0; k < lineNumbers; k++)
            {
                string tempString = "";
                for (int i = 0; i < wordsPerLine; i++)
                {
                    tempString += (wordList[i] + " ");
                }
                stringList.Add(tempString);
            }

            //foreach (string word in wordList)
            //{
            //    tempString += word;
            //}

            spriteBatch.Begin();

            Vector2 temp = Position;

            foreach (string _string in stringList)
            {
                spriteBatch.DrawString(font, _string, temp, Color.White);
                temp.Y += font.LineSpacing;
            }
            spriteBatch.End();
        }
    }
}
