using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TOOLS
{
    class ObjectIntersect : Microsoft.Xna.Framework.Game
    {
        Matrix View { get; set; }
        Matrix Projection { get; set; }

        public ObjectIntersect(Game game, Caméra camera, List<IPhysicalObject> StaticObjects)
        {
            View = camera.Vue;
            Projection = camera.Projection;
        }
    }
}
