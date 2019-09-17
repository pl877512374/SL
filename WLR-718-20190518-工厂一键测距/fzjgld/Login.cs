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
    public partial class Login : Form
    {
        Form1 main = new Form1();
        //public string g_sResearchPassword = "wanjiyanfa";
        //public string g_sProductPassword = "wanjishengchan";
        //数据加密读取方法
        string g_smdbname = "Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:DataBase PassWord=123;User Id=Admin;Data source=" + Application.StartupPath + @"\SafetyLidar.mdb";
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            main = (Form1)this.Owner;
            cmb_user.SelectedIndex = 0;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            try
            {
                int l_n32index = cmb_user.SelectedIndex;

                string l_susername = cmb_user.Text;//comb_user.Text;
                string l_spassword = accessoledb.ReadPassword(g_smdbname, l_susername);  //读取用户名对应的密码

                if (l_spassword != null)
                {
                    if (txt_password.Text == l_spassword)
                    {
                        //this.Hide();
                        MessageBox.Show("登录成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        main.loginToolStripMenuItem.Text = "退出登录";

                        if (l_n32index == 0)
                        {
                            main.Text = main.g_sFormName + main.g_sVersionDate + "-生产人员";
                            main.g_sUserName = "生产人员";
                            main.g_bProdLoginFlg = true;
                            main.tabPage1.Parent = main.tabControl2;
                            main.tabPage8.Parent = main.tabControl2;
                            main.tabPage3.Parent = main.tabControl2;
                            main.tabPage6.Parent = main.tabControl2;
                            main.tabPage10.Parent = main.tabControl1;
                        }
                        else if (l_n32index == 1)
                        {
                            main.Text = main.g_sFormName + main.g_sVersionDate + "-研发人员";
                            main.g_sUserName = "研发人员";
                            main.g_bResearchLoginFlg = true;
                            main.tabPage1.Parent = main.tabControl2;
                            main.tabPage8.Parent = main.tabControl2;
                            main.tabPage3.Parent = main.tabControl2;
                            main.tabPage6.Parent = main.tabControl2;
                            main.tabPage10.Parent = main.tabControl1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("密码错误，请重新登录！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    //用户名不存在读取密码为空
                }
            }
            catch
            { }
        }

        private void btn_cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label_modifypassword_Click(object sender, EventArgs e)
        {
            try
            {
                main.MfPasswordForm_Show();     //显示修改密码窗体
                this.Close();                   //关闭登录窗体          
            }
            catch
            { }
        }






    }
}
