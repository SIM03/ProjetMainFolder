using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TOOLS
{
    public class ScreenMessage : Microsoft.Xna.Framework.DrawableGameComponent
    {
      const int MARGE_BAS = 10;
      const int MARGE_DROITE = 15;
      const float AUCUNE_ROTATION = 0f;
      const float AUCUNE_HOMOTHÉTIE = 1f;
      const float AVANT_PLAN = 0f;

      float IntervalleMAJ { get; set; }
      float ElapsedTimeSinceUp { get; set; }

      string Font { get; set; }
      string Message { get; set; }
      string Parameter { get; set; }
      float OffsetX { get; set; }
      float OffsetY { get; set; }
      Vector2 BottomLeftCorner { get; set; }
      Vector2 MessagePosition { get; set; }
      Vector2 Dimension { get; set; }

      CaméraSubjective obj { get; set; }
      SpriteBatch GestionSprites { get; set; }
      RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
      SpriteFont SpriteFont { get; set; }

      public ScreenMessage(Game game,Caméra obj, string parameter, string font,float offsetx, float offsety, float intervalleMAJ)
         : base(game)
      {
         this.obj = (CaméraSubjective)obj;
         Parameter = parameter;
         Font = font;
         OffsetX = offsetx;
         OffsetY = offsety;
         IntervalleMAJ = intervalleMAJ;
      }

      public override void Initialize()
      {
         ElapsedTimeSinceUp = 0;
         Message = "";
         BottomLeftCorner = new Vector2(Game.Window.ClientBounds.Width - MARGE_DROITE,
                                         Game.Window.ClientBounds.Height - MARGE_BAS);
         base.Initialize();
      }

      protected override void LoadContent()
      {
         GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
         GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
         SpriteFont = GestionnaireDeFonts.Find(Font); 
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
          float timelapse = (float)gameTime.ElapsedGameTime.TotalSeconds;
          ElapsedTimeSinceUp += timelapse;
          if (ElapsedTimeSinceUp >= IntervalleMAJ)
          {
              GetMessage();
              ElapsedTimeSinceUp = 0;
          }
          base.Update(gameTime);
      }

      private void GetMessage()
      {
          Message = Parameter + " : " + obj.GetStats(Parameter).ToString();
          Dimension = SpriteFont.MeasureString(Message);
          MessagePosition = new Vector2((BottomLeftCorner - Dimension).X - OffsetX, (BottomLeftCorner - Dimension).Y - OffsetY);
      }

      public override void Draw(GameTime gameTime)
      {
         GestionSprites.Begin();
         GestionSprites.DrawString(SpriteFont, Message, MessagePosition, Color.Tomato, AUCUNE_ROTATION,
                                   Vector2.Zero, AUCUNE_HOMOTHÉTIE, SpriteEffects.None, AVANT_PLAN);
         base.Draw(gameTime);
         GestionSprites.End();
      }
    }
}
