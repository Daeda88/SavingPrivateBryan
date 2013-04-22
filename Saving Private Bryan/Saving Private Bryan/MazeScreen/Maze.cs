using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SPBSession;
using Microsoft.Xna.Framework.Audio;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// The Maze that is navigated through in a game of Saving Private Bryan. Handles all ingame logic.
    /// </summary>
    internal class Maze
    {
        #region Variables
        // Parent information
        SPB game;

        // Maze Information
        Texture2D Background;
        Color[] MazeInfo;
        internal float ActualWidth;
        internal float ActualHeight;

        // Magnifier Information
        Texture2D Magnifier;
        Color[] MagnifierEffect;
        float WindowWidth;
        float WindowHeight;
        RenderTarget2D MagnifierTarget;

        // Antibody Information
        List<Antibody> antibodies;
        internal List<Antibody> toDelete;
        internal Texture2D[] antibodyTextures;
        public static int MAX_ANTIBODIES = 15;
        internal Color[] ABCollisionData;
        internal SpriteFont abFont;

        // Player Information
        internal Player player;
        Vector2 startPosition;
        internal Color[] PlayerCollisionSphere;
        internal SoundEffect PlayerMovingSound;

        // Letter Information
        internal String requiredWord;
        List<Letter> Letters;
        internal SpriteFont LetterFont;
        List<Vector2> LetterPositions;

        // Virus Information;
        internal Virus Virus;
        internal Texture2D VirusTexture;
        Vector2 VirusPosition;

        Random rand;
#endregion
        #region SetUp
        /// <summary>
        /// Strats up a new Maze to be used in a game of Saving Private Bryan.
        /// </summary>
        /// <param name="game">The SPB-game this maze belongs to.</param>
        /// <param name="content">The content managers, used for loading all the assets.</param>
        public Maze(SPB game, ContentManager content)
        {
            this.game = game;

            // Set up and process the maze.
            SetUpMaze(content);
            ProcessMaze();

            // Put the player in the maze.
            SetUpPlayer(content);

            // Create antibodies.
            SetUpAntibodies(content);

            // Place all the letters.
            SetUpLetters(content);

            // Place the virus.
            SetUpVirus(content);
            
        }

        /// <summary>
        /// Set up the maze itself.
        /// </summary>
        /// <param name="content">The contantmanager from which all the assets should be loaded.</param>
        private void SetUpMaze(ContentManager content)
        {
            // Get the background to be drawns
            this.Background = content.Load<Texture2D>("background");

            // Get the collisionmap.
            Texture2D MazeInfoTexture = content.Load<Texture2D>("mazeCollisionMap");
            this.MazeInfo = new Color[MazeInfoTexture.Height * MazeInfoTexture.Width];
            MazeInfoTexture.GetData<Color>(this.MazeInfo);

            // Get all the assets used for the magnifierEffect.
            Magnifier = content.Load<Texture2D>("Magnifier");
            Texture2D MagnifierText = content.Load<Texture2D>("MagnifierEffect");
            MagnifierEffect = new Color[(int)(MagnifierText.Width * MagnifierText.Height)];
            MagnifierText.GetData<Color>(MagnifierEffect);

            // Store information about the actual formats of the maze
            ActualHeight = Background.Height;
            ActualWidth = Background.Width;

            // Get information about the window wherein the game takes place (ie fully zoomed in)
            WindowWidth = MagnifierText.Width;
            WindowHeight = MagnifierText.Height;

            // Set up a rendertarget for the magnifiereffect.
            MagnifierTarget = new RenderTarget2D(game.device, (int)WindowWidth, (int)WindowHeight);
        }

        /// <summary>
        /// Processes the entire maze in search of information stored about the player, letter locations etc.
        /// </summary>
        private void ProcessMaze()
        {
            LetterPositions = new List<Vector2>();
            for (int y = 0; y < ActualHeight; y++ )
            {
                for (int x = 0; x < ActualWidth; x++ )
                {
                    if (MazeInfo[(y * (int)ActualWidth) + x].Equals(Color.Red)) // Player Startposition found
                        startPosition = new Vector2(x, y);
                    if (MazeInfo[(y * (int)ActualWidth) + x].Equals(Color.Blue)) // A letter position found
                        LetterPositions.Add(new Vector2(x,y));
                    if (MazeInfo[(y * (int)ActualWidth) + x].Equals(Color.Lime)) // The virus position found
                        VirusPosition = new Vector2(x, y);
                }
            }
        }

        /// <summary>
        /// Sets up the player
        /// </summary>
        /// <param name="content">Contentmanager from which to load all the assets.</param>
        private void SetUpPlayer(ContentManager content)
        {
            // Load all assets
            Texture2D CollisionSphereTexture = content.Load<Texture2D>("collisionSphere");
            PlayerMovingSound = content.Load<SoundEffect>("move");
            PlayerCollisionSphere = new Color[CollisionSphereTexture.Width * CollisionSphereTexture.Height];
            CollisionSphereTexture.GetData<Color>(PlayerCollisionSphere);

            // Create instance of the player.
            this.player = new Player(this, content.Load<Texture2D>("player"), content.Load<Texture2D>("shield"), content.Load<Texture2D>("exhaust"));
        }

        /// <summary>
        /// Sets up the antibodies
        /// </summary>
        /// <param name="content">Content manager for all assets</param>
        private void SetUpAntibodies(ContentManager content)
        {
            // Load all assets
            antibodyTextures = new Texture2D[3];
            antibodyTextures[0] = content.Load<Texture2D>("AntiBodyGreen");
            antibodyTextures[1] = content.Load<Texture2D>("AntiBodyYellow");
            antibodyTextures[2] = content.Load<Texture2D>("AntiBodyRed");
            ABCollisionData = new Color[antibodyTextures[0].Width * antibodyTextures[0].Height];
            antibodyTextures[0].GetData<Color>(ABCollisionData);
            abFont = content.Load<SpriteFont>("AntibodySpriteFont");
        }

        /// <summary>
        /// Sets up the letters in the maze.
        /// </summary>
        /// <param name="content">Contentmanager for assets</param>
        private void SetUpLetters(ContentManager content)
        {
            // We fill this list after each reset.
            Letters = new List<Letter>();

            // Load assets.
            LetterFont = content.Load<SpriteFont>("LetterFont");               
        }

        /// <summary>
        /// Sets up the Virus in the game
        /// </summary>
        /// <param name="content">Contentmanager for assets</param>
        private void SetUpVirus(ContentManager content)
        {
            // Load assets.
            VirusTexture = content.Load<Texture2D>("Virus");
            
            // Create virus.
            Virus = new Virus(this, VirusPosition);
        }

        #endregion
        #region InputManagers

        /// <summary>
        /// Gets the MovementManager used for the maze
        /// </summary>
        /// <returns>The MovementManager used by this maze</returns>
        internal MovementManager GetMovementManager()
        {
            return game.MovementManager;
        }

        /// <summary>
        /// Gets the BCIManager used for the maze
        /// </summary>
        /// <returns>The BCIManager used by the maze</returns>
        internal BCIManager GetBCIManager()
        {
            return game.BCIManager;
        }

        /// <summary>
        /// Gets the SpeechManager used for the maze
        /// </summary>
        /// <returns>The SpeechManager used by the maze</returns>
        internal SpeechManager GetSpeechManager()
        {
            return game.SpeechManager;
        }

        #endregion
        #region Update

        /// <summary>
        /// Updates the maze for a new frame
        /// </summary>
        /// <param name="time">Time elapsed during gameplay</param>
        /// <returns>A frame describing everything if interest for the SessionManager</returns>
        internal Frame Update(GameTime time)
        {
            MazeFrame frame = new MazeFrame();

            // Update player
            frame = player.Update(time, frame);

            if (player.moving && game.BCIManager.IsConcentrating()) // Moving players cant concentrate
                game.BCIManager.StopConcentrating();
            else if (!player.moving && !game.BCIManager.IsConcentrating()) // Non-moving players should start concentrating
                game.BCIManager.StartConcentrating();

            //Update virus
            frame = Virus.Update(time, frame);

            // Update antibodies and prepare for possible deletion.
            toDelete = new List<Antibody>();
            foreach (Antibody antibody in antibodies)
                frame = antibody.Update(time, frame);

            // Update all letters
            foreach (Letter letter in Letters)
                frame = letter.Update(time, frame);

            // If the player is moving, we might add some antibodies (if we havent reached the maximum and some small chance is present)
            if (player.moving && rand.Next(1000) < 10 && antibodies.Count < MAX_ANTIBODIES)
            {
                // Get a random position for the antibody to appear at. Should be within a specific range of the player.
                float angle = MathHelper.TwoPi * (float)rand.NextDouble() - MathHelper.Pi;
                float distance = rand.Next(100, (int)(WindowWidth / 2.0f) - 20);
                Vector2 abPosition = new Vector2(player.Position.X + ((float)Math.Sin((double)angle) * distance), player.Position.Y + ((float)Math.Cos((double)angle) * distance));
                antibodies.Add(new Antibody(this, abPosition, Keywords.AntibodyWords[rand.Next(Keywords.AntibodyWords.Length)], TimeSpan.FromSeconds(rand.Next(5, 10)).TotalMilliseconds));
            }
            
            // Delete all antibodies that died.
            foreach (Antibody ab in toDelete)
            {
                antibodies.Remove(ab);
            }

            // Reset the player if he died.
            if (player.died)
                game.gameover(false);

            // Set information to the frame (for session manager)
            frame.BCIValue = GetBCIManager().ConcentrationLevel;
            frame.LeanForward = GetMovementManager().MovesForward();
            frame.LeanLeft = GetMovementManager().MovesLeft();
            frame.LeanRight = GetMovementManager().MovesRight();
            return frame;
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        /// <param name="gameEnded">Indicates whether the game should be fully reset or only the player position.</param>
        internal void resetGame(bool gameEnded)
        {
            player.resetPlayer(startPosition);
            antibodies = new List<Antibody>();
            rand = new Random();
            if (gameEnded) // We only start collecting a new word if the game is fully reset.
            {
                resetLetters();
            }
        }

        /// <summary>
        /// Rests all the letters to a new position
        /// </summary>
        private void resetLetters()
        {
            // Make sure we use the maximum amount of available letter spaces
            float MaxLettersF = LetterPositions.Count;
            MathHelper.Clamp(MaxLettersF, 4.0f, 7.0f);
            int MaxLetters = (int)MaxLettersF;

            // Get the correct collection of words to collect and pick one.
            String[] availableWords = Keywords.CollectableWords[MaxLetters - 4];
            Random random = new Random();
            requiredWord = availableWords[random.Next(availableWords.Length)];

            int i = 0;
            float xOffset = 50; // Offset for position of letters after pickup
            Letters = new List<Letter>();
            // Make sure we dont forget the positions of the letters.
            List<Vector2> LetterPositions_Backup = new List<Vector2>();
            while (i < requiredWord.Length)
            {
                // Get a random position for the next letter
                Vector2 LetterPosition = LetterPositions.ElementAt<Vector2>(random.Next(LetterPositions.Count));
                LetterPositions.Remove(LetterPosition);
                LetterPositions_Backup.Add(LetterPosition);

                // Add the next letter to the random position.
                String LetterString = Char.ToUpper(requiredWord.ElementAt<char>(i)).ToString();
                Letters.Add(new Letter(this, LetterPosition, new Vector2(xOffset, game.ScreenHeight - 50), LetterString));
                i++;

                // Update the xOffset for after pickUp.
                xOffset += LetterFont.MeasureString(LetterString).X + 10.0f;
            }
            LetterPositions = LetterPositions_Backup;
        }

        /// <summary>
        /// Checks whether all letters have been picked Up
        /// </summary>
        /// <returns>Boolean indicating whether all letters have been picked up.</returns>
        internal bool LettersCollected()
        {
            bool result = true;
            foreach (Letter letter in Letters)
            {
                result = result && letter.pickedUp;
            }
            return result;
        }

        /// <summary>
        /// Determines whether a rectangle collides with the walls of the maze.
        /// </summary>
        /// <param name="spriteTransformMatrix">World transform of the sprite to check collision with.</param>
        /// <param name="spriteWidth">Width of the sprite's texture.</param>
        /// <param name="spriteHeight">Height of the sprite's texture.</param>
        /// <param name="spriteColorData">Pixel color data of the sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        internal bool HitsMazeWall(
                            Matrix spriteTransformMatrix, int spriteWidth, int spriteHeight, Color[] spriteColorData)
        {


            // Transform the sprite's information to right coordinate system (currently just a standard Unity matrix)
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, spriteTransformMatrix);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, spriteTransformMatrix);

            // Calculate the entire transform Matrix to the right coordinate system.
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, spriteTransformMatrix);

            // For each row of pixels in A
            for (int yA = 0; yA < spriteHeight; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < spriteWidth; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < ActualWidth && 0 <= yB && yB < ActualHeight)
                    {

                        // Get the colors of the overlapping pixels
                        Color colorA = spriteColorData[xA + yA * spriteWidth];
                        Color colorB = MazeInfo[xB + yB * (int)ActualWidth];

                        // If the sprite's pixel is not on a non-collision indicating color,
                        if (colorA.A != 0 && !(colorB.Equals(Color.White) || colorB.Equals(Color.Red) || colorB.Equals(Color.Blue) || colorB.Equals(Color.Green)))
                        {
                            // then an intersection has been found
                            return true;
                        }

                    }
                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
        #endregion
        #region Drawing
        /// <summary>
        /// Draws the entire maze along with all its assets.
        /// </summary>
        internal void Draw()
        {
            // Calculate the scaling of the background image (for zooming and placement purposes)
            float XScale = (game.ScreenWidth / ActualWidth);
            float YScale = (game.ScreenHeight / ActualHeight);

            // First we'll draw the Magnied part.
            game.device.SetRenderTarget(MagnifierTarget);

            // Get a clean screen first.
            game.device.Clear(Color.DarkSlateBlue);//ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            SpriteBatch spriteBatch = game.spriteBatch;
            spriteBatch.Begin();
            // Draw the part to be zoomed into in full resolution.
            spriteBatch.Draw(Background, new Rectangle(0, 0, (int)WindowWidth, (int)WindowHeight), new Rectangle((int)(player.Position.X - (WindowWidth / 2.0f)), (int)(player.Position.Y - (WindowHeight / 2.0f)), (int)WindowWidth, (int)WindowHeight), Color.White);
            spriteBatch.End();

            // Render to screen.
            game.device.SetRenderTarget(null);

            // Process the zoomed in part to a new texture that has the circle form of the magnifier.
            
            // Generate a texture from the renderTarget
            Texture2D ZoomTexture = (Texture2D)MagnifierTarget;
            Color[] ZoomTextureData = new Color[(int)(WindowHeight * WindowWidth)];

            // Get the colorData from the texture.
            ZoomTexture.GetData<Color>(ZoomTextureData);

            // Make the parts that fall into a black area of the MagnifierEffect image transparent
            for (int i = 0; i < ZoomTextureData.Length; i++)
            {
                if (MagnifierEffect[i].R.Equals(Color.Black.R))
                {
                    ZoomTextureData[i] = Color.Transparent;
                }
            }
            // Save the edited texture.
            ZoomTexture.SetData<Color>(ZoomTextureData);

            // Clear the screen once more.
            game.device.Clear(Color.DarkSlateBlue);//ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            spriteBatch.Begin();

            // Draw background (scaled) first.
            spriteBatch.Draw(Background, new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight), Color.White);

            // Draw the zoom effect at the right position.
            spriteBatch.Draw(ZoomTexture, new Rectangle((int)(player.Position.X*XScale - (WindowWidth / 2.0f)), (int)(player.Position.Y*YScale - (WindowHeight / 2.0f)), (int)WindowWidth, (int)WindowHeight), Color.White);

            // Draw the player.
            player.Draw(spriteBatch, XScale, YScale, 0.0f);

            // Draw all antibodies
            foreach (Antibody antibody in antibodies)
                antibody.Draw(spriteBatch, XScale, YScale, WindowWidth/2.0f);

            // Draw the magnifier lens.
            game.spriteBatch.Draw(Magnifier, new Rectangle((int)(player.Position.X * XScale), (int)(player.Position.Y * YScale), Magnifier.Width, Magnifier.Height), null, Color.White, 0.0f, new Vector2(Magnifier.Width - (WindowHeight/2.0f) - 35, WindowHeight/2.0f + 35), SpriteEffects.None, 1);

            // Draw all letters
            foreach (Letter letter in Letters)
                letter.Draw(spriteBatch, XScale, YScale, WindowWidth / 2.0f);

            // Draw the virus
            Virus.Draw(spriteBatch, XScale, YScale, WindowWidth / 2.0f);

            game.spriteBatch.End();
        }

        #endregion

        
    }
}
