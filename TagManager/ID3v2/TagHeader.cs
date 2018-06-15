using System;
using System.Collections.Generic;
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
        private double _size;

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

        public double Size
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
    }
}
