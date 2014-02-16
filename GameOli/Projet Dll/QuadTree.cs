using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TOOLS
{
    public class QuadTree
    {
        QuadNode rootNode { get; set; }
        BufferManager Buffer { get; set; }
        Vector3 Position { get; set; }

        public Vector3 CameraPosition { get; set; }
        Vector3 LastCameraPosition { get; set; }

        int[] Indices { get; set; }

        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        GraphicsDevice Graphic { get; set; }

        internal int TopNodeSize { get; set; }
        QuadNode RootNode { get; set; }
        QuadNode ActiveNode { get; set; }
        QuadNode LastActiveNode { get; set; }
        internal VertexCollection Vertices { get; set; }
        public int MinimumDepth { get; set; }

        public BoundingFrustum ViewFrustrum { get; private set; }

        // Drawing Parameters
        public BasicEffect Effect { get; private set; }
        public int IndexCount { get; private set; }
        public bool Cull { get; set; }

        public QuadTree(Game game,Vector3 position, Texture2D heightmap, Matrix viewMatrix, Matrix projectionMatrix, GraphicsDevice graphic, int scale)
        {
            Graphic = graphic;
            Position = position;
            TopNodeSize = heightmap.Width - 1;

            Vertices = new VertexCollection(Position, heightmap, scale);
            Buffer = new BufferManager(Vertices.Vertices, Graphic);
            RootNode = new QuadNode(NodeType.FullNode, TopNodeSize, 1, null, this, 0);
            View = viewMatrix;
            Projection = projectionMatrix;

            ViewFrustrum = new BoundingFrustum(viewMatrix * projectionMatrix);

            Indices = new int[((heightmap.Width + 1) * (heightmap.Height + 1))];
            MinimumDepth = 0; //// Tutorial said so... ////

            // Drawing Parameters Initialisation
            Effect = new BasicEffect(graphic);
            Effect.EnableDefaultLighting();
            Effect.FogEnabled = true;
            Effect.FogStart = 300f;
            Effect.FogEnd = 1000f;
            Effect.FogColor = Color.Black.ToVector3();
            Effect.TextureEnabled = true;
            Effect.Texture = new Texture2D(graphic, 100, 100);
            Effect.Projection = projectionMatrix;
            Effect.View = viewMatrix;
            Effect.World = Matrix.Identity;
        }

        internal void UpdateBuffer(int vIndex)
        {
            Indices[IndexCount] = vIndex;
            IndexCount++;
        }

        public void Update(GameTime gameTime)
        {
            if (CameraPosition == LastCameraPosition && /* Buggy Update Hot fix Possibly*/ (gameTime.TotalGameTime.TotalSeconds >= 60.0))
                return;

            ViewFrustrum.Matrix = View * Projection;
            Effect.View = View;
            Effect.Projection = Projection;

            LastCameraPosition = CameraPosition;
            IndexCount = 0;

            RootNode.ForceMinimumDepth();

            LastActiveNode = ActiveNode;
            ActiveNode = RootNode.FindCameraNode(CameraPosition);

            if (ActiveNode != LastActiveNode)
                RootNode.Merge();

            if (ActiveNode != null)
                ActiveNode.Split();

            RootNode.SetActiveVertices();

            Buffer.UpdateIndexBuffer(Indices, IndexCount);
            Buffer.SwapBuffer();
        }

        public void Draw(GameTime gameTime)
        {
            Graphic.SetVertexBuffer(Buffer.VertexBuffer);
            Graphic.Indices = Buffer.IndexBuffer;

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Graphic.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Vertices.Vertices.Length, 0, IndexCount / 3);
            }
        }
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
        int NodeSize { get; set; }

        bool HasChildren { get; set; }
        bool IsActive { get; set; }
        bool IsSplit { get; set; }
        bool CanSplit { get { return (NodeSize >= 2); } }
        public bool IsInView
        {
            get { return Contains(ParentTree.ViewFrustrum); }
        }

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

        /// <summary>
        /// Constructor for the QuadNode struct
        /// </summary>
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
            /**/
            Bounds.Min.Y = -950f; //// WATCH OUT!!! HEIGHT OF THESE BOUNDING BOX MAY NEED TO BE REDEFINED ALONG THE WAY AS OUR HEIGTMAP MAY REACH HIGHER THAN 950F UNIT OR LOWER THAN -950 UNIT ////
            /**/
            Bounds.Max.Y = 950f; //// WATCH OUT!!! HEIGHT OF THESE BOUNDING BOX MAY NEED TO BE REDEFINED ALONG THE WAY AS OUR HEIGTMAP MAY REACH HIGHER THAN 950F UNIT OR LOWER THAN -950 UNIT ////

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

        // Draw tomorrow for precisions
        private void AddNeighbors()
        {
            switch (Nodetype)
            {
                case NodeType.FullNode:
                    break;
                case NodeType.TopLeft:
                    if (Parent.NeighborTop != null)
                        NeighborTop = Parent.ChildBottomLeft;

                    NeighborRight = Parent.ChildTopRight;

                    NeighborBottom = Parent.ChildBottomLeft;

                    if (Parent.NeighborLeft != null)
                        NeighborLeft = Parent.NeighborLeft.ChildTopRight;

                    break;
                case NodeType.TopRight:
                    if (Parent.NeighborTop != null)
                        NeighborTop = Parent.NeighborTop.ChildBottomRight;

                    if (Parent.NeighborRight != null)
                        NeighborRight = Parent.NeighborRight.ChildTopLeft;

                    NeighborBottom = Parent.ChildBottomRight;

                    NeighborLeft = Parent.ChildTopLeft;

                    break;
                case NodeType.BottomLeft:

                    NeighborTop = Parent.ChildTopLeft;

                    NeighborRight = Parent.ChildBottomRight;

                    if (Parent.NeighborBottom != null)
                        NeighborBottom = Parent.NeighborBottom.ChildTopLeft;

                    if (Parent.NeighborLeft != null)
                        NeighborLeft = Parent.NeighborLeft.ChildBottomRight;

                    break;
                case NodeType.BottomRight:

                    NeighborTop = Parent.ChildTopRight;

                    if (Parent.NeighborRight != null)
                        NeighborRight = Parent.NeighborRight.ChildBottomLeft;

                    if (Parent.NeighborBottom != null)
                        NeighborBottom = Parent.NeighborBottom.ChildTopRight;

                    NeighborLeft = Parent.ChildBottomLeft;
                    break;
                default:
                    break;
            }

            if (this.HasChildren)
            {
                ChildTopLeft.AddNeighbors();
                ChildTopRight.AddNeighbors();
                ChildBottomLeft.AddNeighbors();
                ChildBottomRight.AddNeighbors();
            }
        }

        /// <summary>
        /// Sets Vertices using Node Type to identify which parent's vertices to reuse
        /// </summary>
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

        /// <summary>
        /// Adds Children to current Node
        /// </summary>
        private void AddChildren()
        {
            // Adds All Four Quadrants Child from top left to bot right
            ChildTopLeft = new QuadNode(NodeType.TopLeft, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexTopLeft.Index);

            ChildTopRight = new QuadNode(NodeType.TopRight, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexTop.Index);

            ChildBottomLeft = new QuadNode(NodeType.BottomLeft, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexLeft.Index);

            ChildBottomRight = new QuadNode(NodeType.BottomRight, NodeSize / 2, NodeDepth + 1, this, ParentTree, VertexCenter.Index);

            HasChildren = true;

        }

        /// <summary>
        /// Activates main Vertices for the Node in the Quad Tree through UpdateBuffer method
        /// </summary>
        internal void SetActiveVertices()
        {
            if (ParentTree.Cull && !IsInView)
                return;

            if (IsSplit && this.HasChildren)
            {
                ChildTopLeft.SetActiveVertices();
                ChildTopRight.SetActiveVertices();
                ChildBottomLeft.SetActiveVertices();
                ChildBottomRight.SetActiveVertices();
                return;
            }

            // Top Triangles
            ParentTree.UpdateBuffer(VertexCenter.Index);
            ParentTree.UpdateBuffer(VertexTopLeft.Index);

            if (VertexTop.Activated)
            {
                ParentTree.UpdateBuffer(VertexTop.Index);

                ParentTree.UpdateBuffer(VertexCenter.Index);
                ParentTree.UpdateBuffer(VertexTop.Index);
            }
            ParentTree.UpdateBuffer(VertexTopRight.Index);

            // Right Triangles
            ParentTree.UpdateBuffer(VertexCenter.Index);
            ParentTree.UpdateBuffer(VertexTopRight.Index);

            if (VertexRight.Activated)
            {
                ParentTree.UpdateBuffer(VertexRight.Index);

                ParentTree.UpdateBuffer(VertexCenter.Index);
                ParentTree.UpdateBuffer(VertexBottom.Index);
            }
            ParentTree.UpdateBuffer(VertexBottomRight.Index);

            // Bottom Triangles
            ParentTree.UpdateBuffer(VertexCenter.Index);
            ParentTree.UpdateBuffer(VertexBottomRight.Index);

            if (VertexBottom.Activated)
            {
                ParentTree.UpdateBuffer(VertexBottom.Index);

                ParentTree.UpdateBuffer(VertexCenter.Index);
                ParentTree.UpdateBuffer(VertexBottom.Index);
            }
            ParentTree.UpdateBuffer(VertexBottomLeft.Index);

            // Left Triangles
            ParentTree.UpdateBuffer(VertexCenter.Index);
            ParentTree.UpdateBuffer(VertexBottomLeft.Index);

            if (VertexLeft.Activated)
            {
                ParentTree.UpdateBuffer(VertexLeft.Index);

                ParentTree.UpdateBuffer(VertexCenter.Index);
                ParentTree.UpdateBuffer(VertexLeft.Index);
            }
            ParentTree.UpdateBuffer(VertexTopLeft.Index);
        }

        /// <summary>
        /// Activates Main 5 Vertices for a Node
        /// </summary>
        internal void Activate()
        {
            VertexTopLeft.Activated = true;
            VertexTopRight.Activated = true;
            VertexCenter.Activated = true;
            VertexBottomLeft.Activated = true;
            VertexBottomRight.Activated = true;
        }

        /// <summary>
        /// Force Node to activate until QuadTree's minimum depth as been reached (Depth 6 = 5 Children to Top Node)
        /// </summary>
        public void ForceMinimumDepth()
        {
            if (NodeDepth < ParentTree.MinimumDepth)
	        {
                if (this.HasChildren)
	            {
                    IsActive = false;
                    IsSplit = true;

                    ChildTopLeft.ForceMinimumDepth();
                    ChildTopRight.ForceMinimumDepth();
                    ChildBottomLeft.ForceMinimumDepth();
                    ChildBottomRight.ForceMinimumDepth();
	            }
                else
	            {
                    this.Activate();
                    IsSplit = false;
	            }

                return;
            }

            if (NodeDepth == ParentTree.MinimumDepth || (NodeDepth < ParentTree.MinimumDepth && !this.HasChildren))
	        {
		        this.Activate();
                IsSplit = false;
	        }
        }

        /// <summary>
        /// Returns wether the 3d position is in this Node's bounding box
        /// </summary>
        public bool Contains(Vector3 position)
        {
            return Bounds.Contains(position) == ContainmentType.Contains;
        }

        public bool Contains(BoundingFrustum boundingFrustrum)
        {
            return Bounds.Intersects(boundingFrustrum);
        }

        /// <summary>
        ///  Finds which Node contains our camera
        /// </summary>
        public QuadNode FindCameraNode(Vector3 position)
        {
            // Not Found
            if (!Contains(position))
                return null;

            // Check Children
            if (HasChildren)
            {
                if (ChildTopLeft.Contains(position))
                    return ChildTopLeft.FindCameraNode(position);
                if (ChildTopRight.Contains(position))
                    return ChildTopRight.FindCameraNode(position);
                if (ChildBottomLeft.Contains(position))
                    return ChildBottomLeft.FindCameraNode(position);
                // Must be last child Since it is contain in parent Node
                return ChildBottomRight.FindCameraNode(position);
            }

            // No child && Contains = this Node
            return this;
        }
        
        /// <summary>
        /// Performs various check for splitting (neighbors and parent and size) and splits Node if able
        /// </summary>
        public void Split()
        {
            if (ParentTree.Cull && !IsInView)
                return;

            if ( (Parent != null) && !Parent.IsSplit)
                Parent.Split();
            if (CanSplit)
            {

                if (HasChildren)
                {
                    ChildTopLeft.Activate();
                    ChildTopRight.Activate();
                    ChildBottomLeft.Activate();
                    ChildBottomRight.Activate();

                    IsActive = false;

                }
                else
                {
                    IsActive = true;
                }

                IsSplit = true;

                // Set Extra Active Vertex
                VertexTop.Activated = true;
                VertexLeft.Activated = true;
                VertexRight.Activated = true;
                VertexBottom.Activated = true;
            }

            // Unsure I understand yet ?
            CheckNeighborParentSplit(NeighborTop);
            CheckNeighborParentSplit(NeighborLeft);
            CheckNeighborParentSplit(NeighborRight);
            CheckNeighborParentSplit(NeighborBottom);

            // Activated Neighbor's vertices to match this Splitted Node vertices
            if (NeighborTop != null)
                NeighborTop.VertexBottom.Activated = true;

            if (NeighborLeft != null)
                NeighborLeft.VertexRight.Activated = true;

            if (NeighborRight != null)
                NeighborRight.VertexLeft.Activated = true;

            if (NeighborBottom != null)
                NeighborBottom.VertexTop.Activated = true;

        }

        private static void CheckNeighborParentSplit(QuadNode neighbor)
        {
            if (neighbor != null && neighbor.Parent != null)
            {
                if (!neighbor.Parent.IsSplit)
                    neighbor.Parent.Split();
            }
        }

        public void Merge()
        {
            VertexTop.Activated = false;
            VertexLeft.Activated = false;
            VertexRight.Activated = false;
            VertexBottom.Activated = false;

            if (Nodetype != NodeType.FullNode)
            {
                VertexTopLeft.Activated = false;
                VertexTopRight.Activated = false;
                VertexBottomLeft.Activated = false;
                VertexBottomRight.Activated = false;
            }
            IsActive = true;
            IsSplit = false;

            if (HasChildren)
            {

                if (ChildTopLeft.IsSplit)
                {
                    ChildTopLeft.Merge();
                    ChildTopLeft.IsActive = false;
                }
                else
                {
                    ChildTopLeft.VertexTop.Activated = false;
                    ChildTopLeft.VertexLeft.Activated = false;
                    ChildTopLeft.VertexRight.Activated = false;
                    ChildTopLeft.VertexBottom.Activated = false;
                }
                if (ChildTopRight.IsSplit)
                {
                    ChildTopRight.Merge();
                    ChildTopRight.IsActive = false;
                }
                else
                {
                    ChildTopRight.VertexTop.Activated = false;
                    ChildTopRight.VertexLeft.Activated = false;
                    ChildTopRight.VertexRight.Activated = false;
                    ChildTopRight.VertexBottom.Activated = false;
                }
                if (ChildBottomLeft.IsSplit)
                {
                    ChildBottomLeft.Merge();
                    ChildBottomLeft.IsActive = false;
                }
                else
                {
                    ChildBottomLeft.VertexTop.Activated = false;
                    ChildBottomLeft.VertexLeft.Activated = false;
                    ChildBottomLeft.VertexRight.Activated = false;
                    ChildBottomLeft.VertexBottom.Activated = false;
                }
                if (ChildBottomRight.IsSplit)
                {
                    ChildBottomRight.Merge();
                    ChildBottomRight.IsActive = false;
                }
                else
                {
                    ChildBottomRight.VertexTop.Activated = false;
                    ChildBottomRight.VertexLeft.Activated = false;
                    ChildBottomRight.VertexRight.Activated = false;
                    ChildBottomRight.VertexBottom.Activated = false;
                }
            }
        }
    }

    class QuadNodeVertex
    {
        public int Index { get; set; }
        public bool Activated { get; set; }
    }
}
