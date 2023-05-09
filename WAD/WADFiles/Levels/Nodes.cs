using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles.Levels
{
    public class MNode : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private short _StartX = 0;
        private short _StartY = 0;
        private short _DeltaX = 0;
        private short _DeltaY = 0;
        private short[] _RightBB = new short[4];
        private short[] _LeftBB = new short[4];
        private ushort _RightChild = 0;
        private ushort _LeftChild = 0;

        public MNode(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _StartX = _Reader.ToInt16(_Start);
            _StartY = _Reader.ToInt16(_Start + 2);
            _DeltaX = _Reader.ToInt16(_Start + 4);
            _DeltaY = _Reader.ToInt16(_Start + 6);

            _RightBB[0] = _Reader.ToInt16(_Start + 8);
            _RightBB[1] = _Reader.ToInt16(_Start + 10);
            _RightBB[2] = _Reader.ToInt16(_Start + 12);
            _RightBB[3] = _Reader.ToInt16(_Start + 14);

            _LeftBB[0] = _Reader.ToInt16(_Start + 16);
            _LeftBB[1] = _Reader.ToInt16(_Start + 18);
            _LeftBB[2] = _Reader.ToInt16(_Start + 20);
            _LeftBB[3] = _Reader.ToInt16(_Start + 22);

            _RightChild = _Reader.ToUInt16(_Start + 24);
            _LeftChild = _Reader.ToUInt16(_Start + 26);
        }

        public static uint LSize => sizeof(short) + sizeof(short) + sizeof(short) + sizeof(short) + sizeof(short) * 4
            + sizeof(short) * 4 + sizeof(ushort) + sizeof(ushort);

        public short StartX { get => _StartX; }
        public short StartY { get => _StartY; }
        public short DeltaX { get => _DeltaX; }
        public short DeltaY { get => _DeltaY; }
        public short[] RightBB { get => _RightBB; }
        public short[] LeftBB { get => _LeftBB; }
        public ushort RightChild { get => _RightChild; }
        public ushort LeftChild { get => _LeftChild; }
    }

    public class MNodes : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MNode> _Nodes = null;

        public MNodes(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Nodes = new List<MNode>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MNode.LSize;

#if DEBUG
            Console.Write("Nodes: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif

            for (uint i = 0; i < blocks; i++)
            {
                MNode node = new MNode(_Reader, start + offset, i);
                _Nodes.Add(node);
                offset += MNode.LSize;
            }
        }

        public List<MNode> Nodes { get => _Nodes; }
    }
}
