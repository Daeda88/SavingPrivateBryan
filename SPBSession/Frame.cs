using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SPBSession
{
    [Serializable]
    [XmlInclude (typeof(MenuFrame)), XmlInclude(typeof(MazeFrame))] 
    public class Frame
    {
        public String GameTime {get; set;}

        
        public String WordSpoken {get; set;}
    }

    [Serializable]
    public class MenuFrame : Frame{

    }

    [Serializable]
    public class MazeFrame : Frame{

        public double BCIValue {get; set;}
        public bool LeanForward {get; set;}
        public bool LeanLeft {get; set;}
        public bool LeanRight{get; set;}
        
        public PlayerInfo Player {get; set;}
        public List<AntibodyInfo> Antibodies {get; set;}
        public List<LetterInfo> Letters {get; set;}

        public MazeFrame()
        {
            Antibodies = new List<AntibodyInfo>();
            Letters = new List<LetterInfo>();
        }

    }
}
