using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using SPBSession;
using Microsoft.Xna.Framework;

namespace Saving_Private_Bryan
{
    /// <summary>
    /// Structure to store a Gameplay Session in. Used for creating an XML file after a playsession, allowing processing afterwards.
    /// </summary>
    [Serializable]
    public struct SessionData
    {
        public Session session;
    }
    
    /// <summary>
    /// Manager that stores all information of a playsession in an XML file, allowing for processing afterwards.
    /// </summary>
    class StorageManager
    {
        StorageDevice storageDevice;

        private Session session;
        private static String Extension = ".xml";

        /// <summary>
        /// Starts up the Storage Manager.
        /// </summary>
        public StorageManager()
        {
            session = new Session();
            IAsyncResult result = StorageDevice.BeginShowSelector(null, null); // Get the Storage Device.

            result.AsyncWaitHandle.WaitOne();

            storageDevice = StorageDevice.EndShowSelector(result);
        }

        /// <summary>
        /// Adds a new gameplay frame to the session manager
        /// </summary>
        /// <param name="frame">The frame to be stored in the Session.</param>
        public void AddFrame(Frame frame)
        {
            session.AddFrame(frame);
        }

        /// <summary>
        /// Saves the entire session with all the relevant information per added frame. Should only be called after exiting the game.
        /// </summary>
        public void SaveSession()
        {
            IAsyncResult result = storageDevice.BeginOpenContainer("SPB", null, null); // Get the Storage Container

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = storageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();
            String fileName;
            do // Create a new XML file with a new, randomly chosen, name.
            {
                fileName = RandomString(10) + Extension;
            } while (container.FileExists(fileName));

            Stream stream = container.CreateFile(fileName);

            SessionData sessionData = new SessionData(){session = session};

            XmlSerializer serializer = new XmlSerializer(typeof(SessionData)); // Serialize the sessionData so it can be stored.
            serializer.Serialize(stream, sessionData);
            stream.Close();
            container.Dispose();
        }

        /// <summary>
        /// Generates a random String, used for filenaming.
        /// </summary>
        /// <param name="size">Size of the String.</param>
        /// <returns>Randomly generated String.</returns>
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

    }
}
