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
    
    public class Ability : Microsoft.Xna.Framework.GameComponent
    {
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; } 
        InputManager GestionInput { get; set; }
        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }

        Vector3 Position { get; set; }
        DynamicPhysicalObject Column;
        DynamicPhysicalObject TennisBall { get; set; }
        List<IPhysicalObject> StaticObjectList { get; set; }
        List<PhysicalObject> CreatedObjectList { get; set; }
        List<DynamicPhysicalObject> DynamicObjectList { get; set; }
        Cam�raSubjectivePhysique Cam�raJeu { get; set; }

        public Ability(Game game, List<IPhysicalObject> staticObjectList, List<DynamicPhysicalObject> dynamicObjectList, Cam�raSubjectivePhysique cam�raJeu)
            : base(game)
        {
            StaticObjectList = staticObjectList;
            DynamicObjectList = dynamicObjectList;
            Cam�raJeu = cam�raJeu;
        }

        
        public override void Initialize()
        {
            CreatedObjectList = new List<PhysicalObject>();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            base.Initialize();
        }

        protected void LoadContent()
        {
            
        }
        public override void Update(GameTime gameTime)
        {

            //if (GestionInput.EstNouveauClicGauche())
            //{
            //    Game.Components.Add(Column = new DynamicPhysicalObject(Game, "AlanTree", 0.001f, new Vector3(0, 0, 0), Cam�raJeu.Position, IntervalleMAJ, StaticObjectList, Cam�raJeu.Vue.Forward, 10, 100, 2));
            //    DynamicObjectList.Add(Column);
            //    CreatedObjectList.Add(Column);
            //}
            if (GestionInput.EstNouveauClicDroit())
            {
                Position = new Vector3(Cam�raJeu.Position.X + 0.1f * Cam�raJeu.Vue.Forward.X, Cam�raJeu.Position.Y, Cam�raJeu.Position.Z + 0.1f * Cam�raJeu.Vue.Forward.Z);
                Game.Components.Add(TennisBall = new DynamicPhysicalObject(Game, "TennisBall", 0.01f, new Vector3(0, 0, 0), Position, IntervalleMAJ, StaticObjectList, Cam�raJeu.Vue.Forward, 10, 80, 50));
                DynamicObjectList.Add(TennisBall);
                CreatedObjectList.Add(TennisBall);
            }

            if (GestionInput.EstNouvelleTouche(Keys.C))
            {
                foreach (PhysicalObject physicalObject in CreatedObjectList)
                {
                    physicalObject.Visible = !physicalObject.Visible;
                }

            }





            base.Update(gameTime);
        }
    }
}
