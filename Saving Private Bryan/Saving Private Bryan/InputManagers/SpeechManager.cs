using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Microsoft.Xna.Framework;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// Manages Speech input in a game of Saving Private Bryan.
    /// </summary>
    class SpeechManager
    {

        SpeechRecognitionEngine speechEngine;
        SpeechSynthesizer synthesizer;

        String lastWord;
        int timeSinceLastWord;

        /// <summary>
        /// Constructs a new Speech Manager.
        /// </summary>
        public SpeechManager()
        {
            lastWord = "";
            timeSinceLastWord = 0;   
        }

        /// <summary>
        /// Starts up the Speech Manager by starting to listen to the microphone.
        /// </summary>
        /// <returns>Boolean indicating whether startup was succesful.</returns>
        internal bool StartSpeechManager()
        {
            try
            {
                synthesizer = new SpeechSynthesizer();
                speechEngine = new SpeechRecognitionEngine();
                speechEngine.SetInputToDefaultAudioDevice(); // Set up new Speech Recognition. Please note that this throws exceptions on Windows platforms not supporting Speech Recognition and will only detect speech properly on English-versions of Windows Vista of Windows 7.
                GrammarBuilder grammarBuilder = new GrammarBuilder(); // Construct a new Grammar.

                // Cosntruct a new list of all the Keywords to be detected.
                String[] phrases = new String[Keywords.MenuWords.Length + Keywords.AntibodyWords.Length + Keywords.FourLetterWords.Length + Keywords.FiveLetterWords.Length + Keywords.SixLetterWords.Length + Keywords.SevenLetterWords.Length ];
                Keywords.MenuWords.CopyTo(phrases, 0);
                Keywords.AntibodyWords.CopyTo(phrases, Keywords.MenuWords.Length);
                Keywords.FourLetterWords.CopyTo(phrases, Keywords.MenuWords.Length + Keywords.AntibodyWords.Length);
                Keywords.FiveLetterWords.CopyTo(phrases, Keywords.MenuWords.Length + Keywords.AntibodyWords.Length + Keywords.FourLetterWords.Length);
                Keywords.SixLetterWords.CopyTo(phrases, Keywords.MenuWords.Length + Keywords.AntibodyWords.Length + Keywords.FourLetterWords.Length + Keywords.FiveLetterWords.Length);
                Keywords.SevenLetterWords.CopyTo(phrases, Keywords.MenuWords.Length + Keywords.AntibodyWords.Length + Keywords.FourLetterWords.Length + Keywords.FiveLetterWords.Length + Keywords.SixLetterWords.Length);
                
                // Set up Grammar.
                grammarBuilder.Append(new Choices(phrases));
                speechEngine.UnloadAllGrammars();
                speechEngine.LoadGrammar(new Grammar(grammarBuilder));

                // Set the event when one of the keywords is heard.
                speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechEngine_SpeechRecognized);
                
                // Start listening for keywords.
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// Function to handle the event when a Keyword is spoken by the player.
        /// </summary>
        /// <param name="sender">Object that sent the event.</param>
        /// <param name="e">Argument that belongs to the event.</param>
        void speechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            lastWord = e.Result.Text.ToString();
            timeSinceLastWord = 0;
        }

        /// <summary>
        /// Updates the state of the Speech Manager.
        /// </summary>
        /// <param name="time">Time elapsed since last Update.</param>
        internal void Update(GameTime time){
            timeSinceLastWord += time.ElapsedGameTime.Milliseconds;
            if (timeSinceLastWord > TimeSpan.FromSeconds(0.5).TotalMilliseconds) // Make sure we only store spoken words for a short amount of time (but larger than one frame in case of lag)
            {
                lastWord = "";
                timeSinceLastWord = 0;
            }
        }

        /// <summary>
        /// Checks whether a specific word was said.
        /// </summary>
        /// <param name="word">The word to be checked for.</param>
        /// <returns>Boolean indicating the specific word was said.</returns>
        internal bool WordWasSaid(String word)
        {
            return lastWord.Equals(word);
        }

        /// <summary>
        /// Gives the last word said by the player.
        /// </summary>
        /// <returns>The last word said by the player.</returns>
        internal String LastWord()
        {
            return lastWord;
        }

    }
}
