using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TOOLS
{
    public class ControlPanel : Microsoft.Xna.Framework.DrawableGameComponent
    {

        bool Open_;

        string FontName { get; set; }
        SpriteFont TextFont { get; set; }
        string BoxTextureName { get; set; }
        string CaretTextureName { get; set; }
        Texture2D CaretTexture { get; set; }
        Texture2D BoxTexture { get; set; }

        TextBox ResolutionX { get; set; }
        TextBox ResolutionY { get; set; }

        SpriteBatch TextBatch { get; set; }
        RessourcesManager<SpriteFont> FontManager { get; set; }
        RessourcesManager<Texture2D> TextureManager { get; set; }
        InputManager Keyboard { get; set; }

        public ControlPanel(Game game, string font, string textBoxTexture, string caretTexture)
            : base(game)
        {
            FontName = font;
            BoxTextureName = textBoxTexture;
            CaretTextureName = caretTexture;
            Open_ = false;
        }

        public override void Initialize()
        {
            TextBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            FontManager = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            TextureManager = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            Keyboard = Game.Services.GetService(typeof(InputManager)) as InputManager;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextFont = FontManager.Find(FontName);
            BoxTexture = TextureManager.Find(BoxTextureName);
            CaretTexture = TextureManager.Find(CaretTextureName);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardManagement();
            if (Open_)
            {
                ResolutionX.Update(gameTime);
                ResolutionY.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void KeyboardManagement()
        {
            if (Keyboard.EstNouvelleTouche(Keys.O))
            {
                ResolutionX = new TextBox(BoxTexture, CaretTexture, TextFont);
                ResolutionY = new TextBox(BoxTexture, CaretTexture, TextFont);
                ResolutionX.X = 150;
                ResolutionX.Y = 150;
                ResolutionX.Width = 300;

                ResolutionY.X = 150;
                ResolutionY.Y = 450;
                ResolutionY.Width = 300;

                Open_ = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            TextBatch.Begin();
            if (Open_)
            {
                ResolutionX.Draw(TextBatch, gameTime);
                ResolutionY.Draw(TextBatch, gameTime);

            }
            base.Draw(gameTime);
            TextBatch.End();
        }


    }
}
