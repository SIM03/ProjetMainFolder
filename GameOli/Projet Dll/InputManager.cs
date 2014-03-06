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
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        Keys[] AnciennesTouches { get; set; }
        Keys[] NouvellesTouches { get; set; }
        KeyboardState ÉtatClavier { get; set; }

        ButtonState[] AnciensClics { get; set; }
        ButtonState[] NouveauxClics { get; set; }
        const int CLIC_GAUCHE = 0;
        const int CLIC_DROIT = 1;

        float LastScrollWheelValue { get; set; }
        

        public InputManager(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            AnciennesTouches = new Keys[0];
            NouvellesTouches = new Keys[0];

            NouveauxClics = new ButtonState[2];
            AnciensClics = new ButtonState[2];

            LastScrollWheelValue = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            AnciennesTouches = NouvellesTouches;
            ÉtatClavier = Keyboard.GetState();
            NouvellesTouches = ÉtatClavier.GetPressedKeys();

            AnciensClics[CLIC_GAUCHE] = NouveauxClics[CLIC_GAUCHE];
            AnciensClics[CLIC_DROIT] = NouveauxClics[CLIC_DROIT];
            EtatSouris();
            base.Update(gameTime);
        }

        public bool EstClavierActivé
        {
            get { return NouvellesTouches.Length > 0; }
        }

        public bool EstEnfoncée(Keys touche)
        {
            return ÉtatClavier.IsKeyDown(touche);
        }

        public bool EstNouvelleTouche(Keys touche)
        {
            int NbTouches = AnciennesTouches.Length;
            bool EstNouvelleTouche = ÉtatClavier.IsKeyDown(touche);
            int i = 0;
            while (i < NbTouches && EstNouvelleTouche)
            {
                EstNouvelleTouche = AnciennesTouches[i] != touche;
                ++i;
            }
            return EstNouvelleTouche;
        }

        public bool EstAncienClicDroit()
        {
            return AnciensClics[CLIC_DROIT] == NouveauxClics[CLIC_DROIT] 
                && AnciensClics[CLIC_GAUCHE] == ButtonState.Pressed;  
        }

        public bool EstAncienClicGauche()
        {
            return AnciensClics[CLIC_GAUCHE] == NouveauxClics[CLIC_GAUCHE] 
                && AnciensClics[CLIC_GAUCHE] == ButtonState.Pressed; 
        }

        public bool EstNouveauClicDroit()
        {
            return AnciensClics[CLIC_DROIT] != NouveauxClics[CLIC_DROIT] 
                && NouveauxClics[CLIC_DROIT] != ButtonState.Released;
        }

        public bool EstNouveauClicGauche()
        {
            return AnciensClics[CLIC_GAUCHE] != NouveauxClics[CLIC_GAUCHE] 
                && NouveauxClics[CLIC_GAUCHE] == ButtonState.Released;
        }

        public void EtatSouris()
        {
            NouveauxClics[CLIC_GAUCHE] = Mouse.GetState().LeftButton;
            NouveauxClics[CLIC_DROIT] = Mouse.GetState().RightButton;
        }

        public Vector2 PositionSouris()
        {
            int X = Mouse.GetState().X;
            int Y = Mouse.GetState().Y;
            return new Vector2(X, Y);
        }

        public float ScrollingValue()
        {
            float value;
            value = (Mouse.GetState().ScrollWheelValue - LastScrollWheelValue);
            LastScrollWheelValue= Mouse.GetState().ScrollWheelValue;
            return value;
        }

       

    }
}

    