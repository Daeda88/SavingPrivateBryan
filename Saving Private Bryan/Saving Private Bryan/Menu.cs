using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SPBSession;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// Menu of a game of Saving Private Bryan. Shown when the game is paused.
    /// </summary>
    internal class Menu
    {
        SPB game;

        Texture2D MenuTexture;
        SpriteFont MenuFont;

        bool supportedPlatform;

        /// <summary>
        /// Constructs a new Menu of the game.
        /// </summary>
        /// <param name="game">The game this Menu belongs to.</param>
        /// <param name="content">The Content Manager of the game, used to get required assets.</param>
        /// <param name="supported">Boolean indicating whether the current platform supports the game (ie has functional Kinect, Speech and BCI) </param>
        public Menu(SPB game, ContentManager content, bool supported)
        {
            this.game = game;
            this.supportedPlatform = supported;
            MenuFont = content.Load<SpriteFont>("MenuFont");
            MenuTexture = content.Load<Texture2D>("menuBackground");

        }

        internal Frame Update()
        {
            if (supportedPlatform) // We only handle the Speech Manager if it is functional.
            {
                if (game.SpeechManager.WordWasSaid(Keywords.ExitWord)) // Exits the game on exit-word
                    game.ExitGame();
                if (game.SpeechManager.WordWasSaid(Keywords.StartWord)) // Starts the game on start-word
                {
                    game.paused = false;
                }
                if (game.SpeechManager.WordWasSaid(Keywords.RestartWord)) // Restarts game on restart-word
                {

                    game.resetGame();
                    game.MovementManager.ResetKinect();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) // Exit button for when speech is not enabled.
                game.ExitGame();
            return new MenuFrame();
        }

        internal void Draw()
        {
            game.spriteBatch.Begin();
            game.spriteBatch.Draw(MenuTexture, new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight), Color.White);

            if (supportedPlatform) // We only Draw the Menu-options if the game can be played, ie all input is functional.
            {
                game.spriteBatch.DrawString(MenuFont, Keywords.StartWord, new Vector2(game.graphics.PreferredBackBufferWidth / 2.0f - MenuFont.MeasureString(Keywords.StartWord).X/2.0f, 300), Color.Black);

                game.spriteBatch.DrawString(MenuFont, Keywords.RestartWord, new Vector2(game.graphics.PreferredBackBufferWidth / 2.0f - MenuFont.MeasureString(Keywords.RestartWord).X / 2.0f, 350), Color.Black);

                game.spriteBatch.DrawString(MenuFont, Keywords.ExitWord, new Vector2(game.graphics.PreferredBackBufferWidth / 2.0f - MenuFont.MeasureString(Keywords.ExitWord).X / 2.0f, 400), Color.Black);
            }
            else // Show an errormessage if parts of the input wont work.
            {
                String errorMessage = "Game Not Supported on your platform.\n\nCheck whether you:\n Are using a Windows-version that supports speech recognition\n Have Kinect and Emotive running Properly.";
                game.spriteBatch.DrawString(MenuFont, errorMessage, new Vector2(game.graphics.PreferredBackBufferWidth / 2.0f - MenuFont.MeasureString(errorMessage).X / 2.0f, 300), Color.Black);
            }
            
            game.spriteBatch.End();
        }

    }
}
