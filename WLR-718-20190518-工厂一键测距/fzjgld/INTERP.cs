using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO ;
using System.Threading;

namespace fzjgld
{
    public partial class INTERP : Form
    {
        #region 参数定义
        //Form1 main = new Form1();
        public string InterpFilePath = "";
        public int j;
        //public int[] g_n32InterpDatax = new int[100];//读取拟合结果存放数组
        //public int[] g_n32InterpDatay = new int[100];//读取拟合结果存放数组

        public List<int> g_n32InterpDataListx = new List<int>();
        public List<int> g_n32InterpDataListy = new List<int>();
        public int IntSi2DChSel = 0;
        #endregion

        public INTERP()
        {
            InitializeComponent();
        }
        private void INTERP_Load(object sender, EventArgs e)
        {
            //main = (Form1)this.Owner;
            txt_PATH .Text =@"d:\xy.txt_H";
        }

        #region 文件路径
        private void btn_path_Click(object sender, EventArgs e)
        {
            try
            {
                string extension;
                if (IntSi2DChSel == 0)
                {
                    saveFileDialog1.FileName = "";
                    saveFileDialog1.Filter = "H Files(*.txt_H)|*.txt_H";
                    saveFileDialog1.ShowDialog();
                    if (saveFileDialog1.FileName != "")
                    {
                        extension = Path.GetExtension(saveFileDialog1.FileName);
                        if (extension != ".txt_H")
                        {
                            MessageBox.Show("请选择高阈值文件");
                            return;
                        }
                        txt_PATH.Text = saveFileDialog1.FileName;
                        InterpFilePath = saveFileDialog1.FileName;

                    }
                }
                else
                {
                    saveFileDialog1.FileName = "";
                    saveFileDialog1.Filter = "L Files(*.txt_L)|*.txt_L";
                    saveFileDialog1.ShowDialog();
                    if (saveFileDialog1.FileName != "")
                    {
                        extension = Path.GetExtension(saveFileDialog1.FileName);
                        if (extension != ".txt_L")
                        {
                            MessageBox.Show("请选择低阈值文件");
                            return;
                        }
                        txt_PATH.Text = saveFileDialog1.FileName;
                        InterpFilePath = saveFileDialog1.FileName;
                       
                    }
                }
                
            }
            catch
            {
                return;
            }
        }
        #endregion

        //对样本数据进行排序
        private bool ArraySort(double[] X, double[] Y, int size)
        {
            int i;
            double temp;
            if (X[0] < X[size - 1])  //数组x是升序不作处理
            {
                for (i = 0; i < size - 1; i++)
                {
                    //if (X[i] < X[i + 1])
                    //    ;
                    //else
                    if (X[i] > X[i + 1])
                        return false;
                }
                return true;
            }
            else                //递减变递增     
            {
                int len = size / 2;
                for (i = 0; i < len; i++)
                {
                    temp = X[i];
                    X[i] = X[size - i - 1];
                    X[size - i - 1] = temp;
                    temp = Y[i];
                    Y[i] = Y[size - i - 1];
                    Y[size - i - 1] = temp;
                }
                for (i = 0; i < size - 1; i++)
                {
                    //if (X[i] < X[i + 1])       //单调递增
                    //    ;
                    //else
                    //    return false;     //非单调递增
                    if (X[i] > X[i + 1])
                        return false;
                }
                return true;
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                int m_start = Convert.ToInt32(txt_START.Text);
                int m_end = Convert.ToInt32(txt_END.Text);
                int m_interval = Convert.ToInt32(txt_INTERVAL.Text);
                int m_movedata = Convert.ToInt32(txt_DIS.Text);
                List<double> dxlist = new List<double>();
                int temp = 0;
                int arraylength = g_n32InterpDataListx.Count();
                //int[] arrayx = new int[arraylength];
                int N=0;   //输入控制点数量
                //int M=0;   //输出的插入点数量
                //List<double> listx = new List<double>();  //输入控制点list
                //List<double> listy = new List<double>();  //输入控制点list
                double[] X = new double[arraylength];
                double[] Y = new double[arraylength];
                List<double> listz = new List<double>();  //输出控制点list
                List<double> listf = new List<double>();  //输出控制点list
                List<double> listh = new List<double>();  //间距，缓存运算中间结果
                List<double> lista = new List<double>();  //间距，缓存运算中间结果
                List<double> listb = new List<double>();  //间距，缓存运算中间结果
                List<double> listc = new List<double>();  //间距，缓存运算中间结果
                List<double> listd = new List<double>();  //间距，缓存运算中间结果

                List<int> listdy = new List<int>();
                double E, E1, K, K1, H1;

                if (arraylength < 3)
                {
                    MessageBox.Show("修正数据个数小于3，请重新修正", "Error");
                    return;
                }
                else
                {
                    fastLine1.Clear();
                    points1.Clear();
                    fastLine1.Add(g_n32InterpDataListx.ToArray(), g_n32InterpDataListy.ToArray());
                    points1.Add(g_n32InterpDataListx.ToArray(), g_n32InterpDataListy.ToArray());
                   
                    N = arraylength - 1;
                    int nDataCount = N + 1;
                    Array.Copy(g_n32InterpDataListx.ToArray(), X, arraylength);
                    Array.Copy(g_n32InterpDataListy.ToArray(), Y, arraylength);
                    bool m_bCreate = ArraySort(X,Y,nDataCount);//对样本数据进行排序
                    if (m_bCreate)
                    {
                        //m_bCreate = Spline();
                        int i, P, L;
                        for (i = 1; i <= N; i++)
                        {
                            listh.Add(X[i] - X[i - 1]);
                        }

                        L = N - 1;
                        lista.Add(1);
                        listb.Add(3 * (Y[1] - Y[0]) / listh[0]);
                        for (i = 1; i <= L; i++)
                        {
                            lista.Add(listh[i - 1] / (listh[i - 1] + listh[i]));
                            listb.Add(3 * ((1 - lista[i]) * (Y[i] - Y[i - 1]) / listh[i - 1] + lista[i] * (Y[i + 1] - Y[i]) / listh[i]));
                        }
                        lista.Add(0);
                        listb.Add(3 * (Y[N] - Y[N - 1]) / listh[N - 1]);

                        for (i = 0; i <= N; i++)
                        {
                            listd.Add(2);
                        }

                        for (i = 0; i <= N; i++)
                        {
                            listc.Add(1 - lista[i]);
                        }

                        P = N;
                        for (i = 1; i <= P; i++)
                        {
                            if (Math.Abs(listd[i]) <= 0.000001)
                            {
                                MessageBox.Show("Math.Abs(listd[i]) <= 0.000001");
                                return;
                            }

                            lista[i - 1] = lista[i - 1] / listd[i - 1];
                            listb[i - 1] = listb[i - 1] / listd[i - 1];
                            listd[i] = lista[i - 1] * (-listc[i]) + listd[i];
                            listb[i] = -listc[i] * listb[i - 1] + listb[i];
                        }
                        listb[P] = listb[P] / listd[P];
                        for (i = 1; i <= P; i++)
                        {
                            listb[P - i] = listb[P - i] - lista[P - i] * listb[P - i + 1];
                        }
                        //return true;   
                        for (int k = m_start; k < m_end; k += m_interval)
                        {

                            int j;
                            int dbInX = k;
                            double dbOutY = 0;
                            if (dbInX < X[0])
                            {
                                j = 0;
                            }
                            else if (dbInX > X[N])
                            {
                                j = N - 1;
                            }
                            else
                            {
                                for (j = 1; j <= N; j++)
                                {
                                    if (dbInX <= X[j])
                                    {
                                        j = j - 1;

                                        break;
                                    }
                                }

                            }

                            //////////////////////////////////////////////////////////////////////////   
                            E = X[j + 1] - dbInX;
                            E1 = E * E;
                            K = dbInX - X[j];
                            K1 = K * K;
                            H1 = listh[j] * listh[j];

                            dbOutY = (3 * E1 - 2 * E1 * E / listh[j]) * Y[j] + (3 * K1 - 2 * K1 * K / listh[j]) * Y[j + 1];
                            dbOutY = dbOutY + (listh[j] * E1 - E1 * E) * listb[j] - (listh[j] * K1 - K1 * K) * listb[j + 1];
                            dbOutY = dbOutY / H1;
                            //dbOutY = dbOutY + m_movedata;
                            listdy.Add((int)dbOutY);
                            //MessageBox .Show ("")
                        }
                    }
                    else
                    {
                        MessageBox.Show("输入的差值数据源不正确，请重新输入！", "Error");
                        return;
                    }
                }


                for (int i = m_start; i < m_end; i += m_interval)
                {
                    dxlist.Add((double)i);
                }
                fastLine1.Add(dxlist.ToArray(), listdy.ToArray());

                FileStream fs = new FileStream(@InterpFilePath, FileMode.Append, FileAccess.Write);
                StreamWriter sb = new StreamWriter(fs);
                for (int jj = 0; jj < listdy.Count; jj++)
                {
                    j++;
                    temp = (int)(listdy[jj] + m_movedata);
                    sb.Write(temp + "\t\t");
                    if (j > 4)
                    {
                        sb.Write("\r\n");
                        j = 0;
                    }
                }
                sb.Close();
                fs.Close();
                MessageBox.Show("数据保存成功","提示",MessageBoxButtons .OK ,MessageBoxIcon .Information );
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
                return;
            }
        }


        private void INTERP_FormClosing(object sender, FormClosingEventArgs e)  
        {
            this.Hide();
            e.Cancel = true;//保证窗口不被关闭
        }


    }
}
