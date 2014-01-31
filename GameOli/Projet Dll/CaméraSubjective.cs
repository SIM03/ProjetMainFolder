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
      const float VITESSE_INITIALE_ROTATION = 20f;
      const float VITESSE_INITIALE_TRANSLATION = 10f;
      const float DELTA_LACET = MathHelper.Pi / 10000; // 1 degré à la fois
      const float DELTA_TANGAGE = MathHelper.Pi / 10000; // 1 degré à la fois

      const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
      const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
      const int DEFAULT_BUFFER_SIZE = 10;
      const float DEFAULT_INTERPOLATION = 0.7f;

      Vector3 Direction { get; set; }
      Vector3 Latéral { get; set; }
      float VitesseTranslation { get; set; }
      float VitesseRotation { get; set; }

      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
      Vector2 OriginalMouseState { get; set; }
      InputManager GestionInput { get; set; }
      Queue<Vector2> MouseBuffer { get; set; }
      int BufferSize { get; set; }
      float InterpolationModifier { get; set; }

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
         BufferSize = DEFAULT_BUFFER_SIZE; /* Temporaire pour faute de savoir ou placer l<initialisaton du buffer size */
         InterpolationModifier = DEFAULT_INTERPOLATION;
         MouseBuffer = new Queue<Vector2>();
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
         Vector2 RotationXY = BufferInterpolation();
         GérerLacet(RotationXY.X);
         GérerTangage(RotationXY.Y);
      }

      private void GérerLacet(float RotationX)
      {
          int rotationLacet = (int)RotationX;
          if (rotationLacet != 0)
            Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Up, DELTA_LACET * rotationLacet * VitesseRotation));
      }
      private void GérerTangage(float RotationY)
      {
          int rotationTangage = (int)RotationY;
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
          BufferManagement();
          if (!(GestionInput.PositionSouris().Y <= Game.Window.ClientBounds.Top || GestionInput.PositionSouris().Y >= Game.Window.ClientBounds.Bottom))
          {
              Mouse.SetPosition((int)GestionInput.PositionSouris().X, (int)Game.Window.ClientBounds.Center.Y);
          }
          if (!(GestionInput.PositionSouris().X >= Game.Window.ClientBounds.Right || GestionInput.PositionSouris().X <= Game.Window.ClientBounds.Left))
          {
              Mouse.SetPosition((int)Game.Window.ClientBounds.Center.X, (int)GestionInput.PositionSouris().Y);
          }
      }

      private void BufferManagement()
      {
          MouseBuffer.Enqueue(new Vector2((OriginalMouseState.X - GestionInput.PositionSouris().X),(OriginalMouseState.Y - GestionInput.PositionSouris().Y)));
          if (MouseBuffer.Count > BufferSize)
          {
              while (MouseBuffer.Count > BufferSize)
              {
                  MouseBuffer.Dequeue();
              }
          }
      }

      private Vector2 BufferInterpolation()
      {
          float yValue = 0f;
          float xValue = 0f;
          float weight = 1.0f;
          for (int i = 0; i < MouseBuffer.Count; i++)
          {
              if (MouseBuffer.ElementAt(i) != Vector2.Zero)
              {
                  xValue += (MouseBuffer.ElementAt(i).X * weight);
                  yValue += (MouseBuffer.ElementAt(i).Y * weight);
                  weight *= InterpolationModifier;
              }
          }
          return new Vector2(xValue, yValue);
      }
   }

   //struct BufferTable<T>
   //{
   //    public T[] Table { get; set; }
   //    private int index { get; set;}

   //    public void BufferTable(int Size)
   //    {
   //        Table = new T[Size];
   //        index = -1;
   //    }

   //    // AJoute un element dans le buffer ce qui deplace les dernier elements vers le haut (plus anciens)
   //    public void Push(T ElementToAdd)
   //    {
   //        // avant d'ajouter on doit d'abord pousser les elements vers le haut
   //        int j = index;
   //        while (j >= 0)
   //        {
   //            if (j >= Table.Length)
   //                j = Table.Length - 1;

   //            Table[j + 1] = Table[j--];
   //        }
   //        Table[0] = ElementToAdd;
   //        ++index;
   //    }
   //}
}
