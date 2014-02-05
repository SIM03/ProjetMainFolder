using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TOOLS
{
    interface iPhysicalObject
    {
        List<BoundingBox> ShellList { get; set; }
    }
}
