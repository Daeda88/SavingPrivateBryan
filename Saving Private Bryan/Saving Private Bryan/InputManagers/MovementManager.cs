using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Nui.Vision;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// Manager for player movement using a Kinect-device in a game of Saving Private Bryan.
    /// </summary>
    internal class MovementManager
    {
        // Kinect stuff
        NuiUserTracker _skeleton;

        // Coordinates of the bodyparts
        float headX, headY, headZ, waistX, waistY, waistZ;

        // Indicates whether the user is recognized by the usertracker
        Boolean found = false;

        // Boundary for leaning
        int LEANING_BOUNDARY = 30;

        // Boundary for movement
        int MOVEMENT_BOUNDARY = 45;

        /// <summary>
        /// Starts up the Kinect, allowing tracking player movement.
        /// </summary>
        /// <returns>A boolean indicating whether Kinect-startup was succesful.</returns>
        internal bool StartKinect()
        {
            found = false;
            try
            {// Initialize the user tracker with sample config
                _skeleton = new NuiUserTracker(@"Data\SamplesConfig.xml");

                // Add eventhandler for user detection
                //_skeleton.UserFound += new EventHandler(Skeleton_UserListUpdated);

                // Add event handler for movement detection
                _skeleton.UsersUpdated += new NuiUserTracker.UserListUpdatedHandler(Skeleton_UsersUpdated);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Function for handling losing track of a user
        /// TODO: Function does not seem to get caught when the event occurs. Problem due to third party, better Kinect Drivers required.
        /// </summary>
        /// <param name="sender">Object that sent out the event.</param>
        /// <param name="e">Arguments belonging to the event.</param>
        void Skeleton_UserLost(object sender, NuiUserEventArgs e)
        {
            Console.WriteLine("User lost: " + e.User.Id);
            found = false;
        }

        /// <summary>
        /// Resets the Kinect for recalculation.
        /// NOTE: Not currently functional
        /// </summary>
        /// <returns>Boolean indicating reset succes.</returns>
        internal bool ResetKinect()
        {
            //_skeleton.UsersUpdated -= Skeleton_UsersUpdated;
            //_skeleton.StopUserTracking();
            found = false;
            //_skeleton = new NuiUserTracker(@"Data\SamplesConfig.xml");
            //_skeleton.UsersUpdated += new NuiUserTracker.UserListUpdatedHandler(Skeleton_UsersUpdated);
            return true;
        }
        

        /// <summary>
        /// Handles the event generated when the player moves in front of the Kinect.
        /// </summary>
        /// <param name="sender">Object that sent out the event</param>
        /// <param name="e">Arguments beloning to the event</param>
        void Skeleton_UsersUpdated(object sender, NuiUserListEventArgs e)
        {
            found = true;

            // For all users, track the bodyparts. We only use Head and Waist.
            foreach (var user in e.Users)
            {

                #region Head & Waist

                headX = user.Head.X;
                headY = user.Head.Y;
                headZ = user.Head.Z;

                waistX = user.Torso.X;
                waistY = user.Torso.Y;
                waistZ = user.Torso.Z;
                #endregion

                #region Other body parts
                /*
                float neckX = user.Neck.X;
                float neckY = user.Neck.Y;

                float leftShoulderX = user.LeftShoulder.X;
                float leftShoulderY = user.LeftShoulder.Y;

                float leftElbowX = user.LeftElbow.X;
                float leftElbowY = user.LeftElbow.Y;

                float leftHandX = user.LeftHand.X;
                float leftHandY = user.LeftHand.Y;

                float rightSholuderX = user.RightShoulder.X;
                float rightShoulderY = user.RightShoulder.Y;

                float rightElbowX = user.RightElbow.X;
                float rightElbowY = user.RightElbow.Y;

                float rightHandX = user.RightHand.X;
                float rightHandY = user.RightHand.Y;
                 
                float leftHipX = user.LeftHip.X;
                float leftHipY = user.LeftHip.Y;

                float leftKneeX = user.LeftKnee.X;
                float leftKneeY = user.LeftKnee.Y;

                float leftFootX = user.LeftFoot.X;
                float leftFootY = user.LeftFoot.Y;

                float rightHipX = user.RightHip.X;
                float rightHipY = user.RightHip.Y;

                float rightKneeX = user.RightKnee.X;
                float rightKneeY = user.RightKnee.Y;

                float rightFootX = user.RightFoot.X;
                float rightFootY = user.RightFoot.Y;
                */
                #endregion
            }
        }


        /// <summary>
        /// Indicates whether the player is leaning to the left.
        /// </summary>
        /// <returns>Boolean indicating left leaning.</returns>
        internal bool MovesLeft()
        {
            // For left-rotation, track the difference between head.X and waist.X
            return (found && Math.Abs(headX - waistX) > LEANING_BOUNDARY && headX < waistX);
        }

        /// <summary>
        /// Indicates whether the player is leaning to the right.
        /// </summary>
        /// <returns>Boolean indicating right leaning.</returns>
        internal bool MovesRight()
        {
            // For right-rotation, track the difference between head.X and waist.X
            return (found && Math.Abs(headX - waistX) > LEANING_BOUNDARY && headX > waistX);
        }

        /// <summary>
        /// Indicates whether the player is leaning forward.
        /// </summary>
        /// <returns>Boolean indicating forward leaning</returns>
        internal bool MovesForward()
        {
            // For propelling, track the difference between head.Z and waist.Z (how near the user is to the camera)
            return (found && Math.Abs(headZ - waistZ) > MOVEMENT_BOUNDARY && headZ < waistZ);
        }

        /// <summary>
        /// Indicates whether the player is leaning backwards.
        /// </summary>
        /// <returns>Boolean indicating backward leaning</returns>
        internal bool MovesBackward()
        {
            // For backwards propelling, track the difference between head.Z and waist.Z (how far the user is from the camera)
            return (found && Math.Abs(headZ - waistZ) > MOVEMENT_BOUNDARY && headZ > waistZ);
        }

        /// <summary>
        /// Indicates whether a player is detected by the usertracker
        /// </summary>
        /// <returns>Boolean indicating player detection.</returns>
        internal bool PlayerDetected()
        {
            return found;
        }

    }
}
