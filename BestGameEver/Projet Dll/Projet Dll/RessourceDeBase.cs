using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   class RessourceDeBase<T>:IEquatable<RessourceDeBase<T>>
   {
      //string NomComplet { get; set; }
      string Nom { get; set; }
      public T Ressource { get; private set; }
      ContentManager Content { get; set; }

      //public RessourceDeBase(ContentManager content, string nomComplet)
      public RessourceDeBase(ContentManager content, string nom)
      {
         //NomComplet = nomComplet;
         //Nom = ExtraireNomRessource(NomComplet);
         Nom = nom;
         Content = content;
         Ressource = default(T);
      }

      //private string ExtraireNomRessource(string nomComplet)
      //{
      //   return nomComplet.Substring(Math.Max(nomComplet.LastIndexOf('/'), nomComplet.LastIndexOf('\\')) + 1).ToLower();
      //}

      //public void Load()
      public void Load(string répertoire)
      {
         if (Ressource == null)
         {
            string nomComplet = répertoire + '/' + Nom;
            Ressource = Content.Load<T>(nomComplet);
         }
      }

      #region IEquatable<TextureDeBase> Membres

      public bool Equals(RessourceDeBase<T> autreRessource)
      {
         return Nom== autreRessource.Nom;
      }

      #endregion
   }
}
