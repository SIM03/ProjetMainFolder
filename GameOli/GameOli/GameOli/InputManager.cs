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
        public KeyboardState ÉtatClavier { get; set; }
        ButtonState[] AnciensClics { get; set; }
        ButtonState[] NouveauxClics { get; set; }
        bool EstSourisActive { get; set; }

        const int CLIC_GAUCHE = 0;
        const int CLIC_DROIT = 1;

        public InputManager(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            AnciennesTouches = new Keys[0];
            NouvellesTouches = new Keys[0];

            NouveauxClics = new ButtonState[2];
            AnciensClics = new ButtonState[2];
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            AnciennesTouches = NouvellesTouches;
            ÉtatClavier = Keyboard.GetState();
            NouvellesTouches = ÉtatClavier.GetPressedKeys();

            AnciensClics = NouveauxClics;
            //EstSourisActive = Mouse.GetState();
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
            return NouveauxClics[CLIC_DROIT] == AnciensClics[CLIC_DROIT];
        }

        public bool EstAncienClicGauche()
        {
            return NouveauxClics[CLIC_GAUCHE] == AnciensClics[CLIC_GAUCHE];
        }

        public bool EstNouveauClicDroit()
        {
            return AnciensClics[CLIC_DROIT] != NouveauxClics[CLIC_DROIT];
        }

        public bool EstNouveauClicGauche()
        {
            return AnciensClics[CLIC_GAUCHE] != NouveauxClics[CLIC_GAUCHE];
        }

        public void EtatSouris()
        {
            NouveauxClics[CLIC_GAUCHE] = Mouse.GetState().LeftButton;
            NouveauxClics[CLIC_DROIT] = Mouse.GetState().RightButton;
        }

        public Vector2 GetPositionSouris()
        {
            return new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

    }
}

