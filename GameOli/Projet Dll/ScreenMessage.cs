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
        const int MARGE_VERTICALE = 5;
        const int MARGE_HORIZONTALE = 5;
        const float AUCUNE_ROTATION = 0f;
        const float AUCUNE_HOMOTHÉTIE = 1f;
        const float AVANT_PLAN = 0f;

        float IntervalleMAJ { get; set; }
        float ElapsedTimeSinceUp { get; set; }

        string Title { get; set; }
        string Font { get; set; }
        string Message { get; set; }
        string Parameter { get; set; }
        float OffsetX { get; set; }
        float OffsetY { get; set; }
        Vector2 UpLeftCorner { get; set; }
        Vector2 MessagePosition { get; set; }
        Vector2 Dimension { get; set; }

        CaméraSubjective GameCamera { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        SpriteFont SpriteFont { get; set; }

        public ScreenMessage(Game game, Caméra gameCamera, string title, string parameter, string font, float offsetx, float offsety, float intervalleMAJ)
            : base(game)
        {
            GameCamera = (CaméraSubjective)gameCamera;
            Title = title;
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
            UpLeftCorner = new Vector2(0 + MARGE_HORIZONTALE,
                                            0 + MARGE_VERTICALE);
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
            if (Parameter != null)
                Message = Title + " : " + GameCamera.GetStats(Parameter).ToString();
            else
                Message = Title;
            Dimension = SpriteFont.MeasureString(Message);
            MessagePosition = new Vector2(UpLeftCorner.X + OffsetX, UpLeftCorner.Y + OffsetY);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(SpriteFont, Message, MessagePosition, Color.White, AUCUNE_ROTATION,
                                      Vector2.Zero, AUCUNE_HOMOTHÉTIE, SpriteEffects.None, AVANT_PLAN);
            base.Draw(gameTime);
            GestionSprites.End();
        }
    }
}
