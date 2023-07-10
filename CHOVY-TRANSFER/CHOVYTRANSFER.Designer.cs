namespace CHOVY_TRANSFER
{
    partial class CHOVYTRANSFER
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CHOVYTRANSFER));
            this.driveLetterSrc = new System.Windows.Forms.ComboBox();
            this.pspFolder = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmaDir = new System.Windows.Forms.TextBox();
            this.driveLetterDst = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pspGames = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.transVita = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressStatus = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dexToggle = new System.Windows.Forms.PictureBox();
            this.currentFile = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dexToggle)).BeginInit();
            this.SuspendLayout();
            // 
            // driveLetterSrc
            // 
            this.driveLetterSrc.BackColor = System.Drawing.Color.DimGray;
            this.driveLetterSrc.DisplayMember = "1";
            this.driveLetterSrc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveLetterSrc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.driveLetterSrc.ForeColor = System.Drawing.Color.Lime;
            this.driveLetterSrc.FormattingEnabled = true;
            this.driveLetterSrc.Items.AddRange(new object[] {
            "A:\\",
            "B:\\",
            "C:\\",
            "D:\\",
            "E:\\",
            "F:\\",
            "G:\\",
            "H:\\",
            "I:\\",
            "J:\\",
            "K:\\",
            "L:\\",
            "M:\\",
            "N:\\",
            "O:\\",
            "P:\\",
            "Q:\\",
            "R:\\",
            "S:\\",
            "T:\\",
            "U:\\",
            "V:\\",
            "W:\\",
            "X:\\",
            "Y:\\",
            "Z:\\"});
            this.driveLetterSrc.Location = new System.Drawing.Point(74, 12);
            this.driveLetterSrc.Name = "driveLetterSrc";
            this.driveLetterSrc.Size = new System.Drawing.Size(41, 21);
            this.driveLetterSrc.TabIndex = 1;
            this.driveLetterSrc.SelectedIndexChanged += new System.EventHandler(this.driveLetterSrc_SelectedIndexChanged);
            // 
            // pspFolder
            // 
            this.pspFolder.BackColor = System.Drawing.Color.DimGray;
            this.pspFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pspFolder.ForeColor = System.Drawing.Color.Lime;
            this.pspFolder.Location = new System.Drawing.Point(121, 13);
            this.pspFolder.Name = "pspFolder";
            this.pspFolder.Size = new System.Drawing.Size(194, 20);
            this.pspFolder.TabIndex = 2;
            this.pspFolder.Text = "PSP";
            this.pspFolder.TextChanged += new System.EventHandler(this.pspFolder_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Black;
            this.groupBox1.Controls.Add(this.cmaDir);
            this.groupBox1.Controls.Add(this.driveLetterDst);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.driveLetterSrc);
            this.groupBox1.Controls.Add(this.pspFolder);
            this.groupBox1.ForeColor = System.Drawing.Color.Lime;
            this.groupBox1.Location = new System.Drawing.Point(162, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(622, 41);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Directories";
            // 
            // cmaDir
            // 
            this.cmaDir.BackColor = System.Drawing.Color.DimGray;
            this.cmaDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cmaDir.ForeColor = System.Drawing.Color.Lime;
            this.cmaDir.Location = new System.Drawing.Point(439, 14);
            this.cmaDir.Name = "cmaDir";
            this.cmaDir.Size = new System.Drawing.Size(177, 20);
            this.cmaDir.TabIndex = 6;
            this.cmaDir.Text = "Users\\XXX\\Documents\\PS Vita";
            this.cmaDir.TextChanged += new System.EventHandler(this.cmaDir_TextChanged);
            // 
            // driveLetterDst
            // 
            this.driveLetterDst.BackColor = System.Drawing.Color.DimGray;
            this.driveLetterDst.DisplayMember = "1";
            this.driveLetterDst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveLetterDst.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.driveLetterDst.ForeColor = System.Drawing.Color.Lime;
            this.driveLetterDst.FormattingEnabled = true;
            this.driveLetterDst.Items.AddRange(new object[] {
            "A:\\",
            "B:\\",
            "C:\\",
            "D:\\",
            "E:\\",
            "F:\\",
            "G:\\",
            "H:\\",
            "I:\\",
            "J:\\",
            "K:\\",
            "L:\\",
            "M:\\",
            "N:\\",
            "O:\\",
            "P:\\",
            "Q:\\",
            "R:\\",
            "S:\\",
            "T:\\",
            "U:\\",
            "V:\\",
            "W:\\",
            "X:\\",
            "Y:\\",
            "Z:\\"});
            this.driveLetterDst.Location = new System.Drawing.Point(392, 12);
            this.driveLetterDst.Name = "driveLetterDst";
            this.driveLetterDst.Size = new System.Drawing.Size(41, 21);
            this.driveLetterDst.TabIndex = 5;
            this.driveLetterDst.SelectedIndexChanged += new System.EventHandler(this.driveLetterDst_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "CMA Folder:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "PSP Folder:";
            // 
            // pspGames
            // 
            this.pspGames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pspGames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pspGames.ForeColor = System.Drawing.Color.Lime;
            this.pspGames.FormattingEnabled = true;
            this.pspGames.Location = new System.Drawing.Point(162, 73);
            this.pspGames.Name = "pspGames";
            this.pspGames.Size = new System.Drawing.Size(622, 223);
            this.pspGames.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Lime;
            this.label3.Location = new System.Drawing.Point(159, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Games on PSP:";
            // 
            // transVita
            // 
            this.transVita.BackColor = System.Drawing.Color.Black;
            this.transVita.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.transVita.ForeColor = System.Drawing.Color.Red;
            this.transVita.Location = new System.Drawing.Point(649, 302);
            this.transVita.Name = "transVita";
            this.transVita.Size = new System.Drawing.Size(135, 23);
            this.transVita.TabIndex = 6;
            this.transVita.Text = "MOV 1, PSVITA";
            this.transVita.UseVisualStyleBackColor = false;
            this.transVita.EnabledChanged += new System.EventHandler(this.transVita_EnabledChanged);
            this.transVita.Click += new System.EventHandler(this.transVita_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(202, 302);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(441, 23);
            this.progressBar.TabIndex = 7;
            // 
            // progressStatus
            // 
            this.progressStatus.AutoSize = true;
            this.progressStatus.ForeColor = System.Drawing.Color.Lime;
            this.progressStatus.Location = new System.Drawing.Point(159, 307);
            this.progressStatus.Name = "progressStatus";
            this.progressStatus.Size = new System.Drawing.Size(21, 13);
            this.progressStatus.TabIndex = 8;
            this.progressStatus.Text = "0%";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::CHOVY_TRANSFER.Properties.Resources.chovytrans;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(9, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 330);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // dexToggle
            // 
            this.dexToggle.Location = new System.Drawing.Point(781, 0);
            this.dexToggle.Name = "dexToggle";
            this.dexToggle.Size = new System.Drawing.Size(16, 16);
            this.dexToggle.TabIndex = 10;
            this.dexToggle.TabStop = false;
            this.dexToggle.Click += new System.EventHandler(this.dexToggle_Click);
            // 
            // currentFile
            // 
            this.currentFile.AutoSize = true;
            this.currentFile.ForeColor = System.Drawing.Color.Lime;
            this.currentFile.Location = new System.Drawing.Point(173, 328);
            this.currentFile.Name = "currentFile";
            this.currentFile.Size = new System.Drawing.Size(55, 13);
            this.currentFile.TabIndex = 11;
            this.currentFile.Text = "Waiting ...";
            // 
            // CHOVYTRANSFER
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(796, 350);
            this.Controls.Add(this.currentFile);
            this.Controls.Add(this.dexToggle);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.progressStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.transVita);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pspGames);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CHOVYTRANSFER";
            this.Text = "Chovy-Transfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CHOVYTRANSFER_FormClosing);
            this.Load += new System.EventHandler(this.CHOVYTRANSFER_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dexToggle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox driveLetterSrc;
        private System.Windows.Forms.TextBox pspFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox cmaDir;
        private System.Windows.Forms.ComboBox driveLetterDst;
        private System.Windows.Forms.ListBox pspGames;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button transVita;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox dexToggle;
        private System.Windows.Forms.Label currentFile;
    }
}

