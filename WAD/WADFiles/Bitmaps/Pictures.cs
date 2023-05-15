using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WAD.WADFiles.Bitmaps
{
    public class MColumn : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private byte _TopDelta = 0;
        private byte _Length = 0;

        private byte[] _Data = null;
        private byte[] _Unused = new byte[2];

        public MColumn(WADReader reader, uint start, uint index)
        {
            _Reader = reader;
            _Start = start;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            uint offset = _Start;
            _TopDelta = _Reader.ToUInt8(offset);
            _Length = _Reader.ToUInt8(offset + 1);
            _Unused[0] = _Reader.ToUInt8(offset + 2);
            _Data = _Reader.ToByteArray(offset + 3, _Length);
            _Unused[1] = _Reader.ToUInt8(offset + 3 + _Length);
        }

        public uint TopDelta { get => (uint)_TopDelta; }
        public uint Length { get => (uint)_Length; }

        public byte[] Data { get => _Data; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Column: ");
            sb.Append(_Index);
            sb.Append(" - ");
            sb.Append(_TopDelta);
            sb.Append(" - ");
            sb.Append(_Length);
            sb.Append(" - [");
            foreach (byte b in _Data)
            {
                sb.Append(b);
                sb.Append(",");
            }
            sb.AppendLine("]");

            return sb.ToString();
        }
    }

    public class MPicture : IElements
    {
        private WADReader _Reader { get; }
        private string _Name { get; }

        private ushort _Width = 0;
        private ushort _Height = 0;

        private short _OffsetX = 0;
        private short _OffsetY = 0;

        private List<MColumn> _Columns = null;

        public MPicture(WADReader reader, string name)
        {
            _Reader = reader;
            _Name = name;

            this.Decode();
        }

        public void Decode()
        {
            Entry entry = _Reader.Entries.LumpByName(_Name);

            if (entry != null)
            {
                uint offset = entry.Offset;
                _Width = _Reader.ToUInt16(offset);
                _Height = _Reader.ToUInt16(offset + 2);
                _OffsetX = _Reader.ToInt16(offset + 4);
                _OffsetY = _Reader.ToInt16(offset + 6);

                _Columns = new List<MColumn>();
                for (uint i = 0; i < _Width; i++)
                {
                    uint ofs = _Reader.ToUInt32(offset + 8 + i * 4);
                    uint delta = 0;
                    uint top = _Reader.ToUInt8(offset + ofs + delta);
                    while(top != 0xff){
                        MColumn column = new MColumn(_Reader, offset + ofs, i);
                        _Columns.Add(column);

                        delta += column.Length + 4;
                        top = _Reader.ToUInt8(offset + ofs + delta);
                    }
                }
            }
        }

        public uint Length { get => (uint)_Width * (uint)_Height; }
        public ushort Width { get => _Width; }
        public ushort Height { get => _Height; }
        public short OffsetX { get => _OffsetX; }
        public short OffsetY { get => _OffsetY; }

        public List<MColumn> Columns { get => _Columns; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Picture: ");
            sb.Append(_Name);
            sb.Append(" - ");
            sb.Append(_Width);
            sb.Append(" - ");
            sb.Append(_Height);
            sb.Append(" - ");
            sb.Append(_OffsetX);
            sb.Append(" - ");
            sb.Append(_OffsetY);
            sb.AppendLine("");

            foreach (MColumn column in _Columns)
            {
                sb.Append(column.ToString());
            }
            return sb.ToString();
        }
    }
}
