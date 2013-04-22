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
    /// Abstract class decribint all possible element to appear within a Maze in a Saving Private Bryan game.
    /// </summary>
    public abstract class MazeObject
    {
        /// <summary>
        /// The maze the MazeObject is placed in.
        /// </summary>
        internal Maze maze;

        /// <summary>
        /// The position of the MazeObject within the maze.
        /// </summary>
        internal Vector2 Position;

        /// <summary>
        /// Updates the MazeObject for the next frame to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed during gameplay.</param>
        /// <param name="frame"> The frame that is currently drawn, used for storing playSessions for evaluation</param>
        /// <returns>An updated version of the frame-parameter, with all the relevant information of this MazeObject</returns>
        internal abstract MazeFrame Update(GameTime time, MazeFrame frame);

        /// <summary>
        /// Draws the MazeObject on the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch on which the sprite of the MazeObject should be drawn.</param>
        /// <param name="XScale">XScale of screen width/actual width of the maze, used for calculating position on screen</param>
        /// <param name="YScale">XScale of screen height/actual height of the maze, used for calculating position on screen</param>
        /// <param name="MaxDistance">Maximum distance from the center of the zoom window for which to draw this element</param>
        internal abstract void Draw(SpriteBatch spriteBatch, float XScale, float YScale, float MaxDistance);

    }
}
