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
    public class DynamicPhysicalObject : PhysicalObject
    {
        const float GRAVITY_FACTOR = 2;
        float Velocity_X = -1;
        float Velocity_Z = 0;

        protected float Velocity { get; set; }
        protected List<IPhysicalObject> StaticObjectList { get; set; }
        float AirTime { get; set; }
        InputManager InputManager { get; set; }
        
 

        public DynamicPhysicalObject(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, List<IPhysicalObject> staticObjectList)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            StaticObjectList = staticObjectList;
        }



        public override void Initialize()
        {
            // TODO: Add your initialization code here
            InputManager = Game.Services.GetService(typeof(InputManager)) as InputManager;
            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            GererDeplacement();
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ > IntervalleMAJ)
            {
                GravityHandler(gameTime);
                MouvementHandler();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void GererDeplacement()
        {
            Velocity_X = 0;
            Velocity_Z = 0;
            if (InputManager.EstEnfoncée(Keys.Up))
            {
                Velocity_X = 2;
            }
            if (InputManager.EstEnfoncée(Keys.Down))
            {
                Velocity_X = -2;
            }
            if (InputManager.EstEnfoncée(Keys.Left))
            {
                Velocity_Z = 2;
            }
            if (InputManager.EstEnfoncée(Keys.Right))
            {
                Velocity_Z = -2;
            }
            
        }


        protected void GravityHandler(GameTime gametime)
        {
            //On suppose que l'objet tombe pour vérifier si il aura une collision
            AirTime += 0.005f;
            Velocity -= (GRAVITY_FACTOR * AirTime);
            
            Vector3 anciennePosition = Position;
            Position = new Vector3(Position.X, Position.Y + Velocity, Position.Z);
            if (VérifierBoîteDeCollision(anciennePosition))
            {
                Monde *= Matrix.CreateTranslation(new Vector3(0,-(Position.Y - anciennePosition.Y),0));
                Position = new Vector3(Position.X, anciennePosition.Y, Position.Z);
                AirTime = 0;
            }
        }

        protected void MouvementHandler()
        {

            Vector3 anciennePosition = Position;
            Position = new Vector3(Position.X + Velocity_X, Position.Y, Position.Z + Velocity_Z);
            if (VérifierBoîteDeCollision(anciennePosition))
            {
                Monde *= Matrix.CreateTranslation(new Vector3(-(Position.X - anciennePosition.X), 0, -(Position.Z - anciennePosition.Z)));
                Position = new Vector3(anciennePosition.X, Position.Y, anciennePosition.Z);
            }
        }

        private bool VérifierBoîteDeCollision(Vector3 anciennePosition)
        {
            bool CollisionEnVue = false;

            List<BoundingBox> newShellList = new List<BoundingBox>(); // Liste des nouvelles boites en supposant que l'objet tombe
            for (int i = 0; i < ShellList.Count; ++i)
            {
                BoundingBox boîteDeCollisionDuMaillage = ShellList[i];
                Vector3[] listeDesCoins = boîteDeCollisionDuMaillage.GetCorners();
                Monde *= Matrix.CreateTranslation(new Vector3(Position.X - anciennePosition.X, Position.Y - anciennePosition.Y, Position.Z - anciennePosition.Z));
                Vector3.Transform(listeDesCoins,ref Monde, listeDesCoins);
                boîteDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);
                newShellList.Add(boîteDeCollisionDuMaillage);
            }

            foreach (BoundingBox shell in newShellList)
            {
                foreach (IPhysicalObject staticObject in StaticObjectList)
                {
                    CollisionEnVue = staticObject.CheckCollison(shell);
                    if (CollisionEnVue)
                        break;
                }
                
            }
            return CollisionEnVue;
        }

        


    }
}
