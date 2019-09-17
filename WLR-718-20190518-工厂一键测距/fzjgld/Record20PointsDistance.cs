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
using NPOI.POIFS.FileSystem;
using System.IO;

namespace fzjgld
{
    public partial class Record20PointsDistance : Form
    {
        #region 参数定义

        Form1 main = new Form1();
        ExcelDocumentName ExcelFormName;
        public string ExcelName = null;
        string g_sStandardPath = Environment.CurrentDirectory.ToString() + @"\NetDataFiles\" + "StandardDiatance.xlsx";
        public int[] g_an32ScanMax = new int[20];                    //扫描数据最大值
        public int[] g_an32ScanMin = new int[20];                    //扫描数据最小值
        public double[] g_an64ScanAve = new double[20];              //扫描数据平均值
        public double[] g_an64DatPercent = new double[20];           //扫描数据在离散度范围内的百分比(概率)
        
        #endregion

        public Record20PointsDistance()
        {
            InitializeComponent();
        }

        private void Record20PointsDistance_Load(object sender, EventArgs e)
        {
            main = (Form1)this.Owner;
            main.g_n32PkgsToCalcAvg_R = Convert.ToInt32(txt_measureinterval.Text);
            main.g_n32DatDispersion_R = Convert.ToInt32(txt_DISPERSION.Text);
        }

        #region 距离标定
        private void btn_standard_Click(object sender, EventArgs e)
        {
            try
            {

                //FileStream file2007 = new FileStream(g_sStandardPath, FileMode.Create);

                if (btn_standard.Text == "距离标定")
                {
                    DialogResult dr = MessageBox.Show("是否重新标定数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Yes)
                    {
                        btn_standard.Text = "保存标定结果";
                        for (int i = 0; i < 20; i++)
                        {
                            string l_srealdistxtbox = "txt_RELDISC" + (i + 1).ToString();
                            object l_orealdistxtbox = FindControlAsName(l_srealdistxtbox);
                            ((TextBox)l_orealdistxtbox).Enabled = true;

                            string l_sangletxtbox = "txt_AngleNum" + (i + 1).ToString();
                            object l_oangletxtbox = FindControlAsName(l_sangletxtbox);
                            ((TextBox)l_oangletxtbox).Enabled = true;
                        }
                        txt_DISPERSION.Enabled = true;
                        txt_measureinterval.Enabled = true;

                        if (File.Exists(g_sStandardPath) == false)
                        {
                            FileStream file2007 = new FileStream(g_sStandardPath, FileMode.Create);
                            IWorkbook workbook = new XSSFWorkbook();
                            ISheet sheet = workbook.CreateSheet("标定数据");            //在工作簿中：建立空白工作表
                            IRow row = sheet.CreateRow(0);                              //在工作表中：建立行，参数为行号，从0计
                            ICell cell = row.CreateCell(0);                             //在行中：建立单元格，参数为列号，从0计
                            cell.SetCellValue("一键测距距离标定");                     //设置单元格内容
                            ICellStyle style = workbook.CreateCellStyle();
                            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //设置单元格的样式：水平对齐居中
                            IFont font = workbook.CreateFont();                         //新建一个字体样式对象
                            //font.Boldweight = (Int16)FontBoldWeight.Bold;                          //设置字体加粗样式
                            font.FontName = "宋体";
                            font.FontHeight = 16;
                            style.SetFont(font);                                        //使用SetFont方法将字体样式添加到单元格样式中 
                            cell.CellStyle = style;                                     //将新的样式赋给单元格
                            row.Height = 15 * 20;                                       //设置单元格的高度
                            //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                            NPOI.SS.Util.CellRangeAddress region = new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 6);//设置一个合并单元格区域，使用上下左右定义CellRangeAddress区域
                            sheet.AddMergedRegion(region);

                            IRow row1 = sheet.CreateRow(2);
                            row1.CreateCell(0).SetCellValue("编号");
                            row1.CreateCell(1).SetCellValue("实际值");
                            row1.CreateCell(2).SetCellValue("测量点");
                            row1.CreateCell(3).SetCellValue(" ");
                            row1.CreateCell(4).SetCellValue(" ");
                            row1.CreateCell(5).SetCellValue("间隔");
                            row1.CreateCell(6).SetCellValue("离散度");
                            //设置单元格的宽度
                            for (int i = 0; i < 7; i++)
                            {
                                sheet.SetColumnWidth(i, 9 * 256);//设置单元格的宽度
                                //row1.GetCell(i).CellStyle = style;//设置单元格格式
                            }

                            int len = 20;
                            for (int i = 0; i < len; i++)
                            {
                                string l_srealdistxtbox = "txt_RELDISC" + (i + 1).ToString();
                                object l_orealdistxtbox = FindControlAsName(l_srealdistxtbox);
                                string l_sangletxtbox = "txt_AngleNum" + (i + 1).ToString();
                                object l_oangletxtbox = FindControlAsName(l_sangletxtbox);

                                IRow rowx = sheet.CreateRow(i + 3);
                                rowx.CreateCell(0).SetCellValue(i + 1);
                                rowx.CreateCell(1).SetCellValue(((TextBox)l_orealdistxtbox).Text);
                                rowx.CreateCell(2).SetCellValue(((TextBox)l_oangletxtbox).Text);
                                rowx.CreateCell(3);
                                rowx.CreateCell(4);
                                if (i == 0)
                                {
                                    rowx.CreateCell(5).SetCellValue(100);
                                    rowx.CreateCell(6).SetCellValue(15);
                                }
                                else
                                {
                                    rowx.CreateCell(5);
                                    rowx.CreateCell(6);
                                }
                            }
                            workbook.Write(file2007);
                            file2007.Close();
                            workbook.Close();
                            //加载excel
                            //System.Diagnostics.Process.Start(g_sStandardPath);
                        }
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {

                    for (int i = 0; i < 20; i++)
                    {
                        string l_srealdistxtbox = "txt_RELDISC" + (i + 1).ToString();
                        object l_orealdistxtbox = FindControlAsName(l_srealdistxtbox);
                        ((TextBox)l_orealdistxtbox).Enabled = false;

                        string l_sangletxtbox = "txt_AngleNum" + (i + 1).ToString();
                        object l_oangletxtbox = FindControlAsName(l_sangletxtbox);
                        ((TextBox)l_oangletxtbox).Enabled = false;
                    }
                    txt_DISPERSION.Enabled = false;
                    txt_measureinterval.Enabled = false;

                    //添加文件选择保存功能 add by yjun 20190128

                    //FileStream fs1 = new FileStream(saveDlg.FileName, FileMode.Create, FileAccess.Write);
                    //fs1.Close();
                    //g_sStandardPath = saveDlg.FileName;
                    FileStream fs = File.Open(g_sStandardPath, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);
                    IWorkbook wbook = null;
                    string sheetName = "标定数据";
                    ISheet sheet = null;
                    if (g_sStandardPath.IndexOf(".xlsx") > 0) // 2007版本
                    {
                        wbook = new XSSFWorkbook(fs);
                    }
                    else if (g_sStandardPath.IndexOf(".xls") > 0) // 2003版本
                    {
                        wbook = new HSSFWorkbook(fs);
                    }
                    if (sheetName != null)
                    {
                        sheet = wbook.GetSheet(sheetName);
                        if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        {
                            sheet = wbook.GetSheetAt(0);
                        }
                    }
                    else
                    {
                        sheet = wbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        int len = 20;
                        for (int i = 0; i < len; i++)
                        {
                            string l_srealdistxtbox = "txt_RELDISC" + (i + 1).ToString();
                            object l_orealdistxtbox = FindControlAsName(l_srealdistxtbox);
                            string l_sangletxtbox = "txt_AngleNum" + (i + 1).ToString();
                            object l_oangletxtbox = FindControlAsName(l_sangletxtbox);

                            string s1 = ((TextBox)l_orealdistxtbox).Text;

                            IRow rowx = sheet.GetRow(i + 3);
                            rowx.GetCell(0).SetCellValue(i + 1);
                            rowx.GetCell(1).SetCellValue(Convert.ToInt32(((TextBox)l_orealdistxtbox).Text));
                            rowx.GetCell(2).SetCellValue(Convert.ToInt32(((TextBox)l_oangletxtbox).Text));

                            if (i == 0)
                            {
                                rowx.GetCell(5).SetCellValue(Convert.ToInt32(txt_measureinterval.Text));
                                rowx.GetCell(6).SetCellValue(Convert.ToInt32(txt_DISPERSION.Text));
                            }
                        }
                        SaveFileDialog saveDlg = new SaveFileDialog();
                        saveDlg.Filter = "Excle2007(*.xlsx)|*.xlsx;|Excle2003(*.xls)|*.xls";
                        if (saveDlg.ShowDialog() == DialogResult.OK)
                        {
                            FileStream file = File.OpenWrite(saveDlg.FileName);   //不加这句无法写入数据
                            wbook.Write(file);


                            wbook.Close();
                            file.Close();
                            fs.Close();
                        }
                        else
                        {
                            MessageBox.Show("保存失败，请重新保存！");
                            return;
                        }
                    }
                    MessageBox.Show("标定数据保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btn_standard.Text = "距离标定";
                }

            }
            catch
            { }

        }
        #endregion

        #region 导入数据
        private void btn_import_Click(object sender, EventArgs e)
        {
            //string fileName = Environment.CurrentDirectory.ToString() + @"\NetDataFiles\" + "\\" + "StandardDiatance.xlsx";
            string fileName = "";
            //添加选择文件对话空 added by Yang Jun 20190128
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.FileName;
                //其他代码
            }
            else
            {
                MessageBox.Show("数据导入失败，请确认选择正确文件！");
                return;
            }
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            try
            {
                string sheetName = "标定数据";
                if (File.Exists(fileName))
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    //fs = File.OpenRead(fileName);
                    //using (fs = File.OpenRead(fileName))
                    {
                        //workbook = new HSSFWorkbook(fs);

                        if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        {
                            workbook = new XSSFWorkbook(fs);
                        }
                        else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        {
                            workbook = new HSSFWorkbook(fs);
                        }
                        if (sheetName != null)
                        {
                            sheet = workbook.GetSheet(sheetName);
                            if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                            {
                                sheet = workbook.GetSheetAt(0);
                            }
                        }
                        else
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                        if (sheet != null)
                        {
                            IRow Row4 = sheet.GetRow(3);      //  第三行数据 间隔和离散度
                            ICell cell6 = Row4.GetCell(5);
                            ICell cell7 = Row4.GetCell(6);
                            if (cell6 != null)
                            {
                                cell6.SetCellType(CellType.String);
                                txt_measureinterval.Text = cell6.StringCellValue;
                            }
                            else
                            {
                                MessageBox.Show("标定文件数据不存在，请重新标定！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (cell7 != null)
                            {
                                cell7.SetCellType(CellType.String);
                                txt_DISPERSION.Text = cell7.StringCellValue;
                            }
                            else
                            {
                                MessageBox.Show("标定文件数据不存在，请重新标定！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            for (int i = 0; i < 20; i++)
                            {
                                IRow Rowx = sheet.GetRow(i + 3);      //  第三行数据 间隔和离散度
                                ICell cellx2 = Rowx.GetCell(1);       //  实际值
                                ICell cellx3 = Rowx.GetCell(2);       //  测量点
                                cellx2.SetCellType(CellType.String);
                                cellx3.SetCellType(CellType.String);
                                string l_scellx2 = cellx2.StringCellValue;
                                string l_scellx3 = cellx3.StringCellValue;

                                main.g_an32PtToMeasure_R[i] = Convert.ToInt32(l_scellx3);

                                if (l_scellx2 == "" || l_scellx3 == "")
                                {
                                    MessageBox.Show("标定文件第" + (i + 4).ToString() + "行数据不存在，请重新标定！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                string l_srealdistxtbox = "txt_RELDISC" + (i + 1).ToString();
                                object l_orealdistxtbox = FindControlAsName(l_srealdistxtbox);
                                ((TextBox)l_orealdistxtbox).Text = l_scellx2;

                                string l_sangletxtbox = "txt_AngleNum" + (i + 1).ToString();
                                object l_oangletxtbox = FindControlAsName(l_sangletxtbox);
                                ((TextBox)l_oangletxtbox).Text = l_scellx3;

                            }

                        }
                        workbook.Close();
                        fs.Close();
                        MessageBox.Show("标定数据导入成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        main.g_n32PkgsToCalcAvg_R = Convert.ToInt32(txt_measureinterval.Text);
                        main.g_n32DatDispersion_R = Convert.ToInt32(txt_DISPERSION.Text);
                    }
                }
                else
                {
                    MessageBox.Show("标定文件不存在，请重新标定！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据导入失败，请确认选择正确文件！");
                return;
            }

        }
        #endregion

        #region 更新界面
        public void UpDateRecordForm()
        {
            for (int i = 0; i < 20; i++)
            {
                string l_savedistxtbox = "txt_AVERAGEDis" + (i + 1).ToString();
                object l_oavedistxtbox = FindControlAsName(l_savedistxtbox);
                ((TextBox)l_oavedistxtbox).Text = g_an64ScanAve[i].ToString();

                string l_smaxdistxtbox = "txt_MAXDIS" + (i + 1).ToString();
                object l_omaxdistxtbox = FindControlAsName(l_smaxdistxtbox);
                ((TextBox)l_omaxdistxtbox).Text = g_an32ScanMax[i].ToString();

                string l_smintxtbox = "txt_MINDIS" + (i + 1).ToString();
                object l_omintxtbox = FindControlAsName(l_smintxtbox);
                ((TextBox)l_omintxtbox).Text = g_an32ScanMin[i].ToString();

                string l_sprotxtbox = "txt_PROBABILITY" + (i + 1).ToString();
                object l_oprotxtbox = FindControlAsName(l_sprotxtbox);
                ((TextBox)l_oprotxtbox).Text = g_an64DatPercent[i].ToString();

                if (i < 11)
                {
                    if (g_an64DatPercent[i] > 99)
                    {
                        ((TextBox)l_oprotxtbox).BackColor = System.Drawing.Color.LawnGreen; 
                    }
                    else
                    {
                        ((TextBox)l_oprotxtbox).BackColor = System.Drawing.Color.DarkSalmon;
                    }
                }
            }
        }
        #endregion 

        #region 根据名字查找控件

        private object FindControlAsName(string p_name)
        {
            return this.GetType().GetField(p_name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);
        }

        #endregion

        #region 保存结果
        string path = null;
        public bool g_bRecordFlg = false;     //将测距值记录到excel 此时停止更新数据
        private void btn_record_disper_Click(object sender, EventArgs e)
        {
            try
            {
                g_bRecordFlg = true;
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

                int len = 11;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        IRow rowx = sheet.CreateRow(i + 2);
                        string l_srealdistxtbox = "txt_RELDISC" + (i + 1).ToString();
                        object l_orealdistxtbox = FindControlAsName(l_srealdistxtbox);
                        int l_n32realdis = Convert.ToInt32((((TextBox)l_orealdistxtbox).Text));
                        rowx.CreateCell(0).SetCellValue(l_n32realdis);
                        rowx.CreateCell(1).SetCellValue(g_an32ScanMax[i]);
                        rowx.CreateCell(2).SetCellValue(g_an32ScanMin[i]);
                        rowx.CreateCell(3).SetCellValue(g_an64ScanAve[i]);

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
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");

                    IRow row2 = sheet.GetRow(2);

                    ICell cellx13 = row2.GetCell(13);
                    //cellx13.SetCellFormula("AVERAGE(H3:H" + (len + 2).ToString() + ")"); //偏移量：平均偏差的平均值
                    cellx13.SetCellFormula("AVERAGE(H4:H12)");
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

                    IRow row3 = sheet.GetRow(3);
                    ICell cell314 = row3.GetCell(14);
                    cell314.SetCellValue("0.05-4m"); //0.05-4 
                    ICell cell315 = row3.GetCell(15);
                    cell315.SetCellFormula("SUM(MAX(F3:F10),MIN(F3:G10)*(-1))"); //0.05-4 绝对精度：最大偏差的最大值减去最小偏差的最小值

                    IRow row4 = sheet.GetRow(4);
                    ICell cell414 = row4.GetCell(14);
                    cell414.SetCellValue("0.05-6m"); //0.05-6
                    ICell cell415 = row4.GetCell(15);
                    cell415.SetCellFormula("SUM(MAX(F3:F13),MIN(G3:G13)*(-1))"); //0.5-6绝对精度：最大偏差的最大值减去最小偏差的最小值

                    //IRow row5 = sheet.GetRow(5);
                    //ICell cell514 = row5.GetCell(14);
                    //cell514.SetCellValue("6-8m"); //0.5-8m
                    //ICell cell515 = row5.GetCell(15);
                    //cell515.SetCellFormula("SUM(MAX(F13:F16),MIN(G13:G16)*(-1))"); //绝对精度：最大偏差的最大值减去最小偏差的最小值

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
                        g_bRecordFlg = false;
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

    }
}
