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
//        //Game lame { get; set; }

//        public Ability(Game game)
//            : base(game)
//        {
//            //Game = game;
//        }

        
//        public override void Initialize()
//        {
//            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
//            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;

//            base.Initialize();
//        }

//        protected void LoadContent()
//        {
            
//        }
//        public override void Update(GameTime gameTime)
//        {
//            ObjetDeBasePhysique Fence;
//                if (GestionInput.EstNouveauClicGauche())
//                {
//                    Game.Components.Add(Fence = new ObjetDeBasePhysique(Game, "fence", 1f, new Vector3(0, 0, 0), Cam�ra.Position, IntervalleMAJ));
//                }
//                if (GestionInput.EstNouveauClicDroit())
//                {
//                    Game.Components.Add(new ObjetDeBasePhysique(Game, "AlanTree", 0.5f, new Vector3(0,0,0), new Vector3(Cam�ra.Position.X, Cam�ra.Position.Y - 100f, Cam�ra.Position.Z), IntervalleMAJ));
//                }
            


                
//            base.Update(gameTime);
//        }
//    }
//}
