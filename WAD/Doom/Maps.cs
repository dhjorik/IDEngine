using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using static WAD.Settings;
using WAD.Doom.Levels;
using System.Reflection.Emit;

namespace WAD.Doom
{
    public class Map : IElement
    {
        private WADReader _Reader { get; }

        private int _Level = 0;
        private int _Episode = 0;

        private MVertexes _Vertexes = null;
        private MLines _Lines = null;
        private MThings _Things = null;
        private MSidedefs _Sidedefs = null;
        private MSegs _Segs = null;
        private MSectors _Sectors = null;
        private MSubSectors _SubSectors = null;
        private MNodes _Nodes = null;

        public Map(WADReader reader, int level, int episode = 0)
        {
            this._Reader = reader;
            this._Level = level;
            this._Episode = episode;

            this.Decode();
        }

        public void Decode()
        {
            string name = Name(_Level, _Episode);
            Dictionary<EMapLumps, Entry> lumps = _Reader.Entries.MapByName(name);
            foreach (KeyValuePair<EMapLumps, Entry> item in lumps)
            {
                switch (item.Key)
                {
                    case EMapLumps.ML_THINGS:
                        {
                            this._Things = new MThings(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_VERTEXES:
                        {
                            this._Vertexes = new MVertexes(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_LINEDEFS:
                        {
                            this._Lines = new MLines(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_SIDEDEFS:
                        {
                            this._Sidedefs = new MSidedefs(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_SEGS:
                        {
                            this._Segs = new MSegs(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_SECTORS:
                        {
                            this._Sectors = new MSectors(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_SSECTORS:
                        {
                            this._SubSectors = new MSubSectors(_Reader, item.Value);
                        };
                        break;
                    case EMapLumps.ML_NODES:
                        {
                            this._Nodes = new MNodes(_Reader, item.Value);
                        };
                        break;
                }
            }
        }

        public static string Name(int level, int episode = 0)
        {
            string name = "";

            if (episode == 0)
            {
                name = string.Format("MAP{0}", level.ToString("00"));
            }
            else
            {
                name = string.Format("E{0}M{1}", episode.ToString("0"), level.ToString("0"));
            }
            return name.ToUpper();
        }

        public override string ToString()
        {
            string name = Name(_Level, _Episode);
            return name.ToUpper();
        }

        public int Level { get => _Level; }
        public int Episode { get => Episode; }

        public MVertexes Vertexes { get => _Vertexes; }
        public MLines Lines { get => _Lines; }
    }

    public class Maps : IElements
    {
        private WADReader _Reader { get; }
        private Dictionary<string, Map> _Maps { get; }

        public Maps(WADReader reader)
        {
            _Reader = reader;
            _Maps = new Dictionary<string, Map>();

            this.Decode();
        }

        public void Decode()
        {
            string maptest = Map.Name(1, 1);
            bool MapMode = _Reader.Entries.HasLumpByName(maptest);

            if (MapMode)
            {
                // Episode + Level Mode
                for (int episode = 1; episode < 10; episode++)
                {
                    for (int level = 1; level < 10; level++)
                    {
                        string name = Map.Name(level, episode);
                        bool found = _Reader.Entries.HasLumpByName(name);
                        if (found)
                        {
                            Map value = new Map(_Reader, level, episode);
                            _Maps.Add(name, value);
                        }
                    }
                }
            }
            else
            {
                // Level Mode
                for (int level = 1; level < 100; level++)
                {
                    string name = Map.Name(level, 0);
                    bool found = _Reader.Entries.HasLumpByName(name);
                    if (found)
                    {
                        Map value = new Map(_Reader, level, 0);
                        _Maps.Add(name, value);
                    }
                }
            }

        }

        public Map MapByName(string name) { return _Maps[name]; }

        public List<string> names { get { return _Maps.Keys.ToList(); } }
    }
}
