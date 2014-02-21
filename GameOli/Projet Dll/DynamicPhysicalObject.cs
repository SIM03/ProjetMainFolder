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
<<<<<<< HEAD
        
 
=======
        Cam�raSubjective PlayerCamera { get; set; }


        public bool Selected
        {
            get;
            set
            {
                if (value)
                {
                    PlayerCamera = Game.Services.GetService(typeof(Cam�raSubjective)) as Cam�raSubjective;
                }
                Selected = value;
            }
        }
        
>>>>>>> origin/Ray_Tracing_(Rate_Racing)

        public DynamicPhysicalObject(Game jeu, String nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, List<IPhysicalObject> staticObjectList)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
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
<<<<<<< HEAD
            GererDeplacement();
=======
            //GererDeplacement();
>>>>>>> origin/Ray_Tracing_(Rate_Racing)
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ > IntervalleMAJ)
            {
<<<<<<< HEAD
                GravityHandler(gameTime);
                MouvementHandler();
                Temps�coul�DepuisMAJ = 0;
=======
                if (!Selected)
                {
                    GravityHandler(gameTime);
                    MouvementHandler();
                    Temps�coul�DepuisMAJ = 0;
                }
                else
                {
                    FollowCamera();
                }
>>>>>>> origin/Ray_Tracing_(Rate_Racing)
            }
            base.Update(gameTime);
        }

<<<<<<< HEAD
=======
        private void FollowCamera()
        {
            Monde *= Matrix.CreateTranslation(-Position);
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromAxisAngle(Vector3.Up, PlayerCamera.RotationLacet);
            Monde *= Matrix.CreateFromAxisAngle(PlayerCamera.Lat�ral, PlayerCamera.RotationTangage);
            Monde *= Matrix.CreateTranslation((new Vector3(PlayerCamera.Direction.X, 0, PlayerCamera.Direction.Z * PlayerCamera.D�placementDirection)) + (PlayerCamera.Lat�ral * PlayerCamera.D�placementLat�ral));
            Monde *= Matrix.CreateTranslation(Position);

        }

>>>>>>> origin/Ray_Tracing_(Rate_Racing)
        private void GererDeplacement()
        {
            Velocity_X = 0;
            Velocity_Z = 0;
            if (InputManager.EstEnfonc�e(Keys.Up))
            {
                Velocity_X = 2;
            }
            if (InputManager.EstEnfonc�e(Keys.Down))
            {
                Velocity_X = -2;
            }
            if (InputManager.EstEnfonc�e(Keys.Left))
            {
                Velocity_Z = 2;
            }
            if (InputManager.EstEnfonc�e(Keys.Right))
            {
                Velocity_Z = -2;
            }
            
        }


        protected void GravityHandler(GameTime gametime)
        {
            //On suppose que l'objet tombe pour v�rifier si il aura une collision
            AirTime += 0.005f;
            Velocity -= (GRAVITY_FACTOR * AirTime);
            
            Vector3 anciennePosition = Position;
            Position = new Vector3(Position.X, Position.Y + Velocity, Position.Z);
            if (V�rifierBo�teDeCollision(anciennePosition))
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
            if (V�rifierBo�teDeCollision(anciennePosition))
            {
                Monde *= Matrix.CreateTranslation(new Vector3(-(Position.X - anciennePosition.X), 0, -(Position.Z - anciennePosition.Z)));
                Position = new Vector3(anciennePosition.X, Position.Y, anciennePosition.Z);
            }
        }

        private bool V�rifierBo�teDeCollision(Vector3 anciennePosition)
        {
            bool CollisionEnVue = false;

            List<BoundingBox> newShellList = new List<BoundingBox>(); // Liste des nouvelles boites en supposant que l'objet tombe
            for (int i = 0; i < ShellList.Count; ++i)
            {
                BoundingBox bo�teDeCollisionDuMaillage = ShellList[i];
                Vector3[] listeDesCoins = bo�teDeCollisionDuMaillage.GetCorners();
                Monde *= Matrix.CreateTranslation(new Vector3(Position.X - anciennePosition.X, Position.Y - anciennePosition.Y, Position.Z - anciennePosition.Z));
                Vector3.Transform(listeDesCoins,ref Monde, listeDesCoins);
                bo�teDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);
                newShellList.Add(bo�teDeCollisionDuMaillage);
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
