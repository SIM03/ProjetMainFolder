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
    class MeshTerrain : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Model Mesh { get; set; }
        HeightMapInfo heightmapInfo { get; set; }
        Matrix projectionMatrix { get; set; }
        Matrix viewMatrix { get; set; }
        BasicEffect Effect { get; set; }
        CaméraSubjective CameraJeu { get; set; }

        public MeshTerrain(Game game, Model mesh, CaméraSubjectivePhysique camera)
            : base(game)
        {
            Mesh = mesh;
            heightmapInfo = mesh.Tag as HeightMapInfo;
            CameraJeu = camera;//(CaméraSubjective)Game.Components[Indexcamera];
            projectionMatrix = camera.Projection;
            viewMatrix = camera.Vue;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh maille in Mesh.Meshes)
            {
                //Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = CameraJeu.Projection;
                    effet.View = CameraJeu.Vue;
                    effet.World = Matrix.Identity;
                }
                maille.Draw();
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper for drawing the terrain model.
        /// </summary>
        void DrawModel(Model model)
        {
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    Effect.World = boneTransforms[mesh.ParentBone.Index];
                    Effect.View = viewMatrix;
                    Effect.Projection = projectionMatrix;

                    Effect.EnableDefaultLighting();
                    Effect.PreferPerPixelLighting = true;

                    // Set the fog to match the black background color
                    Effect.FogEnabled = true;
                    Effect.FogColor = Vector3.Zero;
                    Effect.FogStart = 1000;
                    Effect.FogEnd = 3200;
                }

                mesh.Draw();
            }
        }
    }
}
