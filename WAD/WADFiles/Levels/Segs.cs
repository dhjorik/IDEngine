using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles.Levels
{
    public class MSeg : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private ushort _StartVertex = 0;
        private ushort _EndVertex = 0;
        private short _Angle = 0;
        private ushort _Linedef = 0;
        private short _Direction = 0;
        private short _Offset = 0;

        public MSeg(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _StartVertex = _Reader.ToUInt16(_Start);
            _EndVertex = _Reader.ToUInt16(_Start + 2);
            _Angle = _Reader.ToInt16(_Start + 4);
            _Linedef = _Reader.ToUInt16(_Start + 6);
            _Direction = _Reader.ToInt16(_Start + 8);
            _Offset = _Reader.ToInt16(_Start + 10);
        }

        public static uint LSize => sizeof(ushort) + sizeof(ushort) + sizeof(short) + sizeof(ushort) + sizeof(short) + sizeof(short);

        public ushort StartVertex { get => _StartVertex; }
        public ushort EndVertex { get => _EndVertex; }
        public short Angle { get => _Angle; }
        public ushort Linedef { get => _Linedef; }
        public short Direction { get => _Direction; }
        public short Offset { get => _Offset; }
    }

    public class MSegs : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MSeg> _Segs = null;

        public MSegs(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Segs = new List<MSeg>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MSeg.LSize;

#if DEBUG
            Console.Write("Segs: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif

            for (uint i = 0; i < blocks; i++)
            {
                MSeg sdd = new MSeg(_Reader, start + offset, i);
                _Segs.Add(sdd);
                offset += MSeg.LSize;
            }
        }

        public List<MSeg> Segs { get => _Segs; }
    }
}
