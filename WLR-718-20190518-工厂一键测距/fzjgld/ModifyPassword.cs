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
    public partial class ModifyPassword : Form
    {
        Form1 main = new Form1();
        //数据加密读取方法
        string mdbname = "Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:DataBase PassWord=123;User Id=Admin;Data source=" + Application.StartupPath + @"\SafetyLidar.mdb";
        
        public ModifyPassword()
        {
            InitializeComponent();
        }

        private void ModifyPassword_Load(object sender, EventArgs e)
        {
            main = (Form1)this.Owner;
            comb_user.SelectedIndex = 0;
        }

        public static OleDbConnection Open_Conn(string ConnStr)
        {
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            Conn.Open();
            return Conn;
        }

        #region 保存修改密码
        private void btn_changepassword_Click(object sender, EventArgs e)
        {
            try
            {
                string oldpassword = txt_oldpassword.Text;
                string newpassword = txt_newpassword.Text;
                string verifypassword = txt_verifypassword.Text;
                string username = comb_user.Text;
                string mdbpassword = null;


                OleDbConnection myconn = Open_Conn(mdbname);                             //打开数据库
                string mysql = "select 密码 from [user] where 用户名='" + username + "'";//数据库操作命令  注意此处的表名要加[]
                OleDbCommand mycommd = new OleDbCommand(mysql, myconn);                  //判断数据库中是否存在要查找的用户名
                OleDbDataReader sqlr = mycommd.ExecuteReader();
                if (sqlr.Read())
                {
                    sqlr.Close();
                    mycommd.Parameters.Add(username, OleDbType.VarChar, 10).Value = username;
                    mdbpassword = mycommd.ExecuteScalar().ToString();
                }
                else
                {
                    MessageBox.Show("用户不存在，请重新输入！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (oldpassword == mdbpassword)
                {
                    if (newpassword != null)
                    {
                        if (newpassword == verifypassword)
                        {
                            if (newpassword != oldpassword)
                            {
                                mysql = "update [user] set 密码='" + newpassword + "' where 用户名= '" + username + "'";  //注意此处格式，每个参数都要加单引号
                                mycommd = new OleDbCommand(mysql, myconn);
                                int rows = mycommd.ExecuteNonQuery(); //其中ExecuteNonQuery被成功更改的元组数量
                                if (rows == 1)
                                {
                                    MessageBox.Show("密码更改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    this.Close();//关闭窗体
                                }
                            }
                            else
                            {
                                MessageBox.Show("新密码与原密码不能相同，请重新输入！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("两次密码输入不同，请重新输入！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("新密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("原密码错误！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                myconn.Close();      //关闭数据库
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            main.FactoryResetSecret();
            this.Close();
        }


    }
}
