using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPBSession;
using Microsoft.Xna.Framework.Audio;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// The Player in a Maze in a game of Saving Private Bryan.
    /// Extends upon MazeObject.
    /// </summary>
    class Player : MazeObject
    {
        
        internal float rotation;

        float RotationSpeed = 0.025f;

        private static float MAX_SPEED = 1.0f;

        float Speed;

        Texture2D playerTexture, shieldTexture, exhaustTexture;
        SoundEffectInstance instance;

        /// <summary>
        /// Indicates whether the player is currently moving.
        /// </summary>
        internal bool moving { get; private set; }

        /// <summary>
        /// Indicates whether the player has died
        /// </summary>
        internal bool died;
        float collisionSize;

        /// <summary>
        /// Constructor for the player.
        /// </summary>
        /// <param name="maze">Maze the Player is located in.</param>
        /// <param name="playerTexture">Texture of the player.</param>
        /// <param name="shieldTexture">Texture of the shield of the player.</param>
        /// <param name="exhaustTexture">Texture for the exhaust.</param>
        public Player(Maze maze, Texture2D playerTexture, Texture2D shieldTexture, Texture2D exhaustTexture)
        {
            this.maze = maze;
            this.playerTexture = playerTexture;
            this.shieldTexture = shieldTexture;
            this.exhaustTexture = exhaustTexture;
            instance = maze.PlayerMovingSound.CreateInstance();
            collisionSize = Math.Max(playerTexture.Width, playerTexture.Height);
        }

        /// <summary>
        /// Resets the player to a given Position
        /// </summary>
        /// <param name="startPosition">Position the player should be set to.</param>
        internal void resetPlayer(Vector2 startPosition)
        {
            Position = startPosition;
            Speed = MAX_SPEED;
            rotation = 0.0f;
            died = false;
        }

        /// <summary>
        /// Lowers the speed of the player.
        /// </summary>
        internal void LowerSpeed()
        {
            if (Speed > 0.0f)
                Speed -= MAX_SPEED/20.0f;
        }

        /// <summary>
        /// Raises the speed of the player.
        /// </summary>
        internal void RaiseSpeed()
        {
            if (Speed < MAX_SPEED)
                Speed += MAX_SPEED/20.0f;
        }

        /// <summary>
        /// Updates the Player for the next frame to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed during gameplay.</param>
        /// <param name="frame"> The frame that is currently drawn, used for storing playSessions for evaluation</param>
        /// <returns>An updated version of the frame-parameter, with all the relevant information of this Player</returns>
        internal override MazeFrame Update(GameTime time, SPBSession.MazeFrame frame)
        {
            Vector2 oldPosition = Position;
            // Rotate the ship.
            if (maze.GetMovementManager().MovesLeft())
                rotation -= RotationSpeed;
            if (maze.GetMovementManager().MovesRight())
                rotation += RotationSpeed;

            moving = false;
            // Move in the direction the player is facing. Try moving at maximum speed, but move less if collision occurs.
            for (float i = Speed; i > 0 && maze.GetMovementManager().MovesForward() && !moving; i -= 0.2f)
            {
                
                moving = true;
                // Update position
                Position.X += (float)Math.Sin((double)rotation) * i;
                Position.Y -= (float)Math.Cos((double)rotation) * i;

                // Correct position to maze boundaries.
                if (Position.X < 0)
                    Position.X = 0;
                if (Position.X > maze.ActualWidth)
                    Position.X = maze.ActualWidth;
                if (Position.Y < 0)
                    Position.Y = 0;
                if (Position.Y > maze.ActualHeight)
                    Position.Y = maze.ActualHeight;

                // Determine collisiong
                float collisionSize = Math.Max(playerTexture.Width, playerTexture.Height);

                Matrix playerTransformMatrix =
                        Matrix.CreateTranslation(new Vector3(-collisionSize / 2.0f, -collisionSize / 2.0f, 0.0f)) *
                        Matrix.CreateTranslation(new Vector3(Position, 0.0f));

                // If collision occurs,
                if (maze.HitsMazeWall(playerTransformMatrix, (int)collisionSize, (int)collisionSize, maze.PlayerCollisionSphere))
                {
                    Position = oldPosition; // set player to previous position.
                    moving = false;
                }

                // Play the movement sound.
                if (moving)
                {

                    instance.Play();
                }
                else
                {
                    instance.Stop();
                }

            }
            // Update the frame.
            frame.Player = new PlayerInfo() { Position = Position };
            return frame;
            
        }

        /// <summary>
        /// Draws the Player on the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch on which the sprite of the MazeObject should be drawn.</param>
        /// <param name="XScale">XScale of screen width/actual width of the maze, used for calculating position on screen</param>
        /// <param name="YScale">XScale of screen height/actual height of the maze, used for calculating position on screen</param>
        /// <param name="MaxDistance">Maximum distance from the center of the zoom window for which to draw this element</param>
        internal override void Draw(SpriteBatch spriteBatch, float XScale, float YScale, float MaxDistance)
        {
            float OriginX = playerTexture.Width / 2.0f;
            float OriginY = playerTexture.Height / 2.0f;

            // We only Draw if Kinect has detected the player, so the player knows he's been detected.
            if (maze.GetMovementManager().PlayerDetected())
            {
                // Draw the shield if the player is attempting to concentrate
                if (maze.GetBCIManager().IsConcentrating())
                {
                    // Shield gets more or less transparent based on the strength of the concentration.
                    float transparency = 1.0f - (((MathHelper.Clamp(maze.GetBCIManager().ConcentrationLevel, BCIManager.CONCENTRATION_TRESHOLD, 1.0f) - BCIManager.CONCENTRATION_TRESHOLD) / (1.0f - BCIManager.CONCENTRATION_TRESHOLD)));
                    spriteBatch.Draw(shieldTexture,
                                        new Rectangle((int)(Position.X * XScale), (int)(Position.Y * YScale), shieldTexture.Width, shieldTexture.Height),
                                        null,
                                        new Color(1.0f, 1.0f, 1.0f, transparency),
                                        0.0f,
                                        new Vector2(shieldTexture.Width / 2.0f, shieldTexture.Height / 2.0f),
                                        SpriteEffects.None,
                                        0);
                }

                // Draw the actual ship.
                spriteBatch.Draw(playerTexture,
                                       new Rectangle((int)(Position.X * XScale), (int)(Position.Y * YScale), playerTexture.Width, playerTexture.Height),
                                       null,
                                       Color.White,
                                       rotation,
                                       new Vector2(OriginX, OriginY),
                                       SpriteEffects.None,
                                       0);
                // Draw exhausfumes if the player is moving.
                if (moving)
                {
                    Vector2 flameOffset = new Vector2(-(float)Math.Sin((double)rotation), (float)Math.Cos((double)rotation));
                    flameOffset.Normalize();
                    flameOffset *= playerTexture.Height+5;
                    spriteBatch.Draw(exhaustTexture,
                                           new Rectangle((int)((Position.X + flameOffset.X)* XScale), (int)((Position.Y  + flameOffset.Y) * YScale), playerTexture.Width, playerTexture.Height),
                                           null,
                                           Color.White,
                                           rotation,
                                           new Vector2(exhaustTexture.Width/2.0f, exhaustTexture.Height/2.0f),
                                           SpriteEffects.None,
                                           0);
                }
            }
        }
    }
}
