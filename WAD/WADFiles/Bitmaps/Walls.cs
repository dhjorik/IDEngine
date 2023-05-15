using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles.Bitmaps
{
    public class MWall : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private string _Name = "";

        public MWall(WADReader reader, uint offset, uint index)
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

    public class MWalls : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MWall> _Walls = null;

        public MWalls(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Walls = new List<MWall>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 4;
            uint blocks = size / MWall.LSize;
            uint count = _Reader.ToUInt32(_Entry.Offset);

#if DEBUG
            Console.Write("Walls: ");
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
                MWall ln = new MWall(_Reader, start + offset, i);
                _Walls.Add(ln);
                offset += MWall.LSize;
            }
        }

        public List<MWall> Walls { get => _Walls; }
    }
}
