#define SINGLEMEASURE
//#undef  SINGLEMEASURE

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.IO.Ports;


namespace fzjgld
{
    public partial class Form1 : Form
    {
        #region 参数定义

        System.Windows.Forms.Timer g_timer1;
        System.Windows.Forms.Timer g_timer2;

        public int g_n32TimerInterval = 100;                                 //定时器时间间隔
        public int g_n32Timer1RunTimes = 0;                                  //定时器1执行的次数
        List<byte> g_byteLstXPkgsScanDat = new List<byte>();                 //扫描数据
       
        public static int g_n32MAX_BLOCK=10000;
        public byte[] g_byteArrayNetRcvdDat = new byte[g_n32MAX_BLOCK * 10];  //接收数据缓存到此数组中
        
        struct StructDataRecord
        {
            public int m_u32in;
            public int m_u32out;
            public int m_u32size;
        }
        StructDataRecord g_sNetBuf = new StructDataRecord();

        public int  g_n32XorFlg = 0;                          //校验标志位
        public int g_n32NetRcvInd=0;                          //二维数组的行序号
        public const int g_n32NetBufRowNum = 100;             //二维数组行数
        public const int g_n32NetBufColumnNum = 12000;
        public byte[,] g_byteArrayNetRcvBuf = new byte[g_n32NetBufRowNum, g_n32NetBufColumnNum];  //二维数组放置解析数据
        public int[] g_n32ArrayNetRcvSize = new int[g_n32NetBufRowNum];                           //缓存二维数组中每行数据的字节个数
        public int g_n32ScanDatRcvdFrames = 0;           //扫描数据接收的帧数（每2帧为一圈扫描数据）    
        public int g_n32ScanDatCorrectPkgs = 0;          //扫描数据正确的包数（每2包连续包序号的数据为一包正确数据）
        public int g_n32NetRcvdPkgs = 0;                 //网络接收的包个数
        public double g_n64CenAngle = 45;                //直角坐标系零点横坐标
        public int g_n32MaxDis = 50000;                  //最大测距50米
        public int g_n32SaveScanDatPkgs = 0;             //保存扫描包数


        public const int g_n32DrawDatLen = 271;
        public double[] g_n64ArrayJiX = new double[g_n32DrawDatLen];         //极坐标x轴点
        public double[] g_n64ArrayJiY = new double[g_n32DrawDatLen];         //极坐标y轴点
        public double[] g_n64ArrayRegionZhiX = new double[g_n32DrawDatLen];  //划分区域直角坐标y轴点
        public double[] g_n64ArrayRegionZhiY = new double[g_n32DrawDatLen];  //划分区域直角坐标y轴点
        public int[]    g_an32Reflec = new int[g_n32DrawDatLen];             //反射率数据

        ScanDataSet ScanSetForm;
        Record20PointsDistance RecordDisForm;
        Login LoginForm;
        ModifyPassword MfPasswordForm;
        FactorReset FactoryResetForm;
        public int g_n32ScanDatSetFrmFlg = 0;                 //扫描参数配置窗体打开则为1，关闭则为零
        public int g_n32ScanDatHeadFlg = 1;                   //扫描数据包序号
        public int[] g_n32ArrayScanPtDat = new int[g_n32DrawDatLen];     //扫描点数据一包数据对应1080个点
        public int g_n32ScanDatMaxVal = 0;                    //扫描数据最大值
        public int g_n32ScanDatMinVal = 0;                    //扫描数据最小值
        public int g_n32ScanDatSumVal = 0;                    //扫描数据的和
        public double g_n64ScanDatAvgVal = 0;                 //扫描数据平均值
        public double g_n64ScanDatPercent = 0;                //扫描数据在离散度范围内的百分比(概率)
        public int g_n32ScanPtToMeasure = 0;                  //测量第几个点
        public int g_n32ScanPkgsToCalcAvg = 0;                //计算平均值的包数，文本框输入
        public int g_n32ScanDatDispersion = 0;                //扫描数据的离散度
        public int g_n32RcvdScanPkgsToCalcAvg = 0;            //当此值=进行计算平均值的包数时计算平均值  缩写：RCVD;received,calc:calculate
        public int g_n32ScanPkgsInDispersion = 0;             //扫描数据在离散度范围内的包数                   avg:average pkgs:packages
        public int g_n32CorrectPkgsofScanDat = 0;             //计算最大值最小值平均值离散度概率的总包数       flag:flg   list:lst
        public int g_n32ScanIntervalPkgsToDraw = 0;           //屏显间隔包数由文本框输入得到（每g_n32ScanIntervalPkgsToDraw包做一次图）
        public int g_n32RcvdScanPkgsToDraw = 0;               //接收到的绘画包数当g_n32RcvdScanPkgsToDraw>=g_n32ScanIntervalPkgsToDraw时作图显示
        public int g_n32DrawPkgssOfJi = 0;                    //极坐标绘画包数
        public int g_n32DrawPkgsOfRegion = 0;                 //区域划分绘画包数

        #region 参数查询指令
        /**********参数查询********************/

        //应用设置基本参数查询－0601H
        public byte[] g_abQueryBasicPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                        0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                        0x00,0x00,0x00,0x00,0x06,0x01,0x00,0x00,0x00,
                                                        0x00,0x00,0x00,0x00,0x1C,0xEE,0xEE};
        //应用设置基本参数设置-0602H
        public byte[] g_abSetBasicPARM = new byte[42];
        //重启设备-0603H
        public byte[] g_abRestartEquip = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                      0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                      0x00,0x00,0x00,0x00,0x06,0x03,0x00,0x00,0x00,
                                                      0x00,0x00,0x00,0x00,0x1E,0xEE,0xEE};
        //修正参数查询指令－0604
        public byte[] g_abQueryCorrectedPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                            0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                            0x00,0x00,0x00,0x00,0x06,0x04,0x00,0x00,0x00,
                                                            0x00,0x00,0x00,0x00,0x19,0xEE,0xEE};

        //修正参数设置指令－0605
        public byte[] g_abSetCorrectedPARM = new byte[38];

        //网络参数查询指令－0608H
        public byte[] g_abQueryNetPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                      0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                      0x00,0x00,0x00,0x00,0x06,0x08,0x00,0x00,0x00,
                                                      0x00,0x00,0x00,0x00,0x1D,0xEE,0xEE};

        //功能设置参数查询指令－0701H
        public byte[] g_abQueryFunctionalPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                             0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                             0x00,0x00,0x00,0x00,0x07,0x01,0x00,0x00,0x00,
                                                             0x00,0x00,0x00,0x00,0x1D,0xEE,0xEE};
        //功能设置参数设置-0702H
        public byte[] g_abSetFunctionalPARM = new byte[43];
        //开启/关闭加热-0703H
        public byte[] g_abHeatEquip = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                   0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                   0x00,0x00,0x00,0x00,0x07,0x03,0x00,0x00,0x00,
                                                   0x00,0x00,0x00,0x00,0x1D,0xEE,0xEE};
        //APD参数查询指令－0704H
        public byte[] g_abQueryAPDPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                      0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                      0x00,0x00,0x00,0x00,0x07,0x04,0x00,0x00,0x00,
                                                      0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};
        //APD参数设置指令－0705H
        public byte[] g_abSetAPDPARM = new byte[36];
        //数据源选择-0706H
        public byte[] g_abSelDataSource = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                       0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                       0x00,0x00,0x00,0x00,0x07,0x06,0x00,0x00,0x00,
                                                       0x00,0x00,0x00,0x00,0x1D,0xEE,0xEE};

        //保存出厂设置参数－0708H
        public byte[] g_abSaveFactoryPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                         0x00,0x00,0x00,0x00,0x07,0x08,0x00,0x00,0x00,
                                                         0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};
        //恢复出厂设置参数－0709H
        public byte[] g_abResetFactoryPARM = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                         0x00,0x00,0x00,0x00,0x07,0x09,0x00,0x00,0x00,
                                                         0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};
        ////灰尘检测单板测试
        //private byte[] g_abDustTest = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
        //                                           0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
        //                                           0x00,0x00,0x00,0x00,0x04,0x03,0x00,0x00,0x00,
        //                                           0x00,0x00,0x00,0x00,0x18,0xEE,0xEE};

        public byte[] g_abQueryBANKNO= new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                    0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x09,0x01,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x1D,0xEE,0xEE};
        /*******************************************************/

        #endregion

        SI2DCFG Single2DConfigure;

        public int g_n32Si2DWaveFrmFlg = 0;
        public int g_n32Si2DConfigFrmFlg = 0;             //是否打开单点数据配置界面:0代表没打开，1代表打开
        public int g_n32Si2DDatType = 0;                  //单点数据类型：1：上升沿数据，2：下降沿数据，3：计时数据
        public int g_n32Si2DIntervalPkgsToDraw = 5;       //每路屏显间隔包数，由文本框输入得到
        public int g_n32Si2DPkgsToCalc = 100;             //计算脉宽、基准-上升沿的包数（由文本框输入得到）
        public int g_nSi2DOneChart = 100;                 //每路屏显包数

        public int g_n32RisingEdgeRcvdPkgs = 0;           //接收到的上升沿数据包数g_n32RisingEdgeRcvdPkgs % g_n32Si2DIntervalPkgsToDraw == 0时绘图
        public int g_n32FallingEdgeRcvdPkgs = 0;          //接收到的下降沿数据包数
        public int g_n32TimingRcvdPkgs = 0;               //接收到的计时数据包数
        public int g_n32Si2DRcvdPkgs = 0;                 //接收到的单点数据总包数，包括上升沿数据、下降沿数据和计时数据的包数
        public int g_n32Si2DDrawPkgs = 0;                 //单点数据绘图包数，包括上升沿数据、下降沿数据和计时数据的包数

        //public bool g_bSingleCalDis = false;               //单点测距
        public int g_n32Si2DPtToMeasure = 0;              //单点数据中要进行最大值最小值平均值计算的点
        public int g_n32Si2DCalIndex = 0;
        public int g_n32Si2DDatSum = 0;                   //单点数据和
        public int g_n32Si2DDatMin = 9999999;             //单点数据最小值
        public int g_n32Si2DDatMax = 0;
        public int g_n32Si2DDatAve = 0;
        public double g_n64Si2DDatPercent = 0;            //单点概率
        public int g_n32SiDispersion = 0;                 //离散度
        public int g_n32SiErrCnt = 0;                     //不在离散度范围内的数据
        public int g_n32SiCorrCnt = 0;                    //在离散度范围内的包数
        public int g_n32SiCalNum= 0;
        public int g_n32SiCalNo = 100;                    //计算平均值的包数

        public int g_n32PulseWidth = 0;                   //脉宽（一包数据中的平均值）  
        public int g_n32RisingEdgeDat = 0;                //一包数据中上升沿数据的和（一包数据中有360个上升沿数据） 
        public int g_n32FallingEdgeDat = 0;               //一包数据中下降沿沿数据的和（一包数据中有360个下降沿数据） 
        public int g_n32RisingEdgeAvg = 0;                //每n包的上升沿平均值 n=g_n32Si2DPkgsToCalc
        public int g_n32FallingEdgeAvg = 0;               //每n包的下降沿平均值
        public int g_n32TimingEdgeAvg = 0;
        public int g_n32RisingEdgeSum = 0;                //每n包的上升沿的和
        public int g_n32FallingEdgeSum = 0;               //每n包的下降沿的和
        public int g_n32TimingEdgeSum = 0;                //每n包计时的和
        public int g_n32PulseWidthAvg = 0;                //脉宽(多包数据的平均值)
        public int g_n32RcvdCalcPkgsOfRisingEdge = 0;     //计算上升沿的平均值的包数（上位机接收到的包数）点击计算按钮后此变量才会增加 g_n32RcvdCalcPkgsOfRisingEdge == g_n32Si2DPkgsToCalc
        public int g_n32RcvdCalcPkgsOfFallingEdge = 0;    //计算下降沿的平均值的包数（上位机接收到的包数）
        public int g_n32RcvdCalcPkgsOfTimingEdge = 0;

        public int g_n32ReviseCalculationTimes = 0;       //修正计算次数 将第一次修正计算得到的上升沿数据的平均值作为基准值
        public int g_n32StdRisingEdgeDat = 0;             //基准值
        public int g_n32StdSubRisingEdge = 0;             //基准-上沿

        public List<int> g_n32LstPulseWidthReadFromFile = new List<int>();            //缓存从文件中读取的多包数据的平均脉宽
        public List<int> g_n32LstStdSubRisingEdgeReadFromFile = new List<int>();      //缓存从文件中读取的基准减上升沿值
        public List<int> g_n32LstPulseWidthToWrite = new List<int>();                 //缓存计算脉宽然后写入文件中
        public List<int> g_n32LstStdSubRisingEdgeToWrite = new List<int>();           //缓存计算基准差值然后写入文件中
        public List<int> g_n32LTimingEdgeToWrite = new List<int>();                   //缓存计时值然后写入文件中

        public int g_n32Si2DRcvdPkgsForMax = 0;                   //计算单点数据最大值最小值平均值时的总包数
        public int g_n32Si2DXToDraw = 0;                          //单点数据作图的横坐标值
        public int g_n32Si2DXScalMax = 24000;                     //单点绘图x轴点的最大个数
        public int g_n32Si2DSaveDataNo = 0;                       //保存单点数据的包数

        public int g_n32RisingDat = 0;
        public int g_n32FallingDat = 0;
        public int g_n32TimingDat = 0;

        public List<int> g_listRisingDatY = new List<int>();
        public List<int> g_listFallingDatY = new List<int>();
        public List<int> g_listTimingDatY = new List<int>();


        public int g_n32Si2DCHSEL = 0;                   //修正时的通道选择
        public int g_n32CorrectionPackNo = 0;            //接收到下位机回复的修正数据的包号
        public bool g_bCorrectionSuccessFlg = false;     //修正数据发送成功标志位

        public bool g_bHeartStateFlg = false;            //心跳状态：定时接收到数据则为true;超时未收到为false
        //public bool g_bManualDisConnectFlg = false;      //手动断开连接
        public int g_n32RcvdLidarStateFrames = 0;        //设备运行状态接收到的次数
        //public int g_n32Timer2RunTimes = 0;              //定时器2定时的次数


        INTERP InterpForm;        

        public int g_n32MotorSpeedX = 0;                              //电机转速作图横坐标
        public bool g_bDrawMotorSpeedFlg = false;                     //电机转速绘图标志位
        public List<int> g_n32LstMototSpeedLinex = new List<int>();   //缓存电机转速横坐标
        public List<int> g_n32LstMototSpeedLiney = new List<int>();

        public bool g_bProdLoginFlg = false;             //生产人员
        public bool g_bResearchLoginFlg = false;         //研发人员
        public string g_sUserName = "生产人员";
        public int g_n32ArmPkgs = 0;                     //下位机发送的包数
        public string ErrorFileName = Environment.CurrentDirectory.ToString() + @"\OperationLog\" + " 错误内容" + ".txt";
        public FileStream ScanErrFS;
        public StreamWriter ScanErrSW;


        #endregion

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
        }

        #region 主窗体加载函数

        public string g_sVersionDate = "20190518";
        public string g_sFormName = "WLR-F718-V1.0-";
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = g_sFormName + g_sVersionDate;// +"-未登录";
            ScanSetForm = new ScanDataSet();
            Single2DConfigure = new SI2DCFG();
            InterpForm= new INTERP();

            g_n32BitMapWidth = pictureBox1.Width;
            g_n32BitMapHeight = pictureBox1.Height;
            g_epCanvasOrigin = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2); //画布原点在设备上的坐标(屏幕坐标原点)

            c_DrawCd.bitmap = new Bitmap(g_n32BitMapWidth, g_n32BitMapHeight);
            c_DrawCd.graphics = Graphics.FromImage(c_DrawCd.bitmap);
            c_DrawCd.PicBox = pictureBox1;
            c_DrawCd.g_n32Scale = g_n32Scale;
            c_DrawCd.g_epCanvasOrigin = g_epCanvasOrigin;

            cmb_selectcodetype.SelectedIndex = 0;
            tscomb_bank.SelectedIndex = 0;
            g_n32BankNo = 1;

            //tabPage1.Parent = null;
            //tabPage8.Parent = null;
            //tabPage3.Parent = null;
            //tabPage6.Parent = null;
            //tabPage4.Parent = null;
            //tabPage10.Parent = null;
            g_bResearchLoginFlg = true;
            btn_openSerial_app.Enabled = true;
            comb_serialbaudapp .Text ="115200";
            comb_serialbaudapp.Enabled = true;
            comb_serialport_app.Enabled = true;

            btn_updataopencom.Enabled = true;
            comb_updatacombaud.Enabled = true;
            comb_updatacomport.Enabled = true;

            ResearchSerialPort();              //查找串口
          
            btn_openSerial_app.Enabled = true;
            comb_output1.SelectedIndex = 0;
            comb_output2.SelectedIndex = 0;
            comb_output3.SelectedIndex = 0;
            comb_output4.SelectedIndex = 0;

            comb_input1.SelectedIndex = 0;
            comb_input2.SelectedIndex = 0;
            comb_input3.SelectedIndex = 0;
            comb_input4.SelectedIndex = 0;

            comb_LDpower.Items.Clear();
            for (int i = 0; i <= 40; i++)
            {
                comb_LDpower.Items.Add(i);
            }
        }
        #endregion

        #region 主窗体关闭
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen == true)
                {
                    serialPort1.Close();
                }

                if (serialPort2.IsOpen == true)
                {
                    serialPort2.Close();
                }
                if (btn_savezero.Text == "暂停保存")
                {
                    g_swZero.Write("\r\n\r\n零点最大值：" + g_n32maxzero.ToString() + "\r\n");
                    g_swZero.Write("\r\n零点最小值：" + g_n32minzero.ToString() + "\r\n");
                    g_swZero.Write("\r\n差值：" + g_n32zerodiff.ToString() + "\r\n");
                    g_swZero.Write("\r\n\r\n停止保存时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "\r\n");
                    g_swZero.Close();
                    g_fsZero.Close();
                    g_bsavezeroflag = false;
                }
                //Application.ExitThread();
                 System.Environment.Exit(0);
            }
            catch
            {
 
            }
        }
        #endregion

        #region 定时器1/2定时发送查询指令
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    g_n32Timer1RunTimes++;
                    if (g_n32Timer1RunTimes > 20)
                    {
                        //bank查询
                        g_abQueryBANKNO[31] = ToolFunc.XorCheck_byte(2, g_abQueryBANKNO, 4);
                        serialPort1.Write(g_abQueryBANKNO, 0, g_abQueryBANKNO.Length);

                        g_timer1.Stop();
                        g_n32Timer1RunTimes = 0;
                        return;
                    }

                    switch (g_n32Timer1RunTimes % 5)
                    {
                        case 0:
                            {
                                //应用设置基本参数查询(起始角)
                                serialPort1.Write(g_abQueryBasicPARM, 0, g_abQueryBasicPARM.Length);
                            }
                            break;
                        case 1:
                            {
                                //修正参数查询
                                serialPort1.Write(g_abQueryCorrectedPARM, 0, g_abQueryCorrectedPARM.Length);
                            }
                            break;
                        case 2:
                            {
                               //功能参数查询（高低整体偏移）
                                serialPort1.Write(g_abQueryFunctionalPARM, 0, g_abQueryFunctionalPARM.Length);
                            }
                            break;
                        case 3:
                            {
                                //APD参数查询
                                serialPort1.Write(g_abQueryAPDPARM, 0, g_abQueryAPDPARM.Length);
                            }
                            break;
                        case 4:
                            {
                                g_abQueryNetPARM[31] = ToolFunc.XorCheck_byte(2, g_abQueryNetPARM, 4);
                                serialPort1.Write(g_abQueryNetPARM, 0, g_abQueryNetPARM.Length); 
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    return;
                }
            }
            catch
            {
                //MessageBox.Show("定时器1发送错误！！！");
                return;
            }

        }
        int g_n32timer2no = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                g_n32timer2no++;

                if (g_n32timer2no % 20 == 0)
                {
                    if (g_bHeartStateFlg == false && g_bmanualopencom1)           //串口已打开但是没有接收到心跳
                    {
                        MethodInvoker mi = new MethodInvoker(closereopen_com1);
                        this.BeginInvoke(mi);
                    }
                    g_n32timer2no = 0;
                }
                if (g_n32timer2no % 10 == 0)
                {
                    
                    NetEvent_Query_LidarState();                                   //设备运行状态查询
                   
                    g_bHeartStateFlg = false;
                }
            }
            catch
            {
                //MessageBox.Show("定时器2错误！！！！");
                //return;
            }
        }

        private void SendHeartFrame()  //心跳
        {
            if(serialPort1.IsOpen)
            {
                byte[] l_abHeartStateFrame = new byte[34] {0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,
                                                           0x00,0x00,0x01,0x01,0x00,0x05,0x00,0x00,
                                                           0x00,0x00,0x00,0x00,0x00,0x00,0x05,0x04,
                                                           0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                           0xEE,0xEE};

                l_abHeartStateFrame[31] = ToolFunc.XorCheck_byte(2, l_abHeartStateFrame, 4);
                serialPort1.Write(l_abHeartStateFrame, 0, l_abHeartStateFrame.Length);
            }
        }

        private void NetEvent_Query_LidarState()  //设备运行状态查询
        {
            if(serialPort1.IsOpen)
            {
                byte[] l_abLidarStateFrame = new byte[34] {0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,
                                                           0x00,0x00,0x01,0x01,0x00,0x05,0x00,0x00,
                                                           0x00,0x00,0x00,0x00,0x00,0x00,0x05,0x01,
                                                           0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                           0xEE,0xEE};

                l_abLidarStateFrame[31] = ToolFunc.XorCheck_byte(2, l_abLidarStateFrame, 4);
                serialPort1.Write(l_abLidarStateFrame, 0, l_abLidarStateFrame.Length);
            }
        }
        private void show_comm()
        {
            progressBar1.Value = 0;
            label_Communication.Text = "TCP异常";
        }

        private void show_comm1()
        {
            progressBar1.Value = 99;
            label_Communication.Text = "TCP正常";
        }

        private void reopen_com1()
        {
            try
            {
                serialPort1.PortName = comb_serialport_app.Text;
                serialPort1.BaudRate = int.Parse(comb_serialbaudapp.Text);
                serialPort1.Open();
                btn_openSerial_app.Text = "关闭串口";
                progressBar1.Value = 99;
                label_Communication.Text = "串口已打开";
            }
            catch
            { }
        }

        private void closereopen_com1()
        {
            try
            {
                if (serialPort1 != null)
                {
                    if (serialPort1.IsOpen)
                    {
                        closing = true;                //正在关闭串口
                        while (Listening)
                            Application.DoEvents();
                        serialPort1.Close();
                        closing = false;              //关闭串口完成
                    }
                    serialPort1.PortName = comb_serialport_app.Text;
                    serialPort1.BaudRate = int.Parse(comb_serialbaudapp.Text);
                    serialPort1.Open();
                    btn_openSerial_app.Text = "关闭串口";
                    progressBar1.Value = 99;
                    label_Communication.Text = "串口已打开";
                    //RichTextBox_Invoke(richTextBox1, "串口已重新连接\r\n");
                }
            }
            catch
            { }
        }
        #endregion

        #region ScanDataSet界面相关

        #region 计算最大值最小值
        List<int> g_an32originalmaxmindata = new List<int>();    //先将计算最大值最小值的数据缓存到此变量
        private void Calculate_MaxMin()
        {
            MaxMinCalculate();
            if (g_n32ScanDatSetFrmFlg == 1)
            {
                UpdateScandataSetFormUI();
                if (ScanSetForm == null || ScanSetForm.IsDisposed)
                {
                    g_n32ScanDatSetFrmFlg = 0;
                }
            }
        }
        private void MaxMinCalculate()
        {
            g_n32CorrectPkgsofScanDat++;
            int n = 0;                    //测量点的位置     
            if (g_n32ScanPtToMeasure > 0)
            {
                n = g_n32ScanPtToMeasure - 1;
            }
            else
            {
                n = 0;
            }
            g_an32originalmaxmindata.Add(g_n32ArrayScanPtDat[n]);

        }
        #endregion
        int g_n32ARMSendNo = 0;
        private void UpdateScandataSetFormUI()
        {
            ScanSetForm.g_n32ScanCorrectPackageNum = g_n32ScanDatCorrectPkgs;
            ScanSetForm.g_n32ScanDrawPackageNum = g_n32DrawPkgssOfJi;
            ScanSetForm.SaveScanDataPackNum = g_n32SaveScanDatPkgs;
            ScanSetForm.g_n32SingleFrameCmdCnt = g_n32ScanDatRcvdFrames;  //上位机接收到的总包数
            ScanSetForm.g_n32ScanArmPkgs = g_n32ARMSendNo;                //下位机发送的包数
            //if (g_bReflectEnable)
            //{
            //    ScanSetForm.g_n32ScanFPGAPkgs = g_an32Reflec[g_n32ScanPtToMeasure]; //反射率//g_n32Error;//g_n32FPGAInterNo;                //下位机发送的包数
            //}
            ScanSetForm.UpDateScanDataSetForm1();                       //更新ScanDataSet界面

            if (g_n32RcvdScanPkgsToCalcAvg >= g_n32ScanPkgsToCalcAvg)
            {
                g_n32ScanDatMaxVal = g_an32originalmaxmindata[0];
                g_n32ScanDatMinVal = g_an32originalmaxmindata[0];
                g_n32ScanDatSumVal += g_an32originalmaxmindata[0];
                for (int i = 1; i < g_an32originalmaxmindata.Count; i++)
                {
                    g_n32ScanDatMaxVal = Math.Max(g_an32originalmaxmindata[i], g_n32ScanDatMaxVal);
                    g_n32ScanDatMinVal = Math.Min(g_an32originalmaxmindata[i], g_n32ScanDatMinVal);
                    g_n32ScanDatSumVal += g_an32originalmaxmindata[i];
                }
                g_n64ScanDatAvgVal = (g_n32ScanDatSumVal / g_n32CorrectPkgsofScanDat);

                for (int i = 0; i < g_an32originalmaxmindata.Count; i++)
                {
                    if ((g_n64ScanDatAvgVal - g_n32ScanDatDispersion) <= g_an32originalmaxmindata[i] &&
                        g_an32originalmaxmindata[i] <= (g_n64ScanDatAvgVal + g_n32ScanDatDispersion))
                    {
                        g_n32ScanPkgsInDispersion++;
                    }
                }
                g_an32originalmaxmindata.Clear();
                g_n64ScanDatPercent = (float)g_n32ScanPkgsInDispersion / g_n32CorrectPkgsofScanDat;
                g_n64ScanDatPercent = g_n64ScanDatPercent * 100;

                ScanSetForm.g_n32ScanMax = g_n32ScanDatMaxVal;
                ScanSetForm.g_n32ScanMin = g_n32ScanDatMinVal;
                ScanSetForm.g_n64ScanAve = g_n64ScanDatAvgVal;
                ScanSetForm.g_n64ScanPercent = g_n64ScanDatPercent;
                ScanSetForm.UpDateScanDataSetForm(); //更新ScanDataSet界面

                //g_n64ScanDatAvgVal = (g_n32ScanDatSumVal / g_n32CorrectPkgsofScanDat);

                g_n32RcvdScanPkgsToCalcAvg = 0;
                g_n32CorrectPkgsofScanDat = 0;
                g_n32ScanPkgsInDispersion = 0;
                g_n32ScanDatSumVal = 0;
            }

        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length; //Set the current caret position at the end
            richTextBox1.ScrollToCaret(); //Now scroll it automatically
        }
        private void btn_clearrichtextbox_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
        #endregion

        #region 记录20个点的测距值界面相关

        List<List<int>> g_al20pointdata = new List<List<int>>();     //记录20组数据

        public int[] g_an32ScanMax = new int[20];                    //扫描数据最大值
        public int[] g_an32ScanMin = new int[20];                    //扫描数据最小值
        public int[] g_an32ScanSum = new int[20];                    //扫描数据的和
        public double[] g_an64ScanAve = new double[20];              //扫描数据平均值
        public int[] g_an32PkgsInDispersion = new int[20];           //扫描数据在离散度范围内的包数                   avg:average pkgs:packages
        public double[] g_an64DatPercent = new double[20];           //扫描数据在离散度范围内的百分比(概率)
        public int[] g_an32PtToMeasure_R = new int[20];              //测量第几个点    _R表示与记录20点数据界面相关的参数
        public int g_n32PkgsToCalcAvg_R = 0;                        //一键测距计算平均值的包数，文本框输入
        public int g_n32DatDispersion_R = 0;                        //一键测距扫描数据的离散度
        public int g_n32CorrectPkgsofScanDat_R = 0;                 //计算最大值最小值平均值离散度概率的总包数       flag:flg   list:lst
       
        
        #region 20点界面相关

        private void RecordForm_relevant()
        {
            
            if (g_n32RecordFrmFlg == 1)
            {
                Calculate_MaxMinAve();         //计算
                UpdataRecordUi();             //更新界面
                if (RecordDisForm == null || RecordDisForm.IsDisposed)
                {
                    g_n32RecordFrmFlg = 0;
                }
            }
        }

        #endregion

        #region 计算最大值最小值平均值
        private void Calculate_MaxMinAve()
        {
            g_n32CorrectPkgsofScanDat_R++;
            for (int i = 0; i < 20; i++)
            {
                g_al20pointdata[i].Add(g_n32ArrayScanPtDat[g_an32PtToMeasure_R[i]]);
            }
           
        }
        #endregion

        #region 更新界面

        private void UpdataRecordUi()
        {
            if (g_n32CorrectPkgsofScanDat_R >= g_n32PkgsToCalcAvg_R)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < g_al20pointdata[i].Count; j++)
                    {
                        if (j == 0)
                        {
                            g_an32ScanMax[i] = g_al20pointdata[i][j];
                            g_an32ScanMin[i] = g_al20pointdata[i][j];
                            g_an32ScanSum[i] += g_al20pointdata[i][j];    //和
                        }
                        else
                        {
                            g_an32ScanMax[i] = Math.Max(g_al20pointdata[i][j], g_an32ScanMax[i]);
                            g_an32ScanMin[i] = Math.Min(g_al20pointdata[i][j], g_an32ScanMin[i]);
                            g_an32ScanSum[i] += g_al20pointdata[i][j];    //和
                        }
                    }

                    g_an64ScanAve[i] = (g_an32ScanSum[i] / g_n32CorrectPkgsofScanDat_R);

                    for (int j = 0; j < g_al20pointdata[i].Count; j++)
                    {
                        if ((g_an64ScanAve[i] - g_n32DatDispersion_R) <= g_al20pointdata[i][j] &&
                             g_al20pointdata[i][j] <= (g_an64ScanAve[i] + g_n32DatDispersion_R))
                        {
                            g_an32PkgsInDispersion[i]++;
                        }
                    }
                }

                for (int i = 0; i < 20; i++)
                {
                    g_an64DatPercent[i] = (float)g_an32PkgsInDispersion[i] / g_n32CorrectPkgsofScanDat_R;
                    g_an64DatPercent[i] = g_an64DatPercent[i] * 100;

                    g_al20pointdata[i].Clear();  //清除数据
                }

                if (!RecordDisForm.g_bRecordFlg)                //点击记录数据后不更新数据
                {
                    Array.Copy(g_an32ScanMax, RecordDisForm.g_an32ScanMax, 20);
                    Array.Copy(g_an32ScanMin, RecordDisForm.g_an32ScanMin, 20);
                    Array.Copy(g_an64ScanAve, RecordDisForm.g_an64ScanAve, 20);
                    Array.Copy(g_an64DatPercent, RecordDisForm.g_an64DatPercent, 20);
                    RecordDisForm.UpDateRecordForm();
                }

                g_n32CorrectPkgsofScanDat_R = 0;
                Array.Clear(g_an32PkgsInDispersion, 0, 20);
                Array.Clear(g_an32ScanSum, 0, 20);
                Array.Clear(g_an32ScanMax, 0, 20);
                Array.Clear(g_an32ScanMin, 0, 20);


            }
 
        }

        #endregion

        #endregion

        #region 参数读取下载按钮相关

        #region 应用配置参数读取
        private void btn_BASICPIB_READ_Click(object sender, EventArgs e)
        {
            try
            {
                //if (g_SocketClient.Connected == true)
                if(serialPort1.IsOpen)
                {
                    //g_SocketClient.Send(g_abQueryBasicPARM);
                    serialPort1.Write(g_abQueryBasicPARM, 0, g_abQueryBasicPARM.Length);
                    RichTextBox_Invoke(richTextBox1, "应用设置参数读取中...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "应用设置参数读取失败\r\n");
                return;
            }
        }
        #endregion

        #region 应用配置参数下载

       int g_n32drawstaangle = 0;
       int g_n32protocoltype = 0;
        private void btn_BASICPIB_WRITE_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    byte[] l_byteBufSetBasicParameterson = new byte[26] {0xFF,0xAA,0x00,0x26,0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x06,0x02,0x00,0x00};
                   
                    if (comb_drawstaangle.Text == "225")
                    {
                        g_n32drawstaangle = 0;
                        comb_scansta.Text = "225";
                    }
                    else if (comb_drawstaangle.Text == "315")
                    {
                        g_n32drawstaangle = 1;
                        comb_scansta.Text = "315";
                    }
                 
                    int l_n32ScanSta = Convert.ToInt16(txt_EDIT_SCAN_START.Text);//扫描起始角度
                    if (l_n32ScanSta > 149)
                    {
                        l_n32ScanSta = 149;
                        txt_EDIT_SCAN_START.Text = l_n32ScanSta.ToString();
                    }
                    if (l_n32ScanSta < 1)
                    {
                        l_n32ScanSta = 1;
                        txt_EDIT_SCAN_START.Text = l_n32ScanSta.ToString();
                    }
                    //int l_n32ScanEnd = Convert.ToInt16(txt_EDIT_SCAN_END.Text);  //扫描终止角度
                    Byte[] SCS = BitConverter.GetBytes(l_n32ScanSta);
                   // Byte[] SCE = BitConverter.GetBytes(l_n32ScanEnd);
                    Byte[] DSA = BitConverter.GetBytes(g_n32drawstaangle);


                    int l_n32IWDGState = 0;
                    if (comb_WDGSET.Text == "关闭")
                    {
                        l_n32IWDGState = 0;
                    }
                    else if (comb_WDGSET.Text == "开启")
                    {
                        l_n32IWDGState = 1;
                    }

                    if (comb_HEARTSTATE.Text == "关闭")
                    {
                        g_n32HeartState = 0;
                    }
                    else if (comb_HEARTSTATE.Text == "开启")
                    {
                        g_n32HeartState = 1;
                    }

                    if (comb_protocol.Text=="UDP")       //数据协议
                    {
                        g_n32protocoltype = 1;
                    }
                    else if (comb_protocol.Text=="TCP")
                    {
                        g_n32protocoltype = 0;
                    }

                    Array.Copy(l_byteBufSetBasicParameterson, g_abSetBasicPARM,
                               l_byteBufSetBasicParameterson.Length);

                    g_abSetBasicPARM[27] = SCS[1]; //起始角度
                    g_abSetBasicPARM[26] = SCS[0];
                    g_abSetBasicPARM[29] = (Convert.ToByte((UInt32)g_n32protocoltype >> 8));  //数据协议
                    g_abSetBasicPARM[28] = (Convert.ToByte((UInt32)g_n32protocoltype & 0xff));
                    g_abSetBasicPARM[31] = DSA[1]; //绘图起始角
                    g_abSetBasicPARM[30] = DSA[0];

                    g_abSetBasicPARM[35] = (Convert.ToByte((UInt32)g_n32HeartState >> 8));      //心跳状态
                    g_abSetBasicPARM[34] = (Convert.ToByte((UInt32)g_n32HeartState & 0xff));

                    g_abSetBasicPARM[37] = (Convert.ToByte((UInt32)l_n32IWDGState >> 8));      //看门狗状态
                    g_abSetBasicPARM[36] = (Convert.ToByte((UInt32)l_n32IWDGState & 0xff));

                    g_abSetBasicPARM[39] = ToolFunc .XorCheck_byte(2, g_abSetBasicPARM, 4);
                    g_abSetBasicPARM[40] = 0xEE;
                    g_abSetBasicPARM[41] = 0xEE;
                    //g_SocketClient.Send(g_abSetBasicPARM);
                    serialPort1.Write(g_abSetBasicPARM, 0, g_abSetBasicPARM.Length);
                }
            }
            catch
            {
                MessageBox_Invoke("应用设置参数下载失败");
                return;
            }
        }
        #endregion

        #region 重启设备
        private void btn_RESET_Click(object sender, EventArgs e)
        {
            try
            {
                //if (g_SocketClient.Connected == true)
                if (serialPort1.IsOpen)
                {
                    //g_SocketClient.Send(g_abRestartEquip);
                    serialPort1.Write(g_abRestartEquip, 0, g_abRestartEquip.Length);
                    RichTextBox_Invoke(richTextBox1, "重启设备中...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "重启设备失败！！！\r\n");
            }
        }
        #endregion

        #region 生产设置APD参数读取 
        private void btn_APDPIB_READ_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    serialPort1.Write(g_abQueryAPDPARM, 0, g_abQueryAPDPARM.Length);
                    RichTextBox_Invoke(richTextBox1, "生产设置APD参数读取中...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "生产设置APD参数读取失败\r\n");
            }
        }

        #endregion 

        #region 生产设置APD参数下载
        private void btn_BTN_APDPIB_WR_Click(object sender, EventArgs e)
        {
            try
            {
                //if (g_SocketClient.Connected == true)
                if(serialPort1.IsOpen)
                {
                    byte[] l_byteBufSetAPDParameterson = new byte[26]{0xFF,0xAA,0x00,0x20,0x00,0x00,0x00,0x00,0x00,
                                                                      0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                      0x00,0x00,0x00,0x00,0x07,0x05,0x00,0x00};

                    float l_fAPDHvValue = Convert.ToSingle(txt_APDHVVALUE.Text);             //击穿电压
                    float l_fAPDTemperValue = Convert.ToSingle(txt_APDTEMPERVALUE.Text);     //击穿电压温度
                    float l_fAPDHV_OP_Ratio = Convert.ToSingle(txt_APDHV_OP_RATIO.Text);     //电压衰减系数
                    float l_fAPDTemperRatio = Convert.ToSingle(txt_tempradio.Text);          //温度系数

                    l_fAPDHvValue = l_fAPDHvValue * 100.0f;
                    l_fAPDTemperValue = l_fAPDTemperValue * 100.0f;
                    l_fAPDHV_OP_Ratio = l_fAPDHV_OP_Ratio * 100.0f;
                    l_fAPDTemperRatio = l_fAPDTemperRatio * 100.0f;

                    Array.Copy(l_byteBufSetAPDParameterson, g_abSetAPDPARM,
                               l_byteBufSetAPDParameterson.Length);

                    g_abSetAPDPARM[24] = (Convert.ToByte((UInt32)l_fAPDTemperRatio >> 8));
                    g_abSetAPDPARM[25] = (Convert.ToByte((UInt32)l_fAPDTemperRatio & 0xff));
                    g_abSetAPDPARM[26] = (Convert.ToByte((UInt32)l_fAPDHvValue >> 8));
                    g_abSetAPDPARM[27] = (Convert.ToByte((UInt32)l_fAPDHvValue & 0xff));
                    g_abSetAPDPARM[28] = (Convert.ToByte((UInt32)l_fAPDTemperValue >> 8));
                    g_abSetAPDPARM[29] = (Convert.ToByte((UInt32)l_fAPDTemperValue & 0xff));
                    g_abSetAPDPARM[30] = (Convert.ToByte((UInt32)l_fAPDHV_OP_Ratio >> 8));
                    g_abSetAPDPARM[31] = (Convert.ToByte((UInt32)l_fAPDHV_OP_Ratio & 0xff));
                    g_abSetAPDPARM[33] = ToolFunc.XorCheck_byte(2, g_abSetAPDPARM, 4);
                    g_abSetAPDPARM[34] = 0xEE;
                    g_abSetAPDPARM[35] = 0xEE;
                    //g_SocketClient.Send(g_abSetAPDPARM);
                    serialPort1.Write(g_abSetAPDPARM, 0, g_abSetAPDPARM.Length);

                }
            }
            catch
            {
                MessageBox_Invoke("生产设置APD参数下载失败");
                return;
            }
        }
        #endregion

        #region 生产设置功能设置参数读取
        private void btn_PROD_BASICPIBUP_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    serialPort1.Write(g_abQueryFunctionalPARM, 0, g_abQueryFunctionalPARM.Length);
                    RichTextBox_Invoke(richTextBox1, "生产设置功能设置参数读取中...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "生产设置功能设置参数读取失败\r\n");
                return;
            }
          
        }
        #endregion

        #region 生产设置功能设置参数下载
        private void btn_PRODBASICPIBDOWN_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    byte[] l_byteBufSetFunctionalParameter = new byte[26]{0xFF,0xAA,0x00,0x27,0x00,0x00,0x00,0x00,0x00,
                                                                          0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                          0x00,0x00,0x00,0x00,0x07,0x02,0x00,0x00};

                    Array.Copy(l_byteBufSetFunctionalParameter, g_abSetFunctionalPARM,
                               l_byteBufSetFunctionalParameter.Length);

                    int l_n32EquipNo = Convert.ToInt32(txt_PROD_EQUIPNO1.Text);      //设备号
                    int l_n32ZeroDisc = 0; //Convert.ToInt32(txt_ZERO_offset.Text);       //零点偏移
                    int l_n32HWholeDisc = Convert.ToInt32(txt_PROD_HWHOLEDISC.Text); //高整体偏移
                    int l_n32Motor_speed = 0;
                    if (comb_motorspeed.Text == "15Hz")
                    {
                        l_n32Motor_speed = 0;
                    }
                    else if (comb_motorspeed.Text == "25Hz")
                    {
                        l_n32Motor_speed = 1;
                    }

                    int l_n32LD_power = comb_LDpower.SelectedIndex;

                    g_abSetFunctionalPARM[26] = Convert.ToByte((l_n32EquipNo & 0xff0000) >> 16);
                    g_abSetFunctionalPARM[27] = Convert.ToByte((l_n32EquipNo & 0x00ff00) >> 8);
                    g_abSetFunctionalPARM[28] = Convert.ToByte((l_n32EquipNo & 0x0000ff));

                    g_abSetFunctionalPARM[29] = Convert.ToByte((l_n32Motor_speed >> 8) & 0xff);
                    g_abSetFunctionalPARM[30] = Convert.ToByte((l_n32Motor_speed & 0xff));

                    g_abSetFunctionalPARM[31] = Convert.ToByte((l_n32ZeroDisc >> 8) & 0xff);
                    g_abSetFunctionalPARM[32] = Convert.ToByte((l_n32ZeroDisc & 0xff));

                    g_abSetFunctionalPARM[33] = Convert.ToByte((l_n32HWholeDisc >> 8) & 0xff);
                    g_abSetFunctionalPARM[34] = Convert.ToByte((l_n32HWholeDisc & 0xff));

                    g_abSetFunctionalPARM[35] = Convert.ToByte((l_n32LD_power >> 8) & 0xff);
                    g_abSetFunctionalPARM[36] = Convert.ToByte((l_n32LD_power & 0xff));

                    g_abSetFunctionalPARM[37] = Convert.ToByte(comb_PROD_SI2SCSET.SelectedIndex); //单点扫描

                    g_abSetFunctionalPARM[38] = 0x00;                                             //高阈值

                    g_abSetFunctionalPARM[40] = ToolFunc.XorCheck_byte(2, g_abSetFunctionalPARM, 4);
                    g_abSetFunctionalPARM[41] = 0xEE;
                    g_abSetFunctionalPARM[42] = 0xEE;
                    serialPort1.Write(g_abSetFunctionalPARM,0, g_abSetFunctionalPARM.Length);

                }
            }
            catch
            {
                MessageBox_Invoke("生产设置功能设置参数下载失败");
                return;
            }
           
        }
        #endregion

        #region 修正参数设置参数读取
        private void btn_BASICSTATE_READ_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    serialPort1.Write(g_abQueryCorrectedPARM, 0, g_abQueryCorrectedPARM.Length);
                    RichTextBox_Invoke(richTextBox1, "修正参数设置参数读取中...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "修正参数设置参数读取失败\r\n");
                return;
            }
        }
        #endregion

        #region 修正参数设置参数下载
        private void btn_BASICSTATE_WRITE_Click(object sender, EventArgs e)
        {
            try
            {
                //if (g_SocketClient.Connected == true)
                if(serialPort1.IsOpen)
                {
                    byte[] l_byteBufSetCorrectedParameter = new byte[26]{0xFF,0xAA,0x00,0x22,0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x06,0x05,0x00,0x00};
                    Array.Copy(l_byteBufSetCorrectedParameter, g_abSetCorrectedPARM,
                               l_byteBufSetCorrectedParameter.Length);

                    double l_n64HLevel = Convert.ToDouble(txt_H_LEVEL1.Text);//高阈值电压
                    l_n64HLevel = (l_n64HLevel * 4096 / 4.096);
                    int l_n32MinPulsewidth = Convert.ToInt32(txt_pulsewidth.Text);//最小脉宽
                    int l_n32SomthingValue = Convert.ToInt32(txt_smtval.Text);  //滤波阈值

                    int l_n32TaskState = 0;  //修正表状态
                    if (comb_MODIFY.Text == "不修正")
                    {
                        l_n32TaskState = 0;
                    }
                    else if (comb_MODIFY.Text == "修正")
                    {
                        l_n32TaskState = 1;
                    }
                    else
                    {
                        l_n32TaskState = 0;
                    }

                    g_abSetCorrectedPARM[26] = (Convert.ToByte(l_n32TaskState >> 8));
                    g_abSetCorrectedPARM[27] = (Convert.ToByte(l_n32TaskState & 0xff));

                    g_abSetCorrectedPARM[28] = (Convert.ToByte((UInt32)l_n64HLevel >> 8));
                    g_abSetCorrectedPARM[29] = (Convert.ToByte((UInt32)l_n64HLevel & 0xff));

                    g_abSetCorrectedPARM[30] = (Convert.ToByte(l_n32MinPulsewidth >> 8));
                    g_abSetCorrectedPARM[31] = (Convert.ToByte(l_n32MinPulsewidth & 0xff));

                    g_abSetCorrectedPARM[32] = (Convert.ToByte(l_n32SomthingValue >> 8));
                    g_abSetCorrectedPARM[33] = (Convert.ToByte(l_n32SomthingValue & 0xff));

                    g_abSetCorrectedPARM[35] = ToolFunc.XorCheck_byte(2, g_abSetCorrectedPARM, 4);

                    g_abSetCorrectedPARM[36] = 0xEE;
                    g_abSetCorrectedPARM[37] = 0xEE;

                    serialPort1.Write(g_abSetCorrectedPARM, 0, g_abSetCorrectedPARM.Length);
                    txt_smothingset.Text = l_n32SomthingValue.ToString();
                }
            }
            catch
            {
                MessageBox_Invoke("修正参数设置参数下载失败");
                return;
            }
        }

        #endregion

        #region 电机转速测试
        private void btn_PIDSTART_CHECK_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_PIDSTART_CHECK.Text == "电机转速测试")
                {
                    btn_PIDSTART_CHECK.Text = "关闭测试";
                    g_bDrawMotorSpeedFlg = true;

                }
                else if (btn_PIDSTART_CHECK.Text == "关闭测试")
                {
                    btn_PIDSTART_CHECK.Text = "电机转速测试";
                    g_bDrawMotorSpeedFlg = false;
                }

            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "电机转速测试失败\r\n");
                return;
            }
        }
        #endregion

        #region 开启加热
        //private void btn_PROD_HEATEN_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (g_SocketClient.Connected == true)
        //        {

        //            if (btn_PROD_HEATEN.Text == "停止加热")
        //            {
        //                btn_PROD_HEATEN.Text = "开启加热";
        //                g_abHeatEquip[29] = 0x00;
        //                g_abHeatEquip[31] = ToolFunc.XorCheck_byte(2, g_abHeatEquip, 4);
        //                g_SocketClient.Send(g_abHeatEquip);
        //            }
        //            else
        //            {
        //                btn_PROD_HEATEN.Text = "停止加热";
        //                g_abHeatEquip[29] = 0x01;
        //                g_abHeatEquip[31] = ToolFunc.XorCheck_byte(2, g_abHeatEquip, 4);
        //                g_SocketClient.Send(g_abHeatEquip);
        //            }

        //        }
        //    }
        //    catch
        //    {
        //        RichTextBox_Invoke(richTextBox1, "网络连接异常，开启/停止加热失败！\r\n");
        //        return;
        //    }
        //}
        #endregion

        #region 清除复位次数
        private void btn_CLEAR_RSTNUM_Click(object sender, EventArgs e)
        {
            try
            {
                //if (g_SocketClient.Connected == true)
                if(serialPort1.IsOpen)
                {
                    byte[] l_abAPP_Config_CLRRSTNUM = new byte[34] {0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x01,0x01,0x00,0x05,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00,0x00,0x05,0x03,
                                                                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xEE,0xEE};

                    l_abAPP_Config_CLRRSTNUM[31] = ToolFunc.XorCheck_byte(2, l_abAPP_Config_CLRRSTNUM, 4);
                    //g_SocketClient.Send(l_abAPP_Config_CLRRSTNUM);
                    serialPort1.Write(l_abAPP_Config_CLRRSTNUM, 0, l_abAPP_Config_CLRRSTNUM.Length);
                }
            }
            catch
            {
                //richtext_show("应用设置参数下载失败\r\n");
                MessageBox_Invoke("清除复位次数失败！");
                return;
            }
        }

        #endregion

        #region 显示参数下载

        private void btn_setScanParam_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.IsOpen)
                {
                    byte[] l_byteBufSetBasicParameterson = new byte[26] {0xFF,0xAA,0x00,0x26,0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x06,0x02,0x00,0x00};
                    if (comb_scansta.Text == "225")
                    {
                        g_n32drawstaangle = 0;
                        comb_drawstaangle.Text = "225";
                    }
                    else if (comb_scansta.Text == "315")
                    {
                        g_n32drawstaangle = 1;
                        comb_drawstaangle.Text = "315";
                    }

                    int l_n32ScanSta = Convert.ToInt16(txt_EDIT_SCAN_START.Text);//扫描起始角度
                    if (l_n32ScanSta > 149)
                    {
                        l_n32ScanSta = 149;
                        txt_EDIT_SCAN_START.Text = l_n32ScanSta.ToString();
                    }
                    if (l_n32ScanSta < 1)
                    {
                        l_n32ScanSta = 1;
                        txt_EDIT_SCAN_START.Text = l_n32ScanSta.ToString();
                    }
                    Byte[] SCS = BitConverter.GetBytes(l_n32ScanSta);
                    Byte[] DSA = BitConverter.GetBytes(g_n32drawstaangle);

                    int l_n32IWDGState = 0;
                    if (comb_WDGSET.Text == "关闭")
                    {
                        l_n32IWDGState = 0;
                    }
                    else if (comb_WDGSET.Text == "开启")
                    {
                        l_n32IWDGState = 1;
                    }

                    if (comb_HEARTSTATE.Text == "关闭")
                    {
                        g_n32HeartState = 0;
                    }
                    else if (comb_HEARTSTATE.Text == "开启")
                    {
                        g_n32HeartState = 1;
                    }

                    Array.Copy(l_byteBufSetBasicParameterson, g_abSetBasicPARM,
                               l_byteBufSetBasicParameterson.Length);

                    g_abSetBasicPARM[27] = SCS[1]; //起始角度
                    g_abSetBasicPARM[26] = SCS[0];
                    //g_abSetBasicPARM[29] = SCE[1]; //终止角度
                    //g_abSetBasicPARM[28] = SCE[0];
                    g_abSetBasicPARM[31] = DSA[1]; //绘图起始角
                    g_abSetBasicPARM[30] = DSA[0];

                    g_abSetBasicPARM[35] = (Convert.ToByte((UInt32)g_n32HeartState >> 8));      //心跳状态
                    g_abSetBasicPARM[34] = (Convert.ToByte((UInt32)g_n32HeartState & 0xff));

                    g_abSetBasicPARM[37] = (Convert.ToByte((UInt32)l_n32IWDGState >> 8));      //看门狗状态
                    g_abSetBasicPARM[36] = (Convert.ToByte((UInt32)l_n32IWDGState & 0xff));

                    g_abSetBasicPARM[39] = ToolFunc.XorCheck_byte(2, g_abSetBasicPARM, 4);
                    g_abSetBasicPARM[40] = 0xEE;
                    g_abSetBasicPARM[41] = 0xEE;
                    //g_SocketClient.Send(g_abSetBasicPARM);
                    serialPort1.Write(g_abSetBasicPARM, 0, g_abSetBasicPARM.Length);
                }
            }


            catch
            {
                MessageBox_Invoke("显示参数下载失败");
                return;
            }
        }

        #endregion 

        #region 过度点设置
        private void btn_smothingset_Click(object sender, EventArgs e)
        {
            try
            {
                //if (g_SocketClient.Connected == true)
                if(serialPort1.IsOpen)
                {
                    byte[] l_byteBufSetCorrectedParameter = new byte[26]{0xFF,0xAA,0x00,0x22,0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x06,0x05,0x00,0x00};
                    Array.Copy(l_byteBufSetCorrectedParameter, g_abSetCorrectedPARM,
                               l_byteBufSetCorrectedParameter.Length);

                    double l_n64HLevel = Convert.ToDouble(txt_H_LEVEL1.Text);//高阈值电压
                    l_n64HLevel = (l_n64HLevel * 4096 / 4.096);
                    int l_n32MinPulsewidth = Convert.ToInt32(txt_pulsewidth.Text);//最小脉宽
                    int l_n32SomthingValue = Convert.ToInt32(txt_smothingset.Text);  //滤波阈值

                    int l_n32TaskState = 0;  //修正表状态
                    if (comb_MODIFY.Text == "不修正")
                    {
                        l_n32TaskState = 0;
                    }
                    else if (comb_MODIFY.Text == "修正")
                    {
                        l_n32TaskState = 1;
                    }
                    else
                    {
                        l_n32TaskState = 0;
                    }

                    g_abSetCorrectedPARM[26] = (Convert.ToByte(l_n32TaskState >> 8));
                    g_abSetCorrectedPARM[27] = (Convert.ToByte(l_n32TaskState & 0xff));

                    g_abSetCorrectedPARM[28] = (Convert.ToByte((UInt32)l_n64HLevel >> 8));
                    g_abSetCorrectedPARM[29] = (Convert.ToByte((UInt32)l_n64HLevel & 0xff));

                    g_abSetCorrectedPARM[30] = (Convert.ToByte(l_n32MinPulsewidth >> 8));
                    g_abSetCorrectedPARM[31] = (Convert.ToByte(l_n32MinPulsewidth & 0xff));

                    g_abSetCorrectedPARM[32] = (Convert.ToByte(l_n32SomthingValue >> 8));
                    g_abSetCorrectedPARM[33] = (Convert.ToByte(l_n32SomthingValue & 0xff));

                    g_abSetCorrectedPARM[35] = ToolFunc.XorCheck_byte(2, g_abSetCorrectedPARM, 4);

                    g_abSetCorrectedPARM[36] = 0xEE;
                    g_abSetCorrectedPARM[37] = 0xEE;
                    //g_SocketClient.Send(g_abSetCorrectedPARM);
                    serialPort1.Write(g_abSetCorrectedPARM, 0, g_abSetCorrectedPARM.Length);

                    txt_smtval.Text = l_n32SomthingValue.ToString();
                }
            }
            catch
            {
                MessageBox_Invoke("过渡点设置失败");
                return;
            }
        }
        #endregion

        #region 保存出厂设置
        private void btn_SaveFactoryParam_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    g_abSaveFactoryPARM[31] = ToolFunc.XorCheck_byte(2, g_abSaveFactoryPARM, 4);
                    serialPort1.Write(g_abSaveFactoryPARM, 0, g_abSaveFactoryPARM.Length);                   
                    RichTextBox_Invoke(richTextBox1, "正在保存出厂设置参数...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "保存出厂设置参数失败\r\n");
                return;
            }
        }
        #endregion

        #region 恢复出厂设置
        private void btn_resetfactoryparam_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    g_abResetFactoryPARM[31] = ToolFunc.XorCheck_byte(2, g_abResetFactoryPARM, 4);
                    serialPort1.Write(g_abResetFactoryPARM, 0, g_abResetFactoryPARM.Length);      
                    RichTextBox_Invoke(richTextBox1, "正在恢复出厂设置参数...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "恢复出厂设置参数失败\r\n");
                return;
            }
        }
        #endregion

        #region io 检测
        //状态设置
        private void btn_iodownload_Click(object sender, EventArgs e)
        {
            byte[] l_abSetOutput = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                0x00,0x00,0x00,0x00,0x07,0x0B,0x00,0x00,0x00,
                                                0x00,0x00,0x00,0x00,0x00,0xEE,0xEE};
            byte l_n8iostate = 0;

            l_n8iostate = Convert.ToByte(l_n8iostate | ((comb_output1.SelectedIndex & 0xff) << 3));
            l_n8iostate = Convert.ToByte(l_n8iostate | ((comb_output2.SelectedIndex & 0xff) << 2));
            l_n8iostate = Convert.ToByte(l_n8iostate | ((comb_output3.SelectedIndex & 0xff) << 1));
            l_n8iostate = Convert.ToByte(l_n8iostate | ((comb_output4.SelectedIndex & 0xff) ));

            l_abSetOutput[29] = l_n8iostate;
            l_abSetOutput[31] = ToolFunc.XorCheck_byte(2, l_abSetOutput, 4);

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(l_abSetOutput, 0, l_abSetOutput.Length);
            }

        }
        //状态查询
        private void btn_ioupload_Click(object sender, EventArgs e)
        {
            byte[] l_abQueryOutput = new byte[34]{0xFF,0xAA,0x00,0x1E,0x00,0x00,0x00,0x00,0x00,
                                                  0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                  0x00,0x00,0x00,0x00,0x07,0x0A,0x00,0x00,0x00,
                                                  0x00,0x00,0x00,0x00,0x00,0xEE,0xEE};


            l_abQueryOutput[31] = ToolFunc.XorCheck_byte(2, l_abQueryOutput, 4);

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(l_abQueryOutput, 0, l_abQueryOutput.Length);
            }
        }
        #endregion

        #region mac设置
        private void btn_setnetparm_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    byte[] l_abSetNetParameter = new byte[68] {0xFF,0xAA,0x00,0x40,0x00,0x00,0x00,0x00,0x00,
                                                               0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                               0x00,0x00,0x00,0x00,0x06,0x09,0x00,0x00,0x00,
                                                               0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                               0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                               0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                               0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                               0x00,0x00,0x00,0xEE,0xEE};

                    byte[] l_abmac = ToolFunc.MACToByteArray(txt_mac.Text);          //mac

                    l_abSetNetParameter[53] = l_abmac[0];
                    l_abSetNetParameter[52] = l_abmac[1];
                    l_abSetNetParameter[55] = l_abmac[2];
                    l_abSetNetParameter[54] = l_abmac[3];
                    l_abSetNetParameter[57] = l_abmac[4];
                    l_abSetNetParameter[56] = l_abmac[5];
                    l_abSetNetParameter[59] = l_abmac[6];
                    l_abSetNetParameter[58] = l_abmac[7];
                    l_abSetNetParameter[61] = l_abmac[8];
                    l_abSetNetParameter[60] = l_abmac[9];
                    l_abSetNetParameter[63] = l_abmac[10];
                    l_abSetNetParameter[62] = l_abmac[11];

                    l_abSetNetParameter[65] = ToolFunc.XorCheck_byte(2, l_abSetNetParameter, 4);

                    serialPort1.Write(l_abSetNetParameter, 0, l_abSetNetParameter.Length);
                }
            }
            catch
            {
                MessageBox_Invoke("MAC下载失败");
                return;
            }
        }
        #endregion

        #region mac读取
        private void btn_readnetparm_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    g_abQueryNetPARM[31] = ToolFunc.XorCheck_byte(2, g_abQueryNetPARM, 4);
                    serialPort1.Write(g_abQueryNetPARM, 0, g_abQueryNetPARM.Length);
                    RichTextBox_Invoke(richTextBox1, "MAC读取中...\r\n");
                }
            }
            catch
            {
                RichTextBox_Invoke(richTextBox1, "MAC读取失败\r\n");
                return;
            }
        }
        #endregion

        #endregion

        #region 子窗体显示相关

        #region ScanDataSet扫描参数设置界面
        private void axTChart2_OnDblClick(object sender, EventArgs e)
        {
            try
            {
                if (g_bProdLoginFlg || g_bResearchLoginFlg)
                {
                    if (ScanSetForm == null || ScanSetForm.IsDisposed)
                    {
                        ScanSetForm = new ScanDataSet();
                    }
                    ScanSetForm.Show(this);  //注意不加this 不能传递参数

                    g_n32ScanDatSetFrmFlg = 1;
                }

            }
            catch
            {
                return;
            }
        }
        #endregion

        #region SI2DCFG 单点参数设置界面
        private void axTChart1_OnDblClick(object sender, EventArgs e)
        {
            try
            {
                if (g_bProdLoginFlg||g_bResearchLoginFlg)
                {
                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                    {
                        Single2DConfigure = new SI2DCFG();
                    }
                    Single2DConfigure.Show(this);  //注意不加this 不能传递参数
                    g_n32Si2DConfigFrmFlg = 1;
                }
                else
                {
                    //MessageBox.Show("请以研发人员的身份登录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }

            catch
            {
                return;
            }
        }
        #endregion

        #region 拟合界面
        public void OpenInterpform()
        {
            try
            {
                if (InterpForm == null || InterpForm.IsDisposed)
                {
                    InterpForm = new INTERP();
                }
                InterpForm.g_n32InterpDataListx.Clear();
                InterpForm.g_n32InterpDataListy.Clear();
                InterpForm.g_n32InterpDataListx.AddRange(g_n32LstPulseWidthReadFromFile);
                InterpForm.g_n32InterpDataListy.AddRange(g_n32LstStdSubRisingEdgeReadFromFile);
                InterpForm.IntSi2DChSel = g_n32Si2DCHSEL;
                InterpForm.Show(this);  //注意不加this 不能传递参数
                //InterpForm.ShowDialog(this);
            }
            catch
            {
                return;
            }

        }
        #endregion

        #region 登录界面窗体显示
        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (loginToolStripMenuItem.Text == "登录")
                {
                    if (LoginForm == null || LoginForm.IsDisposed)
                    {
                        LoginForm = new Login();       //定义子窗体对象
                    }
                    LoginForm.Owner = this;           //建立父子窗体关系
                    LoginForm.Show(this);           //显示子窗体
                }
                else if (loginToolStripMenuItem.Text == "退出登录")
                {
                    g_bProdLoginFlg = false;
                    g_bResearchLoginFlg = false;
                    loginToolStripMenuItem.Text = "登录";
                    this.Text = g_sFormName + g_sVersionDate + "-未登录";
                    tabPage1.Parent = null;
                    tabPage8.Parent = null;
                    tabPage3.Parent = null;
                    tabPage6.Parent = null;
                    //txt_mac.Enabled = false;
                    tabPage10.Parent = null;
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region 修改密码窗体
        public void MfPasswordForm_Show()
        {
            try
            {
                if (MfPasswordForm == null || MfPasswordForm.IsDisposed)
                {
                    MfPasswordForm = new ModifyPassword();       //定义子窗体对象
                }
                MfPasswordForm.Owner = this;                     //建立父子窗体关系
                MfPasswordForm.Show(this);                       //显示子窗体
            }
            catch
            {
                return;
            }
        }

        #region 恢复出厂设置
        public void FactoryResetSecret()                 //恢复出厂设置
        {
            if (FactoryResetForm == null || FactoryResetForm.IsDisposed)
            {
                FactoryResetForm = new FactorReset();       //定义子窗体对象
            }
            FactoryResetForm.Owner = this;                     //建立父子窗体关系
            FactoryResetForm.Show(this);                       //显示子窗体
        }
        #endregion 

        #endregion

        #region 一键测距

        public int g_n32RecordFrmFlg = 0;                 //扫描参数配置窗体打开则为1，关闭则为零
        private void btn_measure_Click(object sender, EventArgs e)
        {
            try
            {
                if (RecordDisForm == null || RecordDisForm.IsDisposed)
                {
                    RecordDisForm = new Record20PointsDistance();
                }
                RecordDisForm.Show(this);  //注意不加this 不能传递参数
                g_n32RecordFrmFlg = 1;
                g_al20pointdata.Clear();
                for (int i = 0; i < 20; i++)
                {
                    List<int> l_alpointdata = new List<int>();
                    g_al20pointdata.Add(l_alpointdata);
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #endregion

        #region 绘图相关

        #region 作图函数
        public int g_n32DrawStyle = 2;    //绘图方式：1：线图  2：点图

        #region 扫描数据作图
        private void Draw_JI()  //将扫描数据绘制到TeeChart控件中（极坐标系）
        {
            try
            {
                axTChart2.Series(0).Clear();
                axTChart2.Series(0).AddArray(g_n32DrawDatLen,g_n64ArrayJiY);
                g_n32DrawPkgssOfJi++;                     //绘画包数
                //g_n32ScanDatCorrectPkgs = g_n32DrawPkgssOfJi * 6;

            }
            catch
            {
                //MessageBox_Invoke("极坐标系绘图错误！！！！");
                return;
            }

        }

        public void Draw_JI(double[] arrayY)            //将保存的历史扫描数据绘制到TeeChart控件中（极坐标系）
        {
            try
            {
                axTChart2.Series(0).Clear();
                axTChart2.Series(0).AddArray(arrayY.Length, arrayY);
            }
            catch
            {
                return;
            }
        }
        public void Clear_JI()
        {
            try
            {
                axTChart2.Series(0).Clear();
            }
            catch
            { }
        }
        private void Draw_ZHI()                      //将扫描数据绘制到picturebox1中（直角坐标系）
        {
            try
            {
                //if (!g_bBeginDrawRegionFlg)
                if(c_DrawCd.g_bpicturerefresh)
                {
                    c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1,g_n32BankNo,
                                        g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                    pictureBox1.Image = c_DrawCd.bitmap;
                }

            }
            catch
            {
                //MessageBox_Invoke("直角坐标系作图错误！！！");
                return;
            }
        }


        private void Draw_MotorSpeed()
        {
            try
            {
                if (g_bDrawMotorSpeedFlg)
                {
                    fastLine1.Add(g_n32MotorSpeed);
                    if (fastLine1.Count >= 100)
                    {
                        fastLine1.Clear();
                    }
                }
            }
            catch
            { }
        }

        #region 绘图方式更改
        private void comb_drawstyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comb_drawstyle.Text == "线图")
            {
                g_n32DrawStyle = 1;
                comb_scandrawstyle.Text = "线图";
            }
            else if (comb_drawstyle.Text == "点图")
            {
                g_n32DrawStyle = 2;
                comb_scandrawstyle.Text = "点图";
            }
        }

        private void comb_scandrawstyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comb_scandrawstyle.Text == "线图")
            {
                g_n32DrawStyle = 1;
                comb_drawstyle.Text = "线图";
            }
            else if (comb_scandrawstyle.Text == "点图")
            {
                g_n32DrawStyle = 2;
                comb_drawstyle.Text = "点图";
            }
        }

        #endregion
        #endregion

        #region 单点数据作图
        public void TchartClear()
        {
            g_listRisingDatY.Clear();
            g_listFallingDatY.Clear();
            g_listTimingDatY.Clear();
            axTChart1.Series(0).Clear();
            axTChart1.Series(1).Clear();
            axTChart1.Series(2).Clear();
        }

        #endregion

        #endregion

        #region 自定义绘图

        #region 参数定义

        DrawCoordinate c_DrawCd = new DrawCoordinate();
        public bool g_bBeginDrawRegionFlg = false;       //是否开始划分区域

        Point g_epCanvasOrigin;                          //画布原点在设备上的坐标(像素点)
        Point g_ptCanvasBuf;                             //重置画布坐标计算时用的临时变量
        private float g_n32Scale = 1.1F;                 //缩放比例
        private int g_n32BitMapWidth = 0;
        private int g_n32BitMapHeight = 0;
        List<Point> g_ptLstp = new List<Point>();        //存放鼠标移动时产生的坐标点(像素点转换后的屏幕点)
        List<Point> g_ptLste = new List<Point>();        //存放鼠标移动时产生的坐标点(像素点)

        Point g_ptStartPt = new Point();
        Point g_ptEndPt = new Point();
        Point g_ptMouseDown;                             //鼠标点下时在设备坐标上的坐标


        #region Bank初始参数

        Point g_Bank1Field3point1 = new Point(1520, 0);
        Point g_Bank1Field3point2 = new Point(1520, 1520);
        Point g_Bank1Field3point3 = new Point(-1520, 1520);
        Point g_Bank1Field3point4 = new Point(-1520, 0);

        Point g_Bank2Field3point1 = new Point(1520, 0);
        Point g_Bank2Field3point2 = new Point(1520, 1900);
        Point g_Bank2Field3point3 = new Point(-1520, 1900);
        Point g_Bank2Field3point4 = new Point(-1520, 0);

        Point g_Bank3Field3point1 = new Point(1520, 0);
        Point g_Bank3Field3point2 = new Point(1520, 2280);
        Point g_Bank3Field3point3 = new Point(-1520, 2280);
        Point g_Bank3Field3point4 = new Point(-1520, 0);

        Point g_Bank4Field3point1 = new Point(1520, 0);
        Point g_Bank4Field3point2 = new Point(1520, 2660);
        Point g_Bank4Field3point3 = new Point(-1520, 2660);
        Point g_Bank4Field3point4 = new Point(-1520, 0);

        Point g_Bank5Field3point1 = new Point(-1140, 0);
        Point g_Bank5Field3point2 = new Point(0, 1140);
        Point g_Bank5Field3point3 = new Point(1140, 0);

        Point g_Bank5Field2point1 = new Point(-937, 0);
        Point g_Bank5Field2point2 = new Point(0, 937);
        Point g_Bank5Field2point3 = new Point(937, 0);

        Point g_Bank5Field1point1 = new Point(-750, 0);
        Point g_Bank5Field1point2 = new Point(0, 750);
        Point g_Bank5Field1point3 = new Point(750, 0);

        Point g_Bank6Field3point1 = new Point(-1520, 0);
        Point g_Bank6Field3point2 = new Point(0, 1520);
        Point g_Bank6Field3point3 = new Point(1520, 0);

        Point g_Bank6Field2point1 = new Point(-1250, 0);
        Point g_Bank6Field2point2 = new Point(0, 1250);
        Point g_Bank6Field2point3 = new Point(1250, 0);

        Point g_Bank6Field1point1 = new Point(-1000, 0);
        Point g_Bank6Field1point2 = new Point(0, 1000);
        Point g_Bank6Field1point3 = new Point(1000, 0);

        Point g_Bank7Field3point1 = new Point(-2280, 0);
        Point g_Bank7Field3point2 = new Point(0, 2280);
        Point g_Bank7Field3point3 = new Point(2280, 0);

        Point g_Bank7Field2point1 = new Point(-1875, 0);
        Point g_Bank7Field2point2 = new Point(0, 1875);
        Point g_Bank7Field2point3 = new Point(1875, 0);

        Point g_Bank7Field1point1 = new Point(-1500, 0);
        Point g_Bank7Field1point2 = new Point(0, 1500);
        Point g_Bank7Field1point3 = new Point(1500, 0);

        Point g_Bank8Field3point1 = new Point(-3040, 0);
        Point g_Bank8Field3point2 = new Point(0, 3040);
        Point g_Bank8Field3point3 = new Point(3040, 0);

        Point g_Bank8Field2point1 = new Point(-2500, 0);
        Point g_Bank8Field2point2 = new Point(0, 2500);
        Point g_Bank8Field2point3 = new Point(2500, 0);

        Point g_Bank8Field1point1 = new Point(-2000, 0);
        Point g_Bank8Field1point2 = new Point(0, 2000);
        Point g_Bank8Field1point3 = new Point(2000, 0);


        Point g_Bank9Field3point1 = new Point(1500, 1500);
        Point g_Bank9Field3point2 = new Point(0, 3000);
        Point g_Bank9Field3point3 = new Point(-1500, 1500);

        Point g_Bank9Field2point1 = new Point(1250, 1250);
        Point g_Bank9Field2point2 = new Point(0, 2500);
        Point g_Bank9Field2point3 = new Point(-1250, 1250);

        Point g_Bank9Field1point1 = new Point(1000, 1000);
        Point g_Bank9Field1point2 = new Point(0, 2000);
        Point g_Bank9Field1point3 = new Point(-1000, 1000);

        Point g_Bank10Field3point1 = new Point(2250, 563);
        Point g_Bank10Field3point2 = new Point(0, 1125);
        Point g_Bank10Field3point3 = new Point(-2250, 563);

        Point g_Bank10Field2point1 = new Point(1875, 469);
        Point g_Bank10Field2point2 = new Point(0, 937);
        Point g_Bank10Field2point3 = new Point(-1875, 469);

        Point g_Bank10Field1point1 = new Point(1500, 375);
        Point g_Bank10Field1point2 = new Point(0, 750);
        Point g_Bank10Field1point3 = new Point(-1500, 375);

        Point g_Bank11Field3point1 = new Point(2250, 750);
        Point g_Bank11Field3point2 = new Point(0, 1500);
        Point g_Bank11Field3point3 = new Point(-2250, 750);

        Point g_Bank11Field2point1 = new Point(1875, 625);
        Point g_Bank11Field2point2 = new Point(0, 1250);
        Point g_Bank11Field2point3 = new Point(-1875, 625);

        Point g_Bank11Field1point1 = new Point(1500, 500);
        Point g_Bank11Field1point2 = new Point(0, 1000);
        Point g_Bank11Field1point3 = new Point(-1500, 500);

        Point g_Bank12Field3point1 = new Point(2250, 938);
        Point g_Bank12Field3point2 = new Point(0, 1875);
        Point g_Bank12Field3point3 = new Point(-2250, 938);

        Point g_Bank12Field2point1 = new Point(1875, 782);
        Point g_Bank12Field2point2 = new Point(0, 1563);
        Point g_Bank12Field2point3 = new Point(-1875, 782);

        Point g_Bank12Field1point1 = new Point(1500, 625);
        Point g_Bank12Field1point2 = new Point(0, 1250);
        Point g_Bank12Field1point3 = new Point(-1500, 625);

        Point g_Bank13Field3point1 = new Point(2250, 1125);
        Point g_Bank13Field3point2 = new Point(0, 2250);
        Point g_Bank13Field3point3 = new Point(-2250, 1125);

        Point g_Bank13Field2point1 = new Point(1875, 938);
        Point g_Bank13Field2point2 = new Point(0, 1875);
        Point g_Bank13Field2point3 = new Point(-1875, 938);

        Point g_Bank13Field1point1 = new Point(1500, 750);
        Point g_Bank13Field1point2 = new Point(0, 1500);
        Point g_Bank13Field1point3 = new Point(-1500, 750);

        Point g_Bank14Field3point1 = new Point(2250, 1313);
        Point g_Bank14Field3point2 = new Point(0, 2625);
        Point g_Bank14Field3point3 = new Point(-2250, 1313);

        Point g_Bank14Field2point1 = new Point(1875, 1094);
        Point g_Bank14Field2point2 = new Point(0, 2188);
        Point g_Bank14Field2point3 = new Point(-1875, 1094);

        Point g_Bank14Field1point1 = new Point(1500, 875);
        Point g_Bank14Field1point2 = new Point(0, 1750);
        Point g_Bank14Field1point3 = new Point(-1500, 875);

        Point g_Bank15Field3point1 = new Point(2625, 1313);
        Point g_Bank15Field3point2 = new Point(0, 2625);
        Point g_Bank15Field3point3 = new Point(-2625, 1313);

        Point g_Bank15Field2point1 = new Point(2188, 1094);
        Point g_Bank15Field2point2 = new Point(0, 2188);
        Point g_Bank15Field2point3 = new Point(-2188, 1094);

        Point g_Bank15Field1point1 = new Point(1750, 875);
        Point g_Bank15Field1point2 = new Point(0, 1750);
        Point g_Bank15Field1point3 = new Point(-1750, 875);

        Point g_Bank16Field3point1 = new Point(2625, 1500);
        Point g_Bank16Field3point2 = new Point(0, 3000);
        Point g_Bank16Field3point3 = new Point(-2625, 1500);

        Point g_Bank16Field2point1 = new Point(2188, 1250);
        Point g_Bank16Field2point2 = new Point(0, 2500);
        Point g_Bank16Field2point3 = new Point(-2188, 1250);

        Point g_Bank16Field1point1 = new Point(1750, 1000);
        Point g_Bank16Field1point2 = new Point(0, 2000);
        Point g_Bank16Field1point3 = new Point(-1750, 1000);

        #endregion

        //int g_n32rcvdbankdataflag = 0;                                         //接收到下位机发送来的区域数据

        List<Point> g_lpField1 = new List<Point>();                            //区域轮廓点
        List<Point> g_lpField1Temp = new List<Point>();                        //区域轮廓点临时存放数组
        List<AngleAndRadius> g_larField1 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larField1Caled = new List<AngleAndRadius>();    //区域每隔固定角度计算出的极坐标点

        List<Point> g_lpField2 = new List<Point>();                            //区域field2轮廓点
        List<Point> g_lpField2Temp = new List<Point>();                        //区域field2轮廓点临时存放数组
        List<AngleAndRadius> g_larField2 = new List<AngleAndRadius>();         //区域field2极坐标点
        List<AngleAndRadius> g_larField2Caled = new List<AngleAndRadius>();    //区域field2每隔固定角度计算出的极坐标点

        List<Point> g_lpField3 = new List<Point>();                            //区域field3轮廓点
        List<Point> g_lpField3Temp = new List<Point>();                        //区域field3轮廓点临时存放数组
        List<AngleAndRadius> g_larField3 = new List<AngleAndRadius>();         //区域field3极坐标点
        List<AngleAndRadius> g_larField3Caled = new List<AngleAndRadius>();    //区域field3每隔固定角度计算出的极坐标点


        //List<AngleAndRadius> g_larBank1_Vertex3 = new List<AngleAndRadius>();                             //区域1顶点存放数组
        //List<AngleAndRadius> g_larBank2_Vertex3 = new List<AngleAndRadius>();                             //区域2顶点存放数组
        //List<AngleAndRadius> g_larBank3_Vertex3 = new List<AngleAndRadius>();                             //区域3顶点存放数组
        //List<AngleAndRadius> g_larBank4_Vertex3 = new List<AngleAndRadius>();                             //区域4顶点存放数组
        //List<AngleAndRadius> g_larBank5_Vertex3 = new List<AngleAndRadius>();                             //区域5顶点存放数组
        //List<AngleAndRadius> g_larBank6_Vertex3 = new List<AngleAndRadius>();                             //区域6顶点存放数组
        //List<AngleAndRadius> g_larBank7_Vertex3 = new List<AngleAndRadius>();                             //区域7顶点存放数组
        //List<AngleAndRadius> g_larBank8_Vertex3 = new List<AngleAndRadius>();                             //区域8顶点存放数组      
        //List<AngleAndRadius> g_larBank9_Vertex3 = new List<AngleAndRadius>();                             //区域9顶点存放数组
        //List<AngleAndRadius> g_larBank10_Vertex3 = new List<AngleAndRadius>();                            //区域10顶点存放数组
        //List<AngleAndRadius> g_larBank11_Vertex3 = new List<AngleAndRadius>();                            //区域11顶点存放数组
        //List<AngleAndRadius> g_larBank12_Vertex3 = new List<AngleAndRadius>();                            //区域12顶点存放数组
        //List<AngleAndRadius> g_larBank13_Vertex3 = new List<AngleAndRadius>();                            //区域13顶点存放数组
        //List<AngleAndRadius> g_larBank14_Vertex3 = new List<AngleAndRadius>();                            //区域14顶点存放数组
        //List<AngleAndRadius> g_larBank15_Vertex3 = new List<AngleAndRadius>();                            //区域15顶点存放数组
        //List<AngleAndRadius> g_larBank16_Vertex3 = new List<AngleAndRadius>();                            //区域16顶点存放数组

        List<AngleAndRadius> g_larBank1_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点 lar:list angle and radius  发送至下位机
        List<AngleAndRadius> g_larBank1_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank1_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank2_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank2_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank2_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank3_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank3_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank3_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank4_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank4_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank4_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank5_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank5_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank5_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank6_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank6_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank6_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank7_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank7_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank7_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank8_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank8_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank8_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank9_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank9_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank9_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank10_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank10_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank10_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank11_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank11_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank11_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank12_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank12_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank12_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank13_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank13_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank13_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank14_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank14_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank14_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank15_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank15_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank15_Field3 = new List<AngleAndRadius>();         //区域极坐标点

        List<AngleAndRadius> g_larBank16_Field1 = new List<AngleAndRadius>();         //Bank1Field1区域极坐标点
        List<AngleAndRadius> g_larBank16_Field2 = new List<AngleAndRadius>();         //区域极坐标点
        List<AngleAndRadius> g_larBank16_Field3 = new List<AngleAndRadius>();         //区域极坐标点


        int g_n32SelectedPointFirst = 65535;    //第一次选中的点的索引
        int g_n32SelectedPointSecond = 65534;   //第二次选中的点的索引
        int g_n32MouseUpTimes = 0;              //鼠标弹起的次数

        #endregion

        #region 鼠标滑轮滚动
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (g_n32Scale <= 0.3 && e.Delta <= 0)
            {
                return;        //缩小下限
            }
            if (g_n32Scale >= 4.9 && e.Delta >= 0)
            {
                return;        //放大上限
            }

            SizeF szSub = (Size)g_epCanvasOrigin - (Size)e.Location;     //获取当前点到画布坐标原点的距离

            float tempX = szSub.Width / g_n32Scale;                      //当前的距离差除以缩放比还原到未缩放长度         
            float tempY = szSub.Height / g_n32Scale;

            g_epCanvasOrigin.X -= (int)(szSub.Width - tempX);            //还原上一次的偏移   
            g_epCanvasOrigin.Y -= (int)(szSub.Height - tempY);

            szSub.Width = tempX;                                        //重置距离差为未缩放状态  
            szSub.Height = tempY;
            g_n32Scale += e.Delta > 0 ? 0.2F : -0.2F;
            c_DrawCd.g_n32Scale = g_n32Scale;

            g_epCanvasOrigin.X += (int)(szSub.Width * g_n32Scale - szSub.Width);//重新计算缩放并重置画布原点坐标
            g_epCanvasOrigin.Y += (int)(szSub.Height * g_n32Scale - szSub.Height);

            c_DrawCd.g_epCanvasOrigin = g_epCanvasOrigin;
            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                       g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
            
            pictureBox1.Image = c_DrawCd.bitmap;
        }

        #endregion

        #region 鼠标按下
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {   //如果中键点下初始化计算要用的临时数据
                g_ptMouseDown = e.Location;
                g_ptCanvasBuf = g_epCanvasOrigin;
            }
            if (e.Button == MouseButtons.Left)
            {
                g_ptLstp.Clear();
                g_ptLste.Clear();
                g_ptStartPt.X = e.X;
                g_ptStartPt.Y = e.Y;
            }
            pictureBox1.Focus();
        }

        #endregion

        #region 鼠标弹起
        int g_n32minfielddisdiff = 60;     //field之间的最小间隔
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                g_n32MouseUpTimes++;

                #region 添加点
                if (e.Button == MouseButtons.Left && g_baddpointflag)
                {
                    Point l_pnewpoint = c_DrawCd.PixelPointtoScreenPoint(g_ptStartPt);        //将鼠标点转换为屏幕坐标点
                    g_lpField3.Add(l_pnewpoint);

                    CalculateRegionData();
                    c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                    pictureBox1.Image = c_DrawCd.bitmap;

                    g_n32MouseUpTimes = 0;
                }
                #endregion

                #region 删除点

                else if (e.Button == MouseButtons.Left && g_bdeletepointflag)
                {
                    float l_n32width = 7 * g_n32Scale;
                    if (g_n32MouseUpTimes % 2 == 1)
                    {
                        g_n32SelectedPointFirst = SelectPoint(g_lpField3,60);
                        c_DrawCd.DrawVertex(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3[g_n32SelectedPointFirst]), Color.DarkSeaGreen, l_n32width);
                        pictureBox1.Image = c_DrawCd.bitmap;
                    }
                    else if (g_n32MouseUpTimes % 2 == 0)
                    {
                        g_n32SelectedPointSecond = SelectPoint(g_lpField3,60);
                        if (g_n32SelectedPointFirst == g_n32SelectedPointSecond)
                        {
                            g_lpField3.RemoveAt(g_n32SelectedPointFirst);
                            g_n32SelectedPointFirst = 65535;
                            g_n32SelectedPointSecond = 65534;
                            //RedrawFieled1to4();
                            CalculateRegionData();
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                        g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;
                        }
                        else
                        {
                            c_DrawCd.DrawVertex(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3[g_n32SelectedPointFirst]), Color.Green, l_n32width);
                            pictureBox1.Image = c_DrawCd.bitmap;//还原选中点的颜色
                        }
                    }
                    else
                    {
                        g_n32MouseUpTimes = 0;
                        g_n32SelectedPointFirst = 65535;
                        g_n32SelectedPointSecond = 65534;
                    }


                    
                }
                #endregion

                #region 修改点

                else if (e.Button == MouseButtons.Left && g_bmodifypointflag)
                {
                    float l_n32width = 7 * g_n32Scale;
                    if (g_n32MouseUpTimes % 2 == 1)
                    {
                        g_lpField3Temp.Clear ();
                        if (g_n32BankNo < 9)
                        {
                            g_lpField3Temp.AddRange(g_lpField3);
                            if (g_n32BankNo > 3)
                            {
                                g_lpField3Temp.AddRange(g_lpField2);
                                g_lpField3Temp.AddRange(g_lpField1);
                            }
                        }
                        else
                        {
                            g_lpField3Temp.AddRange(g_lpField3Bank9_16);
                            g_lpField3Temp.AddRange(g_lpField2Bank9_16);
                            g_lpField3Temp.AddRange(g_lpField1Bank9_16);
                        }
                        g_n32SelectedPointFirst = SelectPoint(g_lpField3Temp,60);
                        if (g_n32SelectedPointFirst < 272)
                        {
                            c_DrawCd.DrawVertex(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3Temp[g_n32SelectedPointFirst]), Color.DarkSeaGreen, l_n32width);
                        }
                        else
                        {
                            c_DrawCd.DrawVertex(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3Temp[g_n32SelectedPointFirst]), Color.Green, l_n32width);
                        }
                        pictureBox1.Image = c_DrawCd.bitmap;
                    }
                    else if (g_n32MouseUpTimes % 2 == 0)
                    {
                        g_n32SelectedPointSecond = SelectPoint(g_lpField3Temp,60);
                        c_DrawCd.DrawVertex(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3Temp[g_n32SelectedPointFirst]), Color.Green, l_n32width);
                        pictureBox1.Image = c_DrawCd.bitmap;
                        if (g_n32SelectedPointSecond > 272)
                        {
                            return;
                        }
                        else
                        {
                            if (g_n32SelectedPointFirst == g_n32SelectedPointSecond)
                            {
                                if (g_n32BankNo < 5)
                                {
                                    if (JudgeSelectedPoint(g_lpField3, g_ptLstp[1], g_n32SelectedPointFirst))
                                    {
                                        g_lpField3[g_n32SelectedPointSecond] = g_ptLstp[1];
                                    }
                                }

                                #region bank5-bank8
                                else if (g_n32BankNo > 4 && g_n32BankNo < 9)
                                {
                                    if (g_n32SelectedPointSecond < 4)
                                    {
                                        if (JudgeSelectedPoint(g_lpField3, g_ptLstp[1], g_n32SelectedPointFirst))
                                        {
                                            List<AngleAndRadius> l_lartemp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField3);
                                            List<AngleAndRadius> l_lartemp1 = new List<AngleAndRadius>();
                                            AngleAndRadius l_artemp = c_DrawCd.ScreenPoint2PolarPoint(g_ptLstp[1]);

                                            List<AngleAndRadius> l_larfield2temp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2);
                                            if (l_artemp.RAD < l_larfield2temp[1].RAD)
                                            {
                                                l_artemp.RAD = l_larfield2temp[1].RAD + g_n32minfielddisdiff;
                                            }

                                            if (l_artemp.ANG > 585)
                                            {
                                                l_artemp.ANG = 585;
                                            }
                                            if (l_artemp.ANG < 315)
                                            {
                                                l_artemp.ANG = 315;
                                            }
                                            if (g_n32SelectedPointSecond == 1)   //角度不变，半径变
                                            {
                                                AngleAndRadius l_artemp1 = new AngleAndRadius(l_lartemp[0].ANG, l_artemp.RAD);
                                                AngleAndRadius l_artemp2 = new AngleAndRadius(l_lartemp[1].ANG, l_artemp.RAD);
                                                AngleAndRadius l_artemp3 = new AngleAndRadius(l_lartemp[2].ANG, l_artemp.RAD);
                                                l_lartemp1.Clear();
                                                l_lartemp1.Add(l_artemp1);
                                                l_lartemp1.Add(l_artemp2);
                                                l_lartemp1.Add(l_artemp3);
                                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(l_lartemp1);
                                            }
                                            else                                   //半径不变角度变
                                            {
                                                AngleAndRadius l_artemp1 = new AngleAndRadius(l_lartemp[0].ANG, l_lartemp[1].RAD);
                                                AngleAndRadius l_artemp2 = new AngleAndRadius(l_lartemp[1].ANG, l_lartemp[1].RAD);
                                                AngleAndRadius l_artemp3 = new AngleAndRadius(l_lartemp[2].ANG, l_lartemp[1].RAD);
                                                l_lartemp1.Clear();
                                                l_lartemp1.Add(l_artemp1);
                                                l_lartemp1.Add(l_artemp2);
                                                l_lartemp1.Add(l_artemp3);
                                                l_lartemp1[g_n32SelectedPointSecond].ANG = l_artemp.ANG;

                                                double l_n64startangle = l_lartemp1[0].ANG;
                                                double l_n64endangle = l_lartemp1[2].ANG;
                                                double l_n32sweepangle = Math.Abs(l_n64endangle - l_n64startangle);
                                                l_lartemp1[1].ANG = l_n64startangle + l_n32sweepangle / 2;

                                                List<AngleAndRadius> l_lartemp2 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2);
                                                List<AngleAndRadius> l_lartemp11 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField1);
                                                double l_n64radius2 = l_lartemp2[1].RAD;
                                                double l_n64radius1 = l_lartemp11[1].RAD;

                                                l_lartemp2.Clear();
                                                l_lartemp2.Add(new AngleAndRadius(l_n64startangle, l_n64radius2));
                                                l_lartemp2.Add(new AngleAndRadius(l_n64startangle + l_n32sweepangle / 2, l_n64radius2));
                                                l_lartemp2.Add(new AngleAndRadius(l_n64endangle, l_n64radius2));

                                                l_lartemp11.Clear();
                                                l_lartemp11.Add(new AngleAndRadius(l_n64startangle, l_n64radius1));
                                                l_lartemp11.Add(new AngleAndRadius(l_n64startangle + l_n32sweepangle / 2, l_n64radius1));
                                                l_lartemp11.Add(new AngleAndRadius(l_n64endangle, l_n64radius1));

                                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(l_lartemp1);
                                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(l_lartemp2);
                                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(l_lartemp11);
                                            }

                                        }
                                    }

                                    else if (g_n32SelectedPointSecond == 4)
                                    {
                                        if (JudgeSelectedPoint(g_lpField2, g_ptLstp[1], g_n32SelectedPointFirst - 3))
                                        {
                                            List<AngleAndRadius> l_lartemp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2);
                                            List<AngleAndRadius> l_lartemp1 = new List<AngleAndRadius>();
                                            AngleAndRadius l_artemp = c_DrawCd.ScreenPoint2PolarPoint(g_ptLstp[1]);

                                            List<AngleAndRadius> l_larfield3temp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField3);
                                            List<AngleAndRadius> l_larfield1temp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField1);

                                            if (l_artemp.RAD > l_larfield3temp[1].RAD)
                                            {
                                                l_artemp.RAD = l_larfield3temp[1].RAD - g_n32minfielddisdiff;
                                            }
                                            if (l_artemp.RAD < l_larfield1temp[1].RAD)
                                            {
                                                l_artemp.RAD = l_larfield1temp[1].RAD + g_n32minfielddisdiff;
                                            }

                                            AngleAndRadius l_artemp1 = new AngleAndRadius(l_lartemp[0].ANG, l_artemp.RAD);
                                            AngleAndRadius l_artemp2 = new AngleAndRadius(l_lartemp[1].ANG, l_artemp.RAD);
                                            AngleAndRadius l_artemp3 = new AngleAndRadius(l_lartemp[2].ANG, l_artemp.RAD);
                                            l_lartemp1.Clear();
                                            l_lartemp1.Add(l_artemp1);
                                            l_lartemp1.Add(l_artemp2);
                                            l_lartemp1.Add(l_artemp3);
                                            g_lpField2 = c_DrawCd.PolarPointToScreenPoint(l_lartemp1);
                                        }
                                    }
                                    else if (g_n32SelectedPointSecond == 7)
                                    {
                                        if (JudgeSelectedPoint(g_lpField1, g_ptLstp[1], g_n32SelectedPointFirst - 6))
                                        {
                                            List<AngleAndRadius> l_lartemp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField1);
                                            List<AngleAndRadius> l_lartemp1 = new List<AngleAndRadius>();
                                            AngleAndRadius l_artemp = c_DrawCd.ScreenPoint2PolarPoint(g_ptLstp[1]);

                                            List<AngleAndRadius> l_larfield2temp = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2);
                                            if (l_artemp.RAD > l_larfield2temp[1].RAD)
                                            {
                                                l_artemp.RAD = l_larfield2temp[1].RAD - g_n32minfielddisdiff;
                                            }
                                            if (l_artemp.RAD < g_n32minfielddisdiff)
                                            {
                                                l_artemp.RAD = g_n32minfielddisdiff;
                                            }
                                            AngleAndRadius l_artemp1 = new AngleAndRadius(l_lartemp[0].ANG, l_artemp.RAD);
                                            AngleAndRadius l_artemp2 = new AngleAndRadius(l_lartemp[1].ANG, l_artemp.RAD);
                                            AngleAndRadius l_artemp3 = new AngleAndRadius(l_lartemp[2].ANG, l_artemp.RAD);
                                            l_lartemp1.Clear();
                                            l_lartemp1.Add(l_artemp1);
                                            l_lartemp1.Add(l_artemp2);
                                            l_lartemp1.Add(l_artemp3);
                                            g_lpField1 = c_DrawCd.PolarPointToScreenPoint(l_lartemp1);
                                        }
                                    }
                                }
                                #endregion 

                                #region bank9-bank16
                                else 
                                {
                                    int l_n32tempX = 0;
                                    int l_n32tempY = 0;

                                    #region field3
                                    if (g_n32SelectedPointSecond < 3)
                                    {
                                        if (JudgeSelectedPoint(g_lpField3Bank9_16, g_ptLstp[1], g_n32SelectedPointFirst))
                                        {
                                            if (g_n32SelectedPointSecond == 0)
                                            {
                                                if (g_ptLstp[1].X < g_lpField2Bank9_16[0].X)
                                                {
                                                    l_n32tempX = g_lpField2Bank9_16[0].X + g_n32minfielddisdiff;
                                                }
                                                else
                                                {
                                                    l_n32tempX = g_ptLstp[1].X;
                                                }
                                                g_lpField3Bank9_16.Add(new Point(l_n32tempX, g_lpField3Bank9_16[0].Y));
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[1].X, g_lpField3Bank9_16[1].Y));
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[2].X, g_lpField3Bank9_16[2].Y));

                                                g_lpField3Bank9_16.RemoveRange(0, 3);
                                            }
                                            else if (g_n32SelectedPointSecond == 1)
                                            {
                                                if (g_ptLstp[1].Y < g_lpField2Bank9_16[1].Y)
                                                {
                                                    l_n32tempY = g_lpField2Bank9_16[1].Y + g_n32minfielddisdiff;
                                                }
                                                else
                                                {
                                                    l_n32tempY = g_ptLstp[1].Y;
                                                }
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[0].X, l_n32tempY / 2));
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[1].X, l_n32tempY));
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[2].X, l_n32tempY / 2));

                                                g_lpField3Bank9_16.RemoveRange(0, 3);
                                            }
                                            else
                                            {
                                                if (Math.Abs( g_ptLstp[1].X) <Math.Abs( g_lpField2Bank9_16[2].X))
                                                {
                                                    l_n32tempX = -(Math.Abs(g_lpField2Bank9_16[2].X) + g_n32minfielddisdiff);
                                                }
                                                else
                                                {
                                                    l_n32tempX = -(Math.Abs(g_ptLstp[1].X));
                                                }
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[0].X, g_lpField3Bank9_16[0].Y));
                                                g_lpField3Bank9_16.Add(new Point(g_lpField3Bank9_16[1].X, g_lpField3Bank9_16[1].Y));
                                                g_lpField3Bank9_16.Add(new Point(l_n32tempX, g_lpField3Bank9_16[2].Y));

                                                g_lpField3Bank9_16.RemoveRange(0, 3);
                                            }
                                        }

                                    }
                                    #endregion 

                                    #region field2
                                    else if (g_n32SelectedPointSecond > 2 && g_n32SelectedPointSecond < 6)
                                    {
                                        if (JudgeSelectedPoint(g_lpField2Bank9_16, g_ptLstp[1], g_n32SelectedPointFirst-3))
                                        {
                                            if (g_n32SelectedPointSecond == 3)
                                            {
                                                if (g_ptLstp[1].X <= g_lpField1Bank9_16[0].X)
                                                {
                                                    l_n32tempX = g_lpField1Bank9_16[0].X + g_n32minfielddisdiff;
                                                }
                                                else if (g_ptLstp[1].X >= g_lpField3Bank9_16[0].X)
                                                {
                                                    l_n32tempX = g_lpField3Bank9_16[0].X - g_n32minfielddisdiff;
                                                }
                                                else
                                                {
                                                    l_n32tempX = g_ptLstp[1].X;
                                                }
                                                g_lpField2Bank9_16.Add(new Point(l_n32tempX, g_lpField2Bank9_16[0].Y));
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[1].X, g_lpField2Bank9_16[1].Y));
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[2].X, g_lpField2Bank9_16[2].Y));

                                                g_lpField2Bank9_16.RemoveRange(0, 3);
                                            }
                                            else if (g_n32SelectedPointSecond == 4)
                                            {
                                                if (g_ptLstp[1].Y <= g_lpField1Bank9_16[1].Y)
                                                {
                                                    l_n32tempY = g_lpField1Bank9_16[1].Y + g_n32minfielddisdiff;
                                                }
                                                else if (g_ptLstp[1].Y >= g_lpField3Bank9_16[1].Y)
                                                {
                                                    l_n32tempY = g_lpField3Bank9_16[1].Y - g_n32minfielddisdiff;
                                                }
                                                else
                                                {
                                                    l_n32tempY = g_ptLstp[1].Y;
                                                }
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[0].X, l_n32tempY / 2));
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[1].X, l_n32tempY));
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[2].X, l_n32tempY / 2));

                                                g_lpField2Bank9_16.RemoveRange(0, 3);
                                            }
                                            else
                                            {
                                                if (Math.Abs(g_ptLstp[1].X) <= Math.Abs(g_lpField1Bank9_16[2].X))
                                                {
                                                    l_n32tempX = -(Math.Abs(g_lpField1Bank9_16[2].X) + g_n32minfielddisdiff);
                                                }
                                                else if (Math.Abs(g_ptLstp[1].X) >= Math.Abs(g_lpField3Bank9_16[2].X))
                                                {
                                                    l_n32tempX = -(Math.Abs(g_lpField3Bank9_16[2].X) - g_n32minfielddisdiff);
                                                }
                                                else
                                                {
                                                    l_n32tempX = -(Math.Abs(g_ptLstp[1].X));
                                                }
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[0].X, g_lpField2Bank9_16[0].Y));
                                                g_lpField2Bank9_16.Add(new Point(g_lpField2Bank9_16[1].X, g_lpField2Bank9_16[1].Y));
                                                g_lpField2Bank9_16.Add(new Point(l_n32tempX, g_lpField2Bank9_16[2].Y));

                                                g_lpField2Bank9_16.RemoveRange(0, 3);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region field1
                                    else if(g_n32SelectedPointSecond > 5 && g_n32SelectedPointSecond < 9)
                                    {
                                        if (JudgeSelectedPoint(g_lpField1Bank9_16, g_ptLstp[1], g_n32SelectedPointFirst-6))
                                        {
                                            if (g_n32SelectedPointSecond == 6)
                                            {
                                                if (g_ptLstp[1].X <= 0)
                                                {
                                                    l_n32tempX =  g_n32minfielddisdiff;
                                                }
                                                else if (g_ptLstp[1].X >= g_lpField2Bank9_16[0].X)
                                                {
                                                    l_n32tempX = g_lpField2Bank9_16[0].X - g_n32minfielddisdiff;
                                                }
                                                else
                                                {
                                                    l_n32tempX = g_ptLstp[1].X;
                                                }
                                                g_lpField1Bank9_16.Add(new Point(l_n32tempX, g_lpField1Bank9_16[0].Y));
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[1].X, g_lpField1Bank9_16[1].Y));
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[2].X, g_lpField1Bank9_16[2].Y));

                                                g_lpField1Bank9_16.RemoveRange(0, 3);
                                            }
                                            else if (g_n32SelectedPointSecond == 7)
                                            {
                                                if (g_ptLstp[1].Y <= 0)
                                                {
                                                    l_n32tempY = g_n32minfielddisdiff;
                                                }
                                                else if (g_ptLstp[1].Y >= g_lpField2Bank9_16[1].Y)
                                                {
                                                    l_n32tempY = g_lpField2Bank9_16[1].Y - g_n32minfielddisdiff;
                                                }
                                                else
                                                {
                                                    l_n32tempY = g_ptLstp[1].Y;
                                                }
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[0].X, l_n32tempY / 2));
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[1].X, l_n32tempY));
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[2].X, l_n32tempY / 2));

                                                g_lpField1Bank9_16.RemoveRange(0, 3);
                                            }
                                            else
                                            {
                                                if (Math.Abs(g_ptLstp[1].X) <= 0)
                                                {
                                                    l_n32tempX = -g_n32minfielddisdiff;
                                                }
                                                else if (Math.Abs(g_ptLstp[1].X) >= Math.Abs(g_lpField2Bank9_16[2].X))
                                                {
                                                    l_n32tempX = -(Math.Abs(g_lpField2Bank9_16[2].X) - g_n32minfielddisdiff);
                                                }
                                                else
                                                {
                                                    l_n32tempX = -(Math.Abs(g_ptLstp[1].X));
                                                }
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[0].X, g_lpField1Bank9_16[0].Y));
                                                g_lpField1Bank9_16.Add(new Point(g_lpField1Bank9_16[1].X, g_lpField1Bank9_16[1].Y));
                                                g_lpField1Bank9_16.Add(new Point(l_n32tempX, g_lpField1Bank9_16[2].Y));

                                                g_lpField1Bank9_16.RemoveRange(0, 3);
                                            }
                                        }

                                    }
                                    #endregion

                                }
                                #endregion 

                                CalculateRegionData();
                                c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                                pictureBox1.Image = c_DrawCd.bitmap;
                                g_n32SelectedPointFirst = 65535;
                                g_n32SelectedPointSecond = 65534;
                            }
                            else
                            {
                                c_DrawCd.DrawVertex(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3[g_n32SelectedPointFirst]), Color.Green, l_n32width);
                                pictureBox1.Image = c_DrawCd.bitmap;//还原选中点的颜色
                            }
                        }
                    }
                    else
                    {
                        g_n32MouseUpTimes = 0;
                        g_n32SelectedPointFirst = 65535;
                        g_n32SelectedPointSecond = 65534;
                    }
                    //SelectPointAndReplace(60);

                    
                }

                #endregion 

            }
            catch
            {
                //c_DrawCd.DrawRectanglePoint(c_DrawCd.ScreenPointtoPixelPoint(g_lpField3[g_n32SelectedPointFirst]), Color.Green, 7 * g_n32Scale);
                //pictureBox1.Image = c_DrawCd.bitmap;//还原选中点的颜色
                //重新绘制轮廓
            }
        }
        #endregion

        #region 鼠标移动
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {   //移动过程中，中键点下重置画布坐标系
                    //我总感觉这样写不妥 但却是方便计算，如果多次这样搞的话还是重载操作符吧
                    g_epCanvasOrigin = (Point)((Size)g_ptCanvasBuf + ((Size)e.Location - (Size)g_ptMouseDown));
                    c_DrawCd.g_epCanvasOrigin = g_epCanvasOrigin;
                    c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                        g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
  
                    pictureBox1.Image = c_DrawCd.bitmap;
                }

                // SizeF ef = (Size)g_epCanvasOrigin;                         //计算鼠标当前点对应画布中的坐标(屏幕坐标)
                SizeF szSub = (Size)e.Location - (Size)g_epCanvasOrigin;   //计算鼠标当前点对应画布中的坐标
                szSub.Width /= g_n32Scale;
                szSub.Height /= g_n32Scale;
                szSub.Width *= 10;                                         //每一格作图间隔为50，实际显示的坐标为500，因此此处再乘以10
                szSub.Height *= -10;
                double dis = Math.Sqrt(Math.Pow(szSub.Width, 2) + Math.Pow(szSub.Height, 2));
                double ang = Math.Atan(Math.Abs(szSub.Height) / Math.Abs(szSub.Width)) / Math.PI * 180;
                double ang2 = 0;
                if (szSub.Width > 0 && szSub.Height > 0)
                {
                    ang2 = ang;
                }
                else if (szSub.Width < 0 && szSub.Height > 0)
                {
                    ang2 = 90 - ang + 90;
                }
                else if (szSub.Width < 0 && szSub.Height < 0)
                {
                    ang2 = ang + 180;

                }
                else if (szSub.Width > 0 && szSub.Height < 0)
                {
                    ang2 = 90 - ang + 270;
                }
                else if (szSub.Width >= 0 && szSub.Height == 0)
                {
                    ang2 = 0;
                }
                else if (szSub.Width == 0 && szSub.Height > 0)
                {
                    ang2 = 90;
                }
                else if (szSub.Width < 0 && szSub.Height == 0)
                {
                    ang2 = 180;
                }
                else if (szSub.Width == 0 && szSub.Height < 0)
                {
                    ang2 = 270;
                }
                if (g_n32drawstaangle == 1)
                {
                    if (ang2 >= 0 && ang2 < 315)
                    {
                        ang2 += 45;
                    }
                    else
                    {
                        ang2 -= 315;
                    }
                }
                else if (g_n32drawstaangle == 0)
                {
                    if (ang2 >= 0 && ang2 <= 225)
                    {
                        ang2 = 225-ang2;
                    }
                    else
                    {
                        ang2 = 270-ang2+315;
                    }
                }

                label45.Text = "x: " + ((int)szSub.Width).ToString() + " mm" +" , " +  //显示鼠标位置
                               "y: " + ((int)szSub.Height).ToString() + " mm";

                label37.Text = "距离：" + ((int)dis).ToString() + " mm" + " , " +
                               "角度：" + Math.Round(ang2, 2).ToString() + "°";

                g_ptEndPt.X = e.X;
                g_ptEndPt.Y = e.Y;
                c_DrawCd.graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Pen redpen = new Pen(Color.Red, 1);
                redpen.StartCap = LineCap.Round; //设置起止点线帽
                redpen.EndCap = LineCap.Round;
                redpen.LineJoin = LineJoin.Round;//设置连续两段的连接样式

                if (e.Button == MouseButtons.Left && g_bmodifypointflag)
                {
                    g_ptLstp.Add(new Point((int)szSub.Width, (int)szSub.Height));
                    g_ptLste.Add(new Point(e.X, e.Y));
                    c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                       g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);

                    if (g_ptLste.Count > 2)
                    {
                        g_ptLstp.Clear();
                        g_ptLstp.Add(g_ptLste[0]);
                        g_ptLstp.Add(g_ptLste[g_ptLste.Count - 1]);
                        g_ptLstp = c_DrawCd.PixelPointtoScreenPoint(g_ptLstp);//像素点转换为屏幕点
                        List<Point> l_lpFieldTemp = new List<Point>();
                        if (g_n32BankNo < 5)                                                     //BANK1-4
                        {
                            if (JudgePoint(g_lpField3, g_n32SelectedPointFirst, 60))    //判断选中的点
                            {
                                if (JudgeSelectedPoint(g_lpField3Temp, g_ptLstp[1], g_n32SelectedPointFirst))
                                {
                                    l_lpFieldTemp.Clear();
                                    for (int i = 0; i < g_lpField3.Count; i++)
                                    {
                                        l_lpFieldTemp.Add(g_lpField3[i]);
                                    }

                                    l_lpFieldTemp[g_n32SelectedPointFirst] = g_ptLstp[1];
                                    c_DrawCd.FillPathColor(l_lpFieldTemp, Color.Yellow);
                                    pictureBox1.Image = c_DrawCd.bitmap;
                                }
                            }
                        }
                        else if (g_n32BankNo > 4 && g_n32BankNo < 9)                             //BANK5-8
                        {
                            if (JudgePoint(g_lpField3Temp, g_n32SelectedPointFirst, 60))    //判断选中的点
                            {
                                int l_n32selectindex = 0;
                                if (g_n32SelectedPointFirst < 3)
                                {
                                    if (JudgeSelectedPoint(g_lpField3, g_ptLstp[1], g_n32SelectedPointFirst))
                                    {
                                        l_n32selectindex = g_n32SelectedPointFirst;
                                        l_lpFieldTemp.Clear();
                                        l_lpFieldTemp.Add(g_lpField3[0]);
                                        l_lpFieldTemp.Add(g_lpField3[1]);
                                        l_lpFieldTemp.Add(g_lpField3[2]);
                                    }
                                }
                                else if (g_n32SelectedPointFirst == 4) //(g_n32SelectedPointFirst > 2 && g_n32SelectedPointFirst < 6)
                                {
                                    l_n32selectindex = g_n32SelectedPointFirst - 3;
                                    if (JudgeSelectedPoint(g_lpField2, g_ptLstp[1], l_n32selectindex))
                                    {
                                        l_lpFieldTemp.Clear();
                                        l_lpFieldTemp.Add(g_lpField2[0]);
                                        l_lpFieldTemp.Add(g_lpField2[1]);
                                        l_lpFieldTemp.Add(g_lpField2[2]);
                                    }
                                }
                                else if (g_n32SelectedPointFirst == 7)//else
                                {
                                    l_n32selectindex = g_n32SelectedPointFirst - 6;
                                    if (JudgeSelectedPoint(g_lpField1, g_ptLstp[1], l_n32selectindex))
                                    {
                                        l_lpFieldTemp.Clear();
                                        l_lpFieldTemp.Add(g_lpField1[0]);
                                        l_lpFieldTemp.Add(g_lpField1[1]);
                                        l_lpFieldTemp.Add(g_lpField1[2]);
                                    }
                                }
                                l_lpFieldTemp[l_n32selectindex] = g_ptLstp[1];
                                c_DrawCd.FillPathColor(l_lpFieldTemp, Color.Yellow);
                                pictureBox1.Image = c_DrawCd.bitmap;
                            }
                        }
                        else
                        {
                            if (JudgePoint(g_lpField3Temp, g_n32SelectedPointFirst, 60))    //判断选中的点
                            {
                                int l_n32selectindex = 0;
                                if (g_n32SelectedPointFirst < 3)
                                {
                                    if (JudgeSelectedPoint(g_lpField3Bank9_16, g_ptLstp[1], g_n32SelectedPointFirst))
                                    {
                                        l_n32selectindex = g_n32SelectedPointFirst;
                                        l_lpFieldTemp.Clear();
                                        l_lpFieldTemp.Add(g_lpField3Bank9_16[0]);
                                        l_lpFieldTemp.Add(g_lpField3Bank9_16[1]);
                                        l_lpFieldTemp.Add(g_lpField3Bank9_16[2]);
                                    }
                                }
                                else if (g_n32SelectedPointFirst > 2 && g_n32SelectedPointFirst < 6) //(g_n32SelectedPointFirst > 2 && g_n32SelectedPointFirst < 6)
                                {
                                    l_n32selectindex = g_n32SelectedPointFirst - 3;
                                    if (JudgeSelectedPoint(g_lpField2Bank9_16, g_ptLstp[1], l_n32selectindex))
                                    {
                                        l_lpFieldTemp.Clear();
                                        l_lpFieldTemp.Add(g_lpField2Bank9_16[0]);
                                        l_lpFieldTemp.Add(g_lpField2Bank9_16[1]);
                                        l_lpFieldTemp.Add(g_lpField2Bank9_16[2]);
                                    }
                                }
                                else if (g_n32SelectedPointFirst > 5 && g_n32SelectedPointFirst < 9)//else
                                {
                                    l_n32selectindex = g_n32SelectedPointFirst - 6;
                                    if (JudgeSelectedPoint(g_lpField1Bank9_16, g_ptLstp[1], l_n32selectindex))
                                    {
                                        l_lpFieldTemp.Clear();
                                        l_lpFieldTemp.Add(g_lpField1Bank9_16[0]);
                                        l_lpFieldTemp.Add(g_lpField1Bank9_16[1]);
                                        l_lpFieldTemp.Add(g_lpField1Bank9_16[2]);
                                    }
                                }
                                l_lpFieldTemp[l_n32selectindex] = g_ptLstp[1];
                                c_DrawCd.FillPathColor(l_lpFieldTemp, Color.Yellow);
                                pictureBox1.Image = c_DrawCd.bitmap;
                            }
                        }
                  

                        


                    }
                }
            }
            catch 
            {
                return;
            }

        }



        #endregion

        #region picturebox1大小改变
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            g_n32BitMapWidth = pictureBox1.Width;
            g_n32BitMapHeight = pictureBox1.Height;
            g_epCanvasOrigin = new Point(pictureBox1.Width / 2, pictureBox1.Height / 5 * 4); //画布原点在设备上的坐标

            c_DrawCd.bitmap = new Bitmap(g_n32BitMapWidth, g_n32BitMapHeight);
            c_DrawCd.graphics = Graphics.FromImage(c_DrawCd.bitmap);
            c_DrawCd.PicBox = pictureBox1;
            c_DrawCd.g_n32Scale = g_n32Scale;
            c_DrawCd.g_epCanvasOrigin = g_epCanvasOrigin;
            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                        g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);

            pictureBox1.Image = c_DrawCd.bitmap;
        }
        #endregion

        #region 选点

        private int SelectPoint( List<Point> p_lpField,int p_n32error)
        {
            int l_n32index = 65536;
            int l_n32len = p_lpField.Count;
            Point l_pnewpointsp = c_DrawCd.PixelPointtoScreenPoint(g_ptStartPt);        //将鼠标点转换为屏幕坐标点
            if (l_n32len >= 3)
            {
                for (int i = 0; i < l_n32len; i++)
                {
                    if ((p_lpField[i].X - p_n32error < l_pnewpointsp.X && l_pnewpointsp.X < p_lpField[i].X + p_n32error) &&
                        (p_lpField[i].Y - p_n32error < l_pnewpointsp.Y && l_pnewpointsp.Y < p_lpField[i].Y + p_n32error))
                    {

                        l_n32index = i;
                        break;
                    }
                }
            }
            return l_n32index;
        }
        private bool JudgePoint(List<Point> p_lpField, int p_n32index,int p_n32error)
        {
            Point l_pnewpointsp = c_DrawCd.PixelPointtoScreenPoint(g_ptStartPt);        //将鼠标点转换为屏幕坐标点

            if ((p_lpField[p_n32index].X - p_n32error < l_pnewpointsp.X && l_pnewpointsp.X < p_lpField[p_n32index].X + p_n32error) &&
               (p_lpField[p_n32index].Y - p_n32error < l_pnewpointsp.Y && l_pnewpointsp.Y < p_lpField[p_n32index].Y + p_n32error))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 判断移动后的点在两个相邻的点的角度范围内

        private bool JudgeSelectedPoint(List<Point> p_lsppath,Point p_spendpoint ,int p_n32selectedindex)
        {
            double l_n64angle1 =0;
            if (p_n32selectedindex == 0)
            {
                l_n64angle1= 0;
            }
            else
            {
                l_n64angle1 = c_DrawCd.PointToAngle(p_lsppath[p_n32selectedindex - 1]);
            }
            double l_n64angle2 = c_DrawCd.PointToAngle(p_spendpoint);
            double l_n64angle3;
            if (p_n32selectedindex == p_lsppath.Count - 1)
            {
                l_n64angle3 = 635;
            }
            else
            {
                l_n64angle3 = c_DrawCd.PointToAngle(p_lsppath[p_n32selectedindex + 1]);
            }
            double l_n64anglemin = Math.Min(l_n64angle1, l_n64angle3);
            double l_n64anglemax = Math.Max(l_n64angle1, l_n64angle3);
            if (l_n64anglemin < l_n64angle2 && l_n64angle2 < l_n64anglemax)
            {
                return true;
            }
            else
                return false;
        }

        #endregion

        #region 选点替换
        private void SelectPointAndReplace(int p_n32error)
        {
            int l_n32len = g_lpField3.Count;
            if (l_n32len > 3)
            {
                for (int i = 0; i < l_n32len; i++)
                {
                    if ((g_lpField3[i].X - p_n32error < g_ptLstp[0].X && g_ptLstp[0].X < g_lpField3[i].X + p_n32error) &&
                        (g_lpField3[i].Y - p_n32error < g_ptLstp[0].Y && g_ptLstp[0].Y < g_lpField3[i].Y + p_n32error))
                    {
                        double l_n64angle1 = c_DrawCd.PointToAngle(g_lpField3[i - 1]);
                        double l_n64angle2 = c_DrawCd.PointToAngle(g_ptLstp[1]);
                        double l_n64angle3 = c_DrawCd.PointToAngle(g_lpField3[i + 1]);
                        double l_n64anglemin = Math.Min(l_n64angle1, l_n64angle3);
                        double l_n64anglemax = Math.Max(l_n64angle1, l_n64angle3);
                        if (l_n64anglemin < l_n64angle2 && l_n64angle2 < l_n64anglemax)
                        {
                            g_lpField3[i] = g_ptLstp[1];
                        }
                        break;
                    }
                }
            }
        }

        #endregion

        #region 区域数据计算

        int g_n32BankNo = 1;
        List<Point> g_lpField3Bank9_16 = new List<Point>();                            //区域field3轮廓点
        List<Point> g_lpField2Bank9_16 = new List<Point>();                            //区域field2轮廓点
        List<Point> g_lpField1Bank9_16 = new List<Point>();                            //区域field1轮廓点

        private void CalculateRegionData()
        {
            if (g_n32BankNo < 9)
            {
                g_larField3 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField3);
                g_larField3 = c_DrawCd.SortPointListAsAngleS2B(g_larField3);                        //按角度小到大排序
                //发送个下位机的轮廓顶点
                g_larField3 = c_DrawCd.DeleteSelectedAngle(270.1, 315.0, g_larField3);              //删除指定角度的数据
                g_larField3 = c_DrawCd.DeleteSelectedAngle(585.1, 630.0, g_larField3);
                g_larField3Caled = c_DrawCd.CalAngleRadius(g_larField3, 1.0f, g_n32BankNo);         //每1度计算一个距离值
                //发送给下位机的每1个角度值对应的距离值
                g_larField3Caled = c_DrawCd.DeleteSelectedAngle(270.1, 315.0, g_larField3Caled);    //删除指定角度的数据
                g_larField3Caled = c_DrawCd.DeleteSelectedAngle(585.1, 630.0, g_larField3Caled);
                //绘图时顶点数据
                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larField3);                          //极坐标点转换为直角坐标系点

                if (g_n32BankNo < 5)
                {
                    //发送给下位机的每1个角度值对应的距离值
                    g_larField2Caled = c_DrawCd.ListPolarMultiple(g_larField3Caled, 0.822368f);
                    g_larField1Caled = c_DrawCd.ListPolarMultiple(g_larField3Caled, 0.65789f);
                    //发送给下位机的顶点距离值               
                    g_larField2 = c_DrawCd.ListPolarMultiple(g_larField3, 0.822368f);
                    g_larField1 = c_DrawCd.ListPolarMultiple(g_larField3, 0.65789f);
                    //绘图时阴影数据
                    g_lpField2 = c_DrawCd.ListScreenMultiple(g_lpField3, 0.822368f);
                    g_lpField1 = c_DrawCd.ListScreenMultiple(g_lpField3, 0.65789f);
                }
                else if (g_n32BankNo > 4 && g_n32BankNo < 9)
                {
                    //发送给下位机的顶点
                    g_larField2 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2);
                    g_larField2 = c_DrawCd.SortPointListAsAngleS2B(g_larField2);                        //按角度小到大排序
                    //发送给下位机的安全区域
                    g_larField2Caled = c_DrawCd.CalAngleRadius(g_larField2, 1.0f, g_n32BankNo);         //每1度计算一个距离值
                    //绘图时使用的顶点值
                    g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larField2);                         //极坐标点转换为直角坐标系点

                    g_larField1 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField1);
                    g_larField1 = c_DrawCd.SortPointListAsAngleS2B(g_larField1);                        //按角度小到大排序
                    g_larField1Caled = c_DrawCd.CalAngleRadius(g_larField1, 1.0f, g_n32BankNo);         //每1度计算一个距离值
                    g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larField1);                         //极坐标点转换为直角坐标系点
                }
            }
            else 
            {
                g_lpField3.Clear();
                g_lpField2.Clear();
                g_lpField1.Clear();

                g_lpField3.Add(new Point(g_lpField3Bank9_16[0].X, 0));                         //区域3的矩形四个顶点
                g_lpField3.Add(new Point(g_lpField3Bank9_16[0].X, g_lpField3Bank9_16[1].Y));
                g_lpField3.Add(new Point(g_lpField3Bank9_16[2].X, g_lpField3Bank9_16[1].Y));
                g_lpField3.Add(new Point(g_lpField3Bank9_16[2].X, 0));

                g_lpField2.Add(new Point(g_lpField2Bank9_16[0].X, 0));                         //区域2的矩形四个顶点
                g_lpField2.Add(new Point(g_lpField2Bank9_16[0].X, g_lpField2Bank9_16[1].Y));
                g_lpField2.Add(new Point(g_lpField2Bank9_16[2].X, g_lpField2Bank9_16[1].Y));
                g_lpField2.Add(new Point(g_lpField2Bank9_16[2].X, 0));

                g_lpField1.Add(new Point(g_lpField1Bank9_16[0].X, 0));                          //区域1的矩形四个顶点
                g_lpField1.Add(new Point(g_lpField1Bank9_16[0].X, g_lpField1Bank9_16[1].Y));
                g_lpField1.Add(new Point(g_lpField1Bank9_16[2].X, g_lpField1Bank9_16[1].Y));
                g_lpField1.Add(new Point(g_lpField1Bank9_16[2].X, 0));

                //fild3数据
                List<AngleAndRadius> l_larField3 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField3);
                l_larField3 = c_DrawCd.SortPointListAsAngleS2B(l_larField3);                        //按角度小到大排序
                //发送给下位机的每1个角度值对应的距离值
                g_larField3Caled = c_DrawCd.CalAngleRadius(l_larField3, 1.0f, g_n32BankNo);         //每1度计算一个距离值
                //发送给下位机绘图时顶点数据
                g_larField3 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField3Bank9_16);
                g_larField3 = c_DrawCd.SortPointListAsAngleS2B(g_larField3);                        //按角度小到大排序

                //fild2数据
                List<AngleAndRadius> l_larField2 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2);
                l_larField2 = c_DrawCd.SortPointListAsAngleS2B(l_larField2);                        //按角度小到大排序
                //发送给下位机的每1个角度值对应的距离值
                g_larField2Caled = c_DrawCd.CalAngleRadius(l_larField2, 1.0f, g_n32BankNo);         //每1度计算一个距离值
                //发送给下位机绘图时顶点数据
                g_larField2 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField2Bank9_16);
                g_larField2 = c_DrawCd.SortPointListAsAngleS2B(g_larField2);                        //按角度小到大排序

                //fild1数据
                List<AngleAndRadius> l_larField1 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField1);
                l_larField1 = c_DrawCd.SortPointListAsAngleS2B(l_larField1);                        //按角度小到大排序
                //发送给下位机的每1个角度值对应的距离值
                g_larField1Caled = c_DrawCd.CalAngleRadius(l_larField1, 1.0f, g_n32BankNo);         //每1度计算一个距离值
                //发送给下位机绘图时顶点数据
                g_larField1 = c_DrawCd.ScreenPointListToPolarPointList(g_lpField1Bank9_16);
                g_larField1 = c_DrawCd.SortPointListAsAngleS2B(g_larField1);                        //按角度小到大排序
              
            }
            SortRegionData();
        }

       
        #endregion

        #region 区域数据组合 校验发送
        private void PolarRaduis2ByteArray( List<AngleAndRadius> p_larradius, int p_n32checkindex, byte[] p_absourcedata)
        {
            int l_n32len = p_larradius.Count;

            for (int i = 0; i < l_n32len; i++)
            {
                int l_n32radius = (int)Math.Round(p_larradius[i].RAD, 0);
                int l_n32dataindex = ((int)Math.Round(p_larradius[i].ANG, 0) - 315) * 2 + 26;  //角度数据存放的起始地址
               
                p_absourcedata[l_n32dataindex] = (byte)((l_n32radius & 0xFF00) >> 8);
                p_absourcedata[l_n32dataindex + 1] = (byte)(l_n32radius & 0xFF);
            }

            p_absourcedata[p_n32checkindex] = ToolFunc.XorCheck_byte(2, p_absourcedata, 4);  //校验
            p_absourcedata[p_n32checkindex + 1] = 0xEE;
            p_absourcedata[p_n32checkindex + 2] = 0xEE;
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(p_absourcedata, 0, p_absourcedata.Length);
            }
        }

        #endregion

        #region 区域数据下载
        private void tsbtn_download_Click(object sender, EventArgs e)
        {

            int l_n32TotalLen = 572;                                  //发送数据的总长度包含帧头帧尾和校验位
            int l_n32CheckLoc = l_n32TotalLen - 3;                     //校验位位置
            int l_n32ClearLen = 572-25;

            byte[] l_abRegionDatToSend = new byte[l_n32TotalLen];
            byte[] l_abRegionDatToSendson = new byte[26]{0xFF,0xAA,0x02,0x38,0x00,0x00,0x00,0x00,0x00,
                                                         0x00,0x01,0x01,0x00,0x05,0x00,0x00,0x00,0x00,
                                                         0x00,0x00,0x00,0x00,0x09,0x02,0x00,0x00};
            try
            {
                Array.Copy(l_abRegionDatToSendson, l_abRegionDatToSend, 26);      //包头
                l_abRegionDatToSend[24] = Convert.ToByte(1);                      //包编号

                CalculateRegionData();                                            //下载前计算数据

                if (g_timer1 != null)
                {
                    g_timer1.Stop();
                }

                if (g_timer2 != null)
                {
                    g_timer2.Stop();
                }

                for (int i = 1; i < 7; i++)
                {
                    Array.Clear(l_abRegionDatToSend, 25, l_n32ClearLen);  //清除除去包头以外的数据
                    if (i == 1)
                    { 
                        l_abRegionDatToSend[25] = 0x01;
                        PolarRaduis2ByteArray(g_larField1Caled, l_n32CheckLoc, l_abRegionDatToSend);
                    }
                    else if (i == 2)
                    { 
                        l_abRegionDatToSend[25] = 0x02;
                        PolarRaduis2ByteArray(g_larField2Caled, l_n32CheckLoc, l_abRegionDatToSend);
                    }
                    else if(i==3)
                    { 
                         l_abRegionDatToSend[25] = 0x03;
                         PolarRaduis2ByteArray(g_larField3Caled, l_n32CheckLoc, l_abRegionDatToSend);
                    }
                    else if (i == 4)
                    {
                        l_abRegionDatToSend[25] = 0x04;
                        PolarRaduis2ByteArray(g_larField1, l_n32CheckLoc, l_abRegionDatToSend);
                    }
                    else if (i == 5)
                    {
                        l_abRegionDatToSend[25] = 0x05;
                        PolarRaduis2ByteArray(g_larField2, l_n32CheckLoc, l_abRegionDatToSend);
                    }
                    else 
                    {
                        l_abRegionDatToSend[25] = 0x06;
                        PolarRaduis2ByteArray(g_larField3, l_n32CheckLoc, l_abRegionDatToSend);
                    }

                    Delayms(20);
                }
                if (g_timer1 != null)
                {
                    g_timer1.Start();
                }

                if (g_timer2 != null)
                {
                    g_timer2.Start();
                }

            }
            catch
            { }
        }
        #endregion

        #region 区域数据按bank进行分类
        private void SortRegionData()
        {
            switch (g_n32BankNo)
            {
                case 1:
                    {
                        g_larBank1_Field1 = g_larField1;
                        g_larBank1_Field2 = g_larField2;
                        g_larBank1_Field3 = g_larField3;
                    }
                    break;
                case 2:
                    {
                        g_larBank2_Field1 = g_larField1;
                        g_larBank2_Field2 = g_larField2;
                        g_larBank2_Field3 = g_larField3;
                    }
                    break;
                case 3:
                    {
                        g_larBank3_Field1 = g_larField1;
                        g_larBank3_Field2 = g_larField2;
                        g_larBank3_Field3 = g_larField3;
                    }
                    break;
                case 4:
                    {
                        g_larBank4_Field1 = g_larField1;
                        g_larBank4_Field2 = g_larField2;
                        g_larBank4_Field3 = g_larField3;
                    }
                    break;

                case 5:
                    {
                        g_larBank5_Field1 = g_larField1;
                        g_larBank5_Field2 = g_larField2;
                        g_larBank5_Field3 = g_larField3;
                    }
                    break;
                case 6:
                    {
                        g_larBank6_Field1 = g_larField1;
                        g_larBank6_Field2 = g_larField2;
                        g_larBank6_Field3 = g_larField3;
                    }
                    break;
                case 7:
                    {
                        g_larBank7_Field1 = g_larField1;
                        g_larBank7_Field2 = g_larField2;
                        g_larBank7_Field3 = g_larField3;
                    }
                    break;
                case 8:
                    {
                        g_larBank8_Field1 = g_larField1;
                        g_larBank8_Field2 = g_larField2;
                        g_larBank8_Field3 = g_larField3;
                    }
                    break;

                case 9:
                    {
                        g_larBank9_Field1 = g_larField1;
                        g_larBank9_Field2 = g_larField2;
                        g_larBank9_Field3 = g_larField3;
                    }
                    break;

                case 10:
                    {
                        g_larBank10_Field1 = g_larField1;
                        g_larBank10_Field2 = g_larField2;
                        g_larBank10_Field3 = g_larField3;
                    }
                    break;

                case 11:
                    {
                        g_larBank11_Field1 = g_larField1;
                        g_larBank11_Field2 = g_larField2;
                        g_larBank11_Field3 = g_larField3;
                    }
                    break;

                case 12:
                    {
                        g_larBank12_Field1 = g_larField1;
                        g_larBank12_Field2 = g_larField2;
                        g_larBank12_Field3 = g_larField3;
                    }
                    break;

                case 13:
                    {
                        g_larBank13_Field1 = g_larField1;
                        g_larBank13_Field2 = g_larField2;
                        g_larBank13_Field3 = g_larField3;
                    }
                    break;

                case 14:
                    {
                        g_larBank14_Field1 = g_larField1;
                        g_larBank14_Field2 = g_larField2;
                        g_larBank14_Field3 = g_larField3;
                    }
                    break;

                case 15:
                    {
                        g_larBank15_Field1 = g_larField1;
                        g_larBank15_Field2 = g_larField2;
                        g_larBank15_Field3 = g_larField3;
                    }
                    break;

                case 16:
                    {
                        g_larBank16_Field1 = g_larField1;
                        g_larBank16_Field2 = g_larField2;
                        g_larBank16_Field3 = g_larField3;
                    }
                    break;


                default:
                    break;
            }
   
        }

        #endregion 

        #endregion

        #endregion

        #region 延时函数（ms）
        public void Delayms(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }
        #endregion 

        #region 程序更新

        Thread com2sendthread;                    //程序更新串口发送线程
        public int g_n32ProgramFlag = 0;          //开始发送标志位
        public int g_n32ProgramMode = 0;            //0:空闲状态 1：发送连接请求，等待回复  2：收到回复建立连接  3：开始传输数据          
        volatile uint r_flag;                     //上位机发送arm程序时接收到的下位机回复标志，上位机发送程序数据时清零，接收到数据置1；
        byte[] g_budpsendbuf = new byte[1024];	  //发送缓存
        volatile int g_n32sendlen = 0;			  //发送长度
        public string g_sbinpath = "";            //主控程序二进制文件路径
        public static int SEND_MAX = 1000;
        FileStream fs;

        public int g_n32CodesType = 0;            //程序类型选择：ARM:1  FPGA:2
        public static int ARM_SEND_MAX = 256;
        byte[] g_abfpgacode = new byte[290];	  //socket发送缓存
        public bool g_bfpgasendover = false;      //烧写完成标志位
        public bool g_bfpgaupdatesuccess = true;  //烧写成功标志位
        public AutoResetEvent FPGASendResetEvent = new AutoResetEvent(false);
        public int g_n32FPGARetryNo = 0;         //烧写FPGA程序时重新下载次数
        public int g_n32FPGATotalPkgs = 0;       //程序总包数
        public Thread FPGAThread ;               //FPGA程序更新

        #region 查找串口

        private void ResearchSerialPort()
        {
            string[] l_asportname = SerialPort.GetPortNames();//检查是否含有串口 
            if (l_asportname.Length < 1)
            {
                MessageBox_Invoke("本机没有串口！");
                return;
            }
            btn_updataopencom.Enabled = true;
            btn_openSerial_app.Enabled = true;
            comb_updatacomport.Items.Clear();
            comb_serialport_app.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())   //添加串口
            {
                comb_serialport_app.Items.Add(s);　　　　　　　　　　//获取有多少个COM口，添加到控件里
                comb_updatacomport.Items.Add(s);
            }
            comb_serialbaudapp.SelectedIndex = 1;
            comb_serialport_app.SelectedIndex = 0;

            comb_updatacomport.SelectedIndex = 0;
            comb_updatacombaud.SelectedIndex = 0;
        }

        private void btn_updatasearchcom_Click(object sender, EventArgs e)
        {
            ResearchSerialPort();
        }
        #endregion

        #region 打开串口

        private bool g_bcom2listening;
        private void btn_updataopencom_Click(object sender, EventArgs e)
        {
            try
            {
                g_bmanualopencom1 = false;
                if (g_n32CodesType == 1)          //ARM程序更新
                {
                    if (serialPort1.IsOpen)
                    {
                        closing = true;                //正在关闭串口
                        while (Listening)
                            Application.DoEvents();
                        serialPort1.Close();
                        closing = false;              //关闭串口完成
                        btn_openSerial_app.Text = "打开串口";
                        progressBar1.Value = 0;
                        label_Communication.Text = "串口已关闭";
                        if (g_timer1 != null)
                        {
                            g_timer1.Stop();
                        }
                        if (g_timer2 != null)
                        {
                            g_timer2.Stop();
                        }
                    }
                    if (serialPort2.IsOpen == true)
                    {
                        MethodInvoker mi = new MethodInvoker(closecom2);
                        this.BeginInvoke(mi);
                        g_n32ProgramFlag = 0;
                    }
                    else
                    {
                        try
                        {
                            serialPort2.PortName = comb_updatacomport.Text;
                            serialPort2.BaudRate = int.Parse(comb_updatacombaud.Text);
                            serialPort2.Open();
                            btn_updataopencom.Text = "关闭串口";
                            serialPort2.DataReceived += com2recfun;   //串口接收
                            btn_checkport.Enabled = true;
                        }
                        catch
                        {
                            btn_updataopencom.Text = "打开串口";
                        }

                    }
                }
                else if (g_n32CodesType == 2)
                {
                    if (serialPort1.IsOpen)
                    {
                        closing = true;                //正在关闭串口
                        while (Listening)
                            Application.DoEvents();
                        serialPort1.Close();
                        closing = false;              //关闭串口完成
                        btn_openSerial_app.Text = "打开串口";
                        progressBar1.Value = 0;
                        label_Communication.Text = "串口已关闭";
                        if (g_timer1 != null)
                        {
                            g_timer1.Stop();
                        }
                        if (g_timer2 != null)
                        {
                            g_timer2.Stop();
                        }
                        btn_updataopencom.Text = "打开串口";
                    }
                    else
                    {
                        try
                        {
                            serialPort1.PortName = comb_updatacomport.Text;
                            serialPort1.BaudRate = int.Parse(comb_updatacombaud.Text);
                            serialPort1.Open();
                            btn_updataopencom.Text = "关闭串口";
                            serialPort1.DataReceived += comm_DataReceived;   //串口接收
                            btn_checkport.Enabled = true;
                        }
                        catch
                        {
                            btn_updataopencom.Text = "打开串口";
                        }
                    }
                }
            }
            catch
            { }
        }
        #endregion

        #region 程序类型选择
        private void cmb_selectcodetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_selectcodetype.Text == "ARM程序")
            {
                g_n32CodesType = 1;
                txt_filepath.Text = "./data.bin";
                btn_stop.Enabled = true;
                btn_checkport.Enabled = true;
                btn_file.Enabled = false;
                btn_sta.Enabled = false;
                btn_updataopencom.Enabled = true;
                btn_updatasearchcom.Enabled = true;
                comb_updatacombaud.Enabled = true;
                comb_updatacomport.Enabled = true;
            }
            else if (cmb_selectcodetype.Text == "FPGA程序")
            {
                g_n32CodesType = 2;
                txt_filepath.Text = "./data.rbf";
                g_bfpgasendover = false;
                btn_checkport.Enabled = false;
                btn_stop.Enabled = false;
                btn_file.Enabled = true;
                btn_sta.Enabled = true;
                btn_updataopencom.Enabled = false;
                btn_updatasearchcom.Enabled = false;
                comb_updatacombaud.Enabled = false;
                comb_updatacomport.Enabled = false;
            }

        }
        #endregion

        #region 查询端口
        private void btn_checkport_Click(object sender, EventArgs e)
        {
            try
            {
                string[] l_asportname = SerialPort.GetPortNames();    //检查是否含有串口 
                if (l_asportname.Length < 1)
                {
                    MessageBox_Invoke("本机没有串口！");
                    return;
                }

                else if(!serialPort2.IsOpen)
                {
                    MessageBox.Show("串口未打开，请打开串口", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                com2sendthread = new Thread(com2sendfun);             //开启发送消息线程  
                com2sendthread.Start();

                g_n32ProgramFlag = 1;
                g_n32ProgramMode = 0;
                rich_prodown.Text = "";
                rich_prodown.AppendText("\r\n请重启设备！！！\r\n");
                rich_prodown.AppendText("\r\n请重启设备！！！\r\n");
                //rich_prodown.AppendText("\r\n请重启设备！！！\r\n");
                rich_prodown.AppendText("\r\n正在搜寻设备");
                btn_checkport.Enabled = false;
                btn_stop.Enabled = true;


            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString (), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static IPAddress GetSrcIP()
        {
            string AddressIP = "";
            IPAddress IP = null;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                    IP = _IPAddress;  //设定全局的IP
                }
            }
            return IP;
        }
        #endregion

        #region COM2接收函数
        private void com2recfun(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                g_bcom2listening = true;

                {
                    int l_n32rcvdlen = serialPort2.BytesToRead;           //先记录下来，避免某种原因，认为的原因，操作几次之间时间长短，缓存不一致  
                    byte[] l_brcvdbuf = new byte[l_n32rcvdlen];           //声明一个临时数组存储当前的串口数据  
                    serialPort2.Read(l_brcvdbuf, 0, l_n32rcvdlen);        //读取缓冲数据

                    if ((l_n32rcvdlen == 1) && (l_brcvdbuf[0] == 0x55) && g_n32ProgramMode == 1)  //收到检测端口的回复
                    {
                        g_n32ProgramMode = 2;     //收到回复，建立连接
                        RichTextBox_Invoke(rich_prodown, "\n\r检测到 1 个设备!\r\n");
                        RichTextBox_Invoke(rich_prodown, "连接中......\r\n");
                        RichTextBox_Invoke(rich_prodown, "连接成功!\r\n");
                        MethodInvoker mi = new MethodInvoker(Enablebtn);
                        this.BeginInvoke(mi);
                    }
                    else if ((l_n32rcvdlen == 1) && (l_brcvdbuf[0] == 0x55) && (g_n32ProgramMode == 3) && (r_flag == 0))
                    {
                        r_flag = 1;
                        RichTextBox_Invoke(rich_prodown, "> ");
                        g_n32rcvdno++;
                    }
                    else if ((l_n32rcvdlen == 1) && (l_brcvdbuf[0] == 0xdd))
                    {
                        RichTextBox_Invoke(rich_prodown, "\n通信错误，烧写失败！\n");
                        //一轮烧写完成，复位参数
                        g_n32ProgramMode = 0;
                    }
                    if ((l_n32rcvdlen == 1) && (l_brcvdbuf[0] == 0x78) && (g_n32ProgramMode == 3))
                    {
                        RichTextBox_Invoke(rich_prodown, "\n烧写成功！\n");
                        g_n32ProgramMode = 0;
                        MethodInvoker mi = new MethodInvoker(DisableBtn);
                        this.BeginInvoke(mi);
                        g_n32ProgramFlag = 0;
                        RichTextBox_Invoke(rich_prodown, "\r\n已停止!\r\n");
                        Array.Clear(g_budpsendbuf, 0, 1024);
                        //关闭串口
                        //serialPort2.Close();
                        MethodInvoker mj = new MethodInvoker(closecom2);
                        this.BeginInvoke(mj);
                        g_n32rcvdno=0;
                        g_n32sendno = 0;
                        if (com2sendthread != null)
                        {
                            com2sendthread.Abort();
                        }
                    }
                }
            }
            catch
            {
                return;
                //MessageBox.Show(ex.ToString(), "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                g_bcom2listening = false;
            }
        }

        private void Enablebtn()
        {
            btn_sta.Enabled = true;
            btn_file.Enabled = true;
        }
        private void DisableBtn()
        {
            btn_checkport.Enabled = true;
            btn_stop.Enabled = false;
            btn_file.Enabled = false;
            btn_sta.Enabled = false;
            btn_updataopencom.Text = "打开串口";
        }
        #endregion

        #region com2发送函数

        private void opencom2()
        {
            try
            {
                if (serialPort2 != null)
                {
                    if (!serialPort2.IsOpen)
                    {
                        serialPort2.PortName = comb_updatacomport.Text;
                        serialPort2.BaudRate = int.Parse(comb_updatacombaud.Text);
                        serialPort2.Open();
                    }
                }
            }
            catch
            { }
        }
        private void closecom2()
        {
            try
            {
                serialPort2.Close();
               btn_updataopencom.Text = "打开串口";
            }
            catch
            {
 
            }
        }

        int g_n32sendno = 0;
        int g_n32rcvdno = 0;
        double l_n64filelen = 0;
        int l_n32fileindex = 0;
        private void com2sendfun()
        {
            while (true)
            {
                try
                {
                     l_n64filelen = 0;
                    int l_n32sendflag = 0;
                    int l_n32senddatasum = 0;   //发送数据的和
                    int l_n32sendover = 0;		//发送完成标志
                     l_n32fileindex = 0;

                    if (!serialPort2.IsOpen && g_n32ProgramFlag == 1)
                    {
                        MethodInvoker mi = new MethodInvoker(opencom2);
                        this.BeginInvoke(mi);
                    }

                    while (g_n32ProgramFlag == 1 && serialPort2.IsOpen)
                    {
                        if (g_n32ProgramMode == 0)								//空闲状态
                        {
                            l_n32fileindex = 0;
                            l_n32senddatasum = 0;
                            Array.Clear(g_budpsendbuf, 0, 1024);
                            l_n64filelen = 0;
                            g_n32sendlen = 0;
                            r_flag = 0;
                            l_n32sendover = 0;
                            g_n32ProgramMode = 1;
                        }
                        else if (g_n32ProgramMode == 1)				 //主动建立连接状态
                        {
                            g_n32sendlen = 0;
                            g_budpsendbuf[0] = 0x30;			     //帧头
                            g_budpsendbuf[1] = 0x24;
                            g_budpsendbuf[2] = Convert.ToByte((g_n32sendlen + 9) >> 8);   //帧长
                            g_budpsendbuf[3] = Convert.ToByte((g_n32sendlen + 9) & 0xFF);
                            g_budpsendbuf[4] = 0xCD;
                            g_budpsendbuf[5] = 0x45;		         //命令号
                            g_budpsendbuf[6] = 0;	                 //序号
                            g_budpsendbuf[7] = 0;
                            l_n32senddatasum = 0;
                            for (int i = 0; i < 8 + g_n32sendlen; i++)
                            {
                                l_n32senddatasum = (l_n32senddatasum + g_budpsendbuf[i]) & 0xFF;
                            }
                            g_budpsendbuf[8 + g_n32sendlen] = Convert.ToByte(l_n32senddatasum);

                            g_n32sendlen += 9;                            //发送数据的长度
                            byte[] l_bsendbuf = new byte[g_n32sendlen];
                            Array.Copy(g_budpsendbuf, 0, l_bsendbuf, 0, g_n32sendlen);
                            //socketudp.SendTo(l_bsendbuf, l_dstpoint);
                            serialPort2.Write(l_bsendbuf, 0, l_bsendbuf.Length);
                            RichTextBox_Invoke(rich_prodown, " - ");
                            Thread.Sleep(500);

                        }
                        else if ((g_n32ProgramMode == 3) && (r_flag == 1) && (l_n32sendover == 0))  //开始发送数据状态
                        {
                            r_flag = 0;
                            if (l_n32fileindex == 0)
                            {
                                FileInfo fi = new FileInfo(g_sbinpath);
                                if (fi.Length == 0)
                                {
                                    MessageBox.Show("文件中无数据！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                fs = File.OpenRead(g_sbinpath);
                                l_n64filelen = fi.Length;
                            }
                            if ((l_n64filelen - l_n32fileindex * SEND_MAX) >= SEND_MAX)
                            {
                                g_n32sendlen = SEND_MAX;
                                fs.Seek(l_n32fileindex * SEND_MAX, SeekOrigin.Begin);
                                fs.Read(g_budpsendbuf, 8, SEND_MAX);
                                l_n32sendflag = 0x02;
                            }
                            else if ((l_n64filelen - l_n32fileindex * SEND_MAX) > 0)
                            {
                                g_n32sendlen = (int)(l_n64filelen - l_n32fileindex * SEND_MAX);
                                fs.Seek(l_n32fileindex * SEND_MAX, SeekOrigin.Begin);
                                fs.Read(g_budpsendbuf, 8, SEND_MAX);
                                l_n32sendflag = 0x02;
                            }
                            else
                            {
                                g_n32sendlen = 0;
                                l_n32sendflag = 0x03;
                                l_n32sendover = 1;
                                fs.Close();
                            }
                            g_budpsendbuf[0] = 0x30;			//帧头
                            g_budpsendbuf[1] = 0x24;
                            g_budpsendbuf[2] = Convert.ToByte((g_n32sendlen + 9) >> 8); //帧长
                            g_budpsendbuf[3] = Convert.ToByte((g_n32sendlen + 9) & 0xFF);
                            g_budpsendbuf[4] = 0x00;
                            g_budpsendbuf[5] = Convert.ToByte(l_n32sendflag);		//命令号
                            g_budpsendbuf[6] = Convert.ToByte(l_n32fileindex >> 8);	//序号
                            g_budpsendbuf[7] = Convert.ToByte(l_n32fileindex & 0xFF);
                            l_n32senddatasum = 0;
                            for (int i = 0; i < 8 + g_n32sendlen; i++)
                            {
                                l_n32senddatasum = (l_n32senddatasum + g_budpsendbuf[i]) & 0xFF;
                            }
                            g_budpsendbuf[8 + g_n32sendlen] = Convert.ToByte(l_n32senddatasum);

                            g_n32sendlen += 9;

                            byte[] l_bsendbuf = new byte[g_n32sendlen];
                            Array.Copy(g_budpsendbuf, 0, l_bsendbuf, 0, g_n32sendlen);
                            serialPort2.Write(l_bsendbuf, 0, l_bsendbuf.Length);
                            l_n32fileindex++;
                            g_n32sendno++;
                        }

                        Thread.Sleep(100);
                    }
                }
                catch
                {

                }
            }
        }
        #endregion

        #region 停止
        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (g_n32CodesType == 1)
            {
                if (serialPort2.IsOpen)
                {
                    //serialPort2.Close();
                    //btn_openSerial_app.Text = "打开串口";

                    MethodInvoker mi = new MethodInvoker(closecom2);
                    this.BeginInvoke(mi);
                }
                g_n32ProgramFlag = 0;
                rtbshow(rich_prodown, "\r\nARM程序更新已停止!\r\n");
                btn_checkport.Enabled = true;
                btn_stop.Enabled = false;
                btn_file.Enabled = false;
                btn_sta.Enabled = false;

            }
            else if (g_n32CodesType == 2)
            {
            }
        }
        #endregion

        #region 打开文件
        private void btn_file_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.FileName = "";
                if (g_n32CodesType == 1)
                {
                    openFileDialog1.Filter = "bin files(*.bin)|*.bin|All files(*.*)|*.*||";
                }
                else if (g_n32CodesType == 2)
                {
                    openFileDialog1.Filter = "bin files(*.rpd,*.rbf)|*.rpd;*.rbf|All files(*.*)|*.*||";
                }
                openFileDialog1.ShowDialog();
                if (openFileDialog1.FileName != "")
                {
                    g_sbinpath = openFileDialog1.FileName;
                    txt_filepath.Text  = g_sbinpath;
                }
            }
            catch
            {
                MessageBox.Show("文件打开失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion

        #region 开始下载
        private void btn_sta_Click(object sender, EventArgs e)
        {
            if (g_n32CodesType == 1)
            {
                rich_prodown.AppendText("开始下载\r\n");
                btn_sta.Enabled = false;
                g_n32ProgramMode = 3; //开始刷写
                r_flag = 1;
            }
            else if (g_n32CodesType == 2)
            {
                FPGAThread = new Thread(FPGADataSendFunc);
                this.FPGAThread.IsBackground = true;
                this.FPGAThread.Start();
                btn_sta.Enabled = false;
            }
        }

        #region FPGA 下载程序函数
        private void FPGADataSendFunc()
        {
            try
            {
                g_bfpgasendover = false;
                g_bfpgaupdatesuccess = true;
                FPGASendResetEvent.Reset();

                if (serialPort1.IsOpen == false)
                {
                    MessageBox_Invoke("串口未打开！！！");
                    return;
                }
                RichTextBox_Invoke(rich_prodown, "开始下载\r\n");
                g_n32HeartState = 0;                 //关闭心跳否则下载过程会出现断网的情况    
                FileInfo fi = new FileInfo(g_sbinpath);
                if (fi.Length == 0)
                {
                    MessageBox_Invoke("文件中无数据！");
                    return;
                }

                fs = File.OpenRead(g_sbinpath);
                double l_n64filelen = fi.Length;

                if (l_n64filelen % ARM_SEND_MAX == 0)
                {
                    g_n32FPGATotalPkgs = (int)(l_n64filelen / ARM_SEND_MAX);
                }
                else
                {
                    g_n32FPGATotalPkgs = (int)(l_n64filelen / ARM_SEND_MAX) + 1;
                }
                RichTextBox_Invoke(rich_prodown, "文件中总共读取到" + g_n32FPGATotalPkgs.ToString() + "包数据" + "\r\n");
                int l_n32PkgsSerialno = 0;                               //包序号
                for (int i = 0; i < g_n32FPGATotalPkgs; i++)             //循环读文件
                {
                    if ((l_n64filelen - i * ARM_SEND_MAX) > 0)
                    {
                        fs.Seek(i * ARM_SEND_MAX, SeekOrigin.Begin);
                        fs.Read(g_abfpgacode, 30, ARM_SEND_MAX);
                    }
                    else
                    {
                        fs.Close();
                    }
                    g_abfpgacode[0] = 0xff;
                    g_abfpgacode[1] = 0xaa;
                    g_abfpgacode[2] = 0x01;
                    g_abfpgacode[3] = 0x1e;

                    g_abfpgacode[4] = 0x00;
                    g_abfpgacode[5] = 0x00;

                    g_abfpgacode[6] = 0x00;
                    g_abfpgacode[7] = 0x00;
                    g_abfpgacode[8] = 0x00;
                    g_abfpgacode[9] = 0x00;

                    g_abfpgacode[10] = 0x01;
                    g_abfpgacode[11] = 0x01;

                    g_abfpgacode[12] = 0x00;
                    g_abfpgacode[13] = 0x05;

                    g_abfpgacode[22] = 0x07;
                    g_abfpgacode[23] = 0x07;

                    g_abfpgacode[26] = Convert.ToByte(g_n32FPGATotalPkgs >> 8 & 0xff);
                    g_abfpgacode[27] = Convert.ToByte(g_n32FPGATotalPkgs & 0xff);

                    g_abfpgacode[28] = Convert.ToByte(i >> 8 & 0xff);
                    g_abfpgacode[29] = Convert.ToByte(i & 0xff);

                    g_abfpgacode[287] = ToolFunc.XorCheck_byte(2, g_abfpgacode, 4);

                    g_abfpgacode[288] = 0xEE;
                    g_abfpgacode[289] = 0xEE;

                    if (serialPort1.IsOpen)
                    {
                        serialPort1.Write(g_abfpgacode, 0, g_abfpgacode.Length);
                    }
                    //添加等待发送成功事件
                    if (FPGASendResetEvent.WaitOne(5000))
                    {
                        l_n32PkgsSerialno++;
                        g_n32FPGARetryNo = 0;
                    }
                    else
                    {
                        RichTextBox_Invoke(rich_prodown, "超时未收到回复，第" + l_n32PkgsSerialno.ToString() + "包数据烧写失败，正在重新烧写！\r\n");
                        i--;
                        g_n32FPGARetryNo++;
                        if (g_n32FPGARetryNo > 20)
                        {
                            MessageBox_Invoke("FPGA程序更新失败，请重新烧写！");
                            g_bfpgasendover = false;
                            btn_sta.Enabled = true;
                            return;
                        }
                    }
                    Delayms(1);
                }
                g_bfpgasendover = true;

                MethodInvoker mi = new MethodInvoker(Enablebtn_sta);
                this.BeginInvoke(mi);
            }
            catch (Exception ex)
            {
                RichTextBox_Invoke(rich_prodown, ex.ToString());
            }
        }

        #endregion

        private void Enablebtn_sta()
        {
            btn_sta.Enabled = true;
        }

        #endregion

        private void rich_prodown_TextChanged(object sender, EventArgs e)
        {
            if (rich_prodown.Text.Length % 5 == 0)
            {
                rich_prodown.SelectionStart = rich_prodown.Text.Length; //Set the current caret position at the end
                rich_prodown.ScrollToCaret();                           //Now scroll it automatically

            }
            //string str = rich_prodown.Text;
            //string[] richArrayString = str.Split(' ');
            //int len=richArrayString.Length-1;
            //if (richArrayString[len] == "\r\n")
            //{ }
        }
        #endregion

        #region 自定义委托
        
        /// <summary>
        /// 异步调用messagebox
        /// </summary>
        /// <param name="message"></param>
        private delegate void MessageBoxInvoke(string message);
        private void MessageBox_Invoke(string message)
        {
            MessageBoxInvoke mi = new MessageBoxInvoke(showMessageBox);
            this.BeginInvoke(mi, message);
        }
        private void showMessageBox(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        
        /// <summary>
        /// Groupbox内容更新
        /// </summary>
        /// <param name="str"></param>
        /// <param name="msg"></param>
        /// <param name="nameno"></param>
        public delegate void ShowGroupBoxTextInvoke(GroupBox GBName, string msg, int nameno);
        public void GroupBoxItemUpdata_Invoke(GroupBox GBName, string msg, int txt_name_no)
        {
            if (GBName.Controls[txt_name_no].InvokeRequired)
            {
                ShowGroupBoxTextInvoke _myinvoke = new ShowGroupBoxTextInvoke(GroupBoxItemUpdata_Invoke);
                GBName.Controls[txt_name_no].Invoke(_myinvoke, new object[] { GBName, msg, txt_name_no });
            }
            else
            {
                GBName.Controls[txt_name_no].Text = msg;//.AppendText(msg);

            }
        }

        /// <summary>
        /// richTextBox 内容更新
        /// </summary>
        /// <param name="msg"></param>
        public delegate void RichTextBoxInvoke(RichTextBox rtb, string msg);
        private void RichTextBox_Invoke(RichTextBox rtb, string msg)
        {
            RichTextBoxInvoke srti = new RichTextBoxInvoke(rtbshow);
            this.Invoke(srti, new object[] { rtb, msg });
        }
        private void rtbshow(RichTextBox rtb, string str)
        {
            rtb.AppendText(str);
        }


        /// <summary>
        /// panel 控件内容更新
        /// </summary>
        /// <param name="str"></param>
        /// <param name="msg"></param>
        /// <param name="nameno"></param>

        public delegate void ShowPanelTextInvoke(Panel str, string msg, int nameno);
        public void PanelItemUpdata_Invoke(Panel str, string msg, int txt_name_no)
        {
            if (str.Controls[txt_name_no].InvokeRequired)
            {
                ShowPanelTextInvoke _myinvoke = new ShowPanelTextInvoke(PanelItemUpdata_Invoke);
                str.Controls[txt_name_no].Invoke(_myinvoke, new object[] { str, msg, txt_name_no });
            }
            else
            {
                str.Controls[txt_name_no].Text = msg;//.AppendText(msg);

            }
        }


        /// <summary>
        /// 灰尘检测
        /// </summary>
        /// <param name="dustbuffer"></param>
        private delegate void DustRInvoke(byte[] dustbuffer);


        /// <summary>
        /// Teechart4委托绘图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private delegate void Tchart4Invoke(Steema.TeeChart.Styles.FastLine linex, int[] x, int[] y);
        private void Tchart4DrawLine(Steema.TeeChart.Styles.FastLine linex, int[] x, int[] y)
        {
            Tchart4Invoke ti = new Tchart4Invoke(Tchart4_Draw);
            this.BeginInvoke(ti, new object[] { linex, x, y });
        }
        private void Tchart4_Draw(Steema.TeeChart.Styles.FastLine linex, int[] x, int[] y)
        {
            linex.Add(x, y);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            ToolFunc.GBIndexQuery(groupBox5);
            int groupcount = panel4.Controls.Count;
            string[] strnames = new string[groupcount];
            for (int i = 0; i < groupcount; i++)
            {
                strnames[i] = panel4.Controls[i].Name;
            }
        }

        
        #region 串口

        private bool Listening = false;                                        //是否没有执行完invoke相关操作  不添加监视会出现串口关闭时卡死程序
        private bool closing = false;                                          //正在关闭串口标志位
        private bool g_bmanualopencom1 = false;                                //执行手动打开串口1操作
       
        #region 打开串口
        private void btn_openSerial_app_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                DialogResult dr = MessageBox.Show("程序更新中，是否关闭程序更新串口", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    serialPort2.Close() ;
                }
                else
                {
                    return;
                }
            }

            #region 根据按钮名称判断

            if (btn_openSerial_app.Text == "打开串口")
            {
                try
                {
                    if (serialPort1.IsOpen == true)
                    {
                        closing = true;                //正在关闭串口
                        while (Listening)
                            Application.DoEvents();
                        serialPort1.Close();
                        closing = false;              //关闭串口完成
                    }
                    serialPort1.PortName = comb_serialport_app.Text;
                    serialPort1.BaudRate = int.Parse(comb_serialbaudapp.Text);
                    serialPort1.Open();
                    btn_openSerial_app.Text = "关闭串口";
                    serialPort1.DataReceived += comm_DataReceived;
                    progressBar1.Value = 99;
                    label_Communication.Text = "串口已打开";

                    if (g_timer1 != null)
                    {
                        g_timer1.Stop();
                        g_timer1.Dispose();
                    }
                    if (g_timer2 != null)
                    {
                        g_timer2.Stop();
                        g_timer2.Dispose();
                    }

                    g_timer1 = new System.Windows.Forms.Timer();      //
                    g_timer1.Interval = g_n32TimerInterval;
                    g_timer1.Tick += new EventHandler(timer1_Tick);
                    g_timer1.Start();                                //启动定时器1  查询参数

                    g_timer2 = new System.Windows.Forms.Timer();
                    g_timer2.Interval = g_n32TimerInterval;
                    g_timer2.Tick += new EventHandler(timer2_Tick);
                    g_timer2.Start();
                    g_bmanualopencom1 = true;                        //手动打开串口标志
                }
                catch
                {
                    btn_openSerial_app.Text = "打开串口";
                    progressBar1.Value = 0;
                    label_Communication.Text = "串口已关闭";
                    if (g_timer1 != null)
                    {
                        g_timer1.Stop();
                        g_timer1.Dispose();
                    }
                    if (g_timer2 != null)
                    {
                        g_timer2.Stop();
                        g_timer2.Dispose();
                    }
                    g_bmanualopencom1 = false;                        //手动关闭串口标志
                }
            }
            else
            {
                if (serialPort1.IsOpen == true)
                {
                    closing = true;                //正在关闭串口
                    while (Listening)
                        Application.DoEvents();
                    serialPort1.Close();
                    closing = false;              //关闭串口完成
                }
                if (g_timer1 != null)
                {
                    g_timer1.Stop();
                    g_timer1.Dispose();
                }
                if (g_timer2 != null)
                {
                    g_timer2.Stop();
                    g_timer2.Dispose();
                }
                btn_openSerial_app.Text = "打开串口";
                progressBar1.Value = 0;
                label_Communication.Text = "串口已关闭";
                g_bmanualopencom1 = false;                        //手动关闭串口标志
            }
            #endregion

            #region 根据串口状态判断

            //if (serialPort1.IsOpen == true)
            //{
            //    serialPort1.Close();
            //    btn_openSerial_app.Text = "打开串口";
            //    progressBar1.Value = 0;
            //    label_Communication.Text = "串口已关闭";
            //    g_timer1.Stop();
            //    g_timer2.Stop();
            //    g_timer1.Dispose();
            //    g_timer2.Dispose();
            //    g_bmanualopencom1 = false;                        //手动关闭串口标志
            //}
            //else
            //{
            //    try
            //    {
            //        serialPort1.PortName = comb_serialport_app.Text;
            //        serialPort1.BaudRate = int.Parse(comb_serialbaudapp.Text);
            //        serialPort1.Open();
            //        btn_openSerial_app.Text = "关闭串口";
            //        serialPort1.DataReceived += comm_DataReceived;
            //        progressBar1.Value = 99;
            //        label_Communication.Text = "串口已打开";

            //        if (g_timer1 != null)
            //        {
            //            g_timer1.Stop();
            //            g_timer1.Dispose();
            //        }
            //        if (g_timer2 != null)
            //        {
            //            g_timer2.Stop();
            //            g_timer2.Dispose();
            //        }

            //        //comb_serialport_app.Enabled = false;
            //        //comb_serialbaudapp.Enabled = false;
            //        g_timer1 = new System.Windows.Forms.Timer();      //
            //        g_timer1.Interval = g_n32TimerInterval;
            //        g_timer1.Tick += new EventHandler(timer1_Tick);
            //        g_timer1.Start();                                //启动定时器1  查询参数

            //        g_timer2 = new System.Windows.Forms.Timer();
            //        g_timer2.Interval = g_n32TimerInterval ;
            //        g_timer2.Tick += new EventHandler(timer2_Tick);
            //        g_timer2.Start();

            //        g_bmanualopencom1 = true;                        //手动打开串口标志
            //    }
            //    catch
            //    {
            //        btn_openSerial_app.Text = "打开串口";
            //        progressBar1.Value = 0;
            //        label_Communication.Text = "串口已关闭";
            //        if (g_timer1 != null)
            //        {
            //            g_timer1.Stop();
            //            g_timer1.Dispose();
            //        }
            //        if (g_timer2 != null)
            //        {
            //            g_timer2.Stop();
            //            g_timer2.Dispose();
            //        }
            //    }

            //}

            #endregion
        }
        #endregion 打开串口

        #region 串口接收
        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (closing)
            {
                return;                                          //如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            }
            try
            {
                Listening = true;                                  //设置标记，说明我已经开始处理数据，一会儿要使用系统的UI  
                int nRecvSize = serialPort1.BytesToRead;           //先记录下来，避免某种原因，认为的原因，操作几次之间时间长短，缓存不一致  
                byte[] acRecvDataBuf = new byte[nRecvSize];        //声明一个临时数组存储当前的串口数据  
                serialPort1.Read(acRecvDataBuf, 0, nRecvSize);     //读取缓冲数据

                Array.Copy(acRecvDataBuf, 0, g_byteArrayNetRcvdDat, g_sNetBuf.m_u32in, nRecvSize);
                g_sNetBuf.m_u32in += nRecvSize;
                if (nRecvSize < 10)  //接收字节个数少于10个继续接收
                {
                    return;
                }
                if (g_sNetBuf.m_u32in >= g_n32MAX_BLOCK * 10)
                {
                    Array.Clear(g_byteArrayNetRcvdDat, 0, g_byteArrayNetRcvdDat.Length);
                    g_sNetBuf.m_u32in = 0;
                    g_sNetBuf.m_u32out = 0;
                    g_sNetBuf.m_u32size = 0;
                    return;  //
                }

                while (g_sNetBuf.m_u32out < g_sNetBuf.m_u32in)
                {
                    if ((g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out] == 0xff &&
                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 1] == 0xff) ||
                        (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out] == 0x02 &&
                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 1] == 0x02 &&
                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 2] == 0x02 &&
                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 3] == 0x02) ||
                        (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out] == 0xff &&
                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 1] == 0xaa))
                    {
                        //计算包长度
                        int l_u32reallen = 0;
                        g_n32NetRcvdPkgs++;
                        if (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out] == 0x02)
                        {
                            l_u32reallen = ((int)(g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 6] << 8) +
                                            (int)(g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 7]));
                            l_u32reallen = l_u32reallen + 9;
                        }
                        else if (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out] == 0xFF &&       //单点数据    
                                 g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 1] == 0xFF)
                        {
                            l_u32reallen = ((int)(g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 5] << 8) +
                                            (int)(g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 4]));
                        }
                        else if (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out] == 0xFF &&       //具体参数
                            g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 1] == 0xAA)
                        {
                            l_u32reallen = ((int)(g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 2] << 8) +
                                            (int)(g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + 3]));
                            l_u32reallen += 4;                                             //实际长度应加上帧头帧尾4个字节
                        }
                        else
                            g_sNetBuf.m_u32out++;//既然已经进入了第一个if这条语句不是应该永远不执行吗？

                        ////判断包长度与实际缓存长度之间的关系
                        if (l_u32reallen < 10)
                        {
                            break;             //数据中不存在长度小于10的指令，小于10跳出循环解析，继续接收
                        }
                        if (l_u32reallen <= (g_sNetBuf.m_u32in - g_sNetBuf.m_u32out + 1))
                        {
                            //数据分帧处理
                            byte[] buffer = new byte[l_u32reallen];
                            Array.Copy(g_byteArrayNetRcvdDat, g_sNetBuf.m_u32out, buffer, 0, l_u32reallen);//m_acbuf.CopyTo(g_sNetBuf.m_u32out, buffer, 0, l_u32reallen);
                            g_n32XorFlg = CheckNetReceivedData(buffer);
                            //1.校验处理
                            if (g_n32XorFlg == 1)
                            {
                                int i;
                                for (i = 1; i < l_u32reallen; i++)
                                {
                                    if ((g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i] == 0xff &&
                                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i + 1] == 0xff) ||
                                        (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i] == 0x02 &&
                                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i + 1] == 0x02 &&
                                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i + 2] == 0x02 &&
                                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i + 3] == 0x02) ||
                                        (g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i] == 0xff &&
                                         g_byteArrayNetRcvdDat[g_sNetBuf.m_u32out + i + 1] == 0xaa))
                                    {
                                        g_sNetBuf.m_u32out += i;
                                        //RichTextBox_Invoke(richTextBox1, "校验错误\r\n");
                                        g_n32XorFlg = 0;
                                        break;
                                    }

                                }
                                if (i == l_u32reallen && g_n32XorFlg == 1)
                                {
                                    g_sNetBuf.m_u32out += l_u32reallen;
                                }
                                g_n32XorFlg = 0;
                            }
                            else
                            {
                                g_sNetBuf.m_u32out += l_u32reallen;
                            }
                        }

                        else if (l_u32reallen >= g_n32MAX_BLOCK)
                        {
                            Array.Clear(g_byteArrayNetRcvdDat, 0, g_byteArrayNetRcvdDat.Length);//m_acbuf.Clear();
                            g_sNetBuf.m_u32in = 0;
                            g_sNetBuf.m_u32out = 0;
                            g_sNetBuf.m_u32size = 0;
                            continue;
                        }
                        else
                        {
                            break;    //实际长度大于缓存数跳出解析继续接收
                        }

                    }
                    else
                    {
                        g_sNetBuf.m_u32out++;  //找包头
                    }

                }

                if ((g_sNetBuf.m_u32in + nRecvSize) >= g_n32MAX_BLOCK * 10)
                {
                    //大于最大缓存，原来的数全清0
                    Array.Clear(g_byteArrayNetRcvdDat, 0, g_byteArrayNetRcvdDat.Length);//m_acbuf.Clear();
                    g_sNetBuf.m_u32in = 0;
                    g_sNetBuf.m_u32out = 0;
                    g_sNetBuf.m_u32size = 0;
                    return;
                }
                if (g_sNetBuf.m_u32out >= g_sNetBuf.m_u32in)//此时证明整个缓存的数据都已经解析完成，清除缓存
                {
                    //此时证明整个缓存的数据都已经解析完成，清除缓存");
                    Array.Clear(g_byteArrayNetRcvdDat, 0, g_byteArrayNetRcvdDat.Length);
                    g_sNetBuf.m_u32in = 0;
                    g_sNetBuf.m_u32out = 0;
                    g_sNetBuf.m_u32size = 0;
                    //richtext_show( "out>in\r\n");
                }
            }
            catch (Exception ex)
            {
                //MessageBox_Invoke("数据接收错误！！");
                //RichTextBox_Invoke(richTextBox1, "网络数据接收错误！！！");
                // return;
            }
            finally
            {
                Listening = false;            //我用完了，UI可以关闭串口了  
            }
        }

        #endregion 串口接收

        #region 数据校验处理
        private int CheckNetReceivedData(byte[] sourcebuffer)
        {
            try
            {
                int nDataBufSize = sourcebuffer.Length;
                int l_n32xorflg = 0;
                if (nDataBufSize > 0)
                {
                    if (ToolFunc.checkXor(sourcebuffer))
                    {
                        l_n32xorflg = 0;
                        for (int i = 0; i < nDataBufSize; i++)
                        {
                            g_byteArrayNetRcvBuf[g_n32NetRcvInd, i] = sourcebuffer[i];
                        }
                        g_n32ArrayNetRcvSize[g_n32NetRcvInd] = nDataBufSize;
                        g_n32NetRcvInd = (g_n32NetRcvInd + 1) % g_n32NetBufRowNum;
                        AnalysisNetReceivedData(g_byteArrayNetRcvBuf);   //校验成功后进行数据处理
                    }
                    else
                    {
                        l_n32xorflg = 1;
                    }
                }
                return l_n32xorflg;

            }
            catch (Exception ex)
            {
                return 1;
            }

        }


        #endregion

        #region 对接收数据进行解析处理
        //public List<int> g_ln32armpkgs = new List<int>();      //
        private void AnalysisNetReceivedData(byte[,] p_cNetRcvdBuf)
        {
            try
            {
                int nBufID = g_n32NetRcvInd - 1;
                if (nBufID == -1)
                    nBufID = 99;
                if (p_cNetRcvdBuf[nBufID, 0] == 0x02 && p_cNetRcvdBuf[nBufID, 1] == 0x02 &&
                    p_cNetRcvdBuf[nBufID, 2] == 0x02 && p_cNetRcvdBuf[nBufID, 3] == 0x02)
                {
                    //g_n32ScanDatRcvdFrames++;
                    //JointScanDataPackageFunc(nBufID, p_cNetRcvdBuf);                //解析扫描数据和反射率

                }
                else if (p_cNetRcvdBuf[nBufID, 0] == 0xff && p_cNetRcvdBuf[nBufID, 1] == 0xff)
                {
                    AnalysisSingleDataInvoke(nBufID, p_cNetRcvdBuf);                //解析单点数据
                }
                else if (p_cNetRcvdBuf[nBufID, 0] == 0xff && p_cNetRcvdBuf[nBufID, 1] == 0xaa)
                {
                    AnalysisSpecificCommand(nBufID, p_cNetRcvdBuf);                 //解析具体命令
                }
                //计算完毕
                if (g_n32RcvdCalcPkgsOfRisingEdge >= g_n32Si2DPkgsToCalc && g_n32RcvdCalcPkgsOfFallingEdge >= g_n32Si2DPkgsToCalc)
                {
                    CompleteCalculateSingleSampleData();
                }
            }
            catch (Exception ex)
            {
               //return;
            }
        }

        #region 解析扫描数据
        int g_n32MotorSpeed = 0;
        private bool JointScanDataPackageFunc(int bufID, byte[,] p_cNetRcvdBuf)
        {
            g_n32ARMSendNo =       (int)(p_cNetRcvdBuf[bufID, 4] << 24) +
                                   (int)(p_cNetRcvdBuf[bufID, 5] << 16) +
                                   (int)(p_cNetRcvdBuf[bufID, 20] << 8) +
                                   (int)(p_cNetRcvdBuf[bufID, 21]);       //arm发送包数

            int l_n32IWDGRSTNum = (int)(p_cNetRcvdBuf[bufID, 14] << 8) +
                                  (int)(p_cNetRcvdBuf[bufID, 15]);       //看门狗复位次数
            int l_n32SOFTRSTNum = (int)(p_cNetRcvdBuf[bufID, 16] << 8) +
                                  (int)(p_cNetRcvdBuf[bufID, 17]);       //软件复位次数
            int l_n32PORRSTNum = (int)(p_cNetRcvdBuf[bufID, 18] << 8) +
                                  (int)(p_cNetRcvdBuf[bufID, 19]);       //掉电复位次数

            int l_n32ScanZeroDisc = ((int)(p_cNetRcvdBuf[bufID, 8] << 8) + (int)p_cNetRcvdBuf[bufID, 9]);     //APD温度
            g_n32maxzero = Math.Max(l_n32ScanZeroDisc, g_n32maxzero);
            g_n32minzero = Math.Min(l_n32ScanZeroDisc, g_n32minzero);
            g_n32zerodiff = g_n32maxzero - g_n32minzero;
            if (g_bsavezeroflag)  //保存零点数据
            {
                g_swZero.Write(l_n32ScanZeroDisc + "\t");
                g_n32savezerotimes++;
                if (g_n32savezerotimes % 80 == 0)
                {
                    g_swZero.Write("\r\n");
                }
            }
            GroupBoxItemUpdata_Invoke(groupBox5, l_n32ScanZeroDisc.ToString(), 5);
            for (int j = 26; j < 568; j++)          
            {
                g_byteLstXPkgsScanDat.Add(p_cNetRcvdBuf[bufID, j]);
            }

            Array.Clear(g_n64ArrayJiX, 0, g_n32DrawDatLen);
            Array.Clear(g_n64ArrayJiY, 0, g_n32DrawDatLen);
            Array.Clear(g_n64ArrayRegionZhiY, 0, g_n32DrawDatLen);
            Array.Clear(g_n64ArrayRegionZhiX, 0, g_n32DrawDatLen);
            int nDataZhiCount = 0;
            int nScanDataCount = 0;
            int nDataJi = 0;

            double l_64nDataRegionZhiX = 0;
            double l_64nDataRegionZhiY = 0;
            for (int m = 0; m < g_byteLstXPkgsScanDat.Count; m++)
            {
                nDataJi = ((int)(g_byteLstXPkgsScanDat[m++] << 8) + (int)(g_byteLstXPkgsScanDat[m]));
                if (nDataJi > g_n32MaxDis)         //最大测距值
                    nDataJi = g_n32MaxDis;
                g_n64ArrayJiX[nDataZhiCount] = nDataZhiCount;
                g_n64ArrayJiY[nDataZhiCount] = nDataJi;

                double l_n64angle = 270 - nDataZhiCount;

                //区域划分坐标
                double l_n64actualangle = l_n64angle - g_n64CenAngle;
                if (l_n64actualangle >= 360)
                {
                    l_n64actualangle -= 360;
                }
                if (l_n64actualangle < 0)
                {
                    l_n64actualangle += 360;
                }
                if (l_n64actualangle >= 0 && l_n64actualangle <= 90)
                {
                    l_64nDataRegionZhiY = nDataJi * Math.Sin(l_n64actualangle / 180 * Math.PI);
                    l_64nDataRegionZhiX = nDataJi * Math.Cos(l_n64actualangle / 180 * Math.PI);
                }
                else if (l_n64actualangle > 90 && l_n64actualangle <= 180)
                {
                    double l_n64angle2 = 180 - l_n64actualangle;
                    l_64nDataRegionZhiY = nDataJi * Math.Sin(l_n64angle2 / 180 * Math.PI);
                    l_64nDataRegionZhiX = -1 * nDataJi * Math.Cos(l_n64angle2 / 180 * Math.PI);
                }
                else if (l_n64actualangle > 180 && l_n64actualangle <= 270)
                {
                    if (l_n64actualangle > 225)
                    {
                        l_64nDataRegionZhiY = 0;
                        l_64nDataRegionZhiX = 0;
                    }
                    else
                    {
                        double l_n64angle3 = l_n64actualangle - 180;
                        l_64nDataRegionZhiY = -1 * nDataJi * Math.Sin(l_n64angle3 / 180 * Math.PI);
                        l_64nDataRegionZhiX = -1 * nDataJi * Math.Cos(l_n64angle3 / 180 * Math.PI);
                    }
                }
                else if (l_n64actualangle > 270 && l_n64actualangle < 360)
                {
                    double l_n64angle4 = 360 - l_n64actualangle;
                    l_64nDataRegionZhiY = -1 * nDataJi * Math.Sin(l_n64angle4 / 180 * Math.PI);
                    l_64nDataRegionZhiX = nDataJi * Math.Cos(l_n64angle4 / 180 * Math.PI);
                }
                g_n64ArrayRegionZhiX[nDataZhiCount] = l_64nDataRegionZhiX;
                g_n64ArrayRegionZhiY[nDataZhiCount] = l_64nDataRegionZhiY;
                nDataZhiCount++;
                g_n32ArrayScanPtDat[nScanDataCount++] = nDataJi;
            }
            g_byteLstXPkgsScanDat.Clear();
            g_n32ScanDatCorrectPkgs++;                    //上位机运行过程中总共接收的扫描数据的正确包数
            g_n32RcvdScanPkgsToCalcAvg++;                    //当此值=进行计算平均值的包数时计算平均值
            g_n32RcvdScanPkgsToDraw++;
            if (g_n32ScanDatCorrectPkgs == 1 && g_n32ARMSendNo != 1)
            {
                //g_n32ARMSendNo = g_n32ARMSendNo;
                g_n32ScanDatCorrectPkgs = 0;
                g_n32RcvdScanPkgsToDraw = 0;
            }
            MethodInvoker Calculate = new MethodInvoker(Calculate_MaxMin);
            this.BeginInvoke(Calculate);

            MethodInvoker Record = new MethodInvoker(RecordForm_relevant);
            this.BeginInvoke(Record);

            if (g_n32ScanDatCorrectPkgs % 100 == 0)
            {
                GroupBoxItemUpdata_Invoke(groupBox9, l_n32IWDGRSTNum.ToString(), 8);
                GroupBoxItemUpdata_Invoke(groupBox9, l_n32SOFTRSTNum.ToString(), 5);
                GroupBoxItemUpdata_Invoke(groupBox9, l_n32PORRSTNum.ToString(), 9);
            }

            if (g_n32RcvdScanPkgsToDraw >= g_n32ScanIntervalPkgsToDraw)
            {
                MethodInvoker Ji = new MethodInvoker(Draw_JI);
                this.BeginInvoke(Ji);
                MethodInvoker ZHi = new MethodInvoker(Draw_ZHI);
                this.BeginInvoke(ZHi);
                g_n32RcvdScanPkgsToDraw = 0;
                if (ScanSetForm.SaveScanDataToFile)
                {
                    WriteOrReadFiles.WriteDataToFileByStream(ScanSetForm.NetDataFilesSonPath, g_n64ArrayJiY);
                    g_n32SaveScanDatPkgs++;
                }
            }

            return true;
        }

        #endregion

        #region 解析具体命令
        int g_n32HeartState;            //心跳状态
        int g_n32banknotemp;            //下位机传输的bank号
        private void switchbank()
        {
            tscomb_bank.SelectedIndex = g_n32BankNo - 1;
        }
        private void AnalysisSpecificCommand(int nBufID, byte[,] p_cNetRcvdBuf)
        {
            switch (p_cNetRcvdBuf[nBufID, 22])              //根据主命令号区分指令
            {

                case 0x02:
                    {
                        #region 扫描数据
                        g_n32ScanDatRcvdFrames++;
                        JointScanDataPackageFunc(nBufID, p_cNetRcvdBuf);
                        #endregion
                    }
                    break;

                #region 灰尘检测
                case 0x04:    //灰尘检测
                    {
                        switch (p_cNetRcvdBuf[nBufID, 23])  //根据子命令号区分指令
                        {
                            case 0x01:
                                {
                                    #region 透过率初始化
                                    MessageBox_Invoke("透过率初始化成功");
                                    #endregion
                                }
                                break;

                            case 0x02:
                                {
                                    #region 灰尘检测查询回复指令

                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }
                                    //DustRInvoke dri = new DustRInvoke(OnDustR);
                                    //this.Invoke(dri, l_byteBufStateFrame);
                                    #endregion
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    break;
                #endregion

                case 0x05:                                  //
                    {
                        #region case5  设备运行状态 心跳 清除复位次数

                        switch (p_cNetRcvdBuf[nBufID, 23])  //根据子命令号区分指令
                        {

                            case 0x01:
                                {
                                    #region 设备运行状态
                                    g_bHeartStateFlg = true;
                                    g_n32RcvdLidarStateFrames++;
                                    int bufferlength = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] StateBuffer = new byte[bufferlength];
                                    for (int i = 0; i < bufferlength; i++)
                                    {
                                        StateBuffer[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (StateBuffer[bufferlength - 3] != ToolFunc.XorCheck_byte(2, StateBuffer, 4))
                                    {
                                        return;
                                    }
                                    int l_n32ArmVersionLen = Convert.ToInt32(StateBuffer[26]);//Arm版本号长度
                                    int l_n32FPGAVersionLen = Convert.ToInt32(StateBuffer[27]);//FPGA版本号长度
                                    string l_strArmProgramVer = "";     //Arm版本号 
                                    string l_strFPGAProgramVer = "";    //FPGA版本号
                                    List<byte> l_byteLstArm = new List<byte>();
                                    List<byte> l_byteLstFPGA = new List<byte>();
                                    for (int i = 28; i < 28 + l_n32ArmVersionLen; i++)
                                    {
                                        l_byteLstArm.Add(StateBuffer[i]);
                                    }
                                    l_strArmProgramVer = System.Text.Encoding.ASCII.GetString(l_byteLstArm.ToArray());
                                    PanelItemUpdata_Invoke(panel4, l_strArmProgramVer, 2);

                                    for (int i = 28 + l_n32ArmVersionLen; i < 28 + l_n32FPGAVersionLen + l_n32ArmVersionLen; i++)
                                    {
                                        l_byteLstFPGA.Add(StateBuffer[i]);
                                    }
                                    l_strFPGAProgramVer = System.Text.Encoding.ASCII.GetString(l_byteLstFPGA.ToArray());
                                    PanelItemUpdata_Invoke(panel4, l_strFPGAProgramVer, 4);
                                    int IRQNotemp = (int)(StateBuffer[75] << 8) + (int)(StateBuffer[74]);
                                    int IRQNo = (int)(StateBuffer[77] << 8) + (int)(StateBuffer[76]);          //中断次数
                                    int scanstate = (int)(StateBuffer[87] << 8) + (int)(StateBuffer[86]);      //状态位
                                    g_n32MotorSpeed = (int)(StateBuffer[89] << 8) + (int)(StateBuffer[88]);    //电机转速
                                    int APD_HV = (int)(StateBuffer[91] << 8) + (int)(StateBuffer[90]);         //APD高压
                                    int APD_temper = (int)(StateBuffer[93] << 8) + (int)(StateBuffer[92]);     //APD温度
                                    APD_temper = FormatConversion.HexToSigned(APD_temper);
                                    int Code_count = (int)(StateBuffer[95] << 8) + (int)(StateBuffer[94]);        //码盘线数
                                    int l_n32ScanZeroDisc = (int)(StateBuffer[97] << 8) + (int)(StateBuffer[96]); //平均零点
                                    //int l_n32ScanZeroDisc = (int)(StateBuffer[97] << 8) + (int)(StateBuffer[96]);     //APD温度
                                    g_n32maxzeroave = Math.Max(l_n32ScanZeroDisc, g_n32maxzeroave);
                                    g_n32minzeroave = Math.Min(l_n32ScanZeroDisc, g_n32minzeroave);
                                    g_n32zerodiffave = g_n32maxzeroave - g_n32minzeroave;
                                    if (g_bsavezeroflag)  //保存零点数据
                                    {
                                        g_swZeroave.Write(l_n32ScanZeroDisc + "\t");
                                        g_n32savezeroavetimes++;
                                        if (g_n32savezeroavetimes % 80 == 0)
                                        {
                                            g_swZeroave.Write("\r\n");
                                        }
                                    }
                                    if (g_n32RcvdLidarStateFrames >= 6) //每3s更新一次
                                    {
                                        //如果进度条的数据大于最大数据的85%进度条显示为红色；
                                        GroupBoxItemUpdata_Invoke(groupBox1, (APD_HV / 100.0).ToString(), 5);
                                        verticalProgressBar_APDHV.Value = (int)(APD_HV / 100.0);

                                        if (verticalProgressBar_APDHV.Value >= 300)
                                        {
                                            verticalProgressBar_APDHV.Color = System.Drawing.Color.Red;
                                            //RichTextBox_Invoke(richTextBox1, "APD高压超出指定范围！！\r\n");
                                        }
                                        else
                                        {
                                            verticalProgressBar_APDHV.Color = System.Drawing.Color.Green;
                                        }

                                        GroupBoxItemUpdata_Invoke(groupBox1, (APD_temper / 100.0).ToString(), 4);
                                        verticalProgressBar_APDTEMP.Value = (int)(Math.Abs(APD_temper) / 100.0);
                                        if ((APD_temper / 100.0) >= 80 || (APD_temper / 100.0) <= -20)
                                        {
                                            verticalProgressBar_APDTEMP.Color = System.Drawing.Color.Red;
                                        }
                                        else
                                        {
                                            verticalProgressBar_APDTEMP.Color = System.Drawing.Color.Green;
                                        }

                                        GroupBoxItemUpdata_Invoke(groupBox9, IRQNo.ToString() + "/" + IRQNotemp.ToString(), 3);
                                        GroupBoxItemUpdata_Invoke(groupBox9, Code_count.ToString() + "/" + scanstate.ToString(), 0);
                                        GroupBoxItemUpdata_Invoke(groupBox5, l_n32ScanZeroDisc.ToString(), 9);

                                        GroupBoxItemUpdata_Invoke(groupBox14, g_n32maxzero.ToString(), 4);
                                        GroupBoxItemUpdata_Invoke(groupBox14, g_n32minzero.ToString(), 3);
                                        GroupBoxItemUpdata_Invoke(groupBox14, g_n32zerodiff.ToString(), 2);
                                        MethodInvoker mi = new MethodInvoker(Draw_MotorSpeed);
                                        this.BeginInvoke(mi);

                                    }
                                    #endregion
                                }
                                break;

                            case 0x03:
                                {
                                    #region 清除复位次数
                                    MessageBox_Invoke("清除复位次数成功");
                                    #endregion
                                }
                                break;

                            case 0x04:
                                {
                                    #region 接收到心跳包

                                    g_bHeartStateFlg = true;
                                    MethodInvoker mi1 = new MethodInvoker(show_comm1);
                                    this.BeginInvoke(mi1);

                                    #endregion
                                }
                                break;


                            default:
                                break;
                        }
                        #endregion
                    }
                    break;
                case 0x06:                                  //应用设置
                    {
                        #region case6  应用设置
                        switch (p_cNetRcvdBuf[nBufID, 23])  //根据子命令号区分指令
                        {

                            case 0x01:
                                {
                                    #region 基本参数查询（起始角终止角）
                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }

                                    int ScanSta = (int)(l_byteBufStateFrame[26] << 8) +
                                                  (int)(l_byteBufStateFrame[27]);          //扫描起始角度
                                    int Proto = (int)(l_byteBufStateFrame[28] << 8) +
                                                  (int)(l_byteBufStateFrame[29]);          //数据协议
                                    int DrawSta = (int)(l_byteBufStateFrame[30] << 8) +
                                                   (int)(l_byteBufStateFrame[31]);         //绘图起始角
                                    int LWaveSet = (int)(l_byteBufStateFrame[32] << 8) +
                                                   (int)(l_byteBufStateFrame[33]);         //低回波数
                                    g_n32HeartState = (int)(l_byteBufStateFrame[34] << 8) +
                                                     (int)(l_byteBufStateFrame[35]);       //心跳状态
                                    int WatchDog = (int)(l_byteBufStateFrame[36] << 8) +
                                                   (int)(l_byteBufStateFrame[37]);        //看门狗配置

                                    GroupBoxItemUpdata_Invoke(groupBox3, ScanSta.ToString(), 11);
                                    if (Proto == 1)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "UDP", 0);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "TCP", 0);
                                    }

                                    if (DrawSta == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "225", 2);
                                        GroupBoxItemUpdata_Invoke(groupBox10, "225", 1);
                                        g_n32drawstaangle = 0;
                                    }
                                    else if (DrawSta == 1)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "315", 2);
                                        GroupBoxItemUpdata_Invoke(groupBox10, "315", 1);
                                        g_n32drawstaangle = 1;
                                    }

                                    if (WatchDog == 1)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "开启", 5);
                                    }
                                    else if (WatchDog == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "关闭", 5);
                                    }

                                    if (g_n32HeartState == 1)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "开启", 9);
                                    }
                                    else if (g_n32HeartState == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox3, "关闭", 9);
                                    }
                                    RichTextBox_Invoke(richTextBox1, "应用设置参数读取成功\r\n");
                                    #endregion
                                }
                                break;

                            case 0x02:
                                {
                                    #region 基本参数设置成功
                                    MessageBox_Invoke("显示参数下载成功");
                                    #endregion
                                }
                                break;

                            case 0x03:
                                {
                                    #region 重启设备（未使用）

                                    #endregion
                                }
                                break;

                            case 0x04:
                                {
                                    #region 修正参数查询
                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }

                                    int l_n32ModifyState = ((int)(l_byteBufStateFrame[26] << 8) +
                                                            (int)(l_byteBufStateFrame[27]));                     //修正状态                
                                    double l_n64HThresholdVoltage = ((int)(l_byteBufStateFrame[28] << 8) +
                                                                     (int)(l_byteBufStateFrame[29])) / 1000.0;   //阈值电压
                                    int l_n32MinPulseWidth = ((int)(l_byteBufStateFrame[30] << 8) +
                                                                 (int)(l_byteBufStateFrame[31]));               //最小脉宽
                                    int l_n32SmothingValue = ((int)(l_byteBufStateFrame[32] << 8) +
                                                                (int)(l_byteBufStateFrame[33]));                 //远距离滤波阈值


                                    if (l_n32ModifyState == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox2, "不修正", 2);
                                    }
                                    else if (l_n32ModifyState == 1)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox2, "修正", 2);
                                    }
                                    GroupBoxItemUpdata_Invoke(groupBox2, l_n32MinPulseWidth.ToString(), 6);
                                    GroupBoxItemUpdata_Invoke(groupBox2, l_n64HThresholdVoltage.ToString(), 7);
                                    GroupBoxItemUpdata_Invoke(groupBox2, l_n32SmothingValue.ToString(), 0);
                                    GroupBoxItemUpdata_Invoke(groupBox12, l_n32SmothingValue.ToString(), 0);

                                    RichTextBox_Invoke(richTextBox1, "修正参数读取成功\r\n");
                                    #endregion
                                }
                                break;

                            case 0x05:
                                {
                                    #region 修正参数设置成功
                                    MessageBox_Invoke("过渡点设置成功");
                                    #endregion
                                }
                                break;

                            case 0x06:
                                {
                                    #region 修正表查询

                                    switch (p_cNetRcvdBuf[nBufID, 25])
                                    {
                                        case 0x01://A9BAH表示当前激光已烧写好修正表，并且传输修正表的前20个数据供检测。;
                                            {
                                                if (g_n32Si2DConfigFrmFlg == 1)
                                                {
                                                    string str = "";
                                                    if (p_cNetRcvdBuf[nBufID, 24] == 0x01)       //高阈值修正表
                                                    {
                                                        str = "收到回复：查询高阈值修正表成功。前20个数据如下：\r\n";
                                                    }
                                                    else if (p_cNetRcvdBuf[nBufID, 24] == 0x02)  //反射率
                                                    {
                                                        str = "收到回复：查询反射率表成功。前20个数据如下：\r\n";
                                                    }
                                                    Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                                                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                                                    {
                                                        g_n32Si2DConfigFrmFlg = 0;
                                                    }
                                                }

                                                string l_strCorrectionDat = "";              //修正表前20 个数据
                                                for (int k = 0; k < 20; k++)                 //将修正表的前20个数据放入l_Si2DCorrectionData中；
                                                {
                                                    int tempdata = (int)((p_cNetRcvdBuf[nBufID, 2 * k + 27] << 8) |
                                                                         (p_cNetRcvdBuf[nBufID, 2 * k + 26]));
                                                    if ((tempdata & 0x8000) == 0x8000)      //该判断语句是判断负数，如果最高位置位，说明是负值，则转化为负值进行显示，仅仅是为了显示正确，没有其他作用
                                                    {
                                                        tempdata = (0xffff - tempdata) + 1;
                                                        tempdata = ~tempdata + 1;
                                                    }
                                                    l_strCorrectionDat += tempdata.ToString();
                                                    l_strCorrectionDat += " ";
                                                }
                                                if (g_n32Si2DConfigFrmFlg == 1)
                                                {
                                                    string str = l_strCorrectionDat + "\r\n";
                                                    Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                                                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                                                    {
                                                        g_n32Si2DConfigFrmFlg = 0;
                                                    }
                                                }

                                            }
                                            break;
                                        case 0x00:
                                            {
                                                if (g_n32Si2DConfigFrmFlg == 1)
                                                {
                                                    string str = "收到回复：激光未烧写修正表\r\n";
                                                    Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                                                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                                                    {
                                                        g_n32Si2DConfigFrmFlg = 0;
                                                    }
                                                }

                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    #endregion
                                }
                                break;

                            case 0x07:
                                {
                                    #region 修正表下载
                                    switch (p_cNetRcvdBuf[nBufID, 29])
                                    {
                                        case 0x01:                     //成功
                                            g_n32CorrectionPackNo = Convert.ToInt32(p_cNetRcvdBuf[nBufID, 27]);
                                            g_bCorrectionSuccessFlg = true;

                                            if (g_n32Si2DConfigFrmFlg == 1)
                                            {
                                                Single2DConfigure.g_n32Si2DCorrectedPkgNo = g_n32CorrectionPackNo;
                                                string str = "";
                                                if (p_cNetRcvdBuf[nBufID, 24] == 0x01)      //高阈值修正表
                                                {
                                                    str = "收到回复：高阈值第" + g_n32CorrectionPackNo.ToString() + "包发送成功\r\n";
                                                }
                                                else if (p_cNetRcvdBuf[nBufID, 24] == 0x02) //反射率
                                                {
                                                    str = "收到回复：反射率第" + g_n32CorrectionPackNo.ToString() + "包发送成功\r\n";
                                                }
                                                Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                                                if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                                                {
                                                    g_n32Si2DConfigFrmFlg = 0;
                                                }
                                                Single2DConfigure.Si2DSendResetEvent.Set();
                                            }

                                            break;
                                        case 0x02:                     //失败
                                            g_n32CorrectionPackNo = Convert.ToInt32(p_cNetRcvdBuf[nBufID, 24]);
                                            g_bCorrectionSuccessFlg = false;

                                            if (g_n32Si2DConfigFrmFlg == 1)
                                            {
                                                Single2DConfigure.g_n32Si2DCorrectedPkgNo = g_n32CorrectionPackNo;
                                                string str = "";
                                                if (p_cNetRcvdBuf[nBufID, 24] == 0x01)      //高阈值
                                                {
                                                    str = "收到回复：高阈值第" + g_n32CorrectionPackNo.ToString() + "包发送失败\r\n";
                                                }
                                                else if (p_cNetRcvdBuf[nBufID, 24] == 0x02) //低阈值
                                                {
                                                    str = "收到回复：低阈值第" + g_n32CorrectionPackNo.ToString() + "包发送失败\r\n";
                                                }
                                                Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                                                if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                                                {
                                                    g_n32Si2DConfigFrmFlg = 0;
                                                }
                                                Single2DConfigure.Si2DSendResetEvent.Set();
                                            }

                                            break;
                                        default:
                                            break;
                                    }

                                    #endregion
                                }
                                break;

                            case 0x08:
                                {
                                    #region MAC查询
                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }

                                   
                                    string l_smac = (Convert.ToString((int)(l_byteBufStateFrame[52] << 8) + (int)(l_byteBufStateFrame[53]), 16)).PadLeft(2, '0') + ":" +
                                                    (Convert.ToString((int)(l_byteBufStateFrame[54] << 8) + (int)(l_byteBufStateFrame[55]), 16)).PadLeft(2, '0') + ":" +
                                                    (Convert.ToString((int)(l_byteBufStateFrame[56] << 8) + (int)(l_byteBufStateFrame[57]), 16)).PadLeft(2, '0') + ":" +
                                                    (Convert.ToString((int)(l_byteBufStateFrame[58] << 8) + (int)(l_byteBufStateFrame[59]), 16)).PadLeft(2, '0') + ":" +
                                                    (Convert.ToString((int)(l_byteBufStateFrame[60] << 8) + (int)(l_byteBufStateFrame[61]), 16)).PadLeft(2, '0') + ":" +
                                                    (Convert.ToString((int)(l_byteBufStateFrame[62] << 8) + (int)(l_byteBufStateFrame[63]), 16)).PadLeft(2, '0');

                                    GroupBoxItemUpdata_Invoke(groupBox15, l_smac, 0);
                                    RichTextBox_Invoke(richTextBox1, "MAC读取成功\r\n");

                                    #endregion
                                }
                                break;

                            case 0x09:
                                {
                                    #region MAC设置
                                    MessageBox_Invoke("MAC下载成功\r\n");
                                    #endregion
                                }
                                break;

                            default:
                                break;
                        }
                        #endregion
                    }
                    break;
                case 0x07:                                  //生产设置          
                    {
                        #region case7  生产设置
                        switch (p_cNetRcvdBuf[nBufID, 23])  //根据子命令号区分指令
                        {

                            case 0x01:
                                {
                                    #region 功能设置参数查询（通道、高低整体偏移等）

                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }
                                    int l_n32EquipNo = (int)(l_byteBufStateFrame[26] << 16) +
                                                          (int)(l_byteBufStateFrame[27] << 8) +
                                                          (int)(l_byteBufStateFrame[28]);                      //设备号
                                    int l_n32Motor_speed = (int)(l_byteBufStateFrame[29] << 8) +
                                                           (int)(l_byteBufStateFrame[30]);                     //电机相数
                                    int l_n32ZeroOffset = (int)(l_byteBufStateFrame[31] << 8) +
                                                          (int)(l_byteBufStateFrame[32]);                      //零点偏移
                                    int l_n32HWholeDisc = (int)(l_byteBufStateFrame[33] << 8) +
                                                          (int)(l_byteBufStateFrame[34]);                      //高整体偏移
                                    l_n32HWholeDisc = FormatConversion.HexToSigned(l_n32HWholeDisc);
                                    int l_n32LD_power = (int)(l_byteBufStateFrame[35] << 8) +
                                                          (int)(l_byteBufStateFrame[36]);                      //LD功率控制

                                    GroupBoxItemUpdata_Invoke(groupBox5, l_n32HWholeDisc.ToString(), 12);
                                    GroupBoxItemUpdata_Invoke(groupBox5, l_n32ZeroOffset.ToString(), 9);
                                    GroupBoxItemUpdata_Invoke(groupBox5, l_n32EquipNo.ToString(), 15);

                                    if (Convert.ToInt32(l_byteBufStateFrame[37]) == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox5, "单点", 10);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox5, "扫描", 10);
                                    }

                                    if (l_n32Motor_speed == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox5, "15Hz", 2);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox5, "25Hz", 2);
                                    }
                                    //LD功率
                                    string l_sLD_power = l_n32LD_power.ToString();
                                    GroupBoxItemUpdata_Invoke(groupBox5, l_sLD_power, 0);

                                    RichTextBox_Invoke(richTextBox1, "生产设置设备参数读取成功\r\n");

                                    #endregion
                                }
                                break;

                            case 0x02:
                                {
                                    #region 功能设置参数设置
                                    MessageBox_Invoke("生产设置功能设置参数下载成功");
                                    #endregion
                                }
                                break;

                            case 0x03:
                                {
                                    #region 开启/关闭加热

                                    #endregion
                                }
                                break;

                            case 0x04:
                                {
                                    #region APD参数查询

                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }
                                    double l_dAPDTemperRadio = ((int)(l_byteBufStateFrame[24] << 8) +
                                                               (int)(l_byteBufStateFrame[25])) / 100.0; //APD温度系数
                                    double l_dAPDHvValue = ((int)(l_byteBufStateFrame[26] << 8) +
                                                         (int)(l_byteBufStateFrame[27])) / 100.0; //APD击穿电压
                                    double l_dAPDTemperValue = ((int)(l_byteBufStateFrame[28] << 8) +
                                                         (int)(l_byteBufStateFrame[29])) / 100.0; //APD击穿电压温度
                                    double l_dAPDHV_OP_Ratio = ((int)(l_byteBufStateFrame[30] << 8) +
                                                          (int)(l_byteBufStateFrame[31])) / 100.0; //电压衰减系数

                                    GroupBoxItemUpdata_Invoke(groupBox4, l_dAPDTemperRadio.ToString(), 0);
                                    GroupBoxItemUpdata_Invoke(groupBox4, l_dAPDHV_OP_Ratio.ToString(), 4);
                                    GroupBoxItemUpdata_Invoke(groupBox4, l_dAPDTemperValue.ToString(), 6);
                                    GroupBoxItemUpdata_Invoke(groupBox4, l_dAPDHvValue.ToString(), 7);
                                    RichTextBox_Invoke(richTextBox1, "生产设置APD参数读取成功\r\n");

                                    #endregion
                                }
                                break;

                            case 0x05:
                                {
                                    #region APD参数设置
                                    MessageBox_Invoke("生产设置APD参数下载成功");
                                    #endregion
                                }
                                break;

                            case 0x06:
                                {
                                    #region 单点开始/停止发数

                                    #endregion
                                }
                                break;
                            case 0x07:
                                {
                                    #region FPGA程序更新

                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }

                                    int l_n32PkgsSerialsno = (int)(l_byteBufStateFrame[26] << 8) + (int)(l_byteBufStateFrame[27]);   //当前包号
                                    int l_n32dataflag = (int)(l_byteBufStateFrame[28] << 8) + (int)(l_byteBufStateFrame[29]);       //下载标志位
                                    string l_smessage = "";
                                    if (l_n32dataflag == 1)
                                    {
                                        l_smessage = "第" + l_n32PkgsSerialsno.ToString() + "包数据下载成功" + "\r\n";
                                        FPGASendResetEvent.Set();

                                    }
                                    else
                                    {
                                        l_smessage = "第" + l_n32PkgsSerialsno.ToString() + "包数据下载失败" + "\r\n";
                                        g_bfpgaupdatesuccess = false;
                                        FPGAThread.Abort();
                                        RichTextBox_Invoke(rich_prodown, "FPGA程序更新失败！\r\n");
                                        MessageBox_Invoke("FPGA程序更新失败，请重新烧写！！");
                                    }
                                    RichTextBox_Invoke(rich_prodown, l_smessage);
                                    if (l_n32PkgsSerialsno == g_n32FPGATotalPkgs - 1)
                                    {
                                        serialPort1.Write(g_abQueryBasicPARM, 0, g_abQueryBasicPARM.Length);

                                        RichTextBox_Invoke(rich_prodown, "总共下载" + g_n32FPGATotalPkgs.ToString() + "包数据！\r\n");
                                        RichTextBox_Invoke(rich_prodown, "FPGA程序更新成功！\r\n");
                                        //g_SocketClient.Send(g_abRestartEquip);      //软件重新启动设备
                                        serialPort1.Write(g_abRestartEquip, 0, g_abRestartEquip.Length);
                                        RichTextBox_Invoke(richTextBox1, "重启设备中...\r\n");
   
                                    }

                                    #endregion
                                }
                                break;

                            case 0x08:
                                {
                                    #region 保存出厂参数
                                    MessageBox_Invoke("保存出厂参数成功");
                                    #endregion
                                }
                                break;

                            case 0x09:
                                {
                                    #region 恢复出厂参数
                                    MessageBox_Invoke("恢复出厂参数成功成功");
                                    #endregion
                                }
                                break;

                            case 0x0A:
                                {
                                    #region io状态检测查询

                                    int l_n32buflen = g_n32ArrayNetRcvSize[nBufID];
                                    byte[] l_byteBufStateFrame = new byte[l_n32buflen];
                                    for (int i = 0; i < l_n32buflen; i++)
                                    {
                                        l_byteBufStateFrame[i] = p_cNetRcvdBuf[nBufID, i];
                                    }
                                    if (l_byteBufStateFrame[l_n32buflen - 3] != ToolFunc.XorCheck_byte(2, l_byteBufStateFrame, 4))  //校验数据
                                    {
                                        return;
                                    }

                                    int l_n32iostate = l_byteBufStateFrame[29];
                                    int l_n32input1 = (l_n32iostate & 0x80) >> 7;
                                    int l_n32input2 = (l_n32iostate & 0x40) >> 6;
                                    int l_n32input3 = (l_n32iostate & 0x20) >> 5;
                                    int l_n32input4 = (l_n32iostate & 0x10) >> 4;
                                    int l_n32output1 = (l_n32iostate & 0x08) >> 3;
                                    int l_n32output2 = (l_n32iostate & 0x04) >> 2;
                                    int l_n32output3 = (l_n32iostate & 0x02) >> 1;
                                    int l_n32output4 = l_n32iostate & 0x01;
                                    if (l_n32input1 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "0", 6);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "1", 6);
                                    }

                                    if (l_n32input2 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "0", 4);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "1", 4);
                                    }
                                    if (l_n32input3 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "0", 2);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "1", 2);
                                    }
                                    if (l_n32input4 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "0", 0);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox7, "1", 0);
                                    }

                                    if (l_n32output1 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "0", 3);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "1", 3);
                                    }

                                    if (l_n32output2 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "0", 7);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "1", 7);
                                    }
                                    if (l_n32output3 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "0", 4);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "1", 4);
                                    }
                                    if (l_n32output4 == 0)
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "0", 0);
                                    }
                                    else
                                    {
                                        GroupBoxItemUpdata_Invoke(groupBox8, "1", 0);
                                    }

                                    #endregion
                                }
                                break;

                            case 0x0B:
                                {
                                    MessageBox_Invoke("IO状态设置成功");
                                }
                                break;

                            default:
                                break;
                        }
                        #endregion
                    }
                    break;

                case 0x09:
                    {
                        #region 安全区域相关
                        switch (p_cNetRcvdBuf[nBufID, 23])  //根据子命令号区分指令
                        {
                            case 0x01:
                                {
                                    #region bank号切换
                                    try
                                    {
                                        int bufferlength = g_n32ArrayNetRcvSize[nBufID];
                                        byte[] StateBuffer = new byte[bufferlength];
                                        for (int i = 0; i < bufferlength; i++)
                                        {
                                            StateBuffer[i] = p_cNetRcvdBuf[nBufID, i];
                                        }
                                        if (StateBuffer[bufferlength - 3] != ToolFunc.XorCheck_byte(2, StateBuffer, 4))
                                        {
                                            return;
                                        }
                                        g_n32BankNo = (int)(StateBuffer[29]);        //bank号
                                        MethodInvoker mi = new MethodInvoker(switchbank);
                                        this.BeginInvoke(mi);
                                    }
                                    catch
                                    { }
                                    #endregion
                                }
                                break;

                            case 0x02:
                                {
                                    #region BANK数据发送完成回复指令

                                    MessageBox_Invoke("BANK数据下载成功！");

                                    #endregion
                                }
                                break;
                            case 0x03:
                                {
                                    #region bank数据读取

                                    try
                                    {
                                        //int l_n32bankdatanull = 0;
                                        int bufferlength = g_n32ArrayNetRcvSize[nBufID];
                                        byte[] StateBuffer = new byte[bufferlength];
                                        for (int i = 0; i < bufferlength; i++)
                                        {
                                            StateBuffer[i] = p_cNetRcvdBuf[nBufID, i];
                                        }
                                        if (StateBuffer[bufferlength - 3] != ToolFunc.XorCheck_byte(2, StateBuffer, 4))
                                        {
                                            return;
                                        }

                                        g_n32banknotemp = (int)(StateBuffer[24]);           //BANK号
                                        int l_n32regiontype = (int)(StateBuffer[25]);       //数据类型：04：安全   05：警戒1   06：警戒2
                                        List<AngleAndRadius> l_larWLRbankdata = new List<AngleAndRadius>();
                                        bool l_ball0xff = true;                           //全是0xff
                                        for (int j = 0; j < 271; j++)
                                        {
                                            int l_n32tempdata = (int)(StateBuffer[j * 2 + 26] << 8) + (int)(StateBuffer[j * 2 + 1 + 26]);
                                            double l_n64angle = j + 360 - (int)(Math.Round(g_n64CenAngle, 0));
                                            if (l_n32tempdata != 0)
                                            {
                                                l_larWLRbankdata.Add(new AngleAndRadius(l_n64angle, l_n32tempdata));
                                            }
                                            if(l_n32tempdata<45000)
                                            {
                                                l_ball0xff = false;
                                            }
                                        }

                                        if ((l_larWLRbankdata.Count > 0) && (!l_ball0xff))            //接收的数据不全为0
                                        {
                                            if (l_n32regiontype == 4)
                                            {
                                                g_larField1 = l_larWLRbankdata;
                                            }
                                            else if (l_n32regiontype == 5)
                                            {
                                                g_larField2 = l_larWLRbankdata;
                                            }
                                            else if (l_n32regiontype == 6)
                                            {
                                                g_larField3 = l_larWLRbankdata;

                                                SortRegionData();              //将数据缓存在相应的bank中

                                                if (g_n32banknotemp != g_n32BankNo)
                                                {
                                                   //如果发生BANK切换，在切换中进行作图
                                                    MethodInvoker mi = new MethodInvoker(switchbank);
                                                    this.BeginInvoke(mi);
                                                }
                                                else                                                //直接作图
                                                {
                                                    if (g_n32BankNo < 9)
                                                    {
                                                        g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larField3);
                                                        g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larField2);
                                                        g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larField1);
                                                    }
                                                    else
                                                    {
                                                        g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larField3);
                                                        g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larField2);
                                                        g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larField1);
                                                        CalculateRegionData();
                                                    }
                                                    c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                                                g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            MessageBox_Invoke("WLR中未保存 " + g_n32banknotemp.ToString() + " 区域数据，请重新设置！");
                                            //if (l_n32regiontype == 4)
                                            //{
                                            //    MessageBox_Invoke("WLR中未保存4444区域数据，请重新设置！");
                                            //}
                                            //else if (l_n32regiontype == 5)
                                            //{
                                            //    MessageBox_Invoke("WLR中未保存5555区域数据，请重新设置！");
                                            //}
                                            //else if (l_n32regiontype == 6)
                                            //{
                                            //    MessageBox_Invoke("WLR中未保存6666区域数据，请重新设置！");
                                            //}
                                        }
                                    }

                                    catch
                                    {

                                    }

                                    #endregion
                                }
                                break;
                            default:
                                break;

                        }
                        #endregion
                    }
                    break;


                default:
                    break;
            }
        }


        #endregion

        #region 解析单点数据

        private delegate void SingleDataInvoke(int nBufID, byte[,] p_cNetRecvBuf);
        private void AnalysisSingleDataInvoke(int nBufID, byte[,] p_cNetRecvBuf)
        {
            SingleDataInvoke si = new SingleDataInvoke(AnalysisSingleData);
            this.BeginInvoke(si, new object[] { nBufID, p_cNetRecvBuf });
        }
        private void AnalysisSingleData(int nBufID, byte[,] p_cNetRecvBuf)
        {
            int l_u32reallen = ((int)(g_byteArrayNetRcvBuf[nBufID, 5] << 8) +
                                (int)(g_byteArrayNetRcvBuf[nBufID, 4]));
            l_u32reallen -= 10;                   //减去包头包尾
            l_u32reallen /= 4;                    //一个数据占四个字节
            int l_n32datano = l_u32reallen/3;     //一帧数据包含3种数据（上升、下降、计时）
            int l_n32datelen = l_n32datano * 4;

            g_n32Si2DRcvdPkgs++;
            if (g_n32RisingEdgeRcvdPkgs % g_n32Si2DIntervalPkgsToDraw == 0)
            {

                #region 保存单点数据
                if (Single2DConfigure.m_bSiDataSaveFlag == true)
                {
                    //将数组数据存入文件  上升沿
                    for (int i = 8; i < l_n32datelen + 8; i++)
                    {
                        int l_n32time = (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 3] & 0xFF) << 24) +
                                        (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 2] & 0xFF) << 16) +
                                        (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 1] & 0xFF) << 8) +
                                        (Int32)(g_byteArrayNetRcvBuf[nBufID, i] & 0xFF);
                        i += 3;
                        Single2DConfigure.Si2Dsb_ris.Write(l_n32time + "\t");
                    }
                    Single2DConfigure.Si2Dsb_ris.Write("\r\n");
                    g_n32Si2DSaveDataNo++;
                    //下降沿
                    for (int i = 8 + l_n32datelen; i < l_n32datelen * 2 + 8; i++)
                    {
                        int l_n32time = (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 3] & 0xFF) << 24) +
                                        (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 2] & 0xFF) << 16) +
                                        (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 1] & 0xFF) << 8) +
                                        (Int32)(g_byteArrayNetRcvBuf[nBufID, i] & 0xFF);
                        i += 3;
                        Single2DConfigure.Si2Dsb_ris.Write(l_n32time + "\t");
                    }
                    Single2DConfigure.Si2Dsb_ris.Write("\r\n");
                    g_n32Si2DSaveDataNo++;
                    //计时
                    for (int i = 8 + l_n32datelen * 2; i < l_n32datelen * 3 + 8; i++)
                    {
                        int l_n32time = (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 3] & 0xFF) << 24) +
                                        (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 2] & 0xFF) << 16) +
                                        (Int32)((g_byteArrayNetRcvBuf[nBufID, i + 1] & 0xFF) << 8) +
                                        (Int32)(g_byteArrayNetRcvBuf[nBufID, i] & 0xFF);
                        i += 3;
                        Single2DConfigure.Si2Dsb_tim.Write(l_n32time + "\t");
                    }
                    Single2DConfigure.Si2Dsb_tim.Write("\r\n");
                    g_n32Si2DSaveDataNo++;

                }

                #endregion

                //上升沿数据
                g_n32RisingEdgeDat = 0;
                for (int k = 8; k < l_n32datelen + 8; k = k + 4)
                {
                    g_n32RisingDat = (int)((g_byteArrayNetRcvBuf[nBufID, k + 3] & 0xFF) << 24) +
                                     (int)((g_byteArrayNetRcvBuf[nBufID, k + 2] & 0xFF) << 16) +
                                     (int)((g_byteArrayNetRcvBuf[nBufID, k + 1] & 0xFF) << 8) +
                                     (int)(g_byteArrayNetRcvBuf[nBufID, k] & 0xFF);

                    g_n32RisingEdgeDat += g_n32RisingDat;
                    g_listRisingDatY.Add(g_n32RisingDat);
                    g_n32Si2DDrawPkgs++;

                }
                axTChart1.Series(0).Clear();
                axTChart1.Series(0).AddArray(g_listRisingDatY.Count, g_listRisingDatY.ToArray());
                if (axTChart1.Series(0).Count >= g_n32Si2DXScalMax)
                {

                    TchartClear();
                }

                if (Single2DConfigure.m_bCacuFlag == true)
                {
                    Int32 nSum = (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 3] & 0xFF) << 24) +
                                 (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 2] & 0xFF) << 16) +
                                 (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 1] & 0xFF) << 8) +
                                 (Int32)(g_byteArrayNetRcvBuf[nBufID, 8 + 0] & 0xFF);
                    g_n32RisingEdgeSum += nSum;
                    g_n32RcvdCalcPkgsOfRisingEdge++;
                    if (g_n32RcvdCalcPkgsOfRisingEdge == g_n32Si2DPkgsToCalc)
                    {
                        g_n32RisingEdgeAvg = g_n32RisingEdgeSum / g_n32Si2DPkgsToCalc;
                    }
                }
                //下降沿数据
                g_n32FallingEdgeDat = 0;
                for (int k = 8 + l_n32datelen; k < l_n32datelen * 2 + 8; k = k + 4)
                {

                    g_n32FallingDat = (int)((g_byteArrayNetRcvBuf[nBufID, k + 3] & 0xFF) << 24) +
                                      (int)((g_byteArrayNetRcvBuf[nBufID, k + 2] & 0xFF) << 16) +
                                      (int)((g_byteArrayNetRcvBuf[nBufID, k + 1] & 0xFF) << 8) +
                                      (int)(g_byteArrayNetRcvBuf[nBufID, k] & 0xFF);
                    g_n32FallingEdgeDat += g_n32FallingDat;

                    g_listFallingDatY.Add(g_n32FallingDat);
                    g_n32Si2DDrawPkgs++;
                }
                axTChart1.Series(1).Clear();
                axTChart1.Series(1).AddArray(g_listFallingDatY.Count, g_listFallingDatY.ToArray());
                if (axTChart1.Series(1).Count >= g_n32Si2DXScalMax)
                {
                    TchartClear();
                }

                if (Single2DConfigure.m_bCacuFlag == true)
                {
                    Int32 nSum = (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 3 + l_n32datelen * 1] & 0xFF) << 24) +
                                 (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 2 + l_n32datelen * 1] & 0xFF) << 16) +
                                 (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 1 + l_n32datelen * 1] & 0xFF) << 8) +
                                 (Int32)(g_byteArrayNetRcvBuf[nBufID, 8 + l_n32datelen * 1] & 0xFF);
                    g_n32FallingEdgeSum += nSum;
                    g_n32RcvdCalcPkgsOfFallingEdge++;
                    if (g_n32RcvdCalcPkgsOfFallingEdge == g_n32Si2DPkgsToCalc)
                    {
                        g_n32FallingEdgeAvg = g_n32FallingEdgeSum / g_n32Si2DPkgsToCalc;
                    }
                }


                g_n32PulseWidth = (g_n32FallingEdgeDat - g_n32RisingEdgeDat) / l_n32datano;//脉宽

                #region 界面显示统计数据

                if (g_n32Si2DConfigFrmFlg == 1)
                {
                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                    {
                        g_n32Si2DConfigFrmFlg = 0;
                    }
                    else
                    {
                        Single2DConfigure.g_n32SiAvgdata = g_n32PulseWidth;
                        Single2DConfigure.g_n32SiRecvNum = g_n32Si2DRcvdPkgs;
                        Single2DConfigure.g_n32SiDrawNum = g_n32Si2DDrawPkgs;
                        Single2DConfigure.g_n32SiSaveNum = g_n32Si2DSaveDataNo;
                        Single2DConfigure.UpdataSI2DCFG();
                    }
                }

                #endregion

                //计时值
                for (int k = 8 + l_n32datelen * 2; k < l_n32datelen * 3 + 8; k = k + 4)
                {
                    g_n32TimingDat = (int)((g_byteArrayNetRcvBuf[nBufID, k + 3] & 0xFF) << 24) +
                                     (int)((g_byteArrayNetRcvBuf[nBufID, k + 2] & 0xFF) << 16) +
                                     (int)((g_byteArrayNetRcvBuf[nBufID, k + 1] & 0xFF) << 8) +
                                     (int)(g_byteArrayNetRcvBuf[nBufID, k] & 0xFF);
                    g_listTimingDatY.Add(g_n32TimingDat);
                    g_n32Si2DDrawPkgs++;
                }
                axTChart1.Series(2).Clear();
                axTChart1.Series(2).AddArray(g_listTimingDatY.Count, g_listTimingDatY.ToArray());
                if (axTChart1.Series(2).Count >= g_n32Si2DXScalMax)
                {
                    TchartClear();
                }

                if (Single2DConfigure.m_bCacuFlag == true)
                {
                    Int32 nSum = (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 3 + l_n32datelen * 2] & 0xFF) << 24) +
                                 (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 2 + l_n32datelen * 2] & 0xFF) << 16) +
                                 (Int32)((g_byteArrayNetRcvBuf[nBufID, 8 + 1 + l_n32datelen * 2] & 0xFF) << 8) +
                                 (Int32)(g_byteArrayNetRcvBuf[nBufID, 8 + l_n32datelen * 2] & 0xFF);
                    g_n32TimingEdgeSum += nSum;
                    g_n32RcvdCalcPkgsOfTimingEdge++;
                    if (g_n32RcvdCalcPkgsOfTimingEdge == g_n32Si2DPkgsToCalc)
                    {
                        g_n32TimingEdgeAvg = g_n32TimingEdgeSum / g_n32Si2DPkgsToCalc;
                    }
                }
            }
        }

        #endregion

        #region 完成脉宽和基准减上沿的计算
        private void CompleteCalculateSingleSampleData()
        {
            if (g_n32RisingEdgeAvg > 1 && g_n32FallingEdgeAvg > 5)
            {
                if (g_n32ReviseCalculationTimes == 0)
                {
                    g_n32StdRisingEdgeDat = g_n32RisingEdgeAvg;
                }
                g_n32PulseWidthAvg = g_n32FallingEdgeAvg - g_n32RisingEdgeAvg;          //脉宽
                g_n32StdSubRisingEdge = g_n32StdRisingEdgeDat - g_n32RisingEdgeAvg;     //基准-上沿

                //将基准和脉宽保存到数组（二维）(通过list即可查找位置所以不用二维数组)
                g_n32LstPulseWidthToWrite.Add(g_n32PulseWidthAvg);
                g_n32LstStdSubRisingEdgeToWrite.Add(g_n32StdSubRisingEdge);
                g_n32LTimingEdgeToWrite.Add(g_n32TimingEdgeAvg);

                if (g_n32Si2DConfigFrmFlg == 1)
                {    //si2dcfg 更新操作日志内容（下沿 上沿 基准）
                    //更新操作日志（脉宽   基准-上沿    -------------------+换行）

                    string str = "下沿：" + " " + g_n32FallingEdgeAvg.ToString() + "\r\n" +
                                 "上沿：" + " " + g_n32RisingEdgeAvg.ToString() + "\r\n" +
                                 "计时：" + " " + g_n32TimingEdgeAvg.ToString() + "\r\n" +
                                 //"基准：" + " " + g_n32StdRisingEdgeDat.ToString() + "\r\n" +
                                 "脉宽：" + " " + g_n32PulseWidthAvg.ToString() + "\r\n" +
                                 //"基准-上沿：" + " " + g_n32StdSubRisingEdge.ToString() + "\r\n" +
                                 "---------------------" + "\r\n";
                    Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                    {
                        g_n32Si2DConfigFrmFlg = 0;
                    }
                }
                g_n32ReviseCalculationTimes++;
                if (g_n32Si2DConfigFrmFlg == 1)
                {
                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                    {
                        g_n32Si2DConfigFrmFlg = 0;
                    }
                    else
                    {
                        MethodInvoker mi = new MethodInvoker(stopcaculatebtn);
                        this.BeginInvoke(mi);
                    }
                }
                g_n32RcvdCalcPkgsOfRisingEdge = 0;
                g_n32RcvdCalcPkgsOfFallingEdge = 0;
                g_n32RcvdCalcPkgsOfTimingEdge = 0;

                //g_n32LstPulseWidthReadFromFile.Add(g_n32PulseWidthAvg);
                //g_n32LstStdSubRisingEdgeReadFromFile.Add(g_n32StdSubRisingEdge);	
            }
            else if (g_n32FallingEdgeAvg <= 5 || g_n32RisingEdgeAvg <= 5)
            {

                if (g_n32Si2DConfigFrmFlg == 1)
                {
                    if (Single2DConfigure == null || Single2DConfigure.IsDisposed)
                    {
                        g_n32Si2DConfigFrmFlg = 0;
                    }
                    else
                    {
                        string str = "数据错误，无法计算" + "\r\n";
                        Single2DConfigure.UpDatarichTextBoxTOPERATEMESSAGE(str);
                        //Single2DConfigure.UpData_richTextBoxT_OPERATEMESSAGE2();//si2dcfg 更新操作日志内容（"数据错误，无法计算" +"\r\n"）
                        MethodInvoker mi = new MethodInvoker(stopcaculatebtn);
                        this.BeginInvoke(mi);
                    }


                }
                g_n32RcvdCalcPkgsOfRisingEdge = 0;
                g_n32RcvdCalcPkgsOfFallingEdge = 0;
                g_n32RcvdCalcPkgsOfTimingEdge = 0;
            }
        }
        #endregion

        private void stopcaculatebtn()
        {
            Single2DConfigure.StopCaCuLate();
        }
        #endregion

        #region  查找本机串口
        private void btn_scanserialport_Click(object sender, EventArgs e)
        {
            ResearchSerialPort();
            //axTChart2.Axis.Bottom.Maximum = 90;
        }
        #endregion

        #endregion   串口

        #region 是否解析反射率

        public bool g_bReflectEnable = false;

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                g_bReflectEnable = true;
            }
            else
            {
                g_bReflectEnable = false;
            }
        }
        #endregion

        #region 将广播到的ip填充到连接网络的txt中

        public void UseBroadCastIp(string p_sip,string p_sport )
        {
            //IP = p_sip;                         //网络连接使用的IP
            //Port = p_sport;                     //网络连接使用的端口号
            //txt_ipset.Text = p_sip;
            //txt_portset.Text = p_sport;
            //ServerIpForm.txt_ip.Text = p_sip;
            //ServerIpForm.txt_port.Text = p_sport;
        }


        #endregion

        #region 显示二重回波
        public bool g_bEchoEnable = false;
        private void checkBox_echo_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox_echo.Checked)
            {
                g_bEchoEnable = true;
            }
            else
            {
                g_bEchoEnable = false;
            }
        }
        #endregion

        #region toolstrip

        #region 还原缩放
        private void tsbtn_zoomauto_Click(object sender, EventArgs e)
        {
            g_n32BitMapWidth = pictureBox1.Width;
            g_n32BitMapHeight = pictureBox1.Height;
            g_epCanvasOrigin = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2); //画布原点在设备上的坐标(屏幕坐标原点)

            c_DrawCd.bitmap = new Bitmap(g_n32BitMapWidth, g_n32BitMapHeight);
            c_DrawCd.graphics = Graphics.FromImage(c_DrawCd.bitmap);
            c_DrawCd.PicBox = pictureBox1;
            g_n32Scale = 1.1F;
            c_DrawCd.g_n32Scale = g_n32Scale;
            c_DrawCd.g_epCanvasOrigin = g_epCanvasOrigin;
            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                        g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
            pictureBox1.Image = c_DrawCd.bitmap;

        }

        #endregion

        #region 点操作

        bool g_baddpointflag = false;
        bool g_bdeletepointflag = false;
        bool g_bmodifypointflag = false;

        int g_n32tsbtn_addno = 0;      //添加点按下的次数
        int g_n32tsbtn_modifyno = 0;   //修改点按下的次数
        int g_n32tsbtn_deleteno = 0;   //删除点按下的次数

        

        #region 添加点

        private void stbtn_addpoint_Click(object sender, EventArgs e)
        {
            try
            {
                g_n32tsbtn_addno++;
                if (g_n32tsbtn_addno % 2 == 0)
                {
                    this.tsbtn_addpoint.Image = Properties.Resources.add;
                    tsbtn_editpoint.Enabled = true;
                    tsbtn_deletepoint.Enabled = true;

                    g_baddpointflag = false;
                    g_bdeletepointflag = false;
                    g_bmodifypointflag = false;

                    g_n32tsbtn_addno = 0;
                   c_DrawCd.g_bpicturerefresh = true;
                }
                else
                {
                    this.tsbtn_addpoint.Image = Properties.Resources.addselected;
                    this.tsbtn_editpoint.Image = Properties.Resources.modify;
                    this.tsbtn_deletepoint.Image = Properties.Resources.delete;
                    tsbtn_editpoint.Enabled = false;
                    tsbtn_deletepoint.Enabled = false;

                    g_n32tsbtn_deleteno = 0;
                    g_n32tsbtn_modifyno = 0;

                    g_baddpointflag = true;
                    g_bdeletepointflag = false;
                    g_bmodifypointflag = false;

                    c_DrawCd.g_bpicturerefresh = false;
                }
            }
            catch
            { }
        }
        #endregion

        #region 删除点

        private void tsbtn_deletepoint_Click(object sender, EventArgs e)
        {
            try
            {
                g_n32tsbtn_deleteno++;
                if (g_n32tsbtn_deleteno % 2 == 0)
                {
                    this.tsbtn_deletepoint.Image = Properties.Resources.delete;
                    tsbtn_editpoint.Enabled = true;
                    tsbtn_addpoint.Enabled = true;

                    g_baddpointflag = false;
                    g_bdeletepointflag = false;
                    g_bmodifypointflag = false;

                    g_n32tsbtn_deleteno = 0;
                    c_DrawCd.g_bpicturerefresh = true;
                }
                else
                {
                    this.tsbtn_addpoint.Image = Properties.Resources.add;
                    this.tsbtn_editpoint.Image = Properties.Resources.modify;
                    this.tsbtn_deletepoint.Image = Properties.Resources.deleteselected;
                    tsbtn_editpoint.Enabled = false;
                    tsbtn_addpoint.Enabled = false;

                    g_n32tsbtn_addno = 0;
                    g_n32tsbtn_modifyno = 0;

                    g_baddpointflag = false;
                    g_bdeletepointflag = true;
                    g_bmodifypointflag = false;

                    c_DrawCd.g_bpicturerefresh = false;
                }
            }
            catch
            { }
        }
        #endregion

        #region 修改点

        private void stbtn_editpoints_Click(object sender, EventArgs e)
        {
            try
            {
                g_n32tsbtn_modifyno++;
                if (g_n32tsbtn_modifyno % 2 == 0)
                {
                    this.tsbtn_editpoint.Image = Properties.Resources.modify;
                    if (g_n32BankNo < 5)
                    {
                        tsbtn_deletepoint.Enabled = true;
                        tsbtn_addpoint.Enabled = true;
                    }

                    g_baddpointflag = false;
                    g_bdeletepointflag = false;
                    g_bmodifypointflag = false;

                    g_n32tsbtn_modifyno = 0;
                    c_DrawCd.g_bpicturerefresh = true;
                }
                else
                {
                    this.tsbtn_addpoint.Image = Properties.Resources.add;
                    this.tsbtn_editpoint.Image = Properties.Resources.editselected;
                    this.tsbtn_deletepoint.Image = Properties.Resources.delete;
                    tsbtn_deletepoint.Enabled = false;
                    tsbtn_addpoint.Enabled = false;

                    g_n32tsbtn_addno = 0;
                    g_n32tsbtn_deleteno = 0;

                    g_baddpointflag = false;
                    g_bdeletepointflag = false;
                    g_bmodifypointflag = true;

                    c_DrawCd.g_bpicturerefresh = false;
                }
            }
            catch
            { }
        }
        #endregion

        #region 从下位机读取区域数据
        private void tsbtn_upload_Click(object sender, EventArgs e)
        {
            try
            {   /************************  流程  *******************
                 * 
                 *  网络接收271个距离值
                 *  根据距离将271个点还原成极坐标点（角度和距离）
                 *  将极坐标点转换成屏幕坐标点
                 *  添加坐标零点
                 *  更新坐标系
                 *  
                 * **************************************************/

                //读取下位机bank数据之前现将之前的数据清除
                //g_arrayLstWHistoryRegion.Clear();
                //g_arrayLstWPolarHistoryRegion.Clear();

                //g_arrayLstSHistoryRegion.Clear();
                //g_arrayLstSPolarHistoryRegion.Clear();

                //g_arrayLstW2HistoryRegion.Clear();
                //g_arrayLstW2PolarHistoryRegion.Clear();
                //c_DrawCd.PictureBox_Refresh(g_arrayLstWHistoryRegion, g_arrayLstW2HistoryRegion,
                //                                   g_arrayLstSHistoryRegion, g_arrayLstSHistoryPt,
                //                                   g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY);

                if(serialPort1.IsOpen)
                {
                    byte[] l_abQuaryBankData = new byte[34] {0xff,0xaa,0x00,0x1e,0x00,0x00,0x00,0x00,
                                                             0x00,0x00,0x01,0x01,0x00,0x05,0x00,0x00, 
                                                             0x00,0x00,0x00,0x00,0x00,0x00,0x09,0x03,
                                                             0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xee,0xee};

                    l_abQuaryBankData[24] = Convert.ToByte(g_n32BankNo);
                    l_abQuaryBankData[31] = ToolFunc.XorCheck_byte(2, l_abQuaryBankData, 4);
                    serialPort1.Write(l_abQuaryBankData, 0, l_abQuaryBankData.Length);
                }


            }
            catch
            {
                MessageBox_Invoke("BANK数据读取失败");
                return;
            }

        }
        #endregion

        #endregion

        #region BANK切换
        private void tscomb_bank_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int l_n32selectedindex = tscomb_bank.SelectedIndex;

                //bank号大于四禁止添加点和删除点
                if (l_n32selectedindex > 3)
                {
                    tsbtn_addpoint.Enabled = false;
                    tsbtn_deletepoint.Enabled = false;
                    tsbtn_editpoint.Enabled = true;
                    g_baddpointflag = false;
                    g_bdeletepointflag = false;

                }
                else
                {
                    tsbtn_addpoint.Enabled = true;
                    tsbtn_deletepoint.Enabled = true;
                    tsbtn_editpoint.Enabled = true;
                    g_baddpointflag = false;
                    g_bdeletepointflag = false;
                }

                switch (l_n32selectedindex)
                {
                    #region bank1-bank4
                    case 0:
                        {
                            g_n32BankNo = 1;
                            if (g_larBank1_Field1.Count ==0)
                            {
                                g_lpField3.Clear();

                                g_lpField3.Add(g_Bank1Field3point1);
                                g_lpField3.Add(g_Bank1Field3point2);
                                g_lpField3.Add(g_Bank1Field3point3);
                                g_lpField3.Add(g_Bank1Field3point4);

                                CalculateRegionData();
                            }
                            else
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank1_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank1_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank1_Field1);

                                g_larField1 = g_larBank1_Field1;
                                g_larField2 = g_larBank1_Field2;
                                g_larField3 = g_larBank1_Field3;
                            }

                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                          g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;
                        }

                        break;

                    case 1:
                        {
                            g_n32BankNo = 2;
                            if (g_larBank2_Field1.Count == 0 )           //手动切换bank
                            {
                                g_lpField3.Clear();

                                g_lpField3.Add(g_Bank2Field3point1);
                                g_lpField3.Add(g_Bank2Field3point2);
                                g_lpField3.Add(g_Bank2Field3point3);
                                g_lpField3.Add(g_Bank2Field3point4);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank2_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank2_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank2_Field1);

                                g_larField1 = g_larBank2_Field1;
                                g_larField2 = g_larBank2_Field2;
                                g_larField3 = g_larBank2_Field3;

                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                          g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;
                        }
                        break;

                    case 2:
                        {
                            g_n32BankNo = 3;
                            if (g_larBank3_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3.Clear();

                                g_lpField3.Add(g_Bank3Field3point1);
                                g_lpField3.Add(g_Bank3Field3point2);
                                g_lpField3.Add(g_Bank3Field3point3);
                                g_lpField3.Add(g_Bank3Field3point4);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank3_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank3_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank3_Field1);

                                g_larField1 = g_larBank3_Field1;
                                g_larField2 = g_larBank3_Field2;
                                g_larField3 = g_larBank3_Field3;
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                           g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;
                        }
                        break;

                    case 3:
                        {
                            g_n32BankNo = 4;
                            if (g_larBank4_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3.Clear();

                                g_lpField3.Add(g_Bank4Field3point1);
                                g_lpField3.Add(g_Bank4Field3point2);
                                g_lpField3.Add(g_Bank4Field3point3);
                                g_lpField3.Add(g_Bank4Field3point4);

                                CalculateRegionData();
                                
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank4_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank4_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank4_Field1);

                                g_larField1 = g_larBank4_Field1;
                                g_larField2 = g_larBank4_Field2;
                                g_larField3 = g_larBank4_Field3;
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                        g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;
                        }
                        break;
                    #endregion

                    #region bank5-bank8
                    case 4:
                        {
                            g_n32BankNo = 5;
                            if (g_larBank5_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3.Clear();
                                g_lpField2.Clear();
                                g_lpField1.Clear();
                                g_lpField3.Add(g_Bank5Field3point1);
                                g_lpField3.Add(g_Bank5Field3point2);
                                g_lpField3.Add(g_Bank5Field3point3);

                                g_lpField2.Add(g_Bank5Field2point1);
                                g_lpField2.Add(g_Bank5Field2point2);
                                g_lpField2.Add(g_Bank5Field2point3);

                                g_lpField1.Add(g_Bank5Field1point1);
                                g_lpField1.Add(g_Bank5Field1point2);
                                g_lpField1.Add(g_Bank5Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank5_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank5_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank5_Field1);
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 5:
                        {
                            g_n32BankNo = 6;
                            if (g_larBank6_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3.Clear();
                                g_lpField2.Clear();
                                g_lpField1.Clear();
                                g_lpField3.Add(g_Bank6Field3point1);
                                g_lpField3.Add(g_Bank6Field3point2);
                                g_lpField3.Add(g_Bank6Field3point3);

                                g_lpField2.Add(g_Bank6Field2point1);
                                g_lpField2.Add(g_Bank6Field2point2);
                                g_lpField2.Add(g_Bank5Field2point3);

                                g_lpField1.Add(g_Bank6Field1point1);
                                g_lpField1.Add(g_Bank6Field1point2);
                                g_lpField1.Add(g_Bank6Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank6_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank6_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank6_Field1);
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 6:
                        {
                            g_n32BankNo = 7;
                            if (g_larBank7_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3.Clear();
                                g_lpField2.Clear();
                                g_lpField1.Clear();
                                g_lpField3.Add(g_Bank7Field3point1);
                                g_lpField3.Add(g_Bank7Field3point2);
                                g_lpField3.Add(g_Bank7Field3point3);

                                g_lpField2.Add(g_Bank7Field2point1);
                                g_lpField2.Add(g_Bank7Field2point2);
                                g_lpField2.Add(g_Bank7Field2point3);

                                g_lpField1.Add(g_Bank7Field1point1);
                                g_lpField1.Add(g_Bank7Field1point2);
                                g_lpField1.Add(g_Bank7Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank7_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank7_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank7_Field1);
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 7:
                        {
                            g_n32BankNo = 8;
                            if (g_larBank8_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3.Clear();
                                g_lpField2.Clear();
                                g_lpField1.Clear();
                                g_lpField3.Add(g_Bank8Field3point1);
                                g_lpField3.Add(g_Bank8Field3point2);
                                g_lpField3.Add(g_Bank8Field3point3);

                                g_lpField2.Add(g_Bank8Field2point1);
                                g_lpField2.Add(g_Bank8Field2point2);
                                g_lpField2.Add(g_Bank8Field2point3);

                                g_lpField1.Add(g_Bank8Field1point1);
                                g_lpField1.Add(g_Bank8Field1point2);
                                g_lpField1.Add(g_Bank8Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3 = c_DrawCd.PolarPointToScreenPoint(g_larBank8_Field3);
                                g_lpField2 = c_DrawCd.PolarPointToScreenPoint(g_larBank8_Field2);
                                g_lpField1 = c_DrawCd.PolarPointToScreenPoint(g_larBank8_Field1);
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;
                    #endregion 

                    case 8:
                        {
                            g_n32BankNo = 9;
                            if (g_larBank9_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank9Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank9Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank9Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank9Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank9Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank9Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank9Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank9Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank9Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank9_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank9_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank9_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 9:
                        {
                            g_n32BankNo = 10;
                            if (g_larBank10_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank10Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank10Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank10Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank10Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank10Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank10Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank10Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank10Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank10Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank10_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank10_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank10_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 10:
                        {
                            g_n32BankNo = 11;
                            if (g_larBank11_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank11Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank11Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank11Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank11Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank11Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank11Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank11Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank11Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank11Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank11_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank11_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank11_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 11:
                        {
                            g_n32BankNo = 12;
                            if (g_larBank12_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank12Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank12Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank12Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank12Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank12Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank12Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank12Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank12Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank12Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank12_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank12_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank12_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 12:
                        {
                            g_n32BankNo = 13;
                            if (g_larBank13_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank13Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank13Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank13Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank13Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank13Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank13Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank13Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank13Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank13Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank13_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank13_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank13_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 13:
                        {
                            g_n32BankNo = 14;
                            if (g_larBank14_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank14Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank14Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank14Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank14Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank14Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank14Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank14Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank14Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank14Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank14_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank14_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank14_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    case 14:
                        {
                            g_n32BankNo = 15;
                            if (g_larBank15_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank15Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank15Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank15Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank15Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank15Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank15Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank15Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank15Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank15Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank15_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank15_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank15_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;
                    case 15:
                        {
                            g_n32BankNo = 16;
                            if (g_larBank16_Field1.Count == 0)           //手动切换bank
                            {
                                g_lpField3Bank9_16.Clear();
                                g_lpField2Bank9_16.Clear();
                                g_lpField1Bank9_16.Clear();

                                g_lpField3Bank9_16.Add(g_Bank16Field3point1);
                                g_lpField3Bank9_16.Add(g_Bank16Field3point2);
                                g_lpField3Bank9_16.Add(g_Bank16Field3point3);

                                g_lpField2Bank9_16.Add(g_Bank16Field2point1);
                                g_lpField2Bank9_16.Add(g_Bank16Field2point2);
                                g_lpField2Bank9_16.Add(g_Bank16Field2point3);

                                g_lpField1Bank9_16.Add(g_Bank16Field1point1);
                                g_lpField1Bank9_16.Add(g_Bank16Field1point2);
                                g_lpField1Bank9_16.Add(g_Bank16Field1point3);

                                CalculateRegionData();
                            }
                            else                                       //接收到下位机的bank
                            {
                                g_lpField3Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank16_Field3);
                                g_lpField2Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank16_Field2);
                                g_lpField1Bank9_16 = c_DrawCd.PolarPointToScreenPoint(g_larBank16_Field1);
                                CalculateRegionData();
                            }
                            c_DrawCd.PictureBox_Refresh(g_lpField3, g_lpField2, g_lpField1, g_n32BankNo,
                                                            g_n64ArrayRegionZhiX, g_n64ArrayRegionZhiY, g_n32DrawStyle);
                            pictureBox1.Image = c_DrawCd.bitmap;

                        }
                        break;

                    default:
                        break;
                }

            }
            catch
            { }
        }

        #endregion

        #endregion

        #region 保存零点

        FileStream g_fsZero;      //上升沿
        StreamWriter g_swZero;
        string g_filepathZero;
        int g_n32maxzero = 0;
        int g_n32minzero = 88888;
        int g_n32zerodiff = 0;

        string ZeroDataFilesPath = Environment.CurrentDirectory.ToString() + @"\OperationLog\";
        bool g_bsavezeroflag = false;
        int g_n32savezerotimes = 0;       //保存零点次数
        int g_n32savezeroavetimes = 0;    //保存零点次数

        FileStream g_fsZeroave;      //上升沿
        StreamWriter g_swZeroave;
        string g_filepathZeroave;
        int g_n32maxzeroave = 0;
        int g_n32minzeroave = 88888;
        int g_n32zerodiffave = 0;

        private void btn_savezero_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_savezero.Text == "保存零点")
                {
                    btn_savezero.Text = "暂停保存";
                    string l_smac = txt_mac.Text.Replace(":", "-");
                    string filename = l_smac+"--实时零点统计数据-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".txt_Zero";   //固定形式的文件名及后缀
                    g_filepathZero = ZeroDataFilesPath + filename;
                    g_fsZero = new FileStream(g_filepathZero, FileMode.Append, FileAccess.Write);
                    g_swZero = new StreamWriter(g_fsZero);
                    g_n32maxzero = 0;
                    g_n32minzero = 88888;
                    g_n32zerodiff = 0;
                    g_bsavezeroflag = true;
                    g_swZero.Write("开始保存时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "\r\n\r\n\r\n");

                    string filenameave = l_smac + "--平均零点统计数据-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".txt_Zero";   //固定形式的文件名及后缀
                    g_filepathZeroave = ZeroDataFilesPath + filenameave;
                    g_fsZeroave = new FileStream(g_filepathZeroave, FileMode.Append, FileAccess.Write);
                    g_swZeroave = new StreamWriter(g_fsZeroave);
                    g_n32maxzeroave = 0;
                    g_n32minzeroave = 88888;
                    g_n32zerodiffave = 0;
                    g_swZeroave.Write("开始保存时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "\r\n\r\n\r\n");
                }
                else
                {
                    btn_savezero.Text = "保存零点";

                    g_swZero.Write("\r\n\r\n零点最大值：" + g_n32maxzero.ToString() + "\r\n");
                    g_swZero.Write("\r\n零点最小值：" + g_n32minzero.ToString() + "\r\n");
                    g_swZero.Write("\r\n差值：" + g_n32zerodiff.ToString() + "\r\n");
                    g_swZero.Write("\r\n\r\n停止保存时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "\r\n");
                    g_swZero.Close();
                    g_fsZero.Close();

                    g_swZeroave.Write("\r\n\r\n零点最大值：" + g_n32maxzeroave.ToString() + "\r\n");
                    g_swZeroave.Write("\r\n零点最小值：" + g_n32minzeroave.ToString() + "\r\n");
                    g_swZeroave.Write("\r\n差值：" + g_n32zerodiffave.ToString() + "\r\n");
                    g_swZeroave.Write("\r\n\r\n停止保存时间：" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "\r\n");
                    g_swZeroave.Close();
                    g_fsZeroave.Close();

                    g_bsavezeroflag = false;
                }
            }
            catch
            { }
        }
        #endregion

        private void btn_clearzero_Click(object sender, EventArgs e)
        {
            g_n32maxzero = 0;
            g_n32minzero = 88888;
            g_n32zerodiff = 0;

            g_n32maxzeroave = 0;
            g_n32minzeroave = 88888;
            g_n32zerodiffave = 0;
        }

      
    }
}
