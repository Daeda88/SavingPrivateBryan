using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SPBSession;
using Microsoft.Xna.Framework.Graphics;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// A Virus found in the Maze of an Saving Private Bryan game.
    /// Extends on MazeObject.
    /// </summary>
    class Virus : MazeObject
    {

        /// <summary>
        /// Constructs the Virus
        /// </summary>
        /// <param name="maze">Maze the Virus is positioned in.</param>
        /// <param name="Position">Position of the Virus in the Maze.</param>
        public Virus(Maze maze, Vector2 Position)
        {
            this.maze = maze;
            this.Position = Position;
        }

        /// <summary>
        /// Updates the Virus for the next frame to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed during gameplay.</param>
        /// <param name="frame"> The frame that is currently drawn, used for storing playSessions for evaluation</param>
        /// <returns>An updated version of the frame-parameter, with all the relevant information of this Virus</returns>
        internal override MazeFrame Update(GameTime time, MazeFrame frame)
        {
            if (Vector2.Distance(maze.player.Position, Position) < 100.0f && maze.LettersCollected() && maze.GetSpeechManager().WordWasSaid(maze.requiredWord))
                maze.resetGame(true);
            return frame;
        }

        /// <summary>
        /// Draws the Virus on the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch on which the sprite of the Virus should be drawn.</param>
        /// <param name="XScale">XScale of screen width/actual width of the maze, used for calculating position on screen</param>
        /// <param name="YScale">XScale of screen height/actual height of the maze, used for calculating position on screen</param>
        /// <param name="MaxDistance">Maximum distance from the center of the zoom window for which to draw this element. At a higher distance the Virus is drawn to smaller proportions.</param>
        internal override void Draw(SpriteBatch spriteBatch, float XScale, float YScale, float MaxDistance)
        {
            if (Vector2.Distance(Position, maze.player.Position) < MaxDistance - 20.0f) // Case in Zoomframe
            {
                Vector2 relativePosition = Position - maze.player.Position;
                spriteBatch.Draw(maze.VirusTexture, new Rectangle((int)(maze.player.Position.X * XScale + relativePosition.X), (int)(maze.player.Position.Y * YScale + relativePosition.Y), (int)(maze.VirusTexture.Width / XScale), (int)(maze.VirusTexture.Height / YScale)), null, Color.White, 0.0f, new Vector2(maze.VirusTexture.Width / 2.0f, maze.VirusTexture.Height / 2.0f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1.0f);
            }
            else if (Vector2.Distance(Position, maze.player.Position) > MaxDistance + 170.0f) // Case outside Zoomframe
            {
                spriteBatch.Draw(maze.VirusTexture, new Rectangle((int)(Position.X * XScale), (int)(Position.Y * YScale), (int)maze.VirusTexture.Width, (int)maze.VirusTexture.Height), null, Color.White, 0.0f, new Vector2(maze.VirusTexture.Width / 2.0f, maze.VirusTexture.Height / 2.0f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1.0f);
            }
            // Blindspot not drawn.
        }
    }
}
