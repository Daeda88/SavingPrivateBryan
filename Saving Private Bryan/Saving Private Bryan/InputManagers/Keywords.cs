using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// Static class used for storing all Speech Commands
    /// </summary>
    public static class Keywords
    {

        /// <summary>
        /// Speech Command to start the game.
        /// </summary>
        public static String StartWord = "start";

        /// <summary>
        /// Speech Command to pause the game.
        /// </summary>
        public static String PauseWord = "pause";

        /// <summary>
        /// Speech Command to exit the game.
        /// </summary>
        public static String ExitWord = "exit";

        /// <summary>
        /// Speech Command to reset the game.
        /// </summary>
        public static String RestartWord = "restart";

        /// <summary>
        /// List of the words used by the Menu.
        /// </summary>
        public static String[] MenuWords = { StartWord, PauseWord, ExitWord, RestartWord };

        /// <summary>
        /// List of words used to kill the antibodies
        /// </summary>
        public static String[] AntibodyWords = { "flower", "cat", "paper", "tree", "house" };

        /// <summary>
        /// List of collectable four letter words
        /// </summary>
        public static String[] FourLetterWords = { "face", "nose", "hair", "eyes", "ears" };

        /// <summary>
        /// List of collectable five letter words.
        /// </summary>
        public static String[] FiveLetterWords = { "brain", "blood", "mouth", "tumor", "hands" };

        /// <summary>
        /// List of collectable six letter words.
        /// </summary>
        public static String[] SixLetterWords = { "kidney" };

        /// <summary>
        /// List of collectable seven letter words.
        /// </summary>
        public static String[] SevenLetterWords = { "stomach"};

        /// <summary>
        /// List of all collectable wordlists.
        /// </summary>
        public static String[][] CollectableWords = {FourLetterWords, FiveLetterWords, SixLetterWords, SevenLetterWords};

    }
}
