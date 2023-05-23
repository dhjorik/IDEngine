using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.Doom.Levels;

namespace WAD.Doom.Sounds
{
    public class MSample : IElement
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }

        private ushort _Format = 0;
        private ushort _Rate = 0;
        private uint _Length = 0;

        private byte[] _Pad1 = null;
        private byte[] _Pad2 = null;

        private byte[] _Samples = null;

        public MSample(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            _Format = _Reader.ToUInt16(_Entry.Offset);
            _Rate = _Reader.ToUInt16(_Entry.Offset + 2);
            _Length = _Reader.ToUInt32(_Entry.Offset + 4);
            _Pad1 = _Reader.ToByteArray(_Entry.Offset + 8, 16);
            _Samples = _Reader.ToByteArray(_Entry.Offset + 24, _Length);
            _Pad1 = _Reader.ToByteArray(_Entry.Offset + 24 + _Length, 16);
        }

        public ushort Format { get { return _Format; } }
        public ushort Rate { get { return _Rate; } }
        public uint Length { get { return _Length; } }
        public byte[] Samples { get { return _Samples; } }

        public override string ToString()
        {
            string name = _Entry.Name;
            return name.ToUpper();
        }
    }

    public class MSamples : IElements
    {
        private WADReader _Reader { get; }
        private List<MSample> _Samples = null;

        public MSamples(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Samples = new List<MSample>();
            foreach (string sname in Infos.SFXNames.Values)
            {
                string lumpo_name = "DS" + sname.ToUpper();
                if (_Reader.Entries.HasLumpByName(lumpo_name))
                {
                    Entry entry = _Reader.Entries.LumpByName(lumpo_name);
                    MSample mSample = new MSample(_Reader, entry);
                    _Samples.Add(mSample);
                    Console.Write(lumpo_name);
                    Console.WriteLine(" found");
                }
            }
        }

        public List<MSample> Samples { get { return _Samples; } }
    }
}
