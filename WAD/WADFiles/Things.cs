using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles
{
    public class MThing : IElements
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private short _X = 0;
        private short _Y = 0;
        private short _Angle = 0;
        private short _Type = 0;
        private short _Flags = 0;

        public MThing(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _X = _Reader.ToInt16(_Start);
            _Y = _Reader.ToInt16(_Start + 2);
            _Angle = _Reader.ToInt16(_Start + 4);
            _Type = _Reader.ToInt16(_Start + 6);
            _Flags = _Reader.ToInt16(_Start + 8);
        }

        public static uint LSize => sizeof(short) + sizeof(short) + sizeof(short) + sizeof(short) + sizeof(short);

        public short X { get => _X; }
        public short Y { get => _Y; }
        public short Angle { get => _Angle; }
        public short Type { get => _Type; }
        public short Flags { get => _Flags; }
    }

    public class MThings : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MThing> _Things = null;

        public MThings(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Things = new List<MThing>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MThing.LSize;

#if DEBUG
            Console.Write("Things: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif

            for (uint i = 0; i < blocks; i++)
            {
                MThing ln = new MThing(_Reader, start + offset, i);
                _Things.Add(ln);
                offset += MThing.LSize;
            }
        }

        public List<MThing> Things { get => _Things; }
    }
}
