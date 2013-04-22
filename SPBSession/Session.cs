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

namespace SPBSession
{
    [Serializable]
    public class Session
    {

        public List<Frame> Frames;

        public Session()
        {
            Frames = new List<Frame>();
        }

        public void AddFrame(Frame frame)
        {
            Frames.Add(frame);
        }

    }
}
