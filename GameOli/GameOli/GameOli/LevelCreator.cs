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
//using TOOLS;

//namespace GameOli
//{
//    public static class LevelCreator
//    {
//        delegate GameLevel LevelCreatorMethod(Game game);

//        public enum Levels
//        {
//            ISLAND,
//            LEVEL1,
//            LEVEL2,
//            LEVEL3,
//            LEVEL4,
//            LEVEL5
//        }

//        //public static LevelCreatorMethod CreateLevel(Game game, Levels levelToUnload, Levels levelToLoad)
//        //{
//            //LevelCreatorMethod levelCreatorMethod;
//            //switch (levelToLoad)
//            //{
//            //    case Levels.ISLAND:
//            //        levelCreatorMethod = CreateIsland(Game game);
//            //        break;
//            //    case Levels.LEVEL1:
//            //        levelCreatorMethod = CreateLevel1(Game game);
//            //        break;
//            //    case Levels.LEVEL2:
//            //        levelCreatorMethod = CreateLevel2(Game game);
//            //        break;
//            //    case Levels.LEVEL3:
//            //        levelCreatorMethod = CreateLevel3(Game game);
//            //        break;
//            //    case Levels.LEVEL4:
//            //        levelCreatorMethod = CreateLevel4(Game game);
//            //        break;
//            //    case Levels.LEVEL5:
//            //        levelCreatorMethod = CreateLevel5(Game game);
//            //        break;
//            //}
//            //return levelCreatorMethod;
//        //}

//        public static GameLevel CreateLevel1(Game game)
//        {
//            GestionnaireDeTextures = new RessourcesManager<Texture2D>(game, "Textures");
//            GestionnaireDeModèles = new RessourcesManager<Model>(game, "Models");
//            Vector3 positionCamera = new Vector3(0, 100, 10);
//            Vector3 cibleCaméra = new Vector3(0, 0, 0);
//            AddCamera(game, positionCamera, cibleCaméra, INTERVALLE_MAJ_STANDARD);
//        }

//        public static void AddCamera(Game game, Vector3 positionCamera, Vector3 cibleCaméra, float intervalleMaj)
//        {
//            CaméraJeu = new CaméraSubjective(game, positionCamera, cibleCaméra, INTERVALLE_MAJ_STANDARD);
//            game.Components.Add(CaméraJeu);
//        }

//        static void AddLight()
//        {
//            gameLevel.LightManager = new LightManager();
//            gameLevel.LightManager.AmbientLightColor = new Vector3(0.1f);

//            // Create the game lights and add them to the light manager
//            gameLevel.LightManager.Add("MainLight",
//               new PointLight(new Vector3(10000, 10000, 10000),
//               new Vector3(0.2f)));
//            gameLevel.LightManager.Add("CameraLight",
//               new PointLight(Vector3.Zero, Vector3.One));

//            // Add the light manager to the service container
//            game.Services.AddService(typeof(LightManager),
//            gameLevel.LightManager);
//        }

//        static void AddTerrain()
//        {
//            gameLevel.Terrain = new Terrain(game);
//            gameLevel.Terrain.Initialize();
//            gameLevel.Terrain.Load("Terrain1", 128, 128, 12.0f, 1.0f);

//            // Create the terrain material and add it to the terrain
//            TerrainMaterial terrainMaterial = new TerrainMaterial();
//            terrainMaterial.LightMaterial = new LightMaterial(
//               new Vector3(0.8f), new Vector3(0.3f), 32.0f);
//            terrainMaterial.DiffuseTexture1 = GetTextureMaterial(
//               game, "Terrain1", new Vector2(40, 40));
//            terrainMaterial.DiffuseTexture2 = GetTextureMaterial(
//               game, "Terrain2", new Vector2(25, 25));
//            terrainMaterial.DiffuseTexture3 = GetTextureMaterial(
//               game, "Terrain3", new Vector2(15, 15));
//            terrainMaterial.DiffuseTexture4 = GetTextureMaterial(
//               game, "Terrain4", Vector2.One);
//            terrainMaterial.AlphaMapTexture = GetTextureMaterial(
//               game, "AlphaMap", Vector2.One);
//            terrainMaterial.NormalMapTexture = GetTextureMaterial(
//               game, "Rockbump", new Vector2(128, 128));
//            gameLevel.Terrain.Material = terrainMaterial;

//            // Add the terrain to the service container
//            game.Services.AddService(typeof(Terrain), gameLevel.Terrain);
//        }

//        static void AddCollisionManager()
//        {
//        }

//        static void AddObjects()
//        {
//        }
//    }

//    public struct GameLevel
//    {
//        public CaméraSubjective Camera;
//        public LightManager LightManager;
//        public Terrain Terrain;
//        public SkyDome SkyDome;

//        // Player and Enemies
//        //public List<BasicSolidObject> BasicSolidObjectList;
//    }
//}