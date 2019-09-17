using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OleDb;
using System.Windows.Forms;

namespace fzjgld
{
    class accessoledb
    {
        #region 打开数据库操作
        public static OleDbConnection Open_Conn(string ConnStr)
        {
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            Conn.Open();
            return Conn;
        }
        #endregion

        #region 读取用户名密码
        public static string ReadPassword(string mdbname,string username)
        {
            string mdbpassword = "";
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
                mdbpassword = null;
            }
            return mdbpassword;
        }

        #endregion


    }
}
