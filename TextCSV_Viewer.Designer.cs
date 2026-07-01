namespace FileProcessing
{
	partial class frmTextView
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbFileName = new System.Windows.Forms.TextBox();
			this.btBrowse = new System.Windows.Forms.Button();
			this.tabcMain = new System.Windows.Forms.TabControl();
			this.tabpText = new System.Windows.Forms.TabPage();
			this.rtbShow = new System.Windows.Forms.RichTextBox();
			this.btRead = new System.Windows.Forms.Button();
			this.tabpCSV = new System.Windows.Forms.TabPage();
			this.dgvData = new System.Windows.Forms.DataGridView();
			this.btReadCSV = new System.Windows.Forms.Button();
			this.RegisterDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SHA256_Hash = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MD5_Hash = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SHA1_Hash = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabcMain.SuspendLayout();
			this.tabpText.SuspendLayout();
			this.tabpCSV.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 16);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "File";
			// 
			// tbFileName
			// 
			this.tbFileName.Location = new System.Drawing.Point(128, 16);
			this.tbFileName.Margin = new System.Windows.Forms.Padding(4);
			this.tbFileName.Name = "tbFileName";
			this.tbFileName.Size = new System.Drawing.Size(1111, 29);
			this.tbFileName.TabIndex = 1;
			this.tbFileName.Text = "D:\\YutData\\DistributedHome\\cdti.Code\\FileProcessing\\data\\malware_500.csv";
			// 
			// btBrowse
			// 
			this.btBrowse.Location = new System.Drawing.Point(1287, 9);
			this.btBrowse.Margin = new System.Windows.Forms.Padding(4);
			this.btBrowse.Name = "btBrowse";
			this.btBrowse.Size = new System.Drawing.Size(138, 40);
			this.btBrowse.TabIndex = 2;
			this.btBrowse.Text = "Browse";
			this.btBrowse.UseVisualStyleBackColor = true;
			this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
			// 
			// tabcMain
			// 
			this.tabcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabcMain.Controls.Add(this.tabpText);
			this.tabcMain.Controls.Add(this.tabpCSV);
			this.tabcMain.Location = new System.Drawing.Point(15, 57);
			this.tabcMain.Margin = new System.Windows.Forms.Padding(4);
			this.tabcMain.Name = "tabcMain";
			this.tabcMain.SelectedIndex = 0;
			this.tabcMain.Size = new System.Drawing.Size(1452, 1333);
			this.tabcMain.TabIndex = 3;
			// 
			// tabpText
			// 
			this.tabpText.Controls.Add(this.rtbShow);
			this.tabpText.Controls.Add(this.btRead);
			this.tabpText.Location = new System.Drawing.Point(4, 33);
			this.tabpText.Margin = new System.Windows.Forms.Padding(4);
			this.tabpText.Name = "tabpText";
			this.tabpText.Padding = new System.Windows.Forms.Padding(4);
			this.tabpText.Size = new System.Drawing.Size(1444, 1296);
			this.tabpText.TabIndex = 0;
			this.tabpText.Text = "Text";
			this.tabpText.UseVisualStyleBackColor = true;
			// 
			// rtbShow
			// 
			this.rtbShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtbShow.Location = new System.Drawing.Point(21, 73);
			this.rtbShow.Margin = new System.Windows.Forms.Padding(4);
			this.rtbShow.Name = "rtbShow";
			this.rtbShow.Size = new System.Drawing.Size(1415, 1204);
			this.rtbShow.TabIndex = 1;
			this.rtbShow.Text = "";
			this.rtbShow.WordWrap = false;
			// 
			// btRead
			// 
			this.btRead.Location = new System.Drawing.Point(21, 19);
			this.btRead.Margin = new System.Windows.Forms.Padding(4);
			this.btRead.Name = "btRead";
			this.btRead.Size = new System.Drawing.Size(141, 46);
			this.btRead.TabIndex = 0;
			this.btRead.Text = "read as text file";
			this.btRead.UseVisualStyleBackColor = true;
			this.btRead.Click += new System.EventHandler(this.btRead_Click);
			// 
			// tabpCSV
			// 
			this.tabpCSV.Controls.Add(this.dgvData);
			this.tabpCSV.Controls.Add(this.btReadCSV);
			this.tabpCSV.Location = new System.Drawing.Point(4, 33);
			this.tabpCSV.Margin = new System.Windows.Forms.Padding(4);
			this.tabpCSV.Name = "tabpCSV";
			this.tabpCSV.Padding = new System.Windows.Forms.Padding(4);
			this.tabpCSV.Size = new System.Drawing.Size(1444, 1296);
			this.tabpCSV.TabIndex = 1;
			this.tabpCSV.Text = "CSV";
			this.tabpCSV.UseVisualStyleBackColor = true;
			// 
			// dgvData
			// 
			this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RegisterDate,
            this.SHA256_Hash,
            this.MD5_Hash,
            this.SHA1_Hash});
			this.dgvData.Location = new System.Drawing.Point(30, 68);
			this.dgvData.Margin = new System.Windows.Forms.Padding(4);
			this.dgvData.Name = "dgvData";
			this.dgvData.RowHeadersWidth = 62;
			this.dgvData.RowTemplate.Height = 28;
			this.dgvData.Size = new System.Drawing.Size(1385, 1204);
			this.dgvData.TabIndex = 1;
			// 
			// btReadCSV
			// 
			this.btReadCSV.Location = new System.Drawing.Point(28, 8);
			this.btReadCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btReadCSV.Name = "btReadCSV";
			this.btReadCSV.Size = new System.Drawing.Size(131, 43);
			this.btReadCSV.TabIndex = 0;
			this.btReadCSV.Text = "read as csv";
			this.btReadCSV.UseVisualStyleBackColor = true;
			this.btReadCSV.Click += new System.EventHandler(this.btReadCSV_Click);
			// 
			// RegisterDate
			// 
			this.RegisterDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.RegisterDate.HeaderText = "Date";
			this.RegisterDate.MinimumWidth = 8;
			this.RegisterDate.Name = "RegisterDate";
			this.RegisterDate.Width = 84;
			// 
			// SHA256_Hash
			// 
			this.SHA256_Hash.HeaderText = "SHA256 Hash";
			this.SHA256_Hash.MinimumWidth = 8;
			this.SHA256_Hash.Name = "SHA256_Hash";
			this.SHA256_Hash.Width = 150;
			// 
			// MD5_Hash
			// 
			this.MD5_Hash.HeaderText = "MD5Hash";
			this.MD5_Hash.MinimumWidth = 8;
			this.MD5_Hash.Name = "MD5_Hash";
			this.MD5_Hash.Width = 150;
			// 
			// SHA1_Hash
			// 
			this.SHA1_Hash.HeaderText = "SHA1 Hash";
			this.SHA1_Hash.MinimumWidth = 8;
			this.SHA1_Hash.Name = "SHA1_Hash";
			this.SHA1_Hash.Width = 150;
			// 
			// frmTextView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1481, 1403);
			this.Controls.Add(this.tabcMain);
			this.Controls.Add(this.btBrowse);
			this.Controls.Add(this.tbFileName);
			this.Controls.Add(this.label1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "frmTextView";
			this.Text = "Text/CSV viewer";
			this.tabcMain.ResumeLayout(false);
			this.tabpText.ResumeLayout(false);
			this.tabpCSV.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbFileName;
		private System.Windows.Forms.Button btBrowse;
		private System.Windows.Forms.TabControl tabcMain;
		private System.Windows.Forms.TabPage tabpText;
		private System.Windows.Forms.TabPage tabpCSV;
		private System.Windows.Forms.Button btRead;
		private System.Windows.Forms.RichTextBox rtbShow;
		private System.Windows.Forms.DataGridView dgvData;
		private System.Windows.Forms.Button btReadCSV;
		private System.Windows.Forms.DataGridViewTextBoxColumn RegisterDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn SHA256_Hash;
		private System.Windows.Forms.DataGridViewTextBoxColumn MD5_Hash;
		private System.Windows.Forms.DataGridViewTextBoxColumn SHA1_Hash;
	}
}

