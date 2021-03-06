﻿using GalaSoft.MvvmLight;
using ID3v2;
using System;
using System.Linq;

namespace TagManager.ViewModel
{
    public class TrackViewModel : ObservableObject
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
                _id = value;
                RaisePropertyChanged(nameof(ID));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }


        public string Holder
        {
            get { return _holder; }
            set
            {
                _holder = value;
                RaisePropertyChanged(nameof(Holder));
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Composer
        {
            get { return _composer; }
            set
            {
                _composer = value;
                RaisePropertyChanged(nameof(Composer));
            }
        }

        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                RaisePropertyChanged(nameof(Album));
            }
        }

        public string Tempo
        {
            get { return _tempo; }
            set
            {
                _tempo = value;
                RaisePropertyChanged(nameof(Tempo));
            }
        }

        public string Genre
        {
            get { return _genre; }
            set
            {
                _genre = value;
                RaisePropertyChanged(nameof(Genre));
            }
        }

        public string Copyright
        {
            get { return _copyright; }
            set
            {
                _copyright = value;
                RaisePropertyChanged(nameof(Copyright));
            }
        }

        public string RecDate
        {
            get { return _recDate; }
            set
            {
                _recDate = value;
                RaisePropertyChanged(nameof(RecDate));
            }
        }

        public string PlaylistDelay
        {
            get { return _playlistDelay; }
            set
            {
                _playlistDelay = value;
                RaisePropertyChanged(nameof(PlaylistDelay));
            }
        }

        public string Lyrics
        {
            get { return _lyrics; }
            set
            {
                _lyrics = value;
                RaisePropertyChanged(nameof(Lyrics));
            }
        }

        public string RecTime
        {
            get { return _recTime; }
            set
            {
                _recTime = value;
                RaisePropertyChanged(nameof(RecTime));
            }
        }

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                RaisePropertyChanged(nameof(Key));
            }
        }

        public string Lang
        {
            get { return _lang; }
            set
            {
                _lang = value;
                RaisePropertyChanged(nameof(Lang));
            }
        }

        public int Length
        {
            get { return _length; }
            set
            {
                _length = value;
                RaisePropertyChanged(nameof(Length));
            }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                RaisePropertyChanged(nameof(Artist));
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                RaisePropertyChanged(nameof(Publisher));
            }
        }

        public string TrackNumber
        {
            get { return _trackNumber; }
            set
            {
                _trackNumber = value;
                RaisePropertyChanged(nameof(TrackNumber));
            }
        }

        public string FullDate
        {
            get { return _fullDate; }
            set
            {
                _fullDate = value;
                RaisePropertyChanged(nameof(FullDate));
            }
        }

        public string MediaSize
        {
            get { return _mediaSize; }
            set
            {
                _mediaSize = value;
                RaisePropertyChanged(nameof(MediaSize));
            }
        }

        public string ISRC
        {
            get { return _ISRC; }
            set
            {
                _ISRC = value;
                RaisePropertyChanged(nameof(ISRC));
            }
        }

        public string Year
        {
            get { return _year; }
            set
            {
                _year = value;
                RaisePropertyChanged(nameof(Year));
            }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set
            {
                _subTitle = value;
                RaisePropertyChanged(nameof(SubTitle));
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