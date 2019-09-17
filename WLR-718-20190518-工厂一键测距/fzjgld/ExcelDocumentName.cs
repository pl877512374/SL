using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace fzjgld
{
    public partial class ExcelDocumentName : Form
    {
        ScanDataSet SetForm;
        SI2DCFG Single2DForm;
        string path = Environment.CurrentDirectory.ToString() + @"\Excel\" + "\\";
        public string name = null;
        public ExcelDocumentName()
        {
            InitializeComponent();
        }

        private void ExcelDocumentName_Load(object sender, EventArgs e)
        {
            SetForm = new ScanDataSet();
            Single2DForm = new SI2DCFG();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != null)
                {
                    path = Environment.CurrentDirectory.ToString() + @"\Excel\" + "\\";
                    name = textBox1.Text;
                    path = path + name + ".xlsx";
                    if (File.Exists(path) == false)
                    {
                        //button1.Enabled = true;
                        SetForm.ExcelName = textBox1.Text;
                        Single2DForm.ExcelName = textBox1.Text;
                        this.Close();
                    }

                    else
                    {
                        //button1.Enabled = false;
                        name = null;
                        path = Environment.CurrentDirectory.ToString() + @"\Excel\" + "\\";
                        MessageBox.Show("文件名已存在，请重新输入", "Error");
                    }

                }
                else
                    MessageBox.Show("文件名不能为空，请重新输入");
            }
            catch
            {
                return;
            }

        }
    }
}
