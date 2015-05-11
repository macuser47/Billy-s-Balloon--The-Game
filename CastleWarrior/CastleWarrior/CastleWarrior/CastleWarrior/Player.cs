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
    class Player : CameraFollowableObject
    {

        public void Initialize(Texture2D texture, Vector2 pos, int mapWidth, int mapHeight, GraphicsDevice graphicsDevice, float scale)
        {
            base.Initialize(texture, pos, mapWidth, mapHeight, graphicsDevice, scale);
        }

        public void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            base.Update(graphicsDevice, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        
    }
}
