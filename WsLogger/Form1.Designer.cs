namespace WsLogger
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_Connect = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.btn_SelectFile = new System.Windows.Forms.Button();
            this.txt_FileName = new System.Windows.Forms.TextBox();
            this.tv_DataItems = new System.Windows.Forms.TreeView();
            this.btn_StartLogging = new System.Windows.Forms.Button();
            this.btn_StopLogging = new System.Windows.Forms.Button();
            this.cb_IP = new System.Windows.Forms.ComboBox();
            this.lbl_MessagesReceived = new System.Windows.Forms.Label();
            this.tb_Search = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tb_UpdateRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Connect
            // 
            this.btn_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Connect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btn_Connect.Location = new System.Drawing.Point(393, 48);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(163, 28);
            this.btn_Connect.TabIndex = 1;
            this.btn_Connect.Text = "Connect";
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Disconnect.Enabled = false;
            this.btn_Disconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btn_Disconnect.Location = new System.Drawing.Point(562, 48);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(163, 28);
            this.btn_Disconnect.TabIndex = 2;
            this.btn_Disconnect.Text = "Disconnect";
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // btn_SelectFile
            // 
            this.btn_SelectFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SelectFile.Location = new System.Drawing.Point(12, 12);
            this.btn_SelectFile.Name = "btn_SelectFile";
            this.btn_SelectFile.Size = new System.Drawing.Size(110, 28);
            this.btn_SelectFile.TabIndex = 3;
            this.btn_SelectFile.Text = "Select File";
            this.btn_SelectFile.Click += new System.EventHandler(this.btn_SelectFile_Click);
            // 
            // txt_FileName
            // 
            this.txt_FileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_FileName.Location = new System.Drawing.Point(130, 17);
            this.txt_FileName.Name = "txt_FileName";
            this.txt_FileName.Size = new System.Drawing.Size(470, 20);
            this.txt_FileName.TabIndex = 4;
            // 
            // tv_DataItems
            // 
            this.tv_DataItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tv_DataItems.CheckBoxes = true;
            this.tv_DataItems.Location = new System.Drawing.Point(12, 150);
            this.tv_DataItems.Name = "tv_DataItems";
            this.tv_DataItems.Size = new System.Drawing.Size(713, 343);
            this.tv_DataItems.TabIndex = 5;
            this.tv_DataItems.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tv_DataItems_NodeMouseClick);
            this.tv_DataItems.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tv_DataItems_NodeMouseClick);
            // 
            // btn_StartLogging
            // 
            this.btn_StartLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_StartLogging.Enabled = false;
            this.btn_StartLogging.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btn_StartLogging.Location = new System.Drawing.Point(393, 100);
            this.btn_StartLogging.Name = "btn_StartLogging";
            this.btn_StartLogging.Size = new System.Drawing.Size(191, 28);
            this.btn_StartLogging.TabIndex = 7;
            this.btn_StartLogging.Text = "Save and Start Logging";
            this.btn_StartLogging.Click += new System.EventHandler(this.btn_StartLogging_Click);
            // 
            // btn_StopLogging
            // 
            this.btn_StopLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_StopLogging.Enabled = false;
            this.btn_StopLogging.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btn_StopLogging.Location = new System.Drawing.Point(590, 100);
            this.btn_StopLogging.Name = "btn_StopLogging";
            this.btn_StopLogging.Size = new System.Drawing.Size(135, 28);
            this.btn_StopLogging.TabIndex = 8;
            this.btn_StopLogging.Text = "Stop Logging";
            this.btn_StopLogging.Click += new System.EventHandler(this.btn_StopLogging_Click);
            // 
            // cb_IP
            // 
            this.cb_IP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_IP.Location = new System.Drawing.Point(12, 53);
            this.cb_IP.Name = "cb_IP";
            this.cb_IP.Size = new System.Drawing.Size(372, 21);
            this.cb_IP.TabIndex = 9;
            // 
            // lbl_MessagesReceived
            // 
            this.lbl_MessagesReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_MessagesReceived.AutoSize = true;
            this.lbl_MessagesReceived.Location = new System.Drawing.Point(12, 487);
            this.lbl_MessagesReceived.Name = "lbl_MessagesReceived";
            this.lbl_MessagesReceived.Size = new System.Drawing.Size(0, 13);
            this.lbl_MessagesReceived.TabIndex = 10;
            // 
            // tb_Search
            // 
            this.tb_Search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_Search.Location = new System.Drawing.Point(65, 106);
            this.tb_Search.Name = "tb_Search";
            this.tb_Search.Size = new System.Drawing.Size(322, 20);
            this.tb_Search.TabIndex = 12;
            this.tb_Search.TextChanged += new System.EventHandler(this.btn_Search_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Filter:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::WsLogger.Properties.Resources.B_G_Logo;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(606, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(119, 30);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // tb_UpdateRate
            // 
            this.tb_UpdateRate.Location = new System.Drawing.Point(96, 80);
            this.tb_UpdateRate.Name = "tb_UpdateRate";
            this.tb_UpdateRate.Size = new System.Drawing.Size(100, 20);
            this.tb_UpdateRate.TabIndex = 15;
            this.tb_UpdateRate.Text = "Max";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(12, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 16);
            this.label2.TabIndex = 16;
            this.label2.Text = "Freq (ms):";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 505);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_UpdateRate);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_Search);
            this.Controls.Add(this.lbl_MessagesReceived);
            this.Controls.Add(this.cb_IP);
            this.Controls.Add(this.btn_StopLogging);
            this.Controls.Add(this.btn_StartLogging);
            this.Controls.Add(this.tv_DataItems);
            this.Controls.Add(this.txt_FileName);
            this.Controls.Add(this.btn_SelectFile);
            this.Controls.Add(this.btn_Disconnect);
            this.Controls.Add(this.btn_Connect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "B&G Data Logger";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.Button btn_SelectFile;
        private System.Windows.Forms.TextBox txt_FileName;
        private System.Windows.Forms.TreeView tv_DataItems;
        private System.Windows.Forms.Button btn_StartLogging;
        private System.Windows.Forms.Button btn_StopLogging;
        private System.Windows.Forms.ComboBox cb_IP;
        private System.Windows.Forms.Label lbl_MessagesReceived;
        private System.Windows.Forms.TextBox tb_Search;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox tb_UpdateRate;
        private System.Windows.Forms.Label label2;
    }
}

