using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TOOLS
{
    class RayTracer : Microsoft.Xna.Framework.GameComponent
    {
        Matrix View { get; set; }
        Matrix Projection { get; set; }

        GraphicsDevice Graphics { get; set; }
        List<DynamicPhysicalObject> Models { get; set; }

        public RayTracer(Game game, Matrix view, Matrix projection, GraphicsDevice graphics, List<DynamicPhysicalObject> models)
            : base(game)
        {
            View = view;
            Projection = projection;
            Graphics = graphics;
            Models = models;
        }

        Ray GetPickRay()
        {

            MouseState mouseState = Mouse.GetState();

            float mouseX = Game.Window.ClientBounds.Width / 2;
            float mouseY = Game.Window.ClientBounds.Height / 2;

            Vector3 nearSource = new Vector3(mouseX, mouseY, 0f);
            Vector3 farSource = new Vector3(mouseX, mouseY, 1f);

            Matrix World = Matrix.CreateTranslation(0, 0, 0);

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
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Ray ray = GetPickRay();
                //Distance de sélection
                float selectedDistance = float.MaxValue;

                foreach (DynamicPhysicalObject Obj in Models)
                {
                    foreach (BoundingBox Box in Obj.ShellList)
                    {
                        Nullable<float> result = ray.Intersects(Box);
                        if (result < selectedDistance)
                        {

                            Obj.Selected = true;
                        }

                    }

                }

            }
        }
        private void DisableGravity()
        {

        }

    }
}
