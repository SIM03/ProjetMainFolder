using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace TOOLS
{
    public class PhysicalObject : Microsoft.Xna.Framework.DrawableGameComponent, IPhysicalObject
    {
        const float VITESSE_INITIALE_ROTATION = 0.01f;
        string NomModèle { get; set; }
        protected float IntervalleMAJ { get; set; }
        protected float TempsÉcouléDepuisMAJ { get; set; }
        protected Model Modèle { get; private set; }
        protected Matrix[] TransformationsModèle { get; private set; }
        protected Matrix Monde;
        public Vector3 Position { get; set; }
        public float Échelle { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        InputManager GestionInput { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        Caméra CaméraJeu { get; set; }
        public List<BoundingBox> ShellList { get; set; }
        bool Pause { get; set; }

        public BoundingBox Shell { get; set; }
        public Vector2 Zone { get; set; }
        CollisionManager CollisionManagerTest { get; set; }

        float angle;
        public float Angle
        {
            get
            {
                angle = MathHelper.WrapAngle(angle + VITESSE_INITIALE_ROTATION);
                return angle;
            }
            set { angle = value; }
        }

        public PhysicalObject(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu)
        {
            NomModèle = nomModèle;
            Position = positionInitiale;
            Échelle = échelleInitiale;
            Rotation = rotationInitiale;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            CaméraJeu = Game.Services.GetService(typeof(CaméraSubjective)) as Caméra;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Modèle = GestionnaireDeModèles.Find(NomModèle);
            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
            Angle = 0;
            Pause = true;
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Échelle) *
                    Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                    Matrix.CreateTranslation(Position);
                    
            CréerListeDesBôites();
            //VisualiserSphèreDeCollision();
            CollisionManagerTest = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            GérerClavier();
            if (TempsÉcouléDepuisMAJ > IntervalleMAJ)
            {
                //if (!Pause)
                //{
                //    Monde = Matrix.Identity * Matrix.CreateScale(Échelle);
                //    Monde *= Matrix.CreateFromYawPitchRoll(Angle, Rotation.X, Rotation.Z);
                //    Monde *= Matrix.CreateTranslation(Position);
                //}
                Zone = CollisionManagerTest.GetZone(Position);
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Space))
            {
                Pause = !Pause;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            
            foreach (ModelMesh maille in Modèle.Meshes)
            {
                Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = CaméraJeu.Projection;
                    effet.View = CaméraJeu.Vue;
                    effet.World = mondeLocal;
                }
                maille.Draw();
            }
            base.Draw(gameTime);
        }

        //public void VisualiserSphèreDeCollision()
        //{
        //    foreach (ModelMesh maille in Modèle.Meshes)
        //    {
        //        BoundingSphere sphèreDeCollision = maille.BoundingSphere;
        //        Matrix mondeLocal = maille.ParentBone.Transform;
        //        sphèreDeCollision = sphèreDeCollision.Transform(mondeLocal);
        //        sphèreDeCollision = sphèreDeCollision.Transform(Monde);
        //        SphèreDeCollision Sphère = new SphèreDeCollision(Game, sphèreDeCollision.Center, sphèreDeCollision.Radius, new Vector2(12, 12), "rouge", 0.01f);
        //        Sphère.Visible = false;
        //        Game.Components.Insert(Game.Components.Count - 2, Sphère); // j'utilise "insert" pour insérer les composants avant l'affichage 2D terminal
        //    }
        //}

        public virtual Matrix GetMonde()
        {
            return Monde;
        }

        public bool CheckCollison(BoundingSphere sphèreCollision)
        {
            bool collision = false;
            for (int i = 0; i < Modèle.Meshes.Count; ++i)
            {
                BoundingSphere sphèreCollisionDuMaillage = Modèle.Meshes[i].BoundingSphere;
                if (sphèreCollisionDuMaillage.Transform(Monde).Intersects(sphèreCollision))
                {
                    collision = true;
                    break;
                }
            }
            return collision;
        }

        public bool CheckCollison(BoundingBox boîteCollision)
        {
            bool collision = false;
            for (int i = 0; i < ShellList.Count; ++i)
            {
                BoundingBox boîteDeCollisionDuMaillage = ShellList[i];
                Vector3[] listeDesCoins = boîteDeCollisionDuMaillage.GetCorners();
                Matrix mondeLocal = GetMonde();
                Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
                boîteDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);
                if (boîteDeCollisionDuMaillage.Intersects(boîteCollision))
                {
                    collision = true;
                    break;
                }
            }
            return collision;
        }


        protected void CréerListeDesBôites()
        {
            ShellList = new List<BoundingBox>();
            // on créé un tableau contenant la liste des sommets du modèles
            ModelMeshPart portionMaillage = Modèle.Meshes[0].MeshParts[0];
            int tailleSommetEnFloat = portionMaillage.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
            int tailleTamponEnFloat = portionMaillage.VertexBuffer.VertexCount * tailleSommetEnFloat;

            // On va chercher le contenu du tampon de sommets (vertex buffer) en float pour que cela fonctionne 
            // avec les différents formats de sommets (VertexPositionNormalTexture, VertexPositionTexture, VertexPositionColor...)
            float[] sommetsDuModèles = new float[tailleTamponEnFloat];
            portionMaillage.VertexBuffer.GetData<float>(sommetsDuModèles);
            int début = 0;
            //On crée une boîte de collision par maillage (un modèle contient de 1 à N maillages)
            foreach (ModelMesh maillage in Modèle.Meshes)
            {
                //On crée la boîte de collision (BoundingBox) correspondant à une partie du modèle et on l'ajoute à la liste des boîtes de collision
                BoundingBox boîteCollision = CalculerBoundingBox(maillage, sommetsDuModèles, tailleSommetEnFloat, ref début);
                ShellList.Add(boîteCollision);
                //On crée la primitive (un cube) qui servira à visualiser la boîte de collision et on l'ajoute à la liste des components
                BoîteDeCollision boîte = new BoîteDeCollision(Game, this, boîteCollision, Color.Blue, 0.01f);
                Game.Components.Insert(Game.Components.Count - 2, boîte); // j'utilise "insert" pour insérer les composants avant l'affichage 2D terminal
            }
        }

        private BoundingBox CalculerBoundingBox(ModelMesh maillage, float[] sommetsDuModèle, int tailleSommetEnFloat, ref int début)
        {
            int nbSommets = 0;
            foreach (ModelMeshPart portionDeMaillage in maillage.MeshParts)
            {
                nbSommets += portionDeMaillage.NumVertices;
            }
            Vector3 maxMaillage = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 minMaillage = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            int index = 0;
            Vector3[] positionSommets = new Vector3[nbSommets];
            int fin = début + nbSommets * tailleSommetEnFloat;
            // on parcourt la portion de la liste des sommets correspondant à cette partie du modèle
            for (int indiceSommet = début; indiceSommet < fin && indiceSommet < sommetsDuModèle.Length; indiceSommet += tailleSommetEnFloat)
            {
                Vector3 sommet = new Vector3(sommetsDuModèle[indiceSommet], sommetsDuModèle[indiceSommet + 1], sommetsDuModèle[indiceSommet + 2]);
                positionSommets[index] = sommet;
                minMaillage = Vector3.Min(sommet, minMaillage);
                maxMaillage = Vector3.Max(sommet, maxMaillage);
                ++index;
            }
            début = fin;
            BoundingBox boîte = new BoundingBox(minMaillage, maxMaillage);
            Vector3[] listeDesCoins = boîte.GetCorners();
            Matrix mondeLocal = maillage.ParentBone.Transform;
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
            boîte = BoundingBox.CreateFromPoints(listeDesCoins);
            return boîte;
        }

       
        
    }
}
