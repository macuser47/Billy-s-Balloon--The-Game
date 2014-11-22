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
    public class MenuHandler
    {
        public enum GameState
        {
            InGameplay = 0,
            HoverMenu = 1,
            FullScreenMenu = 2
        }

        public enum BackgroundType
        {
            None = 0,
            Rectangle = 1,
            FullScreenIMG = 2,
            HoverIMG = 3
        }

        public static GameState currentGameState = GameState.InGameplay;

        public static GameState previousGameState = GameState.FullScreenMenu;

        static MenuContent menu = new MenuContent();

        private bool backgroundRectToggle = false;

        private BackgroundType backgroundType = BackgroundType.None;

        private int xDisp;

        private int yDisp;

        static List<Textbox> textboxList = new List<Textbox>();

        static List<Button> buttonList = new List<Button>();

        static List<Texture2D> textureList = new List<Texture2D>();

        public void UpdateGameState(GameState newGameState)
        {
            if (newGameState != currentGameState)
            {
                previousGameState = currentGameState;
                currentGameState = newGameState;
            } 
        }

        public static bool ButtonPressedInMenu(string buttonId)
        {
            foreach (Button button in buttonList)
            {
                if (button.id == buttonId && button.isPressed())
                    return true;
            }

            return false;
        }

        public void ToggleMenu(string menuName, ContentManager content, GraphicsDevice graphicsDevice)
        {
            if (currentGameState != GameState.InGameplay)
            {
                UpdateGameState(GameState.InGameplay);
                textboxList = new List<Textbox>();
                buttonList = new List<Button>();
                textureList = new List<Texture2D>();
                backgroundRectToggle = false;
                backgroundType = BackgroundType.None;
            }
            else
            {

                menu = content.Load<MenuContent>("Xml/Menus/" + menuName);

                if (menu.type == "fullscreen")
                    UpdateGameState(GameState.FullScreenMenu);
                else if (menu.type == "hover")
                    UpdateGameState(GameState.HoverMenu);
                else
                    throw new Exception("Menu type not recognized. You probably spelled it wrong.");


                if (currentGameState == GameState.HoverMenu && menu.backgroundimage == "")
                {
                    backgroundRectToggle = true;
                    backgroundType = BackgroundType.Rectangle;
                }
                else if (menu.backgroundimage != "" && currentGameState == GameState.HoverMenu)
                {
                    backgroundRectToggle = false;
                    backgroundType = BackgroundType.HoverIMG;
                }
                else if (menu.backgroundimage == "" && currentGameState == GameState.FullScreenMenu)
                {
                    backgroundRectToggle = false;
                    backgroundType = BackgroundType.Rectangle;
                }
                else if (menu.backgroundimage != "" && currentGameState == GameState.FullScreenMenu)
                {
                    backgroundRectToggle = false;
                    backgroundType = BackgroundType.FullScreenIMG;
                }
                else
                {
                    throw new Exception("Dammit, Jim, it's just not working.");
                }

                xDisp = ((graphicsDevice.Viewport.Width) - (menu.width)) / 2;
                yDisp = ((graphicsDevice.Viewport.Height) - (menu.height)) / 2;


                Console.WriteLine("X: {0}", xDisp);
                Console.WriteLine("Y: {0}", yDisp);

                foreach (TextboxContent obj in menu.textboxes)
                {
                    Textbox textbox;
                    if (currentGameState == GameState.HoverMenu)
                        textbox = new Textbox(content.Load<SpriteFont>("FRAMEWORK_FONTS/" + obj.font), obj._string, obj.numberoflines, new Vector2(obj.position.X + xDisp, obj.position.Y + yDisp));
                    else
                        textbox = new Textbox(content.Load<SpriteFont>("FRAMEWORK_FONTS/" + obj.font), obj._string, obj.numberoflines, obj.position);
                    textboxList.Add(textbox);
                }

                foreach (ButtonContent obj in menu.buttons)
                {
                    Button button = new Button();
                    Texture2D text1 = content.Load<Texture2D>(obj.uptexture);
                    Texture2D text2 = content.Load<Texture2D>(obj.downtexture);
                    if (currentGameState == GameState.HoverMenu)
                        button.Initialize(text1, text2, new Vector2(xDisp + obj.position.X, obj.position.Y + yDisp), true, obj.id);
                    else
                        button.Initialize(text1, text2, obj.position, true, obj.id);
                    buttonList.Add(button);
                }

                foreach (Texture2DContent obj in menu.textures)
                {
                    Texture2D texture = content.Load<Texture2D>(obj.src);
                    textureList.Add(texture);
                }
            }

        }


        public void DrawHoverRect(SpriteBatch spriteBatch, Color color, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, int width, int height)
        {   
            var rect = new Texture2D(graphics.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            //rect.SetData(new[] { color });
            Color[] colorData = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                colorData[i] = color;
            }

            rect.SetData<Color>(colorData);

            spriteBatch.Begin();
            spriteBatch.Draw(rect, new Vector2((graphicsDevice.Viewport.Width / 2 ) - (width / 2), (graphicsDevice.Viewport.Height / 2) - (height / 2)), color);
            spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, ContentManager content)
        {
            if (currentGameState != GameState.InGameplay)
            {
                if (backgroundRectToggle)
                    DrawHoverRect(spriteBatch, menu.backgroundcolor, graphics, graphicsDevice, menu.width, menu.height);
                else if (backgroundType == BackgroundType.Rectangle)
                    DrawHoverRect(spriteBatch, menu.backgroundcolor, graphics, graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
                else
                {
                    Texture2D backgroundIMG = content.Load<Texture2D>(menu.backgroundimage);
                    float scalar;

                    spriteBatch.Begin();

                    if (backgroundType == BackgroundType.FullScreenIMG)
                    {
                        if (backgroundIMG.Width > backgroundIMG.Height)
                        {
                            scalar = graphicsDevice.Viewport.Height / backgroundIMG.Height;
                        }
                        else
                        {
                            scalar = graphicsDevice.Viewport.Width / backgroundIMG.Width;
                        }

                        spriteBatch.Draw(backgroundIMG, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), scalar, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        if (backgroundIMG.Width > backgroundIMG.Height)
                        {
                            scalar = menu.height / backgroundIMG.Height;
                        }
                        else
                        {
                            scalar = menu.width / backgroundIMG.Width;
                        }

                        spriteBatch.Draw(backgroundIMG, new Vector2((graphicsDevice.Viewport.Width / 2) - (menu.width / 2), (graphicsDevice.Viewport.Height / 2) - (menu.height / 2)), null, Color.White, 0f, new Vector2(0, 0), scalar, SpriteEffects.None, 0f);
                    }

                    spriteBatch.End();
                }


                foreach (Button button in buttonList)
                {
                    button.isPressed();
                    button.Draw(spriteBatch);
                }

                foreach (Textbox text in textboxList)
                {
                    text.Draw(spriteBatch);
                }

                foreach (Texture2DContent obj in menu.textures)
                {
                    Texture2D texture = content.Load<Texture2D>(obj.src);
                    spriteBatch.Begin();
                    if (currentGameState == GameState.HoverMenu)
                        spriteBatch.Draw(texture, new Vector2(xDisp + obj.position.X, obj.position.Y + yDisp), Color.White);
                    else
                        spriteBatch.Draw(texture, new Vector2(obj.position.X, obj.position.Y), Color.White);
                    spriteBatch.End();
                }
            }
        }
    }
}
