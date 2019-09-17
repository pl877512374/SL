using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace fzjgld
{
    public partial class ScanDataSet : Form
    {
        #region 参数定义
        Form1 main = new Form1();
        struct RecordStruct
        {
            public int m_int32max;
            public int m_int32min;
            public int m_int32ave;
            public int m_int32realdisc;
            public int m_int32maxdiff;//最大值偏差：最大值减实际值
            public int m_int32mindiff;//最小值偏差：最小值减实际值
            public int m_int32avediff;//平均值偏差：平均值减实际值
            public int m_int32dispersion;//离散度：最大值减最小值
        };
         

        public int RealValue = 0;              //实际值
        ArrayList Recordlist = new ArrayList();
        string path = null;//Environment.CurrentDirectory.ToString() + @"\Excel\" + "\\";
        ExcelDocumentName ExcelFormName;
        public string  ExcelName= null;

        MillisecondTimer g_timer1;
        MillisecondTimer g_timer2;
        public int g_n32SingleFrameCmdCnt = 0;//单帧命令数

        public int g_n32ScanMax = 0;
        public int g_n32ScanMin = 0;
        public double g_n64ScanAve = 0;
        public double g_n64ScanPercent = 0;
        public int g_n32ScanCorrectPackageNum = 0;
        public int g_n32ScanDrawPackageNum = 0;
        public int g_n32AbsoluteAccuracy = 0;//绝对精度：最大值偏差的最大值减去最小值偏差的最小值
        public int g_n32MaxMax = 0;//最大值偏差的最大值；
        public int g_n32MinMin = 0;//最小值偏差的最小值
        public double g_n64Offset = 0; //偏移量；

        string NetDataFilesPath = Environment.CurrentDirectory.ToString() + @"\NetDataFiles\";
        public string NetDataFilesSonPath;
        public bool SaveScanDataToFile = false;
        public const int ScanDrawDataLength = 2700;
        public double[] ScanArrayJiY = new double[ScanDrawDataLength]; //极坐标y轴点
        public double[] ScanArrayJiYToDraw = new double[ScanDrawDataLength]; //极坐标y轴点

        string selectpath = "";
        int wjfilesnum = 0;
        //FileInfo[] wjFiles = new FileInfo[];
        ArrayList ArrayFileInfo = new ArrayList();
        public int SaveScanDataPackNum = 0;

        public int showindex = 0;
        public int g_n32ScanArmPkgs = 0;
        public int g_n32ScanFPGAPkgs = 0;
        public bool g_bTestRcvdPkgsFlg = true;   //是否开始记录接收数据包数
        #endregion
        
        public ScanDataSet()
        {
            InitializeComponent();
        }

        private void ScanDataSet_Load(object sender, EventArgs e)
        {
            main = (Form1)this.Owner;
            main.g_n32ScanPkgsToCalcAvg = Convert.ToInt32(txt_measureinterval.Text);
            main.g_n32ScanPtToMeasure = Convert.ToInt32(txt_AngleNum.Text); 
            main.g_n32ScanDatDispersion = Convert.ToInt32(txt_DISPERSION.Text);
            main.g_n64CenAngle = Convert.ToDouble(txt_ZEROX.Text);

            g_timer2 = new MillisecondTimer();     // 
            g_timer2.Interval = 20;
            g_timer2.Tick += new EventHandler(timer2_Tick);
            g_timer2.Stop();

            g_timer1 = new MillisecondTimer();     // 
            g_timer1.Interval = 300;
            g_timer1.Tick += new EventHandler(timer1_Tick);
            g_timer1.Stop();
        }

        #region 更新界面
        public void UpDateScanDataSetForm()
        {
            txt_MAXDISTANCE.Text = g_n32ScanMax.ToString();
            txt_MINDISTANCE.Text = g_n32ScanMin.ToString();
            txt_AVERAGEDISTANCE.Text = g_n64ScanAve.ToString();
            txt_VALREF.Text = g_n64ScanAve.ToString();
            txt_PROBABILITY.Text = g_n64ScanPercent.ToString();
        }
        public void UpDateScanDataSetForm1()
        {
            if (g_bTestRcvdPkgsFlg)
            {
                txt_RECVNUM.Text = g_n32ScanCorrectPackageNum.ToString();
                txt_ARMPkgs.Text = g_n32ScanArmPkgs.ToString();         //下位机发送的包数
                txt_DRAWNUM.Text = g_n32ScanDrawPackageNum.ToString();   //绘画包数
                txt_SIGDATACMD.Text = g_n32SingleFrameCmdCnt.ToString(); //单帧命令数
                txt_SAVENUM.Text = SaveScanDataPackNum.ToString();       //保存包数        
                txt_FPGAPkgs.Text = g_n32ScanFPGAPkgs.ToString();
            }
            
        }
        #endregion

        #region 更改textbox数据
        private void txt_DISPERSION_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.KeyCode ==Keys.Enter )
            //    main.g_n32ScanDatDispersion = Convert.ToInt32(txt_DISPERSION.Text);
        }
        private void txt_DISPERSION_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txt_DISPERSION.Text == "")
                //    txt_DISPERSION.Text = "0";
                main.g_n32ScanDatDispersion = Convert.ToInt32(txt_DISPERSION.Text);
            }
            catch 
            {
                return;
            }
            
        }

        private void txt_measureinterval_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.KeyCode ==Keys.Enter )
            //    main.g_n32ScanPkgsToCalcAvg = Convert.ToInt32(txt_measureinterval.Text);
        }
        private void txt_measureinterval_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int l_n32measureinterval=Convert.ToInt32(txt_measureinterval.Text);
                if(l_n32measureinterval >=810)
                {
                    l_n32measureinterval =100;
                    txt_measureinterval.Text = l_n32measureinterval.ToString();
                }
                if(l_n32measureinterval <1)
                {
                    l_n32measureinterval =100;
                    txt_measureinterval.Text = l_n32measureinterval.ToString();
                }
                main.g_n32ScanPkgsToCalcAvg = l_n32measureinterval;
            }
            catch
            {
                return;
            }
           
        }

        private void txt_AngleNum_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //    main.g_n32ScanPtToMeasure = Convert.ToInt32(txt_AngleNum.Text);
        }
        private void txt_AngleNum_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int l_n32AngleNum=Convert.ToInt32(txt_AngleNum.Text);
                if(l_n32AngleNum>=271)
                {
                    l_n32AngleNum =135;
                    txt_AngleNum.Text = l_n32AngleNum.ToString();
                }
                if(l_n32AngleNum <0)
                {
                    l_n32AngleNum =0;
                    txt_AngleNum.Text = l_n32AngleNum.ToString();
                }
                main.g_n32ScanPtToMeasure = l_n32AngleNum;
            }
            catch
            { 
                return;
            }
            
        }

        //零点横坐标
        private void txt_ZEROX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_n64CenAngle = Convert.ToDouble(txt_ZEROX.Text);
            }
            catch
            {
                return;
            }

        }

        //最大测距
        private void txt_MAXDISTANCE1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_n32MaxDis = Convert.ToInt32(txt_MAXDISTANCE1.Text);
            }
            catch
            {
                return;
            }

        }

        //屏显间隔包数
        private void txt_INTERVAL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_n32ScanIntervalPkgsToDraw = Convert.ToInt32(txt_INTERVAL.Text);
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 记录数据
        private void btnrecord_Click(object sender, EventArgs e)
        {
            try
            {
                RealValue = Convert.ToInt32(txt_RELDISC.Text);
                richTextBox1.AppendText("第" + Recordlist.Count + "组数据" + "\n" +
                                        "实际值" + RealValue.ToString() + "\n" +
                                        "最大值" + g_n32ScanMax.ToString() + "\n" +
                                        "最小值" + g_n32ScanMin.ToString() + "\n" +
                                        "平均值" + g_n64ScanAve.ToString() + "\n" +
                                        "离散度" + "±" + txt_DISPERSION.Text + "\n" +
                                        "概率" + g_n64ScanPercent.ToString() + "\r\n");
                RecordStruct l_sRecordData = new RecordStruct();
                l_sRecordData.m_int32max = g_n32ScanMax;
                l_sRecordData.m_int32min = g_n32ScanMin;
                l_sRecordData.m_int32ave = (Int32)g_n64ScanAve;
                l_sRecordData.m_int32realdisc = RealValue;
                l_sRecordData.m_int32maxdiff = g_n32ScanMax - RealValue;
                l_sRecordData.m_int32mindiff = g_n32ScanMin - RealValue;
                l_sRecordData.m_int32avediff = (Int32)g_n64ScanAve - RealValue;
                l_sRecordData.m_int32dispersion = g_n32ScanMax - g_n32ScanMin;
                Recordlist.Add(l_sRecordData);
            }
            catch
            { }
        }
        #endregion

        #region 删除数据
        private void btndelete_Click(object sender, EventArgs e)
        {
            if (Recordlist.Count >= 1)
            {
                richTextBox1.Text += "删除第" + (Recordlist.Count - 1) + "组数据\n";
                Recordlist.RemoveAt(Recordlist.Count - 1);
            }
            else
            {
                richTextBox1.Text += "已经是第0组数据\n";
            }
        }
        private void btn_DeleteAllData_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("是否删除所有数据？", "提示", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    Recordlist.Clear();
                    richTextBox1.Text += "已删除所有记录数据！\n";
                }
                else
                {
                    return;
                }
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 使richtext光标始终定位在最后一行
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length; //Set the current caret position at the end
            richTextBox1.ScrollToCaret(); //Now scroll it automatically
        }
        #endregion 

        #region 保存数据到Excel
        private void btnsave_Click(object sender, EventArgs e)
        {
            try
            {
                path = Environment.CurrentDirectory.ToString() + @"\Excel\" + "\\";
                IWorkbook workbook = new XSSFWorkbook();                    //建立空白工作簿
                ISheet sheet = workbook.CreateSheet("扫描数据");            //在工作簿中：建立空白工作表
                IRow row = sheet.CreateRow(0);                              //在工作表中：建立行，参数为行号，从0计
                ICell cell = row.CreateCell(0);                             //在行中：建立单元格，参数为列号，从0计
                cell.SetCellValue("小型化激光雷达扫描数据");              //设置单元格内容
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //设置单元格的样式：水平对齐居中
                IFont font = workbook.CreateFont();                         //新建一个字体样式对象
                //font.Boldweight = short.MaxValue;                           //设置字体加粗样式
                font.FontName = "宋体";
                font.FontHeight = 16;
                style.SetFont(font);                                        //使用SetFont方法将字体样式添加到单元格样式中 
                cell.CellStyle = style;                                     //将新的样式赋给单元格
                row.Height = 30 * 20;                                       //设置单元格的高度
                //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                NPOI.SS.Util.CellRangeAddress region = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 15);//设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                sheet.AddMergedRegion(region);
                
                IRow row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("实际值");
                row1.CreateCell(1).SetCellValue("最大值");
                row1.CreateCell(2).SetCellValue("最小值");
                row1.CreateCell(3).SetCellValue("平均值");
                row1.CreateCell(4).SetCellValue(" ");
                row1.CreateCell(5).SetCellValue("最大偏差");
                row1.CreateCell(6).SetCellValue("最小偏差");
                row1.CreateCell(7).SetCellValue("平均值偏差");
                row1.CreateCell(8).SetCellValue("离散度");
                row1.CreateCell(9).SetCellValue(" ");
                row1.CreateCell(10).SetCellValue("绝对最大偏差");
                row1.CreateCell(11).SetCellValue("绝对最小偏差");
                row1.CreateCell(12).SetCellValue("绝对平均偏差");
                row1.CreateCell(13).SetCellValue("偏移量");
                row1.CreateCell(14).SetCellValue("绝对精度");
                //设置单元格的宽度
                for (int i = 0; i < 15; i++)
                {
                    sheet.SetColumnWidth(i, 13 * 256);//设置单元格的宽度
                    //row1.GetCell(i).CellStyle = style;//设置单元格格式
                }

                int len = Recordlist.Count;
                //int l_n32criticalno = 0;
                int temp = 0;
                if (len > 0)
                {
                    //for (int i = 0; i < len; i++)     //查找临界值
                    //{
                    //    if (((RecordStruct)Recordlist[temp]).m_int32realdisc > 500 &&
                    //        ((RecordStruct)Recordlist[temp]).m_int32realdisc < 1500)
                    //    {
                    //        l_n32criticalno = i;
                    //        break;
                    //    }
                    //}
                    for (int i = 0; i < len; i++)
                    {
                        IRow rowx = sheet.CreateRow(i + 2);

                        //if (i % 2 == 0)
                        //{
                        //    temp = (i / 2 + 10);    //temp = (i / 2 + l_n32criticalno+1);
                        //}
                        //else
                        //{
                        //    temp = (i - 1) / 2;   //temp = (l_n32criticalno - (i - 1) / 2);
                        //}
                        temp = i;
                        rowx.CreateCell(0).SetCellValue(((RecordStruct)Recordlist[temp]).m_int32realdisc);
                        rowx.CreateCell(1).SetCellValue(((RecordStruct)Recordlist[temp]).m_int32max);
                        rowx.CreateCell(2).SetCellValue(((RecordStruct)Recordlist[temp]).m_int32min);
                        rowx.CreateCell(3).SetCellValue(((RecordStruct)Recordlist[temp]).m_int32ave);

                        rowx.CreateCell(4);

                        rowx.CreateCell(5);
                        rowx.CreateCell(6);
                        rowx.CreateCell(7);
                        rowx.CreateCell(8);

                        rowx.CreateCell(9);

                        rowx.CreateCell(10);//绝对最大偏差
                        rowx.CreateCell(11);//绝对最小偏差
                        rowx.CreateCell(12);//绝对平均偏差
                        rowx.CreateCell(13);//偏移量
                        rowx.CreateCell(14);//绝对精度
                        rowx.CreateCell(15);//绝对精度

                    }
                    //设置保留小数点的个数
                    ICellStyle cellStyle = workbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                    IRow row2 = sheet.GetRow(2);

                    ICell cellx13 = row2.GetCell(13);
                    cellx13.SetCellFormula("AVERAGE(H3:H"+(len+2).ToString()+")"); //偏移量：平均偏差的平均值
                    cellx13.CellStyle = cellStyle;
                   
                   
                    if (len > 0)
                    {
                        for (int i = 0; i < len; i++)
                        {
                            IRow rowx = sheet.GetRow(i + 2);
                            string rowno = (i + 3).ToString();
                            ICell cellx5 = rowx.GetCell(5);
                            cellx5.SetCellFormula("SUM(B" + rowno + ",A" + rowno + "*(-1))");  //最大偏差： 最大值减实际值

                            ICell cellx6 = rowx.GetCell(6);
                            cellx6.SetCellFormula("SUM(C" + rowno + ",A" + rowno + "*(-1))"); //最小偏差：最小值减实际值

                            ICell cellx7 = rowx.GetCell(7);
                            cellx7.SetCellFormula("SUM(D" + rowno + ",A" + rowno + "*(-1))"); //平均偏差： 平均值减实际值

                            ICell cellx8 = rowx.GetCell(8);
                            cellx8.SetCellFormula("SUM(B" + rowno + ",C" + rowno + "*(-1))"); //离散度：最大值减最小值

                            ICell cellx10 = rowx.GetCell(10);
                            cellx10.SetCellFormula("SUM(F" + rowno + ",N3*(-1))");  //绝对最大偏差：最大偏差减去偏移量
                            cellx10.CellStyle = cellStyle;

                            ICell cellx11 = rowx.GetCell(11);
                            cellx11.SetCellFormula("SUM(G" + rowno + ",N3*(-1))"); //绝对最小偏差：最小偏差减去偏移量
                            cellx11.CellStyle = cellStyle;

                            ICell cellx12 = rowx.GetCell(12);
                            cellx12.SetCellFormula("SUM(H" + rowno + ",N3*(-1))"); //绝对平均偏差：平均偏差减去偏移量
                            cellx12.CellStyle = cellStyle;
                        }
                    }

                    //ICell cellx14 = row2.GetCell(14);
                    //cellx14.SetCellFormula("SUM(MAX(F3:F"+(len+2).ToString()+"),MIN(G3:G"+(len+2).ToString()+")*(-1))"); //绝对精度：最大偏差的最大值减去最小偏差的最小值

                    IRow row3 = sheet.GetRow(3);
                    ICell cell314 = row3.GetCell(14);
                    cell314.SetCellValue("0.5-6m"); //0.5-6 
                    ICell cell315 = row3.GetCell(15);
                    cell315.SetCellFormula("SUM(MAX(F4:F14),MIN(G4:G14)*(-1))"); //0.5-6 绝对精度：最大偏差的最大值减去最小偏差的最小值


                    IRow row4 = sheet.GetRow(4);
                    ICell cell414 = row4.GetCell(14);
                    cell414.SetCellValue("0.5-8m"); //0.5-8
                    ICell cell415 = row4.GetCell(15);
                    cell415.SetCellFormula("SUM(MAX(F4:F18),MIN(G4:G18)*(-1))"); //0.5-8绝对精度：最大偏差的最大值减去最小偏差的最小值

                    IRow row5 = sheet.GetRow(5);
                    ICell cell514 = row5.GetCell(14);
                    cell514.SetCellValue("0-10m"); //0.5-10
                    ICell cell515 = row5.GetCell(15);
                    cell515.SetCellFormula("SUM(MAX(F3:F" + (len + 2).ToString() + "),MIN(G3:G" + (len + 2).ToString() + ")*(-1))"); //绝对精度：最大偏差的最大值减去最小偏差的最小值

                    if (ExcelFormName == null || ExcelFormName.IsDisposed)
                    {
                        ExcelFormName = new ExcelDocumentName();
                    }
                    ExcelFormName.ShowDialog(this);
                    ExcelName = ExcelFormName.name;
                    if (ExcelName != null)
                    {
                        path = path + ExcelName + ".xlsx";
                        FileStream file2007 = new FileStream(path, FileMode.Create);
                        workbook.Write(file2007);
                        file2007.Close();
                        workbook.Close();
                        MessageBox.Show("数据保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //加载excel
                        System.Diagnostics.Process.Start(path);
                    }
                }
                else
                {
                    MessageBox.Show("记录数据为空，请重新记录！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            catch
            {
                MessageBox.Show("保存数据出错，请重新操作", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        #endregion

        #region 单帧取数命令
        private void btn_single_Click(object sender, EventArgs e)
        {
            try
            {
                //if (main.g_SocketClient.Connected == true)
                if(main.serialPort1.IsOpen)
                {
                    //byte[] l_abGetOneFrame = new byte[34] {0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                    //                                       0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                    //                                       0x00,0x00,0x00,0x00,0x06,0x09,0x00,0x00,0x00,
                    //                                       0x00,0x00,0x00,0x00,0x1C,0xEE,0xEE};

                    //l_abGetOneFrame[31] = ToolFunc.XorCheck_byte(2, l_abGetOneFrame, 4);
                    byte[] l_abGetOneFrame = new byte[24] {0x02,0x02,0x02,0x02,0x00,0x00,0x00,0x0F,
                                                           0x73,0x52,0x4e,0x20,0x4c,0x4d,0x44,0x73,
                                                           0x63,0x61,0x6e,0x64,0x61,0x74,0x61,0x05};

                    l_abGetOneFrame[23] = ToolFunc.XorCheck_byte(8, l_abGetOneFrame, 1);
                    //main.g_SocketClient.Send(l_abGetOneFrame);
                    main.serialPort1.Write(l_abGetOneFrame, 0, l_abGetOneFrame.Length);
                    g_n32SingleFrameCmdCnt++;
                }
                richTextBox1.AppendText(DateTime.Now.ToString() + " " + "发送单帧取数命令" + "\r");
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 连续获取命令
        private void btn_continue_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_continue.Text == "停止获取")
                {
                    btn_continue.Text = "连续获取";
                    if(main.serialPort1.IsOpen)
                    {

                        byte[] acStopGetSickContinueFrameBuf = new byte[26] {0x02,0x02,0x02,0x02,0x00,0x00,0x00,0x11,
                                                                             0x73,0x45,0x4e,0x20,0x4c,0x4d,0x44,0x73,
                                                                             0x63,0x61,0x6e,0x64,0x61,0x74,0x61,0x20,0x00,0x00};
                        acStopGetSickContinueFrameBuf[25] = ToolFunc.XorCheck_byte(8, acStopGetSickContinueFrameBuf, 1);
                        main.serialPort1.Write(acStopGetSickContinueFrameBuf, 0, acStopGetSickContinueFrameBuf.Length);
                    }
                    richTextBox1.AppendText(DateTime.Now.ToString() + " " + "发送停止获取命令" + "\r");
                }
                else
                {
                    btn_continue.Text = "停止获取";
                    if(main.serialPort1.IsOpen)
                    {
                        byte[] acStopGetSickContinueFrameBuf = new byte[26] {0x02,0x02,0x02,0x02,0x00,0x00,0x00,0x11,
                                                                             0x73,0x45,0x4e,0x20,0x4c,0x4d,0x44,0x73,
                                                                             0x63,0x61,0x6e,0x64,0x61,0x74,0x61,0x20,0x01,0x00};
                        acStopGetSickContinueFrameBuf[25] = ToolFunc.XorCheck_byte(8, acStopGetSickContinueFrameBuf, 1);
                        main.serialPort1.Write(acStopGetSickContinueFrameBuf, 0, acStopGetSickContinueFrameBuf.Length);
                    }
                    richTextBox1.AppendText(DateTime.Now.ToString() + " " + "发送连续获取命令" + "\r");
                }
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 定时20ms连续发送单帧获取指令
        private void checkBox_continuesend_Click(object sender, EventArgs e)
        {
            if (checkBox_continuesend.CheckState == CheckState.Checked)
            {
                g_timer2.Start ();
                richTextBox1.AppendText(DateTime.Now.ToString() + " " + "20ms定时发送单帧取数命令"+"\r");
            }

            else
            {
                g_timer2.Stop();
                richTextBox1.AppendText(DateTime.Now.ToString() + " " + "停止发送单帧取数命令" + "\r");
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                //if (main.g_SocketClient.Connected == true)
                if(main.serialPort1.IsOpen)
                {
                    byte[] acGetSickOneFrameBuf = new byte[24] {0x02,0x02,0x02,0x02,0x00,0x00,0x00,0x0F,
                                                                0x73,0x52,0x4e,0x20,0x4c,0x4d,0x44,0x73,
                                                                0x63,0x61,0x6e,0x64,0x61,0x74,0x61,0x05};
                    //main.g_SocketClient.Send(acGetSickOneFrameBuf);
                    main.serialPort1.Write(acGetSickOneFrameBuf, 0, acGetSickOneFrameBuf.Length);
                    g_n32SingleFrameCmdCnt++;
                }
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 清除计数
        private void btnCLEARCNT_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] l_byteBufClearCounter = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                            0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                            0x00,0x00,0x00,0x00,0x07,0x0C,0x00,0x00,0x00,
                                                            0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};
                if(main.serialPort1.IsOpen)
                {
                    l_byteBufClearCounter[31] = ToolFunc.XorCheck_byte(2, l_byteBufClearCounter, 4);
                    main.serialPort1.Write(l_byteBufClearCounter, 0, l_byteBufClearCounter.Length);
                }
                
                main.g_n32ScanDatCorrectPkgs = 0;
                main.g_n32DrawPkgssOfJi = 0;
                main.g_n32SaveScanDatPkgs = 0;
                main.g_n32ScanDatRcvdFrames = 0;
                //main.g_ln32armpkgs.Clear();
                g_bTestRcvdPkgsFlg = true;
                label_start.Text = DateTime.Now.ToString("MM-dd-HH-mm-ss-fff");  
            }
            catch { }
            
        }
        #endregion

        #region 查看历史数据

        #region 打开历史数据文件夹
        private void btndrawhistory_Click(object sender, EventArgs e)
        {
            try
            {
                g_timer1.Stop();
                showindex = 0;
                main.Clear_JI();
                btn_startShow.Text = "播放";
                folderBrowserDialog1.SelectedPath = NetDataFilesPath;
                folderBrowserDialog1.Description = "select";
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    selectpath = folderBrowserDialog1.SelectedPath;
                }

                if (selectpath != "")
                {
                    DialogResult dr = MessageBox.Show(selectpath, "所选文件夹", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        richTextBox1.Text += "界面操作 点击\'选择\'按钮,并选择了文件夹：\n" + selectpath + "\r\n";
                    }
                    else
                    {
                        return;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(selectpath);
                    //int wjfileNum = dir.GetFiles("*.wj").Length; // 该目录下的文件数量。
                    FileInfo[] wjFiles = dir.GetFiles("*.txt");
                    wjfilesnum = wjFiles.Length;
                    richTextBox1.Text += DateTime.Now.ToString() + "\r\n" + "选择数据文件夹，共有" + wjfilesnum.ToString() + "条数据" + "\r\n";
                    string readpath = selectpath + "\\" + wjFiles[0].ToString();
                    //ScanArrayJiYToDraw = WriteOrReadFiles.Readtxt(readpath, ScanDrawDataLength);// readtxt(readpath);
                    List<string> ListDataReadFromFile = WriteOrReadFiles.ReadDataFromFileByStreamOneLine(readpath);
                    for (int i = 0; i < ListDataReadFromFile.Count; i++)
                    {
                        ScanArrayJiYToDraw[i] =Convert .ToDouble (ListDataReadFromFile[i]);
                    }
                    main.Draw_JI(ScanArrayJiYToDraw);
                    ArrayFileInfo.Clear();
                    ArrayFileInfo.AddRange(wjFiles);
                    txt_jump.Text = 0.ToString();
                }
            }
            catch
            {
                return;
            }
        }
        #endregion 

        #region 开始保存按钮
        //开始保存
        private void btnSAVEDATA_Click(object sender, EventArgs e)
        {
            if (btn_SAVEDATA.Text == "暂停保存")
            {
                btn_SAVEDATA.Text = "开始保存";
                SaveScanDataToFile = false;
            }
            else
            {
                btn_SAVEDATA.Text = "暂停保存";
                SaveScanDataToFile = true;
                string SonDirectoryName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff_") + txt_RELDISC.Text;
                NetDataFilesSonPath = NetDataFilesPath + SonDirectoryName;
                Directory.CreateDirectory(NetDataFilesSonPath);//创建子文件夹
                richTextBox1.Text += "子文件夹名称为：" + SonDirectoryName + "\r\n";
            }

        }
        #endregion 

        #region 前一帧
        private void btnprevious_Click(object sender, EventArgs e)
        {
            try
            {
                int fileindex = Convert.ToInt32(txt_jump.Text);
                int arraycount = ArrayFileInfo.Count;
                if (fileindex < arraycount)
                {
                    if (fileindex >= 1)
                    {
                        fileindex -= 1;
                        string readpath = selectpath + "\\" + ArrayFileInfo[fileindex].ToString();
                        //ScanArrayJiYToDraw = WriteOrReadFiles.Readtxt(readpath, ScanDrawDataLength);// readtxt(readpath);
                        List<string> ListDataReadFromFile = WriteOrReadFiles.ReadDataFromFileByStreamOneLine(readpath);
                        for (int i = 0; i < ListDataReadFromFile.Count; i++)
                        {
                            ScanArrayJiYToDraw[i] = Convert.ToDouble(ListDataReadFromFile[i]);
                        }
                        main.Draw_JI(ScanArrayJiYToDraw);
                        txt_jump.Text = fileindex.ToString();
                        showindex = fileindex;
                    }
                    else
                    {
                        MessageBox.Show("已经是第一组数据！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("输入的索引值超过文件个数！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                    
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 后一帧
        private void btnnext_Click(object sender, EventArgs e)
        {
            try
            {
                int fileindex = Convert.ToInt32(txt_jump.Text);
                int arraycount = ArrayFileInfo.Count;
                if (fileindex < arraycount)
                {
                    if (fileindex <= arraycount - 2)
                    {
                        fileindex += 1;
                        string readpath = selectpath + "\\" + ArrayFileInfo[fileindex].ToString();
                        //ScanArrayJiYToDraw = WriteOrReadFiles.Readtxt(readpath, ScanDrawDataLength);// readtxt(readpath);
                        List<string> ListDataReadFromFile = WriteOrReadFiles.ReadDataFromFileByStreamOneLine(readpath);
                        for (int i = 0; i < ListDataReadFromFile.Count; i++)
                        {
                            ScanArrayJiYToDraw[i] = Convert.ToDouble(ListDataReadFromFile[i]);
                        }
                        main.Draw_JI(ScanArrayJiYToDraw);
                        txt_jump.Text = fileindex.ToString();
                        showindex = fileindex;
                    }
                    else
                    {
                        MessageBox.Show("已经是最后一组数据！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("输入的索引值超过文件个数！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch
            {
                return;
            }
        }
        #endregion

        #region 跳转
        private void btn_jump_Click(object sender, EventArgs e)
        {
            try
            {
                int fileindex = Convert.ToInt32(txt_jump.Text);
                int arraycount = ArrayFileInfo.Count;
                if (fileindex < arraycount)
                {
                    string readpath = selectpath + "\\" + ArrayFileInfo[fileindex].ToString();
                    ScanArrayJiYToDraw = WriteOrReadFiles.Readtxt(readpath, ScanDrawDataLength);// readtxt(readpath);
                    main.Draw_JI(ScanArrayJiYToDraw);
                    showindex = fileindex;
                }
                else
                {
                    MessageBox.Show("输入的索引值超过文件个数！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch
            {
                return;
            }
        }
        #endregion

        #region 播放
        private void btn_startShow_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_startShow.Text == "暂停")
                {
                    btn_startShow.Text = "播放";
                    g_timer1.Stop();
                }
                else
                {
                    btn_startShow.Text = "暂停";
                    if (showindex > ArrayFileInfo.Count)
                    {
                        showindex = 0;
                    }
                    g_timer1.Start();
                }

            }
            catch
            {
                return;
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (showindex < ArrayFileInfo.Count)
                {
                    string readpath = selectpath + "\\" + ArrayFileInfo[showindex++].ToString();
                    ScanArrayJiYToDraw = WriteOrReadFiles.Readtxt(readpath, ScanDrawDataLength);// readtxt(readpath);
                    main.Draw_JI(ScanArrayJiYToDraw);
                    MethodInvoker mi = new MethodInvoker(show_index);
                    this.BeginInvoke(mi);
                }
            }
            catch
            {
                return;
            }

        }
        private void show_index()
        {
            txt_jump.Text = showindex.ToString();
        }

        #endregion

        private void btnCLEAR_Click(object sender, EventArgs e)
        {
            g_bTestRcvdPkgsFlg = false;
            label_End.Text = DateTime.Now.ToString("MM-dd-HH-mm-ss-fff");   
        }
        #endregion




    }
}
