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
        const float OFFSET_Y = 30;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;

        GraphicsDeviceManager GraphicManager { get; set; }
        SpriteBatch SpritesManager { get; set; }

        RessourcesManager<SpriteFont> FontsManager { get; set; }
        RessourcesManager<Texture2D> TexturesManager { get; set; }
        RessourcesManager<Model> ModelsManager { get; set; }
        RessourcesManager<SoundEffect> SoundManager { get; set; }
        RessourcesManager<Song> MusicManager { get; set; }
        CollisionManager CollisionManagerTest { get; set; }
        SoundManager SoundManage { get; set; }

        ScreenNotification SavingMessage { get; set; }
        ScreenNotification LoadingMessage { get; set; }
        ScreenNotification ResetMessage { get; set; }

        Afficheur2D Crosshair { get; set; }
        
        public List<IPhysicalObject> StaticObjectList { get; set; }
        public List<DynamicPhysicalObject> DynamicObjectList { get; set; }
        public CaméraSubjectivePhysique CaméraJeu { get; set; }

        public InputManager InputManage { get; private set; }
        public Game()
        {
            GraphicManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            /// FPS Lock/Unlock (Testing Purpose)
            GraphicManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;

            /// FullScreen Switch
            //GraphicManager.ToggleFullScreen();
        }


        protected override void Initialize()
        {
            #region Manager's Section
            CollisionManagerTest = new CollisionManager(this);
            FontsManager = new RessourcesManager<SpriteFont>(this, "Fonts");
            TexturesManager = new RessourcesManager<Texture2D>(this, "Textures");
            ModelsManager = new RessourcesManager<Model>(this, "Models");
            SoundManager = new RessourcesManager<SoundEffect>(this, "Sounds/SoundEffect");
            MusicManager = new RessourcesManager<Song>(this, "Sounds/Music");
            SpritesManager = new SpriteBatch(GraphicsDevice);
            InputManage = new InputManager(this);
            SoundManage = new SoundManager(this, "Sounds.txt");
            #endregion

            #region Objects related code

            //List initialisation
            StaticObjectList = new List<IPhysicalObject>();
            DynamicObjectList = new List<DynamicPhysicalObject>();

            TextFileManager.LoadPhysicalObjects(this, "level1");
            TextFileManager.LoadTexturedPlans(this, "level1");
            TextFileManager.LoadDynamicObjects(this, "level1");
            TextFileManager.LoadCamera(this, "level1");
            #endregion

            Crosshair = new Afficheur2D(this, 1, "CrossHair2", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2));
            

            /// Adding elements declared to Game
            {
            AddBackground2D();
            Add3DComponents();
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

            foreach (DrawableGameComponent staticObject in StaticObjectList)
                Components.Add(staticObject);

            foreach (DynamicPhysicalObject dynamicObject in DynamicObjectList)
                Components.Add(dynamicObject);
        }

        ///<summary>
        /// Adds the Camera and other useful class ( raytracer, ability, ...)
        /// </summary>
        private void AddUtilitiesComponents()
        {
            // ajout de la caméra physique
            Components.Add(SoundManage);
            Components.Add(InputManage);
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

                Components.Add(new ScreenMessage(this, CaméraJeu, null, "InterpolationModifier", "Calibri16", 0, 0 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                Components.Add(new ScreenMessage(this, CaméraJeu, null, "Sensitivity", "Calibri16", 0, 1 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                Components.Add(new ScreenMessage(this, CaméraJeu, null, "Position", "Calibri16", 0, 2 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                Components.Add(new ScreenMessage(this, CaméraJeu, null, "Direction", "Calibri16", 0, 3 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                Components.Add(new ScreenMessage(this, CaméraJeu, null, "Velocity", "Calibri16", 0, 4 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                Components.Add(new ScreenMessage(this, CaméraJeu, null, "AirTime", "Calibri16", 0, 5 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                Components.Add(new ScreenMessage(this, CaméraJeu, null, "IsOnFloor", "Calibri16", 0, 6 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));

                //Components.Add(new ScreenMessage(this, DynamicObjectList[0], null, "Position", "Calibri16", 0, 0 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, DynamicObjectList[0], null, "Velocity", "Calibri16", 0, 1 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, DynamicObjectList[0], null, "OnGround", "Calibri16", 0, 2 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));
                //Components.Add(new ScreenMessage(this, DynamicObjectList[0], null, "GravityVelocity", "Calibri16", 0, 3 * OFFSET_Y, INTERVALLE_MAJ_STANDARD));

                // Saving Loading Message
                SavingMessage = new ScreenNotification(this, CaméraJeu, "Saved", null, "Calibri16", 0, 0.9f * this.Window.ClientBounds.Height, INTERVALLE_MAJ_STANDARD, 1.0f);
                SavingMessage.Visible = false;
                Components.Add(SavingMessage);

                LoadingMessage = new ScreenNotification(this, CaméraJeu, "Loaded", null, "Calibri16", 0, 0.9f * this.Window.ClientBounds.Height, INTERVALLE_MAJ_STANDARD, 1.0f);
                LoadingMessage.Visible = false;
                Components.Add(LoadingMessage);

                ResetMessage = new ScreenNotification(this, CaméraJeu, "Level reset", null, "Calibri16", 0, 0.9f * this.Window.ClientBounds.Height, INTERVALLE_MAJ_STANDARD, 1.0f);
                ResetMessage.Visible = false;
                Components.Add(ResetMessage);

                // Other Stats
                Components.Add(new AfficheurFPS(this, "Calibri16", INTERVALLE_MAJ_STANDARD));
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
            Services.AddService(typeof(RessourcesManager<SpriteFont>), FontsManager);
            Services.AddService(typeof(RessourcesManager<Texture2D>), TexturesManager);
            Services.AddService(typeof(RessourcesManager<Model>), ModelsManager);
            Services.AddService(typeof(RessourcesManager<SoundEffect>), SoundManager);
            Services.AddService(typeof(RessourcesManager<Song>), MusicManager);
            Services.AddService(typeof(InputManager), InputManage);
            Services.AddService(typeof(SpriteBatch), SpritesManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (InputManage.EstNouvelleTouche(Keys.Escape))
                Exit();
            if (InputManage.EstNouvelleTouche(Keys.F))
                GraphicManager.ToggleFullScreen();
            if (InputManage.EstNouvelleTouche(Keys.F1))
            {
                GraphicManager.PreferredBackBufferHeight = 1080;
                GraphicManager.PreferredBackBufferWidth = 1920;
                GraphicManager.ApplyChanges();
            }
            if (InputManage.EstNouvelleTouche(Keys.F2))
            {
                GraphicManager.PreferredBackBufferHeight = 768;
                GraphicManager.PreferredBackBufferWidth = 1280;
                GraphicManager.ApplyChanges();
            }
            if (InputManage.EstNouvelleTouche(Keys.F3))
            {
                GraphicManager.PreferredBackBufferHeight = 768;
                GraphicManager.PreferredBackBufferWidth = 1366;
                GraphicManager.ApplyChanges();
            }

            if (InputManage.EstNouvelleTouche(Keys.F6))
                QuickSave();

            if (InputManage.EstNouvelleTouche(Keys.F7))
                QuickLoad();

            if (InputManage.EstNouvelleTouche(Keys.R))
                ResetLevel();

            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Update(gameTime);
        }

        private void QuickSave()
        {
            SavingMessage.Visible = true;
        }

        private void QuickLoad()
        {
            LoadingMessage.Visible = true;
        }

        private void ResetLevel()
        {
            ResetMessage.Visible = true;
        }
    }
}

