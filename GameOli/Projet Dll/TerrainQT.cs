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
    public class TerrainQt : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Vector3 Position { get; set; }
        string TextureTerrainName { get; set; }
        string HeightMapName { get; set; }
        Texture2D HeightTexture { get; set; }

        int IndexCamera { get; set; }
        CaméraSubjective Camera { get; set; }
        Matrix ViewMatrix { get; set; }
        Matrix ProjectionMatrix { get; set; }

        GraphicsDevice Graphic { get; set; }
        RessourcesManager<Texture2D> TextureManager { get; set; }
        InputManager KeyboardManager { get; set; }

        RasterizerState RS_Default { get; set; }
        RasterizerState RS_WireFrame { get; set; }
        bool IsWire { get; set; }

        QuadTree Terrain3d { get; set; }

        public TerrainQt(Game game, Vector3 position, string heightMapName, string textureTerrain, Matrix viewMatrix, Matrix projectionMatrix, GraphicsDevice graphic, int indexCamera)
            : base(game)
        {
            Position = position;
            HeightMapName = heightMapName;
            TextureTerrainName = textureTerrain;
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
            Graphic = graphic;
            IndexCamera = indexCamera;
        }

        public override void Initialize()
        {
            TextureManager = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            KeyboardManager = Game.Services.GetService(typeof(InputManager)) as InputManager;
            HeightTexture = TextureManager.Find(HeightMapName);
            Terrain3d = new QuadTree(Game, Position, HeightTexture, ViewMatrix, ProjectionMatrix, Graphic,2);
            Terrain3d.Effect.Texture = TextureManager.Find(TextureTerrainName);

            // Initialization of both RS for default and wireframe draw mode
            RS_Default = new RasterizerState();
            RS_Default.CullMode = CullMode.CullCounterClockwiseFace;
            RS_Default.FillMode = FillMode.Solid;

            RS_WireFrame = new RasterizerState();
            RS_WireFrame.CullMode = CullMode.CullCounterClockwiseFace;
            RS_WireFrame.FillMode = FillMode.WireFrame;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GetCamera();
            KeyboardHandler();
            Terrain3d.View = Camera.Vue;
            Terrain3d.Projection = Camera.Projection;
            Terrain3d.CameraPosition = Camera.Position;
            Terrain3d.Update(gameTime);
            Game.Window.Title = string.Format(" Triangles Rendered: {0} -||- Culling Enable {1}", Terrain3d.IndexCount / 3, Terrain3d.Cull);
            base.Update(gameTime);
        }

        private void GetCamera()
        {
            Camera = (CaméraSubjective)Game.Components[IndexCamera];
        }

        private void KeyboardHandler()
        {
            if (KeyboardManager.EstNouvelleTouche(Keys.W) && KeyboardManager.EstEnfoncée(Keys.LeftShift))
                IsWire = !IsWire;

            if (KeyboardManager.EstNouvelleTouche(Keys.C) && KeyboardManager.EstEnfoncée(Keys.LeftShift))
                Terrain3d.Cull = !Terrain3d.Cull;
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState OldRS = Graphic.RasterizerState;
            
            Graphic.RasterizerState = RS_Default;
            if (IsWire)
                Graphic.RasterizerState = RS_WireFrame;

            Terrain3d.Draw(gameTime);

            Graphic.RasterizerState = OldRS;
            base.Draw(gameTime);
        }
    }
}
