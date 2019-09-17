namespace fzjgld
{
    partial class ModifyPassword
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
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_changepassword = new System.Windows.Forms.Button();
            this.txt_verifypassword = new System.Windows.Forms.TextBox();
            this.txt_newpassword = new System.Windows.Forms.TextBox();
            this.txt_oldpassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comb_user = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Location = new System.Drawing.Point(144, 213);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(96, 29);
            this.btn_cancel.TabIndex = 19;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_changepassword
            // 
            this.btn_changepassword.Location = new System.Drawing.Point(39, 213);
            this.btn_changepassword.Name = "btn_changepassword";
            this.btn_changepassword.Size = new System.Drawing.Size(96, 29);
            this.btn_changepassword.TabIndex = 18;
            this.btn_changepassword.Text = "保存";
            this.btn_changepassword.UseVisualStyleBackColor = true;
            this.btn_changepassword.Click += new System.EventHandler(this.btn_changepassword_Click);
            // 
            // txt_verifypassword
            // 
            this.txt_verifypassword.Location = new System.Drawing.Point(172, 161);
            this.txt_verifypassword.Name = "txt_verifypassword";
            this.txt_verifypassword.Size = new System.Drawing.Size(121, 21);
            this.txt_verifypassword.TabIndex = 17;
            // 
            // txt_newpassword
            // 
            this.txt_newpassword.Location = new System.Drawing.Point(172, 117);
            this.txt_newpassword.Name = "txt_newpassword";
            this.txt_newpassword.Size = new System.Drawing.Size(121, 21);
            this.txt_newpassword.TabIndex = 16;
            // 
            // txt_oldpassword
            // 
            this.txt_oldpassword.Location = new System.Drawing.Point(172, 73);
            this.txt_oldpassword.Name = "txt_oldpassword";
            this.txt_oldpassword.Size = new System.Drawing.Size(121, 21);
            this.txt_oldpassword.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "确认新密码：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(88, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "新密码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "原密码：";
            // 
            // comb_user
            // 
            this.comb_user.FormattingEnabled = true;
            this.comb_user.Items.AddRange(new object[] {
            "生产人员",
            "研发人员"});
            this.comb_user.Location = new System.Drawing.Point(172, 29);
            this.comb_user.Name = "comb_user";
            this.comb_user.Size = new System.Drawing.Size(121, 20);
            this.comb_user.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "用户名：";
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(249, 213);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(96, 29);
            this.btn_Reset.TabIndex = 20;
            this.btn_Reset.Text = "恢复出厂设置";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // ModifyPassword
            // 
            this.AcceptButton = this.btn_changepassword;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.ClientSize = new System.Drawing.Size(392, 266);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_changepassword);
            this.Controls.Add(this.txt_verifypassword);
            this.Controls.Add(this.txt_newpassword);
            this.Controls.Add(this.txt_oldpassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comb_user);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "ModifyPassword";
            this.Text = "修改密码";
            this.Load += new System.EventHandler(this.ModifyPassword_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_changepassword;
        private System.Windows.Forms.TextBox txt_verifypassword;
        private System.Windows.Forms.TextBox txt_newpassword;
        private System.Windows.Forms.TextBox txt_oldpassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comb_user;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Reset;
    }
}