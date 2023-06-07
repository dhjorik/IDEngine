using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD
{
    public interface IReader
    {
        void Decode();
        string ToString(uint offset, int size);
        byte[] ToByteArray(uint offset, uint size);
        sbyte ToInt8(uint offset);
        byte ToUInt8(uint offset);
        Int16 ToInt16(uint offset);
        UInt16 ToUInt16(uint offset);
        Int32 ToInt32(uint offset);
        UInt32 ToUInt32(uint offset);
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

        public void Decode()
        {
            _Items = new List<Titem>();

            string name = "";
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = 0;

            while (offset < size)
            {
                Titem sdd = new Titem();
                name = sdd.GetType().Name;
                sdd.Init(_Reader, start + offset, blocks);
                sdd.Decode();
                _Items.Add(sdd);
                blocks++;
                offset += sdd.LSize();
            }
        }

        public List<Titem> Items { get => _Items; }
    }
}
