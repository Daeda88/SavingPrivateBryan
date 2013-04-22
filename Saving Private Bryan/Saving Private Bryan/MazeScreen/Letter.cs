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
    /// Letter inside the Maze of a Saving Private Bryan game.
    /// Extends upon MazeObject.
    /// </summary>
    internal class Letter : MazeObject
    {
        Vector2 PickedUpPosition;
        String LetterString;

        /// <summary>
        /// Indicates the Letter has been picked up.
        /// </summary>
        internal bool pickedUp;

        /// <summary>
        /// Constructs the Letter.
        /// </summary>
        /// <param name="maze">Maze the Letter is placed in</param>
        /// <param name="Position">Position for the letter inside the Maze</param>
        /// <param name="PickedUpPosition">Position on screen where the letter should be drawn after it has been picked up.</param>
        /// <param name="LetterString">String describing the actual letter represented.</param>
        public Letter(Maze maze, Vector2 Position,Vector2 PickedUpPosition, String LetterString)
        {
            this.maze = maze;
            this.Position = Position;
            this.PickedUpPosition = PickedUpPosition;
            this.LetterString = LetterString;
        }

        /// <summary>
        /// Updates the Letter for the next frame to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed during gameplay.</param>
        /// <param name="frame"> The frame that is currently drawn, used for storing playSessions for evaluation</param>
        /// <returns>An updated version of the frame-parameter, with all the relevant information of this Letter</returns>
        internal override MazeFrame Update(GameTime time, MazeFrame frame)
        {
            if (Vector2.Distance(maze.player.Position, Position) < 50) // If close we pick up the letter
                pickedUp = true;
            frame.Letters.Add(new LetterInfo() { Position = Position, PickedUp = pickedUp, Text = LetterString }); // Add Session information
            return frame;
        }

        /// <summary>
        /// Draws the Letter on the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch on which the sprite of the MazeObject should be drawn.</param>
        /// <param name="XScale">XScale of screen width/actual width of the maze, used for calculating position on screen</param>
        /// <param name="YScale">XScale of screen height/actual height of the maze, used for calculating position on screen</param>
        /// <param name="MaxDistance">Maximum distance from the center of the zoom window for which to draw this Letter.</param>
        internal override void Draw(SpriteBatch spriteBatch, float XScale, float YScale, float MaxDistance)
        {
            Vector2 letterLength = maze.LetterFont.MeasureString(LetterString);
            if (pickedUp) // If the letter has been picked up we draw at specified location
            {
                spriteBatch.DrawString(maze.LetterFont, LetterString, PickedUpPosition, Color.White);
            }
            else if (Vector2.Distance(Position, maze.player.Position) < MaxDistance - 20.0f) // Case Letter in Zoomframe
            {
                Vector2 relativePosition = Position - maze.player.Position; // Since we have a zooming window its best to draw relative to the center of the zoomPlane.
                spriteBatch.DrawString(maze.LetterFont, LetterString, new Vector2(maze.player.Position.X * XScale + relativePosition.X - (letterLength.X / 2.0f), maze.player.Position.Y * YScale + relativePosition.Y - (letterLength.Y / 2.0f)), Color.Blue);
            }
            else if (Vector2.Distance(Position, maze.player.Position) > MaxDistance + 170.0f) // Case Letter outside ZoomFrame
            {
                spriteBatch.DrawString(maze.LetterFont, LetterString, new Vector2(Position.X * XScale - (letterLength.X / 2.0f), Position.Y * YScale - (letterLength.Y / 2.0f)), Color.White);
            }
            // Blidspot not accounted for
        }
    }
}
