using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3v2
{
    class Frame
    {
        private FrameHeader _header;
        private byte[] _data;

        public FrameHeader Header
        {
            get
            {
                return _header;
            }
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public Frame(FrameHeader header, byte[] data)
        {
            _header = header;
            _data = data;
        }
    }
}
