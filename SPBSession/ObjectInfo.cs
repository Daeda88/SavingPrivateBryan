using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace SPBSession
{
    [Serializable]
    [XmlInclude (typeof(PlayerInfo)), XmlInclude(typeof(AntibodyInfo)), XmlInclude(typeof(LetterInfo))] 
    public class ObjectInfo
    {
        public Vector2 Position {get; set;}
    }

    [Serializable]
    public class PlayerInfo : ObjectInfo{

    }

    [Serializable]
    public class AntibodyInfo : ObjectInfo{

        public bool Attached { get; set; }
        public int State { get; set; }
        public String Killword { get; set; }

    }

    [Serializable]
    public class LetterInfo : ObjectInfo{

        public String Text { get; set; }
        public bool PickedUp { get; set; }
    }

    [Serializable]
    public class VirusInfo : ObjectInfo
    {

    }
}
