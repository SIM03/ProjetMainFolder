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

namespace GAME
{
    public static class LevelManager
    {

        public static void CreateLevel(Game game, int levelToUnload, int levelToLoad)
        {
            //Components.Add(new ObjetDeDemo(this, "Floor", 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), INTERVALLE_MAJ_STANDARD));

            ////Components.Add(new Terrain(this, 1f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(512, 50, 1024), "Canyon", "D�tailsTerrain", 5, INTERVALLE_MAJ_STANDARD));
            //Components.Add(new AfficheurFPS(this,"Arial20",INTERVALLE_MAJ_STANDARD));

            ////Murs Gauche
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, 0), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, DIMENSION_Y / 2, -DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            ////Murs Haut Gauche
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, -DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, 0), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            ////Murs Droites
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, 0), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, DIMENSION_Y / 2, -DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            ////Murs Haut Droites
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, -DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y, DIMENSION_Z / 2), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_X / 2, (float)1.5 * DIMENSION_Y,0), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            ////Murs des fonds Bas
            //Components.Add(new PlanTextur�(this, 1f, Vector3.Zero, new Vector3(0, DIMENSION_Y / 2, 3 * -DIMENSION_Z / 4), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, DIMENSION_Y / 2,3 * DIMENSION_Z / 4), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            ////Murs des fonds Haut
            //Components.Add(new PlanTextur�(this, 1f, Vector3.Zero, new Vector3(0, (float)1.5 * DIMENSION_Y, 3 * -DIMENSION_Z / 4), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, (float)1.5 * DIMENSION_Y, 3 * DIMENSION_Z / 4), �tenduePlan, charpentePlan, "Wall", INTERVALLE_MAJ_STANDARD));

            ////Plafond
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, 2* DIMENSION_Y, 0), �tenduePlan1, charpentePlan, "Roof", INTERVALLE_MAJ_STANDARD));

            ////Plancher
            //Components.Add(new PlanTextur�(this, 1f, new Vector3(-MathHelper.PiOver2, 0, 0), new Vector3(0,0, 0), �tenduePlan1, charpentePlan, "Floor", INTERVALLE_MAJ_STANDARD));

            ////Porte
            //Components.Add(new PlanTextur�(this, 1f, Vector3.Zero, new Vector3(1, DIMENSION_Y / 2, 3 * (-DIMENSION_Z / 4) + 1), �tenduePlan2, charpentePlan, "BlackDoor", INTERVALLE_MAJ_STANDARD));

            //Components.Add(new AfficheurFPS(this, "Arial20", INTERVALLE_MAJ_STANDARD));
            
            //GestionSprites = new SpriteBatch(GraphicsDevice);
            //Services.AddService(typeof(SpriteBatch), GestionSprites);
            //base.Initialize();
        }

        public static void RemovePhysicalComponents(Game game)
        {
            foreach (ObjetDeBase objetDeBase in game.Components)
            {
                game.Components.Remove(objetDeBase);
            }

            foreach (Plan plan in game.Components)
            {
                game.Components.Remove(plan);
            }
        }
        
        public static Cam�raSubjective AddCamera(Game game, int levelToLoad)
        {
            float positionX = Convert.ToInt16(ExcelApp.GetCell(2, 4, levelToLoad + 1));
            float positionY = Convert.ToInt16(ExcelApp.GetCell(3, 4, levelToLoad + 1));
            float positionZ = Convert.ToInt16(ExcelApp.GetCell(4, 4, levelToLoad + 1));

            float targetX = Convert.ToInt16(ExcelApp.GetCell(2, 5, levelToLoad + 1));
            float targetY = Convert.ToInt16(ExcelApp.GetCell(3, 5, levelToLoad + 1));
            float targetZ = Convert.ToInt16(ExcelApp.GetCell(4, 5, levelToLoad + 1));

            float intervalleMAJStandard = 1 / 60f;

            Vector3 cameraPosition = new Vector3(positionX, positionY, positionZ);
            Vector3 cameraTarget = new Vector3(targetX, targetY, targetZ);

            Cam�raSubjective cam�raJeu = new Cam�raSubjective(game, cameraPosition, cameraTarget, intervalleMAJStandard);

            return cam�raJeu;
        }
    }
}