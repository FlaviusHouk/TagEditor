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

        #region DataProperties

        public string Album
        {
            get
            {
                return GetFrameDataAsString("TALB");
            }
        }

        public string Tempo
        {
            get
            {
                return GetFrameDataAsString("TBPM");
            }
        }

        public string Composer
        {
            get
            {
                return GetFrameDataAsString("TCOM");
            }
        }

        public string Genre
        {
            get
            {
                return GetFrameDataAsString("TCON");
            }
        }

        public string Copyright
        {
            get
            {
                return GetFrameDataAsString("TCOP");
            }
        }

        public string RecodringDate
        {
            get
            {
                return GetFrameDataAsString("TDAT");
            }
        }

        public string PlaylistDelay
        {
            get
            {
                return GetFrameDataAsString("TDLY");
            }
        }

        public string EncodedBy
        {
            get
            {
                return GetFrameDataAsString("TENC");
            }
        }

        public string Lyrics
        {
            get
            {
                return GetFrameDataAsString("TEXT");
            }
        }

        public string FileType
        {
            get
            {
                return GetFrameDataAsString("TFLT");
            }
        }

        public string RecordingTime
        {
            get
            {
                return GetFrameDataAsString("TIME");
            }
        }

        public string ContentGroup
        {
            get
            {
                return GetFrameDataAsString("TIT1");
            }
        }

        public string Title
        {
            get
            {
                return GetFrameDataAsString("TIT2");
            }
        }

        public string SubTitle
        {
            get
            {
                return GetFrameDataAsString("TIT3");
            }
        }

        public string Key
        {
            get
            {
                return GetFrameDataAsString("TKEY");
            }
        }

        public string Language
        {
            get
            {
                return GetFrameDataAsString("TLAN");
            }
        }

        /// <summary>
        /// Song length in milisecons.
        /// </summary>
        public int Length
        {
            get
            {
                return int.Parse(GetFrameDataAsString("TLEN"));
            }
        }

        public string MediaType
        {
            get
            {
                return GetFrameDataAsString("TMED");
            }
        }

        public string OriginalName
        {
            get
            {
                return GetFrameDataAsString("TOAL");
            }
        }

        public string OriginalFileName
        {
            get
            {
                return GetFrameDataAsString("TOFN");
            }
        }

        public string OriginalLyrics
        {
            get
            {
                return GetFrameDataAsString("TOLY");
            }
        }

        public string OriginalAuthor
        {
            get
            {
                return GetFrameDataAsString("TOPE");
            }
        }

        public string OriginalReleaseYear
        {
            get
            {
                return GetFrameDataAsString("TORY");
            }
        }

        public string FileOwner
        {
            get
            {
                return GetFrameDataAsString("TOWN");
            }
        }

        public string Artist
        {
            get
            {
                return GetFrameDataAsString("TPE1");
            }
        }
        
        public string Accompaniment
        {
            get
            {
                return GetFrameDataAsString("TPE2");
            }
        } 

        public string Conductor
        {
            get
            {
                return GetFrameDataAsString("TPE3");
            }
        }

        public string RemixAuthor
        {
            get
            {
                return GetFrameDataAsString("TPE4");
            }
        }

        public string PartOfASet
        {
            get
            {
                return GetFrameDataAsString("TPOS");
            }
        }

        public string Publisher
        {
            get
            {
                return GetFrameDataAsString("TPUB");
            }
        }

        public string TrackNumber
        {
            get
            {
                return GetFrameDataAsString("TRCK");
            }
        }

        public string FullDate
        {
            get
            {
                return GetFrameDataAsString("TRDA");
            }
        }

        public string RadiostationName
        {
            get
            {
                return GetFrameDataAsString("TRSN");
            }
        }
       
        public string RadiostationOwner
        {
            get
            {
                return GetFrameDataAsString("TRSO");
            }
        }
        
        public string MediaSize
        {
            get
            {
                return GetFrameDataAsString("TSIZ");
            }
        }

        public string ISRC
       {
            get
            {
                return GetFrameDataAsString("TSRC");
            }
        }

        public string HardwareSettings
        {
            get
            {
                return GetFrameDataAsString("TSSE");
            }
        }

        public string Year
        {
            get
            {
                return GetFrameDataAsString("TYER");
            }
        }

        public byte[] Image
        {
            get
            {
                return _frames.FirstOrDefault(f => string.Compare(f.Header.Title, "APIC", StringComparison.OrdinalIgnoreCase) == 0)?.Data;
            }
        }
        #endregion

        private string GetFrameDataAsString(string name)
        {
            Frame cur = _frames.FirstOrDefault(f => string.Compare(f.Header.Title, name, StringComparison.OrdinalIgnoreCase) == 0);

            return cur == null ? string.Empty : EncodeHelper.EncodeByteArray(cur.Data);
        }
    }
}
