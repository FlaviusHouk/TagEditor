using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3v2
{
    public static class SizeHelper
    {
        public static int GetSize(byte[] rawData)
        {
            int toRet = 0;

            for (int i = 0; i < rawData.Length; i++)
            {
                toRet = toRet << 8;
                toRet |= rawData[i];
            }

            return toRet;
        }

        public static byte[] EncodeSize(int size)
        {
            byte[] arr = new byte[4];

            for (int i = 0; i < arr.Length; i++)
            {
                byte check = (byte)(i == 3 ? 15 : 255);
                byte part = (byte)(size >> 8 * i);
                arr[i] = (byte)(part & check);
            }

            return arr;
        }
    }

    public static class EncodeHelper
    {
        public static byte GetEncoding(byte[] data, out Encoding enc)
        {
            byte flag = data[0];

            enc = null;

            switch (flag)
            {
                case 0:
                    {
                        enc = Encoding.ASCII;
                        break;
                    }
                case 1:
                    {
                        enc = data[3] == 254 ? Encoding.BigEndianUnicode : Encoding.Unicode;
                        break;
                    }
                case 2:
                    {
                        enc = Encoding.BigEndianUnicode;
                        break;
                    }
                case 3:
                    {
                        enc = Encoding.UTF8;
                        break;
                    }
            }

            return flag;
        }

        public static string EncodeByteArray(byte[] data)
        {
            Encoding enc = null;

            byte flag = GetEncoding(data, out enc);

            return enc.GetString(data.Skip(flag == 1 ? 3 : 1).ToArray());
        }

        public static byte[] DecodeString(string value, byte[] old)
        {
            Encoding enc = null;

            GetEncoding(old, out enc);

            int encMark = (old[0] == 1 ? 3 : 1);
            int newSize = enc.GetByteCount(value) + encMark;
            
            byte[] data = enc.GetBytes(value);

            byte[] toRet = new byte[newSize];

            for (int i = 0; i < encMark; i++)
            {
                toRet[i] = old[i];
            }

            for (int i = 0; i < data.Length; i++)
            {
                toRet[i + encMark] = data[i];
            }

            return toRet;
        }
    }
}
