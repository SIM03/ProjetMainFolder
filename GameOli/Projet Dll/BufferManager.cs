using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TOOLS
{
    class BufferManager
    {
        int Active = 0;
        internal VertexBuffer VertexBuffer;
        IndexBuffer[] IndexBuffers;
        GraphicsDevice Device;
        internal IndexBuffer IndexBuffer
        {
            get { return IndexBuffers[Active]; }
        }

        public BufferManager(VertexPositionNormalTexture[] vertices, GraphicsDevice device)
        {
            Device = device;

            VertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices);

            IndexBuffers = new IndexBuffer[]
                {
                    new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, 100000, BufferUsage.WriteOnly),
                    new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, 100000, BufferUsage.WriteOnly)
                };
        }

        internal void UpdateIndexBuffer(int[] indices, int indexCount)
        {
            int inactive = Active == 0 ? 1 : 0;

            IndexBuffers[inactive].SetData(indices, 0, indexCount);

        }

        internal void SwapBuffer()
        {
            Active = Active == 0 ? 1 : 0; ;
        }
    }
}
