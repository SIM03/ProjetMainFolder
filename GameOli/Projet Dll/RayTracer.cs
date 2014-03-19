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

      List<Ray> Rays { get; set; }

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

      List<Ray> GetPickRay()
      {
         Rays = new List<Ray>();

         float mouseX = Game.Window.ClientBounds.Width / 2;
         float mouseY = Game.Window.ClientBounds.Height / 2;

         Matrix World = Matrix.Identity;

         for (int i = (int)mouseX - 1; i <= mouseX + 1; i++)
         {
            for (int j = (int)mouseY - 1; j <= mouseY + 1; j++)
            {
               Vector3 nearSource = new Vector3(i, j, GameCamera.Position.Z);
               Vector3 farSource = new Vector3(i, j, GameCamera.Position.Z + 1);

               Vector3 nearPoint = Graphics.Viewport.Unproject(nearSource, Projection, View, World);
               Vector3 farPoint = Graphics.Viewport.Unproject(farSource, Projection, View, World);

               Vector3 direction = farPoint - nearPoint;
               direction.Normalize();
               Ray pickRay = new Ray(nearPoint, direction);

               Rays.Add(pickRay);
            }
         }
         return Rays;
      }

      public void CheckMouse()
      {

         List<Ray> Rays = GetPickRay();

         //Distance de sélection
         float selectedDistance = float.MaxValue;

         Crosshair.Color = Color.DeepSkyBlue;

         foreach (Ray ray in Rays)
         {
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
                     Obj.Selected = false;

                  else if (Obj.Selected)
                     Crosshair.Color = Color.Firebrick;

                  else if (result < selectedDistance && MouseManager.EstNouveauClicGauche())
                  {
                     Crosshair.Color = Color.Firebrick;
                     Obj.Selected = true;
                  }

                  else if (result < selectedDistance && !Obj.Selected)
                  {
                     Crosshair.Color = Color.Chartreuse;
                  }
               }

            }
         }


      }
   }
}
