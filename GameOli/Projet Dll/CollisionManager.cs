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
   
    public class CollisionManager : Microsoft.Xna.Framework.GameComponent
    {
        const float DELTAY_BASE = 10f;
        const float DELTAX_BASE = 10f;

        float DeltaX { get; set; }
        float DeltaZ { get; set; }
        int X_Tiles { get; set; }
        int Z_Tiles { get; set; }

        Vector2 XY_SubdivisionLevel { get; set; }


        public CollisionManager(Game game, float height, float width)
            : base(game)
        {
            X_Tiles = (int)(Math.Floor(width/DELTAX_BASE));
            Z_Tiles = (int)(Math.Floor(height/DELTAY_BASE));
            DeltaX = width / Z_Tiles;
            DeltaZ = height / X_Tiles;
            XY_SubdivisionLevel = new Vector2(DeltaX, DeltaZ);
        }
     
        public override void Update(GameTime gameTime)
        {
           

            base.Update(gameTime);
        }

        public int GetZone(Vector3 position3D)
        {
            Vector2 position2D = new Vector2(position3D.X, position3D.Z);
            return (int)(Math.Ceiling(position2D.X / DeltaX));
        }


        public bool CollideWith()
        {
            return true;
        }

        public bool CollideWithFloor()
        {
            return true;
        }

    }
}
