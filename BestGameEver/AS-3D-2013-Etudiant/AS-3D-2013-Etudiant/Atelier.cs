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
   public class Atelier : Microsoft.Xna.Framework.Game
   {
      const float INTERVALLE_CALCUL_FPS = 1f;
      const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
      GraphicsDeviceManager PériphériqueGraphique { get; set; }
      SpriteBatch GestionSprites { get; set; }

      RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
      RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
      RessourcesManager<Model> GestionnaireDeModèles { get; set; }
      RessourcesManager<Effect> GestionnaireDeShaders { get; set; }
      Camera CaméraJeu { get; set; }

      public InputManager GestionInput { get; private set; }

      public Atelier()
      {
         PériphériqueGraphique = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
         PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
         IsFixedTimeStep = false;
         IsMouseVisible = true;
      }

      protected override void Initialize()
      {
         const int DIMENSION_TERRAIN = 256;
         Vector2 étenduePlan = new Vector2(DIMENSION_TERRAIN, DIMENSION_TERRAIN);
         Vector2 charpentePlan = new Vector2(4, 3);
         Vector3 positionCaméra = new Vector3(0, 250, 125);
         Vector3 cibleCaméra = new Vector3(0, 0, 0);
         Vector3 positionPhare = new Vector3(0, 0, 0);
         Vector3 positionAvion1 = new Vector3(30, 20, 0);
         Vector3 positionAvion2 = new Vector3(20, 15, 5);
         Vector3 positionVaisseau = new Vector3(0, 25, 25);

         GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
         GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
         GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
         GestionnaireDeShaders = new RessourcesManager<Effect>(this, "Effects"); 
         GestionInput = new InputManager(this);
         CaméraJeu = new CaméraSubjective(this, positionCaméra, cibleCaméra, Vector3.Up, INTERVALLE_MAJ_STANDARD);

         Components.Add(GestionInput);
         Components.Add(CaméraJeu);
         Components.Add(new Afficheur3D(this));
         Components.Add(new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(DIMENSION_TERRAIN, 50, DIMENSION_TERRAIN), "CarteAS", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD));
         Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_TERRAIN / 2, DIMENSION_TERRAIN / 2, 0), étenduePlan, charpentePlan, "CielGauche", INTERVALLE_MAJ_STANDARD));
         Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_TERRAIN / 2, DIMENSION_TERRAIN / 2, 0), étenduePlan, charpentePlan, "CielDroite", INTERVALLE_MAJ_STANDARD));
         Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, DIMENSION_TERRAIN / 2, -DIMENSION_TERRAIN / 2), étenduePlan, charpentePlan, "CielAvant", INTERVALLE_MAJ_STANDARD));
         Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, DIMENSION_TERRAIN / 2, DIMENSION_TERRAIN / 2), étenduePlan, charpentePlan, "CielArrière", INTERVALLE_MAJ_STANDARD));
         Components.Add(new PlanTexturé(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, DIMENSION_TERRAIN-1, 0), étenduePlan, charpentePlan, "CielDessus", INTERVALLE_MAJ_STANDARD));
         Components.Add(new Drapeau(this, 1f, Vector3.Zero, positionPhare + Vector3.Up * 26 + Vector3.Right * 7.5f, new Vector2(15, 10), new Vector2(100, 1), "Panem", 1, 0.25f, INTERVALLE_MAJ_STANDARD));
         Components.Add(new ObjetDeBase(this, "Phare", 0.10f, Vector3.Zero, Vector3.Zero));
         Components.Add(new ObjetDePatrouille(this, "Airplane_blue", 1f, Vector3.Zero, positionAvion1, positionPhare, 18, 3, INTERVALLE_MAJ_STANDARD));
         Components.Add(new ObjetDePatrouille(this, "Feisar_Ship", 0.01f, Vector3.Zero, positionVaisseau, positionPhare, 24, 3, INTERVALLE_MAJ_STANDARD));
         Components.Add(new ObjetDePatrouille(this, "Airplane_blue", 1f, new Vector3(0, 0, 0), positionAvion2, positionPhare, 12, 3, INTERVALLE_MAJ_STANDARD));
         Components.Add(new AfficheurFPS(this, "Arial20", INTERVALLE_CALCUL_FPS));

         Services.AddService(typeof(Random), new Random());
         Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
         Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
         Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
         Services.AddService(typeof(RessourcesManager<Effect>), GestionnaireDeShaders);
         Services.AddService(typeof(InputManager), GestionInput);
         Services.AddService(typeof(Camera), CaméraJeu);
         GestionSprites = new SpriteBatch(GraphicsDevice);
         Services.AddService(typeof(SpriteBatch), GestionSprites);
         base.Initialize();
      }

      protected override void Update(GameTime gameTime)
      {
         GérerClavier();
         base.Update(gameTime);
      }

      private void GérerClavier()
      {
         Vector3 positionCaméra0 = new Vector3(0, 250, 125);
         Vector3 positionCaméra1 = new Vector3(0, 200, 0F);
         Vector3 positionCaméra2 = new Vector3(0, 70, 50);
         Vector3 positionCaméra3 = new Vector3(0, 15, 50);
         Vector3 positionCaméra4 = new Vector3(8, 26, 30);
         Vector3 cibleCaméra = new Vector3(0, 0, 0);
         if (GestionInput.EstEnfoncée(Keys.Escape))
         {
            Exit();
         }
         if (!(GestionInput.EstEnfoncée(Keys.LeftShift) || GestionInput.EstEnfoncée(Keys.RightShift) ||
               GestionInput.EstEnfoncée(Keys.LeftControl) || GestionInput.EstEnfoncée(Keys.RightControl)))
         {
            if (GestionInput.EstNouvelleTouche(Keys.D0) || GestionInput.EstNouvelleTouche(Keys.NumPad0))
            {
               CaméraJeu.Déplacer(positionCaméra0, cibleCaméra, Vector3.Up);
            }
            else
            {
               if (GestionInput.EstNouvelleTouche(Keys.D1) || GestionInput.EstNouvelleTouche(Keys.NumPad1))
               {
                  CaméraJeu.Déplacer(positionCaméra1, cibleCaméra, Vector3.Forward);
               }
               else
               {
                  if (GestionInput.EstNouvelleTouche(Keys.D2) || GestionInput.EstNouvelleTouche(Keys.NumPad2))
                  {
                     CaméraJeu.Déplacer(positionCaméra2, cibleCaméra+Vector3.Forward*10, Vector3.Up);
                  }
                  else
                  {
                     if (GestionInput.EstNouvelleTouche(Keys.D3) || GestionInput.EstNouvelleTouche(Keys.NumPad3))
                     {
                        CaméraJeu.Déplacer(positionCaméra3, cibleCaméra + Vector3.Forward * 10 + Vector3.Up * 20, Vector3.Up);
                     }
                     else
                     {
                        if (GestionInput.EstNouvelleTouche(Keys.D4) || GestionInput.EstNouvelleTouche(Keys.NumPad4))
                        {
                           CaméraJeu.Déplacer(positionCaméra4, positionCaméra4 + Vector3.Forward * 10, Vector3.Up);
                        }
                     }
                  }
               }
            }
         }
      }

      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.DarkBlue);
         base.Draw(gameTime);
      }
   }
}

