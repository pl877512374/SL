using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Threading;

namespace fzjgld
{
    public partial class SI2DCFG : Form
    {
        #region 参数定义
        Form1 main = new Form1();
        struct RecordStruct
        {
            public int m_int32SImax;
            public int m_int32SImin;
            public int m_int32SIave;
            public int m_int32SIrealdisc;
        };
        public int RealValue = 0;              //实际值
        ArrayList Recordlist = new ArrayList();
        string path = Environment.CurrentDirectory.ToString() + @"\Excel\" + "\\";
        ExcelDocumentName ExcelFormName;
        public string ExcelName = null;


        public bool m_bSiDataSaveFlag=false ;//开始保存
        public bool m_bCacuFlag = false;//计算
        //public string Si2DPath = Environment.CurrentDirectory.ToString() + @"\NetDataFiles\";
        //public string file = "";
        public string g_srisingfile = "";
        public string g_sfallingfile = "";
        public string g_stimingfile = "";
        public FileStream Si2Dfs_ris;
        public FileStream Si2Dfs_fall;
        public FileStream Si2Dfs_tim;
        public StreamWriter Si2Dsb_ris;
        public StreamWriter Si2Dsb_fall;
        public StreamWriter Si2Dsb_tim;
        //public FileStream Si2Dfs ;
        //public BinaryWriter Si2Dsb;


        public string Si2DCalculateResultPath = Environment.CurrentDirectory.ToString() + @"\CalculateResult\";
        //List<Int64> g_datax=new List<Int64>();
        //List<Int64> g_datay=new List<Int64>();
        public string CorrectedSheetFilePath = "";
        public const int readbuflen = 20000;
        public int[] g_n32BufCorrectedDat = new int[readbuflen];//烧写缓存
        public int g_nReadbuflen = 0; //实际长度
        public byte[] g_byteBufCorrectedPkg = new byte[2000];
        
        Thread MyXiuzhengDataSendThread;
        public int Si2DChselIndex = 0;

        public const int arrayXYsize = 100;
        //public Int64[] g_Interpdatax = new Int64[arrayXYsize];//读取拟合结果存放数组
        //public Int64[] g_Interpdatay = new Int64[arrayXYsize];//读取拟合结果存放数组
        public AutoResetEvent Si2DSendResetEvent = new AutoResetEvent(false);
        public int g_n32RetryNum = 0;//重发次数

        public int g_n32SiAvgdata = 0;  //脉宽
        public int g_n32SiRecvNum = 0;  //接收包数
        public int g_n32SiDrawNum = 0;  //绘画包数
        public int g_n32SiSaveNum = 0;  //接收包数
        public int g_n32Si2DMax = 0;
        public int g_n32Si2DMin = 0;
        public int g_n32Si2DAve = 0;
        public double g_n64Si2DPercent = 0;

        public int g_n32Si2DCorrectedPkgNo = 0;




        #endregion
        public SI2DCFG()
        {
            InitializeComponent();
        }
        
        private void SI2DCFG_Load(object sender, EventArgs e)
        {
            main = (Form1)this.Owner;
            //InterpForm = new INTERP();放在这直接卡死界面
            main.g_n32Si2DIntervalPkgsToDraw = Convert.ToInt32(txt_INTERVAL.Text);
            main.g_n32Si2DPkgsToCalc = Convert.ToInt32(txt_CACULATE.Text);
            main.g_n32SiDispersion = Convert.ToInt32(txt_DISPERSION.Text);
            main.g_n32Si2DPtToMeasure = Convert.ToInt32(txt_calindex.Text);
            main.g_n32SiCalNo = Convert.ToInt32(txt_sicalno.Text);
            main.g_nSi2DOneChart = Convert.ToInt32(txt_ONECHART.Text);
            comb_CHSEL.SelectedIndex = 0;
            //comb_reflective.Text = "8%";
            btn_SEND_DATA.Enabled = false;
        }

        #region 清屏
        private void btn_CLEAN_Click(object sender, EventArgs e)
        {
            richTextBox_RICHEDIT_DATA.Text = "";
            richTextBoxT_OPERATEMESSAGE.Text = "";
        }
        #endregion

        #region 记录
        private void btnrecord_Click(object sender, EventArgs e)
        {
            RealValue = Convert.ToInt32(txt_REALDISC.Text);
            richTextBox_RICHEDIT_DATA.AppendText("第" + Recordlist.Count + "组数据" + "\n" +
                                                 "实际值" + RealValue.ToString() + "\n" +
                                                 "最大值" + g_n32Si2DMax.ToString() + "\n" +
                                                 "最小值" + g_n32Si2DMin.ToString() + "\n" +
                                                 "平均值" + g_n32Si2DAve.ToString() + "\n" +
                                                 "离散度" + "±" + txt_DISPERSION.Text + "\n" +
                                                 "概率" + g_n64Si2DPercent.ToString() + "\r\n");
            RecordStruct l_sRecordData = new RecordStruct();
            l_sRecordData.m_int32SImax = (Int32)g_n32Si2DMax;
            l_sRecordData.m_int32SImin = (Int32)g_n32Si2DMin;
            l_sRecordData.m_int32SIave = (Int32)g_n32Si2DAve;
            l_sRecordData.m_int32SIrealdisc = (Int32)RealValue;
            Recordlist.Add(l_sRecordData);
        }
        #endregion

        #region 删除
        private void btndelete_Click(object sender, EventArgs e)
        {
            if (Recordlist.Count >= 1)
            {
                richTextBox_RICHEDIT_DATA.Text += "删除第" + (Recordlist.Count - 1) + "组数据\n";
                Recordlist.RemoveAt(Recordlist.Count - 1);
            }
            else
            {
                richTextBox_RICHEDIT_DATA.Text += "已经是第0组数据\n";
            }
        }
        #endregion

        #region 使richtext光标始终定位在最后一行
        private void richTextBox_RICHEDIT_DATA_TextChanged(object sender, EventArgs e)
        {
            richTextBox_RICHEDIT_DATA.SelectionStart = richTextBox_RICHEDIT_DATA.Text.Length; //Set the current caret position at the end
            richTextBox_RICHEDIT_DATA.ScrollToCaret(); //Now scroll it automatically
        }
        private void richTextBoxT_OPERATEMESSAGE_TextChanged(object sender, EventArgs e)
        {
            richTextBoxT_OPERATEMESSAGE.SelectionStart = richTextBoxT_OPERATEMESSAGE.Text.Length; //Set the current caret position at the end
            richTextBoxT_OPERATEMESSAGE.ScrollToCaret(); //Now scroll it automatically
        }
        #endregion

        #region 保存
        private void btnsave_Click(object sender, EventArgs e)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook();                    //建立空白工作簿
                ISheet sheet = workbook.CreateSheet("单点数据");            //在工作簿中：建立空白工作表
                IRow row = sheet.CreateRow(0);                              //在工作表中：建立行，参数为行号，从0计
                ICell cell = row.CreateCell(0);                             //在行中：建立单元格，参数为列号，从0计
                cell.SetCellValue("小型化激光雷达单点数据");              //设置单元格内容
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //设置单元格的样式：水平对齐居中
                IFont font = workbook.CreateFont();                         //新建一个字体样式对象
                font.Boldweight = short.MaxValue;                           //设置字体加粗样式
                style.SetFont(font);                                        //使用SetFont方法将字体样式添加到单元格样式中 
                cell.CellStyle = style;                                     //将新的样式赋给单元格
                row.Height = 30 * 20;                                       //设置单元格的高度
                //设置单元格的宽度
                //sheet.SetColumnWidth(0, 30 * 256)
                //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                NPOI.SS.Util.CellRangeAddress region = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 10);//设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                sheet.AddMergedRegion(region);

                IRow row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("实际值");
                row1.CreateCell(1).SetCellValue("最大值");
                row1.CreateCell(2).SetCellValue("最小值");
                row1.CreateCell(3).SetCellValue("平均值");
                for (int i = 0; i < Recordlist.Count; i++)
                {
                    IRow row2 = sheet.CreateRow(i + 2);
                    row2.CreateCell(0).SetCellValue(((RecordStruct)Recordlist[i]).m_int32SIrealdisc);
                    row2.CreateCell(1).SetCellValue(((RecordStruct)Recordlist[i]).m_int32SImax);
                    row2.CreateCell(2).SetCellValue(((RecordStruct)Recordlist[i]).m_int32SImin);
                    row2.CreateCell(3).SetCellValue(((RecordStruct)Recordlist[i]).m_int32SIave);
                }

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
                    MessageBox.Show("数据保存成功");
                }
            }
            catch
            {
                MessageBox.Show("保存数据出错，请重新操作", "Error");
                return;
            }
        }
        #endregion

        #region 开始发数
        private void btn_PAUSE_SEND_Click(object sender, EventArgs e)
        {
            try
            {
                //if (main.g_SocketClient.Connected == true)
                if(main.serialPort1.IsOpen)
                {
                    byte[] l_byteBufQueryData = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                             0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                             0x00,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x00,
                                                             0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};

                    if (btn_PAUSE_SEND.Text == "暂停发数")
                    {
                        btn_PAUSE_SEND.Text = "开始发数";
                        l_byteBufQueryData[25] = 0x00;
                        l_byteBufQueryData[31] = ToolFunc.XorCheck_byte(2, l_byteBufQueryData, 4);
                        //main.g_SocketClient.Send(l_byteBufQueryData);
                        main.serialPort1.Write(l_byteBufQueryData, 0, l_byteBufQueryData.Length);
                    }
                    else
                    {
                        l_byteBufQueryData[25] = 0x01;
                        l_byteBufQueryData[31] = ToolFunc.XorCheck_byte(2, l_byteBufQueryData, 4);
                        //main.g_SocketClient.Send(l_byteBufQueryData);
                        main.serialPort1.Write(l_byteBufQueryData, 0, l_byteBufQueryData.Length);
                        btn_PAUSE_SEND.Text = "暂停发数";
                    }

                }
            }
            catch
            {
                MessageBox.Show("数据发送失败，请重新操作", "Error");

            }
        }
        #endregion

        # region 清除显示（清除单点数据tchart4的3条图像）
        private void btn_IDOK7_Click(object sender, EventArgs e)
        {
            main.TchartClear();
            main.g_n32Si2DRcvdPkgs = 0;
        }
        #endregion

        #region 打开修正表
        private void btn_IDOK8_Click(object sender, EventArgs e)
        {
            string extension ;//扩展名 ".aspx"

            main.g_n32CorrectionPackNo = 0;
            main.g_bCorrectionSuccessFlg = false;
            g_n32Si2DCorrectedPkgNo = 0;
            Si2DSendResetEvent.Reset();

            if (comb_CHSEL.Text == "前沿修正")
            {
                openFileDialog2.FileName = "";
                openFileDialog2.Filter = "H Files(*.txt_H)|*.txt_H";
                openFileDialog2.ShowDialog();
                if (openFileDialog2.FileName != "")
                {
                    extension = Path.GetExtension(openFileDialog2.FileName);
                    if (extension != ".txt_H")
                    {
                        richTextBoxT_OPERATEMESSAGE.Text = "";
                        richTextBoxT_OPERATEMESSAGE.AppendText("打开文件失败o(>﹏<)o！\n表和选项不对应，请选择高阈值文件！");
                        MessageBox.Show("请选择高阈值文件");
                        return;
                    }
                    else
                    {
                        CorrectedSheetFilePath = openFileDialog2.FileName;
                    }
                }
            }
            else if (comb_CHSEL.Text == "计时修正")
            {
                openFileDialog2.FileName = "";
                openFileDialog2.Filter = "L Files(*.txt_L)|*.txt_L";
                openFileDialog2.ShowDialog();
                if (openFileDialog2.FileName != "")
                {
                    extension = Path.GetExtension(openFileDialog2.FileName);//扩展名 ".aspx"
                    if (extension != ".txt_L")
                    {
                        richTextBoxT_OPERATEMESSAGE.Text = "";
                        richTextBoxT_OPERATEMESSAGE.AppendText("打开文件失败o(>﹏<)o！\n表和选项不对应，请选择反射率文件！");
                        MessageBox.Show("请选择计时修正文件");
                        return;
                    }
                    else
                    {
                        CorrectedSheetFilePath = openFileDialog2.FileName;
                    }
                }
            }
            else
            {
                MessageBox.Show("模式选择错误！", "Warning");
            }
            if (CorrectedSheetFilePath != "")
            {
                btn_SEND_DATA.Enabled = true;
            }
        }
        #endregion

        #region 每路间隔包数
        private void txt_INTERVAL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_n32Si2DIntervalPkgsToDraw = Convert.ToInt32(txt_INTERVAL.Text);
            }
            catch
            { 
            }
        }
        #endregion

        #region 计算包数
        private void txt_CACULATE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_n32Si2DPkgsToCalc = Convert.ToInt32(txt_CACULATE.Text);
            }
            catch
            {
            }
        }
        #endregion

        #region 每路屏显包数
        private void txt_ONECHART_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_nSi2DOneChart = Convert.ToInt32(txt_ONECHART.Text);
            }
            catch
            {
            }
        }
        #endregion

        #region 离散度
        private void txt_DISPERSION_TextChanged(object sender, EventArgs e)
        {
            try
            {
                main.g_n32SiDispersion = Convert.ToInt32(txt_DISPERSION.Text);
            }
            catch
            {
            }
        }
        #endregion

        #region 计算索引 calindex(作图索引，在0—1079个数据中选择第几个数据进行计算，作图)
        private void txt_calindex_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int index=Convert.ToInt32(txt_calindex.Text);
                if(index>1079)
                {
                    index =540;
                    txt_calindex.Text = index.ToString();
                }
                if(index<0)
                {
                    index=0;
                    txt_calindex.Text = index.ToString();
                }
                main.g_n32Si2DPtToMeasure = index;
            }
            catch
            {
            }
        }
        #endregion

        #region 保存单点数据
        public string FallingPath;
        public string RisingPath; 
        public string TimingPath;
        private void btn_StartSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_bSiDataSaveFlag == false)
                {
                    m_bSiDataSaveFlag = true;

                    //保存.WJ文件
                    if (Directory.Exists(g_sFolderPath) == false)
                    {
                        Directory.CreateDirectory(g_sFolderPath);
                    }
                    string filename_ris = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                      DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-" +
                      DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "-" +
                      DateTime.Now.Millisecond.ToString() + "-" + "Rising";
                    string filename_fall = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                                           DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-" +
                                           DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "-" +
                                           DateTime.Now.Millisecond.ToString() + "-" + "Falling";
                    string filename_tim = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                                          DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-" +
                                          DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "-" +
                                          DateTime.Now.Millisecond.ToString() + "-" + "Timing";
                    g_srisingfile = g_sFolderPath + filename_ris + ".txt_R";
                    g_sfallingfile = g_sFolderPath + filename_fall + ".txt_F";
                    g_stimingfile = g_sFolderPath + filename_tim + ".txt_T";

                    Si2Dfs_ris = new FileStream(@g_srisingfile, FileMode.Append, FileAccess.Write);
                    Si2Dsb_ris = new StreamWriter(Si2Dfs_ris);

                    Si2Dfs_fall = new FileStream(@g_sfallingfile, FileMode.Append, FileAccess.Write);
                    Si2Dsb_fall = new StreamWriter(Si2Dfs_fall);

                    Si2Dfs_tim = new FileStream(@g_stimingfile, FileMode.Append, FileAccess.Write);
                    Si2Dsb_tim = new StreamWriter(Si2Dfs_tim);
                    //Si2Dsb = new BinaryWriter(Si2Dfs);


                    

              /************************************  文件夹的形式保存数据    **************************************
               * 
                    string FallingName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff--") + "Falling";
                    FallingPath = g_sFolderPath + FallingName;
                    Directory.CreateDirectory(FallingPath);//创建子文件夹

                    string RisingName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff--") + "Rising";
                    RisingPath = g_sFolderPath + RisingName;
                    Directory.CreateDirectory(RisingPath);//创建子文件夹

                    string TimingName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff--") + "Timing";
                    TimingPath = g_sFolderPath + TimingName;
                    Directory.CreateDirectory(TimingPath);//创建子文件夹
               * 
               * **********************************************************************************************************/
                    
                    btn_StartSave.Text = "暂停保存";
                    main.g_n32Si2DRcvdPkgs = 0;
                    main.g_n32Si2DXToDraw = 0;
                }
                else if (m_bSiDataSaveFlag == true)
                {
                    m_bSiDataSaveFlag = false;
                    btn_StartSave.Text = "开始保存";
                    btn_StartSave.Enabled = false;

 
                    //关闭写数据流
                    Si2Dsb_ris.Close();
                    Si2Dfs_ris.Close();

                    Si2Dsb_fall.Close();
                    Si2Dfs_fall.Close();

                    Si2Dsb_tim.Close();
                    Si2Dfs_tim.Close();


                }
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 计算
        private void btn_CACULATE_Click(object sender, EventArgs e)
        {
            m_bCacuFlag = true;
            btn_CACULATE.Text = "计算中...";
            //初始化变量
            main.g_n32RisingEdgeSum = 0;
            main.g_n32FallingEdgeSum = 0;
            main.g_n32TimingEdgeSum = 0;
            main.g_n32RcvdCalcPkgsOfRisingEdge = 0;
            main.g_n32RcvdCalcPkgsOfFallingEdge = 0;
            main.g_n32RcvdCalcPkgsOfTimingEdge = 0;
            main.g_n32RisingEdgeAvg = 0;
            main.g_n32FallingEdgeAvg = 0;
            main.g_n32TimingEdgeAvg = 0;
        }

        public void StopCaCuLate()
        {
            m_bCacuFlag = false ;
            btn_CACULATE.Text = "计算";
        }
        #endregion

        #region 更新界面

        public void UpdataSI2DCFG()
        {
            try
            {
                MethodInvoker updatasi2dcfg = new MethodInvoker(updata_si2dcfg);
                if (this.IsDisposed)//执行委托之前要先判断窗体是否已经关闭
                    return;
                else
                    this.BeginInvoke(updatasi2dcfg);              
            }
            catch
            {
                //return;
            }

        }
        public void updata_si2dcfg()
        {
            txt_EDITSUB.Text = g_n32SiAvgdata.ToString();
            txt_RECVNUM.Text = g_n32SiRecvNum.ToString();
            txt_DRAWNUM.Text = g_n32SiDrawNum.ToString();
            txt_SAVENUM.Text = g_n32SiSaveNum.ToString();
            //txt_EDITSUB.Text = main.avgdata.ToString();//脉宽
            //txt_RECVNUM.Text = main.g_n32Si2DRcvdPkgs.ToString();//接收包数
            ////保存包数
            //txt_DRAWNUM.Text = main.g_n32Si2DDrawPkgs.ToString();//绘画包数
        }
        public void UpDataSi2DCFGAveMax()
        {
            try
            {
                MethodInvoker maxmin = new MethodInvoker(show_maxmin);
                if (this.IsDisposed)
                    return;
                else
                {
                    this.BeginInvoke(maxmin);
                }
               
            }
            catch
            {
                //return;
            }

        }
        public void show_maxmin()
        {
            txt_MAX.Text = g_n32Si2DMax.ToString();
            txt_MIN.Text = g_n32Si2DMin.ToString();
            txt_AVE.Text = g_n32Si2DAve.ToString();
            txt_PERC.Text = g_n64Si2DPercent.ToString();
            //txt_MAX.Text = main.g_n32Si2DDatMax.ToString();
            //txt_MIN.Text = main.g_n32Si2DDatMin.ToString();
            //txt_AVE.Text = main.g_n32Si2DDatAve.ToString();
            //txt_PERC.Text = main.g_n64Si2DDatPercent.ToString();
        }
        #endregion

        #region 更新操作日志
        public delegate void RichTextInvoke(string str);
        public void UpDatarichTextBoxTOPERATEMESSAGE(string str)
        {
            try
            {
                RichTextInvoke mi = new RichTextInvoke(showRich);
                if (this.IsDisposed)
                    return;
                else 
                this.BeginInvoke(mi,str);

            }
            catch
            {
                return;
            }
           
        }
        private void showRich(string str)
        {
            //richTextBoxT_OPERATEMESSAGE.Text = "";
            richTextBoxT_OPERATEMESSAGE.AppendText(str);
        }
        #endregion

        #region 查询
        private void btn_QUERY_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] l_byteBufQueryCorrectedSheet = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                                   0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                   0x00,0x00,0x00,0x00,0x06,0x06,0x00,0x00,0x00,
                                                                   0x00,0x00,0x00,0x00,0x1C,0xEE,0xEE};

                //if (main.g_SocketClient.Connected == false)
                if(main.serialPort1.IsOpen==false)
                {
                    //MessageBox.Show("网络已断开，请连接网络！", "Error");
                    MessageBox.Show("串口已关闭，请打开串口！", "Error");
                    return;
                }
                if (comb_CHSEL.Text == "前沿修正")
                {
                    l_byteBufQueryCorrectedSheet[24] = 0x01;
                    l_byteBufQueryCorrectedSheet[31] = ToolFunc.XorCheck_byte(2, l_byteBufQueryCorrectedSheet, 4);
                    //main.g_SocketClient.Send(l_byteBufQueryCorrectedSheet);
                    main.serialPort1.Write(l_byteBufQueryCorrectedSheet, 0, l_byteBufQueryCorrectedSheet.Length);
                }
                else if (comb_CHSEL.Text == "求反射率")
                {
                    l_byteBufQueryCorrectedSheet[24] = 0x02;
                    l_byteBufQueryCorrectedSheet[31] = ToolFunc.XorCheck_byte(2, l_byteBufQueryCorrectedSheet, 4);
                    //main.g_SocketClient.Send(l_byteBufQueryCorrectedSheet);
                    main.serialPort1.Write(l_byteBufQueryCorrectedSheet, 0, l_byteBufQueryCorrectedSheet.Length);
                }
                else
                {
                    MessageBox.Show("请先选择模式！", "Warning");
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region 通道改变
        private void comb_CHSEL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comb_CHSEL .Text =="前沿修正")
            {
                main.g_n32Si2DCHSEL = 0;
                Si2DChselIndex = 0;
                //comb_reflective.Enabled = false;
                //btn_REVISE.Text = "开始";
                //btn_CACULATE.Enabled = false;
                //btn_IDOK4.Enabled = true;
                //btn_FITTING.Enabled = true;
                //btn_IDOK8.Text = "打开修正表";
            }
            else if (comb_CHSEL.Text == "计时修正")
            {
                main.g_n32Si2DCHSEL = 1;
                Si2DChselIndex = 1;
                ////comb_reflective.Enabled = true;
                //btn_REVISE.Text = "开始";
                //btn_CACULATE.Enabled = false;
                //btn_IDOK4.Enabled = false;
                //btn_FITTING.Enabled = false;
                //btn_IDOK8.Text = "打开计时表";
            }
            //MyXiuzhengDataSendThread.Abort();

        }
        #endregion 

        #region 选择单点保存路径

        string g_sFolderPath = "";
        string NetDataFilesPath = Environment.CurrentDirectory.ToString() + @"\NetDataFiles\";

        private void btn_singledatapath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = NetDataFilesPath;
            folderBrowserDialog1.Description = "请选择一个文件夹";
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    //记录选中的目录
                    g_sFolderPath = folderBrowserDialog1.SelectedPath + "\\";
                    btn_StartSave.Enabled = true;
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region 保存计算结果
        

        private void btn_IDOK2_Click(object sender, EventArgs e)
        {
            try
            {
                string file = txt_FILE_PATH.Text;
                string DirectoryName = Path.GetDirectoryName(file);
                string filenamewithoutextension = Path.GetFileNameWithoutExtension(file);
                string extension = Path.GetExtension(file);
               // string file1 = DirectoryName + filenamewithoutextension + "-handle" + extension;

                if (file != "")
                {
                    //if (comb_CHSEL.Text == "前沿修正")
                    {
                        FileStream fs = new FileStream(@file, FileMode.Append, FileAccess.Write);
                        StreamWriter sb = new StreamWriter(fs);
                        int l_nlen = main.g_n32LstPulseWidthToWrite.Count();
                        //int l_n32TimSum = 0;
                        int l_n32TimAve = 0;
                        for (int i = 0; i < l_nlen; i++)
                        {
                            l_n32TimAve += main.g_n32LTimingEdgeToWrite[i];
                        }
                        l_n32TimAve /= l_nlen;

                        for (int i = 0; i < l_nlen; i++)
                        {
                            sb.Write(main.g_n32LstPulseWidthToWrite[i] + "\t" + (main.g_n32LTimingEdgeToWrite[i] - l_n32TimAve) + "\r\n");
                        }
                        sb.Close();
                        fs.Close();

                       richTextBoxT_OPERATEMESSAGE.AppendText(filenamewithoutextension + "保存了" + l_nlen.ToString() + "组数据\r\n");
                    }
                    //else if (comb_CHSEL.Text == "求反射率")
                    //{
                    //    IWorkbook workbook = null;                     //新建IWorkbook对象
                    //    string fileName = txt_FILE_PATH.Text;
                    //    FileStream fileStream = new FileStream(@fileName, FileMode.Open, FileAccess.ReadWrite);
                    //    if (fileName.IndexOf(".xlsx") > 0)             //2007版本
                    //    {
                    //        workbook = new XSSFWorkbook(fileStream);   //xlsx数据读入workbook
                    //    }
                    //    else if (fileName.IndexOf(".xls") > 0)         //2003版本
                    //    {
                    //        workbook = new HSSFWorkbook(fileStream);   //xls数据读入workbook
                    //    }
                    //    ISheet sheet = workbook.GetSheetAt(0);        //获取第一个工作表
                    //    IRow row;                                     //新建当前工作表行数据
                    //    int l_n32len = main.g_n32LstPulseWidthToWrite.Count;
                    //    if (l_n32len > 0)
                    //    {
                    //        if (comb_reflective.Text == "8%")
                    //        {
                    //            for (int i = 3; i < l_n32len + 3; i++)    //对工作表每一行
                    //            {
                    //                row = sheet.GetRow(i);   //row读入第i行数据
                    //                if (row != null)
                    //                {
                    //                    ICell cellx0 = row.GetCell(0);
                    //                    cellx0.SetCellValue(main.g_n32LTimingEdgeToWrite[i - 3]);
                    //                    ICell cellx1 = row.GetCell(1);
                    //                    cellx1.SetCellValue(main.g_n32LstPulseWidthToWrite[i - 3]);
                    //                }
                    //            }
                    //        }

                    //        else if (comb_reflective.Text == "10%")
                    //        {
                    //            for (int i = 3; i < l_n32len + 3; i++)    //对工作表每一行
                    //            {
                    //                row = sheet.GetRow(i);   //row读入第i行数据
                    //                if (row != null)
                    //                {
                    //                    ICell cellx0 = row.GetCell(3);
                    //                    cellx0.SetCellValue(main.g_n32LTimingEdgeToWrite[i - 3]);
                    //                    ICell cellx1 = row.GetCell(4);
                    //                    cellx1.SetCellValue(main.g_n32LstPulseWidthToWrite[i - 3]);
                    //                }
                    //            }
                    //        }

                    //        else if (comb_reflective.Text == "30%")
                    //        {
                    //            for (int i = 3; i < l_n32len + 3; i++)    //对工作表每一行
                    //            {
                    //                row = sheet.GetRow(i);                //row读入第i行数据
                    //                if (row != null)
                    //                {
                    //                    ICell cellx0 = row.GetCell(6);
                    //                    cellx0.SetCellValue(main.g_n32LTimingEdgeToWrite[i - 3]);
                    //                    ICell cellx1 = row.GetCell(7);
                    //                    cellx1.SetCellValue(main.g_n32LstPulseWidthToWrite[i - 3]);
                    //                }
                    //            }
                    //        }

                    //        else if (comb_reflective.Text == "300%")
                    //        {
                    //            for (int i = 3; i < l_n32len + 3; i++)    //对工作表每一行
                    //            {
                    //                row = sheet.GetRow(i);                //row读入第i行数据
                    //                if (row != null)
                    //                {
                    //                    ICell cellx0 = row.GetCell(9);
                    //                    cellx0.SetCellValue(main.g_n32LTimingEdgeToWrite[i - 3]);
                    //                    ICell cellx1 = row.GetCell(10);
                    //                    cellx1.SetCellValue(main.g_n32LstPulseWidthToWrite[i - 3]);
                    //                }
                    //            }
                    //        }

                    //        using (fileStream = File.OpenWrite(@fileName))
                    //        {
                    //            workbook.Write(fileStream);    //向打开的这个xls文件中写入数据  
                    //        }
                    //        fileStream.Close();
                    //        workbook.Close();
                    //        MessageBox.Show(comb_reflective.Text + "反射率的数据保存成功！！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("反射率数据为空，请重新保存！！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    }

                    //}
 
                }
                else
                {
                    richTextBoxT_OPERATEMESSAGE.AppendText("文件名为空，无法打开文件，保存失败！\r\n");
                }
                
            }
            catch
            {
                return;
            }

        }
        #endregion 

        #region 清除一组
        private void btn_DELETE_Click(object sender, EventArgs e)
        {
            try
            {
                //main.g_nInterpNum = 0;
                int l_n32arraylen = main.g_n32LstPulseWidthToWrite.Count;
                string l_spulse = main.g_n32LstPulseWidthToWrite[l_n32arraylen - 1].ToString();
                if (l_n32arraylen >= 1)
                {
                    main.g_n32LstPulseWidthToWrite.RemoveAt(l_n32arraylen - 1);
                    main.g_n32LstStdSubRisingEdgeToWrite.RemoveAt(l_n32arraylen - 1);
                    main.g_n32LTimingEdgeToWrite.RemoveAt(l_n32arraylen - 1);
                    //richTextBox_RICHEDIT_DATA.Text = "";
                    richTextBox_RICHEDIT_DATA.AppendText("已删除脉宽为：" + l_spulse + " 的数据！\r\n");
                }
                else
                {
                    richTextBox_RICHEDIT_DATA.AppendText("已删除最后一组数据！！\r\n");
                }
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region  开始
        private void btn_REVISE_Click(object sender, EventArgs e)
        {
            btn_CACULATE.Enabled = true;        //使能计算
             
            if (comb_CHSEL.Text == "前沿修正")
            {
               saveFileDialog1.FileName = "";
               saveFileDialog1.Filter = "H Files(*.txt_H)|*.txt_H";
               saveFileDialog1.ShowDialog();
            }
            else if (comb_CHSEL.Text == "求反射率")
            {
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "E Files(*.xlsx)|*.xlsx";
                saveFileDialog1.ShowDialog();
            }
            else
            {
                MessageBox.Show("请先选择操作模式！", "Warning");
                return;
            }
            if (saveFileDialog1.FileName != "")
            {
                txt_FILE_PATH.Text = saveFileDialog1.FileName;
                if (comb_CHSEL.Text == "求反射率")
                {
                    try
                    {
                        IWorkbook workbook = new XSSFWorkbook();                    //建立空白工作簿
                        ISheet sheet = workbook.CreateSheet("反射率数据");            //在工作簿中：建立空白工作表
                        IRow row = sheet.CreateRow(0);                              //在工作表中：建立行，参数为行号，从0计
                        ICell cell = row.CreateCell(0);                             //在行中：建立单元格，参数为列号，从0计
                        cell.SetCellValue("mini激光雷达反射率数据");              //设置单元格内容
                        ICellStyle style = workbook.CreateCellStyle();
                        style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //设置单元格的样式：水平对齐居中
                        IFont font = workbook.CreateFont();                         //新建一个字体样式对象
                        font.Boldweight = short.MaxValue;                           //设置字体加粗样式
                        style.SetFont(font);                                        //使用SetFont方法将字体样式添加到单元格样式中 
                        cell.CellStyle = style;                                     //将新的样式赋给单元格
                        row.Height = 30 * 20;                                       //设置单元格的高度
                        //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                        NPOI.SS.Util.CellRangeAddress region = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 10);//设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                        sheet.AddMergedRegion(region);

                        IRow row1 = sheet.CreateRow(1);
                        row1.CreateCell(0).SetCellValue("8%");
                        row1.CreateCell(1);
                        row1.CreateCell(2);
                        row1.CreateCell(3).SetCellValue("10%");
                        row1.CreateCell(4);
                        row1.CreateCell(5);
                        row1.CreateCell(6).SetCellValue("30%");
                        row1.CreateCell(7);
                        row1.CreateCell(8);
                        row1.CreateCell(9).SetCellValue("300%");
                        row1.CreateCell(10);

                        IRow row2 = sheet.CreateRow(2);
                        row2.CreateCell(0).SetCellValue("计时值");
                        row2.CreateCell(1).SetCellValue("脉宽");
                        row2.CreateCell(2);
                        row2.CreateCell(3).SetCellValue("计时值");
                        row2.CreateCell(4).SetCellValue("脉宽");
                        row2.CreateCell(5);
                        row2.CreateCell(6).SetCellValue("计时值");
                        row2.CreateCell(7).SetCellValue("脉宽");
                        row2.CreateCell(8);
                        row2.CreateCell(9).SetCellValue("计时值");
                        row2.CreateCell(10).SetCellValue("脉宽");

                        for (int i = 0; i < 500; i++)
                        {
                            IRow row3 = sheet.CreateRow(i + 3);
                            row3.CreateCell(0);
                            row3.CreateCell(1);
                            row3.CreateCell(2);
                            row3.CreateCell(3);
                            row3.CreateCell(4);
                            row3.CreateCell(5);
                            row3.CreateCell(6);
                            row3.CreateCell(7);
                            row3.CreateCell(8);
                            row3.CreateCell(9);
                            row3.CreateCell(10);
                        }
                        FileStream file2007 = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                        workbook.Write(file2007);
                        file2007.Close();
                        workbook.Close();
                        MessageBox.Show("Excel表格创建成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Excel表格创建出错，请重新操作", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            main.g_n32StdRisingEdgeDat = 0; //上升沿的第一包数据
            if (btn_REVISE.Text == "重新开始")
            {
                DialogResult ds1 = MessageBox.Show("    是否要重新开始!", "提示", MessageBoxButtons.OKCancel);
                if (ds1 == DialogResult.OK)
                {
                    main.g_n32ReviseCalculationTimes = 0;
                    richTextBoxT_OPERATEMESSAGE.AppendText(DateTime.Now.ToString() + "\r\n");
                    if (comb_CHSEL.Text == "前沿修正")
                    {
                        richTextBox_RICHEDIT_DATA.AppendText("----重新开始前沿修正----\r\n");
                    }
                    else if (comb_CHSEL.Text == "求反射率")
                    {
                        richTextBox_RICHEDIT_DATA.AppendText("----重新开始求反射率----\r\n");
                    }
                    if (btn_CACULATE.Enabled == false)
                        btn_CACULATE.Enabled = true;
                    main.g_n32LstPulseWidthToWrite.Clear();
                    main.g_n32LstStdSubRisingEdgeToWrite.Clear();
                    main.g_n32LTimingEdgeToWrite.Clear();
                }
                else
                {
                    return;
                }
            }
            else
            {
                richTextBox_RICHEDIT_DATA.Text = "";
                richTextBoxT_OPERATEMESSAGE.Text = "";
                richTextBoxT_OPERATEMESSAGE.AppendText(DateTime.Now.ToString() + "\r\n");
                if (comb_CHSEL.Text == "前沿修正")
                {
                    richTextBox_RICHEDIT_DATA.AppendText("----开始前沿修正----\r\n");
                }
                else if (comb_CHSEL.Text == "求反射率")
                {
                    richTextBox_RICHEDIT_DATA.AppendText("----开始求反射率----\r\n");
                }
                btn_REVISE.Text = "重新开始";
                if (btn_CACULATE.Enabled == false)
                    btn_CACULATE.Enabled = true;
            }
           
        }
        #endregion

        #region 打开修正数据（读数据）
        private void btn_IDOK4_Click(object sender, EventArgs e)
        {
            try
            {
                string filepath = "";
                int i = 0;
                if (comb_CHSEL.Text == "前沿修正")
                {
                    openFileDialog2.FileName = "";
                    openFileDialog2.Filter = "H Files(*.txt_H)|*.txt_H";
                    openFileDialog2.ShowDialog();
                    if (openFileDialog2.FileName != "")
                    {
                        filepath = openFileDialog2.FileName;
                    }
                }
                else if (comb_CHSEL.Text == "计时修正")
                {
                    openFileDialog2.FileName = "";
                    openFileDialog2.Filter = "L Files(*.txt_L)|*.txt_L";
                    openFileDialog2.ShowDialog();
                    if (openFileDialog2.FileName != "")
                    {
                        filepath = openFileDialog2.FileName;
                    }
                }
                //else if (comb_CHSEL.Text == "求反射率")
                //{
                //    openFileDialog2.FileName = "";
                //    openFileDialog2.Filter = "F Files(*.txt)|*.txt";
                //    openFileDialog2.ShowDialog();
                //    if (openFileDialog2.FileName != "")
                //    {
                //        filepath = openFileDialog2.FileName;
                //    }
                //}
                else
                {
                    MessageBox.Show("模式选择错误！！", "Warning");
                }
                if (filepath != "")
                {
                    richTextBox_RICHEDIT_DATA.Text = "";
                    main.g_n32LstPulseWidthReadFromFile.Clear();
                    main.g_n32LstStdSubRisingEdgeReadFromFile.Clear();
                    richTextBox_RICHEDIT_DATA.AppendText("高-低                基-低\r\n");
                    FileStream fs = File.OpenRead(filepath);
                    StreamReader sr = new StreamReader(fs, Encoding.Default);
                    fs.Seek(0, SeekOrigin.Begin);
                    while (sr.Peek() > -1)
                    {
                        string data = sr.ReadLine();
                        richTextBox_RICHEDIT_DATA.AppendText(data + "\r\n");
                        string strdata1 = MergeSpace(data);
                        string[] strdata = strdata1.Split(' ');
                        main.g_n32LstPulseWidthReadFromFile.Add(Convert.ToInt32(strdata[0]));
                        main.g_n32LstStdSubRisingEdgeReadFromFile.Add(Convert.ToInt32(strdata[1]));
                        i++;
                    }
                    //main.g_nInterpNum = i;
                    sr.Close();
                    fs.Close();
                }
                else
                {
                    richTextBoxT_OPERATEMESSAGE.AppendText("打开文件失败，请先选择要打开的文件！！！"); //MessageBox.Show("打开文件失败！", "Error");
                    return;
                }
                richTextBox_RICHEDIT_DATA.AppendText("**********分隔符***********\r\n");
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region 发送
        int g_n32ChselTyte = 2;
        byte[] g_abSingleDataControl = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,    //单点开始/停止要数
                                                    0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};
        private void btn_SEND_DATA_Click(object sender, EventArgs e)
        {
            try
            {
                if(main.serialPort1.IsOpen)
                {
                    if (CorrectedSheetFilePath != "")
                    {
                        btn_SEND_DATA.Text = "发送中";

                        richTextBoxT_OPERATEMESSAGE.Text = "";
                        if (comb_CHSEL.Text == "前沿修正")
                        {
                            richTextBoxT_OPERATEMESSAGE.AppendText("修正表数据发送中" + "\r\n");
                            g_n32ChselTyte = 0;
                        }
                        else if (comb_CHSEL.Text == "计时修正")
                        {
                            richTextBoxT_OPERATEMESSAGE.AppendText("计时修正数据发送中" + "\r\n");
                            g_n32ChselTyte = 1;
                        }
                        //暂停发送单点数据
                        g_abSingleDataControl[25] = 0x00;
                        g_abSingleDataControl[31] = ToolFunc.XorCheck_byte(2, g_abSingleDataControl, 4);
                        main.serialPort1.Write(g_abSingleDataControl, 0, g_abSingleDataControl.Length);

                        //发送数据
                        MyXiuzhengDataSendThread = new Thread(Si2DXiuZhengDataSend);
                        MyXiuzhengDataSendThread.IsBackground = true;
                        MyXiuzhengDataSendThread.Start();

                    }
                    else
                    {
                        MessageBox.Show("请先打开文件！", "Error");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("串口已关闭，请先打开串口！", "Error");
                    return;
                }

                
            }
            catch
            {
                return;
            }

        }

        private void Si2DXiuZhengDataSend()
        {
            try
            {
                int l_n32totalnum = 0;   //修正表中数据总个数
                int l_n32rowdatnum = 0;  //修正表中每一行数据个数
                int l_n32pkglen=0;       //包长度
                int l_n32pkgno = 1;      //数据分包发送的包序号

                FileInfo fi = new FileInfo(CorrectedSheetFilePath);
                if (fi.Length == 0)
                {
                    MessageBox.Show("文件中无数据！", "Error");
                    return;
                }

                FileStream fs = File.OpenRead(CorrectedSheetFilePath);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                fs.Seek(0, SeekOrigin.Begin);
                while (sr.Peek() > -1)
                {
                    string l_strdata = MergeSpace(sr.ReadLine());
                    string[] l_strbufdata = l_strdata.Split(' ');
                    if (g_n32ChselTyte == 0)
                    {
                        l_n32rowdatnum = l_strbufdata.Length - 1;         //会多读取一个""
                        for (int i = 0; i < l_n32rowdatnum; i++)
                        {
                            g_n32BufCorrectedDat[l_n32totalnum + i] = Convert.ToInt32(l_strbufdata[i]);
                        }
                    }
                    else if (g_n32ChselTyte == 1)
                    {
                        l_n32rowdatnum = l_strbufdata.Length - 2;         //会多读取两个""
                        for (int i = 0; i < l_n32rowdatnum; i++)
                        {
                            g_n32BufCorrectedDat[l_n32totalnum + i] = Convert.ToInt32(l_strbufdata[i+1]);
                        }
                    }
                    if (l_n32rowdatnum != 0)
                    {
                        l_n32totalnum += l_n32rowdatnum;
                        if (l_n32rowdatnum != 5)
                            break; //对应while (sr.Peek() > -1)
                    }
                }
                sr.Close();
                fs.Close();

                while (true)
                {
                    l_n32pkglen = PkgSplitFunc(g_n32ChselTyte, g_byteBufCorrectedPkg, g_n32BufCorrectedDat, l_n32totalnum, l_n32pkgno);//从第1包发送(每次发2000字节？？)
                    if (l_n32pkglen != 0)
                    {
                        if(main.serialPort1.IsOpen)
                        {
                            byte[] l_byteBufPkgDat = new byte[l_n32pkglen + 4];                                  //发送给下位机的分包数据 
                            Array.Copy(g_byteBufCorrectedPkg, l_byteBufPkgDat, l_n32pkglen);
                            l_byteBufPkgDat[l_n32pkglen + 1] = ToolFunc.XorCheck_byte(2, l_byteBufPkgDat, 0);    //校验位
                            l_byteBufPkgDat[l_n32pkglen + 2] = 0xEE;                                             //包尾
                            l_byteBufPkgDat[l_n32pkglen + 3] = 0xEE;                                             //包尾
                            main.serialPort1.Write(l_byteBufPkgDat, 0, l_byteBufPkgDat.Length);
                        }
                    }
                    else
                    {
                        break;  //发送完 结束
                    }

                    //事件提示发送成功，失败，超时。。。
                    bool result = false;
                    result = Si2DSendResetEvent.WaitOne(10000);//
                    if (result)
                    {
                        if (main.g_bCorrectionSuccessFlg == true && g_n32Si2DCorrectedPkgNo == l_n32pkgno)  //发送成功
                        {
                            l_n32pkgno++;
                            main.g_bCorrectionSuccessFlg = false;
                            g_n32RetryNum = 0;
                        }
                        else
                        {
                            string str = "第" + g_n32Si2DCorrectedPkgNo.ToString() + "包数据烧写失败，请重新烧写！\r\n";
                            UpDatarichTextBoxTOPERATEMESSAGE(str);
                            if (g_n32RetryNum > 2)
                            {
                                MethodInvoker mj = new MethodInvoker(btntext_change);
                                this.BeginInvoke(mj);
                                return ;
                            }
                            g_n32RetryNum++;
                        }
                    }
                    else
                    {
                        string str = "超时未收到第" + l_n32pkgno.ToString() + "包回复\r\n";
                        UpDatarichTextBoxTOPERATEMESSAGE(str);
                    }
                    Thread.Sleep(500);
                }
                MethodInvoker mi = new MethodInvoker(send_success);
                this.BeginInvoke(mi);


            }
            catch
            {
                
                MethodInvoker mj = new MethodInvoker(btntext_change);
                this.BeginInvoke(mj);
                return;
            }

        }

        private void send_success()
        {
            //开始发送单点数据
            g_abSingleDataControl[25] = 0x01;
            g_abSingleDataControl[31] = ToolFunc.XorCheck_byte(2, g_abSingleDataControl, 4);
            main.serialPort1.Write(g_abSingleDataControl, 0, g_abSingleDataControl.Length);

            MessageBox.Show("数据烧写发送成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btn_SEND_DATA.Text = "发送";
            MyXiuzhengDataSendThread.Abort();

        }
        private void btntext_change()
        {
            //MessageBox.Show("数据烧写发送成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btn_SEND_DATA.Text = "发送";
        }

        private int PkgSplitFunc(int index, byte[] datatosendpacket, int[] sourcedatapacket, int len, int num)
        {
            byte[] l_byteBufCorrectedPkg = new byte[26] {0xFF,0xAA,0x00,0x26,0x00,0x00,0x00,0x00,0x00,            //修正数据头
                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                         0x00,0x00,0x00,0x00,0x06,0x07,0x00,0x00};
            int i = 0;
            int l_n32CopyIndex = 512 * (num - 1);                          //分包时每包数据从修正表中读取的数据缓存中（sourcedatapacket）开始复制的位置
            int l_n32PkgRealLen = 0;                                       //分包后每包数据长度，不包含校验位和包尾
            int l_n32TotalPkgs = (len % 512 == 0) ?
                                 (len / 512) : (len / 512 + 1);            //修正表中的数据总共可以分为几包

            if (l_n32CopyIndex > len)
                return 0;
            Array.Copy(l_byteBufCorrectedPkg, datatosendpacket, 26);
            if (index == 0)
            {
                datatosendpacket[24] = 0x01;                                  //修正表
            }
            else if (index == 1)
            {
                datatosendpacket[24] = 0x02;                                  //低阈值
            }

            datatosendpacket[26] = Convert.ToByte(l_n32TotalPkgs >> 8);
            datatosendpacket[27] = Convert.ToByte(l_n32TotalPkgs & 0xff);    //总包数
            datatosendpacket[28] = Convert.ToByte(num >> 8);
            datatosendpacket[29] = Convert.ToByte(num & 0xff);                //包号
            for (i = 0; i < 512 && (l_n32CopyIndex + i) < len; i++)
            {
                byte[] l_byteBuf = BitConverter.GetBytes(sourcedatapacket[l_n32CopyIndex + i]);
                datatosendpacket[35 + i * 2] = l_byteBuf[1];                 //数据高位
                datatosendpacket[34 + i * 2] = l_byteBuf[0];                 //数据低位
            }
            datatosendpacket[30] = Convert.ToByte((i * 2) >> 8);
            datatosendpacket[31] = Convert.ToByte((i * 2) & 0xff);           //数据长度(帧数据中包含的修正表的数据个数)
            datatosendpacket[32] = 0x55;                                     //标识符
            datatosendpacket[33] = 0xaa;                                     //标识符
            l_n32PkgRealLen = i * 2 + 34;                                    //帧长（不包含帧头帧尾）
            datatosendpacket[2] = Convert.ToByte((l_n32PkgRealLen >> 8) & 0xFF);
            datatosendpacket[3] = Convert.ToByte(l_n32PkgRealLen & 0xff);
            return l_n32PkgRealLen;
        }
        #endregion

        #region 拟合
        private void btn_FITTING_Click(object sender, EventArgs e)
        {
            try
            {
                main.OpenInterpform();
            }
            catch
            {
 
            }

        }
        #endregion

        #region 字符串中多个连续空格转为一个空格
        /// <summary>
        /// 字符串中多个连续空格转为一个空格
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>合并空格后的字符串</returns>
        public static string MergeSpace(string str)
        {
            if (str != string.Empty &&
                str != null &&
                str.Length > 0
                )
            {
                str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(str, " ");
            }
            return str;
        }


        #endregion

        private void SI2DCFG_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;//保证窗口不被关闭
        }

        private void txt_sicalno_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(txt_sicalno.Text);
                main.g_n32SiCalNo = index;
            }
            catch
            {
            }
        }

        #region 反射率改变  
        public int g_n32comb_reflectiveindex = 0;
        private void comb_reflective_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (g_n32comb_reflectiveindex != comb_reflective.SelectedIndex)
            {
                DialogResult dr = MessageBox.Show("是否保存了计算结果？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    //清除需要保存的数据重新计算
                    main.g_n32LstPulseWidthToWrite.Clear();
                    main.g_n32LstStdSubRisingEdgeToWrite.Clear();
                    main.g_n32LTimingEdgeToWrite.Clear();
                    g_n32comb_reflectiveindex = comb_reflective.SelectedIndex;
                }
                else
                {
                    comb_reflective.SelectedIndex = g_n32comb_reflectiveindex;
                    return;
                }
            }
            else
            {
                return;
            }
           
        }
        #endregion

        #region 播放单点历史数据
        string selectpath = "";
        private void btn_replay_Click(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region 选择历史数据文件夹
        private void btn_selecthisdata_Click(object sender, EventArgs e)
        {
            try
            {
                //g_timer1.Stop();
                //showindex = 0;
                //main.Clear_JI();
                btn_replay.Text = "播放";
                folderBrowserDialog2.SelectedPath = NetDataFilesPath;
                folderBrowserDialog2.Description = "请选择需要播放的文件";
                if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
                {
                    selectpath = folderBrowserDialog2.SelectedPath;
                }

                if (selectpath != "")
                {
                    DialogResult dr = MessageBox.Show(selectpath, "所选文件夹", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        //richTextBoxT_OPERATEMESSAGE.Text += "界面操作 点击\'选择\'按钮,并选择了文件夹：\n" + selectpath + "\r\n";
                    }
                    else
                    {
                        return;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(selectpath);
                    FileInfo[] RisingFiles = dir.GetFiles("*.txt_R");
                    FileInfo[] FallingFiles = dir.GetFiles("*.txt_F");
                    FileInfo[] TimingFiles = dir.GetFiles("*.txt_T");
                    //wjfilesnum = wjFiles.Length;
                    //richTextBox1.Text += DateTime.Now.ToString() + "\r\n" + "选择数据文件夹，共有" + wjfilesnum.ToString() + "条数据" + "\r\n";
                    //string readpath = selectpath + "\\" + wjFiles[0].ToString();
                    ////ScanArrayJiYToDraw = WriteOrReadFiles.Readtxt(readpath, ScanDrawDataLength);// readtxt(readpath);
                    //List<string> ListDataReadFromFile = WriteOrReadFiles.ReadDataFromFileByStreamOneLine(readpath);
                    //for (int i = 0; i < ListDataReadFromFile.Count; i++)
                    //{
                    //    ScanArrayJiYToDraw[i] = Convert.ToDouble(ListDataReadFromFile[i]);
                    //}
                    //PolarData2RecData(ScanArrayJiYToDraw);
                    //main.Draw_JI(ScanArrayJiYToDraw);
                    //main.Draw_ZHI();
                    //ArrayFileInfo.Clear();
                    //ArrayFileInfo.AddRange(wjFiles);
                    //txt_jump.Text = 0.ToString();
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

    }
}
