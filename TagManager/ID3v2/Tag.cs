using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3v2
{
    public class Tag
    {
        private TagHeader _header;
        private List<Frame> _frames;

        public Tag(string path)
        {
            FileStream file = File.OpenRead(path);
            _frames = new List<Frame>();

            byte[] header = new byte[10];
            file.Read(header, 0, 10);
            _header = new TagHeader(header);

            int pos = 10;
            while (pos < _header.Size)
            {
                file.Read(header, 0, 10);
                pos += 10;
                FrameHeader head = new FrameHeader(header);
                if (head.Title.All(c => char.IsLetterOrDigit(c)))
                {
                    byte[] data = new byte[head.Size];
                    file.Read(data, 0, head.Size);
                    Frame frame = new Frame(head, data);
                    _frames.Add(frame);
                }

                pos += head.Size;
            }
        }

        public Tag(Stream file)
        {
            
        }

        public string Title
        {
            get
            {
                return PrintFrameData("TIT2");
            }
        }

        public string Album
        {
            get
            {
                return PrintFrameData("TALB");
            } 
        }

        public string Artist
        {
            get
            {
                return PrintFrameData("TPE1");
            }
        } 

        public string Year
        {
            get
            {
                return PrintFrameData("TYER");
            }
        }

        public string TrackNumber
        {
            get
            {
                return PrintFrameData("TRCK");
            }
        }

        public string Genre
        {
            get
            {
                return PrintFrameData("TCON");
            }
        }

        public string Copyright
        {
            get
            {
                return PrintFrameData("TCOP");
            }
        }

        /*public byte[] Image
        {
            get
            {

            }
        }*/

        private string PrintFrameData(string name)
        {
            Frame cur = _frames.FirstOrDefault(f => string.Compare(f.Header.Title, name, StringComparison.OrdinalIgnoreCase) == 0);

            return cur == null ? string.Empty : EncodeHelper.EncodeByteArray(cur.Data);
        }
    }
}
