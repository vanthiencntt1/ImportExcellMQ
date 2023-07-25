namespace ImportXML
{
    partial class Frmimpdanhmucduoc
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
            this.path = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.butPath = new System.Windows.Forms.Button();
            this.sheet = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button118 = new System.Windows.Forms.Button();
            this.gridView4 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dataGridView1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView5 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cbdanhmucduoc_kieuso = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnimpcotduoc = new System.Windows.Forms.Button();
            this.cbdanhmucduoc = new System.Windows.Forms.ComboBox();
            this.button32 = new System.Windows.Forms.Button();
            this.cboNhomkho = new System.Windows.Forms.ComboBox();
            this.dmbd = new System.Windows.Forms.Button();
            this.loaibd = new System.Windows.Forms.Button();
            this.nhombd = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.nuoc = new System.Windows.Forms.Button();
            this.hang = new System.Windows.Forms.Button();
            this.nhacc = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btncapnhatnhomdieutri = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView5)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // path
            // 
            this.path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.path.BackColor = System.Drawing.SystemColors.HighlightText;
            this.path.Enabled = false;
            this.path.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.path.Location = new System.Drawing.Point(110, 2);
            this.path.Name = "path";
            this.path.Size = new System.Drawing.Size(889, 21);
            this.path.TabIndex = 10;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(6, 3);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(104, 16);
            this.label20.TabIndex = 9;
            this.label20.Text = "Tập tin excel :";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butPath
            // 
            this.butPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butPath.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butPath.Location = new System.Drawing.Point(1005, 4);
            this.butPath.Name = "butPath";
            this.butPath.Size = new System.Drawing.Size(52, 21);
            this.butPath.TabIndex = 11;
            this.butPath.Text = "...";
            this.butPath.Click += new System.EventHandler(this.butPath_Click);
            // 
            // sheet
            // 
            this.sheet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sheet.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheet.Location = new System.Drawing.Point(110, 24);
            this.sheet.Name = "sheet";
            this.sheet.Size = new System.Drawing.Size(889, 21);
            this.sheet.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(43, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Sheet :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button118
            // 
            this.button118.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button118.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button118.Location = new System.Drawing.Point(1005, 26);
            this.button118.Name = "button118";
            this.button118.Size = new System.Drawing.Size(52, 21);
            this.button118.TabIndex = 49;
            this.button118.Text = "Xem";
            this.button118.Click += new System.EventHandler(this.button118_Click);
            // 
            // gridView4
            // 
            this.gridView4.GridControl = this.dataGridView1;
            this.gridView4.Name = "gridView4";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.MainView = this.gridView1;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1041, 633);
            this.dataGridView1.TabIndex = 13;
            this.dataGridView1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1,
            this.gridView5,
            this.gridView4});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.dataGridView1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.ShowAlways;
            // 
            // gridView5
            // 
            this.gridView5.GridControl = this.dataGridView1;
            this.gridView5.Name = "gridView5";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btncapnhatnhomdieutri);
            this.tabPage3.Controls.Add(this.cbdanhmucduoc_kieuso);
            this.tabPage3.Controls.Add(this.button2);
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.btnimpcotduoc);
            this.tabPage3.Controls.Add(this.cbdanhmucduoc);
            this.tabPage3.Controls.Add(this.button32);
            this.tabPage3.Controls.Add(this.cboNhomkho);
            this.tabPage3.Controls.Add(this.dmbd);
            this.tabPage3.Controls.Add(this.loaibd);
            this.tabPage3.Controls.Add(this.nhombd);
            this.tabPage3.Controls.Add(this.button6);
            this.tabPage3.Controls.Add(this.button7);
            this.tabPage3.Controls.Add(this.button8);
            this.tabPage3.Controls.Add(this.button19);
            this.tabPage3.Controls.Add(this.nuoc);
            this.tabPage3.Controls.Add(this.hang);
            this.tabPage3.Controls.Add(this.nhacc);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1047, 639);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Dược";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cbdanhmucduoc_kieuso
            // 
            this.cbdanhmucduoc_kieuso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbdanhmucduoc_kieuso.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbdanhmucduoc_kieuso.FormattingEnabled = true;
            this.cbdanhmucduoc_kieuso.Location = new System.Drawing.Point(809, 334);
            this.cbdanhmucduoc_kieuso.Name = "cbdanhmucduoc_kieuso";
            this.cbdanhmucduoc_kieuso.Size = new System.Drawing.Size(235, 28);
            this.cbdanhmucduoc_kieuso.TabIndex = 23;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Top;
            this.button2.Enabled = false;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Red;
            this.button2.Location = new System.Drawing.Point(3, 333);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(806, 30);
            this.button2.TabIndex = 22;
            this.button2.Text = "Tên cột kiểu số:";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(3, 585);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(806, 30);
            this.button1.TabIndex = 21;
            this.button1.Text = "CHỌN NHÓM KHO :";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnimpcotduoc
            // 
            this.btnimpcotduoc.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnimpcotduoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnimpcotduoc.ForeColor = System.Drawing.Color.Red;
            this.btnimpcotduoc.Location = new System.Drawing.Point(3, 303);
            this.btnimpcotduoc.Name = "btnimpcotduoc";
            this.btnimpcotduoc.Size = new System.Drawing.Size(806, 30);
            this.btnimpcotduoc.TabIndex = 20;
            this.btnimpcotduoc.Text = "Tên cột kiểu chuỗi:";
            this.btnimpcotduoc.UseVisualStyleBackColor = true;
            this.btnimpcotduoc.Click += new System.EventHandler(this.btnimpcotduoc_Click);
            // 
            // cbdanhmucduoc
            // 
            this.cbdanhmucduoc.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbdanhmucduoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbdanhmucduoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbdanhmucduoc.FormattingEnabled = true;
            this.cbdanhmucduoc.Location = new System.Drawing.Point(809, 303);
            this.cbdanhmucduoc.Name = "cbdanhmucduoc";
            this.cbdanhmucduoc.Size = new System.Drawing.Size(235, 28);
            this.cbdanhmucduoc.TabIndex = 19;
            // 
            // button32
            // 
            this.button32.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button32.ForeColor = System.Drawing.Color.Red;
            this.button32.Location = new System.Drawing.Point(3, 391);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(1041, 30);
            this.button32.TabIndex = 18;
            this.button32.Text = "(0) Không dấu";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.button32_Click);
            // 
            // cboNhomkho
            // 
            this.cboNhomkho.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cboNhomkho.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNhomkho.Location = new System.Drawing.Point(3, 615);
            this.cboNhomkho.Name = "cboNhomkho";
            this.cboNhomkho.Size = new System.Drawing.Size(1041, 21);
            this.cboNhomkho.TabIndex = 17;
            // 
            // dmbd
            // 
            this.dmbd.Dock = System.Windows.Forms.DockStyle.Top;
            this.dmbd.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dmbd.Location = new System.Drawing.Point(3, 273);
            this.dmbd.Name = "dmbd";
            this.dmbd.Size = new System.Drawing.Size(1041, 30);
            this.dmbd.TabIndex = 12;
            this.dmbd.Text = "(10) dmbd";
            this.dmbd.UseVisualStyleBackColor = true;
            this.dmbd.Click += new System.EventHandler(this.dmbd_Click);
            // 
            // loaibd
            // 
            this.loaibd.Dock = System.Windows.Forms.DockStyle.Top;
            this.loaibd.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loaibd.Location = new System.Drawing.Point(3, 243);
            this.loaibd.Name = "loaibd";
            this.loaibd.Size = new System.Drawing.Size(1041, 30);
            this.loaibd.TabIndex = 11;
            this.loaibd.Text = "(9) Loại";
            this.loaibd.UseVisualStyleBackColor = true;
            this.loaibd.Click += new System.EventHandler(this.loaibd_Click);
            // 
            // nhombd
            // 
            this.nhombd.Dock = System.Windows.Forms.DockStyle.Top;
            this.nhombd.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nhombd.Location = new System.Drawing.Point(3, 213);
            this.nhombd.Name = "nhombd";
            this.nhombd.Size = new System.Drawing.Size(1041, 30);
            this.nhombd.TabIndex = 10;
            this.nhombd.Text = "(8) Nhóm";
            this.nhombd.UseVisualStyleBackColor = true;
            this.nhombd.Click += new System.EventHandler(this.nhombd_Click);
            // 
            // button6
            // 
            this.button6.Dock = System.Windows.Forms.DockStyle.Top;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(3, 183);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(1041, 30);
            this.button6.TabIndex = 13;
            this.button6.Text = "(7) Nhóm in";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Dock = System.Windows.Forms.DockStyle.Top;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(3, 153);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(1041, 30);
            this.button7.TabIndex = 14;
            this.button7.Text = "(6) Nhóm bộ";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Dock = System.Windows.Forms.DockStyle.Top;
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(3, 123);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(1041, 30);
            this.button8.TabIndex = 15;
            this.button8.Text = "(5) Nhóm kế toán";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button19
            // 
            this.button19.Dock = System.Windows.Forms.DockStyle.Top;
            this.button19.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button19.Location = new System.Drawing.Point(3, 93);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(1041, 30);
            this.button19.TabIndex = 16;
            this.button19.Text = "(4) Nhóm điều trị";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // nuoc
            // 
            this.nuoc.Dock = System.Windows.Forms.DockStyle.Top;
            this.nuoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nuoc.Location = new System.Drawing.Point(3, 63);
            this.nuoc.Name = "nuoc";
            this.nuoc.Size = new System.Drawing.Size(1041, 30);
            this.nuoc.TabIndex = 9;
            this.nuoc.Text = "(3) Nước";
            this.nuoc.UseVisualStyleBackColor = true;
            this.nuoc.Click += new System.EventHandler(this.nuoc_Click);
            // 
            // hang
            // 
            this.hang.Dock = System.Windows.Forms.DockStyle.Top;
            this.hang.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hang.Location = new System.Drawing.Point(3, 33);
            this.hang.Name = "hang";
            this.hang.Size = new System.Drawing.Size(1041, 30);
            this.hang.TabIndex = 8;
            this.hang.Text = "(2) Hãng";
            this.hang.UseVisualStyleBackColor = true;
            this.hang.Click += new System.EventHandler(this.hang_Click);
            // 
            // nhacc
            // 
            this.nhacc.Dock = System.Windows.Forms.DockStyle.Top;
            this.nhacc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nhacc.Location = new System.Drawing.Point(3, 3);
            this.nhacc.Name = "nhacc";
            this.nhacc.Size = new System.Drawing.Size(1041, 30);
            this.nhacc.TabIndex = 7;
            this.nhacc.Text = "(1) Nhà cung cấp";
            this.nhacc.UseVisualStyleBackColor = true;
            this.nhacc.Click += new System.EventHandler(this.nhacc_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1047, 639);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Excel";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(9, 52);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1055, 665);
            this.tabControl1.TabIndex = 50;
            // 
            // btncapnhatnhomdieutri
            // 
            this.btncapnhatnhomdieutri.Dock = System.Windows.Forms.DockStyle.Top;
            this.btncapnhatnhomdieutri.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncapnhatnhomdieutri.Location = new System.Drawing.Point(3, 363);
            this.btncapnhatnhomdieutri.Name = "btncapnhatnhomdieutri";
            this.btncapnhatnhomdieutri.Size = new System.Drawing.Size(806, 30);
            this.btncapnhatnhomdieutri.TabIndex = 24;
            this.btncapnhatnhomdieutri.Text = "Cập Nhật lại Nhóm ";
            this.btncapnhatnhomdieutri.UseVisualStyleBackColor = true;
            this.btncapnhatnhomdieutri.Click += new System.EventHandler(this.btncapnhatnhomdieutri_Click);
            // 
            // Frmimpdanhmucduoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 729);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button118);
            this.Controls.Add(this.sheet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.path);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.butPath);
            this.Name = "Frmimpdanhmucduoc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhập Danh Mục Dược Từ File Excel";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView5)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox path;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button butPath;
        private System.Windows.Forms.ComboBox sheet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button118;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView4;
        private DevExpress.XtraGrid.GridControl dataGridView1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnimpcotduoc;
        private System.Windows.Forms.ComboBox cbdanhmucduoc;
        private System.Windows.Forms.ComboBox cboNhomkho;
        private System.Windows.Forms.Button dmbd;
        private System.Windows.Forms.Button loaibd;
        private System.Windows.Forms.Button nhombd;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button nuoc;
        private System.Windows.Forms.Button hang;
        private System.Windows.Forms.Button nhacc;
        private System.Windows.Forms.TabPage tabPage1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button32;
        private System.Windows.Forms.ComboBox cbdanhmucduoc_kieuso;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btncapnhatnhomdieutri;
    }
}

