using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SharedContent
{
    public class MenuContent
    {
        public string type;
        public int width;
        public int height;
        public string backgroundimage;
        public Color backgroundcolor;
        public List<TextboxContent> textboxes;
        public List<ButtonContent> buttons;
        public List<Texture2DContent> textures;
    }
}
