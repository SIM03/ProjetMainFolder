﻿using TOOLS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Terrain : PrimitiveDeBaseAnimée
{
   private const int NB_SOMMETS_PAR_TRIANGLE = 3;
   private const int NB_POINTS_PAR_TUILE = 4;

   public string NomCarteTerrain { get; set; }
   public string NomTextureTerrain { get; set; }
   public int NbNiveauxTexture { get; set; }
   public int NbColonnes { get; set; }
   public int NbRangées { get; set; }
   public Vector2 Delta { get; set; }
   public Vector2 Tuile { get; set; }
   public Vector3 Étendue { get; set; }
   public Vector3 Origine { get; set; }
   public Vector3[,] PtsSommets { get; set; }
   public Color[] DataTexture { get; set; }
   public Texture2D CarteHauteur { get; set; }
   public Texture2D TextureTerrain { get; set; }
   public VertexPositionTexture[] Sommets { get; set; }
   public BasicEffect EffetDeBase { get; set; }
   public RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
   public float DeltaTexture { get; set; }

   public Terrain(Game jeu, float homotéthieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 étendue, string nomCarteTerrain, string nomTextureTerrain, int nbNiveauxTexture, float intervalleMAJ)
      : base(jeu, homotéthieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
   {
      Étendue = étendue;
      NomCarteTerrain = nomCarteTerrain;
      NomTextureTerrain = nomTextureTerrain;
      NbNiveauxTexture = nbNiveauxTexture;
   }

   public override void Initialize()
   {
      GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
      InitialiserTableaux();
      GérerTexture();
      Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2);
      CréerTableaux();
      InitialiserPtsSommets();
      base.Initialize();
   }

   private void InitialiserTableaux()
   {
      CarteHauteur = GestionnaireDeTextures.Find(NomCarteTerrain);
      NbColonnes = CarteHauteur.Width;
      NbRangées = CarteHauteur.Height;
      DataTexture = new Color[NbColonnes * NbRangées];
      CarteHauteur.GetData<Color>(DataTexture);
      Delta = new Vector2(Étendue.X / NbColonnes, Étendue.Z / NbRangées);
      NbTriangles = (NbColonnes - 1) * (NbRangées - 1) * 2;
      NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;
   }

   private void GérerTexture()
   {
      TextureTerrain = GestionnaireDeTextures.Find(NomTextureTerrain);
      Tuile = new Vector2(TextureTerrain.Width, TextureTerrain.Height / Étendue.Y / NbNiveauxTexture);
      DeltaTexture = 1f / NbNiveauxTexture;
   }

   private void CréerTableaux()
   {
      PtsSommets = new Vector3[NbColonnes + 1, NbRangées + 1];
      Sommets = new VertexPositionTexture[NbSommets];
   }

   protected override void LoadContent()
   {
      EffetDeBase = new BasicEffect(GraphicsDevice);
      GérerEffet();
      base.LoadContent();
   }

   private void GérerEffet()
   {
      EffetDeBase.TextureEnabled = true;
      EffetDeBase.Texture = TextureTerrain;
   }

   private void InitialiserPtsSommets()
   {
      for (int noColonne = 0; noColonne < NbColonnes; ++noColonne)
      {
         for (int noRangée = 0; noRangée < NbRangées; ++noRangée)
         {
            PtsSommets[noColonne, noRangée] = new Vector3(Origine.X + noColonne * Delta.X, CalculerHauteur(noColonne, NbRangées - 1 - noRangée), Origine.Z - noRangée * Delta.Y);
         }
      }
   }

   private float CalculerHauteur(int noColonne, int noRangée)
   {
      return (float)DataTexture[noColonne + NbRangées * noRangée].R / (float)byte.MaxValue * Étendue.Y;
   }

   protected override void InitialiserSommets()
   {
      float deltaAltitude = (float)((int)Math.Ceiling(Étendue.Y / NbNiveauxTexture) + 1);
      int noSommet = 0;
      for (int noRangée = 0; noRangée < NbRangées - 1; ++noRangée)
      {
         for (int noColonne = 0; noColonne < NbColonnes - 1; ++noColonne)
         {
            float y = (float)(int)((PtsSommets[noColonne, noRangée].Y + PtsSommets[noColonne + 1, noRangée].Y + PtsSommets[noColonne, noRangée + 1].Y + PtsSommets[noColonne + 1, noRangée + 1].Y) / NB_POINTS_PAR_TUILE / deltaAltitude) * DeltaTexture;
            Sommets[noSommet] = new VertexPositionTexture(PtsSommets[noColonne, noRangée], new Vector2(0, y + DeltaTexture));
            Sommets[noSommet + 1] = new VertexPositionTexture(PtsSommets[noColonne, noRangée + 1], new Vector2(0, y));
            Sommets[noSommet + 2] = new VertexPositionTexture(PtsSommets[noColonne + 1, noRangée], new Vector2(1, y + DeltaTexture));
            Sommets[noSommet + 3] = new VertexPositionTexture(PtsSommets[noColonne, noRangée + 1], new Vector2(0, y));
            Sommets[noSommet + 4] = new VertexPositionTexture(PtsSommets[noColonne + 1, noRangée + 1], new Vector2(1, y));
            Sommets[noSommet + 5] = new VertexPositionTexture(PtsSommets[noColonne + 1, noRangée], new Vector2(1, y + DeltaTexture));
            noSommet = noSommet + 6;
         }
      }
   }

   public override void Draw(GameTime gameTime)
   {
      EffetDeBase.World = GetMonde();
      EffetDeBase.View = CaméraJeu.Vue;
      EffetDeBase.Projection = CaméraJeu.Projection;
      foreach (EffectPass effectPass in EffetDeBase.CurrentTechnique.Passes)
      {
         effectPass.Apply();
         GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
      }
      base.Draw(gameTime);
   }
}
