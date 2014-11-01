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
            int wordsPerLine = lineNumbers / wordList.Length;

            string tempString = "";

            foreach (string word in wordList)
            {
                tempString += word;
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, String, Position, Color.White);
            spriteBatch.End();
        }
    }
}
