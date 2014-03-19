using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TOOLS
{
    public class PlanTexturé : Plan, IPhysicalObject
    {
        RessourcesManager<Texture2D> GestionnaireDeTextures;
        Texture2D TexturePlan;
        protected VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        string NomTexturePlan { get; set; }
        BlendState GestionAlpha { get; set; }
        Vector2 Étendue { get; set; }
        public Vector3 Position { get; set; }
        //CollisionManager CollisionManagerTest { get; set; }
        public List<BoundingBox> ShellList { get; set; }
        public Vector2 Zone { get; set; }

        public PlanTexturé(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexturePlan, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, intervalleMAJ)
        {
            NomTexturePlan = nomTexturePlan;
            Étendue = étendue;
            Position = positionInitiale;
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TexturePlan = GestionnaireDeTextures.Find(NomTexturePlan);
            CreerListeBoites();
            base.LoadContent();
        }

        private void CreerListeBoites()
        {
            ShellList = new List<BoundingBox>();
            BoundingBox boîtePhysique;
            Vector3[] listeDesCoins;
            GetCorners(out listeDesCoins);
            
            Matrix mondeLocal = GetMonde();
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
            boîtePhysique = BoundingBox.CreateFromPoints(listeDesCoins);
            ShellList.Add(boîtePhysique);

            //On crée la primitive (un cube) qui servira à visualiser la boîte de collision et on l'ajoute à la liste des components
            BoîteDeCollision boîteVisuelle = new BoîteDeCollision(Game, this, ShellList[0], Color.Firebrick, 0.01f);
            Game.Components.Insert(Game.Components.Count - 2, boîteVisuelle); // j'utilise "insert" pour insérer les composants avant l'affichage 2D terminal
        }

        private void GetCorners(out Vector3[] listeDesCoins)
        {
            listeDesCoins = new Vector3[8];
            listeDesCoins[0] = PtsSommets[0, 0];
            listeDesCoins[1] = PtsSommets[0, PtsSommets.GetLength(1) - 1];
            listeDesCoins[2] = PtsSommets[PtsSommets.GetLength(0) - 1, 0];
            listeDesCoins[3] = PtsSommets[PtsSommets.GetLength(0) - 1, PtsSommets.GetLength(1) - 1];

            listeDesCoins[5] = new Vector3(PtsSommets[0, PtsSommets.GetLength(1) - 1].X, PtsSommets[0, PtsSommets.GetLength(1) - 1].Y, PtsSommets[0, PtsSommets.GetLength(1) - 1].Z);
            listeDesCoins[6] = new Vector3(PtsSommets[PtsSommets.GetLength(0) - 1, 0].X, PtsSommets[PtsSommets.GetLength(0) - 1, 0].Y, PtsSommets[PtsSommets.GetLength(0) - 1, 0].Z);
            listeDesCoins[7] = new Vector3(PtsSommets[PtsSommets.GetLength(0) - 1, PtsSommets.GetLength(1) - 1].X, PtsSommets[PtsSommets.GetLength(0) - 1, PtsSommets.GetLength(1) - 1].Y, PtsSommets[PtsSommets.GetLength(0) - 1, PtsSommets.GetLength(1) - 1].Z);
            listeDesCoins[4] = new Vector3(PtsSommets[0, 0].X, PtsSommets[0, 0].Y, PtsSommets[0, 0].Z);
        }

        protected override void CréerTableauSommets()
        {
            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
            CréerTableauPointsTexture();
            Sommets = new VertexPositionTexture[NbSommets];
        }

        protected override void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TexturePlan;
            GestionAlpha = BlendState.AlphaBlend;
        }

        private void CréerTableauPointsTexture()
        {
            float positionX = 0;
            float deltaX = 1 / (float)NbColonnes;
            float deltaY = 1 / (float)NbRangées;

            for (int i = 0; i <= NbColonnes; i++)
            {
                float positionY = 1;
                for (int j = 0; j <= NbRangées; j++)
                {
                    PtsTexture[i, j] = new Vector2(positionX, positionY);
                    positionY -= deltaY;
                }
                positionX += deltaX;
            }
        }

        protected override void InitialiserSommets() // Est appelée par base.Initialize()
        {
            int noSommet = -1;
            for (int j = 0; j < NbRangées; ++j)
            {
                for (int i = 0; i <= NbColonnes; ++i)
                {
                    Sommets[++noSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++noSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Zone = new Vector2(3, 3);
            BlendState blendState = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = GestionAlpha;
            base.Draw(gameTime);
            GraphicsDevice.BlendState = blendState;
        }

        protected override void DessinerTriangleStrip(int noStrip)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, (NbTrianglesParStrip + 2) * noStrip, NbTrianglesParStrip);
        }



        public bool CheckCollison(BoundingBox boîteCollision)
        {
            //ShellList = new List<BoundingBox>();
            //ShellList.Add(new BoundingBox(new Vector3(Position.X + Étendue.X / 2, Position.Y + Étendue.Y/2, Position.Z), new Vector3(Position.X - Étendue.X / 2, Position.Y - Étendue.Y/2, Position.Z - 1)));
            bool collision = false;
            foreach (BoundingBox shell in ShellList)
            {
                BoundingBox collisionBox = shell;
                //Vector3[] listeDesCoins = collisionBox.GetCorners();
                //Matrix mondeLocal = GetMonde();
                //Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
                //collisionBox = BoundingBox.CreateFromPoints(listeDesCoins);
                if (collisionBox.Intersects(boîteCollision))
                {
                    collision = true;
                    break;
                }
            }
            return collision;
        }

    }
}