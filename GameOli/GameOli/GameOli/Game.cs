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
using TOOLS;

namespace GameOli
{
    class Game : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        Caméra CaméraJeu { get; set; }

        public InputManager GestionInput { get; private set; }

        public Game()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            
        }

        
        protected override void Initialize()
        {
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);
            int hello = Window.ClientBounds.Height;
            Components.Add(GestionInput);


            Vector3 positionCaméra = new Vector3(1, 100, 10);
            CaméraJeu = new CaméraSubjective(this, positionCaméra, new Vector3(0, 0, 0), INTERVALLE_MAJ_STANDARD);
            //CaméraJeu = new CaméraFixe(this, positionCaméra, positionTuileDragon, Vector3.Up);
            Components.Add(CaméraJeu);

            Components.Add(new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(512, 50, 512), "Canyon", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD));

            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Caméra), CaméraJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            base.Initialize();
        }
    }
}
