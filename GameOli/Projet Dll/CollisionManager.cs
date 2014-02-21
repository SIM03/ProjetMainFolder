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




namespace TOOLS
{

    public class CollisionManager : Microsoft.Xna.Framework.GameComponent
    {

        float deltaX { get; set; }
        float deltaz { get; set; }
        
        //List<IPhysicalObject> StaticObjectlist { get; set; }
        //List<DynamicPhysicalObject> MovingObjectlist { get; set; }

        Vector2 Xy_subdivisionlevel { get; set; }


        public CollisionManager(Game game)
            : base(game)
        {
            
            
            //StaticObjectlist = staticobjectlist;
            //MovingObjectlist = movingobjectlist;
        }

        public override void Update(GameTime gametime)
        {


            base.Update(gametime);
        }

        public Vector2 GetZone(Vector3 position3d)
        {
            Vector2 position2d = new Vector2(position3d.X, position3d.Z);
            return new Vector2((int)(Math.Ceiling(position2d.X / 150f)), (int)(Math.Ceiling(position2d.Y / 150f)));
        }

        public void IsObjectNear(PhysicalObject objet, List<IPhysicalObject> staticobjectlist)
        {
            Vector2 Zoneobjet = objet.Zone;
            Vector2[] RangeTiles = new Vector2[9];

            RangeTiles[0] = new Vector2(Zoneobjet.X - 1, Zoneobjet.Y + 1);
            RangeTiles[1] = new Vector2(Zoneobjet.X, Zoneobjet.Y + 1);
            RangeTiles[2] = new Vector2(Zoneobjet.X + 1, Zoneobjet.Y + 1);
            RangeTiles[3] = new Vector2(Zoneobjet.X + 1, Zoneobjet.Y);
            RangeTiles[4] = new Vector2(Zoneobjet.X + 1, Zoneobjet.Y - 1);
            RangeTiles[5] = new Vector2(Zoneobjet.X, Zoneobjet.Y - 1);
            RangeTiles[6] = new Vector2(Zoneobjet.X - 1, Zoneobjet.Y - 1);
            RangeTiles[7] = new Vector2(Zoneobjet.X - 1, Zoneobjet.Y);
            RangeTiles[8] = new Vector2(Zoneobjet.X, Zoneobjet.Y);

            foreach (PhysicalObject obj in staticobjectlist)
            {
                for (int i = 0; i < RangeTiles.Length; i++)
                {
                    if (RangeTiles[i] == obj.Zone)
                    {
                        foreach (BoundingBox box in obj.ShellList)
                        {
                            objet.CheckCollison(box);
                        }
                    }
                }
            }


        }

        public bool IsObjectNear(CaméraSubjectivePhysique camera, List<IPhysicalObject>staticobjectlist)
        {
            Vector2 Zoneobjet = camera.Zone;
            Vector2[] RangeTiles = new Vector2[8];
            bool objectNear = false;
            RangeTiles[0] = new Vector2(Zoneobjet.X - 1, Zoneobjet.Y + 1);
            RangeTiles[1] = new Vector2(Zoneobjet.X, Zoneobjet.Y + 1);
            RangeTiles[2] = new Vector2(Zoneobjet.X + 1, Zoneobjet.Y + 1);
            RangeTiles[3] = new Vector2(Zoneobjet.X + 1, Zoneobjet.Y);
            RangeTiles[4] = new Vector2(Zoneobjet.X + 1, Zoneobjet.Y - 1);
            RangeTiles[5] = new Vector2(Zoneobjet.X, Zoneobjet.Y - 1);
            RangeTiles[6] = new Vector2(Zoneobjet.X - 1, Zoneobjet.Y - 1);
            RangeTiles[7] = new Vector2(Zoneobjet.X - 1, Zoneobjet.Y);

            for (int i = 0; i < staticobjectlist.Count; i++)
            {
                if (staticobjectlist[i] is PhysicalObject)
                {
                    for (int j = 0; j < RangeTiles.Length; j++)
                    {
                        if (RangeTiles[j] == staticobjectlist[i].Zone)
                        {
                            if (staticobjectlist[i].CheckCollison(camera.BoîteCollision))
                            {
                                objectNear = true;
                            } 
                           
                        }
                    }
                }
            }

            return objectNear; 
            


        }




        bool CheckCollison(PlanTexturé objet1, PlanTexturé objet2)
        {
            bool collision = false;
            foreach (BoundingBox box in objet1.ShellList)
            {
                foreach (BoundingBox box2 in objet2.ShellList)
                {
                    collision = box.Intersects(box2);
                }

                if (collision)
                {
                    Game.Exit();
                }
            }
            return collision;
        }

        public bool collidewithfloor()
        {
            return true;
        }

    }
}
