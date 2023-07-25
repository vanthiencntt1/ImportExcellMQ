
namespace ImportXML
{
    partial class frmChuyenPhieuMisa
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChuyenPhieuMisa));
            this.tabPane1 = new DevExpress.XtraBars.Navigation.TabPane();
            this.tabNavigationPage1 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.ma_tk_no = new System.Windows.Forms.TextBox();
            this.ma_tk_co = new System.Windows.Forms.TextBox();
            this.tk_no = new SergeUtils.EasyCompletionComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tk_co = new SergeUtils.EasyCompletionComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblNofication = new System.Windows.Forms.Label();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnTransport = new DevExpress.XtraEditors.SimpleButton();
            this.dateTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.cbStatus = new SergeUtils.EasyCompletionComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.cbUser = new SergeUtils.EasyCompletionComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grList = new DevExpress.XtraGrid.GridControl();
            this.gvList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tabNavigationPage2 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            ((System.ComponentModel.ISupportInitialize)(this.tabPane1)).BeginInit();
            this.tabPane1.SuspendLayout();
            this.tabNavigationPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPane1
            // 
            this.tabPane1.AllowCollapse = DevExpress.Utils.DefaultBoolean.Default;
            this.tabPane1.Controls.Add(this.tabNavigationPage1);
            this.tabPane1.Controls.Add(this.tabNavigationPage2);
            this.tabPane1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPane1.Location = new System.Drawing.Point(0, 0);
            this.tabPane1.Name = "tabPane1";
            this.tabPane1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.tabNavigationPage1,
            this.tabNavigationPage2});
            this.tabPane1.RegularSize = new System.Drawing.Size(897, 677);
            this.tabPane1.SelectedPage = this.tabNavigationPage1;
            this.tabPane1.Size = new System.Drawing.Size(897, 677);
            this.tabPane1.TabIndex = 4;
            this.tabPane1.Text = "tabPane1";
            // 
            // tabNavigationPage1
            // 
            this.tabNavigationPage1.Caption = "Danh sách phiếu thu";
            this.tabNavigationPage1.Controls.Add(this.ma_tk_no);
            this.tabNavigationPage1.Controls.Add(this.ma_tk_co);
            this.tabNavigationPage1.Controls.Add(this.tk_no);
            this.tabNavigationPage1.Controls.Add(this.label6);
            this.tabNavigationPage1.Controls.Add(this.tk_co);
            this.tabNavigationPage1.Controls.Add(this.label5);
            this.tabNavigationPage1.Controls.Add(this.lblNofication);
            this.tabNavigationPage1.Controls.Add(this.btnClose);
            this.tabNavigationPage1.Controls.Add(this.btnSearch);
            this.tabNavigationPage1.Controls.Add(this.chkAll);
            this.tabNavigationPage1.Controls.Add(this.btnTransport);
            this.tabNavigationPage1.Controls.Add(this.dateTo);
            this.tabNavigationPage1.Controls.Add(this.label2);
            this.tabNavigationPage1.Controls.Add(this.cbStatus);
            this.tabNavigationPage1.Controls.Add(this.label4);
            this.tabNavigationPage1.Controls.Add(this.dateFrom);
            this.tabNavigationPage1.Controls.Add(this.label3);
            this.tabNavigationPage1.Controls.Add(this.cbUser);
            this.tabNavigationPage1.Controls.Add(this.label1);
            this.tabNavigationPage1.Controls.Add(this.grList);
            this.tabNavigationPage1.Name = "tabNavigationPage1";
            this.tabNavigationPage1.Size = new System.Drawing.Size(897, 650);
            // 
            // ma_tk_no
            // 
            this.ma_tk_no.Location = new System.Drawing.Point(77, 58);
            this.ma_tk_no.Name = "ma_tk_no";
            this.ma_tk_no.Size = new System.Drawing.Size(100, 21);
            this.ma_tk_no.TabIndex = 6;
            this.ma_tk_no.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ma_tk_no_KeyDown);
            // 
            // ma_tk_co
            // 
            this.ma_tk_co.Location = new System.Drawing.Point(77, 34);
            this.ma_tk_co.Name = "ma_tk_co";
            this.ma_tk_co.Size = new System.Drawing.Size(100, 21);
            this.ma_tk_co.TabIndex = 4;
           // this.ma_tk_co.TextChanged += new System.EventHandler(this.ma_tk_co_TextChanged);
            this.ma_tk_co.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ma_tk_co_KeyDown);
            // 
            // tk_no
            // 
            this.tk_no.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tk_no.FormattingEnabled = true;
            this.tk_no.Location = new System.Drawing.Point(181, 58);
            this.tk_no.Name = "tk_no";
            this.tk_no.Size = new System.Drawing.Size(710, 21);
            this.tk_no.TabIndex = 7;
            this.tk_no.SelectedValueChanged += new System.EventHandler(this.tk_no_SelectedValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Tk nợ:";
            // 
            // tk_co
            // 
            this.tk_co.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tk_co.FormattingEnabled = true;
            this.tk_co.Location = new System.Drawing.Point(181, 34);
            this.tk_co.Name = "tk_co";
            this.tk_co.Size = new System.Drawing.Size(710, 21);
            this.tk_co.TabIndex = 5;
            this.tk_co.SelectedValueChanged += new System.EventHandler(this.tk_co_SelectedValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Tk có:";
            // 
            // lblNofication
            // 
            this.lblNofication.AutoSize = true;
            this.lblNofication.BackColor = System.Drawing.Color.Transparent;
            this.lblNofication.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic);
            this.lblNofication.ForeColor = System.Drawing.Color.Red;
            this.lblNofication.Location = new System.Drawing.Point(415, 99);
            this.lblNofication.Name = "lblNofication";
            this.lblNofication.Size = new System.Drawing.Size(101, 14);
            this.lblNofication.TabIndex = 26;
            this.lblNofication.Text = "Chuyển dữ liệu...";
            this.lblNofication.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.Image")));
            this.btnClose.Location = new System.Drawing.Point(816, 91);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(69, 34);
            this.btnClose.TabIndex = 25;
            this.btnClose.Text = "Thoát";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.ImageOptions.Image")));
            this.btnSearch.Location = new System.Drawing.Point(639, 91);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(84, 34);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(13, 142);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 16;
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.Click += new System.EventHandler(this.chkAll_Click);
            // 
            // btnTransport
            // 
            this.btnTransport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTransport.ImageOptions.Image")));
            this.btnTransport.Location = new System.Drawing.Point(729, 91);
            this.btnTransport.Name = "btnTransport";
            this.btnTransport.Size = new System.Drawing.Size(81, 34);
            this.btnTransport.TabIndex = 23;
            this.btnTransport.Text = "Chuyển";
            this.btnTransport.Click += new System.EventHandler(this.btnTransport_Click);
            // 
            // dateTo
            // 
            this.dateTo.CustomFormat = "dd/MM/yyyy";
            this.dateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTo.Location = new System.Drawing.Point(77, 7);
            this.dateTo.Name = "dateTo";
            this.dateTo.Size = new System.Drawing.Size(115, 21);
            this.dateTo.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Từ ngày:";
            // 
            // cbStatus
            // 
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Items.AddRange(new object[] {
            "Tất cả",
            "Đã chuyển",
            "Chưa chuyển"});
            this.cbStatus.Location = new System.Drawing.Point(482, 7);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(115, 21);
            this.cbStatus.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(415, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Trạng thái:";
            // 
            // dateFrom
            // 
            this.dateFrom.CustomFormat = "dd/MM/yyyy";
            this.dateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateFrom.Location = new System.Drawing.Point(274, 7);
            this.dateFrom.Name = "dateFrom";
            this.dateFrom.Size = new System.Drawing.Size(126, 21);
            this.dateFrom.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Đến ngày:";
            // 
            // cbUser
            // 
            this.cbUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUser.FormattingEnabled = true;
            this.cbUser.Location = new System.Drawing.Point(678, 7);
            this.cbUser.Name = "cbUser";
            this.cbUser.Size = new System.Drawing.Size(213, 21);
            this.cbUser.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(606, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Người dùng:";
            // 
            // grList
            // 
            this.grList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grList.Location = new System.Drawing.Point(3, 85);
            this.grList.MainView = this.gvList;
            this.grList.Name = "grList";
            this.grList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.grList.Size = new System.Drawing.Size(891, 558);
            this.grList.TabIndex = 24;
            this.grList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvList});
            // 
            // gvList
            // 
            this.gvList.ColumnPanelRowHeight = 35;
            this.gvList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10});
            this.gvList.GridControl = this.grList;
            this.gvList.IndicatorWidth = 35;
            this.gvList.Name = "gvList";
            this.gvList.OptionsFind.AlwaysVisible = true;
            this.gvList.OptionsFind.FindNullPrompt = "Nhập dữ liệu cần tìm...";
            this.gvList.OptionsFind.SearchInPreview = true;
            this.gvList.OptionsFind.ShowClearButton = false;
            this.gvList.OptionsFind.ShowFindButton = false;
            this.gvList.OptionsView.ColumnAutoWidth = false;
            this.gvList.OptionsView.ShowAutoFilterRow = true;
            this.gvList.OptionsView.ShowGroupPanel = false;
            this.gvList.RowHeight = 25;
            this.gvList.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvlist_RowStyle);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "Chọn";
            this.gridColumn1.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn1.FieldName = "check";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.FixedWidth = true;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 59;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.Caption = "MaBN";
            this.gridColumn2.FieldName = "MABN";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 100;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.Caption = "Họ tên";
            this.gridColumn3.FieldName = "HOTEN";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.ReadOnly = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            this.gridColumn3.Width = 180;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.Caption = "Số tiền";
            this.gridColumn4.DisplayFormat.FormatString = "###,###0";
            this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn4.FieldName = "SOTIEN";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.ReadOnly = true;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 100;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn5.AppearanceHeader.Options.UseFont = true;
            this.gridColumn5.Caption = "Số hiệu";
            this.gridColumn5.FieldName = "SOHIEU";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.ReadOnly = true;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            this.gridColumn5.Width = 100;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn6.AppearanceHeader.Options.UseFont = true;
            this.gridColumn6.Caption = "Số biên lai";
            this.gridColumn6.FieldName = "SOBIENLAI";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.ReadOnly = true;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 6;
            this.gridColumn6.Width = 100;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn7.AppearanceHeader.Options.UseFont = true;
            this.gridColumn7.Caption = "Người thu";
            this.gridColumn7.FieldName = "NGUOITHU";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.ReadOnly = true;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 7;
            this.gridColumn7.Width = 180;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn8.AppearanceHeader.Options.UseFont = true;
            this.gridColumn8.Caption = "Hình thức";
            this.gridColumn8.FieldName = "HINHTHUC";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.ReadOnly = true;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 8;
            this.gridColumn8.Width = 100;
            // 
            // gridColumn9
            // 
            this.gridColumn9.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn9.AppearanceHeader.Options.UseFont = true;
            this.gridColumn9.Caption = "Trạng thái";
            this.gridColumn9.FieldName = "TRANGTHAI";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 1;
            this.gridColumn9.Width = 83;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "gridColumn10";
            this.gridColumn10.FieldName = "matrangthai";
            this.gridColumn10.Name = "gridColumn10";
            // 
            // tabNavigationPage2
            // 
            this.tabNavigationPage2.Caption = "Nội dung chi tiết";
            this.tabNavigationPage2.Name = "tabNavigationPage2";
            this.tabNavigationPage2.Size = new System.Drawing.Size(858, 609);
            // 
            // frmChuyenPhieuMisa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 677);
            this.Controls.Add(this.tabPane1);
            this.Name = "frmChuyenPhieuMisa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chuyển phiếu Misa";
            this.Load += new System.EventHandler(this.frmChuyenPhieuMisa_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tabPane1)).EndInit();
            this.tabPane1.ResumeLayout(false);
            this.tabNavigationPage1.ResumeLayout(false);
            this.tabNavigationPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraBars.Navigation.TabPane tabPane1;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage1;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage2;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private System.Windows.Forms.CheckBox chkAll;
        private DevExpress.XtraEditors.SimpleButton btnTransport;
        private System.Windows.Forms.DateTimePicker dateTo;
        private System.Windows.Forms.Label label2;
        private SergeUtils.EasyCompletionComboBox cbStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateFrom;
        private System.Windows.Forms.Label label3;
        private SergeUtils.EasyCompletionComboBox cbUser;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraGrid.GridControl grList;
        private DevExpress.XtraGrid.Views.Grid.GridView gvList;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private System.Windows.Forms.Label lblNofication;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private SergeUtils.EasyCompletionComboBox tk_co;
        private System.Windows.Forms.Label label5;
        private SergeUtils.EasyCompletionComboBox tk_no;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ma_tk_co;
        private System.Windows.Forms.TextBox ma_tk_no;
    }
}