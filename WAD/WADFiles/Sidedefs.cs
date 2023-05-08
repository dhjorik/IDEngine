using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles
{
    public class MSidedef : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private short _XOffset = 0;
        private short _YOffset = 0;
        private string _UpperTX = "";
        private string _LowerTX = "";
        private string _MiddleTX = "";
        private short _Sector = 0;

        public MSidedef(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _XOffset = _Reader.ToInt16(_Start);
            _YOffset = _Reader.ToInt16(_Start + 2);
            _UpperTX = _Reader.ToString(_Start + 4, 8);
            _LowerTX = _Reader.ToString(_Start + 12, 8);
            _MiddleTX = _Reader.ToString(_Start + 20, 8);
            _Sector = _Reader.ToInt16(_Start + 28);
        }

        public static uint LSize => sizeof(short) + sizeof(short) + 8 + 8 + 8 + sizeof(short);

        public short XOffset { get => _XOffset; }
        public short YOffset { get => _YOffset; }
        public string UpperTX { get => _UpperTX; }
        public string LowerTX { get => _LowerTX; }
        public string MiddleTX { get => _MiddleTX; }
        public short Sector { get => _Sector; }
    }

    public class MSidedefs : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MSidedef> _Sides = null;

        public MSidedefs(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Sides = new List<MSidedef>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MSidedef.LSize;

#if DEBUG
            Console.Write("Sidedefs: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif

            for (uint i = 0; i < blocks; i++)
            {
                MSidedef sdd = new MSidedef(_Reader, start + offset, i);
                _Sides.Add(sdd);
                offset += MSidedef.LSize;
            }
        }

        public List<MSidedef> Sides { get => _Sides; }
    }
}
