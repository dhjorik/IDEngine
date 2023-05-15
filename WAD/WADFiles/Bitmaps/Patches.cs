using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles.Bitmaps
{
    public class MPatch : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private string _Name = "";
        private MPicture _Picture = null;

        public MPatch(WADReader reader, uint index)
        {
            _Reader = reader;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            Entry entry = _Reader.Entries.LumpByIndex(this._Index);
            _Name = entry.Name;
            _Picture = new MPicture(_Reader, _Name);
        }

        public string Name { get { return _Name; } }
        public MPicture Picture { get { return _Picture; } }
        
        public override string ToString()
        {
            return _Name;
        }
    }

    public class MPatches : IElements
    {
        private static readonly string SUFFIX_START = "_START";
        private static readonly string SUFFIX_END = "_END";

        private static readonly string PREFIX = "P";

        private WADReader _Reader { get; }

        private List<MPatch> _Patches = null;

        public MPatches(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            string name = "";
            Entry entry = null;
            _Patches = new List<MPatch>();

            name = string.Concat(PREFIX, SUFFIX_START);
            uint start_all = _Reader.Entries.LumpIndexByName(name);
            name = string.Concat(PREFIX, SUFFIX_END);
            uint end_all = _Reader.Entries.LumpIndexByName(name);

#if DEBUG
            Console.Write("Patches: ");
            Console.Write(start_all);
            Console.Write(" - ");
            Console.Write(end_all);
            Console.WriteLine("");
#endif

            for (uint i = start_all + 1; i < end_all; i++)
            {
                entry = _Reader.Entries.LumpByIndex(i);
                if (entry.Size > 0)
                {
                    MPatch ln = new MPatch(_Reader, i);
                    _Patches.Add(ln);
                }
            }
        }

        public List<MPatch> Patches { get => _Patches; }

        public MPatch PatchByName(string name)
        {
            MPatch value = _Patches.Find(x => x.Name == name);
            return value;
        }
    }
}
