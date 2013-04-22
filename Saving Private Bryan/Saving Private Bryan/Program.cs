using System;

namespace Saving_Private_Bryan
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SPB game = new SPB())
            {
                game.Run();
            }
        }
    }
#endif
}

