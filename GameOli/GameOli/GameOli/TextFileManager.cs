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
using System.IO;

namespace TOOLS
{
    public static class TextFileManager
    {
        public static void LoadPositions(string fileToLoad, out Vector3 cam�raPosition, out Vector3 cam�raTarget)
        {
            StreamReader txtReader = new StreamReader("../../../../GameOliContent/Database/" + fileToLoad + ".txt");
            cam�raPosition = LoadCameraPosition(txtReader);
            cam�raTarget = Vector3.Normalize(LoadCameraTarget(txtReader));
            txtReader.Close();
        }

        private static Vector3 LoadCameraPosition(StreamReader txtReader)
        {
            Return(txtReader, 1);
            float positionX = float.Parse(txtReader.ReadLine());
            float positionY = float.Parse(txtReader.ReadLine());
            float positionZ = float.Parse(txtReader.ReadLine());
            
            Vector3 cameraPosition = new Vector3(positionX, positionY, positionZ);

            return cameraPosition;
        }

        private static Vector3 LoadCameraTarget(StreamReader txtReader)
        {
            Return(txtReader, 2);
            float targetX = float.Parse(txtReader.ReadLine());
            float targetY = float.Parse(txtReader.ReadLine());
            float targetZ = float.Parse(txtReader.ReadLine());

            Vector3 cameraTarget = new Vector3(targetX, targetY, targetZ);

            return cameraTarget;
        }

        public static void SavePositions(string fileToSave, Cam�raSubjective cam�raJeu)
        {
            StreamWriter TxtWriter = new StreamWriter("../../../../GameOliContent/Database/" + fileToSave + ".txt");

            TxtWriter.WriteLine("CameraPosition");
            TxtWriter.WriteLine(cam�raJeu.Position.X);
            TxtWriter.WriteLine(cam�raJeu.Position.Y);
            TxtWriter.WriteLine(cam�raJeu.Position.Z);
            TxtWriter.WriteLine();
            TxtWriter.WriteLine("CameraTarget");
            TxtWriter.WriteLine(cam�raJeu.Direction.X);
            TxtWriter.WriteLine(cam�raJeu.Direction.Y);
            TxtWriter.WriteLine(cam�raJeu.Direction.Z);

            TxtWriter.Close();
        }

        private static void Return(StreamReader txtReader, int nbLines)
        {
            for (int i = 0; i < nbLines; i++)
            {
                txtReader.ReadLine();
            }
        }
    }
}