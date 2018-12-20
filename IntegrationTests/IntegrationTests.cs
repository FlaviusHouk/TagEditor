using NUnit.Framework;
using System;
using System.IO;

using ID3v2;

namespace IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private const string _libPath = "..\\..\\..\\MusicLib";

        [Test]
        public void CheckTags()
        {
            string[] files = Directory.GetFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, _libPath), "*.mp3");

            foreach (string currentFile in files)
            {
                Tag tag = new Tag(currentFile);

                TagLib.Id3v2.Tag libTag = null;
                using (FileStream stream = File.Open(currentFile, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    libTag = new TagLib.Id3v2.Tag(new TagLib.ByteVector(data, data.Length));
                }

                Assert.NotNull(libTag);

                Assert.AreEqual(tag.Title, libTag.Title);
                Assert.AreEqual(tag.Accompaniment, libTag.FirstAlbumArtist);
                Assert.AreEqual(tag.Album, libTag.Album);
                Assert.AreEqual(tag.Image, (libTag.Pictures[0] as TagLib.Id3v2.AttachedPictureFrame).Data);
                Assert.AreEqual(tag.Composer, libTag.FirstComposer ?? string.Empty);
                Assert.AreEqual(tag.Conductor, libTag.Conductor ?? string.Empty);
                Assert.AreEqual(tag.ContentGroup, libTag.Grouping ?? string.Empty);
                Assert.AreEqual(tag.Copyright, libTag.Copyright ?? string.Empty);
                //Assert.AreEqual(tag.EncodedBy, libTag.)
                //Assert.AreEqual(tag.FileOwner, libTag.)
                //Assert.AreEqual(tag.FileType, libTag.);
                //Assert.AreEqual(tag.FullDate, libTag.);
                Assert.AreEqual(tag.Genre.Split(new[] { ';', '/', ',' }), libTag.Genres);
                //Assert.AreEqual(tag.HardwareSettings, libTag.);
                //Assert.AreEqual(tag.ISRC, libTag.);
                //Assert.AreEqual(tag.Key, libTag.);
                //Assert.AreEqual(tag.Language, libTag.);
                Assert.AreEqual(tag.Length, tag.Length);
                Assert.AreEqual(tag.Lyrics, tag.Lyrics ?? string.Empty);
                //Assert.AreEqual(tag.MediaSize, libTag.);
                //Assert.AreEqual(tag.MediaType, libTag.T);
                //Assert.AreEqual(tag.OriginalAuthor, libTag.);
                //Assert.AreEqual(tag.OriginalFileName, libTag.);
                //Assert.AreEqual(tag.PartOfASet, libTag.Disc); - different formats
                //Assert.AreEqual(tag.PlaylistDelay, libTag.);
                //Assert.AreEqual(tag.Publisher, libTag.Pu);
                //Assert.AreEqual(tag.RadiostationName, libTag.);
                //Assert.AreEqual(tag.RecodringDate, libTag.);
                //Assert.AreEqual(tag.SubTitle, libTag.S);
                Assert.AreEqual(string.IsNullOrEmpty(tag.Tempo) ? 0 : uint.Parse(tag.Tempo), libTag.BeatsPerMinute);
                //Assert.AreEqual(string.IsNullOrEmpty(tag.TrackNumber) ? 0 : uint.Parse(tag.TrackNumber), libTag.Track); - different formats
                Assert.AreEqual(string.IsNullOrEmpty(tag.Year) ? 0 : uint.Parse(tag.Year), libTag.Year);
            }
        }
    }
}
