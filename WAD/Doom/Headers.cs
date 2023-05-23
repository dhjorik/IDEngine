using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WAD.Doom
{
    public class Header : IElements
    {
        private WADReader _Reader { get; }

        private string _ID = "";
        private uint _Entries = 0;
        private uint _Directories = 0;

        public Header(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            _ID = _Reader.ToString(0, 4);
            _Entries = _Reader.ToUInt32(4);
            _Directories = _Reader.ToUInt32(8);
        }

        public static uint LSize => 4 + sizeof(uint) + sizeof(uint);

        public override string ToString()
        {
            List<string> log = new List<string>
            {
                "ID: " + _ID,
                "Entries number: " + _Entries.ToString(),
                "Directory index: " + _Directories.ToString()
            };

            return string.Join(Environment.NewLine, log.ToArray());
        }

        /// <summary>
        /// File ID. Must be an ASCII string (either "IWAD" or "PWAD").
        /// </summary>
        public string ID { get => _ID; }

        /// <summary>
        /// The number entries in the directory.
        /// </summary>
        public uint Entries { get => _Entries; }

        /// <summary>
        /// Offset in bytes to the directory in the WAD file.
        /// </summary>
        public uint Directories { get => _Directories; }
    }
}
