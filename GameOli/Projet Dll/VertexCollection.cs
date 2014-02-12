using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TOOLS
{
    class VertexCollection
    {
        public VertexPositionNormalTexture[] Vertices { get; set; }
        Vector3 Postion { get; set; }
        int MapSize { get; set; } // Grandeur Max de la map
        int HalfSize { get; set; }
        int VertexCount { get; set; }
        int Scale { get; set; }

        public VertexPositionNormalTexture this[int index]
        {
            get { return Vertices[index]; }
            set { Vertices[index] = value; }
        }

        public VertexCollection(Vector3 position, Texture2D heightMap, int scale)
        {
            Scale = scale;
            MapSize = heightMap.Width - 1;
            HalfSize = MapSize / 2;
            VertexCount = heightMap.Width * heightMap.Width;

            Vertices = new VertexPositionNormalTexture[VertexCount];

            BuildVertices(heightMap); // Construction de la liste de Vertex
            CalculateNormals();
        }
        private void BuildVertices(Texture2D heightMap)
        {
            const float HEIGHT_RANGE_REDUCTION = 5.0f; // Réduire la différence de hauteur dans la heightMap

            Color[] heightMapColors = new Color[VertexCount];
            heightMap.GetData(heightMapColors);

            float x = Postion.X;
            float z = Postion.Z;
            float y = Postion.Y;
            float maxX = x + MapSize;

            VertexPositionNormalTexture vertex;

            for (int i = 0; i < VertexCount; i++)
            {
                if (x > maxX)
                {
                    x = Postion.X;
                    z++;
                }

                y = Postion.Y + (heightMapColors[i].R / HEIGHT_RANGE_REDUCTION);
                vertex = new VertexPositionNormalTexture(new Vector3(x * Scale, y * Scale, z * Scale), Vector3.Zero, Vector2.Zero);
                vertex.TextureCoordinate = new Vector2((vertex.Position.X - Postion.X) / MapSize, (vertex.Position.Z - Postion.Z) / MapSize);
                Vertices[i] = vertex;
                x++;
            }
 
        }
        private void CalculateNormals()
        {
            if (VertexCount < 9)
                return;

            int i = MapSize + 2,
                j = 0,
                k = i + MapSize;

            for (int n = 0; i <= (VertexCount - MapSize) - 2; i += 2, n++)
            {
                if (n == MapSize)
                {
                    n = 0;
                    i += MapSize + 2;
                    j += MapSize + 2;
                    k += MapSize + 2;
                }

                SetNormals(i, j, j + 1);
                SetNormals(i, j + 1, j + 2);
                SetNormals(i, j + 2, i + 1);
                SetNormals(i, i + 1, k + 2);
                SetNormals(i, k + 2, k + 1);
                SetNormals(i, k + 1, k);
                SetNormals(i, k, i - 1);
                SetNormals(i, i - 1, j);
            }
        }

        private void SetNormals(int idx1, int idx2, int idx3)
        {
            Vector3 normal;

            if (idx3 >= Vertices.Length)
                idx3 = Vertices.Length - 1;

            normal = Vector3.Cross(Vertices[idx2].Position - Vertices[idx1].Position, Vertices[idx1].Position - Vertices[idx3].Position);
            normal.Normalize();
            Vertices[idx1].Normal += normal;
            Vertices[idx2].Normal += normal;
            Vertices[idx3].Normal += normal;
        }
    }
}
