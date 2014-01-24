//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;

//namespace TOOLS
//{
//   class PlanTexturé : Plan
//   {
//      RessourcesManager<Texture2D> GestionnaireDeTextures;
//      Texture2D TexturePlan;
//      protected VertexPositionTexture[] Sommets { get; set; }
//      Vector2[,] PtsTexture { get; set; }
//      string NomTexturePlan { get; set; }
//      BlendState GestionAlpha { get; set; }

//      public PlanTexturé(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexturePlan, float intervalleMAJ)
//         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, intervalleMAJ)
//      {
//         NomTexturePlan = nomTexturePlan;
//      }

//      protected override void LoadContent()
//      {
//         GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
//         TexturePlan = GestionnaireDeTextures.Find(NomTexturePlan);
//         base.LoadContent();
//      }

//      protected override void CréerTableauSommets()
//      {
//         PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
//         CréerTableauPointsTexture();
//         Sommets = new VertexPositionTexture[NbSommets];
//      }

//      protected override void InitialiserParamètresEffetDeBase()
//      {
//         EffetDeBase.TextureEnabled = true;
//         EffetDeBase.Texture = TexturePlan;
//         GestionAlpha = BlendState.AlphaBlend;
//      }

//      private void CréerTableauPointsTexture()
//      {
//         float positionX = 0;
//         float deltaX = 1 / (float)NbColonnes;
//         float deltaY = 1 / (float)NbRangées;

//         for (int i = 0; i <= NbColonnes; i++)
//         {
//            float positionY = 1;
//            for (int j = 0; j <= NbRangées; j++)
//            {
//               PtsTexture[i, j] = new Vector2(positionX, positionY);
//               positionY -= deltaY;
//            }
//            positionX += deltaX;
//         }
//      }

//      protected override void InitialiserSommets() // Est appelée par base.Initialize()
//      {
//         int noSommet = -1;
//         for (int j = 0; j < NbRangées; ++j)
//         {
//            for (int i = 0; i <= NbColonnes; ++i)
//            {
//               Sommets[++noSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
//               Sommets[++noSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
//            }
//         }
//      }

//      public override void Draw(GameTime gameTime)
//      {
//         BlendState blendState = GraphicsDevice.BlendState;
//         GraphicsDevice.BlendState = GestionAlpha;
//         base.Draw(gameTime);
//         GraphicsDevice.BlendState = blendState;
//      }

//      protected override void DessinerTriangleStrip(int noStrip)
//      {
//         GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, (NbTrianglesParStrip + 2) * noStrip, NbTrianglesParStrip);
//      }
//   }
//}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class PlanTexturé : Plan
    {
        RessourcesManager<Texture2D> gestionnaireDeTextures;
        Vector2[,] PtsTexture { get; set; }
        protected VertexPositionTexture[] TableauSommets;
        string NomTexturePlan { get; set; }
        Texture2D TexturePlan;
        BlendState GestionAlpha { get; set; }

        public PlanTexturé(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, intervalleMAJ)
        {
            NomTexturePlan = nomTexture;
        }
        protected override void LoadContent()
        {
            gestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TexturePlan = gestionnaireDeTextures.Find(NomTexturePlan);
            base.LoadContent();
        }
        protected override void InitialiserSommets()
        {
            int k = 0;
            for (int j = 0; j < NbRangées; ++j)
            {
                for (int i = 0; i <= NbColonnes; ++i)
                {
                    TableauSommets[k++] = new VertexPositionTexture(TableauPoints[i, j], PtsTexture[i, j]);
                    TableauSommets[k++] = new VertexPositionTexture(TableauPoints[i, (j + 1)], PtsTexture[i, (j + 1)]);
                }
            }
        }
        protected override void CréerTableauSommets()
        {
            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
            TableauSommets = new VertexPositionTexture[NbSommets];
            CréerTableauPointsTexture();
        }
        private void CréerTableauPointsTexture()
        {
            for (int j = 0; j <= NbRangées; j++)
            {
                for (int i = 0; i <= NbColonnes; i++)
                {
                    PtsTexture[i, j] = new Vector2((float)i / NbColonnes, ((float)NbRangées - j) / NbRangées);
                }
            }
        }
        protected override void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TexturePlan;
            GestionAlpha = BlendState.AlphaBlend;
        }
        protected override void DessinerTriangleStrip(int noStrip)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, TableauSommets, noStrip * (NbTrianglesParStrip + 2), NbTrianglesParStrip);
        }
        public override void Draw(GameTime gameTime)
        {
            BlendState oldBlendState = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = GestionAlpha;
            base.Draw(gameTime);
            GraphicsDevice.BlendState = oldBlendState;
        }
    }
}
