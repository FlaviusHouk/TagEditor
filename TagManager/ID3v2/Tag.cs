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
        private string _filePath;

        public Tag(string path)
        {
            _filePath = path;
            using (FileStream file = File.OpenRead(path))
            {
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
        }

        public Tag(Stream file)
        {
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

        #region DataProperties

        public string Album
        {
            get
            {
                return GetTextData("TALB").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TALB", value);
            }
        }

        public string Tempo
        {
            get
            {
                return GetTextData("TBPM").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TBPM", value);
            }
        }

        public string Composer
        {
            get
            {
                return GetTextData("TCOM").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TCOM", value);
            }
        }

        public string Genre
        {
            get
            {
                return GetTextData("TCON").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TCON", value);
            }
        }

        public string Copyright
        {
            get
            {
                return GetTextData("TCOP").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TCOP", value);
            }
        }

        public string RecodringDate
        {
            get
            {
                return GetTextData("TDAT").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TDAT", value);
            }
        }

        public string PlaylistDelay
        {
            get
            {
                return GetTextData("TDLY").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TDLY", value);
            }
        }

        public string EncodedBy
        {
            get
            {
                return GetTextData("TENC").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TENC", value);
            }
        }

        public string Lyrics
        {
            get
            {
                return GetTextData("TEXT").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TEXT", value);
            }
        }

        public string FileType
        {
            get
            {
                return GetTextData("TFLT").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TFLT", value);
            }
        }

        public string RecordingTime
        {
            get
            {
                return GetTextData("TIME").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TIME", value);
            }
        }

        public string ContentGroup
        {
            get
            {
                return GetTextData("TIT1").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TIT1", value);
            }
        }

        public string Title
        {
            get
            {
                return GetTextData("TIT2").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TIT2", value);
            }
        }

        public string SubTitle
        {
            get
            {
                return GetTextData("TIT3").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TIT3", value);
            }
        }

        public string Key
        {
            get
            {
                return GetTextData("TKEY").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TKEY", value);
            }
        }

        public string Language
        {
            get
            {
                return GetTextData("TLAN").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TLAN", value);
            }
        }

        /// <summary>
        /// Song length in milisecons.
        /// </summary>
        public int Length
        {
            get
            {
                string retVal = GetTextData("TLEN").Trim('\0');
                int toRet;

                return int.TryParse(retVal, out toRet) ? toRet : 0;
            }
        }

        public string MediaType
        {
            get
            {
                return GetTextData("TMED").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TMED", value);
            }
        }

        public string OriginalName
        {
            get
            {
                return GetTextData("TOAL").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TOAL", value);
            }
        }

        public string OriginalFileName
        {
            get
            {
                return GetTextData("TOFN").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TOFN", value);
            }
        }

        public string OriginalLyrics
        {
            get
            {
                return GetTextData("TOLY").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TOLY", value);
            }
        }

        public string OriginalAuthor
        {
            get
            {
                return GetTextData("TOPE").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TOPE", value);
            }
        }

        public string OriginalReleaseYear
        {
            get
            {
                return GetTextData("TORY").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TORY", value);
            }
        }

        public string FileOwner
        {
            get
            {
                return GetTextData("TOWN").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TOWN", value);
            }
        }

        public string Artist
        {
            get
            {
                return GetTextData("TPE1").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TPE1", value);
            }
        }

        public string Accompaniment
        {
            get
            {
                return GetTextData("TPE2").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TPE2", value);
            }
        }

        public string Conductor
        {
            get
            {
                return GetTextData("TPE3").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TPE3", value);
            }
        }

        public string RemixAuthor
        {
            get
            {
                return GetTextData("TPE4").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TPE4", value);
            }
        }

        public string PartOfASet
        {
            get
            {
                return GetTextData("TPOS").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TPOS", value);
            }
        }

        public string Publisher
        {
            get
            {
                return GetTextData("TPUB").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TPUB", value);
            }
        }

        public string TrackNumber
        {
            get
            {
                return GetTextData("TRCK").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TRCK", value);
            }
        }

        public string FullDate
        {
            get
            {
                return GetTextData("TRDA").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TRDA", value);
            }
        }

        public string RadiostationName
        {
            get
            {
                return GetTextData("TRSN").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TRSN", value);
            }
        }

        public string RadiostationOwner
        {
            get
            {
                return GetTextData("TRSO").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TRSO", value);
            }
        }

        public string MediaSize
        {
            get
            {
                return GetTextData("TSIZ").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TSIZ", value);
            }
        }

        public string ISRC
        {
            get
            {
                return GetTextData("TSRC").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TSRC", value);
            }
        }

        public string HardwareSettings
        {
            get
            {
                return GetTextData("TSSE").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TSSE", value);
            }
        }

        public string Year
        {
            get
            {
                return GetTextData("TYER").Trim('\0');
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                SetTextData("TYER", value);
            }
        }

        public byte[] Image
        {
            get
            {
                byte[] data = _frames.FirstOrDefault(f => string.Compare(f.Header.Title, "APIC", StringComparison.OrdinalIgnoreCase) == 0)?.Data;
                if (data != null)
                {
                    int i = 1;
                    for (int j = 0; j < 2; j++)
                    {
                        for (; i < data.Length; i++)
                        {
                            if (data[i] == 0)
                                break;
                        }

                        i++;
                    }

                    string type = EncodeHelper.EncodeByteArray(data.Take(i).ToArray());

                    return data.Skip(i).ToArray();
                }
                return  new byte[2];
            }
        }
        #endregion

        private string GetTextData(string name)
        {
            Frame cur = _frames.FirstOrDefault(f => string.Compare(f.Header.Title, name, StringComparison.OrdinalIgnoreCase) == 0);

            return cur == null ? string.Empty : EncodeHelper.EncodeByteArray(cur.Data);
        }

        private void SetTextData(string frameName, string data)
        {
            Frame cur = _frames.FirstOrDefault(f => string.Compare(f.Header.Title, frameName, StringComparison.OrdinalIgnoreCase) == 0);

            if (cur == null)
            {
                FrameHeader newHeader = new FrameHeader(frameName, 0, new byte[2]);
                Frame toAdd = new Frame(newHeader, new byte[]{ 1, 255, 255 });
                _frames.Add(toAdd);

                cur = toAdd;
            }

            byte[] encoded = EncodeHelper.DecodeString(data, cur.Data.Take(3).ToArray());

            cur.Header.Size = encoded.Length;
            cur.Data = encoded;
        }

        public void SaveTag()
        {
            using (FileStream file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                file.Position = _header.Size;

                byte[] mp3Data = new byte[file.Length - _header.Size];

                file.Read(mp3Data, 0, mp3Data.Length);

                long delta = _frames.Select(f => f.Header.Size - f.Header.IniialSize).Sum();

                byte[] newFile = new byte[_header.Size + delta + 10 + mp3Data.Length];

                using (MemoryStream mem = new MemoryStream(newFile))
                {
                    mem.Write(_header.GetRawRepresentation(), 0, 10);

                    foreach(Frame f in _frames)
                    {
                        mem.Write(f.Header.GetRawRepresentation(), 0, 10);
                        mem.Write(f.Data, 0, f.Data.Length);
                    }

                    mem.Write(mp3Data, 0, mp3Data.Length);
                }

                file.SetLength(newFile.Length);

                file.Seek(0, SeekOrigin.Begin);

                file.Write(newFile, 0, newFile.Length);
            }
        }
    }
}