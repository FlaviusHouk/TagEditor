using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3v2
{
    public class FrameHeader
    {
        private string _title;
        private int _size;
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
            _flags = rawData.Skip(8).ToArray();
        }
    }
}
