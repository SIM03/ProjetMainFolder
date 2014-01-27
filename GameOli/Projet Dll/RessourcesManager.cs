using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TOOLS
{
   class RessourceNotFoundException : ApplicationException { }

   public class RessourcesManager<T>
   {
      Game Jeu { get; set; }
      List<RessourceDeBase<T>> ListeRessources { get; set; }
      String Répertoire { get; set; }

      //public RessourcesManager(Game game)
      public RessourcesManager(Game game, string répertoire)
      {
         Jeu = game;
         ListeRessources = new List<RessourceDeBase<T>>();
         Répertoire = répertoire;
      }

      //public void Add(string nom)
      //{
      //   RessourceDeBase<T> ressourceÀAjouter = new RessourceDeBase<T>(Jeu.Content, nom);
      //   if (!ListeRessources.Contains(ressourceÀAjouter))
      //   {
      //      ressourceÀAjouter.Load();
      //      ListeRessources.Add(ressourceÀAjouter);
      //   }
      //}

      void Add(RessourceDeBase<T> ressourceÀAjouter)
      {
         ressourceÀAjouter.Load(Répertoire);
         ListeRessources.Add(ressourceÀAjouter);
      }

      public T Find(string nom)
      {
         const int RESSOURCE_NOT_FOUND = -1;
         RessourceDeBase<T> ressourceÀRechercher = new RessourceDeBase<T>(Jeu.Content, nom);
         int indexRessource = ListeRessources.IndexOf(ressourceÀRechercher);
         if (indexRessource == RESSOURCE_NOT_FOUND)
         {
            //throw new RessourceNotFoundException();
            Add(ressourceÀRechercher);
            indexRessource = ListeRessources.Count - 1;
         }
         return ListeRessources[indexRessource].Ressource;
      }
   }
}
