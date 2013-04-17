#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SpineTest.Mono.Windows
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static SpineTest game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            game = new SpineTest();
            game.Run();
        }
    }
}
