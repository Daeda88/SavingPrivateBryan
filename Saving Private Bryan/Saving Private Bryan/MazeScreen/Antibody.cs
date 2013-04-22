using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPBSession;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// An Antibody in the Maze of a game of Saving Private Bryan.
    /// Extends upon MazeObject.
    /// </summary>
    internal class Antibody : MazeObject
    {
        int stage;
        bool attached;

        float Speed = .8f;
        float relativeAngle;

        int timeSinceLevelRaise;
        double MillisecondsTimer;
        double requiredConcentration;
        static double CONCENTRATION_TIME_PER_STATE = 1.5;

        String killWord;

        /// <summary>
        /// Constructs the Antibody
        /// </summary>
        /// <param name="maze">Maze the Antibody is placed in.</param>
        /// <param name="Position">Starting position of the Antibody.</param>
        /// <param name="killWord">Word required to be spoken to kill Antibody.</param>
        /// <param name="timer">Time in Milliseconds between stages of Antibody.</param>
        public Antibody(Maze maze, Vector2 Position, String killWord, double timer)
        {
            this.maze = maze;
            this.Position = Position;
            stage = 0;
            this.killWord = killWord;
            this.MillisecondsTimer = timer;
            attached = false;

            // We should not draw the Antibody on a place where it collides with the Wall.
            Matrix abTransformMatrix =
                        Matrix.CreateTranslation(new Vector3(-maze.antibodyTextures[stage].Width / 2.0f, -maze.antibodyTextures[stage].Height / 2.0f, 0.0f)) *
                        Matrix.CreateTranslation(new Vector3(Position, 0.0f));

            // Remove if collision takes place from startup.
            if (maze.HitsMazeWall(abTransformMatrix, (int)maze.antibodyTextures[stage].Width, (int)maze.antibodyTextures[stage].Height, maze.ABCollisionData))
            {
                maze.toDelete.Add(this);
            }
        }

        /// <summary>
        /// Lowers the stage of the Antibody.
        /// If the Antibody was already at the lowest stage, it will be deleted from the Maze.
        /// </summary>
        public void LowerStage()
        {
            if (stage > 0)
                stage--;
            else
                maze.toDelete.Add(this);
            maze.player.RaiseSpeed();
            requiredConcentration += TimeSpan.FromSeconds(CONCENTRATION_TIME_PER_STATE).TotalMilliseconds; // We need to wait another Required concentration time to lower to the previous stage, else lowering the stage once will most likely kill the antibody immediately. See update.
        }

        /// <summary>
        /// Raises the stage of an Antibody if it has not reached the maximum stage yet.
        /// </summary>
        public void RaiseStage()
        {
            if (stage < maze.antibodyTextures.Length - 1)
                stage++;
            else
                maze.player.died = true;
            maze.player.LowerSpeed();
        }

        /// <summary>
        /// Updates the Antibody for the next frame to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed during gameplay.</param>
        /// <param name="frame"> The frame that is currently drawn, used for storing playSessions for evaluation</param>
        /// <returns>An updated version of the frame-parameter, with all the relevant information of this Antibody</returns>
        internal override MazeFrame Update(GameTime time, MazeFrame frame)
        {
            // Case Attached Antibody
            if (attached)
            {
                timeSinceLevelRaise += time.ElapsedGameTime.Milliseconds; // Timer for next level raise.
                if (timeSinceLevelRaise > MillisecondsTimer) // Required time since last Level Raise has passed.
                {
                    RaiseStage(); // Raise level.
                    timeSinceLevelRaise = 0;
                }
                // If the player has only been concentrated for a short while we reset the required concentration time.
                // This is due to the fact that, after lowering a stage, the player has to concentrate even longer to lower another stage.
                // But since concentration can be lost, this could be a big punishment for players.
                // As such, we only require the minimum concentration time every time the player has lost concentration.
                if (!maze.GetBCIManager().IsConcentratedFor(TimeSpan.FromSeconds(0.1).TotalMilliseconds))
                    requiredConcentration = TimeSpan.FromSeconds(CONCENTRATION_TIME_PER_STATE).TotalMilliseconds;
                // If concentrated long enough the stage should be lowered.
                if (maze.GetBCIManager().IsConcentratedFor(requiredConcentration))
                    LowerStage();
            }
            // Case the antibody is too far from the player or the killword was said.
            else if (Vector2.Distance(Position, maze.player.Position) > 400.0f || (Vector2.Distance(Position, maze.player.Position) < 150.0f && maze.GetSpeechManager().WordWasSaid(killWord)))
            {
                maze.toDelete.Add(this); // Delete the antibody.
            }
            else // Case antibody movement
            {
                Vector2 movementDirection = Vector2.Normalize(maze.player.Position - Position); // Move in the direction of the player.
                Vector2 oldPosition = Position;
                bool moving = false;
                for (float i = Speed; i > 0 && !moving; i -= 0.2f) // Move by speed, unless collision occurs which results in slower movement.
                {

                    moving = true;
                    Position.X += movementDirection.X * i; // Move in the direction at the given speed
                    Position.Y += movementDirection.Y * i;

                    // In case of collision reset the movement.
                    Matrix abTransformMatrix =
                            Matrix.CreateTranslation(new Vector3(-maze.antibodyTextures[stage].Width / 2.0f, -maze.antibodyTextures[stage].Height / 2.0f, 0.0f)) *
                            Matrix.CreateTranslation(new Vector3(Position, 0.0f));

                    if (maze.HitsMazeWall(abTransformMatrix, (int)maze.antibodyTextures[stage].Width, (int)maze.antibodyTextures[stage].Height, maze.ABCollisionData))
                    {
                        Position = oldPosition; // Reset movement
                    }
                }
                // Antibody should become attached due to short distance to player.
                if (Vector2.Distance(Position, maze.player.Position) < 20)
                {
                    attached = true;
                    Position = Position - maze.player.Position; // Calculate relative position to the player and store position as such.
                    relativeAngle = (float)Math.Atan2(Position.X, -Position.Y) - maze.player.rotation; // Calculate the relative angle to the player ship. Used for rotating along.
                    maze.player.LowerSpeed();
                    timeSinceLevelRaise = 0;
                    requiredConcentration = TimeSpan.FromSeconds(CONCENTRATION_TIME_PER_STATE).TotalMilliseconds; // Set required concentration time.
                }
            }
            // Give frame all relevant information.
            frame.Antibodies.Add(new AntibodyInfo() { Position = Position, Attached = attached, Killword = killWord, State = stage });
            return frame;
        }

        /// <summary>
        /// Draws the Antibody on the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch on which the sprite of the MazeObject should be drawn.</param>
        /// <param name="XScale">XScale of screen width/actual width of the maze, used for calculating position on screen</param>
        /// <param name="YScale">XScale of screen height/actual height of the maze, used for calculating position on screen</param>
        /// <param name="MaxDistance">Maximum distance from the center of the zoom window for which to draw this element</param>
        internal override void Draw(SpriteBatch spriteBatch, float XScale, float YScale, float MaxDistance)
        {
            Texture2D textureToDraw = maze.antibodyTextures[stage];
            float OriginX = textureToDraw.Width / 2.0f;
            float OriginY = textureToDraw.Height / 2.0f;

            // Case attached
            if (attached)
            {
                float rotation = relativeAngle + maze.player.rotation; // Rotate along Player center based on relative angle and player angle.
                float distance = Position.Length(); // Position is stored as relative to the player. Calculate range from center to rotate along.
                float XDisplacement = (float)Math.Sin(rotation) * distance; // Calculate X and Y position relative to the player position based on rotation.
                float YDisplacement = -(float)Math.Cos(rotation) * distance;
                // Draw the antibody. Keep in mind that the position should be modified to the scale due to the zoom window effect.
                spriteBatch.Draw(textureToDraw, new Rectangle((int)(maze.player.Position.X * XScale + XDisplacement), (int)(maze.player.Position.Y * YScale + YDisplacement), textureToDraw.Width, textureToDraw.Height), null, Color.White, 0.0f, new Vector2(textureToDraw.Width / 2.0f, textureToDraw.Height / 2.0f), SpriteEffects.None, 1);
            }
            // Case not attached but in zoomframe.
            else if (Vector2.Distance(Position, maze.player.Position) < MaxDistance - 20.0f)
            {
                // Draw in zoomframe using position rleative to the player (due to zoomWindow scaling)
                Vector2 relativePosition = Position - maze.player.Position;
                spriteBatch.Draw(textureToDraw, new Rectangle((int)(maze.player.Position.X * XScale + relativePosition.X), (int)(maze.player.Position.Y * YScale + relativePosition.Y), textureToDraw.Width, textureToDraw.Height), null, Color.White, 0.0f, new Vector2(textureToDraw.Width / 2.0f, textureToDraw.Height / 2.0f), SpriteEffects.None, 1);
                spriteBatch.DrawString(maze.abFont, killWord, new Vector2(maze.player.Position.X * XScale + relativePosition.X - maze.abFont.MeasureString(killWord).X / 2.0f, maze.player.Position.Y * YScale + relativePosition.Y + textureToDraw.Height / 3.0f), Color.White);
            }
            // Dont draw if outside zoomFrame.
        }
    }
}
