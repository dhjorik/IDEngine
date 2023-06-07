using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Colors
{
    public class MColorMap : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private byte[] _Colors = new byte[256];

        public MColorMap(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _Colors = new byte[256];
            uint offset = 0;
            for (uint idx = 0; idx < 256; idx++)
            {
                _Colors[idx] = _Reader.ToUInt8(_Start + offset);
                offset ++;
            }
        }

        public static uint LSize => sizeof(byte) * 256;

        public byte[] Colors { get => _Colors; }

    }

    public class MColorMaps : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MColorMap> _ColorMaps = null;

        public MColorMaps(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _ColorMaps = new List<MColorMap>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MColorMap.LSize;

            for (uint i = 0; i < blocks; i++)
            {
                MColorMap ln = new MColorMap(_Reader, start + offset, i);
                _ColorMaps.Add(ln);
                offset += MColorMap.LSize;
            }
        }

        public List<MColorMap> ColorMaps { get => _ColorMaps; }
    }
}
