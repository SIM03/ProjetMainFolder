using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Projet_Dll
{
    class QuadTree
    {
        QuadNode rootNode { get; set; }
        public VertexCollection Vertices { get; private set; }
        BufferManager Buffer { get; set; }
        Vector3 Position { get; set; }
        public int TopNodeSize { get; private set; }

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

        BoundingBox Bounds;

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

            Bounds = new BoundingBox(ParentTree.Vertices[VertexTopLeft.Index].Position,
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

        private void AddNeighbors()
        {

        }

      
        private void AddVertices()
        {
            // Copy Vertices From The Parent Using NodeType to affect the child vertices to the existing parent vertices
            switch (Nodetype)
            {
                case NodeType.TopLeft:
                    VertexTopLeft = Parent.VertexTopLeft;
                    VertexTopRight = Parent.VertexTop;
                    VertexBottomLeft = Parent.VertexLeft;
                    VertexBottomRight = Parent.VertexCenter;
                    break;
                case NodeType.TopRight:
                    VertexTopLeft = Parent.VertexTop;
                    VertexTopRight = Parent.VertexTopRight;
                    VertexBottomLeft = Parent.VertexCenter;
                    VertexBottomRight = Parent.VertexRight;
                    break;
                case NodeType.BottomLeft:
                    VertexTopLeft = Parent.VertexLeft;
                    VertexTopRight = Parent.VertexCenter;
                    VertexBottomLeft = Parent.VertexBottomLeft;
                    VertexBottomRight = Parent.VertexBottom;
                    break;
                case NodeType.BottomRight:
                    VertexTopLeft = Parent.VertexCenter;
                    VertexTopRight = Parent.VertexRight;
                    VertexBottomLeft = Parent.VertexBottom;
                    VertexBottomRight = Parent.VertexBottomRight;
                    break;
                default:
                    VertexTopLeft = new QuadNodeVertex { Activated = true, Index = 0 };
                    VertexTopRight = new QuadNodeVertex { Activated = true, Index = VertexTopLeft.Index + NodeSize };
                    VertexBottomLeft = new QuadNodeVertex { Activated = true, Index = (ParentTree.TopNodeSize + 1) * ParentTree.TopNodeSize };
                    VertexBottomRight = new QuadNodeVertex { Activated = true, Index = VertexBottomLeft.Index + NodeSize };
                    break;
            }
            
            // Adding All Secondary Vertex Of Child

            VertexTop = new QuadNodeVertex 
            { 
                Activated = false,
                Index = VertexTopLeft.Index + (NodeSize / 2) 
            };

            VertexLeft = new QuadNodeVertex
            {
                Activated = false,
                Index = VertexTopLeft.Index + (ParentTree.TopNodeSize + 1) * (NodeSize / 2)
            };

            VertexCenter = new QuadNodeVertex
            {
                Activated = false,
                Index = VertexLeft.Index + (NodeSize / 2)
            };
            
            VertexRight = new QuadNodeVertex
            {
                Activated = false,
                Index = VertexLeft.Index + NodeSize
            };

            VertexBottom = new QuadNodeVertex
            {
                Activated = false,
                Index = VertexBottomLeft.Index + (NodeSize / 2)
            };

        }

        private void AddChildren()
        {
            // Adds All Four Quadrants Child from top left to bot right
            ChildTopLeft = new QuadNode(NodeType.TopLeft, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexTopLeft.Index);

            ChildTopRight = new QuadNode(NodeType.TopRight, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexTop.Index);

            ChildBottomLeft = new QuadNode(NodeType.BottomLeft, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexLeft.Index);

            ChildBottomRight = new QuadNode(NodeType.BottomRight, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexCenter.Index);

            HasChildren = true;

        }
    }

    class QuadNodeVertex
    {
        public int Index { get; set; }
        public bool Activated { get; set; }
    }
}
