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
                return GetFrameDataAsString("TALB").Trim('\0');
            }
        }

        public string Tempo
        {
            get
            {
                return GetFrameDataAsString("TBPM").Trim('\0');
            }
        }

        public string Composer
        {
            get
            {
                return GetFrameDataAsString("TCOM").Trim('\0');
            }
        }

        public string Genre
        {
            get
            {
                return GetFrameDataAsString("TCON").Trim('\0');
            }
        }

        public string Copyright
        {
            get
            {
                return GetFrameDataAsString("TCOP").Trim('\0');
            }
        }

        public string RecodringDate
        {
            get
            {
                return GetFrameDataAsString("TDAT").Trim('\0');
            }
        }

        public string PlaylistDelay
        {
            get
            {
                return GetFrameDataAsString("TDLY").Trim('\0');
            }
        }

        public string EncodedBy
        {
            get
            {
                return GetFrameDataAsString("TENC").Trim('\0');
            }
        }

        public string Lyrics
        {
            get
            {
                return GetFrameDataAsString("TEXT").Trim('\0');
            }
        }

        public string FileType
        {
            get
            {
                return GetFrameDataAsString("TFLT").Trim('\0');
            }
        }

        public string RecordingTime
        {
            get
            {
                return GetFrameDataAsString("TIME").Trim('\0');
            }
        }

        public string ContentGroup
        {
            get
            {
                return GetFrameDataAsString("TIT1").Trim('\0');
            }
        }

        public string Title
        {
            get
            {
                return GetFrameDataAsString("TIT2").Trim('\0');
            }
        }

        public string SubTitle
        {
            get
            {
                return GetFrameDataAsString("TIT3").Trim('\0');
            }
        }

        public string Key
        {
            get
            {
                return GetFrameDataAsString("TKEY").Trim('\0');
            }
        }

        public string Language
        {
            get
            {
                return GetFrameDataAsString("TLAN").Trim('\0');
            }
        }

        /// <summary>
        /// Song length in milisecons.
        /// </summary>
        public int Length
        {
            get
            {
                string retVal = GetFrameDataAsString("TLEN").Trim('\0');
                int toRet;

                return int.TryParse(retVal, out toRet) ? toRet : 0;
            }
        }

        public string MediaType
        {
            get
            {
                return GetFrameDataAsString("TMED").Trim('\0');
            }
        }

        public string OriginalName
        {
            get
            {
                return GetFrameDataAsString("TOAL").Trim('\0');
            }
        }

        public string OriginalFileName
        {
            get
            {
                return GetFrameDataAsString("TOFN").Trim('\0');
            }
        }

        public string OriginalLyrics
        {
            get
            {
                return GetFrameDataAsString("TOLY").Trim('\0');
            }
        }

        public string OriginalAuthor
        {
            get
            {
                return GetFrameDataAsString("TOPE").Trim('\0');
            }
        }

        public string OriginalReleaseYear
        {
            get
            {
                return GetFrameDataAsString("TORY").Trim('\0');
            }
        }

        public string FileOwner
        {
            get
            {
                return GetFrameDataAsString("TOWN").Trim('\0');
            }
        }

        public string Artist
        {
            get
            {
                return GetFrameDataAsString("TPE1").Trim('\0');
            }
        }
        
        public string Accompaniment
        {
            get
            {
                return GetFrameDataAsString("TPE2").Trim('\0');
            }
        } 

        public string Conductor
        {
            get
            {
                return GetFrameDataAsString("TPE3").Trim('\0');
            }
        }

        public string RemixAuthor
        {
            get
            {
                return GetFrameDataAsString("TPE4").Trim('\0');
            }
        }

        public string PartOfASet
        {
            get
            {
                return GetFrameDataAsString("TPOS").Trim('\0');
            }
        }

        public string Publisher
        {
            get
            {
                return GetFrameDataAsString("TPUB").Trim('\0');
            }
        }

        public string TrackNumber
        {
            get
            {
                return GetFrameDataAsString("TRCK").Trim('\0');
            }
        }

        public string FullDate
        {
            get
            {
                return GetFrameDataAsString("TRDA").Trim('\0');
            }
        }

        public string RadiostationName
        {
            get
            {
                return GetFrameDataAsString("TRSN").Trim('\0');
            }
        }
       
        public string RadiostationOwner
        {
            get
            {
                return GetFrameDataAsString("TRSO").Trim('\0');
            }
        }
        
        public string MediaSize
        {
            get
            {
                return GetFrameDataAsString("TSIZ").Trim('\0');
            }
        }

        public string ISRC
       {
            get
            {
                return GetFrameDataAsString("TSRC").Trim('\0');
            }
        }

        public string HardwareSettings
        {
            get
            {
                return GetFrameDataAsString("TSSE").Trim('\0');
            }
        }

        public string Year
        {
            get
            {
                return GetFrameDataAsString("TYER").Trim('\0');
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
