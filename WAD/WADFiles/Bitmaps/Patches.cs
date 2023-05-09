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

        public MPatch(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _Name = _Reader.ToString(_Start,8);
        }

        public static uint LSize => 8;

        public string Name { get => _Name; }

    }

    public class MPatches : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MPatch> _Patches = null;

        public MPatches(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Patches = new List<MPatch>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 4;
            uint blocks = size / MPatch.LSize;
            uint count = _Reader.ToUInt32(_Entry.Offset);

#if DEBUG
            Console.Write("Patches: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.Write(" - ");
            Console.Write(count);
            Console.WriteLine("");
#endif
            for (uint i = 0; i < count; i++)
            {
                MPatch ln = new MPatch(_Reader, start + offset, i);
                _Patches.Add(ln);
                offset += MPatch.LSize;
            }
        }

        public List<MPatch> Patches { get => _Patches; }
    }
}
