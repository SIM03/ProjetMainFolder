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
        string NomMod�le { get; set; }
        protected float IntervalleMAJ { get; set; }
        protected float Temps�coul�DepuisMAJ { get; set; }
        protected Model Mod�le { get; private set; }
        protected Matrix[] TransformationsMod�le { get; private set; }
        protected Matrix Monde;
        public Vector3 Position { get; set; }
        public float �chelle { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        InputManager GestionInput { get; set; }
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }
        Cam�ra Cam�raJeu { get; set; }
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

        public PhysicalObject(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu)
        {
            NomMod�le = nomMod�le;
            Position = positionInitiale;
            �chelle = �chelleInitiale;
            Rotation = rotationInitiale;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;
            Cam�raJeu = Game.Services.GetService(typeof(Cam�raSubjective)) as Cam�ra;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionnaireDeMod�les = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Mod�le = GestionnaireDeMod�les.Find(NomMod�le);
            TransformationsMod�le = new Matrix[Mod�le.Bones.Count];
            Mod�le.CopyAbsoluteBoneTransformsTo(TransformationsMod�le);
            Angle = 0;
            Pause = true;
            Monde = Matrix.Identity *
                    Matrix.CreateScale(�chelle) *
                    Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                    Matrix.CreateTranslation(Position);
                    
            Cr�erListeDesB�ites();
            //VisualiserSph�reDeCollision();
            CollisionManagerTest = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            G�rerClavier();
            if (Temps�coul�DepuisMAJ > IntervalleMAJ)
            {
                //if (!Pause)
                //{
                //    Monde = Matrix.Identity * Matrix.CreateScale(�chelle);
                //    Monde *= Matrix.CreateFromYawPitchRoll(Angle, Rotation.X, Rotation.Z);
                //    Monde *= Matrix.CreateTranslation(Position);
                //}
                Zone = CollisionManagerTest.GetZone(Position);
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Space))
            {
                Pause = !Pause;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            
            foreach (ModelMesh maille in Mod�le.Meshes)
            {
                Matrix mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = Cam�raJeu.Projection;
                    effet.View = Cam�raJeu.Vue;
                    effet.World = mondeLocal;
                }
                maille.Draw();
            }
            base.Draw(gameTime);
        }

        //public void VisualiserSph�reDeCollision()
        //{
        //    foreach (ModelMesh maille in Mod�le.Meshes)
        //    {
        //        BoundingSphere sph�reDeCollision = maille.BoundingSphere;
        //        Matrix mondeLocal = maille.ParentBone.Transform;
        //        sph�reDeCollision = sph�reDeCollision.Transform(mondeLocal);
        //        sph�reDeCollision = sph�reDeCollision.Transform(Monde);
        //        Sph�reDeCollision Sph�re = new Sph�reDeCollision(Game, sph�reDeCollision.Center, sph�reDeCollision.Radius, new Vector2(12, 12), "rouge", 0.01f);
        //        Sph�re.Visible = false;
        //        Game.Components.Insert(Game.Components.Count - 2, Sph�re); // j'utilise "insert" pour ins�rer les composants avant l'affichage 2D terminal
        //    }
        //}

        public virtual Matrix GetMonde()
        {
            return Monde;
        }

        public bool CheckCollison(BoundingSphere sph�reCollision)
        {
            bool collision = false;
            for (int i = 0; i < Mod�le.Meshes.Count; ++i)
            {
                BoundingSphere sph�reCollisionDuMaillage = Mod�le.Meshes[i].BoundingSphere;
                if (sph�reCollisionDuMaillage.Transform(Monde).Intersects(sph�reCollision))
                {
                    collision = true;
                    break;
                }
            }
            return collision;
        }

        public bool CheckCollison(BoundingBox bo�teCollision)
        {
            bool collision = false;
            for (int i = 0; i < ShellList.Count; ++i)
            {
                BoundingBox bo�teDeCollisionDuMaillage = ShellList[i];
                Vector3[] listeDesCoins = bo�teDeCollisionDuMaillage.GetCorners();
                Matrix mondeLocal = GetMonde();
                Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
                bo�teDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);
                if (bo�teDeCollisionDuMaillage.Intersects(bo�teCollision))
                {
                    collision = true;
                    break;
                }
            }
            return collision;
        }


        protected void Cr�erListeDesB�ites()
        {
            ShellList = new List<BoundingBox>();
            // on cr�� un tableau contenant la liste des sommets du mod�les
            ModelMeshPart portionMaillage = Mod�le.Meshes[0].MeshParts[0];
            int tailleSommetEnFloat = portionMaillage.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
            int tailleTamponEnFloat = portionMaillage.VertexBuffer.VertexCount * tailleSommetEnFloat;

            // On va chercher le contenu du tampon de sommets (vertex buffer) en float pour que cela fonctionne 
            // avec les diff�rents formats de sommets (VertexPositionNormalTexture, VertexPositionTexture, VertexPositionColor...)
            float[] sommetsDuMod�les = new float[tailleTamponEnFloat];
            portionMaillage.VertexBuffer.GetData<float>(sommetsDuMod�les);
            int d�but = 0;
            //On cr�e une bo�te de collision par maillage (un mod�le contient de 1 � N maillages)
            foreach (ModelMesh maillage in Mod�le.Meshes)
            {
                //On cr�e la bo�te de collision (BoundingBox) correspondant � une partie du mod�le et on l'ajoute � la liste des bo�tes de collision
                BoundingBox bo�teCollision = CalculerBoundingBox(maillage, sommetsDuMod�les, tailleSommetEnFloat, ref d�but);
                ShellList.Add(bo�teCollision);
                //On cr�e la primitive (un cube) qui servira � visualiser la bo�te de collision et on l'ajoute � la liste des components
                Bo�teDeCollision bo�te = new Bo�teDeCollision(Game, this, bo�teCollision, Color.Blue, 0.01f);
                Game.Components.Insert(Game.Components.Count - 2, bo�te); // j'utilise "insert" pour ins�rer les composants avant l'affichage 2D terminal
            }
        }

        private BoundingBox CalculerBoundingBox(ModelMesh maillage, float[] sommetsDuMod�le, int tailleSommetEnFloat, ref int d�but)
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
            int fin = d�but + nbSommets * tailleSommetEnFloat;
            // on parcourt la portion de la liste des sommets correspondant � cette partie du mod�le
            for (int indiceSommet = d�but; indiceSommet < fin && indiceSommet < sommetsDuMod�le.Length; indiceSommet += tailleSommetEnFloat)
            {
                Vector3 sommet = new Vector3(sommetsDuMod�le[indiceSommet], sommetsDuMod�le[indiceSommet + 1], sommetsDuMod�le[indiceSommet + 2]);
                positionSommets[index] = sommet;
                minMaillage = Vector3.Min(sommet, minMaillage);
                maxMaillage = Vector3.Max(sommet, maxMaillage);
                ++index;
            }
            d�but = fin;
            BoundingBox bo�te = new BoundingBox(minMaillage, maxMaillage);
            Vector3[] listeDesCoins = bo�te.GetCorners();
            Matrix mondeLocal = maillage.ParentBone.Transform;
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
            bo�te = BoundingBox.CreateFromPoints(listeDesCoins);
            return bo�te;
        }

       
        
    }
}
