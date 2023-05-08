using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles
{
    public interface IReader
    {

    }

    public interface IElement
    {
        void Decode();
    }

    public interface IElements
    {
        void Decode();
    }

    public interface IItem
    {
        void Init(IReader reader, uint offset, uint index);
        void Decode();
        uint LSize();
    }

    public class ListOf<Titem, Sitem>
        where Titem : IItem, new()
        where Sitem : struct
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<Titem> _Items = null;

        public ListOf(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }

        public void Decode() { 
            _Items = new List<Titem>();

            string name = "";
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = 0;

            while(offset<size)
            {
                Titem sdd = new Titem();
                name = sdd.GetType().Name;
                sdd.Init(_Reader, start + offset, blocks);
                sdd.Decode();
                _Items.Add(sdd);
                blocks++;
                offset += sdd.LSize();
            }

#if DEBUG
            Console.Write(": " + name);
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.WriteLine("");
#endif
        }

        public List<Titem> Items { get => _Items; }
    }
}
