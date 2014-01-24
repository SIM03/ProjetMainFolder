using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class CaméraSubjective : Camera
   {
      const float ACCÉLÉRATION = 0.001f;
      const float VITESSE_INITIALE_ROTATION = 5f;
      const float VITESSE_INITIALE_TRANSLATION = 0.5f;
      const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
      const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
      const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
      const float INTERVALLE_MAJ_STANDARD = 1f / 60f;

      Vector3 Direction { get; set; }
      Vector3 Latéral { get; set; }
      float VitesseTranslation { get; set; }
      float VitesseRotation { get; set; }

      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
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
         CréerPointDeVue(positionCaméra, cible, Vector3.Up);
         EstEnZoom = false;
      }

      public override void Initialize()
      {
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
         if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
         {
            if (GestionInput.EstEnfoncée(Keys.LeftShift) || GestionInput.EstEnfoncée(Keys.RightShift))
            {
               GérerAccélération();
               GérerDéplacement();
               GérerRotation();
               CréerPointDeVue();
            }
            TempsÉcouléDepuisMAJ = 0;
         }
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
            Position += Direction * déplacementDirection;
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
         int rotationLacet = (GérerTouche(Keys.Left) - GérerTouche(Keys.Right));
         if (rotationLacet != 0)
            Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Up, DELTA_LACET * rotationLacet * VitesseRotation));
      }

      private void GérerTangage()
      {
         int rotationTangage = (GérerTouche(Keys.Down) - GérerTouche(Keys.Up));
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
   }
}
