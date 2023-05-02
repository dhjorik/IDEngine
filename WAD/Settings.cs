using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD
{
    public static class Settings
    {
        public enum EMapLumps : int
        {
            ML_LABEL = 0,
            ML_THINGS = 1,
            ML_LINEDEFS = 2,
            ML_SIDEDEFS = 3,
            ML_VERTEXES = 4,
            ML_SEGS = 5,
            ML_SSECTORS = 6,
            ML_NODES = 7,
            ML_SECTORS = 8,
            ML_REJECT = 9,
            ML_BLOCKMAP = 10,
            ML_COUNT
        };

        public enum EMapLumpsEx : int
        {
            ML_BEHAVIOR = 11,
            ML_COUNT
        };

        public static string[] MapNames = {
            "NAME",
            "THINGS",
            "LINEDEFS",
            "SIDEDEFS",
            "VERTEXES",
            "SEGS",
            "SSECTORS",
            "NODES",
            "SECTORS",
            "REJECT",
            "BLOCKMAP"
        };

        public static string[] MapNamesEx = {
            "BEHAVIOR"
        };

        internal const string tableSequence = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }
}
