using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Levels
{
    public class MSector : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private short _FloorHeight = 0;
        private short _CeilHeight = 0;
        private string _FloorTX = "";
        private string _CeilTX = "";
        private short _Light = 0;
        private short _Special = 0;
        private short _Tags = 0;

        public MSector(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _FloorHeight = _Reader.ToInt16(_Start);
            _CeilHeight = _Reader.ToInt16(_Start + 2);
            _FloorTX = _Reader.ToString(_Start + 4, 8);
            _CeilTX = _Reader.ToString(_Start + 12, 8);
            _Light = _Reader.ToInt16(_Start + 20);
            _Special = _Reader.ToInt16(_Start + 22);
            _Tags = _Reader.ToInt16(_Start + 24);
        }

        public static uint LSize => sizeof(short) + sizeof(short) + 8 + 8 + sizeof(short) + sizeof(short) + sizeof(short);

        public short FloorHeight { get => _FloorHeight; }
        public short CeilHeight { get => _CeilHeight; }
        public string FloorTX { get => _FloorTX; }
        public string CeilTX { get => _CeilTX; }
        public short Light { get => _Light; }
        public short Special { get => _Special; }
        public short Tags { get => _Tags; }
    }

    public class MSectors : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MSector> _Sectors = null;

        public MSectors(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Sectors = new List<MSector>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MSector.LSize;

#if DEBUG
            Console.Write("Sectors: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif

            for (uint i = 0; i < blocks; i++)
            {
                MSector ln = new MSector(_Reader, start + offset, i);
                _Sectors.Add(ln);
                offset += MSector.LSize;
            }
        }

        public List<MSector> Sectors { get => _Sectors; }
    }
}
