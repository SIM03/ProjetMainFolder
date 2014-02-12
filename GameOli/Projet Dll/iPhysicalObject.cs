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
    }
}
