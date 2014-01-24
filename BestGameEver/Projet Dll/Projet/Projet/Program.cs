using System;

namespace Projet
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GAME game = new GAME())
            {
                game.Run();
            }
        }
    }
#endif
}

