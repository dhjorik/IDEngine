using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles.Bitmaps
{
    public class MPart : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private string _Name = "";

        public MPart(WADReader reader, uint index)
        {
            _Reader = reader;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            Entry entry = _Reader.Entries.LumpByIndex(this._Index);
            _Name = entry.Name;
        }

        public override string ToString()
        {
            return _Name;
        }
    }

    public class MParts : IElements
    {
        private static readonly string SUFFIX_START = "_START";
        private static readonly string SUFFIX_END = "_END";

        private static readonly string PREFIX = "P";

        private WADReader _Reader { get; }

        private List<MPart> _PatchParts = null;

        public MParts(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            string name = "";
            _PatchParts = new List<MPart>();

            name = string.Concat(PREFIX, SUFFIX_START);
            uint start_all = _Reader.Entries.LumpIndexByName(name);
            name = string.Concat(PREFIX, SUFFIX_END);
            uint end_all = _Reader.Entries.LumpIndexByName(name);

#if DEBUG
            Console.Write("Patches Parts: ");
            Console.Write(start_all);
            Console.Write(" - ");
            Console.Write(end_all);
            Console.WriteLine("");
#endif

            for (uint i = start_all + 1; i < end_all; i++)
            {
                Entry entry = _Reader.Entries.LumpByIndex(i);
                if (entry.Size > 0)
                {
                    MPart ln = new MPart(_Reader, i);
                    _PatchParts.Add(ln);
                }
            }
        }

        public List<MPart> PatchParts { get => _PatchParts; }
    }
}
