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
using SharedContent;

namespace CastleWarrior
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Block Textures
        Texture2D stoneBrick;
        Texture2D fadedStoneBrick;
        #endregion

        Map currentMap = new Map();

        Texture2D player1Texture;
        Player player1 = new Player();

        SpriteFont debug;

        public static bool isDebugging = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            Vector2 startingPlayerPosition = new Vector2(45,2);
            spriteBatch = new SpriteBatch(GraphicsDevice); // 17 13

            #region Block texture initialization
            stoneBrick = Content.Load<Texture2D>("Blocks/stoneBrick");
            fadedStoneBrick = Content.Load<Texture2D>("Blocks/fadedStoneBrick");
            #endregion

            debug = Content.Load<SpriteFont>("Debug");

            currentMap.LoadMap("Map1", Content);
            currentMap.Generate(stoneBrick, fadedStoneBrick);

            player1Texture = Content.Load<Texture2D>("Player");
            player1.Initialize(player1Texture, new Vector2(45, 2), currentMap.Width, currentMap.Height, GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            currentMap.Update();
            player1.Update(GraphicsDevice);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            currentMap.Draw(spriteBatch);

            player1.Draw(spriteBatch);

            spriteBatch.Begin();
            if (isDebugging)
            {
                spriteBatch.DrawString(debug, player1.Position.X.ToString() + " " + player1.Position.Y.ToString(), new Vector2(200, 200), Color.White);
                spriteBatch.DrawString(debug, GraphicsDevice.Viewport.Width + " " + GraphicsDevice.Viewport.Height, new Vector2(200, 250), Color.White);
                spriteBatch.DrawString(debug, Map.upscroll.ToString(), new Vector2(200, 300), Color.White);
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
