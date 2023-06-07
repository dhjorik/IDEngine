using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Bitmaps
{
    public class MFlat : IElement
    {
        private WADReader _Reader { get; }
        private uint _Index = 0;

        private string _Name = "";
        public uint _Length = 0;
        private byte[] _Data = null;

        public MFlat(WADReader reader, uint index)
        {
            _Reader = reader;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            Entry entry = _Reader.Entries.LumpByIndex(this._Index);
            _Name = entry.Name;
            uint offset = entry.Offset;
            _Length = entry.Size;
            _Data = _Reader.ToByteArray(offset, _Length);
        }

        public uint Length { get => _Length; }
        public int Width { get => 64; }
        public int Height { get => (int)_Length / 64; }

        public byte[] Data { get => _Data; }

        public override string ToString()
        {
            return _Name;
        }
    }

    public class MFlats : IElements
    {
        private static readonly string SUFFIX_START = "_START";
        private static readonly string SUFFIX_END = "_END";

        private static readonly string PREFIX = "F";
        private static readonly List<string> COUNTERS = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        private WADReader _Reader { get; }

        private List<MFlat> _Flats = null;

        public MFlats(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            string name = "";
            _Flats = new List<MFlat>();

            name = string.Concat(PREFIX, SUFFIX_START);
            uint start_all = _Reader.Entries.LumpIndexByName(name);
            name = string.Concat(PREFIX, SUFFIX_END);
            uint end_all = _Reader.Entries.LumpIndexByName(name);

            for (uint i = start_all + 1; i < end_all; i++)
            {
                Entry entry = _Reader.Entries.LumpByIndex(i);
                if (entry.Size > 0)
                {
                    MFlat ln = new MFlat(_Reader, i);
                    _Flats.Add(ln);
                }
            }
        }

        public List<MFlat> Flats { get => _Flats; }
    }
}
