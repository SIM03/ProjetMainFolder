using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TOOLS
{
    public class Bo�teDeCollision : PrimitiveDeBaseAnim�e
    {
        const int NB_SOMMETS = 10;
        const int NB_TRIANGLES = 8;
        Color Couleur { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        Vector3 Min { get; set; }
        Vector3 Max { get; set; }
        BoundingBox Bo�te { get; set; }
        ObjetDeBasePhysique ObjetPhysique { get; set; }
        RasterizerState GestionWireframe { get; set; }
        BasicEffect EffetDeBase { get; set; }


         Bo�teDeCollision(Game jeu, ObjetDeBasePhysique objetPhysique, BoundingBox bo�te, Color couleur, float intervalleMAJ)
            : base(jeu, 1f, Vector3.Zero, objetPhysique.Position, intervalleMAJ)
        {
            Bo�te = bo�te;
            Min = bo�te.Min;
            Max = bo�te.Max;
            Couleur = couleur;
            ObjetPhysique = objetPhysique;
            Visible = false; // on ne l'affiche pas initialement
        }

        public override void Initialize()
        {
            Monde = ObjetPhysique.GetMonde();
            Sommets = new VertexPositionColor[NB_SOMMETS];
            GestionWireframe = new RasterizerState();
            GestionWireframe.CullMode = CullMode.None;
            GestionWireframe.FillMode = FillMode.WireFrame;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.VertexColorEnabled = true;
            base.LoadContent();
        }

        protected override void InitialiserSommets()
        {
            Vector3[] listeDesCoins = Bo�te.GetCorners();
            Sommets[0] = new VertexPositionColor(listeDesCoins[3], Couleur); //A
            Sommets[1] = new VertexPositionColor(listeDesCoins[0], Couleur);//B
            Sommets[2] = new VertexPositionColor(listeDesCoins[7], Couleur);//C
            Sommets[3] = new VertexPositionColor(listeDesCoins[4], Couleur);//D
            Sommets[4] = new VertexPositionColor(listeDesCoins[6], Couleur);//E
            Sommets[5] = new VertexPositionColor(listeDesCoins[5], Couleur);//F
            Sommets[6] = new VertexPositionColor(listeDesCoins[2], Couleur);//G
            Sommets[7] = new VertexPositionColor(listeDesCoins[1], Couleur);//H
            Sommets[8] = new VertexPositionColor(listeDesCoins[3], Couleur); //A
            Sommets[9] = new VertexPositionColor(listeDesCoins[0], Couleur);//B
        }

        protected override void EffectuerMise�Jour()
        {
            Monde = ObjetPhysique.GetMonde();
        }

        protected override void G�rerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.B) &&
               GestionInput.EstEnfonc�e(Keys.LeftShift) || GestionInput.EstEnfonc�e(Keys.RightShift))
            {
                this.Visible = !this.Visible;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState oldRasterizerState = GraphicsDevice.RasterizerState;
            GraphicsDevice.RasterizerState = GestionWireframe;
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
            }

            GraphicsDevice.RasterizerState = oldRasterizerState;
            base.Draw(gameTime);
        }
    }
}
