using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Projet_Dll
{
    class QuadTree
    {
        QuadNode rootNode { get; set; }
        VertexCollection Vertices { get; set; }
        BufferManager Buffer { get; set; }
        Vector3 Position { get; set; }
        int TopNodeSize { get; set; }

        Vector3 CameraPosition { get; set; }
        Vector3 LastCameraPosition { get; set; }

        int[] Indices { get; set; }

        Matrix View { get; set; }
        Matrix Projection { get; set; }

        GraphicsDevice GraphicDevice { get; set; }

        int TopNodeSize { get; set; }
        QuadNode RootNode { get; set; }
        VertexCollection Vertices { get; set; }

        BoundingFrustum ViewFrustrum { get; set; }


    }

    enum NodeType
    {
        FullNode,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    class QuadNode
    {
        QuadNode Parent { get; set; }
        QuadTree ParentTree { get; set; }
        int PositionIndex { get; set; }

        int NodeDepth { get; set; }
        int NodeSize {get; set;}

        bool HasChildren { get; set; }

        #region VERTICES
        QuadNodeVertex VertexTopLeft;
        QuadNodeVertex VertexTop;
        QuadNodeVertex VertexTopRight;
        QuadNodeVertex VertexLeft;
        QuadNodeVertex VertexCenter;
        QuadNodeVertex VertexRight;
        QuadNodeVertex VertexBottomLeft;
        QuadNodeVertex VertexBottom;
        QuadNodeVertex VertexBottomRight;
        #endregion

        #region CHILDREN
        QuadNode ChildTopLeft;
        QuadNode ChildTopRight;
        QuadNode ChildBottomLeft;
        QuadNode ChildBottomRight;
        #endregion

        #region NEIGHBORS
        QuadNode NeighborTop;
        QuadNode NeighborRight;
        QuadNode NeighborBottom;
        QuadNode NeighborLeft;
        #endregion

        BoundingBox Bounds { get; set; }

        NodeType Nodetype { get; set; }

        public QuadNode(NodeType nodeType, int nodeSize, int nodeDepth, QuadNode parent, QuadTree parentTree, int positionIndex)
        {
            Nodetype = nodeType;
            NodeSize = nodeSize;
            NodeDepth = nodeDepth;
            PositionIndex = positionIndex;

            Parent = parent;
            ParentTree = parentTree;

            AddVertices();

            Bounds = new BoundingBox(ParentTree.Vertices[VertexTopLeft.Index],
                ParentTree.Vertices[VertexBottomRight.Index].Position);
            Bounds.Min.Y = -950f;
            Bounds.Max.Y = 950f;

            if (NodeSize >= 4)
                AddChildren();

            if (NodeDepth == 1)
            {
                AddNeighbors();

                VertexTopLeft.Activated = true;
                VertexTopRight.Activated = true;
                VertexCenter.Activated = true;
                VertexBottomLeft.Activated = true;
                VertexBottomRight.Activated = true;
            }
        }

        private void AddVertices()
        {

        }
    }

    class QuadNodeVertex
    {
        int Index { get; set; }
        bool Activated { get; set; }
    }
}
