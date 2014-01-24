using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
   class Drapeau : PlanTexturé
   {
      const int NB_CYCLES_PAR_PÉRIODE = 100;

      float MaxVariation { get; set; }

      float IntervalleVariation { get; set; }

      float TempsÉcouléDepuisMAJ { get; set; }

      int NoCycle { get; set; }

      public Drapeau(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexturePlan, float maxVariation, float intervalleVariation, float intervalleMAJ)
         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, nomTexturePlan, intervalleMAJ)
      {
         MaxVariation = maxVariation;
         IntervalleVariation = 0.01f; // intervalleVariation;
      }

      public override void Initialize()
      {
         TempsÉcouléDepuisMAJ = 0;
         NoCycle = 0;
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         TempsÉcouléDepuisMAJ += tempsÉcoulé;
         if (TempsÉcouléDepuisMAJ >= IntervalleVariation)
         {
            Onduler();
            ++NoCycle;
            TempsÉcouléDepuisMAJ = 0;
         }
         base.Update(gameTime);
      }

      private void Onduler()
      {
         int noSommet = -1;
         for (int j = 0; j < NbRangées; ++j)
         {
            for (int i = 0; i <= NbColonnes; ++i)
            {
               float variationPosition = i == 0 ? 0 : Varier(i) * MaxVariation;
               Sommets[++noSommet].Position.Z = variationPosition;
               Sommets[++noSommet].Position.Z = variationPosition;
            }
         }
      }

      private float Varier(int index)
      {
         double angle = (NoCycle % NB_CYCLES_PAR_PÉRIODE) * (2 * Math.PI) / NB_CYCLES_PAR_PÉRIODE;
         double décalage = index * (2 * Math.PI) / NbColonnes;
         return (float)Math.Sin(angle + décalage);
      }

      public override void Draw(GameTime gameTime)
      {
         RasterizerState rasterizerState = GraphicsDevice.RasterizerState;
         GraphicsDevice.RasterizerState = new RasterizerState()
         {
            CullMode = CullMode.None,
            FillMode = rasterizerState.FillMode
         };
         base.Draw(gameTime);
         GraphicsDevice.RasterizerState = rasterizerState;
      }
   }
}
