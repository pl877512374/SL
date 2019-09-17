namespace fzjgld
{
    partial class ScanDataSet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox_continuesend = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btn_continue = new System.Windows.Forms.Button();
            this.btn_single = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label_End = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label_start = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ZEROX = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txt_MAXDISTANCE1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_INTERVAL = new System.Windows.Forms.TextBox();
            this.txt_ONECHART = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_DeleteAllData = new System.Windows.Forms.Button();
            this.btn_record_disper = new System.Windows.Forms.Button();
            this.txt_PROBABILITY = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_DISPERSION = new System.Windows.Forms.TextBox();
            this.txt_VALREF = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_kaiqi = new System.Windows.Forms.CheckBox();
            this.checkBox_xiuzheng = new System.Windows.Forms.CheckBox();
            this.btnrecord = new System.Windows.Forms.Button();
            this.btnsave = new System.Windows.Forms.Button();
            this.btndelete = new System.Windows.Forms.Button();
            this.txt_AVERAGEDISTANCE = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_MINDISTANCE = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_MAXDISTANCE = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_RELDISC = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_AngleNum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_measureinterval = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ARMPkgs = new System.Windows.Forms.TextBox();
            this.btnCLEAR = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.txt_DRAWNUM = new System.Windows.Forms.TextBox();
            this.btn_SAVEDATA = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.txt_SAVENUM = new System.Windows.Forms.TextBox();
            this.btnCLEARCNT = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txt_RECVNUM = new System.Windows.Forms.TextBox();
            this.txt_SIGDATACMD = new System.Windows.Forms.TextBox();
            this.btn_startShow = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.txt_jump = new System.Windows.Forms.TextBox();
            this.btndrawhistory = new System.Windows.Forms.Button();
            this.btn_jump = new System.Windows.Forms.Button();
            this.btnnext = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.txtnowframe = new System.Windows.Forms.TextBox();
            this.btnprevious = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txt_FPGAPkgs = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox_continuesend);
            this.groupBox6.Controls.Add(this.richTextBox1);
            this.groupBox6.Controls.Add(this.btn_continue);
            this.groupBox6.Controls.Add(this.btn_single);
            this.groupBox6.Location = new System.Drawing.Point(3, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(252, 436);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "数据显示";
            // 
            // checkBox_continuesend
            // 
            this.checkBox_continuesend.AutoSize = true;
            this.checkBox_continuesend.Location = new System.Drawing.Point(36, 58);
            this.checkBox_continuesend.Name = "checkBox_continuesend";
            this.checkBox_continuesend.Size = new System.Drawing.Size(192, 16);
            this.checkBox_continuesend.TabIndex = 17;
            this.checkBox_continuesend.Text = "定时20ms连续发送单帧获取指令";
            this.checkBox_continuesend.UseVisualStyleBackColor = true;
            this.checkBox_continuesend.Click += new System.EventHandler(this.checkBox_continuesend_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.EnableAutoDragDrop = true;
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.Location = new System.Drawing.Point(16, 81);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(219, 337);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // btn_continue
            // 
            this.btn_continue.Location = new System.Drawing.Point(141, 24);
            this.btn_continue.Name = "btn_continue";
            this.btn_continue.Size = new System.Drawing.Size(75, 23);
            this.btn_continue.TabIndex = 1;
            this.btn_continue.Text = "连续获取";
            this.btn_continue.UseVisualStyleBackColor = true;
            this.btn_continue.Click += new System.EventHandler(this.btn_continue_Click);
            // 
            // btn_single
            // 
            this.btn_single.Location = new System.Drawing.Point(36, 24);
            this.btn_single.Name = "btn_single";
            this.btn_single.Size = new System.Drawing.Size(75, 23);
            this.btn_single.TabIndex = 0;
            this.btn_single.Text = "单帧获取";
            this.btn_single.UseVisualStyleBackColor = true;
            this.btn_single.Click += new System.EventHandler(this.btn_single_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label_End);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.label_start);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txt_ZEROX);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.txt_MAXDISTANCE1);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.txt_INTERVAL);
            this.groupBox4.Controls.Add(this.txt_ONECHART);
            this.groupBox4.Location = new System.Drawing.Point(261, 173);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(559, 87);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "显示选项";
            // 
            // label_End
            // 
            this.label_End.AutoSize = true;
            this.label_End.Location = new System.Drawing.Point(389, 56);
            this.label_End.Name = "label_End";
            this.label_End.Size = new System.Drawing.Size(11, 12);
            this.label_End.TabIndex = 32;
            this.label_End.Text = "0";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(342, 56);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(29, 12);
            this.label22.TabIndex = 31;
            this.label22.Text = "End:";
            // 
            // label_start
            // 
            this.label_start.AutoSize = true;
            this.label_start.Location = new System.Drawing.Point(389, 26);
            this.label_start.Name = "label_start";
            this.label_start.Size = new System.Drawing.Size(11, 12);
            this.label_start.TabIndex = 30;
            this.label_start.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(342, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 29;
            this.label2.Text = "Start:";
            // 
            // txt_ZEROX
            // 
            this.txt_ZEROX.Location = new System.Drawing.Point(257, 53);
            this.txt_ZEROX.Name = "txt_ZEROX";
            this.txt_ZEROX.Size = new System.Drawing.Size(75, 21);
            this.txt_ZEROX.TabIndex = 28;
            this.txt_ZEROX.Text = "45";
            this.txt_ZEROX.TextChanged += new System.EventHandler(this.txt_ZEROX_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(174, 56);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 27;
            this.label14.Text = "零点横坐标：";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 56);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 25;
            this.label15.Text = "最大测距：";
            // 
            // txt_MAXDISTANCE1
            // 
            this.txt_MAXDISTANCE1.Location = new System.Drawing.Point(79, 53);
            this.txt_MAXDISTANCE1.Name = "txt_MAXDISTANCE1";
            this.txt_MAXDISTANCE1.Size = new System.Drawing.Size(75, 21);
            this.txt_MAXDISTANCE1.TabIndex = 26;
            this.txt_MAXDISTANCE1.Text = "60000";
            this.txt_MAXDISTANCE1.TextChanged += new System.EventHandler(this.txt_MAXDISTANCE1_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(174, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 23;
            this.label12.Text = "屏显包数：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 26);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 21;
            this.label13.Text = "间隔包数：";
            // 
            // txt_INTERVAL
            // 
            this.txt_INTERVAL.Location = new System.Drawing.Point(79, 23);
            this.txt_INTERVAL.Name = "txt_INTERVAL";
            this.txt_INTERVAL.Size = new System.Drawing.Size(75, 21);
            this.txt_INTERVAL.TabIndex = 22;
            this.txt_INTERVAL.Text = "0";
            this.txt_INTERVAL.TextChanged += new System.EventHandler(this.txt_INTERVAL_TextChanged);
            // 
            // txt_ONECHART
            // 
            this.txt_ONECHART.Location = new System.Drawing.Point(257, 23);
            this.txt_ONECHART.Name = "txt_ONECHART";
            this.txt_ONECHART.Size = new System.Drawing.Size(75, 21);
            this.txt_ONECHART.TabIndex = 24;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_DeleteAllData);
            this.groupBox3.Controls.Add(this.btn_record_disper);
            this.groupBox3.Controls.Add(this.txt_PROBABILITY);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.txt_DISPERSION);
            this.groupBox3.Controls.Add(this.txt_VALREF);
            this.groupBox3.Location = new System.Drawing.Point(261, 101);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(559, 87);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "离散度概率";
            // 
            // btn_DeleteAllData
            // 
            this.btn_DeleteAllData.Location = new System.Drawing.Point(486, 25);
            this.btn_DeleteAllData.Name = "btn_DeleteAllData";
            this.btn_DeleteAllData.Size = new System.Drawing.Size(64, 23);
            this.btn_DeleteAllData.TabIndex = 21;
            this.btn_DeleteAllData.Text = "清除记录";
            this.btn_DeleteAllData.UseVisualStyleBackColor = true;
            this.btn_DeleteAllData.Click += new System.EventHandler(this.btn_DeleteAllData_Click);
            // 
            // btn_record_disper
            // 
            this.btn_record_disper.Enabled = false;
            this.btn_record_disper.Location = new System.Drawing.Point(416, 25);
            this.btn_record_disper.Name = "btn_record_disper";
            this.btn_record_disper.Size = new System.Drawing.Size(64, 23);
            this.btn_record_disper.TabIndex = 17;
            this.btn_record_disper.Text = "保存";
            this.btn_record_disper.UseVisualStyleBackColor = true;
            // 
            // txt_PROBABILITY
            // 
            this.txt_PROBABILITY.Enabled = false;
            this.txt_PROBABILITY.Location = new System.Drawing.Point(335, 27);
            this.txt_PROBABILITY.Name = "txt_PROBABILITY";
            this.txt_PROBABILITY.ReadOnly = true;
            this.txt_PROBABILITY.Size = new System.Drawing.Size(75, 21);
            this.txt_PROBABILITY.TabIndex = 20;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(145, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "离散度：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(296, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 19;
            this.label9.Text = "概率：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 15;
            this.label11.Text = "参考值：";
            // 
            // txt_DISPERSION
            // 
            this.txt_DISPERSION.Location = new System.Drawing.Point(200, 27);
            this.txt_DISPERSION.Name = "txt_DISPERSION";
            this.txt_DISPERSION.Size = new System.Drawing.Size(75, 21);
            this.txt_DISPERSION.TabIndex = 18;
            this.txt_DISPERSION.Text = "15";
            this.txt_DISPERSION.TextChanged += new System.EventHandler(this.txt_DISPERSION_TextChanged);
            this.txt_DISPERSION.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DISPERSION_KeyDown);
            // 
            // txt_VALREF
            // 
            this.txt_VALREF.Enabled = false;
            this.txt_VALREF.Location = new System.Drawing.Point(65, 27);
            this.txt_VALREF.Name = "txt_VALREF";
            this.txt_VALREF.ReadOnly = true;
            this.txt_VALREF.Size = new System.Drawing.Size(75, 21);
            this.txt_VALREF.TabIndex = 16;
            this.txt_VALREF.Text = "0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_kaiqi);
            this.groupBox2.Controls.Add(this.checkBox_xiuzheng);
            this.groupBox2.Controls.Add(this.btnrecord);
            this.groupBox2.Controls.Add(this.btnsave);
            this.groupBox2.Controls.Add(this.btndelete);
            this.groupBox2.Controls.Add(this.txt_AVERAGEDISTANCE);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txt_MINDISTANCE);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txt_MAXDISTANCE);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txt_RELDISC);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txt_AngleNum);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txt_measureinterval);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(261, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(559, 87);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "测量选项";
            // 
            // checkBox_kaiqi
            // 
            this.checkBox_kaiqi.AutoSize = true;
            this.checkBox_kaiqi.Location = new System.Drawing.Point(14, 38);
            this.checkBox_kaiqi.Name = "checkBox_kaiqi";
            this.checkBox_kaiqi.Size = new System.Drawing.Size(48, 16);
            this.checkBox_kaiqi.TabIndex = 16;
            this.checkBox_kaiqi.Text = "开启";
            this.checkBox_kaiqi.UseVisualStyleBackColor = true;
            // 
            // checkBox_xiuzheng
            // 
            this.checkBox_xiuzheng.AutoSize = true;
            this.checkBox_xiuzheng.Location = new System.Drawing.Point(14, 16);
            this.checkBox_xiuzheng.Name = "checkBox_xiuzheng";
            this.checkBox_xiuzheng.Size = new System.Drawing.Size(48, 16);
            this.checkBox_xiuzheng.TabIndex = 15;
            this.checkBox_xiuzheng.Text = "修正";
            this.checkBox_xiuzheng.UseVisualStyleBackColor = true;
            // 
            // btnrecord
            // 
            this.btnrecord.Location = new System.Drawing.Point(486, 15);
            this.btnrecord.Name = "btnrecord";
            this.btnrecord.Size = new System.Drawing.Size(64, 23);
            this.btnrecord.TabIndex = 14;
            this.btnrecord.Text = "记录";
            this.btnrecord.UseVisualStyleBackColor = true;
            this.btnrecord.Click += new System.EventHandler(this.btnrecord_Click);
            // 
            // btnsave
            // 
            this.btnsave.Location = new System.Drawing.Point(416, 54);
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(64, 23);
            this.btnsave.TabIndex = 13;
            this.btnsave.Text = "保存";
            this.btnsave.UseVisualStyleBackColor = true;
            this.btnsave.Click += new System.EventHandler(this.btnsave_Click);
            // 
            // btndelete
            // 
            this.btndelete.Location = new System.Drawing.Point(486, 54);
            this.btndelete.Name = "btndelete";
            this.btndelete.Size = new System.Drawing.Size(64, 23);
            this.btndelete.TabIndex = 12;
            this.btndelete.Text = "删除";
            this.btndelete.UseVisualStyleBackColor = true;
            this.btndelete.Click += new System.EventHandler(this.btndelete_Click);
            // 
            // txt_AVERAGEDISTANCE
            // 
            this.txt_AVERAGEDISTANCE.Enabled = false;
            this.txt_AVERAGEDISTANCE.Location = new System.Drawing.Point(335, 56);
            this.txt_AVERAGEDISTANCE.Name = "txt_AVERAGEDISTANCE";
            this.txt_AVERAGEDISTANCE.ReadOnly = true;
            this.txt_AVERAGEDISTANCE.Size = new System.Drawing.Size(75, 21);
            this.txt_AVERAGEDISTANCE.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(278, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "平均值：";
            // 
            // txt_MINDISTANCE
            // 
            this.txt_MINDISTANCE.Enabled = false;
            this.txt_MINDISTANCE.Location = new System.Drawing.Point(200, 56);
            this.txt_MINDISTANCE.Name = "txt_MINDISTANCE";
            this.txt_MINDISTANCE.ReadOnly = true;
            this.txt_MINDISTANCE.Size = new System.Drawing.Size(75, 21);
            this.txt_MINDISTANCE.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(145, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "最小值：";
            // 
            // txt_MAXDISTANCE
            // 
            this.txt_MAXDISTANCE.Enabled = false;
            this.txt_MAXDISTANCE.Location = new System.Drawing.Point(65, 56);
            this.txt_MAXDISTANCE.Name = "txt_MAXDISTANCE";
            this.txt_MAXDISTANCE.ReadOnly = true;
            this.txt_MAXDISTANCE.Size = new System.Drawing.Size(75, 21);
            this.txt_MAXDISTANCE.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "最大值：";
            // 
            // txt_RELDISC
            // 
            this.txt_RELDISC.Location = new System.Drawing.Point(405, 15);
            this.txt_RELDISC.Name = "txt_RELDISC";
            this.txt_RELDISC.Size = new System.Drawing.Size(75, 21);
            this.txt_RELDISC.TabIndex = 5;
            this.txt_RELDISC.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(336, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "实际距离：";
            // 
            // txt_AngleNum
            // 
            this.txt_AngleNum.Location = new System.Drawing.Point(255, 15);
            this.txt_AngleNum.Name = "txt_AngleNum";
            this.txt_AngleNum.Size = new System.Drawing.Size(75, 21);
            this.txt_AngleNum.TabIndex = 3;
            this.txt_AngleNum.Text = "135";
            this.txt_AngleNum.TextChanged += new System.EventHandler(this.txt_AngleNum_TextChanged);
            this.txt_AngleNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_AngleNum_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(197, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "测量点：";
            // 
            // txt_measureinterval
            // 
            this.txt_measureinterval.Location = new System.Drawing.Point(115, 15);
            this.txt_measureinterval.Name = "txt_measureinterval";
            this.txt_measureinterval.Size = new System.Drawing.Size(75, 21);
            this.txt_measureinterval.TabIndex = 1;
            this.txt_measureinterval.Text = "100";
            this.txt_measureinterval.TextChanged += new System.EventHandler(this.txt_measureinterval_TextChanged);
            this.txt_measureinterval.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_measureinterval_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "间隔：";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txt_ARMPkgs);
            this.groupBox5.Controls.Add(this.btnCLEAR);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.txt_DRAWNUM);
            this.groupBox5.Controls.Add(this.btn_SAVEDATA);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.txt_SAVENUM);
            this.groupBox5.Controls.Add(this.btnCLEARCNT);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.txt_RECVNUM);
            this.groupBox5.Controls.Add(this.txt_SIGDATACMD);
            this.groupBox5.Location = new System.Drawing.Point(261, 263);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(559, 87);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "统计数据";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(394, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 37;
            this.label1.Text = "ARM包数：";
            // 
            // txt_ARMPkgs
            // 
            this.txt_ARMPkgs.Location = new System.Drawing.Point(465, 56);
            this.txt_ARMPkgs.Name = "txt_ARMPkgs";
            this.txt_ARMPkgs.ReadOnly = true;
            this.txt_ARMPkgs.Size = new System.Drawing.Size(75, 21);
            this.txt_ARMPkgs.TabIndex = 38;
            // 
            // btnCLEAR
            // 
            this.btnCLEAR.Location = new System.Drawing.Point(308, 54);
            this.btnCLEAR.Name = "btnCLEAR";
            this.btnCLEAR.Size = new System.Drawing.Size(75, 23);
            this.btnCLEAR.TabIndex = 36;
            this.btnCLEAR.Text = "停止计数";
            this.btnCLEAR.UseVisualStyleBackColor = true;
            this.btnCLEAR.Click += new System.EventHandler(this.btnCLEAR_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(394, 26);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(65, 12);
            this.label19.TabIndex = 30;
            this.label19.Text = "绘画包数：";
            // 
            // txt_DRAWNUM
            // 
            this.txt_DRAWNUM.Location = new System.Drawing.Point(465, 23);
            this.txt_DRAWNUM.Name = "txt_DRAWNUM";
            this.txt_DRAWNUM.ReadOnly = true;
            this.txt_DRAWNUM.Size = new System.Drawing.Size(75, 21);
            this.txt_DRAWNUM.TabIndex = 31;
            // 
            // btn_SAVEDATA
            // 
            this.btn_SAVEDATA.Location = new System.Drawing.Point(224, 54);
            this.btn_SAVEDATA.Name = "btn_SAVEDATA";
            this.btn_SAVEDATA.Size = new System.Drawing.Size(75, 23);
            this.btn_SAVEDATA.TabIndex = 35;
            this.btn_SAVEDATA.Text = "开始保存";
            this.btn_SAVEDATA.UseVisualStyleBackColor = true;
            this.btn_SAVEDATA.Click += new System.EventHandler(this.btnSAVEDATA_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(223, 26);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(65, 12);
            this.label18.TabIndex = 30;
            this.label18.Text = "保存包数：";
            // 
            // txt_SAVENUM
            // 
            this.txt_SAVENUM.Enabled = false;
            this.txt_SAVENUM.Location = new System.Drawing.Point(294, 23);
            this.txt_SAVENUM.Name = "txt_SAVENUM";
            this.txt_SAVENUM.ReadOnly = true;
            this.txt_SAVENUM.Size = new System.Drawing.Size(75, 21);
            this.txt_SAVENUM.TabIndex = 31;
            // 
            // btnCLEARCNT
            // 
            this.btnCLEARCNT.Location = new System.Drawing.Point(169, 23);
            this.btnCLEARCNT.Name = "btnCLEARCNT";
            this.btnCLEARCNT.Size = new System.Drawing.Size(43, 52);
            this.btnCLEARCNT.TabIndex = 34;
            this.btnCLEARCNT.Text = "清除计数";
            this.btnCLEARCNT.UseVisualStyleBackColor = true;
            this.btnCLEARCNT.Click += new System.EventHandler(this.btnCLEARCNT_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 56);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 32;
            this.label16.Text = "接收包数：";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 26);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(77, 12);
            this.label17.TabIndex = 30;
            this.label17.Text = "单帧命令数：";
            // 
            // txt_RECVNUM
            // 
            this.txt_RECVNUM.Enabled = false;
            this.txt_RECVNUM.Location = new System.Drawing.Point(85, 54);
            this.txt_RECVNUM.Name = "txt_RECVNUM";
            this.txt_RECVNUM.ReadOnly = true;
            this.txt_RECVNUM.Size = new System.Drawing.Size(75, 21);
            this.txt_RECVNUM.TabIndex = 33;
            // 
            // txt_SIGDATACMD
            // 
            this.txt_SIGDATACMD.Enabled = false;
            this.txt_SIGDATACMD.Location = new System.Drawing.Point(85, 23);
            this.txt_SIGDATACMD.Name = "txt_SIGDATACMD";
            this.txt_SIGDATACMD.ReadOnly = true;
            this.txt_SIGDATACMD.Size = new System.Drawing.Size(75, 21);
            this.txt_SIGDATACMD.TabIndex = 31;
            // 
            // btn_startShow
            // 
            this.btn_startShow.Location = new System.Drawing.Point(233, 57);
            this.btn_startShow.Name = "btn_startShow";
            this.btn_startShow.Size = new System.Drawing.Size(75, 23);
            this.btn_startShow.TabIndex = 43;
            this.btn_startShow.Text = "播放";
            this.btn_startShow.UseVisualStyleBackColor = true;
            this.btn_startShow.Click += new System.EventHandler(this.btn_startShow_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(466, 57);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 42;
            this.button5.Text = "暂停";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            // 
            // txt_jump
            // 
            this.txt_jump.Location = new System.Drawing.Point(315, 23);
            this.txt_jump.Name = "txt_jump";
            this.txt_jump.Size = new System.Drawing.Size(67, 21);
            this.txt_jump.TabIndex = 41;
            this.txt_jump.Text = "0";
            // 
            // btndrawhistory
            // 
            this.btndrawhistory.Location = new System.Drawing.Point(73, 21);
            this.btndrawhistory.Name = "btndrawhistory";
            this.btndrawhistory.Size = new System.Drawing.Size(75, 23);
            this.btndrawhistory.TabIndex = 37;
            this.btndrawhistory.Text = "选择";
            this.btndrawhistory.UseVisualStyleBackColor = true;
            this.btndrawhistory.Click += new System.EventHandler(this.btndrawhistory_Click);
            // 
            // btn_jump
            // 
            this.btn_jump.Location = new System.Drawing.Point(333, 57);
            this.btn_jump.Name = "btn_jump";
            this.btn_jump.Size = new System.Drawing.Size(75, 23);
            this.btn_jump.TabIndex = 40;
            this.btn_jump.Text = "跳转";
            this.btn_jump.UseVisualStyleBackColor = true;
            this.btn_jump.Click += new System.EventHandler(this.btn_jump_Click);
            // 
            // btnnext
            // 
            this.btnnext.Location = new System.Drawing.Point(117, 57);
            this.btnnext.Name = "btnnext";
            this.btnnext.Size = new System.Drawing.Size(74, 23);
            this.btnnext.TabIndex = 38;
            this.btnnext.Text = ">>";
            this.btnnext.UseVisualStyleBackColor = true;
            this.btnnext.Click += new System.EventHandler(this.btnnext_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.Location = new System.Drawing.Point(266, 26);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(42, 14);
            this.label20.TabIndex = 39;
            this.label20.Text = "帧数:";
            // 
            // txtnowframe
            // 
            this.txtnowframe.Enabled = false;
            this.txtnowframe.Location = new System.Drawing.Point(172, 23);
            this.txtnowframe.Name = "txtnowframe";
            this.txtnowframe.ReadOnly = true;
            this.txtnowframe.Size = new System.Drawing.Size(67, 21);
            this.txtnowframe.TabIndex = 38;
            this.txtnowframe.Visible = false;
            // 
            // btnprevious
            // 
            this.btnprevious.Location = new System.Drawing.Point(26, 57);
            this.btnprevious.Name = "btnprevious";
            this.btnprevious.Size = new System.Drawing.Size(75, 23);
            this.btnprevious.TabIndex = 38;
            this.btnprevious.Text = "<<";
            this.btnprevious.UseVisualStyleBackColor = true;
            this.btnprevious.Click += new System.EventHandler(this.btnprevious_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.txt_jump);
            this.groupBox1.Controls.Add(this.txt_FPGAPkgs);
            this.groupBox1.Controls.Add(this.btn_startShow);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.btnprevious);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.btnnext);
            this.groupBox1.Controls.Add(this.txtnowframe);
            this.groupBox1.Controls.Add(this.btn_jump);
            this.groupBox1.Controls.Add(this.btndrawhistory);
            this.groupBox1.Location = new System.Drawing.Point(261, 359);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(559, 87);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "读取数据";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(404, 27);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(53, 12);
            this.label21.TabIndex = 39;
            this.label21.Text = "反射率：";
            // 
            // txt_FPGAPkgs
            // 
            this.txt_FPGAPkgs.Location = new System.Drawing.Point(465, 23);
            this.txt_FPGAPkgs.Name = "txt_FPGAPkgs";
            this.txt_FPGAPkgs.ReadOnly = true;
            this.txt_FPGAPkgs.Size = new System.Drawing.Size(75, 21);
            this.txt_FPGAPkgs.TabIndex = 40;
            // 
            // ScanDataSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 460);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ScanDataSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "扫描接收数据设置";
            this.Load += new System.EventHandler(this.ScanDataSet_Load);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btn_continue;
        private System.Windows.Forms.Button btn_single;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txt_ZEROX;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_MAXDISTANCE1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_INTERVAL;
        private System.Windows.Forms.TextBox txt_ONECHART;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txt_PROBABILITY;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_DISPERSION;
        private System.Windows.Forms.TextBox txt_VALREF;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnrecord;
        private System.Windows.Forms.Button btnsave;
        private System.Windows.Forms.Button btndelete;
        private System.Windows.Forms.TextBox txt_AVERAGEDISTANCE;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_MINDISTANCE;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_MAXDISTANCE;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_RELDISC;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_AngleNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_measureinterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_record_disper;
        private System.Windows.Forms.CheckBox checkBox_kaiqi;
        private System.Windows.Forms.CheckBox checkBox_xiuzheng;
        private System.Windows.Forms.CheckBox checkBox_continuesend;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btn_startShow;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox txt_jump;
        private System.Windows.Forms.Button btndrawhistory;
        private System.Windows.Forms.Button btn_jump;
        private System.Windows.Forms.Button btnCLEAR;
        private System.Windows.Forms.Button btnnext;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtnowframe;
        private System.Windows.Forms.TextBox txt_DRAWNUM;
        private System.Windows.Forms.Button btn_SAVEDATA;
        private System.Windows.Forms.Button btnprevious;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txt_SAVENUM;
        private System.Windows.Forms.Button btnCLEARCNT;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt_RECVNUM;
        private System.Windows.Forms.TextBox txt_SIGDATACMD;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_DeleteAllData;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_ARMPkgs;
        private System.Windows.Forms.Label label_End;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label_start;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txt_FPGAPkgs;

    }
}