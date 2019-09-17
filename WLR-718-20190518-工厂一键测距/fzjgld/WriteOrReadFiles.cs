using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace fzjgld
{
    class WriteOrReadFiles
    {
        #region 二进制
        //写文件
        public static void WriteDataToFile(string filedirectorypath, double[] DataToSave)
        {
            //filepath = Environment.CurrentDirectory.ToString() + @"\historydata1\" + writenum.ToString() + ".txt";
            string l_filepath = filedirectorypath + "\\" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".wj";
            FileStream fs = new FileStream(@l_filepath, FileMode.Append, FileAccess.Write);
            BinaryWriter sb = new BinaryWriter(fs, Encoding.Default);
            for (int i = 0; i < DataToSave.Length; i++)
            {
                sb.Write(DataToSave[i]);
            }
            sb.Close();
            fs.Close();
        }
        //读文件
        public static double [] Readtxt(string readfilepath,int ArrayLength)
        {
            int m = 0;
            double[] ScanArrayJiYToDraw = new double[ArrayLength]; //极坐标y轴点
            FileStream fs = File.OpenRead(readfilepath);
            BinaryReader sb = new BinaryReader(fs, Encoding.Default);
            fs.Seek(0, SeekOrigin.Begin);
            
            while (sb.PeekChar() > -1)
            {
                ScanArrayJiYToDraw[m++] = sb.ReadDouble();
            }
            sb.Close();
            fs.Close();
            return ScanArrayJiYToDraw;
        }
        #endregion

        #region 流方式读写文件
        //写文件

        public static void WriteDataToFileByStreamAnySuffix(string filepath, int[] DataToSave)
        {
            FileStream fs = new FileStream(@filepath, FileMode.Append, FileAccess.Write);     //文件名及后缀在形参filepath中定义
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            for (int i = 0; i < DataToSave.Length - 1; i++)
            {
                sw.Write(DataToSave[i]);
                sw.Write("\t");            //数据之间用5个空格隔开
                if (i % 10 == 0 && i > 0)
                {
                    sw.Write("\r\n");
                }
            }
            sw.Write(DataToSave[DataToSave.Length - 1]);
            sw.Close();
            fs.Close();
        }
        public static void WriteDataToFileByStream(string filedirectorypath, double[] DataToSave)//已验证
        {
            string filename = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".txt";   //固定形式的文件名及后缀
            string l_filepath = filedirectorypath + "\\" + filename;
            FileStream fs = new FileStream(@l_filepath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            for (int i = 0; i < DataToSave.Length - 1; i++)
            {
                sw.Write(DataToSave[i]);
                sw.Write("\t");            //数据之间用5个空格隔开
            }
            sw.Write(DataToSave[DataToSave.Length - 1]);  //避免for循环写完会在文件最后多些一个"\t"导致读数据时多读一个空格
            sw.Close();
            fs.Close();
        }

        //读文件
        //public static double[] ReadDataFromFileByStream(string readfilepath, int ArrayLength)   //此方式错误
        //{
        //    int m = 0;
        //    double[] ArrayDataReadFromFile = new double[ArrayLength]; //将文件中读取的数据放入数组中
        //    FileStream fs = File.OpenRead(readfilepath);
        //    StreamReader sr = new StreamReader(fs, Encoding.Default);
        //    fs.Seek(0, SeekOrigin.Begin);
        //    while (sr.Peek() > -1)
        //    {
        //        ArrayDataReadFromFile[m++] = sr.Read();
        //    }
        //    sr.Close();
        //    fs.Close();
        //    return ArrayDataReadFromFile;
        //}

        public static List<string> ReadDataFromFileByStreamOneLine(string readfilepath)//每次读一行   (已验证)
        {
            List<string> ListDataReadFromFile = new List<string>();//将文件中读取的数据放入List中
            FileStream fs = File.OpenRead(readfilepath);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            fs.Seek(0, SeekOrigin.Begin);
            while (sr.Peek() > -1)
            {
                string data = sr.ReadLine();  //读一行数据
                string OneSpaceData = MergeSpace(data); //将俩数据间的空格转为1个
                string[] arraydata = OneSpaceData.Split(' ');//将数据按空格区分添加到数组中
                ListDataReadFromFile.AddRange(arraydata);
            }
            sr.Close();
            fs.Close();
            return ListDataReadFromFile;

        }
        #endregion

        #region 字符串中多个连续空格转为一个空格
        /// <summary>
        /// 字符串中多个连续空格转为一个空格
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>合并空格后的字符串</returns>
        public static string MergeSpace(string str)     //已验证
        {
            if (str != string.Empty && str != null && str.Length > 0)
            {
                str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(str, " ");
            }
            return str;
        }
        #endregion
    }
}
