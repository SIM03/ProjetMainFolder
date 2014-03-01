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
    public class ExcelDataManager
    {
        Vector3 CameraPosition { get; set; }
        Vector3 CameraTarget { get; set; }

        Game Jeu { get; set; }

        public ExcelDataManager(Game game)
        {
            Jeu = game;
        }
        
        public Vector3 LoadCameraPosition(int levelToLoad)
        {
            float positionX = Convert.ToInt16(ExcelApp.GetCell(5, 4, levelToLoad + 1));
            float positionY = Convert.ToInt16(ExcelApp.GetCell(6, 4, levelToLoad + 1));
            float positionZ = Convert.ToInt16(ExcelApp.GetCell(7, 4, levelToLoad + 1));

            CameraPosition = new Vector3(positionX, positionY, positionZ);

            return CameraPosition;
        }

        public Vector3 LoadCameraTarget(int levelToLoad)
        {
            float targetX = float.Parse(ExcelApp.GetCell(5, 5, levelToLoad + 1));
            float targetY = float.Parse(ExcelApp.GetCell(6, 5, levelToLoad + 1));
            float targetZ = float.Parse(ExcelApp.GetCell(7, 5, levelToLoad + 1));

            CameraTarget = new Vector3(targetX, targetY, targetZ);

            return CameraTarget;
        }


        public bool LoadIsOnFloor(int levelToLoad)
        {
            return Convert.ToBoolean(ExcelApp.GetCell(3, 6, levelToLoad + 1));
        }


        public bool LoadIsJumping(int levelToLoad)
        {
            return Convert.ToBoolean(ExcelApp.GetCell(3, 7, levelToLoad + 1));
        }


        public float LoadVelocity(int levelToLoad)
        {
            return (float)Convert.ToDouble(ExcelApp.GetCell(3, 8, levelToLoad + 1));
        }


        public float LoadAirTime(int levelToLoad)
        {
            return (float)Convert.ToDouble(ExcelApp.GetCell(3, 9, levelToLoad + 1));
        }


        public Vector3 ResetCameraPosition(int levelToLoad)
        {
            float positionX = Convert.ToInt16(ExcelApp.GetCell(2, 4, levelToLoad + 1));
            float positionY = Convert.ToInt16(ExcelApp.GetCell(3, 4, levelToLoad + 1));
            float positionZ = Convert.ToInt16(ExcelApp.GetCell(4, 4, levelToLoad + 1));

            CameraPosition = new Vector3(positionX, positionY, positionZ);

            return CameraPosition;
        }

        public Vector3 ResetCameraTarget(int levelToLoad)
        {
            float targetX = Convert.ToInt16(ExcelApp.GetCell(2, 5, levelToLoad + 1));
            float targetY = Convert.ToInt16(ExcelApp.GetCell(3, 5, levelToLoad + 1));
            float targetZ = Convert.ToInt16(ExcelApp.GetCell(4, 5, levelToLoad + 1));

            CameraTarget = new Vector3(targetX, targetY, targetZ);

            return CameraTarget;
        }

        public bool ResetIsOnFloor(int levelToLoad)
        {
            return Convert.ToBoolean(ExcelApp.GetCell(2, 6, levelToLoad + 1));
        }


        public bool ResetIsJumping(int levelToLoad)
        {
            return Convert.ToBoolean(ExcelApp.GetCell(2, 7, levelToLoad + 1));
        }


        public float ResetVelocity(int levelToLoad)
        {
            return (float)Convert.ToDouble(ExcelApp.GetCell(2, 8, levelToLoad + 1));
        }


        public float ResetAirTime(int levelToLoad)
        {
            return (float)Convert.ToDouble(ExcelApp.GetCell(2, 9, levelToLoad + 1));
        }

        public void SaveCamera(CaméraSubjective caméraJeu)
        {
            ExcelApp.SetCell<int>(5, 4, 1, (int)caméraJeu.Position.X);
            ExcelApp.SetCell<int>(6, 4, 1, (int)caméraJeu.Position.Y);
            ExcelApp.SetCell<int>(7, 4, 1, (int)caméraJeu.Position.Z);

            ExcelApp.SetCell<float>(5, 5, 1, caméraJeu.Direction.X);
            ExcelApp.SetCell<float>(6, 5, 1, caméraJeu.Direction.Y);
            ExcelApp.SetCell<float>(7, 5, 1, caméraJeu.Direction.Z);

            ExcelApp.SetCell<string>(3, 6, 1, caméraJeu.IsOnFloor.ToString());
            ExcelApp.SetCell<string>(3, 7, 1, caméraJeu.IsJumping.ToString());
            ExcelApp.SetCell<float>(3, 8, 1, caméraJeu.Velocity);
            ExcelApp.SetCell<float>(3, 9, 1, caméraJeu.AirTime);

            ExcelApp.Save();
        }

    }
}