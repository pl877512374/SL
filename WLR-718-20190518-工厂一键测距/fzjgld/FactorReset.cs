using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.OleDb;

namespace fzjgld
{
    public partial class FactorReset : Form
    {

        Form1 main = new Form1();
        public bool g_bproflg = false;
        public bool g_breserchflg = false;
       // public bool g_bsafetyflg = false;
        //数据加密读取方法
        string mdbname = "Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:DataBase PassWord=123;User Id=Admin;Data source=" + Application.StartupPath + @"\SafetyLidar.mdb";
        public FactorReset()
        {
            InitializeComponent();
        }

        private void FactorReset_Load(object sender, EventArgs e)
        {
            main = (Form1)this.Owner;
            g_bproflg = false;
            g_breserchflg = false;
            //g_bsafetyflg = false;
        }
        public static OleDbConnection Open_Conn(string ConnStr)
        {
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            Conn.Open();
            return Conn;
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (txt_checkcode.Text == "wanjikeji")
            {

                OleDbConnection myconn = Open_Conn(mdbname);       //打开数据库
                string mysql;                                     //数据库操作命令  注意此处的表名要加[]
                OleDbCommand mycommd;                            //判断数据库中是否存在要查找的用户名

                mysql = "update [user] set 密码='" + "wanjishengchan" + "' where 用户名= '" + "生产人员" + "'";  //注意此处格式，每个参数都要加单引号
                mycommd = new OleDbCommand(mysql, myconn);
                int rows = mycommd.ExecuteNonQuery();  //其中ExecuteNonQuery被成功更改的元组数量
                if (rows == 1)
                {
                    g_bproflg = true;
                }
                mysql = "update [user] set 密码='" + "wanjiyanfa" + "' where 用户名= '" + "研发人员" + "'";  //注意此处格式，每个参数都要加单引号
                mycommd = new OleDbCommand(mysql, myconn);
                rows = mycommd.ExecuteNonQuery();  //其中ExecuteNonQuery被成功更改的元组数量
                if (rows == 1)
                {
                    g_breserchflg = true;
                }
                //mysql = "update [user] set 密码='" + "1111" + "' where 用户名= '" + "安全人员" + "'";  //注意此处格式，每个参数都要加单引号
                //mycommd = new OleDbCommand(mysql, myconn);
                //rows = mycommd.ExecuteNonQuery();  //其中ExecuteNonQuery被成功更改的元组数量
                //if (rows == 1)
                //{
                //    g_bsafetyflg = true;
                //}
                if (g_bproflg && g_breserchflg )
                {
                    MessageBox.Show("恢复出厂设置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("恢复出厂设置失败，请重新设置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                myconn.Close();      //关闭数据库
            }
            else
            {
                MessageBox.Show("校验码错误，请重新输入！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
