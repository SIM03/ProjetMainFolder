﻿using TOOLS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class TerrainSd : PrimitiveDeBaseAnimée
{
    private const int NB_SOMMETS_PAR_TRIANGLE = 3;
    private const int NB_POINTS_PAR_TUILE = 4;

    public string HeightmapName { get; set; }
    public string TextureMapName { get; set; }
    public int NbNiveauxTexture { get; set; }
    public int NbColonnes { get; set; }
    public int NbRangées { get; set; }
    public Vector2 Delta { get; set; }
    public Vector2 Tuile { get; set; }
    public Vector3 Étendue { get; set; }
    public Vector3 Origine { get; set; }
    public Vector3[,] PtsSommets { get; set; }
    public Color[] DataTexture { get; set; }
    public Texture2D CarteHauteur { get; set; }
    public Texture2D TextureTerrain { get; set; }
    public VertexPositionTexture[] Sommets { get; set; }
    public BasicEffect EffetDeBase { get; set; }
    public RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
    public float DeltaTexture { get; set; }

    int IndexCamera { get; set; }
    CaméraSubjective Camera { get; set; }
    Matrix ViewMatrix { get; set; }
    Matrix ProjectionMatrix { get; set; }
    BoundingFrustum Frustrum { get; set; }
    Vector3 CameraPosition { get; set; }
    List<Tile> Tiles { get; set; } 
    bool IsCull { get; set; }

    GraphicsDevice Graphic { get; set; }
    RasterizerState RS_Default { get; set; }
    RasterizerState RS_WireFrame { get; set; }
    bool IsWire { get; set; }

    InputManager KeyboardManager { get; set; }

    public TerrainSd(Game jeu, GraphicsDevice graphic, int indexCamera, float homotéthieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 size, string nomCarteTerrain, string nomTextureTerrain, int nbNiveauxTexture, float intervalleMAJ)
        : base(jeu, homotéthieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
    {
        Graphic = graphic;
        IndexCamera = indexCamera;
        Étendue = size;
        HeightmapName = nomCarteTerrain;
        TextureMapName = nomTextureTerrain;
        NbNiveauxTexture = nbNiveauxTexture;
    }

    public override void Initialize()
    {
        GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
        KeyboardManager = Game.Services.GetService(typeof(InputManager)) as InputManager;
        InitialiserTableaux();
        GérerTexture();
        Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2);
        CréerTableaux();
        InitialiserPtsSommets();
        Tiles = new List<Tile>();

        Camera = (CaméraSubjective)Game.Components[IndexCamera];
        Frustrum = new BoundingFrustum(Camera.Vue * Camera.Projection);
        IsCull = false;

        // Initialization of both RS for default and wireframe draw mode
        RS_Default = new RasterizerState();
        RS_Default.CullMode = CullMode.CullCounterClockwiseFace;
        RS_Default.FillMode = FillMode.Solid;

        RS_WireFrame = new RasterizerState();
        RS_WireFrame.CullMode = CullMode.CullCounterClockwiseFace;
        RS_WireFrame.FillMode = FillMode.WireFrame;

        base.Initialize();
    }

    private void InitialiserTableaux()
    {
        CarteHauteur = GestionnaireDeTextures.Find(HeightmapName);
        NbColonnes = CarteHauteur.Width;
        NbRangées = CarteHauteur.Height;
        DataTexture = new Color[NbColonnes * NbRangées];
        CarteHauteur.GetData<Color>(DataTexture);
        Delta = new Vector2(Étendue.X / NbColonnes, Étendue.Z / NbRangées);
        NbTriangles = (NbColonnes - 1) * (NbRangées - 1) * 2;
        NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;
    }

    private void GérerTexture()
    {
        TextureTerrain = GestionnaireDeTextures.Find(TextureMapName);
        Tuile = new Vector2(TextureTerrain.Width, TextureTerrain.Height / Étendue.Y / NbNiveauxTexture);
        DeltaTexture = 1f / NbNiveauxTexture;
    }

    private void CréerTableaux()
    {
        PtsSommets = new Vector3[NbColonnes + 1, NbRangées + 1];
        Sommets = new VertexPositionTexture[NbSommets];
    }

    protected override void LoadContent()
    {
        EffetDeBase = new BasicEffect(GraphicsDevice);
        GérerEffet();
        base.LoadContent();
    }

    private void GérerEffet()
    {
        EffetDeBase.TextureEnabled = true;
        EffetDeBase.Texture = TextureTerrain;
    }

    private void InitialiserPtsSommets()
    {
        for (int noColonne = 0; noColonne < NbColonnes; ++noColonne)
        {
            for (int noRangée = 0; noRangée < NbRangées; ++noRangée)
            {
                PtsSommets[noColonne, noRangée] = new Vector3(Origine.X + noColonne * Delta.X, CalculerHauteur(noColonne, NbRangées - 1 - noRangée), Origine.Z - noRangée * Delta.Y);
            }
        }
    }

    private float CalculerHauteur(int noColonne, int noRangée)
    {
        return (float)DataTexture[noColonne + NbRangées * noRangée].R / (float)byte.MaxValue * Étendue.Y;
    }

    protected override void InitialiserSommets()
    {
        float deltaAltitude = (float)((int)Math.Ceiling(Étendue.Y / NbNiveauxTexture) + 1);
        int noSommet = 0;
        for (int noRangée = 0; noRangée < NbRangées - 1; ++noRangée)
        {
            for (int noColonne = 0; noColonne < NbColonnes - 1; ++noColonne)
            {
                AddTile(noSommet,noColonne,noRangée);
                noSommet = noSommet + 6;
            }
        }
    }

    private void AddTile(int noSommet,int noColonne,int noRangée)
    {
        float y = (float)(int)((PtsSommets[noColonne, noRangée].Y + PtsSommets[noColonne + 1, noRangée].Y + PtsSommets[noColonne, noRangée + 1].Y + PtsSommets[noColonne + 1, noRangée + 1].Y) / NB_POINTS_PAR_TUILE / deltaAltitude) * DeltaTexture;
        Sommets[noSommet] = new VertexPositionTexture(PtsSommets[noColonne, noRangée], new Vector2(0, y + DeltaTexture));
        Sommets[noSommet + 1] = new VertexPositionTexture(PtsSommets[noColonne, noRangée + 1], new Vector2(0, y));
        Sommets[noSommet + 2] = new VertexPositionTexture(PtsSommets[noColonne + 1, noRangée], new Vector2(1, y + DeltaTexture));
        Sommets[noSommet + 3] = new VertexPositionTexture(PtsSommets[noColonne, noRangée + 1], new Vector2(0, y));
        Sommets[noSommet + 4] = new VertexPositionTexture(PtsSommets[noColonne + 1, noRangée + 1], new Vector2(1, y));
        Sommets[noSommet + 5] = new VertexPositionTexture(PtsSommets[noColonne + 1, noRangée], new Vector2(1, y + DeltaTexture));
        Tiles.Add(new Tile(new BoundingBox(Sommets[noSommet].Position, Sommets[noSommet + 6].Position), noSommet));
    }

    public override void Update(GameTime gameTime)
    {
        GetCamera();
        KeyboardHandler();
        ViewMatrix = Camera.Vue;
        ProjectionMatrix = Camera.Projection;
        Frustrum.Matrix = ViewMatrix * ProjectionMatrix;
        CameraPosition = Camera.Position;
        Game.Window.Title = string.Format(" Triangles Rendered: {0} -||- Culling Enable {1}", Sommets.Length / 3, IsCull);
        base.Update(gameTime);
    }

    private void GetCamera()
    {
        Camera = (CaméraSubjective)Game.Components[IndexCamera];
    }

    private void KeyboardHandler()
    {
        if (KeyboardManager.EstNouvelleTouche(Keys.W) && KeyboardManager.EstEnfoncée(Keys.LeftShift))
            IsWire = !IsWire;

        if (KeyboardManager.EstNouvelleTouche(Keys.C) && KeyboardManager.EstEnfoncée(Keys.LeftShift))
            IsCull = !IsCull;
    }

    public override void Draw(GameTime gameTime)
    {
        RasterizerState OldRS = Graphic.RasterizerState;

        Graphic.RasterizerState = RS_Default;
        if (IsWire)
            Graphic.RasterizerState = RS_WireFrame;

        EffetDeBase.World = GetMonde();
        EffetDeBase.View = CaméraJeu.Vue;
        EffetDeBase.Projection = CaméraJeu.Projection;
        foreach (EffectPass effectPass in EffetDeBase.CurrentTechnique.Passes)
        {
            effectPass.Apply();
            foreach (Tile tile in Tiles)
            {
                if (tile.Box.Intersects(Frustrum))
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, tile.NodeIndex, tile.NodeIndex + 6);
                }
                else if (tile.Box.Contains(CameraPosition) == ContainmentType.Contains)
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, tile.NodeIndex, tile.NodeIndex + 6);
                }
            }
        }

        Graphic.RasterizerState = OldRS;
        base.Draw(gameTime);
    }
}

struct Tile
{
    public BoundingBox Box { get; private set; }
    public int NodeIndex { get; private set; }

    public Tile(BoundingBox box, int nodeIndex)
    {
        Box = box;
        NodeIndex = nodeIndex;
    }
}