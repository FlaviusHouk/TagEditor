using System;
using NUnit.Framework;
using System.Text;

namespace UnitTests
{
    [TestFixture]
    public class HelperMethodsTests
    {
        [Test]
        [TestCase(new byte[] { 0, 0, 0, 4 })]
        [TestCase(new byte[] { 255, 21, 21})]
        [TestCase(new byte[] { 0, 3, 116, 127 })]
        [TestCase(new byte[] { 255, 255, 255, 255 })]
        [TestCase(new byte[] { 0, 0, 0, 255})]
        public void GetSizeTest(byte[] rawData)
        {
            uint rezult = (uint)ID3v2.SizeHelper.GetSize(rawData);
            TagLib.ByteVector vec = new TagLib.ByteVector(rawData, rawData.Length);
            uint expected = TagLib.Id3v2.SynchData.ToUInt(vec);
  
            Assert.AreEqual(expected, rezult);
        }

        [Test]
        [TestCase(22)]
        [TestCase(20000)]
        [TestCase(244666)]
        [TestCase(65534)]
        public void EncodeSizeTest(int size)
        {
            byte[] rezult = ID3v2.SizeHelper.EncodeSize(size);
            TagLib.ByteVector vec = TagLib.Id3v2.SynchData.FromUInt((uint)size);

            Assert.AreEqual(vec.Data, rezult);
        }

        [Test]
        [TestCase(new byte[] { 0, 48, 49, 50 }, "012")]
        [TestCase(new byte[] { 0, 72, 101, 108, 108, 111, 32, 087, 111, 114, 108, 100, 33}, "Hello World!")]
        [TestCase(new byte[] { 1, byte.MaxValue, byte.MaxValue, 0, 72, 0, 101, 0, 108, 0, 108, 0, 111, 0, 32, 0, 87, 0, 111, 0, 114, 0, 108, 0, 100, 0, 33 }, "Hello World!")]
        [TestCase(new byte[] { 1, byte.MaxValue, byte.MaxValue - 1, 72, 0, 101, 0, 108, 0, 108, 0, 111, 0, 32, 0, 87, 0, 111, 0, 114, 0, 108, 0, 100, 0, 33, 0 }, "Hello World!")]
        public void EncodeByteArrayTest(byte[] data, string expected)
        {
            string encoded = ID3v2.EncodeHelper.EncodeByteArray(data);

            Assert.AreEqual(expected, encoded);
        }

        [Test]
        [TestCase("Hello World!", new byte[] { 0 }, new byte[] { 0, 72, 101, 108, 108, 111, 32, 087, 111, 114, 108, 100, 33 })]
        [TestCase("Hello World!", new byte[] { 1, byte.MaxValue, byte.MaxValue }, new byte[] { 1, byte.MaxValue, byte.MaxValue, 0, 72, 0, 101, 0, 108, 0, 108, 0, 111, 0, 32, 0, 87, 0, 111, 0, 114, 0, 108, 0, 100, 0, 33 })]
        [TestCase("Hello World!", new byte[] { 1, byte.MaxValue, byte.MaxValue - 1 }, new byte[] { 1, byte.MaxValue, byte.MaxValue - 1    
            , 0, 72, 0, 101, 0, 108, 0, 108, 0, 111, 0, 32, 0, 87, 0, 111, 0, 114, 0, 108, 0, 100, 0, 33 })]
        public void DecodeByteArrayTest(string data, byte[] header, byte[] expected)
        {
            byte[] decoded = ID3v2.EncodeHelper.DecodeString(data, header);

            Assert.AreEqual(expected, decoded);
        }
    }
}
