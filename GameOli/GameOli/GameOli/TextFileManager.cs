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
using System.Xml.Linq;
using TOOLS;

namespace GAME
{
    public static class TextFileManager
    {
        public static void LoadPhysicalObjects(Game game, string levelToLoad)
        {
            Stream stream = TitleContainer.OpenStream("Content/Database/level1.xml");
            XDocument xmlFile = XDocument.Load(stream);

            foreach (XElement physicalObject in xmlFile.Descendants("PhysicalObject"))
            {
                string name = physicalObject.Element("name").Value;
                float scale = ConvertToFloat(physicalObject.Element("scale").Value);
                Vector3 rotation = ConvertToVector3(physicalObject.Element("rotation").Value);
                Vector3 position = ConvertToVector3(physicalObject.Element("position").Value);
                float intervalleMAJ = ConvertToFloat(physicalObject.Element("fps").Value);

                game.StaticObjectList.Add(new PhysicalObject(game, name, scale, rotation, position, intervalleMAJ));
            }
            stream.Close();
        }

        public static void LoadDynamicObjects(Game game, string levelToLoad)
        {
            Stream stream = TitleContainer.OpenStream("Content/Database/level1.xml");
            XDocument xmlFile = XDocument.Load(stream);

            foreach (XElement dynamicObject in xmlFile.Descendants("DynamicObject"))
            {
                string name = dynamicObject.Element("name").Value;
                float scale = ConvertToFloat(dynamicObject.Element("scale").Value);
                float intervalleMAJ = ConvertToFloat(dynamicObject.Element("fps").Value);
                float mass = ConvertToFloat(dynamicObject.Element("mass").Value);
                float rebound = ConvertToFloat(dynamicObject.Element("rebound").Value);
                float friction = ConvertToFloat(dynamicObject.Element("friction").Value);
                Vector3 rotation = ConvertToVector3(dynamicObject.Element("rotation").Value);
                Vector3 position = ConvertToVector3(dynamicObject.Element("position").Value);
                Vector3 direction = ConvertToVector3(dynamicObject.Element("direction").Value);

                if (mass == 0)
                    game.DynamicObjectList.Add(new DynamicPhysicalObject(game, name, scale, rotation, position, intervalleMAJ, game.StaticObjectList, rebound, friction));
                else
                    game.DynamicObjectList.Add(new DynamicPhysicalObject(game, name, scale, rotation, position, intervalleMAJ, game.StaticObjectList, direction, mass, rebound, friction));
            }
            stream.Close();
        }

        public static void LoadTexturedPlans(Game game, string levelToLoad)
        {
            Stream stream = TitleContainer.OpenStream("Content/Database/level1.xml");
            XDocument xmlFile = XDocument.Load(stream);

            foreach (XElement texturedPlan in xmlFile.Descendants("TexturedPlan"))
            {
                float echelleInitiale = ConvertToFloat(texturedPlan.Element("scale").Value);
                Vector3 rotationInitiale = ConvertToVector3(texturedPlan.Element("rotation").Value);
                Vector3 positionInitiale = ConvertToVector3(texturedPlan.Element("position").Value);
                Vector2 étendue = ConvertToVector2(texturedPlan.Element("area").Value);
                Vector2 charpente = ConvertToVector2(texturedPlan.Element("frame").Value);
                string nomTexturePlan = texturedPlan.Element("name").Value;
                float intervalleMAJ = ConvertToFloat(texturedPlan.Element("fps").Value);

                game.StaticObjectList.Add(new PlanTexturé(game, echelleInitiale, rotationInitiale, positionInitiale, étendue, charpente, nomTexturePlan, intervalleMAJ));
            }
            stream.Close();
        }

        public static void LoadCamera(Game game, string levelToLoad)
        {
            Stream stream = TitleContainer.OpenStream("Content/Database/level1.xml");
            XDocument xmlFile = XDocument.Load(stream);

            foreach (XElement camera in xmlFile.Descendants("Camera"))
            {
                Vector3 position = ConvertToVector3(camera.Element("position").Value);
                Vector3 target = ConvertToVector3(camera.Element("target").Value);
                float intervalleMAJ = ConvertToFloat(camera.Element("fps").Value);

                game.CaméraJeu = new CaméraSubjectivePhysique(game, position, target, game.StaticObjectList, game.DynamicObjectList, intervalleMAJ);
            }
            stream.Close();
        }

        private static Vector3 ConvertToVector3(string stringValue)
        {
            Vector3 value = new Vector3(0, 0, 0);

            if (stringValue != "")
            {
                // Ex: 23 56 76
                string[] floatTable = stringValue.Split(' ');
                float x = ConvertToFloat(floatTable[0]);
                float y = ConvertToFloat(floatTable[1]);
                float z = ConvertToFloat(floatTable[2]);
                value = new Vector3(x, y, z);
            }

            return value;
        }

        private static Vector2 ConvertToVector2(string stringValue)
        {
            Vector2 value = new Vector2(0, 0);

            if (stringValue != "")
            {
                // Ex: 23 56
                string[] floatTable = stringValue.Split(' ');
                float x = ConvertToFloat(floatTable[0]);
                float y = ConvertToFloat(floatTable[1]);
                value = new Vector2(x, y);
            }

            return value;
        }

        private static float ConvertToFloat(string stringValue)
        {
            float value = 0;
            bool isAFraction = false;

            if (stringValue != "")
            {
                for (int i = 0; i < stringValue.Length; i++)
                {
                    if (stringValue[i] == '/')
                    {
                        isAFraction = true;
                        break;
                    }
                }

                if (isAFraction)
                {
                    float float1;
                    string[] floatTable = stringValue.Split('/');

                    if (floatTable[0] == "PI")
                        float1 = MathHelper.Pi;
                    else if (floatTable[0] == "-PI")
                        float1 = -MathHelper.Pi;
                    else
                        float1 = float.Parse(floatTable[0]);

                    value = float1 / float.Parse(floatTable[1]);
                }
                else
                {
                    value = float.Parse(stringValue);
                }
            }

            return value;
        }
    }
}