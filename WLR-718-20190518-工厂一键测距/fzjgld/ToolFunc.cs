using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows .Forms ;

namespace fzjgld
{
    class ToolFunc
    {

        #region 异或校验

        #region 扫描单点数据异或校验函数
        public static bool checkXor(byte[] buffer)
        {
            //int i = 0;
            int check = 0;
            //int len;
            if (buffer[0] == 0xFF && buffer[1] == 0xFF)    //末尾两位无需校验
            {
                check = XorCheck_byte(2, buffer, 2);
                if (check == buffer[buffer.Length - 2]) //验证一下
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            else if (buffer[0] == 0x02)
            {
                check = XorCheck_byte(8, buffer, 1);
                if (check == buffer[buffer.Length - 1]) 
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            else if (buffer[0] == 0xFF && buffer[1] == 0xAA)
            {
                check = XorCheck_byte(2, buffer, 4);
                if (check == buffer[buffer.Length - 3]) 
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        #endregion

        public static byte XorCheck_byte(int indexstart, byte[] buffer, int indexend)
        {
            int check = 0;
            for (int j = indexstart; j < buffer.Length - indexend; j++)
            {
                check ^= buffer[j];
            }
            Byte[] bytes = BitConverter.GetBytes(check);
            return bytes[0];
        }
        #endregion

        #region groupbox中控件index查询
        public static void GBIndexQuery(GroupBox groupBox)
        {
            int groupcount = groupBox.Controls.Count;
            string[] strnames = new string[groupcount];
            for (int i = 0; i < groupcount; i++)
            {
                strnames[i] = groupBox.Controls[i].Name;
            }
        }
        #endregion

        #region IP转字节数组
        public static byte[] IpToByteArray(string p_sip)
        {
            int i, j;
            byte[] l_abip = new byte[8];
            char[] l_acseparator = new char[] { '.' };
            string[] l_asitems = p_sip.Split(l_acseparator);
            j = 0;
            for (i = 0; i < 4; i++)
            {
                l_abip[j++] = Convert.ToByte(int.Parse(l_asitems[i]) >> 8 & 0xff);
                l_abip[j++] = Convert.ToByte(int.Parse(l_asitems[i]) & 0xff);
            }
            return l_abip;
        }

        #endregion

        #region MAC转字节数组
        public static byte[] MACToByteArray(string p_sMAC)
        {
            int i, j;
            byte[] l_abMAC = new byte[12];
            char[] l_acseparator = new char[] { ':' };
            string[] l_asitems = p_sMAC.Split(l_acseparator);
            j = 0;
            for (i = 0; i < 6; i++)
            {
                l_abMAC[j++] = Convert.ToByte(Convert.ToInt32(l_asitems[i], 16) >> 8 & 0xff);
                l_abMAC[j++] = Convert.ToByte(Convert.ToInt32(l_asitems[i], 16) & 0xff);
            }
            return l_abMAC;
        }

        #endregion


    }
}
