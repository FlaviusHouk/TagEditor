using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3v2
{
    public class FrameHeader
    {
        private string _title;
        private int _size;
        private int _initialSize;
        private byte[] _flags;

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        public int IniialSize
        {
            get
            {
                return _initialSize;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
            }
        }

        public FrameHeader(byte[] rawData)
        {
            _title = Encoding.ASCII.GetString(rawData.Take(4).ToArray());
            _size = SizeHelper.GetSize(rawData.Skip(4).Take(4).ToArray());
            _initialSize = _size;
            _flags = rawData.Skip(8).ToArray();
        }

        public FrameHeader(string title, int size, byte[] flags)
        {
            _title = title;
            _size = size;
            _initialSize = _size;
            _flags = flags;
        }

        public byte[] GetRawRepresentation()
        {
            byte[] toRet = new byte[10];

            using (MemoryStream mem = new MemoryStream(toRet))
            {
                mem.Write(Encoding.ASCII.GetBytes(_title), 0, 4);
                mem.Write(SizeHelper.EncodeSize(_size), 0, 4);
                mem.Write(_flags, 0, 2);
            }

            return toRet;
        }
    }
}
