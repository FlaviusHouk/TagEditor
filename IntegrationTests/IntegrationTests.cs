using NUnit.Framework;
using System;
using System.IO;

using ID3v2;

namespace IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private const string _libPath = "D://Теги";

        [Test]
        [TestCase("0.mp3")]
        public void CheckTag(string fileName)
        {
            string currentFile = Path.Combine(_libPath, fileName);
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
        }
    }
}
