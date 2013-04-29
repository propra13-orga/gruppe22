#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace Gruppe22
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (MainWindow game = new MainWindow())
                game.Run();
        }
    }
#endif
}
