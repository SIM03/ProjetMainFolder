using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TOOLS
{
    public interface IPhysicalObject
    {
        List<BoundingBox> ShellList { get; set; }
        Vector3 Position { get; set; }
        Vector2 Zone { get; set; }
        bool CheckCollison(BoundingBox boîteCollision);
        Matrix GetMonde();
        

    }
}
