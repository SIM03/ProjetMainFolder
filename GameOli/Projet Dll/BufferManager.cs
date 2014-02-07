using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Projet_Dll
{
    class BufferManager
    {
        int Active = 0;
        VertexBuffer VertexBuffer;
        IndexBuffer[] IndexBuffers;
        GraphicsDevice Device;

        BufferManager(VertexPositionNormalTexture[] vertices, GraphicsDevice device)
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

    }
}
