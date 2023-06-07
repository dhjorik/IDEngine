using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Levels
{
    public class MSubSector : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private short _Count = 0;
        private short _Head = 0;

        public MSubSector(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _Count = _Reader.ToInt16(_Start);
            _Head = _Reader.ToInt16(_Start + 2);
        }

        public static uint LSize => sizeof(short) + sizeof(short);

        public short Count { get => _Count; }
        public short Head { get => _Head; }
    }

    public class MSubSectors : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MSubSector> _SubSectors = null;

        public MSubSectors(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _SubSectors = new List<MSubSector>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MSubSector.LSize;

            for (uint i = 0; i < blocks; i++)
            {
                MSubSector ln = new MSubSector(_Reader, start + offset, i);
                _SubSectors.Add(ln);
                offset += MSubSector.LSize;
            }
        }

        public List<MSubSector> SubSectors { get => _SubSectors; }
    }
}
