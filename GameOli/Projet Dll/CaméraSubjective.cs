﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace TOOLS
{
    public class CaméraSubjective : Caméra
    {
        const float ACCÉLÉRATION = 0.001f;
        const float VITESSE_INITIALE_ROTATION = 20f;
        const float VITESSE_INITIALE_TRANSLATION = 1f;
        const float DELTA_LACET = MathHelper.Pi / 10000; // 1 degré à la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 10000; // 1 degré à la fois

        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const int DEFAULT_BUFFER_SIZE = 8;
        const float SENSIVITY = 1.00f;
        const float DEFAULT_INTERPOLATION = 0.50f;
        const float MINIMUM_MOVEMENT_CONST = 1f;
        const float GRAVITY_FACTOR = 2;
        //const float STARTING_GRAVITY = -13; // Gravity resets itself at this value when character touches the ground
        const float JUMPING_HEIGHT = 15;
        const double WAIT_TIME = 1;
        //const float ROTATION_ACCELERATION = 2;

        public Vector3 Direction { get; private set; }
        public Vector3 Latéral { get; private set; }

        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        Vector2 OriginalMouseState { get; set; }
        protected InputManager GestionInput { get; set; }
        Queue<Vector2> MouseBuffer { get; set; }
        public int BufferSize { get; private set; } /* For testing purpose public get are requiered*/
        public float Sensivity { get; private set; }
        public float InterpolationModifier { get; private set; }
        public float MINIMUM_MOVEMENT { get; private set; } /* Future Const (testing parameter only)*/

        public float Velocity { get; set; }
        public float AirTime { get; set; }
        public bool IsJumping { get; set; }
        public bool IsOnFloor { get; protected set; }

        float StartingHeight { get; set; }
        double LastJump { get; set; }
        float Gravity { get; set; }

        //Angle de rotation
        public float RotationTangage { get; private set; }
        public float RotationLacet { get; private set; }

        public float DéplacementDirection { get; private set; }
        public float DéplacementLatéral { get; private set; }

        bool estEnZoom;
        bool EstEnZoom
        {
            get { return estEnZoom; }
            set
            {
                float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
                estEnZoom = value;
                if (estEnZoom)
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
                else
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
            }
        }

        public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, float intervalleMAJ)
            : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, cible, OrientationVerticale);
            EstEnZoom = false;
            StartingHeight = positionCaméra.Y;
        }

        public override void Initialize()
        {
            OriginalMouseState = new Vector2(Game.Window.ClientBounds.Center.X, Game.Window.ClientBounds.Center.Y);
            Sensivity = SENSIVITY;
            MouseBuffer = new Queue<Vector2>();
            VitesseRotation = VITESSE_INITIALE_ROTATION;
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            Velocity = 0;
            AirTime = 0f;
            LastJump = WAIT_TIME;
            RotationLacet = 0f;
            RotationTangage = 0f;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        protected override void CréerPointDeVue()
        {
            Latéral = Vector3.Normalize(Vector3.Cross(Direction, Vector3.Up));
            Direction = Vector3.Normalize(Direction);
            Vue = Matrix.CreateLookAt(Position, Position + Direction, Vector3.Up);
            GénérerFrustum();
        }

        protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
        {
            Position = position;
            Cible = cible;
            Direction = Vector3.Normalize(Cible - Position);
            CréerPointDeVue();
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            GestionClavier();

            if (TempsÉcouléDepuisMAJ >= INTERVALLE_MAJ_STANDARD)
            {
                GravityHandler(gameTime);
                GérerDéplacement();
                GestionSouris(gameTime);
                GérerRotation();
                CréerPointDeVue();
                TempsÉcouléDepuisMAJ = 0;

            }

            OriginalMouseState = GestionInput.PositionSouris();
            base.Update(gameTime);
        }

        protected virtual void GravityHandler(GameTime gameTime)
        {
            if (!IsOnFloor)
            {
                AirTime += 0.005f;
                Velocity -= (GRAVITY_FACTOR * AirTime);
            }
            else
            {
                if (Velocity != 0)
                    Velocity = 0;
                AirTime = 0;
            }

            Velocity += JumpHandler(gameTime);
            Position = new Vector3(Position.X, Position.Y + Velocity, Position.Z);
        }


        private int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }

        protected virtual void GérerDéplacement()
        {
            float DéplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
            float DéplacementLatéral = (GérerTouche(Keys.D) - GérerTouche(Keys.A)) * VitesseTranslation;
            if (DéplacementDirection != 0)
                Position += new Vector3(Direction.X, 0, Direction.Z) * DéplacementDirection;
            if (DéplacementLatéral != 0)
                Position += Latéral * DéplacementLatéral;
        }

        public void ManageTeleportation(Vector3 newCameraPosition, Vector3 newCameraTarget)
        {
            Position = newCameraPosition;
            Direction = newCameraTarget;
        }

        protected float JumpHandler(GameTime gametime)
        {
            float Vertical = 0;

            if (GestionInput.EstEnfoncée(Keys.Space) && IsOnFloor)// && (LastJump >= WAIT_TIME) && !IsJumping)
            {
                //IsJumping = true;
                Vertical = JUMPING_HEIGHT;
                LastJump = 0;
            }

            //if (IsJumping)
            //{
            //    if (IsOnFloor)
            //    {
            //        IsJumping = false;
            //    }

            //    else
            //        Vertical = JUMPING_HEIGHT;
            //}

            //if (!IsJumping)
            //{
               LastJump += gametime.ElapsedGameTime.TotalSeconds;
            //}

            //if (LastJump < 1.4f * WAIT_TIME)
            //{
            //    LandCam();
            //}
            //else
            //{
            //    ForcedRotationY = 0;
            //}

            return Vertical;
        }

        private void GérerRotation()
        {
            Vector2 RotationXY = new Vector2((OriginalMouseState.X - GestionInput.PositionSouris().X), (OriginalMouseState.Y - GestionInput.PositionSouris().Y));
            GérerLacet(MathHelper.WrapAngle(RotationXY.X * DELTA_LACET * Sensivity));
            GérerTangage(MathHelper.WrapAngle(RotationXY.Y * DELTA_TANGAGE * Sensivity));
        }

        private void GérerLacet(float RotationX)
        {
            RotationLacet = RotationX;
            if (RotationLacet != 0)
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Up, RotationLacet));
        }
        private void GérerTangage(float RotationY)
        {
            float RotationTangage = RotationY;
            if (RotationTangage != 0)
            {
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, RotationTangage));
                Latéral = Vector3.Normalize(Vector3.Cross(Direction, Vector3.Up));
            }
        }

        protected virtual void GestionClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Z))
            {
                EstEnZoom = !EstEnZoom;
            }

            if (GestionInput.EstNouvelleTouche(Keys.Q))
            {
                Sensivity += 0.05f;
            }
            if (GestionInput.EstNouvelleTouche(Keys.E))
            {
                Sensivity -= 0.05f;
            }
        }

        private void GestionSouris(GameTime gametime)
        {
            if (!(GestionInput.PositionSouris().Y <= Game.Window.ClientBounds.Top || GestionInput.PositionSouris().Y >= Game.Window.ClientBounds.Bottom))
            {
                Mouse.SetPosition((int)GestionInput.PositionSouris().X, (int)Game.Window.ClientBounds.Center.Y);
            }
            if (!(GestionInput.PositionSouris().X >= Game.Window.ClientBounds.Right || GestionInput.PositionSouris().X <= Game.Window.ClientBounds.Left))
            {
                Mouse.SetPosition((int)Game.Window.ClientBounds.Center.X, (int)GestionInput.PositionSouris().Y);
            }
        }

        public ValueType GetStats(string parameter)
        {
            ValueType Stat;
            switch (parameter)
            {
                case "Sensitivity":
                    Stat = this.Sensivity;
                    break;
                case "Position":
                    Stat = this.Position;
                    break;
                case "Direction":
                    Stat = this.Direction;
                    break;
                case "Velocity":
                    Stat = this.Velocity;
                    break;
                case "AirTime":
                    Stat = this.AirTime;
                    break;
                case "IsOnFloor":
                    Stat = this.IsOnFloor;
                    break;
                case "IsJumping":
                    Stat = this.IsJumping;
                    break;
                default:
                    Stat = 09190;
                    break;
            }

            return Stat;
        }
    }

}