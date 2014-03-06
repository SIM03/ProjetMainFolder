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
        
        const float THROW_VELOCITY = 0.5f;   // (m/s)
        const float MAGNIFICATION_SPEED = 0.001f;
        const float MAXIMUM_SIZE = 3f;
        const float MINIMUM_SIZE = -1f;
        float MagnificationFactor { get; set; }
        float g { get; set; }
        float GravityVelocity { get; set; }
        float Mass { get; set; }
        float K_Energy { get; set; }
        float P_Energy { get; set; }
        float Size { get; set; }
        double AirTime { get; set; }
        double Chrono { get; set; }
        bool OnGround { get; set; }

        InputManager InputManager { get; set; }
        protected List<IPhysicalObject> StaticObjectList { get; set; }

        Vector3 AnciennePositionCamera { get; set; }
        Vector3 Velocity { get; set; }
        Vector3 ReboundVelocity { get; set; }
        Vector3 InitialVelocity { get; set; }
        Vector3 AnciennePosition { get; set; }

        private float reboundFactor;
        public float ReboundFactor { get { return reboundFactor / 100; } set { reboundFactor = value; } } // Facteur de rebond en pourcentage EX: 100% = collision complètement élastique

        private float frictionFactor;
        float FrictionFactor { get { return frictionFactor / 100; } set { frictionFactor = value; } } // Facteur de friction Ex: 100% = l'object ne glisse pas

        bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }

            set
            {
                if (value)
                {
                    CaméraJeu = Game.Services.GetService(typeof(CaméraSubjectivePhysique)) as CaméraSubjectivePhysique;
                }
                selected = value;
            }
        }


        public DynamicPhysicalObject(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, List<IPhysicalObject> staticObjectList, float reboundFactor, float frictionFactor)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            StaticObjectList = staticObjectList;
            ReboundFactor = reboundFactor;
            FrictionFactor = frictionFactor;
        }

        public DynamicPhysicalObject(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, List<IPhysicalObject> staticObjectList, Vector3 initialDirection, float mass, float reboundFactor, float frictionFactor)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            StaticObjectList = staticObjectList;
            Velocity = new Vector3(-initialDirection.X, -initialDirection.Y, initialDirection.Z) * THROW_VELOCITY;
            Mass = mass;
            ReboundFactor = reboundFactor;
            FrictionFactor = frictionFactor;
        }

        public override void Initialize()
        {
            InputManager = Game.Services.GetService(typeof(InputManager)) as InputManager;
            g = -9.8f;
            GravityVelocity = 0;
           
            OnGround = false;
            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            GererDeplacement();
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (TempsÉcouléDepuisMAJ > IntervalleMAJ)
            {
                if (!Selected)
                {
                    PhysicsHandler(gameTime);
                    GravityHandler(gameTime);
                    AnciennePositionCamera = new Vector3(0, 0, 0); // Pour test
                }
                else
                {
                    FollowCamera();
                    ApplySkills();
                }

                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void ApplySkills()
        {
            if (InputManager.EstNouvelleTouche(Keys.P))
            {
                g = -g;
            }

            MagnificationFactor = 1 + InputManager.ScrollingValue() * MAGNIFICATION_SPEED;
            if ((Size + MagnificationFactor) < MAXIMUM_SIZE && (Size + MagnificationFactor) > MINIMUM_SIZE)
            {
                Monde *= Matrix.CreateScale(MagnificationFactor); // Grossit ou rapetisse lobjet
                Size += (MagnificationFactor - 1);
            }
             

            
                
            


        }

        private void FollowCamera()
        {
            
            ////Position = CaméraJeu.Position;
            ////Monde *= Matrix.CreateTranslation(distance);
            //Monde *= Matrix.CreateFromAxisAngle(Vector3.Up, CaméraJeu.RotationLacet);
            //Monde *= Matrix.CreateFromAxisAngle(CaméraJeu.Latéral, CaméraJeu.RotationTangage);
            //Monde *= Matrix.CreateTranslation((new Vector3(CaméraJeu.Direction.X, 0, CaméraJeu.Direction.Z))); // * CaméraJeu.DéplacementDirection)));// + (CaméraJeu.Latéral * CaméraJeu.DéplacementLatéral));

            if (AnciennePositionCamera == new Vector3(0,0,0)) // pour test
            {
                AnciennePositionCamera = new Vector3(CaméraJeu.Position.X, CaméraJeu.Position.Y + g/9.8f, CaméraJeu.Position.Z); // g gives the orientation
            }
            
            Monde *= Matrix.CreateTranslation((CaméraJeu.Position - AnciennePositionCamera));
            AnciennePositionCamera = CaméraJeu.Position;

            
            

        }

        private void PhysicsHandler(GameTime gameTime)
        {
            Chrono += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Chrono > 1)
            {
                Vector3 anciennePosition = Position;
                Position += Velocity;
                CheckNextPosition_Y(anciennePosition);
                CheckNextPosition_X(anciennePosition);
                CheckNextPosition_Z(anciennePosition);
                Position = anciennePosition + Velocity;
            }
            K_Energy = Mass * (float)Math.Pow((double)Velocity.Length(), 2);
            P_Energy = Mass * Position.Y;
        }

        private void GererDeplacement()
        {
            if (InputManager.EstNouvelleTouche(Keys.G))
            {
                g = -g;
            }
            
            if (InputManager.EstEnfoncée(Keys.Up))
            {
                Velocity = new Vector3(2, 0, 0);
            }
            if (InputManager.EstEnfoncée(Keys.Down))
            {
                Velocity = new Vector3(-2, 0, 0);
            }
            if (InputManager.EstEnfoncée(Keys.Left))
            {
                Velocity = new Vector3(0, 0, 2);
            }
            if (InputManager.EstEnfoncée(Keys.Right))
            {
                Velocity = new Vector3(0, 0, -2);
            }

            
            
        }


        protected void GravityHandler(GameTime gametime)
        {
            if (!OnGround)
            {
                AirTime += gametime.ElapsedGameTime.TotalSeconds;
                //float previousGravityVelocity;
                //previousGravityVelocity = GravityVelocity;
                GravityVelocity = ReboundVelocity.Y + (g * (float)AirTime) * 0.2f; // V = a*t

                Velocity = new Vector3(Velocity.X , GravityVelocity, Velocity.Z);    
            }
           
        }


        private void CheckNextPosition_Z(Vector3 anciennePosition)
        {
            anciennePosition = new Vector3(Position.X, Position.Y, anciennePosition.Z);
            if (VérifierBoîteDeCollision(anciennePosition))
            {
                Monde *= Matrix.CreateTranslation(new Vector3(0, 0, -(Position.Z - anciennePosition.Z)));
                Velocity = new Vector3(Velocity.X, Velocity.Y, -Velocity.Z * FrictionFactor);
            }
        }

        private void CheckNextPosition_X(Vector3 anciennePosition)
        {
            anciennePosition = new Vector3(anciennePosition.X, Position.Y, Position.Z);
            if (VérifierBoîteDeCollision(anciennePosition))
            {
                Monde *= Matrix.CreateTranslation(new Vector3(-(Position.X - anciennePosition.X), 0, 0));
                Velocity = new Vector3(-Velocity.X * FrictionFactor, Velocity.Y, Velocity.Z); // 1.5 : perte d<energie collision
            }
        }

        private void CheckNextPosition_Y(Vector3 anciennePosition)
        {
            anciennePosition = new Vector3(Position.X, anciennePosition.Y, Position.Z);
            if (VérifierBoîteDeCollision(anciennePosition))
            {
                Monde *= Matrix.CreateTranslation(new Vector3(0, -(Position.Y - anciennePosition.Y), 0));
                if ((Math.Abs(Velocity.Y) * ReboundFactor > 0.01f))
                {
                    ReboundVelocity = new Vector3(Velocity.X * FrictionFactor, -Velocity.Y * ReboundFactor, Velocity.Z * FrictionFactor); //  4 : facteur de rebond
                    Velocity = new Vector3(Velocity.X * FrictionFactor, Velocity.Y, Velocity.Z * FrictionFactor);
                }
                
                else
                {
                    Velocity = new Vector3(Velocity.X * FrictionFactor, 0, Velocity.Z * FrictionFactor);
                    OnGround = true;
                }
                AirTime = 0;
            }
            else
            {
                OnGround = false;
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
