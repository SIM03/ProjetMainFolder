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


namespace AtelierXNA
{
    public class ObjetDeDemo : ObjetDeBase
    {
        bool EstRotationY;
        bool EstRotationX;
        bool EstRotationZ;
        bool EstZoom;
        bool EstDezoom;
        bool EstDeplacementDroite;
        bool EstDeplacementGauche;
        bool EstDeplacementHaut;
        bool EstDeplacementBas;
        bool EstRotationDefault;

        const float VITESSE_ROTATION = 0.01f;
        const float FACTEUR_DE_ZOOM =0.0002f;
        const float VITESSE_DEPLACEMENT = 0.1f;

        Vector3 RotationInitial;
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        InputManager GestionInput { get; set; }
        public ObjetDeDemo(Game jeu, String nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            EstRotationY = false;
            EstRotationX = false;
            EstRotationZ = false;
            EstZoom = false;
            EstDezoom = false;
            EstDeplacementDroite = false;
            EstDeplacementGauche = false;
            EstDeplacementHaut = false;
            EstDeplacementBas = false;
            EstRotationDefault = false;
            RotationInitial = Rotation;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            
        }

       
        public override void Update(GameTime gameTime)
        {
            Choisirtranslation();
            ChoisirZoom();
            ChoisirRotation();

            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                EffectuerTransformations();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
            
        }

        private void Choisirtranslation()
        {
            EstDeplacementDroite = GestionInput.EstEnfoncée(Keys.Right);
            EstDeplacementGauche = GestionInput.EstEnfoncée(Keys.Left);
            EstDeplacementHaut = GestionInput.EstEnfoncée(Keys.Up);
            EstDeplacementBas = GestionInput.EstEnfoncée(Keys.Down);
        }

        private void ChoisirZoom()
        {
            EstZoom = GestionInput.EstEnfoncée(Keys.OemPlus);
            EstDezoom = GestionInput.EstEnfoncée(Keys.OemMinus);
        }

        private void ChoisirRotation()
        {
            if (GestionInput.EstNouvelleTouche(Keys.D1))
            {
                if (EstRotationY == true)
                    EstRotationY = false;
                else
                    EstRotationY = true;
            }
            if (GestionInput.EstNouvelleTouche(Keys.D2))
            {
                if (EstRotationX == true)
                    EstRotationX = false;
                else
                    EstRotationX = true;
            }
            if (GestionInput.EstNouvelleTouche(Keys.D3))
            {
                if (EstRotationZ == true)
                    EstRotationZ = false;
                else
                    EstRotationZ = true;
            }
            if (GestionInput.EstNouvelleTouche(Keys.Space))
            {
                EstRotationDefault = true;
            }
            
        }

        private void EffectuerTransformations()
        {
            EffectuerRotation();
            EffectuerZoom();
            EffectuerTranslation();
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        private void EffectuerTranslation()
        {
            if (EstDeplacementDroite == true)
            {
                Position = new Vector3(Position.Y, Position.X + VITESSE_DEPLACEMENT, Position.Z);
                Position = new Vector3(Position.Y + VITESSE_DEPLACEMENT, Position.X, Position.Z);
            }
            if (EstDeplacementGauche == true)
            {
                Position = new Vector3(Position.Y, Position.X - VITESSE_DEPLACEMENT, Position.Z);
                Position = new Vector3(Position.Y - VITESSE_DEPLACEMENT, Position.X, Position.Z);
            }
            if (EstDeplacementHaut == true)
            {
                Position = new Vector3(Position.Y + VITESSE_DEPLACEMENT, Position.X, Position.Z);
                Position = new Vector3(Position.Y, Position.X + VITESSE_DEPLACEMENT, Position.Z);
            }
            if (EstDeplacementBas == true)
            {
                Position = new Vector3(Position.Y - VITESSE_DEPLACEMENT, Position.X, Position.Z);
                Position = new Vector3(Position.Y, Position.X - VITESSE_DEPLACEMENT, Position.Z);
            }

        }

        private void EffectuerZoom()
        {
            if (EstZoom && Échelle <= 0.02f)
            {
                Échelle += FACTEUR_DE_ZOOM;
            }
            if (EstDezoom && Échelle >= 0.002f)
            {
               Échelle -= FACTEUR_DE_ZOOM; 
            }

        }

        private void EffectuerRotation()
        {
            if (EstRotationY)
            {
                Rotation = new Vector3(Rotation.Y + VITESSE_ROTATION, Rotation.X , Rotation.Z);
                Rotation = new Vector3(Rotation.Y, Rotation.X + VITESSE_ROTATION, Rotation.Z);
            }
            if (EstRotationX)
            {
                Rotation = new Vector3(Rotation.Y, Rotation.X + VITESSE_ROTATION, Rotation.Z);
                Rotation = new Vector3(Rotation.Y + VITESSE_ROTATION, Rotation.X, Rotation.Z);
            }
            if (EstRotationZ)
            {
                Rotation = new Vector3(Rotation.Y, Rotation.X, Rotation.Z + VITESSE_ROTATION);
                Rotation = new Vector3(Rotation.Y, Rotation.X, Rotation.Z + VITESSE_ROTATION);
            }
            if(EstRotationDefault)
            {
                Rotation = RotationInitial;
                EstRotationDefault = false;
            }
            
           
        }

        

        
    }
}
