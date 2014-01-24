using AtelierXNA;
using Microsoft.Xna.Framework;
using System;

public class ObjetDePatrouille : ObjetDeBase
{
   private const float DISTANCE_ENTRE_DEUX_PTS = 73;
   private const float ANGLE_ROTATION_ROLL = MathHelper.Pi / 4;

   public int NbPtsPatrouille { get; set; }
   public int NoPoint { get; set; }
   public float AngleEntreDeuxPoints { get; set; }
   public float TempsÉcouléDepuisMAJ { get; set; }
   public float Rayon { get; set; }
   public float IntervalleMAJ { get; set; }
   public float Vitesse { get; set; }
   public float DeltaAngle { get; set; }
   public float AngleRotationYaw { get; set; }
   public float AngleDépart { get; set; }
   public Vector3 VecteurRayon { get; set; }
   public Vector3 PositionAxe { get; set; }
   public Vector3 VecteurUnitaire { get; set; }
   public Vector3 PointCentre { get; set; }
   public Vector3 VecteurDirection { get; set; }
   public Vector3[] PtsCercle { get; set; }
   public InputManager GestionInput { get; set; }

   public ObjetDePatrouille(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 positionAxe, int nbPtsPatrouille, float vitesse, float intervalleMAJ)
      : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
   {
      PositionAxe = positionAxe;
      NbPtsPatrouille = nbPtsPatrouille;
      IntervalleMAJ = intervalleMAJ;
      Vitesse = vitesse;
   }

   public override void Initialize()
   {
      PointCentre = new Vector3(PositionAxe.X, Position.Y, PositionAxe.Z);
      Rayon = Vector3.Distance(PointCentre, Position);
      VecteurRayon = Position - PointCentre;
      AngleEntreDeuxPoints = MathHelper.TwoPi / NbPtsPatrouille;
      NoPoint = 0;
      InitialiserPts();
      base.Initialize();
   }

   private void InitialiserPts()
   {
      PtsCercle = new Vector3[NbPtsPatrouille];
      PtsCercle[0] = Position;
      AngleDépart = (float)Math.Atan(Position.Z / Position.X);
      for (int noPt = 1; noPt < NbPtsPatrouille; noPt++)
      {
         PtsCercle[noPt] = new Vector3((float)Math.Cos(AngleEntreDeuxPoints * noPt + AngleDépart) * Rayon + PositionAxe.X, Position.Y, (float)Math.Sin(AngleEntreDeuxPoints * noPt + AngleDépart) * Rayon + PositionAxe.Z);
      }
   }

   protected override void LoadContent()
   {
      GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
      base.LoadContent();
   }

   public override void Update(GameTime gameTime)
   {
      float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
      TempsÉcouléDepuisMAJ += TempsÉcoulé;
      if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
      {
         CalculerPosition();
         TempsÉcouléDepuisMAJ = 0;
      }
      base.Update(gameTime);
   }

   private void CalculerPosition()
   {
      VecteurUnitaire = (PtsCercle[(NoPoint + 1) % NbPtsPatrouille] - PtsCercle[NoPoint]) / DISTANCE_ENTRE_DEUX_PTS;
      Position += VecteurUnitaire * Vitesse;
      VecteurRayon = Position - PointCentre;
      VecteurDirection = Vector3.Cross(VecteurRayon, new Vector3(0, 1, 0));
      AngleRotationYaw = (float)Math.Atan2(VecteurDirection.X - Monde.Right.X, VecteurDirection.Z - Monde.Right.Z);
      Monde = Matrix.Identity * Matrix.CreateScale(Échelle);
      Monde *= Matrix.CreateFromYawPitchRoll(AngleRotationYaw, 0, ANGLE_ROTATION_ROLL);
      Monde *= Matrix.CreateTranslation(Position);
      if (Vector3.Distance(PtsCercle[NoPoint], Position) >= Vector3.Distance(PtsCercle[(NoPoint + 1) % NbPtsPatrouille], PtsCercle[NoPoint]))
      {
         NoPoint++;
         NoPoint = NoPoint % NbPtsPatrouille;
      }

   }
}
