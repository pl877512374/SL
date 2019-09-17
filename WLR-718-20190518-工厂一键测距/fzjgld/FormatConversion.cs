using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fzjgld
{
    class FormatConversion
    {
        #region 字符串转16进制字节数组
        public static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }  

        #endregion

        #region 十进制转Char
        public static string DecimalToChar(ulong Decimal)
        {
            switch (Decimal)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return Decimal.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }

        }
        #endregion

        #region 十进制转换为16进制
        public static string DecimalToHexadecimal(int Decimal)
        {
            ulong l_u64DecimalValue = Convert.ToUInt64(Decimal);
            ulong l_u64divValue; //除数
            ulong l_u64resValue; //余数
            string hex = "";
            do
            {
                l_u64divValue = (ulong)Math.Floor((decimal)(l_u64DecimalValue / 16));
                l_u64resValue = l_u64DecimalValue % 16;
                hex = DecimalToChar(l_u64resValue) + hex;
                l_u64DecimalValue = l_u64divValue;
            }
            while (l_u64DecimalValue >= 16);
            if (l_u64DecimalValue != 0)
                hex = DecimalToChar(l_u64DecimalValue) + hex;
            return hex;
        }
        #endregion

        #region 十六进制数转换为有符号数
        public static int HexToSigned(int a)
        {
            if (a > Convert.ToInt32("7FFF", 16))//十六进制数转换为有符号数
                a = a - Convert.ToInt32("10000", 16);
            return a;
        }
        #endregion

        #region 字符串形式ip地址转换为字节数组
        public static byte[] IPtoByteArray(string ip)
        {
            string []l_astrLocalip = ip.Split('.');
            byte[] l_abyteLocalip = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                l_abyteLocalip[i] = Convert.ToByte(l_astrLocalip[i]);
            }
            return l_abyteLocalip;
        }
        #endregion
    }
}
