using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TOOLS
{

   public class Afficheur2D : Microsoft.Xna.Framework.DrawableGameComponent
   {
      Vector2 Position { get; set; }
      string TextureName { get; set; }
      Texture2D ObjectTexture { get; set; }
      RessourcesManager<Texture2D> TextureManager { get; set; }
      SpriteBatch SpriteManager { get; set; }
      float Scale { get; set; }

      public Color Color { get; set; }
      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }


      public Afficheur2D(Game game, float scale, string nomTexture, Vector2 position)
         : base(game)
      {
         Scale = scale;
         TextureName = nomTexture;
         Position = position;
      }

      protected override void LoadContent()
      {
         TextureManager = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
         ObjectTexture = TextureManager.Find(TextureName);
         SpriteManager = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

         Position = new Vector2(Position.X - ObjectTexture.Width / 2, Position.Y - ObjectTexture.Height / 2);

         Color = Color.White;
         base.LoadContent();
      }


      public override void Draw(GameTime gameTime)
      {

         base.Draw(gameTime);
         SpriteManager.Begin();
         SpriteManager.Draw(ObjectTexture, Position, Color);
         SpriteManager.End();
         

      }
   }

}
