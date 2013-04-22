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
using SPBSession;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// A Game of Saving Private Bryan, wherein a player has to move a ship though a maze, avoiding antibodies and collecting letters.
    /// Game uses multi-modal input, including speech, movement and brain computing.
    /// Extends pon the Xna Framework.
    /// </summary>
    public class SPB : Microsoft.Xna.Framework.Game
    {
        #region Variables
        /// <summary>
        /// Manages the Graphics Device.
        /// </summary>
        internal GraphicsDeviceManager graphics;

        /// <summary>
        /// Used for drawing on screen
        /// </summary>
        internal GraphicsDevice device;

        /// <summary>
        /// Used to draw sprites on the screen.
        /// </summary>
        internal SpriteBatch spriteBatch;

        /// <summary>
        /// Manages the Kinect input
        /// </summary>
        internal MovementManager MovementManager;

        /// <summary>
        /// Manages the Emotive/BCI input.
        /// </summary>
        internal BCIManager BCIManager;

        /// <summary>
        /// Manages the speech input.
        /// </summary>
        internal SpeechManager SpeechManager;

        /// <summary>
        /// Manages the session for starage purposes.
        /// </summary>
        internal StorageManager StorageManager;

        KeyboardState prevState;

        /// <summary>
        /// Width of the screen in pixels.
        /// </summary>
        internal float ScreenWidth = 1280.0f;

        /// <summary>
        /// Height of the screen in pixels.
        /// </summary>
        internal float ScreenHeight = 800.0f;

        Maze maze;
        Menu menu;

        /// <summary>
        /// Indicates whether the game is paused
        /// </summary>
        internal bool paused;

        SpriteFont font;

        bool Debug; // The Debug Menu

        #endregion
        #region Initialisation
        /// <summary>
        /// Starts up the game of Saving Private Bryan.
        /// </summary>
        public SPB()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)ScreenWidth;
            graphics.PreferredBackBufferHeight = (int)ScreenHeight;

            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //this.IsMouseVisible = true;

            MovementManager = new MovementManager(); // Set up input managers
            BCIManager = new BCIManager();
            SpeechManager = new SpeechManager();

            StorageManager = new StorageManager(); // Used for storing gameplay data (allows for processing full playsessions afterwards)
            prevState = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            maze = new Maze(this, Content);

            menu = new Menu(this, Content, SpeechManager.StartSpeechManager() && MovementManager.StartKinect());

            font = Content.Load<SpriteFont>("LetterFont");
            resetGame();
            paused = true;
        }

        #endregion
        #region Updates
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit

            KeyboardState kbState = Keyboard.GetState();

            if (prevState.IsKeyDown(Keys.F) && kbState.IsKeyUp(Keys.F)) // Toggle fullscreen mode
                graphics.ToggleFullScreen();
            if (prevState.IsKeyDown(Keys.Space) && kbState.IsKeyUp(Keys.Space)) // Toggle pause mode
                paused = !paused;
            if (prevState.IsKeyDown(Keys.D) && kbState.IsKeyUp(Keys.D)) // Toggle Debug mode
                Debug = !Debug;
            prevState = kbState;

            BCIManager.Update(gameTime); // Update the input managers
            SpeechManager.Update(gameTime);

            Frame frame;

            if (paused) // Update appropriate screen.
            {
                frame = menu.Update();
            }
            else
            {
                frame = maze.Update(gameTime);
            }
            frame.GameTime = gameTime.TotalGameTime.ToString(); // Set info from playsessions
            frame.WordSpoken = SpeechManager.LastWord();
            StorageManager.AddFrame(frame);
            base.Update(gameTime);
        }

        /// <summary>
        /// Makes the game exit in a proper way. Call this function instead of this.Exit().
        /// </summary>
        public void ExitGame()
        {
            BCIManager.Disconnect();
            StorageManager.SaveSession();
            Exit();
        }

        /// <summary>
        /// Resets the game to the beginning state, allowing a new playsession.
        /// </summary>
        internal void resetGame()
        {
            paused = false;
            maze.resetGame(true);
        }

        /// <summary>
        /// Tells the game a session is over, either by player death or victory. Causes the game to reset entirely, or only the player position.
        /// </summary>
        /// <param name="FullReset">Determines whether a full reset should be exectuted or only the player should be reset (for example after player death).</param>
        internal void gameover(bool FullReset)
        {
            maze.resetGame(FullReset);
        }
#endregion
        #region Drawing
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            maze.Draw(); // We always draw field, even when paused

            if (paused)
                menu.Draw();

            spriteBatch.Begin();
            if (Debug)
                spriteBatch.DrawString(font, BCIManager.ConcentrationLevel.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
