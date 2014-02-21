using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TOOLS
{
    public class CaméraSubjectivePhysique : CaméraSubjective
    {
        const float RAYON_COLLISION = 1f;

        List<IPhysicalObject> StaticObjectList { get; set; }
        List<DynamicPhysicalObject> DynamicObjectList { get; set; }

        CollisionManager CollisionManagerTest { get; set; }
        public Vector2 Zone { get; set; }
        public BoundingBox BoîteCollision { get; set; }
        bool ActiveGrid { get; set; }


        bool CollisionParSphère_;
        bool CollisionParSphère
        {
            get { return CollisionParSphère_; }
            set
            {
                CollisionParSphère_ = value;
                if (CollisionParSphère_)
                {
                    Game.Window.Title = "Utilisation des sphères de collision...";
                }
                else
                {
                    Game.Window.Title = "Utilisation des boîtes de collision...";
                }
            }
        }

        public CaméraSubjectivePhysique(Game jeu, Vector3 positionCaméra, Vector3 cible, List<IPhysicalObject> staticObjectList, List<DynamicPhysicalObject> dynamicObjectList, float intervalleMAJ)
            : base(jeu, positionCaméra, cible, intervalleMAJ)
        {
            StaticObjectList = staticObjectList;
            DynamicObjectList = dynamicObjectList;
            CollisionParSphère = false;
        }

        public override void Initialize()
        {
            CollisionManagerTest = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (ActiveGrid)
            {
                Zone = CollisionManagerTest.GetZone(Position);
                GérerDéplacement();
            }
            
            base.Update(gameTime);
        }
            
        protected override void GérerDéplacement()
        {
            Vector3 anciennePosition = Position;
            base.GérerDéplacement();
            BoîteCollision = new BoundingBox(new Vector3(Position.X + 1, Position.Y - 100, Position.Z + 1), new Vector3(Position.X - 1, Position.Y, Position.Z - 1));
            //if (CollisionAppréhendée(Position))
            if(CollisionManagerTest.IsObjectNear(this, StaticObjectList))
            {
                Position = anciennePosition;
            }
        }

        protected override void GravityHandler(GameTime gametime)
        {
            float anciennepositionvertical = Position.Y;
            IsOnFloor = false;
            base.GravityHandler(gametime);
            if (CollisionAppréhendée(Position))
            {
                Position = new Vector3(Position.X, anciennepositionvertical, Position.Z);
                IsOnFloor = true;
                base.GravityHandler(gametime);
                
            }
        }

        protected override void GestionClavier()
        {
            base.GestionClavier();
            if (GestionInput.EstNouvelleTouche(Keys.G))
            {
                if (ActiveGrid)
                {
                    ActiveGrid = false;
                }
                else
                {
                    ActiveGrid = true;
                }
                
            }
            if (GestionInput.EstNouvelleTouche(Keys.B) &&
              (GestionInput.EstEnfoncée(Keys.LeftAlt) || GestionInput.EstEnfoncée(Keys.RightAlt)))
            {
                CollisionParSphère = !CollisionParSphère;
            }
        }

        private bool CollisionAppréhendée(Vector3 nouvellePosition)
        {
            bool CollisionEnVue;

            if (CollisionParSphère)
            {
                CollisionEnVue = VérifierSphèreDeCollision(nouvellePosition);
            }
            else
            {
                CollisionEnVue = VérifierBoîteDeCollision(nouvellePosition);
            }
            return CollisionEnVue;
        }


        private bool VérifierBoîteDeCollision(Vector3 nouvellePosition)
        {
            bool collisionStaticEnVue;
            bool collisionDynamicEnVue;

            BoîteCollision = new BoundingBox(new Vector3(nouvellePosition.X + 1, nouvellePosition.Y - 100, nouvellePosition.Z + 1), new Vector3(nouvellePosition.X - 1, nouvellePosition.Y, nouvellePosition.Z - 1));

            collisionStaticEnVue = false;
            collisionDynamicEnVue = false;
            foreach (IPhysicalObject staticObject in StaticObjectList)
            {
                collisionStaticEnVue = staticObject.CheckCollison(BoîteCollision);
                if (collisionStaticEnVue)
                    break;
            }

            foreach (DynamicPhysicalObject dynamicObject in DynamicObjectList)
            {
                collisionDynamicEnVue = dynamicObject.CheckCollison(BoîteCollision);
                if (collisionDynamicEnVue)
                    break;
            }

            return collisionStaticEnVue || collisionDynamicEnVue;
        }

        private bool VérifierSphèreDeCollision(Vector3 nouvellePosition)
        {
            bool CollisionEnVue;

            BoundingSphere sphèreCollision = new BoundingSphere(nouvellePosition, RAYON_COLLISION);

            CollisionEnVue = false;
            foreach (PhysicalObject obstacle in StaticObjectList)
            {
                if (obstacle.CheckCollison(sphèreCollision))
                {
                    CollisionEnVue = true;
                    break;
                }
            }
            return CollisionEnVue;
        }
    }
}
