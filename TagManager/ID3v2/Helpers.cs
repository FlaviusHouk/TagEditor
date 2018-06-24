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
    }

    public static class EncodeHelper
    {
        public static string EncodeByteArray(byte[] data)
        {
            byte flag = data[0];

            Encoding enc = null;

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

            return enc.GetString(data.Skip(flag == 1 ? 3 : 1).ToArray());
        }
    }
}
