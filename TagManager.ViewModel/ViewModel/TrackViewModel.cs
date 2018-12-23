using ReactiveUI;
using ID3v2;
using System;
using System.Linq;

namespace TagManager.ViewModel
{
    public class TrackViewModel : ReactiveObject
    {
        #region Fields
        private Tag _trackTag;
        private string _name;
        private string _holder;
        private int _id;

        private string _title;
        private string _composer;
        private string _album;
        private string _tempo;
        private string _genre;
        private string _copyright;
        private string _recDate;
        private string _playlistDelay;
        private string _lyrics;
        private string _recTime;
        private string _key;
        private string _lang;
        private int _length;
        private string _artist;
        private string _publisher;
        private string _trackNumber;
        private string _fullDate;
        private string _mediaSize;
        private string _ISRC;
        private string _subTitle;
        private string _year;

        private byte[] _image;
        private string _path;

        #endregion

        #region Properties

        public string Path
        {
            get { return _path;}
            private set { _path = value; }
        }

        public int ID
        {
            get { return _id; }
            set
            {
                this.RaiseAndSetIfChanged(ref _id, value);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }


        public string Holder
        {
            get { return _holder; }
            set
            {
                this.RaiseAndSetIfChanged(ref _holder, value);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                this.RaiseAndSetIfChanged(ref _title, value);
            }
        }

        public string Composer
        {
            get { return _composer; }
            set
            {
                this.RaiseAndSetIfChanged(ref _composer, value);
            }
        }

        public string Album
        {
            get { return _album; }
            set
            {
                this.RaiseAndSetIfChanged(ref _album, value);
            }
        }

        public string Tempo
        {
            get { return _tempo; }
            set
            {
                this.RaiseAndSetIfChanged(ref _tempo, value);
            }
        }

        public string Genre
        {
            get { return _genre; }
            set
            {
                this.RaiseAndSetIfChanged(ref _genre, value);
            }
        }

        public string Copyright
        {
            get { return _copyright; }
            set
            {
                this.RaiseAndSetIfChanged(ref _copyright, value);
            }
        }

        public string RecDate
        {
            get { return _recDate; }
            set
            {
                this.RaiseAndSetIfChanged(ref _recDate, value);
            }
        }

        public string PlaylistDelay
        {
            get { return _playlistDelay; }
            set
            {
                this.RaiseAndSetIfChanged(ref _playlistDelay, value);
            }
        }

        public string Lyrics
        {
            get { return _lyrics; }
            set
            {
                this.RaiseAndSetIfChanged(ref _lyrics, value);
            }
        }

        public string RecTime
        {
            get { return _recTime; }
            set
            {
                this.RaiseAndSetIfChanged(ref _recTime, value);
            }
        }

        public string Key
        {
            get { return _key; }
            set
            {
                this.RaiseAndSetIfChanged(ref _key, value);
            }
        }

        public string Lang
        {
            get { return _lang; }
            set
            {
                this.RaiseAndSetIfChanged(ref _lang, value);
            }
        }

        public int Length
        {
            get { return _length; }
            set
            {
                this.RaiseAndSetIfChanged(ref _length, value);
            }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                this.RaiseAndSetIfChanged(ref _artist, value);
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                this.RaiseAndSetIfChanged(ref _publisher, value);
            }
        }

        public string TrackNumber
        {
            get { return _trackNumber; }
            set
            {
                this.RaiseAndSetIfChanged(ref _trackNumber, value);
            }
        }

        public string FullDate
        {
            get { return _fullDate; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fullDate, value);
            }
        }

        public string MediaSize
        {
            get { return _mediaSize; }
            set
            {
                this.RaiseAndSetIfChanged(ref _mediaSize, value);
            }
        }

        public string ISRC
        {
            get { return _ISRC; }
            set
            {
                this.RaiseAndSetIfChanged(ref _ISRC, value);
            }
        }

        public string Year
        {
            get { return _year; }
            set
            {
                this.RaiseAndSetIfChanged(ref _year, value);
            }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set
            {
                this.RaiseAndSetIfChanged(ref _subTitle, value);
            }
        }

        public byte[] Image
        {
            get
            {
                return _image;
            }
        }
        #endregion

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _isSelected, value);
            }
        }

        public TrackViewModel(string path, string folder, int id)
        {
            _path = path;
            var parts = path.Split('\\');
            _name = parts.LastOrDefault();

            var s = folder.Split('\\');
            var ss = s.Skip(s.Length -2);
            _holder = $"...\\{ss.FirstOrDefault()}\\{ss.LastOrDefault()}";

            _id = id;
            _trackTag = new Tag(path);
            InitData(_trackTag);
        }

        public TrackViewModel()
        {
        }

        private void InitData(Tag trackTag)
        {
            _title = trackTag.Title;
            _composer = trackTag.Composer;
            _album = trackTag.Album;
            _tempo = trackTag.Tempo;
            _genre = trackTag.Genre;
            _copyright = trackTag.Copyright;
            _recDate = trackTag.RecodringDate;
            _playlistDelay = trackTag.RecodringDate;
            _lyrics = trackTag.Lyrics;
            _recDate = trackTag.RecodringDate;
            _key = trackTag.Key;
            _lang = trackTag.Language;
            _length = trackTag.Length;
            _artist = trackTag.Artist;
            _subTitle = trackTag.SubTitle;
            _publisher = trackTag.Publisher;
            _trackNumber = trackTag.TrackNumber;
            _fullDate = trackTag.FullDate;
            _mediaSize = trackTag.MediaSize;
            _ISRC = trackTag.ISRC;
            _year = trackTag.Year;
            _image = trackTag.Image;
        }

        public void SaveTag()
        {
            _trackTag.Title = _title;
            _trackTag.Artist = _artist;
            _trackTag.Year = _year;
            _trackTag.ISRC = _ISRC;
            _trackTag.Album = _album;
            _trackTag.SaveTag();
        }
    }
}