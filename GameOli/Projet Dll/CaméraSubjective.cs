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


namespace TOOLS
{
   public class CaméraSubjective : Caméra
   {
      const float ACCÉLÉRATION = 0.001f;
      const float VITESSE_INITIALE_ROTATION = 5f;
      const float VITESSE_INITIALE_TRANSLATION = 0.5f;
      const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
      const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
      const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
      const float INTERVALLE_MAJ_STANDARD = 1f / 120f;

      Vector3 Direction { get; set; }
      Vector3 Latéral { get; set; }
      float VitesseTranslation { get; set; }
      float VitesseRotation { get; set; }

      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
      Vector2 OriginalMouseState { get; set; }
      InputManager GestionInput { get; set; }

      bool estEnZoom;
      bool EstEnZoom
      {
         get { return estEnZoom; }
         set
         {
            float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
            estEnZoom = value;
            if (estEnZoom)
            {
               CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            }
            else
            {
               CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            }
         }
      }

      public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, float intervalleMAJ)
         : base(jeu)
      {
         IntervalleMAJ = intervalleMAJ;
         CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
         CréerPointDeVue(positionCaméra, cible, OrientationVerticale);
         EstEnZoom = false;
      }

      public override void Initialize()
      {
         OriginalMouseState = new Vector2(Game.Window.ClientBounds.Center.X, Game.Window.ClientBounds.Center.Y);
         VitesseRotation = VITESSE_INITIALE_ROTATION;
         VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
         TempsÉcouléDepuisMAJ = 0;
         base.Initialize();
         GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
      }

      protected override void CréerPointDeVue()
      {
         Latéral = Vector3.Normalize(Vector3.Cross(Direction, Vector3.Up));
         Direction = Vector3.Normalize(Direction);
         Vue = Matrix.CreateLookAt(Position, Position + Direction, Vector3.Up);
         GénérerFrustum();
      }

      protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
      {
         Position = position;
         Cible = cible;
         Direction = Vector3.Normalize(Cible - Position);
         CréerPointDeVue();
      }

      public override void Update(GameTime gameTime)
      {
         float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         TempsÉcouléDepuisMAJ += TempsÉcoulé;
         GestionClavier();

         if (TempsÉcouléDepuisMAJ >= INTERVALLE_MAJ_STANDARD)
         {
               GérerAccélération();
               GérerDéplacement();
               GérerRotation();
               CréerPointDeVue();

               TempsÉcouléDepuisMAJ = 0;
            
         }
         GestionSouris();
         OriginalMouseState = GestionInput.PositionSouris();
         base.Update(gameTime);
      }

      private int GérerTouche(Keys touche)
      {
         return GestionInput.EstEnfoncée(touche) ? 1 : 0;
      }

      private void GérerAccélération()
      {
         int valAccélération = (GérerTouche(Keys.Subtract) + GérerTouche(Keys.OemMinus)) - (GérerTouche(Keys.Add) + GérerTouche(Keys.OemPlus));
         if (valAccélération != 0)
         {
            IntervalleMAJ += ACCÉLÉRATION * valAccélération;
            IntervalleMAJ = MathHelper.Max(INTERVALLE_MAJ_STANDARD, IntervalleMAJ);
         }
      }

      private void GérerDéplacement()
      {
         float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
         float déplacementLatéral = (GérerTouche(Keys.D) - GérerTouche(Keys.A)) * VitesseTranslation;
         if (déplacementDirection != 0)
            Position += new Vector3(Direction.X, 0, Direction.Z) * déplacementDirection;
         if (déplacementLatéral != 0)
            Position += Latéral * déplacementLatéral;
      }

      private void GérerRotation()
      {
         GérerLacet();
         GérerTangage();
      }

      private void GérerLacet()
      {

          int rotationLacet = (int)((OriginalMouseState.X - GestionInput.PositionSouris().X));
          if (rotationLacet != 0)
            Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Up, DELTA_LACET * rotationLacet * VitesseRotation));
            
      }
      private void GérerTangage()
      {
          int rotationTangage = (int)((OriginalMouseState.Y - GestionInput.PositionSouris().Y));
          if (rotationTangage != 0)
          {
            Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, DELTA_TANGAGE * rotationTangage * VitesseRotation));
            Latéral = Vector3.Normalize(Vector3.Cross(Direction, Vector3.Up));
          }
         
      }

      private void GestionClavier()
      {
         if (GestionInput.EstNouvelleTouche(Keys.Z))
         {
            EstEnZoom = !EstEnZoom;
         }
      }

      private void GestionSouris()
      {
          if (!(GestionInput.PositionSouris().Y <= Game.Window.ClientBounds.Top || GestionInput.PositionSouris().Y >= Game.Window.ClientBounds.Bottom))
          {
              Mouse.SetPosition((int)GestionInput.PositionSouris().X, (int)Game.Window.ClientBounds.Center.Y);
          }
          if (!(GestionInput.PositionSouris().X >= Game.Window.ClientBounds.Right || GestionInput.PositionSouris().X <= Game.Window.ClientBounds.Left))
          {
              Mouse.SetPosition((int)Game.Window.ClientBounds.Center.X, (int)GestionInput.PositionSouris().Y);
          }
      }
   }
}
