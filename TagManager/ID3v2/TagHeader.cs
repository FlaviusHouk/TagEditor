using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3v2
{
    public class TagHeader
    {
        private string _title;
        private byte _magorVersion;
        private byte _minorVersion;
        private byte _flags;
        private long _size;

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

        public long Size
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

        public TagHeader(byte[] rawData)
        {
            _title = Encoding.ASCII.GetString(rawData.Take(3).ToArray());
            _magorVersion = rawData[3];
            _minorVersion = rawData[4];
            _flags = rawData[5];
            _size = SizeHelper.GetSize(rawData.Skip(6).ToArray());
        }

        public byte[] GetRawRepresentation()
        {
            byte[] toRet = new byte[10];

            using (MemoryStream mem = new MemoryStream(toRet))
            {
                mem.Write(Encoding.ASCII.GetBytes(_title), 0, 3);
                mem.Write(new byte[] { _magorVersion }, 0, 1);
                mem.Write(new byte[] { _minorVersion }, 0, 1);
                mem.Write(new byte[] { _flags }, 0, 1);
                mem.Write(SizeHelper.EncodeSize(Convert.ToInt32(_size)), 0, 4);
            }
            
            return toRet;
        }
    }
}
