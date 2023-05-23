using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Levels
{
    public class MLine : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private ushort _StartVertex = 0;
        private ushort _EndVertex = 0;
        private ushort _Flags = 0;
        private ushort _Action = 0;
        private ushort _Tags = 0;
        private ushort _RightSide = 0;
        private ushort _LeftSide = 0;

        public MLine(WADReader reader, uint offset, uint index)
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
            _Flags = _Reader.ToUInt16(_Start + 4);
            _Action = _Reader.ToUInt16(_Start + 6);
            _Tags = _Reader.ToUInt16(_Start + 8);
            _RightSide = _Reader.ToUInt16(_Start + 10);
            _LeftSide = _Reader.ToUInt16(_Start + 12);
        }

        public static uint LSize => sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);

        public ushort StartVertex { get => _StartVertex; }
        public ushort EndVertex { get => _EndVertex; }
        public ushort Flags { get => _Flags; }
        public ushort Action { get => _Action; }
        public ushort Tags { get => _Tags; }
        public ushort RightSide { get => _RightSide; }
        public ushort LeftSide { get => _LeftSide; }
    }

    public class MLines : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MLine> _Lines = null;

        public MLines(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Lines = new List<MLine>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MLine.LSize;

#if DEBUG
            Console.Write("Lines: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif

            for (uint i = 0; i < blocks; i++)
            {
                MLine ln = new MLine(_Reader, start + offset, i);
                _Lines.Add(ln);
                offset += MLine.LSize;
            }
        }

        public List<MLine> Lines { get => _Lines; }
    }
}
