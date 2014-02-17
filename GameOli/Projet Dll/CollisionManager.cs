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

        List<ObjetDeBasePhysique> StaticObjectList { get; set; }
        List<ObjetDeBasePhysique> MovingObjectList { get; set; }

        Vector2 XY_SubdivisionLevel { get; set; }


         CollisionManager(Game game, float depth, float width,List<ObjetDeBasePhysique> staticObjectList,List<ObjetDeBasePhysique> movingObjectList)
            : base(game)
        {
            X_Tiles = (int)(Math.Floor(width/DELTAX_BASE));
            Z_Tiles = (int)(Math.Floor(depth/DELTAY_BASE));
            DeltaX = width / Z_Tiles;
            DeltaZ = depth / X_Tiles;
            XY_SubdivisionLevel = new Vector2(DeltaX, DeltaZ);
            StaticObjectList = staticObjectList;
            MovingObjectList = movingObjectList;
        }
     
        public override void Update(GameTime gameTime)
        {
           

            base.Update(gameTime);
        }

        public Vector2 GetZone(Vector3 position3D)
        {
            Vector2 position2D = new Vector2(position3D.X, position3D.Z);
            return new Vector2 ((int)(Math.Ceiling(position2D.X / DeltaX)),(int)(Math.Ceiling(position2D.Y / DeltaZ))) ;
        }

        //bool isObjectNear(iPhysicalObject objet)
        //{
        //    Vector2 ZoneObjet = objet.Zone;
        //    Vector2[] RangeTiles = new Vector2[8];

        //    RangeTiles[0] = new Vector2(ZoneObjet.X - 1, ZoneObjet.Y + 1);
        //    RangeTiles[1] = new Vector2(ZoneObjet.X, ZoneObjet.Y + 1);
        //    RangeTiles[2] = new Vector2(ZoneObjet.X + 1, ZoneObjet.Y + 1);
        //    RangeTiles[3] = new Vector2(ZoneObjet.X + 1, ZoneObjet.Y);
        //    RangeTiles[4] = new Vector2(ZoneObjet.X + 1, ZoneObjet.Y - 1);
        //    RangeTiles[5] = new Vector2(ZoneObjet.X, ZoneObjet.Y - 1);
        //    RangeTiles[6] = new Vector2(ZoneObjet.X - 1, ZoneObjet.Y - 1);
        //    RangeTiles[7] = new Vector2(ZoneObjet.X - 1, ZoneObjet.Y);

        //    foreach (iPhysicalObject obj in StaticObjectList)
        //    {
        //        for (int i = 0; i < RangeTiles.Length; i++)
        //        {
        //            if (RangeTiles[i] = obj.Zone)
        //            {
        //                CheckCollison(objet, obj);
        //            }
        //        }
			

        //    }


        //}




        bool CheckCollison(iPhysicalObject objet1, iPhysicalObject objet2)
        {
            bool collision = false;
            foreach (BoundingBox Box in objet1.ShellList)
            {
                foreach (BoundingBox Box2 in objet2.ShellList)
                {
                    collision = Box.Intersects(Box2);
                }

                if (collision)
                {
                    Game.Exit();
                }
            }
            return collision;
        }

        public bool CollideWithFloor()
        {
            return true;
        }

    }
}
