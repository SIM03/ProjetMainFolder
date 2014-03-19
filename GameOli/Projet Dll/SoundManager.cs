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
using System.IO;


namespace TOOLS
{
   /// <summary>
   /// This is a game component that implements IUpdateable.
   /// </summary>
   public class SoundManager : Microsoft.Xna.Framework.GameComponent
   {
      List<Song> SongList { get; set; }
      List<SoundEffect> SoundEffectList { get; set; }

      List<string> SongNameList { get; set; }
      List<string> SoundEffectNameList { get; set; }

      RessourcesManager<Song> SongManager { get; set; }
      RessourcesManager<SoundEffect> SoundEffectManager { get; set; }
      InputManager InputManager { get; set; }

      bool song;
      bool soundEffect;
      

      public SoundManager(Game game, string fileName)
         : base(game)
      {
         SongNameList = new List<string>();
         SoundEffectNameList = new List<string>();

         StreamReader X = new StreamReader("../../../../GameOliContent/Sounds/" + fileName);
         int lineCounter = 0;
         string line;

         while((line = X.ReadLine()) != null)
         {
            if (line == "Song")
            {
               song = true;
               soundEffect = false;
            }


            if (line == "SoundEffect")
            {
               soundEffect = true;
               song = false;
            }

            if(song && line != "Song")
               SongNameList.Add(line);

            if(soundEffect && line != "SoundEffect")
               SoundEffectNameList.Add(line);
            lineCounter++;
         }
         X.Close();
      }

      /// <summary>
      /// Allows the game component to perform any initialization it needs to before starting
      /// to run.  This is where it can query for any required services and load content.
      /// </summary>
      public override void Initialize()
      {
         SongList = new List<Song>();
         SoundEffectList = new List<SoundEffect>();

         InputManager = Game.Services.GetService(typeof(InputManager)) as InputManager;

         SongManager = Game.Services.GetService(typeof(RessourcesManager<Song>)) as RessourcesManager<Song>;
         SoundEffectManager = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;

         foreach (string SongName in SongNameList)
         {
            SongList.Add(SongManager.Find(SongName));
         }
         foreach (string SoundEffectName in SoundEffectNameList)
         {
            SoundEffectList.Add(SoundEffectManager.Find(SoundEffectName));
         }

         MediaPlayer.Play(SongList[0]);
         base.Initialize();
      }

      /// <summary>
      /// Allows the game component to update itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      public override void Update(GameTime gameTime)
      {
         if (InputManager.EstEnfoncée(Keys.Space))
            SoundEffectList[0].Play();
         base.Update(gameTime);
      }
   }
}
