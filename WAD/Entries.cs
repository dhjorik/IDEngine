using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static WAD.Settings;

namespace WAD
{
    public class Entry : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private uint _Offset = 0;
        private uint _Size = 0;
        private string _Name = "";


        public Entry(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _Offset = _Reader.ToUInt32(_Start);
            _Size = _Reader.ToUInt32(_Start + 4);
            _Name = _Reader.ToString(_Start + 8, 8);
        }

        public static uint LSize => sizeof(uint) + sizeof(uint) + 8;

        public override string ToString()
        {
            List<string> log = new List<string>
            {
                "Name: " + _Name,
                "Size: " + _Size.ToString(),
                "Index: " + _Index.ToString(),
                "Offset: " + _Offset.ToString("X")
            };

            return string.Join(" - ", log.ToArray());
        }

        public uint Index { get => _Index; }

        /// <summary>
        /// Offset value to the start of the lump data in the WAD file.
        /// </summary>
        public uint Offset { get => _Offset; }

        /// <summary>
        /// The size of the lump in bytes.
        /// </summary>
        public uint Size { get => _Size; }

        /// <summary>
        /// ASCII holding the name of the lump.
        /// </summary>
        public string Name { get => _Name; }
    }

    public class Entries : IElements
    {
        private WADReader _Reader { get; }
        private List<Entry> _Entries = null;

        public Entries(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            _Entries = new List<Entry>();
            uint entrypoint = _Reader.Header.Directories;
            uint entrynumber = _Reader.Header.Entries;

            for (uint i = 0; i < entrynumber; i++)
            {
                uint offset = i * Entry.LSize;
                Entry entry = new Entry(_Reader, entrypoint + offset, i);
                _Entries.Add(entry);
            }
        }

        public bool HasLumpByName(string name)
        {
            int value = _Entries.FindIndex(x => x.Name == name);
            return value >= 0;
        }

        public bool HasLumpByIndex(int index)
        {
            int value = _Entries.FindIndex(x => x.Index == index);
            return value >= 0;
        }

        public Entry LumpByName(string name)
        {
            Entry value = _Entries.Find(x => x.Name == name);
            return value;
        }

        public Entry LumpByIndex(uint index)
        {
            Entry value = _Entries.Find(x => x.Index == index);
            return value;
        }

        public Dictionary<EMapLumps, Entry> MapByName(string name)
        {
            int count = (int)Settings.EMapLumps.ML_COUNT;
            Dictionary<EMapLumps, Entry> ret_val = new Dictionary<EMapLumps, Entry>();

            Entry value = _Entries.Find(x => x.Name == name);
            uint index = value.Index;
            for (uint i = 1; i < count; i++)
            {
                Entry found = this.LumpByIndex(i + index);
                ret_val.Add((EMapLumps)i, found);
            }
            return ret_val;
        }

        public List<Entry> ListEntries { get => _Entries; }

        public List<string> ListToStrings()
        {
            List<string> log = new List<string>();

            foreach (Entry entry in _Entries)
            {
                log.Add(entry.ToString());
            }
            return log;
        }
    }
}
