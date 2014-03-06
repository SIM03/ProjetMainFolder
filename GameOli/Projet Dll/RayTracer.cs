using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TOOLS
{
    public class RayTracer : Microsoft.Xna.Framework.GameComponent
    {
        Matrix View { get; set; }
        Matrix Projection { get; set; }

        GraphicsDevice Graphics { get; set; }
        List<DynamicPhysicalObject> Models { get; set; }
        Caméra GameCamera { get; set; }

        InputManager MouseManager { get; set; }
        Afficheur2D Crosshair { get; set; }

        public RayTracer(Game game, Caméra gameCamera, GraphicsDevice graphics, List<DynamicPhysicalObject> models, Afficheur2D crosshair)
            : base(game)
        {
            GameCamera = gameCamera;
            View = GameCamera.Vue;
            Projection = GameCamera.Projection;
            Graphics = graphics;
            Models = models;
            Crosshair = crosshair;
        }

        public override void Initialize()
        {
            Crosshair.Color = Color.RoyalBlue;
            MouseManager = Game.Services.GetService(typeof(InputManager)) as InputManager;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            View = GameCamera.Vue;
            Projection = GameCamera.Projection;
            CheckMouse();
            base.Update(gameTime);
        }

        Ray GetPickRay()
        {
            //Vector3 nearPoint = new Vector3(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2, GameCamera.Position.Z);
            //Vector3 farPoint = new Vector3(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2, GameCamera.Position.Z + 1);

            //nearPoint = Graphics.Viewport.Unproject(nearPoint, GameCamera.Projection, GameCamera.Vue, Matrix.Identity);
            //farPoint = Graphics.Viewport.Unproject(farPoint, GameCamera.Projection, GameCamera.Vue, Matrix.Identity);

            //Vector3 direction = farPoint - nearPoint;
            //Vector3.Normalize(direction);

            //return new Ray(nearPoint, direction);

            float mouseX = Game.Window.ClientBounds.Width / 2;
            float mouseY = Game.Window.ClientBounds.Height / 2;

            Vector3 nearSource = new Vector3(mouseX, mouseY, GameCamera.Position.Z);
            Vector3 farSource = new Vector3(mouseX, mouseY, GameCamera.Position.Z + 1);

            Matrix World = Matrix.Identity;

            Vector3 nearPoint = Graphics.Viewport.Unproject(nearSource, Projection, View, World);
            Vector3 farPoint = Graphics.Viewport.Unproject(farSource, Projection, View, World);

            //Create the Ray
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            return pickRay;
        }

        public void CheckMouse()
        {

            Ray ray = GetPickRay();
            //Distance de sélection
            float selectedDistance = float.MaxValue;

            foreach (DynamicPhysicalObject Obj in Models)
            {
                foreach (BoundingBox Box in Obj.ShellList)
                {
                    Vector3[] listeDesCoins = Box.GetCorners();
                    Matrix mondeLocal = Obj.GetMonde();
                    Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
                    BoundingBox boîteDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);

                    Nullable<float> result = ray.Intersects(boîteDeCollisionDuMaillage);
                    if (Obj.Selected && MouseManager.EstNouveauClicGauche())
                    {
                        Obj.Selected = false;
                    }
                    else
                    {
                        if (result < selectedDistance && MouseManager.EstNouveauClicGauche())
                        {
                            Crosshair.Color = Color.Firebrick;
                            Obj.Selected = true;
                        }
                        else if (result < selectedDistance)
                        {
                            Crosshair.Color = Color.Chartreuse;
                        }
                        else
                        {
                            Crosshair.Color = Color.AliceBlue;
                        }
                    }

                    

                    
                }

            }


        }
    }
}
