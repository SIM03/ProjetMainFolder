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

namespace GAME
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        const float DIMENSION_X = 512;
        const float DIMENSION_Y = 200;
        const float DIMENSION_Z = 1024;
        Vector2 étenduePlan = new Vector2(DIMENSION_X,DIMENSION_Y);
        Vector2 étenduePlan1 = new Vector2(DIMENSION_X, DIMENSION_X + DIMENSION_Z);
        Vector2 étenduePlan2 = new Vector2(DIMENSION_X / 4, DIMENSION_Y);
        Vector2 charpentePlan = new Vector2(4,3);

        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        Caméra CaméraJeu { get; set; }


        float TempsÉcouléDepuisMAJ { get; set; }

        public InputManager GestionInput { get; private set; }

        public Game()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            IsMouseVisible = false;
            //PériphériqueGraphique.ToggleFullScreen();
        }

        
        protected override void Initialize()
        {
            LevelManager.RemovePhysicalComponents(this);
            CaméraJeu = LevelManager.AddCamera(this, 0);

            Components.Add(CaméraJeu);
            Services.AddService(typeof(Caméra), CaméraJeu);

            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);
            Components.Add(new AfficheurFPS(this, "Arial20",INTERVALLE_MAJ_STANDARD));

            Components.Add(GestionInput);
            Components.Add(new ObjetDeDemo(this, "Floor", 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), INTERVALLE_MAJ_STANDARD));

            //Components.Add(new Terrain(this, 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(512, 50, 1024), "Canyon", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD));
            Components.Add(new AfficheurFPS(this,"Arial20",INTERVALLE_MAJ_STANDARD));

            //Murs Gauche
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, 0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            //Murs Haut Gauche
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, 0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            //Murs Droites
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, 0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            //Murs Haut Droites
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y,0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            //Murs des fonds Bas
            Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, DIMENSION_Y / 2, 3 * -DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, DIMENSION_Y / 2,3 * DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            //Murs des fonds Haut
            Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, (float)1.5 * DIMENSION_Y, 3 * -DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, (float)1.5 * DIMENSION_Y, 3 * DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            //Plafond
            Components.Add(new PlanTexturé(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, 2* DIMENSION_Y, 0), étenduePlan1, charpentePlan, "Roof", INTERVALLE_MAJ_STANDARD));

            //Plancher
            Components.Add(new PlanTexturé(this, 1f, new Vector3(-MathHelper.PiOver2, 0, 0), new Vector3(0,0, 0), étenduePlan1, charpentePlan, "Floor", INTERVALLE_MAJ_STANDARD));

            //Porte
            Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(1, DIMENSION_Y / 2, 3 * (-DIMENSION_Z / 4) + 1), étenduePlan2, charpentePlan, "BlackDoor", INTERVALLE_MAJ_STANDARD));

            Components.Add(new AfficheurFPS(this, "Arial20", INTERVALLE_MAJ_STANDARD));
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(InputManager), GestionInput);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GestionInput.EstNouvelleTouche(Keys.Escape))
            {
                ExcelApp.Quit();
                Exit();
            }

            if (GestionInput.EstNouvelleTouche(Keys.F6))
            {
                ExcelApp.SetCell<int>(2, 4, 1, (int)CaméraJeu.Position.X);
                ExcelApp.SetCell<int>(3, 4, 1, (int)CaméraJeu.Position.Y);
                ExcelApp.SetCell<int>(4, 4, 1, (int)CaméraJeu.Position.Z);

                ExcelApp.SetCell<int>(2, 5, 1, (int)CaméraJeu.Cible.X);
                ExcelApp.SetCell<int>(3, 5, 1, (int)CaméraJeu.Cible.Y);
                ExcelApp.SetCell<int>(4, 5, 1, (int)CaméraJeu.Cible.Z);

                ExcelApp.Save();
            }
            base.Update(gameTime);
        }
    }
}
