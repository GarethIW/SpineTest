using System;

namespace SpineTest
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SpineTest game = new SpineTest())
            {
                game.Run();
            }
        }
    }
#endif
}

