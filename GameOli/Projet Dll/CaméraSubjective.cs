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


namespace TOOLS
{
    public class CaméraSubjective : Caméra
    {
        const float ACCÉLÉRATION = 0.0001f;
        const float VITESSE_INITIALE_ROTATION = 20f;
        const float VITESSE_INITIALE_TRANSLATION = 0.2f;
        const float DELTA_LACET = MathHelper.Pi / 10000; // 1 degré à la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 10000; // 1 degré à la fois

        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const int DEFAULT_BUFFER_SIZE = 8;
        const float SENSIVITY = 1.00f;
        const float DEFAULT_INTERPOLATION = 0.50f;
        const float MINIMUM_MOVEMENT_CONST = 1f;
        const float GRAVITY_FACTOR = 0.1f;
        //const float STARTING_GRAVITY = -13; // Gravity resets itself at this value when character touches the ground
        const float JUMPING_HEIGHT = 0.5f;
        const double WAIT_TIME = 1;
        //const float ROTATION_ACCELERATION = 2;

        public Vector3 Direction { get; set; }
        public Vector3 Latéral { get; set; }
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
        float Gravity { get; set; }
        float Velocity { get; set; }
        protected float AirTime { get; set; }
        float StartingHeight { get; set; }
        double LastJump { get; set; }
        bool IsJumping { get; set; }
        
        //float ForcedRotationY { get; set; }
        //float RotationFactor { get; set; }


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

        protected bool IsOnFloor { get; set; }
        



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
            BufferSize = DEFAULT_BUFFER_SIZE; /* Temporaire pour faute de savoir ou placer l<initialisaton du buffer size */
            InterpolationModifier = DEFAULT_INTERPOLATION;
            MINIMUM_MOVEMENT = MINIMUM_MOVEMENT_CONST;
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

                GérerRotation(gameTime);
                CréerPointDeVue();
                GestionSouris(gameTime);
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
            float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
            float déplacementLatéral = (GérerTouche(Keys.D) - GérerTouche(Keys.A)) * VitesseTranslation;
            if (déplacementDirection != 0)
                Position += new Vector3(Direction.X, 0, Direction.Z) * déplacementDirection;
            if (déplacementLatéral != 0)
                Position += Latéral * déplacementLatéral;
        }

        protected float JumpHandler(GameTime gametime)
        {
            float Vertical = 0;
            if (GestionInput.EstEnfoncée(Keys.Space) && IsOnFloor)// && (LastJump >= WAIT_TIME) && !IsJumping)
            {
                IsJumping = true;
                Vertical = JUMPING_HEIGHT;
                LastJump = 0;
            }
            if (IsJumping)
            {
                if (IsOnFloor)
                {
                    IsJumping = false;
                }

                else
                    Vertical = JUMPING_HEIGHT;
            }

            if (!IsJumping)
            {
               LastJump = (LastJump + gametime.ElapsedGameTime.TotalSeconds);
            }

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

        //private void LandCam()
        //{
        //    if (ForcedRotationY <= 0)
        //    {
        //        ForcedRotationY -= RotationFactor;
        //        RotationFactor *= ROTATION_ACCELERATION;
        //    }
        //    if (ForcedRotationY >= 5000)
        //    {
        //        ForcedRotationY += RotationFactor;
        //        RotationFactor /= ROTATION_ACCELERATION;
        //    }
        //}

        private void GérerRotation(GameTime gameTime)
        {
            Vector2 RotationXY = BufferInterpolation(gameTime);
            //RotationXY.Y += ForcedRotationY * DELTA_TANGAGE;
            GérerLacet(MathHelper.WrapAngle(RotationXY.X * DELTA_LACET));
            GérerTangage(MathHelper.WrapAngle(RotationXY.Y * DELTA_TANGAGE));
        }

        private void GérerLacet(float RotationX)
        {
            float rotationLacet = RotationX;
            if (rotationLacet != 0)
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Up, rotationLacet));
        }
        private void GérerTangage(float RotationY)
        {
            float rotationTangage = RotationY;
            if (rotationTangage != 0)
            {
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, rotationTangage));
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
            if (GestionInput.EstNouvelleTouche(Keys.NumPad8)||(GestionInput.EstNouvelleTouche(Keys.D2)))
            {
                InterpolationModifier += 0.05f;
            }
            if (GestionInput.EstNouvelleTouche(Keys.NumPad5) || (GestionInput.EstNouvelleTouche(Keys.D1)))
            {
                InterpolationModifier -= 0.05f;
            }
            if (GestionInput.EstNouvelleTouche(Keys.NumPad6))
            {
                ++BufferSize;
            }
            if (GestionInput.EstNouvelleTouche(Keys.NumPad4))
            {
                --BufferSize;
            }
            if (GestionInput.EstNouvelleTouche(Keys.NumPad9))
            {
                MINIMUM_MOVEMENT += 0.1f;
            }
            if (GestionInput.EstNouvelleTouche(Keys.NumPad7))
            {
                MINIMUM_MOVEMENT -= 0.1f;
            }
        }

        private void GestionSouris(GameTime gametime)
        {
            BufferManagement(gametime);
            if (!(GestionInput.PositionSouris().Y <= Game.Window.ClientBounds.Top || GestionInput.PositionSouris().Y >= Game.Window.ClientBounds.Bottom))
            {
                Mouse.SetPosition((int)GestionInput.PositionSouris().X, (int)Game.Window.ClientBounds.Center.Y);
            }
            if (!(GestionInput.PositionSouris().X >= Game.Window.ClientBounds.Right || GestionInput.PositionSouris().X <= Game.Window.ClientBounds.Left))
            {
                Mouse.SetPosition((int)Game.Window.ClientBounds.Center.X, (int)GestionInput.PositionSouris().Y);
            }
        }

        private void BufferManagement(GameTime gametime)
        {
            MouseBuffer.Enqueue(new Vector2((OriginalMouseState.X - GestionInput.PositionSouris().X) * (float)gametime.ElapsedGameTime.TotalMilliseconds, (OriginalMouseState.Y - GestionInput.PositionSouris().Y) * (float)gametime.ElapsedGameTime.TotalMilliseconds));
            if (MouseBuffer.Count > BufferSize)
            {
                while (MouseBuffer.Count > BufferSize)
                {
                    MouseBuffer.Dequeue();
                }
            }
        }

        private Vector2 BufferInterpolation(GameTime gameTime)
        {
            //float yValue = 0f;
            //float xValue = 0f;
            //float weight = 1.0f;
            //for (int i = 0; i < MouseBuffer.Count; i++)
            //{
            //    if (MouseBuffer.ElementAt(i) != Vector2.Zero && (Math.Abs(MouseBuffer.ElementAt(i).Y) > MINIMUM_MOVEMENT || Math.Abs(MouseBuffer.ElementAt(i).X) > MINIMUM_MOVEMENT))
            //    {
            //        xValue += (MouseBuffer.ElementAt(i).X * weight) * Sensivity;
            //        yValue += (MouseBuffer.ElementAt(i).Y * weight) * Sensivity;
            //        weight *= InterpolationModifier;
            //    }
            //}
            //if (Math.Abs(xValue) < MINIMUM_MOVEMENT)
            //    xValue = 0;

            //if (Math.Abs(yValue) < MINIMUM_MOVEMENT)
            //    yValue = 0;

            //Vector2 Transformation = new Vector2(xValue, yValue);
            Vector2 Transformation = new Vector2((OriginalMouseState.X - GestionInput.PositionSouris().X) * (float)gameTime.TotalGameTime.TotalSeconds, (OriginalMouseState.Y - GestionInput.PositionSouris().Y) * (float)gameTime.TotalGameTime.TotalSeconds);
            return Transformation;
        }

       
        public ValueType GetStats(string parameter)
        {
            ValueType Stat;
            switch (parameter)
            {
                case "Sensitivity":
                    Stat = this.Sensivity;
                    break;
                case "BufferSize":
                    Stat = this.BufferSize;
                    break;
                case "InterpolationModifier":
                    Stat = this.InterpolationModifier;
                    break;
                case "MINIMUM_MOVEMENT":
                    Stat = this.MINIMUM_MOVEMENT;
                    break;
                default:
                    Stat = 333;
                    break;
            }

            return Stat;
        }
    }

}