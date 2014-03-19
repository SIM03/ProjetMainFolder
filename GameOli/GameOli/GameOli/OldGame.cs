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
    class Game : Microsoft.Xna.Framework.Game
    {
        const float DIMENSION_X = 64;
        const float DIMENSION_Y = 25;
        const float DIMENSION_Z = 128;
        const float OFFSETY = 150;
        Vector2 étenduePlan = new Vector2(DIMENSION_X, DIMENSION_Y);
        Vector2 étenduePlan1 = new Vector2(DIMENSION_X, DIMENSION_X + DIMENSION_Z);
        Vector2 étenduePlan2 = new Vector2(DIMENSION_X / 4, DIMENSION_Y);
        Vector2 charpentePlan = new Vector2(4, 3);

        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager GraphicManager { get; set; }
        SpriteBatch GestionSprites { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        RessourcesManager<SoundEffect> GestionnaireDeSon { get; set; }
        RessourcesManager<Song> GestionnaireDeMusique { get; set; }
        CaméraSubjectivePhysique CaméraJeu { get; set; }
        CollisionManager CollisionManagerTest { get; set; }
        SoundManager GestionSon { get; set; }

        Afficheur2D Crosshair { get; set; }

        PhysicalObject Cube { get; set; }
        PhysicalObject Cube1 { get; set; }
        PhysicalObject Cube2 { get; set; }
        PhysicalObject Cube3 { get; set; }
        PhysicalObject Cube4 { get; set; }

        DynamicPhysicalObject Tree { get; set; }
        DynamicPhysicalObject Column { get; set; }
        DynamicPhysicalObject TennisBall { get; set; }


        PlanTexturé Floor1 { get; set; }
        PlanTexturé Floor2 { get; set; }
        PlanTexturé Ceilling { get; set; }

        public List<IPhysicalObject> StaticObjectList { get; set; }
        public List<DynamicPhysicalObject> DynamicObjectList { get; set; }

        public InputManager GestionInput { get; private set; }

        public Game()
        {
            GraphicManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            /// FPS Lock/Unlock (Testing Purpose)
            GraphicManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;

            /// FullScreen Switch
            // GraphicManager.ToggleFullScreen();
        }


        protected override void Initialize()
        {
            #region Manager's Section
            CollisionManagerTest = new CollisionManager(this);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionnaireDeSon = new RessourcesManager<SoundEffect>(this, "Sounds/SoundEffect");
            GestionnaireDeMusique = new RessourcesManager<Song>(this, "Sounds/Music");
            GestionSprites = new SpriteBatch(GraphicsDevice);
            GestionInput = new InputManager(this);

            GestionSon = new SoundManager(this, "Sounds.txt");
            #endregion

            #region Objects related code

            //List initialisation
            StaticObjectList = new List<IPhysicalObject>();
            DynamicObjectList = new List<DynamicPhysicalObject>();

            //Staticobject
            Cube = new PhysicalObject(this, "AlanTree", 0.2f, new Vector3(0, 0, 0), new Vector3(1000, 1500, -400), INTERVALLE_MAJ_STANDARD);
            Cube1 = new PhysicalObject(this, "Column", 5, new Vector3(0, 0, 0), new Vector3(-7, -0.5f, -15), INTERVALLE_MAJ_STANDARD);
            Cube2 = new PhysicalObject(this, "Column", 8, new Vector3(0, 0, 0), new Vector3(18, -1, -4), INTERVALLE_MAJ_STANDARD);
            Cube3 = new PhysicalObject(this, "Rock1", 50, new Vector3((float)(Math.PI), 0, 0), new Vector3(0, 0, -3000), INTERVALLE_MAJ_STANDARD);
            Cube4 = new PhysicalObject(this, "fence", 0.1f, new Vector3(0, 0, 0), new Vector3(11, 0, 15), INTERVALLE_MAJ_STANDARD);
            

            //DynamicObject
            Column = new DynamicPhysicalObject(this, "Column", 3f, new Vector3(0, 0, 0), new Vector3(0, 9.8f, -4), INTERVALLE_MAJ_STANDARD, StaticObjectList, 40, 5);
            TennisBall = new DynamicPhysicalObject(this, "TennisBall", 0.01f, new Vector3(0, 0, 0), new Vector3(0, 10, 4), INTERVALLE_MAJ_STANDARD, StaticObjectList, 10, 30);

            #endregion

            #region Room Elements

            //Plafond
            Components.Add(Ceilling = new PlanTexturé(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, 2 * DIMENSION_Y, 0), étenduePlan1, charpentePlan, "Roof", INTERVALLE_MAJ_STANDARD));

            //Plancher
            Floor1 = new PlanTexturé(this, 1f, new Vector3(-MathHelper.PiOver2, 0, 0), new Vector3(0, 0, 0), étenduePlan1, charpentePlan, "Floor", INTERVALLE_MAJ_STANDARD);
            Floor2 = new PlanTexturé(this, 1f, new Vector3(0, 0, 0), new Vector3(-5, 0, 0), new Vector2(10, 10), charpentePlan, "Floor", INTERVALLE_MAJ_STANDARD);

            #endregion

            #region Camera related
            // ajout de la caméra physique
            Vector3 positionCaméra = new Vector3(0, 20, 0);
            CaméraJeu = new CaméraSubjectivePhysique(this, positionCaméra, new Vector3(MathHelper.PiOver2, 0, 0), StaticObjectList, DynamicObjectList, INTERVALLE_MAJ_STANDARD);
            Crosshair = new Afficheur2D(this, 1, "CrossHair2", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2));
            #endregion
            

            /// Adding elements declared to Game
            {
            AddBackground2D();
            Add3DComponents();
            AddObjectsToLists();
            AddUtilitiesComponents();
            AddForeground2D();
            InitializeServices();
            }

            base.Initialize();
        }

        ///<summary>
        /// Adds "Afficheur3D" and 3D object declared in intialization
        /// </summary>
        private void Add3DComponents()
        {
            Components.Add(new Afficheur3D(this));

            //Adding StaticObject
            Components.Add(Cube);
            Components.Add(Cube1);
            Components.Add(Cube2);
            Components.Add(Cube3);
            Components.Add(Cube4);

            //Adding DynamicObject
            Components.Add(Column);
            Components.Add(TennisBall);

            //Adding planes
            Components.Add(Floor1);
            Components.Add(Floor2);
        }


        ///<summary>
        /// Adds 3D Object declared in intialization to their respective lists
        /// </summary>
        private void AddObjectsToLists()
        {
            //Adding StaticObject
            StaticObjectList.Add(Cube);
            StaticObjectList.Add(Cube1);
            StaticObjectList.Add(Cube2);
            // StaticObjectList.Add(Cube3);
            StaticObjectList.Add(Cube4);
            StaticObjectList.Add(Floor1);
            StaticObjectList.Add(Ceilling);
            StaticObjectList.Add(Floor2);


            //Adding DynamicObject
            DynamicObjectList.Add(Column);
            DynamicObjectList.Add(TennisBall);
        }

        ///<summary>
        /// Adds the Camera and other useful class ( raytracer, ability, ...)
        /// </summary>
        private void AddUtilitiesComponents()
        {
            // ajout de la caméra physique
            Components.Add(GestionSon);
            Components.Add(GestionInput);
            Components.Add(CaméraJeu);
            Components.Add(new Ability(this, StaticObjectList, DynamicObjectList, CaméraJeu));
            Components.Add(new RayTracer(this, CaméraJeu, GraphicsDevice, DynamicObjectList, Crosshair));
            Services.AddService(typeof(CaméraSubjectivePhysique), CaméraJeu);
        }

        ///<summary>
        /// Adds 2D elements of the background
        /// </summary>
        private void AddBackground2D()
        {

        }

        /// <summary>
        /// Adds 2D elements of the foreground
        /// </summary>
        private void AddForeground2D()
        {
            /// Test Statistics
            {
                // Camera Stats

                //Components.Add(new ScreenMessage(this, CaméraJeu, "BufferSize", "Arial20", 0,0, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "InterpolationModifier", "Arial20", 0, OFFSETY, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "Sensitivity", "Arial20", 0, 2 * OFFSETY, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "MINIMUM_MOVEMENT", "Arial20", 0, 3 * OFFSETY, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "Position", "Position", "Arial20", 0, 3 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "Direction", "Direction", "Arial20", 0, 4 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "Velocity", "Velocity", "Arial20", 0, 5 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "AirTime", "AirTime", "Arial20", 0, 6 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "IsOnFloor", "IsOnFloor", "Arial20", 0, 7 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, CaméraJeu, "IsJumping", "IsJumping", "Arial20", 0, 8 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));

                // Other Stats
                Components.Add(new AfficheurFPS(this, "Arial20", INTERVALLE_MAJ_STANDARD));
            }

            /// Permanent
            Components.Add(Crosshair);

        }

        /// <summary>
        /// Initialize services during game's initialize
        /// </summary>
        private void InitializeServices()
        {
            Services.AddService(typeof(CollisionManager), CollisionManagerTest);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(RessourcesManager<SoundEffect>), GestionnaireDeSon);
            Services.AddService(typeof(RessourcesManager<Song>), GestionnaireDeMusique);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GestionInput.EstNouvelleTouche(Keys.Escape))
                Exit();
            if (GestionInput.EstNouvelleTouche(Keys.F))
                GraphicManager.ToggleFullScreen();
            if (GestionInput.EstNouvelleTouche(Keys.F1))
            {
                GraphicManager.PreferredBackBufferHeight = 1080;
                GraphicManager.PreferredBackBufferWidth = 1920;
                GraphicManager.ApplyChanges();
            }
            if (GestionInput.EstNouvelleTouche(Keys.F2))
            {
                GraphicManager.PreferredBackBufferHeight = 768;
                GraphicManager.PreferredBackBufferWidth = 1280;
                GraphicManager.ApplyChanges();
            }
            if (GestionInput.EstNouvelleTouche(Keys.F3))
            {
                GraphicManager.PreferredBackBufferHeight = 768;
                GraphicManager.PreferredBackBufferWidth = 1366;
                GraphicManager.ApplyChanges();
            }


            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Update(gameTime);
        }
    }
}





//Vector3 positionCaméra = new Vector3(0, 100, 10);
//CaméraJeu = new CaméraSubjective(this, positionCaméra, new Vector3(0, 0, 0), INTERVALLE_MAJ_STANDARD);
//Components.Add(new ScreenMessage(this, CaméraJeu, "BufferSize", "Arial20", 0, INTERVALLE_MAJ_STANDARD));
//Components.Add(new ScreenMessage(this, CaméraJeu, "InterpolationModifier", "Arial20", 2 * OFFSET, INTERVALLE_MAJ_STANDARD));
//Components.Add(new ScreenMessage(this, CaméraJeu, "Sensitivity", "Arial20", 3 * OFFSET, INTERVALLE_MAJ_STANDARD));
//Components.Add(CaméraJeu);
//Components.Add(new ObjetDeDemo(this, "Floor", 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), INTERVALLE_MAJ_STANDARD));

//Components.Add(new Terrain(this, 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(512, 50, 1024), "Canyon", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD));


//Murs Gauche
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, 0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

//Murs Haut Gauche
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, 0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

//Murs Droites
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, 0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

//Murs Haut Droites
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, -DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, DIMENSION_Z / 2), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y,0), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

//Murs des fonds Bas
//Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, DIMENSION_Y / 2, 3 * -DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, DIMENSION_Y / 2,3 * DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

//Murs des fonds Haut
//  Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, (float)1.5 * DIMENSION_Y, 3 * -DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
//  Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, (float)1.5 * DIMENSION_Y, 3 * DIMENSION_Z / 4), étenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

//Porte

// Components.Add(new PlanTexturé(this, 0.5f, Vector3.Zero, new Vector3(1, DIMENSION_Y / 4, 3 * (-DIMENSION_Z / 4)), étenduePlan2, charpentePlan, "BlackDoor", INTERVALLE_MAJ_STANDARD));
// Components.Add(new ScreenMessage(this, CaméraJeu, "BufferSize", "Arial20", 0,0, INTERVALLE_MAJ_STANDARD));
// Components.Add(new ScreenMessage(this, CaméraJeu, "InterpolationModifier", "Arial20",0 ,OFFSETY, INTERVALLE_MAJ_STANDARD));
// Components.Add(new ScreenMessage(this, CaméraJeu, "Sensitivity", "Arial20",0, 2 * OFFSETY, INTERVALLE_MAJ_STANDARD));
// Components.Add(new ScreenMessage(this, CaméraJeu, "MINIMUM_MOVEMENT", "Arial20",0, 3 * OFFSETY, INTERVALLE_MAJ_STANDARD));

