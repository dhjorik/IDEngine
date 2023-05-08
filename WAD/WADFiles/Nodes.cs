using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles
{
    public class MNode : IElement
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

        public MNode(WADReader reader, uint offset, uint index)
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

    public class MNodes : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MNode> _Nodes= null;

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
