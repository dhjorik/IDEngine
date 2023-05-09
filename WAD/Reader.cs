using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WAD.WADFiles;
using WAD.WADFiles.Colors;

namespace WAD
{
    public class WADReader : IElements, IReader
    {
        public string WAD_File { get; set; }
        public byte[] WAD_buffer { get; set; }

        private Header _Header = null;
        private Entries _Entries = null;
        private Maps _Maps = null;
        private Palettes _Palettes = null;
        private Images _Images = null;

        public WADReader(string wad_path)
        {
            if (File.Exists(wad_path))
            {
                long size = new FileInfo(wad_path).Length;
                this.WAD_buffer = new byte[size];
                this.WAD_File = wad_path;

                this.Decode();
            }
            else
            {
                this.WAD_buffer = new byte[0];
                this.WAD_File = null;
            }
        }

        public Header Header { get => _Header; }
        public Entries Entries { get => _Entries; }
        public Maps Maps { get => _Maps; }
        public Palettes Palettes { get => _Palettes; }
        public Images Images { get => _Images; }

        private void Read_Header()
        {
            _Header = new Header(this);
        }

        private void Read_Entries()
        {
            _Entries = new Entries(this);
        }

        private void Read_Maps()
        {
            _Maps = new Maps(this);
        }

        private void Read_Colors()
        {
            _Palettes = new Palettes(this);
        }

        private void Read_Images()
        {
            _Images = new Images(this);
        }

        private void Bufferize()
        {
            if (this.WAD_File == null) return;
            using (Stream source = File.OpenRead(this.WAD_File))
            {
                int start = 0;
                int counts = 0;
                int block_size = (int)Math.Pow(2, 16);
                int buffer_size = this.WAD_buffer.Length;

                int remains = buffer_size - start;
                int block_read = block_size > remains ? remains : block_size;

                byte[] bytes = new byte[block_read];
                int readed = source.Read(bytes, start, block_read);

#if DEBUG
                Console.Write("Start: ");
                Console.Write(start);
                Console.Write(" - ");
                Console.Write(remains);
                Console.Write(" - ");
                Console.Write(block_read);
                Console.Write(" - ");
                Console.WriteLine(readed);
#endif

                while (readed > 0)
                {
                    bytes.CopyTo(this.WAD_buffer, start);
                    counts++;
                    start += block_read;
                    remains = buffer_size - start;
                    block_read = block_size > remains ? remains : block_size;
                    if (block_read != bytes.Length)
                    {
                        bytes = new byte[block_read];
                    }
                    readed = source.Read(bytes, 0, block_read);

#if DEBUG
                    Console.Write("Iter {0}: ", counts);
                    Console.Write(start);
                    Console.Write(" - ");
                    Console.Write(remains);
                    Console.Write(" - ");
                    Console.Write(block_read);
                    Console.Write(" - ");
                    Console.WriteLine(readed);
#endif

                }
            }
        }

        public void Decode()
        {
            this.Bufferize();
            this.Read_Header();
            this.Read_Entries();
            this.Read_Maps();
            this.Read_Colors();
            this.Read_Images();
        }

        public string ToString(uint offset, int size)
        {
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            string str_val = Encoding.UTF8.GetString(buf);
            string ret_val = str_val;
            if (str_val.IndexOf("\0") >= 0)
            {
                ret_val = str_val.Substring(0, str_val.IndexOf("\0"));
            }
            return ret_val;
        }

        public sbyte ToInt8(uint offset)
        {
            int size = 1;
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            sbyte ret_val = (sbyte)buf[0];
            return ret_val;
        }

        public byte ToUInt8(uint offset)
        {
            int size = 1;
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            byte ret_val = (byte)buf[0];
            return ret_val;
        }

        public Int16 ToInt16(uint offset)
        {
            int size = 2;
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            Int16 ret_val = BitConverter.ToInt16(buf, 0);
            return ret_val;
        }

        public UInt16 ToUInt16(uint offset)
        {
            int size = 2;
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            UInt16 ret_val = BitConverter.ToUInt16(buf, 0);
            return ret_val;
        }

        public int ToInt32(uint offset)
        {
            int size = 4;
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            int ret_val = BitConverter.ToInt32(buf, 0);
            return ret_val;
        }

        public uint ToUInt32(uint offset)
        {
            int size = 4;
            byte[] buf = new byte[size];
            Array.Copy(this.WAD_buffer, offset, buf, 0, size);
            uint ret_val = BitConverter.ToUInt32(buf, 0);
            return ret_val;
        }
    }
}
