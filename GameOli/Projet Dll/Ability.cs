//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;


//namespace TOOLS
//{
    
//    public class Ability : Microsoft.Xna.Framework.GameComponent
//    {
//        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; } 
//        InputManager GestionInput { get; set; }
//        float IntervalleMAJ { get; set; }
//        float Temps�coul�DepuisMAJ { get; set; }

//        PhysicalObject Column;
//        PhysicalObject Tree;
//        List<IPhysicalObject> StaticObjectList { get; set; }
//        List<PhysicalObject> CreatedObjectList { get; set; }

//        public Ability(Game game, List<IPhysicalObject> staticObjectList)
//            : base(game)
//        {
//            StaticObjectList = staticObjectList;
//        }

        
//        public override void Initialize()
//        {
//            CreatedObjectList = new List<PhysicalObject>();
//            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
//            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
//            base.Initialize();
//        }

//        protected void LoadContent()
//        {
            
//        }
//        public override void Update(GameTime gameTime)
//        {
            
//                if (GestionInput.EstNouveauClicGauche())
//                {
//                    Game.Components.Add(Column = new PhysicalObject(Game, "Column", 125f, new Vector3(0, 0, 0), new Vector3(Cam�ra.Position.X, Cam�ra.Position.Y - 100f, Cam�ra.Position.Z - 55), IntervalleMAJ));
//                    StaticObjectList.Add(Column);
//                    CreatedObjectList.Add(Column);
//                }
//                if (GestionInput.EstNouveauClicDroit())
//                {
//                    Game.Components.Add(Tree = new PhysicalObject(Game, "AlanTree", 0.5f, new Vector3(0,0,0), new Vector3(Cam�ra.Position.X, Cam�ra.Position.Y - 100f, Cam�ra.Position.Z), IntervalleMAJ));
//                    StaticObjectList.Add(Tree);
//                    CreatedObjectList.Add(Tree);
//                }

//            if(GestionInput.EstEnfonc�e(Keys.C))
//            {
//                foreach (PhysicalObject physicalObject in CreatedObjectList)
//                {
//                    physicalObject.Visible = false; 
//                }
                
//            }

            
            


                
//            base.Update(gameTime);
//        }
//    }
//}
