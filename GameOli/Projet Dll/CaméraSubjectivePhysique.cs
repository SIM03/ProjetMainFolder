using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TOOLS
{
    public class CaméraSubjectivePhysique : CaméraSubjective
    {
        const float RAYON_COLLISION = 1f;

        List<IPhysicalObject> StaticObjectList { get; set; }

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

        public CaméraSubjectivePhysique(Game jeu, Vector3 positionCaméra, Vector3 cible, List<IPhysicalObject> staticObjectList, float intervalleMAJ)
            : base(jeu, positionCaméra, cible, intervalleMAJ)
        {
            StaticObjectList = staticObjectList;
            CollisionParSphère = false;
        }


        protected override void GérerDéplacement()
        {
            Vector3 anciennePosition = Position;
            base.GérerDéplacement();
            if (CollisionAppréhendée(Position))
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
            bool CollisionEnVue;

            BoundingBox boîteCollision = new BoundingBox(new Vector3(nouvellePosition.X + 1, nouvellePosition.Y - 30, nouvellePosition.Z + 1), new Vector3(nouvellePosition.X - 1, nouvellePosition.Y + 20, nouvellePosition.Z - 1));

            CollisionEnVue = false;
            foreach (IPhysicalObject StaticObject in StaticObjectList)
            {
                CollisionEnVue = StaticObject.CheckCollison(boîteCollision);
                if (CollisionEnVue)
                    break;
            }
            return CollisionEnVue;
        }

        private bool VérifierSphèreDeCollision(Vector3 nouvellePosition)
        {
            bool CollisionEnVue;

            BoundingSphere sphèreCollision = new BoundingSphere(nouvellePosition, RAYON_COLLISION);

            CollisionEnVue = false;
            foreach (ObjetDeBasePhysique obstacle in StaticObjectList)
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
