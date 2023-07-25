using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ImportXML
{
    public partial class frmChuyenPhieuMisa : XtraForm
    {
        private static LibHIS.AccessData m;
        private static LibHISExtension.AccessData m_data;
        public static OracleHelper.OracleSupport Oracle;

        private string user, xxx, sql;
        private int i_userid,i_phieuthutien=0;

        private string host = ""; //https://actapp.misa.vn
        private string app_id = "";
        private string access_code = ""; //"Fq8T+1bNZrYexZSTbmxIL3IRLJrxwr8KgdaaTmSi5BrRDqVMmDFrPzG3+6FcohYAaYNBO5ua7/tcFaC/qu6Jkbxt/zLMn0NsnD0E864B5kCx/HZFZh8ji0q2TDHcI7LixBCdGkDOAh7l2RSe4A8QO39uvB/pqEmBVFWQYfQWMwHfstXWUAL4SIicKPnS3hHLllTypXEhhfW/+CM2t27Yg9IDkpN/CPyQbVqBphC7nzt/F1XplCDfMByNZXG/ZkchWES8k1LMrO6I/QlpHC+IGMeju+KsOfquDpkguB0tiok="; // 
        private string org_company_code = ""; //"NB-BENH VIEN 2022";
        private string access_token = "";

        [Obsolete]
        public frmChuyenPhieuMisa(int userId,int phieuthutien)
        {
            InitializeComponent();
            m = Program.dal; Oracle = Program.Oracle;
            if (Oracle == null) Oracle = new OracleHelper.OracleSupport();
            if (m == null) m = new LibHIS.AccessData(Oracle);
            m_data = new LibHISExtension.AccessData(Oracle);
            LibUtility.Utility.f_SetEvent(this);
            user = m.user;
            i_userid = userId;
            xxx = user + m.mmyy(m.ngayhienhanh_server);
            i_phieuthutien = phieuthutien;
            Control.CheckForIllegalCrossThreadCalls = false;
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");
        }

        private void frmChuyenPhieuMisa_Load(object sender, EventArgs e)
        {
            load_taikhoan();
            load_user();
            lblNofication.Text = "";
            cbStatus.SelectedIndex = 2; // status not transport
            dateTo.Value = dateFrom.Value = DateTime.Now.Date;
            grList.DataSource = load_data(dateTo.Text, dateFrom.Text, cbStatus.SelectedIndex, i_userid);
        }

        private void load_taikhoan()
        {
            try
            {
                DataTable dt = m.get_data("select sotk, tentk from "+ user + ".dmtaikhoan ").Tables[0];
                tk_co.DataSource = dt.Copy();
                tk_co.DisplayMember = "tentk";
                tk_co.ValueMember = "sotk";
                tk_co.SelectedIndex = -1;

                tk_no.DataSource = dt.Copy();
                tk_no.DisplayMember = "tentk";
                tk_no.ValueMember = "sotk";
                tk_no.SelectedIndex = -1;
            }
            catch
            {

            }
        }

        private void ma_tk_co_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                tk_co.SelectedValue = ma_tk_co.Text.Trim();
            }
            catch
            {

            }
        }

        private void ma_tk_no_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                tk_no.SelectedValue = ma_tk_no.Text.Trim();
            }
            catch
            {

            }
        }

        private void tk_co_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                ma_tk_co.Text = tk_co.SelectedValue.ToString();
            }
            catch 
            {
                ma_tk_co.Text = "";
            }
        }

        private void tk_no_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                ma_tk_no.Text = tk_no.SelectedValue.ToString();
            }
            catch
            {

                ma_tk_no.Text = "";
            }
        }

        private void load_user()
        {

            sql = "select id, hoten from " + user + ".dlogin ";
            if (!m.bAdmin(i_userid)) sql += "where id=" + i_userid;

            cbUser.DataSource = m.get_data(sql).Tables[0];
            cbUser.DisplayMember = "hoten";
            cbUser.ValueMember = "id";
            cbUser.SelectedValue = i_userid;
        }

        private DataTable load_data(string to, string from, int status, int userid)
        {
            try
            {
                sql = "select * from ( ";
                sql += "select  a.*, to_char(m.ngay,'dd/mm/yyyy') as ngaygui, m.tenuserid, ";
                sql += "(case when m.id is null  then 2 else 1 end) as matrangthai, ";
                sql += "(case when m.id is null  then N'Chưa chuyển' else N'Đã chuyển' end) as trangthai ";
                sql += "from ( ";
                sql += "select  a.id, a.mabn,d.hoten,c.thue, a.loai ,sum(round( round( c.soluong*c.dongia,2) - round( c.BHYTTRA,2)- round( c.mien,2),0)) as  sotien  , s.sohieu,b.sobienlai sobienlai, l.id as manguoithu, l.hoten as nguoithu ,tt.ten hinhthuc,b.thanhtoan idhinhthuocthanhtoan ";
                sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll b on a.id = b.id  ";
                sql += "inner join xxx.v_ttrvct c on a.id = c.id  ";
                sql += "left join " + user + ".btdbn d on a.mabn = d.mabn  ";
                sql += "left join " + user + ".v_quyenso s on b.quyenso=s.id  ";
                sql += "left join " + user + ".v_dlogin l on  b.USERID=l.id  ";
                sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = b.thanhtoan ";
                sql += "where  to_char(b.ngay,'dd/mm/yyyy')= '" + to.Substring(0, 10) + "' and c.tra=0  ";
                sql += "group by a.id, a.mabn,d.hoten,c.thue, a.loai, s.sohieu,b.sobienlai ,l.id,l.hoten,tt.ten,b.thanhtoan  ";
                sql += "union all  ";
                sql += "select  a.id, a.mabn,d.hoten,c.thue, a.loai ,sum( round( round( c.soluong*c.dongia,2)- round(c.mien,2)- round(c.vattu,2)- round(c.thieu,2),0)) as sotien , s.sohieu,a.sobienlai sobienlai, l.id as manguoithu, l.hoten as nguoithu ,tt.ten hinhthuc,a.thanhtoan idhinhthuocthanhtoan  ";
                sql += "from xxx.v_vienphill a  inner join xxx.v_vienphict c on a.id = c.id  ";
                sql += "left join " + user + ".btdbn d on a.mabn = d.mabn  ";
                sql += "left join " + user + ".v_quyenso s on a.quyenso=s.id ";
                sql += "left join " + user + ".v_dlogin l on  a.USERID=l.id  ";
                sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = a.thanhtoan  ";
                sql += "where  to_char(a.ngay,'dd/mm/yyyy')= '" + to.Substring(0, 10) + "'  and c.tra=0  and c.madoituong not in (6)  ";
                sql += "group by a.id, a.mabn,d.hoten,c.thue, a.loai,round( round( c.soluong*c.dongia,2)- round(c.mien,2)- round(c.vattu,2)- round(c.thieu,2),2), s.sohieu,a.sobienlai,l.id,l.hoten, round( 0,2) ,tt.ten ,a.thanhtoan ";
                sql += ") a left join xxx.mqmisa m on a.id=m.id ";
                sql += "where a.sotien>0 ";
                sql += ") tmp ";
                if (status != 0) sql += "where tmp.matrangthai=" + status;
                if (status == 1) sql += " and to_date(tmp.ngaygui,'dd/mm/yy') between to_date('" + to.Substring(0, 10) + "','dd/mm/yy') and to_date('" + from.Substring(0, 10) + "','dd/mm/yy') ";

                DataTable dt = m.get_data_mmyy(sql, to.Substring(0, 10), from.Substring(0, 10), true).Tables[0];
                dt.Columns.Add("check", typeof(bool));
                return dt;
            }
            catch
            {
                return null;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            grList.DataSource = load_data(dateTo.Text, dateFrom.Text, cbStatus.SelectedIndex, i_userid);
        }

        private void chkAll_Click(object sender, EventArgs e)
        {
            if (gvList.RowCount < 1)
            {
                LibUtility.Utility.MsgBox("không có dữ liệu!");
                return;
            }

            DataTable dt = (DataTable)grList.DataSource;
            foreach (DataRow r in dt.Rows)
                r["check"] = chkAll.Checked;
            dt.AcceptChanges();
        }

        private bool checkConnectMisa()
        {
            try
            {
                DataTable dtConfig = m.get_data("select app_id, access_code, org_company_code from  " + user + ".config_misa where maql=1").Tables[0];

                if (dtConfig == null || dtConfig.Rows.Count < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy cấu hình Misa");
                    return false;
                }

                app_id = dtConfig.Rows[0]["app_id"].ToString();
                access_code = dtConfig.Rows[0]["access_code"].ToString();
                org_company_code = dtConfig.Rows[0]["org_company_code"].ToString();

                if (m.sHost_MisaSoft != "" && host == "") host = m.sHost_MisaSoft;
                if (m.sHost_App_id != "" && app_id == "") app_id = m.sHost_App_id;
                if (m.sHost_Access_code != "" && access_code == "") access_code = m.sHost_Access_code;
                if (m.sHost_Org_company_code != "" && org_company_code == "") org_company_code = m.sHost_Org_company_code;

                if (string.IsNullOrWhiteSpace(app_id))
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy (app_id) trong cấu hình Misa");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(access_code))
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy (access_code) trong cấu hình Misa");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(org_company_code))
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy (org_company_code) trong cấu hình Misa");
                    return false;
                }

                ConnectMisa connect = new ConnectMisa();
                connect.app_id = app_id;
                connect.access_code = access_code;
                connect.org_company_code = org_company_code;


                HttpClient client = new HttpClient();
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                Uri uri = new Uri(host.Trim('/') + "/api/oauth/actopen/connect");
                var content = new StringContent(JsonConvert.SerializeObject(connect), Encoding.UTF8, "application/json");
                var result = client.PostAsync(uri, content).Result;

                if (result.IsSuccessStatusCode)
                {
                    var contents = result.Content.ReadAsStringAsync();
                    var json = contents.Result;
                    DataTable dtconnect = (DataTable)JsonConvert.DeserializeObject("[" + json + "]", (typeof(DataTable)));

                    if (dtconnect != null && dtconnect.Rows.Count > 0)
                    {

                        DataRow row = dtconnect.Rows[0];

                        if (row != null)
                        {
                            if (row["Success"].ToString() == "False")
                            {
                                LibUtility.Utility.MsgBox(row["ErrorMessage"].ToString());
                                return false;
                            }
                            else
                            {
                                var jsonData = dtconnect.Rows[0]["Data"].ToString();

                                if (!string.IsNullOrWhiteSpace(jsonData))
                                {
                                    DataTable dtData = (DataTable)JsonConvert.DeserializeObject("[" + jsonData + "]", (typeof(DataTable)));

                                    if (dtData != null && dtData.Rows.Count > 0)
                                    {
                                        access_token = dtData.Rows[0]["access_token"].ToString();
                                        return true;
                                    }
                                }

                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "lỗi");
                return false;
            }
        }
        private void f_phieuthutien()
        {
            try
            {
                if (gvList.RowCount < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dữ liệu!");
                    return;
                }

                if (!checkConnectMisa())
                {
                    LibUtility.Utility.MsgBox("Lỗi kết nối hệ thống MISA!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ma_tk_co.Text) || string.IsNullOrWhiteSpace(ma_tk_no.Text))
                {
                    LibUtility.Utility.MsgBox("Vui lòng chọn tài khoản (có/nợ)!");
                    return;
                }
                else
                {
                    DataTable dttk = (DataTable)tk_co.DataSource;
                    DataRow row = dttk.Select("sotk='" + ma_tk_co.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (có) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }

                    dttk = (DataTable)tk_no.DataSource;
                    row = dttk.Select("sotk='" + ma_tk_no.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (nợ) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }
                }

                DataTable dt = (DataTable)grList.DataSource;

                DataRow[] rows = dt.Select("check=true");
                if (rows.Length < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dòng cần chuyển!");
                    return;
                }

                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, true);
                foreach (DataRow r in rows)
                {

                    long id = long.Parse(r["id"].ToString());
                    string mabn = r["mabn"].ToString();
                    string hoten = r["hoten"].ToString();
                    string manguoithu = r["manguoithu"].ToString();
                    string nguoithu = r["nguoithu"].ToString();
                    //xxx = user + m.mmyy(dateTo.Value.ToString("dd/MM/yyyy"));

                    sql = "select a1.stt as sttct, a1.stt, a.id,a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp , a1.mabs,a.maicd,a.chandoan,nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten,b.dvt,round( a1.soluong,3) as soluong,round(a1.dongia,2) as dongia ,round(round( a1.soluong * a1.dongia,2),0) as sotien ,f.doituong, round(a1.bhyttra,2) as bhyttra,round(round (a1.soluong * a1.dongia,2) -round( a1.bhyttra,2),0) as bntra ,a1.mavp,g.tamung,a1.madoituong, nvl(e.traituyen, 0) as traituyen,a1.id,to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra, g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,0 as loai,0 as done,(nvl(a1.giamua, a1.dongia)) as giamua,'' lydo,g.bhytghichu,0 as dtchitra,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan   as hinhthucthanhtoan ,0 as idtrongoi,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , b.dichvu,b.thuong,0 as chuongtrinh,1 as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp,nvl(a1.mien, 0) - nvl(a1.sotienngansach, 0) - nvl(a1.mienchitiet, 0) as mienct,nvl(a1.thuocxuatban, 0) as thuocxuatban,a1.taitro,nvl(a1.truythu, 0) as truythu,nvl(a1.mienchitiet, 0) as mienchitiet,a1.sttt,a1.THUE,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) -round( a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc ,g.thanhtoan idhinhthucthanhtoan ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join  xxx.v_ttrvct a1 on a.id = a1.id ";
                    sql += "inner join  " + user + ".v_giavp b on a1.mavp = b.id  ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id  ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;
                    sql += " union all ";
                    sql += "select a1.stt as sttct, a1.stt,a.id, a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp, a1.mabs,a.maicd,a.chandoan, nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten || ' ' || b.hamluong as ten,b.dang as dvt,round( a1.soluong,3) soluong , round(a1.dongia,2) as dongia,round( round(a1.soluong * a1.dongia,2),0) as sotien ,f.doituong,round(a1.bhyttra,2) as bhyttra ,round(round(a1.soluong * a1.dongia,2)  - round(a1.bhyttra,2) ,0) as bntra , a1.mavp,g.tamung,a1.madoituong,nvl(e.traituyen, 0) as traituyen,a1.id, to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra,g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,1 as loai,0 as done,nvl(a1.giamua, a1.dongia) as giamua,'' lydo,g.bhytghichu,b.dtchitra ,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan  as hinhthucthanhtoan ,0 as idtrongoi ,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , 0 dichvu,0 as thuong,b.chuongtrinh,nvl(b.phamvi, 1) as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp, round(nvl(a1.mien, 0),2) - round(nvl(a1.sotienngansach, 0),2) - round(nvl(a1.mienchitiet, 0),2) as mienct  ,nvl(a1.thuocxuatban, 0) as thuocxuatban,round(a1.taitro,2) as taitro,nvl(a1.truythu, 0) as truythu,round(nvl(a1.mienchitiet, 0),2) as mienchitiet ,a1.sttt, 0 as thue,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) - round(a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc,g.thanhtoan idhinhthuocthanhtoan  ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join xxx.v_ttrvct a1 on a.id = a1.id  ";
                    sql += "inner join " + user + ".d_dmbd b on a1.mavp = b.id ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id  ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id ";
                    sql += "left join xxx.d_theodoi td on td.id=a1.sttt  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;

                    DataTable dtct = m.get_data_mmyy(sql, dateTo.Value.ToString("dd/MM/yyyy"), dateFrom.Value.ToString("dd/MM/yyyy"), false).Tables[0];
                    List<DetailMisa> lst_detail = new List<DetailMisa>();

                    string refid = Guid.NewGuid().ToString();
                    string account_object_id = Guid.NewGuid().ToString();
                    string employee_id = Guid.NewGuid().ToString();

                    foreach (DataRow r1 in dtct.Rows)
                    {
                        DetailMisa detail = new DetailMisa();
                        detail.ref_detail_id = Guid.NewGuid().ToString();
                        detail.refid = refid;
                        detail.account_object_id = account_object_id;
                        detail.sort_order = 2;
                        detail.un_resonable_cost = false;
                        detail.amount_oc = double.Parse(r1["thanhtoan"].ToString());
                        detail.amount = double.Parse(r1["thanhtoan"].ToString());
                        detail.cash_out_amount_finance = 0;
                        detail.cash_out_diff_amount_finance = 0;
                        detail.cash_out_amount_management = 0;
                        detail.cash_out_diff_amount_management = 0;
                        detail.cash_out_exchange_rate_finance = 0;
                        detail.cash_out_exchange_rate_management = 0;
                        detail.description = r1["ten"].ToString() + "; Số lượng (" + r1["soluong"].ToString() + ")";
                        detail.debit_account = ma_tk_no.Text; //"1111";  //TK nợ (tạo tham số kế toán bv)
                        detail.credit_account = ma_tk_co.Text; //"131";  //TK có (tạo tham số kế toán bv)
                        detail.account_object_code = r["mabn"].ToString();
                        detail.state = 0;
                        lst_detail.Add(detail);
                    }


                    List<VoucherMisa> lst_voucher = new List<VoucherMisa>(); // phieu thu tien 

                    #region voucher
                    VoucherMisa voucher = new VoucherMisa();
                    voucher.detail = lst_detail;
                    voucher.voucher_type = 5;  //loai chung tu tương ứng list chưng từ misa
                    voucher.is_get_new_id = true;
                    voucher.org_refid = Guid.NewGuid().ToString();
                    voucher.is_allow_group = false;
                    voucher.org_refno = id.ToString(); //"PT00000687";  //Số chứng từ trên dữ liệu gốc (số chứng từ bên thứ 3)
                    voucher.org_reftype = 0; // ko bắt buộc
                    voucher.org_reftype_name = "";  //Tên loại chứng từ trên dữ liệu gốc (ko bắt buộc)
                    voucher.refno = "";
                    voucher.act_voucher_type = 0;
                    voucher.refid = refid;
                    voucher.account_object_id = account_object_id;
                    voucher.branch_id = "00000000-0000-0000-0000-000000000000";
                    voucher.employee_id = employee_id;
                    voucher.reason_type_id = 14;  //Lý do thu 1. Thu tiền khách hàng: 14, 2. Rút tiền gửi nhập quỹ: 10, 3. Thu hoàn ứng nhân viên: 12, 4. Khác: 13 
                    voucher.display_on_book = 0;
                    voucher.reforder = long.Parse(r["id"].ToString());
                    voucher.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.is_posted_finance = true;
                    voucher.is_posted_management = false;
                    voucher.is_posted_cash_book_finance = false;
                    voucher.is_posted_cash_book_management = false;
                    voucher.exchange_rate = 1;
                    voucher.total_amount_oc = double.Parse(r["sotien"].ToString());
                    voucher.total_amount = double.Parse(r["sotien"].ToString());
                    voucher.refno_finance = "";
                    voucher.refno_management = "";
                    voucher.account_object_name = hoten;
                    voucher.account_object_address = get_dia_chi(mabn);
                    voucher.account_object_contact_name = hoten;
                    voucher.account_object_code = mabn;
                    voucher.journal_memo = "Thu tiền của " + hoten;
                    voucher.document_included = ""; //Tài liệu kèm theo (ko bắt buộc)
                    voucher.currency_id = "VND";
                    voucher.employee_code = manguoithu;
                    voucher.employee_name = nguoithu;
                    voucher.ca_audit_refid = "00000000-0000-0000-0000-000000000000";
                    voucher.excel_row_index = 0;
                    voucher.is_valid = false;
                    voucher.reftype = 0; //Loại chứng từ(Lấy từ bảng RefType) ko bắt buộc
                    voucher.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.created_by = cbUser.Text;
                    voucher.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.modified_by = m.bAdmin(i_userid) ? "admin" : "user";
                    voucher.auto_refno = true;
                    voucher.state = 0;
                    lst_voucher.Add(voucher);
                    #endregion

                    List<DictionaryMisa> lst_dictionary = new List<DictionaryMisa>();

                    #region customer
                    DictionaryMisa dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = account_object_id;
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = true;
                    dic.is_employee = false;
                    dic.inactive = false;
                    dic.agreement_salary = 0;
                    dic.salary_coefficient = 0;
                    dic.insurance_salary = 0;
                    dic.maximize_debt_amount = 0;
                    dic.receiptable_debt_amount = 0;
                    dic.account_object_code = mabn;
                    dic.account_object_name = hoten;
                    dic.country = "Việt Nam";
                    dic.is_same_address = false;
                    dic.pay_account = ""; //tài khoản phải trả (ko bắt buộc)
                    dic.receive_account = ""; // tài khoản phải thu (không bắt buộc)
                    dic.closing_amount = 0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = Guid.NewGuid().ToString();
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.created_by = cbUser.Text;
                    dic.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.modified_by = cbUser.Text;
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion

                    #region employee
                    dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = employee_id;
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = false;
                    dic.is_employee = true;
                    dic.inactive = false;
                    dic.agreement_salary = 0;
                    dic.salary_coefficient = 0;
                    dic.insurance_salary = 0;
                    dic.maximize_debt_amount = 0;
                    dic.receiptable_debt_amount = 0;
                    dic.account_object_code = manguoithu;
                    dic.account_object_name = nguoithu;
                    dic.country = "Việt Nam";
                    dic.is_same_address = false;
                    dic.pay_account = ""; //tài khoản phải trả (ko bắt buộc)
                    dic.receive_account = ""; // tài khoản phải thu (không bắt buộc)
                    dic.closing_amount = 0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = Guid.NewGuid().ToString();
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.created_by = cbUser.Text;
                    dic.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.modified_by = cbUser.Text;
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion

                    SaveMisa_05 item = new SaveMisa_05();
                    item.app_id = app_id;
                    item.org_company_code = org_company_code;
                    item.voucher = lst_voucher;
                    item.dictionary = lst_dictionary;

                    //var json = "{\r\n    \"org_company_code\": \"congtydemoketnoiact\",\r\n    \"app_id\": \"0e0a14cf-9e4b-4af9-875b-c490f34a581b\",\r\n    \"voucher\": [\r\n        {\r\n            \"detail\": [\r\n                {\r\n                    \"ref_detail_id\": \"b61c9ce3-4e2f-46b3-b90c-425fe5ff2848\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 2,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 15.0,\r\n                    \"amount\": 15.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                },\r\n                {\r\n                    \"ref_detail_id\": \"41d9c0b3-bcfe-4306-a3db-0110d6c746b8\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 1,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 12.0,\r\n                    \"amount\": 12.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                }\r\n            ],\r\n            \"voucher_type\": 5,\r\n            \"is_get_new_id\": true,\r\n            \"org_refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"is_allow_group\": false,\r\n            \"org_refno\": \"PT00000687\",\r\n            \"org_reftype\": 1010,\r\n            \"org_reftype_name\": \"Loại CAReceipt\",\r\n            \"refno\": \"\",\r\n            \"act_voucher_type\": 0,\r\n            \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"branch_id\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"employee_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"reason_type_id\": 13,\r\n            \"display_on_book\": 0,\r\n            \"reforder\": 1621330194754,\r\n            \"refdate\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"posted_date\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"is_posted_finance\": true,\r\n            \"is_posted_management\": false,\r\n            \"is_posted_cash_book_finance\": false,\r\n            \"is_posted_cash_book_management\": false,\r\n            \"exchange_rate\": 1.0,\r\n            \"total_amount_oc\": 27.0,\r\n            \"total_amount\": 27.0,\r\n            \"refno_finance\": \"\",\r\n            \"refno_management\": \"\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"account_object_address\": \"Hà Nội\",\r\n            \"account_object_contact_name\": \"Nguyễn Ngà\",\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"journal_memo\": \"Thu tiền của KHOP0002 Nga\",\r\n            \"document_included\": \"2\",\r\n            \"currency_id\": \"VND\",\r\n            \"employee_code\": \"NV00001\",\r\n            \"employee_name\": \"Nguyễn Kiệt\",\r\n            \"ca_audit_refid\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"reftype\": 1010,\r\n            \"created_date\": \"2022-02-06T09:29:54.754754+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-02-06T09:30:02.87087+07:00\",\r\n            \"modified_by\": \"admin\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ],\r\n    \"dictionary\": [\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331.01\",\r\n            \"receive_account\": \"131.01add-alf56\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"created_by\": \"Nguyễn Ngọc Anh\",\r\n            \"modified_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"modified_by\": \"Nguyễn Ngọc Anh\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"organization_unit_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"gender\": 1,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": false,\r\n            \"is_employee\": true,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"NV00001\",\r\n            \"account_object_name\": \"Nguyễn Kiệt\",\r\n            \"organization_unit_name\": \"AN NHIEN JSC 123\",\r\n            \"is_same_address\": false,\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KH00001\",\r\n            \"account_object_name\": \"Nguyễn Chính\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331\",\r\n            \"receive_account\": \"131\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ]\r\n}"; //json_send_misa(item);
                    var json = JsonConvert.SerializeObject(item);
                    if (!System.IO.Directory.Exists("..\\json")) System.IO.Directory.CreateDirectory("..\\json");
                    File.WriteAllText("..\\json\\json.txt", json);

                    HttpClient client = new HttpClient();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Uri uri = new Uri(host.Trim('/') + "/apir/sync/actopen/save");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpRequestMessage httpRequest = new HttpRequestMessage();
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.RequestUri = uri;
                    httpRequest.Headers.Add("X-MISA-AccessToken", access_token);
                    httpRequest.Content = content;

                    var result = client.SendAsync(httpRequest).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var contents = result.Content.ReadAsStringAsync();
                        var jsonResult = contents.Result;
                        DataTable dtData = (DataTable)JsonConvert.DeserializeObject("[" + jsonResult + "]", (typeof(DataTable)));
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            string msg = dtData.Rows[0]["Data"].ToString();
                            bool success = bool.Parse(dtData.Rows[0]["Success"].ToString());

                            if (!success)
                            {
                                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                            {
                                if (!m_data.upd_mqmisa(xxx, long.Parse(r["id"].ToString()), 0, m.ngayhienhanh_server, cbUser.Text.Trim()))
                                {
                                    LibUtility.Utility.MsgBox("Lỗi cập nhập mqmisa!");
                                    return;
                                }

                                r["matrangthai"] = 1;
                                r["trangthai"] = "Đã chuyển";
                                dt.AcceptChanges();
                                //lblNofication.Text = string.Format("Số dòng gửi: {0}/{1}", index++, rows.Length);
                            }
                        }
                    }
                }
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
            }
            catch (Exception ex)
            {
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                MessageBox.Show(ex.Message, "lỗi");
            }
        }

        private void f_hoadonbanhang()
        {
            try
            {
                if (gvList.RowCount < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dữ liệu!");
                    return;
                }

                if (!checkConnectMisa())
                {
                    LibUtility.Utility.MsgBox("Lỗi kết nối hệ thống MISA!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ma_tk_co.Text) || string.IsNullOrWhiteSpace(ma_tk_no.Text))
                {
                    LibUtility.Utility.MsgBox("Vui lòng chọn tài khoản (có/nợ)!");
                    return;
                }
                else
                {
                    DataTable dttk = (DataTable)tk_co.DataSource;
                    DataRow row = dttk.Select("sotk='" + ma_tk_co.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (có) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }

                    dttk = (DataTable)tk_no.DataSource;
                    row = dttk.Select("sotk='" + ma_tk_no.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (nợ) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }
                }

                DataTable dt = (DataTable)grList.DataSource;

                DataRow[] rows = dt.Select("check=true");
                if (rows.Length < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dòng cần chuyển!");
                    return;
                }

                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, true);
                foreach (DataRow r in rows)
                {

                    long id = long.Parse(r["id"].ToString());
                    string mabn = r["mabn"].ToString();
                    string hoten = r["hoten"].ToString();
                    string manguoithu = r["manguoithu"].ToString();
                    string nguoithu = r["nguoithu"].ToString();
                    //xxx = user + m.mmyy(dateTo.Value.ToString("dd/MM/yyyy"));

                    sql = "select a1.stt as sttct, a1.stt, a.id,a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp , a1.mabs,a.maicd,a.chandoan,nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten,b.dvt,round( a1.soluong,3) as soluong,round(a1.dongia,2) as dongia ,round(round( a1.soluong * a1.dongia,2),0) as sotien ,f.doituong, round(a1.bhyttra,2) as bhyttra,round(round (a1.soluong * a1.dongia,2) -round( a1.bhyttra,2),0) as bntra ,a1.mavp,g.tamung,a1.madoituong, nvl(e.traituyen, 0) as traituyen,a1.id,to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra, g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,0 as loai,0 as done,(nvl(a1.giamua, a1.dongia)) as giamua,'' lydo,g.bhytghichu,0 as dtchitra,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan   as hinhthucthanhtoan ,0 as idtrongoi,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , b.dichvu,b.thuong,0 as chuongtrinh,1 as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp,nvl(a1.mien, 0) - nvl(a1.sotienngansach, 0) - nvl(a1.mienchitiet, 0) as mienct,nvl(a1.thuocxuatban, 0) as thuocxuatban,a1.taitro,nvl(a1.truythu, 0) as truythu,nvl(a1.mienchitiet, 0) as mienchitiet,a1.sttt,a1.THUE,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) -round( a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc ,g.thanhtoan idhinhthucthanhtoan ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join  xxx.v_ttrvct a1 on a.id = a1.id ";
                    sql += "inner join  " + user + ".v_giavp b on a1.mavp = b.id  ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id  ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;
                    sql += " union all ";
                    sql += "select a1.stt as sttct, a1.stt,a.id, a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp, a1.mabs,a.maicd,a.chandoan, nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten || ' ' || b.hamluong as ten,b.dang as dvt,round( a1.soluong,3) soluong , round(a1.dongia,2) as dongia,round( round(a1.soluong * a1.dongia,2),0) as sotien ,f.doituong,round(a1.bhyttra,2) as bhyttra ,round(round(a1.soluong * a1.dongia,2)  - round(a1.bhyttra,2) ,0) as bntra , a1.mavp,g.tamung,a1.madoituong,nvl(e.traituyen, 0) as traituyen,a1.id, to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra,g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,1 as loai,0 as done,nvl(a1.giamua, a1.dongia) as giamua,'' lydo,g.bhytghichu,b.dtchitra ,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan  as hinhthucthanhtoan ,0 as idtrongoi ,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , 0 dichvu,0 as thuong,b.chuongtrinh,nvl(b.phamvi, 1) as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp, round(nvl(a1.mien, 0),2) - round(nvl(a1.sotienngansach, 0),2) - round(nvl(a1.mienchitiet, 0),2) as mienct  ,nvl(a1.thuocxuatban, 0) as thuocxuatban,round(a1.taitro,2) as taitro,nvl(a1.truythu, 0) as truythu,round(nvl(a1.mienchitiet, 0),2) as mienchitiet ,a1.sttt, 0 as thue,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) - round(a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc,g.thanhtoan idhinhthuocthanhtoan  ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join xxx.v_ttrvct a1 on a.id = a1.id  ";
                    sql += "inner join " + user + ".d_dmbd b on a1.mavp = b.id ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id  ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id ";
                    sql += "left join xxx.d_theodoi td on td.id=a1.sttt  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;

                    DataTable dtct = m.get_data_mmyy(sql, dateTo.Value.ToString("dd/MM/yyyy"), dateFrom.Value.ToString("dd/MM/yyyy"), false).Tables[0];

                    #region bắt đầu body hàm 13 trong tài liệu misa

                    List<DetailMisa> lst_detail = new List<DetailMisa>();
                    List<in_outward> lst_in_outward = new List<in_outward>();
                    List<sa_invoice> lst_sa_invoice = new List<sa_invoice>();
                    string refid = Guid.NewGuid().ToString();
                    string account_object_id = Guid.NewGuid().ToString();
                    string employee_id = Guid.NewGuid().ToString();

                    foreach (DataRow r1 in dtct.Rows)
                    {
                        DetailMisa detail = new DetailMisa();
                        detail.ref_detail_id = Guid.NewGuid().ToString();
                        detail.refid = refid;
                        detail.account_object_id = account_object_id;
                        detail.sort_order = 2;
                        detail.un_resonable_cost = false;
                        detail.amount_oc = double.Parse(r1["thanhtoan"].ToString());
                        detail.amount = double.Parse(r1["thanhtoan"].ToString());
                        detail.cash_out_amount_finance = 0;
                        detail.cash_out_diff_amount_finance = 0;
                        detail.cash_out_amount_management = 0;
                        detail.cash_out_diff_amount_management = 0;
                        detail.cash_out_exchange_rate_finance = 0;
                        detail.cash_out_exchange_rate_management = 0;
                        detail.description = r1["ten"].ToString() + "; Số lượng (" + r1["soluong"].ToString() + ")";
                        detail.debit_account = ma_tk_no.Text; //"1111";  //TK nợ (tạo tham số kế toán bv)
                        detail.credit_account = ma_tk_co.Text; //"131";  //TK có (tạo tham số kế toán bv)
                        detail.account_object_code = r["mabn"].ToString();
                        detail.state = 0;
                        lst_detail.Add(detail);
                    }

                    in_outward in_outward = new in_outward();
                    in_outward.voucher_type= 8;
                    in_outward.is_get_new_id = true;
                    in_outward.is_allow_group = false;
                    in_outward.org_reftype = 0;
                    in_outward.act_voucher_type = 0;
                    in_outward.refid = refid; //"e0387bf7-60f3-41f4-9f73-d40eec12ce8a";
                    in_outward.account_object_id = "104c49f2-6828-4dea-88e9-08f1fb027701";
                    in_outward.employee_id = "507abe24-520e-47ad-9c39-feed3cc8ba02";
                    in_outward.branch_id = "fd745cee-9980-11ea-af8e-005056890bf4";
                    in_outward.display_on_book = 0;
                    in_outward.reforder = 1621331477543;
                    in_outward.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-05-18T00:00:00.000+07:00";
                    in_outward.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-05-18T00:00:00.000+07:00";
                    in_outward.in_reforder = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-05-18T09:48:33.000+07:00";
                    in_outward.is_posted_finance = false;
                    in_outward.is_posted_management = false;
                    in_outward.is_posted_inventory_book_finance = false;
                    in_outward.is_posted_inventory_book_management = false;
                    in_outward.is_branch_issued = false;
                    in_outward.is_sale_with_outward = true;
                    in_outward.is_invoice_replace = false;
                    in_outward.total_amount_finance = 0;
                    in_outward.total_amount_management = 0;
                    in_outward.refno_finance = "";
                    in_outward.refno_management = "";
                    in_outward.account_object_name = "KHOP0002 Nga";
                    in_outward.account_object_address = "Hải Phòng";
                    in_outward.journal_memo = "Xuất kho bán hàng KHOP0002 Nga";
                    in_outward.reftype = 2020;
                    in_outward.employee_name = nguoithu;
                    in_outward.payment_term_id = "00000000-0000-0000-0000-000000000000";
                    in_outward.due_time = 0;
                    in_outward.is_executed = false;
                    in_outward.employee_code = manguoithu;
                    in_outward.publish_status = 0;
                    in_outward.is_invoice_deleted = false;
                    in_outward.is_invoice_receipted = false;
                    in_outward.account_object_code = manguoithu;
                    in_outward.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-05-18T09:51:17.5435431+07:00";
                    in_outward.created_by = nguoithu;
                    in_outward.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-05-18T09:51:17.5435431+07:00";
                    in_outward.auto_refno = false;
                    in_outward.state = 0;
                    lst_in_outward.Add(in_outward);

                sa_invoice sa_invoice = new sa_invoice();
                sa_invoice.voucher_type= 11;
                sa_invoice.is_get_new_id = true;
                sa_invoice.is_allow_group = false;
                sa_invoice.org_reftype = 0;
                sa_invoice.act_voucher_type = 0;
                sa_invoice.journal_memo="";
                sa_invoice.reforder=0;
                sa_invoice.refid= refid;//"dbea28ed-a92f-4b5a-8a23-2b6d616c2dce";
                sa_invoice.branch_id="fd745cee-9980-11ea-af8e-005056890bf4";
                sa_invoice.account_object_id="104c49f2-6828-4dea-88e9-08f1fb027701";
                sa_invoice.employee_id="507abe24-520e-47ad-9c39-feed3cc8ba02";
                sa_invoice.voucher_reference_id="00000000-0000-0000-0000-000000000000";
                sa_invoice.display_on_book=0;
                sa_invoice.publish_status=0;
                sa_invoice.discount_type=2;
                sa_invoice.discount_rate_voucher=0;
                sa_invoice.inv_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");//"2022-05-18T00:00:00.000+07:00";
                sa_invoice.is_paid=false;
                sa_invoice.is_posted=true;
                sa_invoice.include_invoice=1;
                sa_invoice.is_attach_list=false;
                sa_invoice.is_branch_issued=false;
                sa_invoice.is_posted_last_year=false;
                sa_invoice.is_invoice_replace=false;
                sa_invoice.exchange_rate=1;
                sa_invoice.total_sale_amount_oc= decimal.Parse(r["sotien"].ToString()); ;
                sa_invoice.total_sale_amount= decimal.Parse(r["sotien"].ToString()); ;
                sa_invoice.total_discount_amount_oc=0;
                sa_invoice.total_discount_amount=0;
                sa_invoice.total_vat_amount_oc=0;
                sa_invoice.total_vat_amount=0;
                sa_invoice.total_amount_oc= decimal.Parse(r["sotien"].ToString()); ;
                sa_invoice.total_amount= decimal.Parse(r["sotien"].ToString()); ;
                sa_invoice.account_object_name="KHOP0002 Nga";
                sa_invoice.account_object_code="KHOP0002";
                sa_invoice.employee_name="Nguyễn Kiệt";
                sa_invoice.employee_code="NV00001";
                sa_invoice.account_object_address="Hải Phòng";
                sa_invoice.account_object_tax_code="2222222222";
                sa_invoice.payment_method="TM/CK";
                sa_invoice.currency_id="VND";
                sa_invoice.refno_finance="";
                sa_invoice.refno_management="";
                sa_invoice.is_created_savoucher=0;
                sa_invoice.send_email_status=0;
                sa_invoice.is_invoice_receipted=false;
                sa_invoice.invoice_status=0;
                sa_invoice.voucher_reference_reftype=0;
                sa_invoice.is_invoice_deleted=false;
                sa_invoice.inv_replacement_id="00000000-0000-0000-0000-000000000000";
                sa_invoice.is_update_template=false;
                sa_invoice.is_increase_invno=false;
                sa_invoice.ccy_exchange_operator=false;
                sa_invoice.business_area=0;
                sa_invoice.excel_row_index=0;
                sa_invoice.is_valid=false;
                sa_invoice.reftype=3560;
                sa_invoice.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-05-18T09:51:17.5385382+07:00";
                sa_invoice.created_by=nguoithu;
                sa_invoice.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");//"2022-05-18T09:51:17.5385382+07:00";
                sa_invoice.auto_refno=false;
                sa_invoice.state = 0;
                lst_sa_invoice.Add(sa_invoice);

                    List<VoucherMisa_13> lst_voucher = new List<VoucherMisa_13>(); // phieu thu tien 

                    #region voucher
                    VoucherMisa_13 voucher = new VoucherMisa_13(); 
                    voucher.detail = lst_detail;
                    voucher.in_outward = lst_in_outward;
                    voucher.sa_invoice = lst_sa_invoice;
                    voucher.voucher_type = 13; // thong so hoa don ban hang
                    voucher.is_get_new_id = true;
                    voucher.org_refid = Guid.NewGuid().ToString();
                    voucher.is_allow_group = false;
                    voucher.org_refno = id.ToString(); //"PT00000687";  //Số chứng từ trên dữ liệu gốc (số chứng từ bên thứ 3)
                    voucher.org_reftype = 0; // ko bắt buộc
                    voucher.org_reftype_name = "";  //Tên loại chứng từ trên dữ liệu gốc (ko bắt buộc)
                    voucher.refno = "";
                    voucher.act_voucher_type = 0;
                    voucher.refid = refid;
                    voucher.account_object_id = account_object_id;
                    voucher.branch_id = "00000000-0000-0000-0000-000000000000";
                    voucher.employee_id = employee_id;
                    voucher.reason_type_id = 14;  //Lý do thu 1. Thu tiền khách hàng: 14, 2. Rút tiền gửi nhập quỹ: 10, 3. Thu hoàn ứng nhân viên: 12, 4. Khác: 13 
                    voucher.display_on_book = 0;
                    voucher.reforder = long.Parse(r["id"].ToString());
                    voucher.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.is_posted_finance = true;
                    voucher.is_posted_management = false;
                    voucher.is_posted_cash_book_finance = false;
                    voucher.is_posted_cash_book_management = false;
                    voucher.exchange_rate = 1;
                    voucher.total_amount_oc = double.Parse(r["sotien"].ToString());
                    voucher.total_amount = double.Parse(r["sotien"].ToString());
                    voucher.refno_finance = "";
                    voucher.refno_management = "";
                    voucher.account_object_name = hoten;
                    voucher.account_object_address = get_dia_chi(mabn);
                    voucher.account_object_contact_name = hoten;
                    voucher.account_object_code = mabn;
                    voucher.journal_memo = "Thu tiền của " + hoten;
                    voucher.document_included = ""; //Tài liệu kèm theo (ko bắt buộc)
                    voucher.currency_id = "VND";
                    voucher.employee_code = manguoithu;
                    voucher.employee_name = nguoithu;
                    voucher.ca_audit_refid = "00000000-0000-0000-0000-000000000000";
                    voucher.excel_row_index = 0;
                    voucher.is_valid = false;
                    voucher.reftype = 0; //Loại chứng từ(Lấy từ bảng RefType) ko bắt buộc
                    voucher.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.created_by = cbUser.Text;
                    voucher.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.modified_by = m.bAdmin(i_userid) ? "admin" : "user";
                    voucher.auto_refno = true;
                    voucher.state = 0;
                    lst_voucher.Add(voucher);
                    #endregion

                    List<DictionaryMisa> lst_dictionary = new List<DictionaryMisa>();

                    #region customer
                    DictionaryMisa dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = account_object_id;
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = true;
                    dic.is_employee = false;
                    dic.inactive = false;
                    dic.agreement_salary = 0;
                    dic.salary_coefficient = 0;
                    dic.insurance_salary = 0;
                    dic.maximize_debt_amount = 0;
                    dic.receiptable_debt_amount = 0;
                    dic.account_object_code = mabn;
                    dic.account_object_name = hoten;
                    dic.country = "Việt Nam";
                    dic.is_same_address = false;
                    dic.pay_account = ""; //tài khoản phải trả (ko bắt buộc)
                    dic.receive_account = ""; // tài khoản phải thu (không bắt buộc)
                    dic.closing_amount = 0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = Guid.NewGuid().ToString();
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.created_by = cbUser.Text;
                    dic.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.modified_by = cbUser.Text;
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion

                    #region employee
                    dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = employee_id;
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = false;
                    dic.is_employee = true;
                    dic.inactive = false;
                    dic.agreement_salary = 0;
                    dic.salary_coefficient = 0;
                    dic.insurance_salary = 0;
                    dic.maximize_debt_amount = 0;
                    dic.receiptable_debt_amount = 0;
                    dic.account_object_code = manguoithu;
                    dic.account_object_name = nguoithu;
                    dic.country = "Việt Nam";
                    dic.is_same_address = false;
                    dic.pay_account = ""; //tài khoản phải trả (ko bắt buộc)
                    dic.receive_account = ""; // tài khoản phải thu (không bắt buộc)
                    dic.closing_amount = 0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = Guid.NewGuid().ToString();
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.created_by = cbUser.Text;
                    dic.modified_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    dic.modified_by = cbUser.Text;
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion

                    SaveMisa_13 item = new SaveMisa_13();
                    item.app_id = app_id;
                    item.org_company_code = org_company_code;
                    item.voucher = lst_voucher;
                    item.dictionary = lst_dictionary;

# endregion ket thuc body hàm 13 trong tài liệu misa

                    //var json = "{\r\n    \"org_company_code\": \"congtydemoketnoiact\",\r\n    \"app_id\": \"0e0a14cf-9e4b-4af9-875b-c490f34a581b\",\r\n    \"voucher\": [\r\n        {\r\n            \"detail\": [\r\n                {\r\n                    \"ref_detail_id\": \"b61c9ce3-4e2f-46b3-b90c-425fe5ff2848\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 2,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 15.0,\r\n                    \"amount\": 15.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                },\r\n                {\r\n                    \"ref_detail_id\": \"41d9c0b3-bcfe-4306-a3db-0110d6c746b8\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 1,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 12.0,\r\n                    \"amount\": 12.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                }\r\n            ],\r\n            \"voucher_type\": 5,\r\n            \"is_get_new_id\": true,\r\n            \"org_refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"is_allow_group\": false,\r\n            \"org_refno\": \"PT00000687\",\r\n            \"org_reftype\": 1010,\r\n            \"org_reftype_name\": \"Loại CAReceipt\",\r\n            \"refno\": \"\",\r\n            \"act_voucher_type\": 0,\r\n            \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"branch_id\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"employee_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"reason_type_id\": 13,\r\n            \"display_on_book\": 0,\r\n            \"reforder\": 1621330194754,\r\n            \"refdate\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"posted_date\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"is_posted_finance\": true,\r\n            \"is_posted_management\": false,\r\n            \"is_posted_cash_book_finance\": false,\r\n            \"is_posted_cash_book_management\": false,\r\n            \"exchange_rate\": 1.0,\r\n            \"total_amount_oc\": 27.0,\r\n            \"total_amount\": 27.0,\r\n            \"refno_finance\": \"\",\r\n            \"refno_management\": \"\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"account_object_address\": \"Hà Nội\",\r\n            \"account_object_contact_name\": \"Nguyễn Ngà\",\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"journal_memo\": \"Thu tiền của KHOP0002 Nga\",\r\n            \"document_included\": \"2\",\r\n            \"currency_id\": \"VND\",\r\n            \"employee_code\": \"NV00001\",\r\n            \"employee_name\": \"Nguyễn Kiệt\",\r\n            \"ca_audit_refid\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"reftype\": 1010,\r\n            \"created_date\": \"2022-02-06T09:29:54.754754+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-02-06T09:30:02.87087+07:00\",\r\n            \"modified_by\": \"admin\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ],\r\n    \"dictionary\": [\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331.01\",\r\n            \"receive_account\": \"131.01add-alf56\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"created_by\": \"Nguyễn Ngọc Anh\",\r\n            \"modified_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"modified_by\": \"Nguyễn Ngọc Anh\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"organization_unit_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"gender\": 1,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": false,\r\n            \"is_employee\": true,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"NV00001\",\r\n            \"account_object_name\": \"Nguyễn Kiệt\",\r\n            \"organization_unit_name\": \"AN NHIEN JSC 123\",\r\n            \"is_same_address\": false,\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KH00001\",\r\n            \"account_object_name\": \"Nguyễn Chính\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331\",\r\n            \"receive_account\": \"131\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ]\r\n}"; //json_send_misa(item);
                    var json = JsonConvert.SerializeObject(item);
                    if (!System.IO.Directory.Exists("..\\json")) System.IO.Directory.CreateDirectory("..\\json");
                    File.WriteAllText("..\\json\\json.txt", json);

                    HttpClient client = new HttpClient();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Uri uri = new Uri(host.Trim('/') + "/apir/sync/actopen/save");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpRequestMessage httpRequest = new HttpRequestMessage();
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.RequestUri = uri;
                    httpRequest.Headers.Add("X-MISA-AccessToken", access_token);
                    httpRequest.Content = content;

                    var result = client.SendAsync(httpRequest).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var contents = result.Content.ReadAsStringAsync();
                        var jsonResult = contents.Result;
                        DataTable dtData = (DataTable)JsonConvert.DeserializeObject("[" + jsonResult + "]", (typeof(DataTable)));
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            string msg = dtData.Rows[0]["Data"].ToString();
                            bool success = bool.Parse(dtData.Rows[0]["Success"].ToString());

                            if (!success)
                            {
                                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                            {
                                if (!m_data.upd_mqmisa(xxx, long.Parse(r["id"].ToString()), 0, m.ngayhienhanh_server, cbUser.Text.Trim()))
                                {
                                    LibUtility.Utility.MsgBox("Lỗi cập nhập mqmisa!");
                                    return;
                                }

                                r["matrangthai"] = 1;
                                r["trangthai"] = "Đã chuyển";
                                dt.AcceptChanges();
                                //lblNofication.Text = string.Format("Số dòng gửi: {0}/{1}", index++, rows.Length);
                            }
                        }
                    }
                }
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
            }
            catch (Exception ex)
            {
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                MessageBox.Show(ex.Message, "lỗi");
            }
        }

        private void f_phieunhapkho()
        {
            try
            {
                if (gvList.RowCount < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dữ liệu!");
                    return;
                }

                if (!checkConnectMisa())
                {
                    LibUtility.Utility.MsgBox("Lỗi kết nối hệ thống MISA!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ma_tk_co.Text) || string.IsNullOrWhiteSpace(ma_tk_no.Text))
                {
                    LibUtility.Utility.MsgBox("Vui lòng chọn tài khoản (có/nợ)!");
                    return;
                }
                else
                {
                    DataTable dttk = (DataTable)tk_co.DataSource;
                    DataRow row = dttk.Select("sotk='" + ma_tk_co.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (có) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }

                    dttk = (DataTable)tk_no.DataSource;
                    row = dttk.Select("sotk='" + ma_tk_no.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (nợ) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }
                }

                DataTable dt = (DataTable)grList.DataSource;

                DataRow[] rows = dt.Select("check=true");
                if (rows.Length < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dòng cần chuyển!");
                    return;
                }

                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, true);
                foreach (DataRow r in rows)
                {

                    long id = long.Parse(r["id"].ToString());
                    string mabn = r["mabn"].ToString();
                    string hoten = r["hoten"].ToString();
                    string manguoithu = r["manguoithu"].ToString();
                    string nguoithu = r["nguoithu"].ToString();
                    //xxx = user + m.mmyy(dateTo.Value.ToString("dd/MM/yyyy"));

                    sql = "select a1.stt as sttct, a1.stt, a.id,a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp , a1.mabs,a.maicd,a.chandoan,nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten,b.dvt,round( a1.soluong,3) as soluong,round(a1.dongia,2) as dongia ,round(round( a1.soluong * a1.dongia,2),0) as sotien ,f.doituong, round(a1.bhyttra,2) as bhyttra,round(round (a1.soluong * a1.dongia,2) -round( a1.bhyttra,2),0) as bntra ,a1.mavp,g.tamung,a1.madoituong, nvl(e.traituyen, 0) as traituyen,a1.id,to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra, g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,0 as loai,0 as done,(nvl(a1.giamua, a1.dongia)) as giamua,'' lydo,g.bhytghichu,0 as dtchitra,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan   as hinhthucthanhtoan ,0 as idtrongoi,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , b.dichvu,b.thuong,0 as chuongtrinh,1 as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp,nvl(a1.mien, 0) - nvl(a1.sotienngansach, 0) - nvl(a1.mienchitiet, 0) as mienct,nvl(a1.thuocxuatban, 0) as thuocxuatban,a1.taitro,nvl(a1.truythu, 0) as truythu,nvl(a1.mienchitiet, 0) as mienchitiet,a1.sttt,a1.THUE,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) -round( a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc ,g.thanhtoan idhinhthucthanhtoan ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join  xxx.v_ttrvct a1 on a.id = a1.id ";
                    sql += "inner join  " + user + ".v_giavp b on a1.mavp = b.id  ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id  ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;
                    sql += " union all ";
                    sql += "select a1.stt as sttct, a1.stt,a.id, a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp, a1.mabs,a.maicd,a.chandoan, nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten || ' ' || b.hamluong as ten,b.dang as dvt,round( a1.soluong,3) soluong , round(a1.dongia,2) as dongia,round( round(a1.soluong * a1.dongia,2),0) as sotien ,f.doituong,round(a1.bhyttra,2) as bhyttra ,round(round(a1.soluong * a1.dongia,2)  - round(a1.bhyttra,2) ,0) as bntra , a1.mavp,g.tamung,a1.madoituong,nvl(e.traituyen, 0) as traituyen,a1.id, to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra,g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,1 as loai,0 as done,nvl(a1.giamua, a1.dongia) as giamua,'' lydo,g.bhytghichu,b.dtchitra ,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan  as hinhthucthanhtoan ,0 as idtrongoi ,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , 0 dichvu,0 as thuong,b.chuongtrinh,nvl(b.phamvi, 1) as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp, round(nvl(a1.mien, 0),2) - round(nvl(a1.sotienngansach, 0),2) - round(nvl(a1.mienchitiet, 0),2) as mienct  ,nvl(a1.thuocxuatban, 0) as thuocxuatban,round(a1.taitro,2) as taitro,nvl(a1.truythu, 0) as truythu,round(nvl(a1.mienchitiet, 0),2) as mienchitiet ,a1.sttt, 0 as thue,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) - round(a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc,g.thanhtoan idhinhthuocthanhtoan  ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join xxx.v_ttrvct a1 on a.id = a1.id  ";
                    sql += "inner join " + user + ".d_dmbd b on a1.mavp = b.id ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id  ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id ";
                    sql += "left join xxx.d_theodoi td on td.id=a1.sttt  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;

                    DataTable dtct = m.get_data_mmyy(sql, dateTo.Value.ToString("dd/MM/yyyy"), dateFrom.Value.ToString("dd/MM/yyyy"), false).Tables[0];

                    #region bắt đầu body hàm 13 trong tài liệu misa

                    List<DetailMisa> lst_detail = new List<DetailMisa>();
                    List<in_outward> lst_in_outward = new List<in_outward>();
                    List<sa_invoice> lst_sa_invoice = new List<sa_invoice>();
                    string refid = Guid.NewGuid().ToString();
                    string account_object_id = Guid.NewGuid().ToString();
                    string employee_id = Guid.NewGuid().ToString();

                    foreach (DataRow r1 in dtct.Rows)
                    {
                        DetailMisa detail = new DetailMisa();
                        detail.ref_detail_id = Guid.NewGuid().ToString();

                        detail.stock_id = "65834d8b-a73b-4e55-9fdf-6eecf61fb6eb";
                        detail.stock_code = "KHO1";
                        detail.stock_name = "Kho hàng hóa";
                        detail.refid = refid;
                        detail.account_object_id = account_object_id;
                        detail.inventory_item_id = "ff874c8f-8686-4f84-83c8-9d1de7365a2e";
                        detail.from_stock_id = "65834d8b-a73b-4e55-9fdf-6eecf61fb6eb";
                        detail.to_stock_id = "99ce3f40-242e-4418-bb68-808c1792007b";
                        detail.unit_id = "81cebf94-9cb9-4b20-880a-a01fc0f6d850";
                        detail.organization_unit_id = "bdb4458c-30fd-4e9d-a34a-9849803fe690";
                        detail.main_unit_id = "81cebf94-9cb9-4b20-880a-a01fc0f6d850";
                        detail.sort_order = 1;
                        detail.inventory_resale_type_id = 0;
                        detail.un_resonable_cost = false;
                        detail.quantity = 2.0;
                        detail.sale_price = 220000.0;
                        detail.sale_amount = 440000.0;
                        detail.unit_price_finance = 500000.0;
                        detail.amount_finance = 1000000.0;
                        detail.unit_price_management = 0.0;
                        detail.amount_management = 1000000.0;
                        detail.main_unit_price_finance = 500000.0;
                        detail.main_unit_price_management = 0.0;
                        detail.main_convert_rate = 1.0;
                        detail.main_quantity = 2.0;
                        detail.description = "Vitamin PEDIA KID";
                        detail.debit_account = "152";
                        detail.credit_account = "156";
                        detail.exchange_rate_operator = "*";
                        detail.inventory_item_code = "PEDIA KID";
                        detail.organization_unit_code = "CN200";
                        detail.unit_name = "Chai";
                        detail.organization_unit_name = "CN200";
                        detail.main_unit_name = "Chai";
                        detail.from_stock_code = "KHO1";
                        detail.from_stock_name = "Kho hàng hóa";
                        detail.to_stock_code = "Kho2";
                        detail.to_stock_name = "Kho nhập";
                        detail.is_description = false;
                        detail.inventory_item_name = "Vitamin PEDIA KID";
                        detail.is_follow_serial_number = false;
                        detail.is_allow_duplicate_serial_number = false;
                        detail.reftype = 0;
                        detail.sale_price1 = 0.0;
                        detail.is_description_import = false;
                        detail.is_promotion_import = false;
                        detail.un_resonable_cost_import = false;
                        detail.state = 0;
                        lst_detail.Add(detail);
                    }


                    List<VoucherMisa_13> lst_voucher = new List<VoucherMisa_13>();
                    #region voucher
                    VoucherMisa_13 voucher = new VoucherMisa_13();
                    voucher.detail = lst_detail;
                    voucher.voucher_type = 7;
                    voucher.is_get_new_id = true;
                    voucher.org_refid = "775b9b77-8e48-4c51-ad5c-7c1d7e4778dc";
                    voucher.is_allow_group = false;
                    voucher.org_refno = "CK0009";
                    voucher.org_reftype = 2014;
                    voucher.org_reftype_name = "Nhập kho";
                    voucher.refno = "";
                    voucher.total_amount = 1000000.0;
                    voucher.refid = "775b9b77-8e48-4c51-ad5c-7c1d7e4778dc";
                    voucher.branch_id = "00000000-0000-0000-0000-000000000000";
                    voucher.transporter_id = "297ba010-8db0-4585-9318-eeaa9575ed41";
                    voucher.account_object_id = "170b9afe-00b2-4d4b-8596-643cce7d2b87";
                    voucher.display_on_book = 0;
                    voucher.reftype = 2014;// loại chứng từ
                    voucher.reforder = 1628864066092;
                    voucher.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");// Ngày chứng từ "2022-08-13T00:00:00.000+07:00";
                    voucher.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-08-13T00:00:00.000+07:00";
                    voucher.contract_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //"2022-08-13T00:00:00.000+07:00";
                    voucher.in_reforder = "2022-08-13T14:13:16.000+07:00";
                    voucher.is_posted_finance = true;
                    voucher.is_posted_management = false;
                    voucher.is_posted_inventory_book_finance = false;
                    voucher.is_posted_inventory_book_management = false;
                    voucher.is_branch_issued = false;
                    voucher.is_attach_list = false;
                    voucher.is_invoice_replace = false;
                    voucher.total_amount_finance = 1000000.0;
                    voucher.total_amount_management = 0.0;
                    voucher.refno_finance = "CK0009";
                    voucher.contract_code = "1234567";
                    voucher.account_object_name = "NCC03";
                    voucher.contract_owner = "Tổng công ty";
                    voucher.journal_memo = "Vận chuyển giao đại lý";
                    voucher.company_tax_code = "023997545";
                    voucher.transporter_name = "Nguyễn Hoàng Nam";
                    voucher.transport_contract_code = "884658";
                    voucher.transport = "Ô tô";
                    voucher.ref_detail_id = "00000000-0000-0000-0000-000000000000";
                    voucher.quantity = 0.0;
                    voucher.publish_status = 0;
                    voucher.send_email_status = 0;
                    voucher.is_invoice_receipted = false;
                    voucher.invoice_status = 0;
                    voucher.is_invoice_deleted = false;
                    voucher.inv_type_id = 0;
                    voucher.created_date = "2022-08-13T14:14:26.0920928+07:00";
                    voucher.created_by = "Nguyen Quan";
                    voucher.modified_date = "2022-08-13T14:16:15.0380383+07:00";
                    voucher.modified_by = "Nguyen Quan ";
                    voucher.auto_refno = false;
                    voucher.state = 0;
                    lst_voucher.Add(voucher);
                    #endregion

                    List<DictionaryMisa> lst_dictionary = new List<DictionaryMisa>();

                    #region customer
                    DictionaryMisa dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = "170b9afe-00b2-4d4b-8596-643cce7d2b87";
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = true;
                    dic.is_employee = false;
                    dic.inactive = false;
                    dic.agreement_salary = 0.0;
                    dic.salary_coefficient = 0.0;
                    dic.insurance_salary = 0.0;
                    dic.maximize_debt_amount = 0.0;
                    dic.receiptable_debt_amount = 0.0;
                    dic.account_object_code = "NCC04";
                    dic.account_object_name = "NCC03";
                    dic.country = "Việt Nam";
                    dic.is_same_address = false;
                    dic.pay_account = "331";
                    dic.receive_account = "131";
                    dic.closing_amount = 0.0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = "437a3ae2-9981-11ea-af8e-005056890bf4";
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = "2022-07-31T23:16:57.5975976+07:00";
                    dic.created_by = "Nguyen Quan";
                    dic.modified_date = "2022-07-31T23:16:57.5975976+07:00";
                    dic.modified_by = "Nguyen Quan";
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion



                    SaveMisa_13 item = new SaveMisa_13();
                    item.app_id = app_id;
                    item.org_company_code = org_company_code;
                    item.voucher = lst_voucher;
                    item.dictionary = lst_dictionary;

                    #endregion ket thuc body hàm 13 trong tài liệu misa

                    //var json = "{\r\n    \"org_company_code\": \"congtydemoketnoiact\",\r\n    \"app_id\": \"0e0a14cf-9e4b-4af9-875b-c490f34a581b\",\r\n    \"voucher\": [\r\n        {\r\n            \"detail\": [\r\n                {\r\n                    \"ref_detail_id\": \"b61c9ce3-4e2f-46b3-b90c-425fe5ff2848\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 2,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 15.0,\r\n                    \"amount\": 15.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                },\r\n                {\r\n                    \"ref_detail_id\": \"41d9c0b3-bcfe-4306-a3db-0110d6c746b8\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 1,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 12.0,\r\n                    \"amount\": 12.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                }\r\n            ],\r\n            \"voucher_type\": 5,\r\n            \"is_get_new_id\": true,\r\n            \"org_refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"is_allow_group\": false,\r\n            \"org_refno\": \"PT00000687\",\r\n            \"org_reftype\": 1010,\r\n            \"org_reftype_name\": \"Loại CAReceipt\",\r\n            \"refno\": \"\",\r\n            \"act_voucher_type\": 0,\r\n            \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"branch_id\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"employee_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"reason_type_id\": 13,\r\n            \"display_on_book\": 0,\r\n            \"reforder\": 1621330194754,\r\n            \"refdate\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"posted_date\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"is_posted_finance\": true,\r\n            \"is_posted_management\": false,\r\n            \"is_posted_cash_book_finance\": false,\r\n            \"is_posted_cash_book_management\": false,\r\n            \"exchange_rate\": 1.0,\r\n            \"total_amount_oc\": 27.0,\r\n            \"total_amount\": 27.0,\r\n            \"refno_finance\": \"\",\r\n            \"refno_management\": \"\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"account_object_address\": \"Hà Nội\",\r\n            \"account_object_contact_name\": \"Nguyễn Ngà\",\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"journal_memo\": \"Thu tiền của KHOP0002 Nga\",\r\n            \"document_included\": \"2\",\r\n            \"currency_id\": \"VND\",\r\n            \"employee_code\": \"NV00001\",\r\n            \"employee_name\": \"Nguyễn Kiệt\",\r\n            \"ca_audit_refid\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"reftype\": 1010,\r\n            \"created_date\": \"2022-02-06T09:29:54.754754+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-02-06T09:30:02.87087+07:00\",\r\n            \"modified_by\": \"admin\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ],\r\n    \"dictionary\": [\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331.01\",\r\n            \"receive_account\": \"131.01add-alf56\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"created_by\": \"Nguyễn Ngọc Anh\",\r\n            \"modified_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"modified_by\": \"Nguyễn Ngọc Anh\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"organization_unit_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"gender\": 1,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": false,\r\n            \"is_employee\": true,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"NV00001\",\r\n            \"account_object_name\": \"Nguyễn Kiệt\",\r\n            \"organization_unit_name\": \"AN NHIEN JSC 123\",\r\n            \"is_same_address\": false,\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KH00001\",\r\n            \"account_object_name\": \"Nguyễn Chính\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331\",\r\n            \"receive_account\": \"131\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ]\r\n}"; //json_send_misa(item);
                    var json = JsonConvert.SerializeObject(item);
                    if (!System.IO.Directory.Exists("..\\json")) System.IO.Directory.CreateDirectory("..\\json");
                    File.WriteAllText("..\\json\\json.txt", json);

                    HttpClient client = new HttpClient();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Uri uri = new Uri(host.Trim('/') + "/apir/sync/actopen/save");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpRequestMessage httpRequest = new HttpRequestMessage();
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.RequestUri = uri;
                    httpRequest.Headers.Add("X-MISA-AccessToken", access_token);
                    httpRequest.Content = content;

                    var result = client.SendAsync(httpRequest).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var contents = result.Content.ReadAsStringAsync();
                        var jsonResult = contents.Result;
                        DataTable dtData = (DataTable)JsonConvert.DeserializeObject("[" + jsonResult + "]", (typeof(DataTable)));
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            string msg = dtData.Rows[0]["Data"].ToString();
                            bool success = bool.Parse(dtData.Rows[0]["Success"].ToString());

                            if (!success)
                            {
                                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                            {
                                if (!m_data.upd_mqmisa(xxx, long.Parse(r["id"].ToString()), 0, m.ngayhienhanh_server, cbUser.Text.Trim()))
                                {
                                    LibUtility.Utility.MsgBox("Lỗi cập nhập mqmisa!");
                                    return;
                                }

                                r["matrangthai"] = 1;
                                r["trangthai"] = "Đã chuyển";
                                dt.AcceptChanges();
                                //lblNofication.Text = string.Format("Số dòng gửi: {0}/{1}", index++, rows.Length);
                            }
                        }
                    }
                }
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
            }
            catch (Exception ex)
            {
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                MessageBox.Show(ex.Message, "lỗi");
            }
        }

        private void f_phieuxuatkho()
        {
            try
            {
                if (gvList.RowCount < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dữ liệu!");
                    return;
                }

                if (!checkConnectMisa())
                {
                    LibUtility.Utility.MsgBox("Lỗi kết nối hệ thống MISA!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ma_tk_co.Text) || string.IsNullOrWhiteSpace(ma_tk_no.Text))
                {
                    LibUtility.Utility.MsgBox("Vui lòng chọn tài khoản (có/nợ)!");
                    return;
                }
                else
                {
                    DataTable dttk = (DataTable)tk_co.DataSource;
                    DataRow row = dttk.Select("sotk='" + ma_tk_co.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (có) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }

                    dttk = (DataTable)tk_no.DataSource;
                    row = dttk.Select("sotk='" + ma_tk_no.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (nợ) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }
                }

                DataTable dt = (DataTable)grList.DataSource;

                DataRow[] rows = dt.Select("check=true");
                if (rows.Length < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dòng cần chuyển!");
                    return;
                }

                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, true);
                foreach (DataRow r in rows)
                {

                    long id = long.Parse(r["id"].ToString());
                    string mabn = r["mabn"].ToString();
                    string hoten = r["hoten"].ToString();
                    string manguoithu = r["manguoithu"].ToString();
                    string nguoithu = r["nguoithu"].ToString();
                    //xxx = user + m.mmyy(dateTo.Value.ToString("dd/MM/yyyy"));

                    sql = "select a1.stt as sttct, a1.stt, a.id,a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp , a1.mabs,a.maicd,a.chandoan,nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten,b.dvt,round( a1.soluong,3) as soluong,round(a1.dongia,2) as dongia ,round(round( a1.soluong * a1.dongia,2),0) as sotien ,f.doituong, round(a1.bhyttra,2) as bhyttra,round(round (a1.soluong * a1.dongia,2) -round( a1.bhyttra,2),0) as bntra ,a1.mavp,g.tamung,a1.madoituong, nvl(e.traituyen, 0) as traituyen,a1.id,to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra, g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,0 as loai,0 as done,(nvl(a1.giamua, a1.dongia)) as giamua,'' lydo,g.bhytghichu,0 as dtchitra,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan   as hinhthucthanhtoan ,0 as idtrongoi,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , b.dichvu,b.thuong,0 as chuongtrinh,1 as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp,nvl(a1.mien, 0) - nvl(a1.sotienngansach, 0) - nvl(a1.mienchitiet, 0) as mienct,nvl(a1.thuocxuatban, 0) as thuocxuatban,a1.taitro,nvl(a1.truythu, 0) as truythu,nvl(a1.mienchitiet, 0) as mienchitiet,a1.sttt,a1.THUE,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) -round( a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc ,g.thanhtoan idhinhthucthanhtoan ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join  xxx.v_ttrvct a1 on a.id = a1.id ";
                    sql += "inner join  " + user + ".v_giavp b on a1.mavp = b.id  ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id  ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;
                    sql += " union all ";
                    sql += "select a1.stt as sttct, a1.stt,a.id, a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp, a1.mabs,a.maicd,a.chandoan, nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten || ' ' || b.hamluong as ten,b.dang as dvt,round( a1.soluong,3) soluong , round(a1.dongia,2) as dongia,round( round(a1.soluong * a1.dongia,2),0) as sotien ,f.doituong,round(a1.bhyttra,2) as bhyttra ,round(round(a1.soluong * a1.dongia,2)  - round(a1.bhyttra,2) ,0) as bntra , a1.mavp,g.tamung,a1.madoituong,nvl(e.traituyen, 0) as traituyen,a1.id, to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra,g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,1 as loai,0 as done,nvl(a1.giamua, a1.dongia) as giamua,'' lydo,g.bhytghichu,b.dtchitra ,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan  as hinhthucthanhtoan ,0 as idtrongoi ,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , 0 dichvu,0 as thuong,b.chuongtrinh,nvl(b.phamvi, 1) as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp, round(nvl(a1.mien, 0),2) - round(nvl(a1.sotienngansach, 0),2) - round(nvl(a1.mienchitiet, 0),2) as mienct  ,nvl(a1.thuocxuatban, 0) as thuocxuatban,round(a1.taitro,2) as taitro,nvl(a1.truythu, 0) as truythu,round(nvl(a1.mienchitiet, 0),2) as mienchitiet ,a1.sttt, 0 as thue,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) - round(a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc,g.thanhtoan idhinhthuocthanhtoan  ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join xxx.v_ttrvct a1 on a.id = a1.id  ";
                    sql += "inner join " + user + ".d_dmbd b on a1.mavp = b.id ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id  ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id ";
                    sql += "left join xxx.d_theodoi td on td.id=a1.sttt  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;

                    DataTable dtct = m.get_data_mmyy(sql, dateTo.Value.ToString("dd/MM/yyyy"), dateFrom.Value.ToString("dd/MM/yyyy"), false).Tables[0];

                    #region bắt đầu body hàm 13 trong tài liệu misa

                    List<DetailMisa> lst_detail = new List<DetailMisa>();
                    List<in_outward> lst_in_outward = new List<in_outward>();
                    List<sa_invoice> lst_sa_invoice = new List<sa_invoice>();
                    string refid = Guid.NewGuid().ToString();
                    string account_object_id = Guid.NewGuid().ToString();
                    string employee_id = Guid.NewGuid().ToString();

                    foreach (DataRow r1 in dtct.Rows)
                    {
                        DetailMisa detail = new DetailMisa();
                        detail.ref_detail_id = account_object_id;
                        detail.refid = refid;
                        detail.inventory_item_id = "72c6e284-f6b9-4cef-a0fc-9e24d187a134";
                        detail.stock_id = "a587b192-ecd8-4711-86aa-72207c86ed7d";
                        detail.unit_id = "81cebf94-9cb9-4b20-880a-a01fc0f6d850";
                        detail.account_object_id = "1829450f-012c-4415-94fb-09b0208c66a6";
                        detail.organization_unit_id = "aa883e97-c903-44f5-8dd3-75fb08755b38";
                        detail.main_unit_id = "81cebf94-9cb9-4b20-880a-a01fc0f6d850";
                        detail.sort_order = 2;
                        detail.inventory_resale_type_id = 0;
                        detail.is_un_update_outward_price = false;
                        detail.un_resonable_cost = false;
                        detail.is_promotion = false;
                        detail.quantity = 1.0;
                        detail.unit_price_finance = 15000000.0;
                        detail.unit_price_management = 0.0;
                        detail.amount_finance = 15000000.0;
                        detail.amount_management = 0.0;
                        detail.main_unit_price_finance = 15000000.0;
                        detail.main_unit_price_management = 0.0;
                        detail.main_convert_rate = 1.0;
                        detail.main_quantity = 1.0;
                        detail.sale_price = 0.0;
                        detail.sale_amount = 0.0;
                        detail.description = "ADVANCE 4T  ULTRA SCOOTER  5W40  1L";
                        detail.debit_account = ma_tk_no.Text; //"1111";  //TK nợ (tạo tham số kế toán bv)
                        detail.credit_account = ma_tk_co.Text; //"131";  //TK có (tạo tham số kế toán bv)
                        detail.exchange_rate_operator = "*";
                        detail.account_object_name = "CÔNG TY CỔ PHẦN MISA";
                        detail.account_object_code = "KH00001402";
                        detail.inventory_item_code = "0115101";
                        detail.organization_unit_code = "vbcong123";
                        detail.unit_name = "Chai";
                        detail.organization_unit_name = "Phòng KD";
                        detail.stock_code = "00001";
                        detail.main_unit_name = "Chai";
                        detail.inventory_item_name = "ADVANCE 4T  ULTRA SCOOTER  5W40  1L";
                        detail.remain_quantity = 0.0;
                        detail.unit_price = 0.0;
                        detail.is_unit_price_after_tax = false;
                        detail.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); // "0001-01-01T00:00:00.000+07:00";
                        detail.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); //ngày chứng từ"0001-01-01T00:00:00.000+07:00";
                        detail.reftype = 0;
                        detail.total_amount_finance = 0.0;
                        detail.vat_rate = 0.0;
                        detail.stock_name = "333";
                        detail.account_name = "Giá mua hàng hóa";
                        detail.is_follow_serial_number = false;
                        detail.is_allow_duplicate_serial_number = false;
                        detail.quantity_delivered = 0.0;
                        detail.quantity_remain = 0.0;
                        detail.is_description = false;
                        detail.is_description_import = false;
                        detail.is_promotion_import = false;
                        detail.un_resonable_cost_import = false;
                        detail.main_quantity_remain = 0.0;
                        
                        detail.state = 0;
                        lst_detail.Add(detail);
                    }

                  


                    List<VoucherMisa_13> lst_voucher = new List<VoucherMisa_13>(); // phieu thu tien 

                    #region voucher
                    VoucherMisa_13 voucher = new VoucherMisa_13();
                    voucher.detail = lst_detail;
                    voucher.voucher_type = 8;
                    voucher.is_get_new_id = true;
                    voucher.org_refid = "c9e07a33-ad7e-49ec-9697-be1ca51215d8";
                    voucher.is_allow_group = false;
                    voucher.org_refno = "XK00083";
                    voucher.org_reftype = 2020;
                    voucher.org_reftype_name = "Phiêú xuất kho";
                    voucher.refno = "";
                    voucher.act_voucher_type = 0;
                    voucher.refid = "c9e07a33-ad7e-49ec-9697-be1ca51215d8";
                    voucher.account_object_id = "1829450f-012c-4415-94fb-09b0208c66a6";
                    voucher.employee_id = "a7ced9ab-5c5e-42b7-8e42-2c90d207cc28";
                    voucher.branch_id = "00000000-0000-0000-0000-000000000000";
                    voucher.display_on_book = 0;
                    voucher.reforder = 1626198032410;
                    voucher.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");  //NGày chứng từ "2022-07-13T00:00:00.000+07:00";
                    voucher.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.in_reforder = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
                    voucher.is_posted_finance = false;
                    voucher.is_posted_management = false;
                    voucher.is_posted_inventory_book_finance = false;
                    voucher.is_posted_inventory_book_management = false;
                    voucher.is_branch_issued = false;
                    voucher.is_sale_with_outward = false;
                    voucher.is_invoice_replace = false;
                    voucher.total_amount_finance = 20000000.0;
                    voucher.total_amount_management = 0.0;
                    voucher.refno_finance = "";
                    voucher.refno_management = "";
                    voucher.account_object_name = "CÔNG TY CỔ PHẦN MISA";
                    voucher.account_object_address = "Tầng 9; tòa nhà Technosoft; phố Duy Tân; Phường Dịch Vọng Hậu; Quận Cầu Giấy; Thành phố Hà Nội; Việt Nam";
                    voucher.contact_name = "TVPhi";
                    voucher.journal_memo = "Xuất kho bán hàng cho CÔNG TY CỔ PHẦN MISA";
                    voucher.reftype = 2020;
                    voucher.employee_name = "Trần Thị Hai";
                    voucher.payment_term_id = "00000000-0000-0000-0000-000000000000";
                    voucher.due_time = 0;
                    voucher.is_executed = false;
                    voucher.employee_code = "14020251";
                    voucher.reftype_name = "Xuất kho bán hàng";
                    voucher.publish_status = 0;
                    voucher.is_invoice_deleted = false;
                    voucher.is_invoice_receipted = false;
                    voucher.account_object_code = "KH00001402";
                    voucher.created_date = "2022-07-13T17:40:32.4104107+07:00";
                    voucher.created_by = "Hoàng Viết Đức";
                    voucher.modified_date = "2022-07-13T17:40:32.4104107+07:00";
                    voucher.auto_refno = false;
                    voucher.state = 0;
                    lst_voucher.Add(voucher);
                   
                    #endregion

                    List<DictionaryMisa> lst_dictionary = new List<DictionaryMisa>();

                    #region customer
                    DictionaryMisa dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = "1829450f-012c-4415-94fb-09b0208c66a6";
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = true;
                    dic.is_employee = false;
                    dic.inactive = false;
                    dic.agreement_salary = 0.0;
                    dic.salary_coefficient = 0.0;
                    dic.insurance_salary = 0.0;
                    dic.maximize_debt_amount = 0.0;
                    dic.debit_account = ma_tk_no.Text; //"1111";  //TK nợ (tạo tham số kế toán bv)
                    dic.credit_account = ma_tk_co.Text;
                    dic.receiptable_debt_amount = 0.0;
                    dic.account_object_code = "KH00001402";
                    dic.account_object_name = "CÔNG TY CỔ PHẦN MISA";
                    dic.address = "Tầng 9; tòa nhà Technosoft; phố Duy Tân; Phường Dịch Vọng Hậu; Quận Cầu Giấy; Thành phố Hà Nội; Việt Nam";
                    dic.einvoice_contact_name = "TVPhi";
                    dic.legal_representative = "Lữ Thành Long";
                    dic.district = "Quận Cầu Giấy";
                    dic.ward_or_commune = "Phường Dịch Vọng Hậu";
                    dic.prefix = "Anh";
                    dic.contact_name = "TVPhi";
                    dic.country = "Việt Nam";
                    dic.province_or_city = "Hà Nội";
                    dic.company_tax_code = "010124315099";
                    dic.tel = "034987559999";
                    dic.is_same_address = false;
                    dic.account_object_group_id_list = "02ed5747-710f-4d69-8544-1b5f1243c788";
                    dic.account_object_group_code_list = "_001.1";
                    dic.account_object_group_name_list = "_001.1";
                    dic.account_object_group_misa_code_list = ";/00002/;";
                    dic.pay_account = "3318";
                    dic.receive_account = "131";
                    dic.closing_amount = 0.0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = "aa883e97-c903-44f5-8dd3-75fb08755b38";
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = "2022-07-13T17:39:33.9899891+07:00";
                    dic.created_by = "Hoàng Viết Đức";
                    dic.modified_date = "2022-07-13T17:39:33.9899891+07:00";
                    dic.modified_by = "Hoàng Viết Đức";
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion

                    SaveMisa_13 item = new SaveMisa_13();
                    item.app_id = app_id;
                    item.org_company_code = org_company_code;
                    item.voucher = lst_voucher;
                    item.dictionary = lst_dictionary;

                    #endregion ket thuc body hàm 13 trong tài liệu misa

                    //var json = "{\r\n    \"org_company_code\": \"congtydemoketnoiact\",\r\n    \"app_id\": \"0e0a14cf-9e4b-4af9-875b-c490f34a581b\",\r\n    \"voucher\": [\r\n        {\r\n            \"detail\": [\r\n                {\r\n                    \"ref_detail_id\": \"b61c9ce3-4e2f-46b3-b90c-425fe5ff2848\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 2,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 15.0,\r\n                    \"amount\": 15.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                },\r\n                {\r\n                    \"ref_detail_id\": \"41d9c0b3-bcfe-4306-a3db-0110d6c746b8\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 1,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 12.0,\r\n                    \"amount\": 12.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                }\r\n            ],\r\n            \"voucher_type\": 5,\r\n            \"is_get_new_id\": true,\r\n            \"org_refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"is_allow_group\": false,\r\n            \"org_refno\": \"PT00000687\",\r\n            \"org_reftype\": 1010,\r\n            \"org_reftype_name\": \"Loại CAReceipt\",\r\n            \"refno\": \"\",\r\n            \"act_voucher_type\": 0,\r\n            \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"branch_id\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"employee_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"reason_type_id\": 13,\r\n            \"display_on_book\": 0,\r\n            \"reforder\": 1621330194754,\r\n            \"refdate\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"posted_date\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"is_posted_finance\": true,\r\n            \"is_posted_management\": false,\r\n            \"is_posted_cash_book_finance\": false,\r\n            \"is_posted_cash_book_management\": false,\r\n            \"exchange_rate\": 1.0,\r\n            \"total_amount_oc\": 27.0,\r\n            \"total_amount\": 27.0,\r\n            \"refno_finance\": \"\",\r\n            \"refno_management\": \"\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"account_object_address\": \"Hà Nội\",\r\n            \"account_object_contact_name\": \"Nguyễn Ngà\",\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"journal_memo\": \"Thu tiền của KHOP0002 Nga\",\r\n            \"document_included\": \"2\",\r\n            \"currency_id\": \"VND\",\r\n            \"employee_code\": \"NV00001\",\r\n            \"employee_name\": \"Nguyễn Kiệt\",\r\n            \"ca_audit_refid\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"reftype\": 1010,\r\n            \"created_date\": \"2022-02-06T09:29:54.754754+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-02-06T09:30:02.87087+07:00\",\r\n            \"modified_by\": \"admin\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ],\r\n    \"dictionary\": [\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331.01\",\r\n            \"receive_account\": \"131.01add-alf56\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"created_by\": \"Nguyễn Ngọc Anh\",\r\n            \"modified_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"modified_by\": \"Nguyễn Ngọc Anh\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"organization_unit_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"gender\": 1,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": false,\r\n            \"is_employee\": true,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"NV00001\",\r\n            \"account_object_name\": \"Nguyễn Kiệt\",\r\n            \"organization_unit_name\": \"AN NHIEN JSC 123\",\r\n            \"is_same_address\": false,\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KH00001\",\r\n            \"account_object_name\": \"Nguyễn Chính\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331\",\r\n            \"receive_account\": \"131\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ]\r\n}"; //json_send_misa(item);
                    var json = JsonConvert.SerializeObject(item);
                    if (!System.IO.Directory.Exists("..\\json")) System.IO.Directory.CreateDirectory("..\\json");
                    File.WriteAllText("..\\json\\json.txt", json);

                    HttpClient client = new HttpClient();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Uri uri = new Uri(host.Trim('/') + "/apir/sync/actopen/save");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpRequestMessage httpRequest = new HttpRequestMessage();
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.RequestUri = uri;
                    httpRequest.Headers.Add("X-MISA-AccessToken", access_token);
                    httpRequest.Content = content;

                    var result = client.SendAsync(httpRequest).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var contents = result.Content.ReadAsStringAsync();
                        var jsonResult = contents.Result;
                        DataTable dtData = (DataTable)JsonConvert.DeserializeObject("[" + jsonResult + "]", (typeof(DataTable)));
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            string msg = dtData.Rows[0]["Data"].ToString();
                            bool success = bool.Parse(dtData.Rows[0]["Success"].ToString());

                            if (!success)
                            {
                                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                            {
                                if (!m_data.upd_mqmisa(xxx, long.Parse(r["id"].ToString()), 0, m.ngayhienhanh_server, cbUser.Text.Trim()))
                                {
                                    LibUtility.Utility.MsgBox("Lỗi cập nhập mqmisa!");
                                    return;
                                }

                                r["matrangthai"] = 1;
                                r["trangthai"] = "Đã chuyển";
                                dt.AcceptChanges();
                                //lblNofication.Text = string.Format("Số dòng gửi: {0}/{1}", index++, rows.Length);
                            }
                        }
                    }
                }
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
            }
            catch (Exception ex)
            {
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                MessageBox.Show(ex.Message, "lỗi");
            }
        }
        private void f_phieuchuyenkho()
        {
            try
            {
                if (gvList.RowCount < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dữ liệu!");
                    return;
                }

                if (!checkConnectMisa())
                {
                    LibUtility.Utility.MsgBox("Lỗi kết nối hệ thống MISA!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ma_tk_co.Text) || string.IsNullOrWhiteSpace(ma_tk_no.Text))
                {
                    LibUtility.Utility.MsgBox("Vui lòng chọn tài khoản (có/nợ)!");
                    return;
                }
                else
                {
                    DataTable dttk = (DataTable)tk_co.DataSource;
                    DataRow row = dttk.Select("sotk='" + ma_tk_co.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (có) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }

                    dttk = (DataTable)tk_no.DataSource;
                    row = dttk.Select("sotk='" + ma_tk_no.Text + "'").FirstOrDefault();

                    if (row == null)
                    {
                        LibUtility.Utility.MsgBox("Số tài khoản (nợ) không tồn tại, vui lòng kiểm tra lại !");
                        return;
                    }
                }

                DataTable dt = (DataTable)grList.DataSource;

                DataRow[] rows = dt.Select("check=true");
                if (rows.Length < 1)
                {
                    LibUtility.Utility.MsgBox("Không tìm thấy dòng cần chuyển!");
                    return;
                }

                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, true);
                foreach (DataRow r in rows)
                {

                    long id = long.Parse(r["id"].ToString());
                    string mabn = r["mabn"].ToString();
                    string hoten = r["hoten"].ToString();
                    string manguoithu = r["manguoithu"].ToString();
                    string nguoithu = r["nguoithu"].ToString();
                    //xxx = user + m.mmyy(dateTo.Value.ToString("dd/MM/yyyy"));

                    sql = "select a1.stt as sttct, a1.stt, a.id,a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp , a1.mabs,a.maicd,a.chandoan,nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten,b.dvt,round( a1.soluong,3) as soluong,round(a1.dongia,2) as dongia ,round(round( a1.soluong * a1.dongia,2),0) as sotien ,f.doituong, round(a1.bhyttra,2) as bhyttra,round(round (a1.soluong * a1.dongia,2) -round( a1.bhyttra,2),0) as bntra ,a1.mavp,g.tamung,a1.madoituong, nvl(e.traituyen, 0) as traituyen,a1.id,to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra, g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,0 as loai,0 as done,(nvl(a1.giamua, a1.dongia)) as giamua,'' lydo,g.bhytghichu,0 as dtchitra,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan   as hinhthucthanhtoan ,0 as idtrongoi,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , b.dichvu,b.thuong,0 as chuongtrinh,1 as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp,nvl(a1.mien, 0) - nvl(a1.sotienngansach, 0) - nvl(a1.mienchitiet, 0) as mienct,nvl(a1.thuocxuatban, 0) as thuocxuatban,a1.taitro,nvl(a1.truythu, 0) as truythu,nvl(a1.mienchitiet, 0) as mienchitiet,a1.sttt,a1.THUE,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) -round( a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc ,g.thanhtoan idhinhthucthanhtoan ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join  xxx.v_ttrvct a1 on a.id = a1.id ";
                    sql += "inner join  " + user + ".v_giavp b on a1.mavp = b.id  ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id  ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;
                    sql += " union all ";
                    sql += "select a1.stt as sttct, a1.stt,a.id, a.mabn,a.mavaovien,a.maql,0 as loaiba,to_char(g.ngay, 'dd/mm/yyyy') as ngaythu,a1.makp, a1.mabs,a.maicd,a.chandoan, nvl(a1.sothe, e.sothe) as sothe,nvl(e.maphu, 1) as maphu,e.mabv,b.ma,b.ten || ' ' || b.hamluong as ten,b.dang as dvt,round( a1.soluong,3) soluong , round(a1.dongia,2) as dongia,round( round(a1.soluong * a1.dongia,2),0) as sotien ,f.doituong,round(a1.bhyttra,2) as bhyttra ,round(round(a1.soluong * a1.dongia,2)  - round(a1.bhyttra,2) ,0) as bntra , a1.mavp,g.tamung,a1.madoituong,nvl(e.traituyen, 0) as traituyen,a1.id, to_char(a.ngayra, 'dd/mm/yyyy hh24:mi') as ngayra,g.mien,to_char(a1.ngay, 'dd/mm/yyyy') as ngay,a.maql as idkhoa,1 as loai,0 as done,nvl(a1.giamua, a1.dongia) as giamua,'' lydo,g.bhytghichu,b.dtchitra ,case when a1.soluong* a1.dongia = 0 then 0 else decode(a1.tlchitra, 0, (a1.bhyttra / (a1.soluong * a1.dongia) * 100), a1.tlchitra) end as tlchitra,g.idtonghop,g.sophieu,b.vat,a.mabs as mabsrv,g.idthu  ,0 as loaidt  ,g.loaibn,b.bhyt,g.thanhtoan  as hinhthucthanhtoan ,0 as idtrongoi ,a1.ngansach,round(a1.sotienngansach,2) as sotienngansach,g.HOADONDICHVU , 0 dichvu,0 as thuong,b.chuongtrinh,nvl(b.phamvi, 1) as phamvi ,a1.DONGIABH ,a1.DONGIABV ,a1.DONGIADV,a1.IDCHENHLECH,a1.idct,h1.tenkp,h2.ten as tennhomvp, round(nvl(a1.mien, 0),2) - round(nvl(a1.sotienngansach, 0),2) - round(nvl(a1.mienchitiet, 0),2) as mienct  ,nvl(a1.thuocxuatban, 0) as thuocxuatban,round(a1.taitro,2) as taitro,nvl(a1.truythu, 0) as truythu,round(nvl(a1.mienchitiet, 0),2) as mienchitiet ,a1.sttt, 0 as thue,a1.giatruocthue,g.quyenso as idquyenso,s.sohieu as quyenso,g.sobienlai as sobienlai,s.sohieu||'/'||g.sobienlai as quyensosobienlai   ,round( round(a1.soluong*a1.dongia,2) - round(a1.bhyttra,2) -round(nvl(a1.mien, 0),2),0)  as thanhtoan ,tt.ten hinhthuc,g.thanhtoan idhinhthuocthanhtoan  ";
                    sql += "from xxx.v_ttrvds a inner join xxx.v_ttrvll g on a.id = g.id  ";
                    sql += "inner join xxx.v_ttrvct a1 on a.id = a1.id  ";
                    sql += "inner join " + user + ".d_dmbd b on a1.mavp = b.id ";
                    sql += "left join xxx.v_ttrvbhyt e on a.id = e.id ";
                    sql += "left join " + user + ".doituong f on a1.madoituong = f.madoituong ";
                    sql += "left join " + user + ".v_quyenso s on g.quyenso = s.id  ";
                    sql += "left join " + user + ".btdbn h on a.mabn = h.mabn ";
                    sql += "left join " + user + ".btdkp_bv h1 on a1.makp = h1.makp ";
                    sql += "left join " + user + ".view_nhombhyt h2 on a1.mavp = h2.id ";
                    sql += "left join xxx.d_theodoi td on td.id=a1.sttt  ";
                    sql += "inner join  " + user + ".dmthanhtoan tt on tt.id = g.thanhtoan  ";
                    sql += "where a.id=" + id;

                    DataTable dtct = m.get_data_mmyy(sql, dateTo.Value.ToString("dd/MM/yyyy"), dateFrom.Value.ToString("dd/MM/yyyy"), false).Tables[0];

                    #region bắt đầu body hàm 13 trong tài liệu misa

                    List<DetailMisa> lst_detail = new List<DetailMisa>();
                    List<in_outward> lst_in_outward = new List<in_outward>();
                    List<sa_invoice> lst_sa_invoice = new List<sa_invoice>();
                    string refid = Guid.NewGuid().ToString();
                    string account_object_id = Guid.NewGuid().ToString();
                    string employee_id = Guid.NewGuid().ToString();

                    foreach (DataRow r1 in dtct.Rows)
                    {
                        DetailMisa detail = new DetailMisa();
                        detail.ref_detail_id = Guid.NewGuid().ToString();
                       
                        detail.stock_id = "65834d8b-a73b-4e55-9fdf-6eecf61fb6eb";
                        detail.stock_code = "KHO1";
                        detail.stock_name = "Kho hàng hóa";
                        detail.refid = refid;
                        detail.account_object_id = account_object_id;
                        detail.inventory_item_id = "ff874c8f-8686-4f84-83c8-9d1de7365a2e";
                        detail.from_stock_id = "65834d8b-a73b-4e55-9fdf-6eecf61fb6eb";
                        detail.to_stock_id = "99ce3f40-242e-4418-bb68-808c1792007b";
                        detail.unit_id = "81cebf94-9cb9-4b20-880a-a01fc0f6d850";
                        detail.organization_unit_id = "bdb4458c-30fd-4e9d-a34a-9849803fe690";
                        detail.main_unit_id = "81cebf94-9cb9-4b20-880a-a01fc0f6d850";
                        detail.sort_order = 1;
                        detail.inventory_resale_type_id = 0;
                        detail.un_resonable_cost = false;
                        detail.quantity = 2.0;
                        detail.sale_price = 220000.0;
                        detail.sale_amount = 440000.0;
                        detail.unit_price_finance = 500000.0;
                        detail.amount_finance = 1000000.0;
                        detail.unit_price_management = 0.0;
                        detail.amount_management = 1000000.0;
                        detail.main_unit_price_finance = 500000.0;
                        detail.main_unit_price_management = 0.0;
                        detail.main_convert_rate = 1.0;
                        detail.main_quantity = 2.0;
                        detail.description = "Vitamin PEDIA KID";
                        detail.debit_account = "152";
                        detail.credit_account = "156";
                        detail.exchange_rate_operator = "*";
                        detail.inventory_item_code = "PEDIA KID";
                        detail.organization_unit_code = "CN200";
                        detail.unit_name = "Chai";
                        detail.organization_unit_name = "CN200";
                        detail.main_unit_name = "Chai";
                        detail.from_stock_code = "KHO1";
                        detail.from_stock_name = "Kho hàng hóa";
                        detail.to_stock_code = "Kho2";
                        detail.to_stock_name = "Kho nhập";
                        detail.is_description = false;
                        detail.inventory_item_name = "Vitamin PEDIA KID";
                        detail.is_follow_serial_number = false;
                        detail.is_allow_duplicate_serial_number = false;
                        detail.reftype = 0;
                        detail.sale_price1 = 0.0;
                        detail.is_description_import = false;
                        detail.is_promotion_import = false;
                        detail.un_resonable_cost_import = false;
                        detail.state = 0;
                        lst_detail.Add(detail);
                    }

                   
                    List<VoucherMisa_13> lst_voucher = new List<VoucherMisa_13>(); 
                    #region voucher
                    VoucherMisa_13 voucher = new VoucherMisa_13();
                    voucher.detail = lst_detail;
                    //  voucher.voucher_type = 9;                
                    voucher.is_get_new_id = true;
                    voucher.org_refid = "775b9b77-8e48-4c51-ad5c-7c1d7e4778dc";
                    voucher.is_allow_group = false;
                    voucher.org_refno = "CK0009";
                    voucher.org_reftype = 2031;
                    voucher.org_reftype_name = "Chuyển kho";
                    voucher.refno = "";
                    voucher.total_amount = 1000000.0;
                    voucher.refid = "775b9b77-8e48-4c51-ad5c-7c1d7e4778dc";
                    voucher.branch_id = "00000000-0000-0000-0000-000000000000";
                    voucher.transporter_id = "297ba010-8db0-4585-9318-eeaa9575ed41";
                    voucher.account_object_id = "170b9afe-00b2-4d4b-8596-643cce7d2b87";
                    voucher.display_on_book = 0;
                    voucher.reftype = 2031;
                    voucher.reforder = 1628864066092;
                    voucher.refdate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); // Ngày chứng từ "2022-08-13T00:00:00.000+07:00";
                    voucher.posted_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");  // Ngày chứng từ "2022-08-13T00:00:00.000+07:00";
                    voucher.contract_date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz"); // Ngày chứng từ  "2022-08-13T00:00:00.000+07:00";
                    voucher.in_reforder = "2022-08-13T14:13:16.000+07:00";
                    voucher.is_posted_finance = true;
                    voucher.is_posted_management = false;
                    voucher.is_posted_inventory_book_finance = false;
                    voucher.is_posted_inventory_book_management = false;
                    voucher.is_branch_issued = false;
                    voucher.is_attach_list = false;
                    voucher.is_invoice_replace = false;
                    voucher.total_amount_finance = 1000000.0;
                    voucher.total_amount_management = 0.0;
                    voucher.refno_finance = "CK0009";
                    voucher.contract_code = "1234567";
                    voucher.account_object_name = "NCC03";
                    voucher.contract_owner = "Tổng công ty";
                    voucher.journal_memo = "Vận chuyển giao đại lý";
                    voucher.company_tax_code = "023997545";
                    voucher.transporter_name = "Nguyễn Hoàng Nam";
                    voucher.transport_contract_code = "884658";
                    voucher.transport = "Ô tô";
                    voucher.ref_detail_id = "00000000-0000-0000-0000-000000000000";
                    voucher.quantity = 0.0;
                    voucher.publish_status = 0;
                    voucher.send_email_status = 0;
                    voucher.is_invoice_receipted = false;
                    voucher.invoice_status = 0;
                    voucher.is_invoice_deleted = false;
                    voucher.inv_type_id = 0;
                    voucher.created_date = "2022-08-13T14:14:26.0920928+07:00";
                    voucher.created_by = "Nguyen Quan";
                    voucher.modified_date = "2022-08-13T14:16:15.0380383+07:00";
                    voucher.modified_by = "Nguyen Quan ";
                    voucher.auto_refno = false;
                    voucher.state = 0;
                    lst_voucher.Add(voucher);
                    #endregion

                    List<DictionaryMisa> lst_dictionary = new List<DictionaryMisa>();

                    #region customer
                    DictionaryMisa dic = new DictionaryMisa();
                    dic.dictionary_type = 1;
                    dic.account_object_id = "170b9afe-00b2-4d4b-8596-643cce7d2b87";
                    dic.due_time = 0;
                    dic.account_object_type = 0;
                    dic.is_vendor = false;
                    dic.is_customer = true;
                    dic.is_employee = false;
                    dic.inactive = false;
                    dic.agreement_salary = 0.0;
                    dic.salary_coefficient = 0.0;
                    dic.insurance_salary = 0.0;
                    dic.maximize_debt_amount = 0.0;
                    dic.receiptable_debt_amount = 0.0;
                    dic.account_object_code = "NCC04";
                    dic.account_object_name = "NCC03";
                    dic.country = "Việt Nam";
                    dic.is_same_address = false;
                    dic.pay_account = "331";
                    dic.receive_account = "131";
                    dic.closing_amount = 0.0;
                    dic.reftype = 0;
                    dic.reftype_category = 0;
                    dic.branch_id = "437a3ae2-9981-11ea-af8e-005056890bf4";
                    dic.is_convert = false;
                    dic.is_group = false;
                    dic.is_remind_debt = true;
                    dic.excel_row_index = 0;
                    dic.is_valid = false;
                    dic.created_date = "2022-07-31T23:16:57.5975976+07:00";
                    dic.created_by = "Nguyen Quan";
                    dic.modified_date = "2022-07-31T23:16:57.5975976+07:00";
                    dic.modified_by = "Nguyen Quan";
                    dic.auto_refno = false;
                    dic.state = 0;
                    lst_dictionary.Add(dic);
                    #endregion

                   

                    SaveMisa_13 item = new SaveMisa_13();
                    item.app_id = app_id;
                    item.org_company_code = org_company_code;
                    item.voucher = lst_voucher;
                    item.dictionary = lst_dictionary;

                    #endregion ket thuc body hàm 13 trong tài liệu misa

                    //var json = "{\r\n    \"org_company_code\": \"congtydemoketnoiact\",\r\n    \"app_id\": \"0e0a14cf-9e4b-4af9-875b-c490f34a581b\",\r\n    \"voucher\": [\r\n        {\r\n            \"detail\": [\r\n                {\r\n                    \"ref_detail_id\": \"b61c9ce3-4e2f-46b3-b90c-425fe5ff2848\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 2,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 15.0,\r\n                    \"amount\": 15.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                },\r\n                {\r\n                    \"ref_detail_id\": \"41d9c0b3-bcfe-4306-a3db-0110d6c746b8\",\r\n                    \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n                    \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n                    \"sort_order\": 1,\r\n                    \"un_resonable_cost\": false,\r\n                    \"amount_oc\": 12.0,\r\n                    \"amount\": 12.0,\r\n                    \"cash_out_amount_finance\": 0.0,\r\n                    \"cash_out_diff_amount_finance\": 0.0,\r\n                    \"cash_out_amount_management\": 0.0,\r\n                    \"cash_out_diff_amount_management\": 0.0,\r\n                    \"cash_out_exchange_rate_finance\": 0.0,\r\n                    \"cash_out_exchange_rate_management\": 0.0,\r\n                    \"description\": \"Thu tiền của KHOP0002 Nga\",\r\n                    \"debit_account\": \"1111\",\r\n                    \"credit_account\": \"1112\",\r\n                    \"account_object_code\": \"KH00001\",\r\n                    \"state\": 0\r\n                }\r\n            ],\r\n            \"voucher_type\": 5,\r\n            \"is_get_new_id\": true,\r\n            \"org_refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"is_allow_group\": false,\r\n            \"org_refno\": \"PT00000687\",\r\n            \"org_reftype\": 1010,\r\n            \"org_reftype_name\": \"Loại CAReceipt\",\r\n            \"refno\": \"\",\r\n            \"act_voucher_type\": 0,\r\n            \"refid\": \"6ad3f846-0ec0-4ddd-adfd-5b09d8695739\",\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"branch_id\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"employee_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"reason_type_id\": 13,\r\n            \"display_on_book\": 0,\r\n            \"reforder\": 1621330194754,\r\n            \"refdate\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"posted_date\": \"2022-02-06T00:00:00.000+07:00\",\r\n            \"is_posted_finance\": true,\r\n            \"is_posted_management\": false,\r\n            \"is_posted_cash_book_finance\": false,\r\n            \"is_posted_cash_book_management\": false,\r\n            \"exchange_rate\": 1.0,\r\n            \"total_amount_oc\": 27.0,\r\n            \"total_amount\": 27.0,\r\n            \"refno_finance\": \"\",\r\n            \"refno_management\": \"\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"account_object_address\": \"Hà Nội\",\r\n            \"account_object_contact_name\": \"Nguyễn Ngà\",\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"journal_memo\": \"Thu tiền của KHOP0002 Nga\",\r\n            \"document_included\": \"2\",\r\n            \"currency_id\": \"VND\",\r\n            \"employee_code\": \"NV00001\",\r\n            \"employee_name\": \"Nguyễn Kiệt\",\r\n            \"ca_audit_refid\": \"00000000-0000-0000-0000-000000000000\",\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"reftype\": 1010,\r\n            \"created_date\": \"2022-02-06T09:29:54.754754+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-02-06T09:30:02.87087+07:00\",\r\n            \"modified_by\": \"admin\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ],\r\n    \"dictionary\": [\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"104c49f2-6828-4dea-88e9-08f1fb027701\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KHOP0002\",\r\n            \"account_object_name\": \"KHOP0002 Nga\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331.01\",\r\n            \"receive_account\": \"131.01add-alf56\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"created_by\": \"Nguyễn Ngọc Anh\",\r\n            \"modified_date\": \"2022-04-09T07:49:45.000+07:00\",\r\n            \"modified_by\": \"Nguyễn Ngọc Anh\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"507abe24-520e-47ad-9c39-feed3cc8ba02\",\r\n            \"organization_unit_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"gender\": 1,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": false,\r\n            \"is_employee\": true,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"NV00001\",\r\n            \"account_object_name\": \"Nguyễn Kiệt\",\r\n            \"organization_unit_name\": \"AN NHIEN JSC 123\",\r\n            \"is_same_address\": false,\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:28:53.5535531+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        },\r\n        {\r\n            \"dictionary_type\": 1,\r\n            \"account_object_id\": \"8c9b3be3-618b-4cf4-b640-fd1fd1e67594\",\r\n            \"due_time\": 0,\r\n            \"account_object_type\": 0,\r\n            \"is_vendor\": false,\r\n            \"is_customer\": true,\r\n            \"is_employee\": false,\r\n            \"inactive\": false,\r\n            \"agreement_salary\": 0.0,\r\n            \"salary_coefficient\": 0.0,\r\n            \"insurance_salary\": 0.0,\r\n            \"maximize_debt_amount\": 0.0,\r\n            \"receiptable_debt_amount\": 0.0,\r\n            \"account_object_code\": \"KH00001\",\r\n            \"account_object_name\": \"Nguyễn Chính\",\r\n            \"country\": \"Việt Nam\",\r\n            \"is_same_address\": false,\r\n            \"pay_account\": \"331\",\r\n            \"receive_account\": \"131\",\r\n            \"closing_amount\": 0.0,\r\n            \"reftype\": 0,\r\n            \"reftype_category\": 0,\r\n            \"branch_id\": \"fd745cee-9980-11ea-af8e-005056890bf4\",\r\n            \"is_convert\": false,\r\n            \"is_group\": false,\r\n            \"is_remind_debt\": true,\r\n            \"excel_row_index\": 0,\r\n            \"is_valid\": false,\r\n            \"created_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"created_by\": \"Hà Thị Hồng Vân\",\r\n            \"modified_date\": \"2022-05-18T09:29:29.7637631+07:00\",\r\n            \"modified_by\": \"Hà Thị Hồng Vân\",\r\n            \"auto_refno\": false,\r\n            \"state\": 0\r\n        }\r\n    ]\r\n}"; //json_send_misa(item);
                    var json = JsonConvert.SerializeObject(item);
                    if (!System.IO.Directory.Exists("..\\json")) System.IO.Directory.CreateDirectory("..\\json");
                    File.WriteAllText("..\\json\\json.txt", json);

                    HttpClient client = new HttpClient();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    Uri uri = new Uri(host.Trim('/') + "/apir/sync/actopen/save");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpRequestMessage httpRequest = new HttpRequestMessage();
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.RequestUri = uri;
                    httpRequest.Headers.Add("X-MISA-AccessToken", access_token);
                    httpRequest.Content = content;

                    var result = client.SendAsync(httpRequest).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var contents = result.Content.ReadAsStringAsync();
                        var jsonResult = contents.Result;
                        DataTable dtData = (DataTable)JsonConvert.DeserializeObject("[" + jsonResult + "]", (typeof(DataTable)));
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            string msg = dtData.Rows[0]["Data"].ToString();
                            bool success = bool.Parse(dtData.Rows[0]["Success"].ToString());

                            if (!success)
                            {
                                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            }
                            else
                            {
                                if (!m_data.upd_mqmisa(xxx, long.Parse(r["id"].ToString()), 0, m.ngayhienhanh_server, cbUser.Text.Trim()))
                                {
                                    LibUtility.Utility.MsgBox("Lỗi cập nhập mqmisa!");
                                    return;
                                }

                                r["matrangthai"] = 1;
                                r["trangthai"] = "Đã chuyển";
                                dt.AcceptChanges();
                                //lblNofication.Text = string.Format("Số dòng gửi: {0}/{1}", index++, rows.Length);
                            }
                        }
                    }
                }
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
            }
            catch (Exception ex)
            {
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                MessageBox.Show(ex.Message, "lỗi");
            }
        }


        private void btnTransport_Click(object sender, EventArgs e)
        {
            if (i_phieuthutien == 5) f_phieuthutien();// đã xong
            if (i_phieuthutien == 13) f_hoadonbanhang();
            if (i_phieuthutien == 7) f_phieunhapkho();
            if (i_phieuthutien == 8) f_phieuxuatkho();
            if (i_phieuthutien == 9) f_phieuchuyenkho();

        }

        private string get_dia_chi(string mabn)
        {
            try
            {
                sql = "select bn.matt, tt.tentt from " + user + ".btdbn bn join btdtt tt on bn.matt=tt.matt where bn.mabn='" + mabn + "' ";
                return m.get_data(sql).Tables[0].Rows[0]["tentt"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        private void gvlist_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                if (cbStatus.SelectedIndex == 2 && e.RowHandle >= 0)
                {
                    var status = gvList.GetRowCellValue(e.RowHandle, "TRANGTHAI");

                    if (status != null && status.ToString() == "Đã chuyển")
                    {
                        e.Appearance.ForeColor = Color.Blue;
                        e.HighPriority = true;
                    }
                }
            }
            catch
            {

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public class ConnectMisa
        {
            public string app_id { get; set; }
            public string access_code { get; set; }
            public string org_company_code { get; set; }
        }

        public class DetailMisa
        {
            public string ref_detail_id { get; set; }
            public string refid { get; set; }
            public string account_object_id { get; set; }
            public int sort_order { get; set; }
            public bool un_resonable_cost { get; set; }
            public double amount_oc { get; set; }
            public double amount { get; set; }
            public double cash_out_amount_finance { get; set; }
            public double cash_out_diff_amount_finance { get; set; }
            public double cash_out_amount_management { get; set; }
            public double cash_out_diff_amount_management { get; set; }
            public double cash_out_exchange_rate_finance { get; set; }
            public double cash_out_exchange_rate_management { get; set; }
            public string description { get; set; }
            public string debit_account { get; set; }
            public string credit_account { get; set; }
            public string account_object_code { get; set; }
            public int state { get; set; }
            public string inventory_item_id { get; internal set; }
            public string unit_id { get; internal set; }
            public string stock_id { get; internal set; }
            public double unit_price_finance { get; internal set; }
            public bool is_promotion { get; internal set; }
            public int inventory_resale_type_id { get; internal set; }
            public string main_unit_id { get; internal set; }
            public double quantity { get; internal set; }
            public double amount_management { get; internal set; }
            public double amount_finance { get; internal set; }
            public double unit_price_management { get; internal set; }
            public double main_unit_price_management { get; internal set; }
            public double main_unit_price_finance { get; internal set; }
            public double main_convert_rate { get; internal set; }
            public double amount_finance_oc { get; internal set; }
            public double main_quantity { get; internal set; }
            public double amount_management_oc { get; internal set; }
            public string exchange_rate_operator { get; internal set; }
            public string account_object_name { get; internal set; }
            public string inventory_item_code { get; internal set; }
            public int inventory_item_type { get; internal set; }
            public string stock_code { get; internal set; }
            public string unit_name { get; internal set; }
            public string main_unit_name { get; internal set; }
            public string inventory_item_name { get; internal set; }
            public string stock_name { get; internal set; }
            public string account_name { get; internal set; }
            public bool is_allow_duplicate_serial_number { get; internal set; }
            public bool is_follow_serial_number { get; internal set; }
            public bool is_description_import { get; internal set; }
            public bool un_resonable_cost_import { get; internal set; }
            public bool is_promotion_import { get; internal set; }
            public bool is_description { get; internal set; }
            public string organization_unit_id { get; internal set; }
            public bool is_un_update_outward_price { get; internal set; }
            public double sale_price { get; internal set; }
            public double sale_amount { get; internal set; }
            public string organization_unit_code { get; internal set; }
            public string organization_unit_name { get; internal set; }
            public double remain_quantity { get; internal set; }
            public double unit_price { get; internal set; }
            public bool is_unit_price_after_tax { get; internal set; }
            public string posted_date { get; internal set; }
            public string refdate { get; internal set; }
            public int reftype { get; internal set; }
            public double total_amount_finance { get; internal set; }
            public double vat_rate { get; internal set; }
            public double quantity_delivered { get; internal set; }
            public double quantity_remain { get; internal set; }
            public double main_quantity_remain { get; internal set; }
            public string from_stock_id { get; internal set; }
            public string to_stock_id { get; internal set; }
            public double sale_price1 { get; internal set; }
            public string from_stock_code { get; internal set; }
            public string from_stock_name { get; internal set; }
            public string to_stock_code { get; internal set; }
            public string to_stock_name { get; internal set; }
        }

        public class in_outward
        {
            public long voucher_type { get; set; }
            public bool  is_get_new_id  { get; set; }
            public bool   is_allow_group { get; set; }
            public int   org_reftype { get; set; }
            public int   act_voucher_type { get; set; }
            public  string  refid { get; set; }
            public  string  account_object_id { get; set; }
            public  string  employee_id { get; set; }
            public  string  branch_id { get; set; }
            public  int  display_on_book { get; set; }
            public  long  reforder { get; set; }
            public  string refdate { get; set; }
            public string posted_date { get; set; }
            public string in_reforder { get; set; }
            public bool is_posted_finance { get; set; }
            public bool is_posted_management { get; set; }
            public bool is_posted_inventory_book_finance { get; set; }
            public bool is_posted_inventory_book_management { get; set; }
            public bool is_branch_issued { get; set; }
            public bool is_sale_with_outward { get; set; }
            public bool is_invoice_replace { get; set; }
            public decimal   total_amount_finance { get; set; }
            public decimal   total_amount_management { get; set; }
            public string   refno_finance { get; set; }
            public string   refno_management { get; set; }
            public string account_object_name { get; set; }
            public string account_object_address { get; set; }
            public string journal_memo { get; set; }
            public long   reftype { get; set; }
            public string employee_name { get; set; }
            public string payment_term_id { get; set; }
            public int   due_time { get; set; }
            public bool is_executed { get; set; }
            public string employee_code { get; set; }
            public  int  publish_status { get; set; }
            public  bool  is_invoice_deleted { get; set; }
            public  bool  is_invoice_receipted { get; set; }
            public string account_object_code { get; set; }
            public string created_date { get; set; }
            public string created_by { get; set; }
            public string modified_date { get; set; }
            public bool   auto_refno { get; set; }
            public int   state { get; set; }
        }
        public class sa_invoice
        {
           public long  voucher_type { get; set; }
            public bool    is_get_new_id { get; set; }
            public bool   is_allow_group { get; set; }
            public  int   org_reftype { get; set; }
            public  int   act_voucher_type { get; set; }
            public string   journal_memo { get; set; }
            public int    reforder { get; set; }
            public string   refid { get; set; }
            public string    branch_id { get; set; }
            public string    account_object_id { get; set; }
            public string    employee_id { get; set; }
            public string    voucher_reference_id { get; set; }
            public  int   display_on_book { get; set; }
            public int    publish_status { get; set; }
            public  int   discount_type { get; set; }
            public  decimal   discount_rate_voucher { get; set; }
            public string    inv_date { get; set; }
            public bool   is_paid { get; set; }
            public bool    is_posted { get; set; }
            public int    include_invoice { get; set; }
            public bool   is_attach_list { get; set; }
            public bool   is_branch_issued { get; set; }
            public bool    is_posted_last_year { get; set; }
            public bool   is_invoice_replace { get; set; }
            public decimal    exchange_rate { get; set; }
            public decimal    total_sale_amount_oc { get; set; }
            public decimal   total_sale_amount { get; set; }
            public decimal   total_discount_amount_oc { get; set; }
            public decimal    total_discount_amount { get; set; }
            public decimal   total_vat_amount_oc { get; set; }
            public decimal   total_vat_amount { get; set; }
            public decimal    total_amount_oc { get; set; }
            public decimal   total_amount { get; set; }
            public string    account_object_name { get; set; }
            public string    account_object_code { get; set; }
            public string    employee_name { get; set; }
            public string   employee_code { get; set; }
            public string    account_object_address { get; set; }
            public string    account_object_tax_code { get; set; }
            public string    payment_method { get; set; }
            public string    currency_id { get; set; }
            public string    refno_finance { get; set; }
            public string   refno_management { get; set; }
            public int    is_created_savoucher { get; set; }
            public  int   send_email_status { get; set; }
            public bool   is_invoice_receipted { get; set; }
            public  int   invoice_status { get; set; }
            public int    voucher_reference_reftype { get; set; }
            public bool    is_invoice_deleted { get; set; }
            public string    inv_replacement_id { get; set; }
            public bool    is_update_template { get; set; }
            public bool    is_increase_invno { get; set; }
            public bool    ccy_exchange_operator { get; set; }
            public  int   business_area { get; set; }
            public  int   excel_row_index { get; set; }
            public bool    is_valid { get; set; }
            public long    reftype { get; set; }
            public string     created_date { get; set; }
            public string    created_by { get; set; }
            public string    modified_date { get; set; }
            public bool    auto_refno { get; set; }
            public int    state { get; set; }
        }
        public class VoucherMisa
        {
            public List<DetailMisa> detail { get; set; }
            public int voucher_type { get; set; }
            public bool is_get_new_id { get; set; }
            public string org_refid { get; set; }
            public bool is_allow_group { get; set; }
            public string org_refno { get; set; }
            public int org_reftype { get; set; }
            public string org_reftype_name { get; set; }
            public string refno { get; set; }
            public int act_voucher_type { get; set; }
            public string refid { get; set; }
            public string account_object_id { get; set; }
            public string branch_id { get; set; }
            public string employee_id { get; set; }
            public int reason_type_id { get; set; }
            public int display_on_book { get; set; }
            public long reforder { get; set; }
            public string refdate { get; set; }
            public string posted_date { get; set; }
            public bool is_posted_finance { get; set; }
            public bool is_posted_management { get; set; }
            public bool is_posted_cash_book_finance { get; set; }
            public bool is_posted_cash_book_management { get; set; }
            public double exchange_rate { get; set; }
            public double total_amount_oc { get; set; }
            public double total_amount { get; set; }
            public string refno_finance { get; set; }
            public string refno_management { get; set; }
            public string account_object_name { get; set; }
            public string account_object_address { get; set; }
            public string account_object_contact_name { get; set; }
            public string account_object_code { get; set; }
            public string journal_memo { get; set; }
            public string document_included { get; set; }
            public string currency_id { get; set; }
            public string employee_code { get; set; }
            public string employee_name { get; set; }
            public string ca_audit_refid { get; set; }
            public int excel_row_index { get; set; }
            public bool is_valid { get; set; }
            public int reftype { get; set; }
            public string created_date { get; set; }
            public string created_by { get; set; }
            public string modified_date { get; set; }
            public string modified_by { get; set; }
            public bool auto_refno { get; set; }
            public int state { get; set; }
        }
        public class VoucherMisa_13
        {
            public List<DetailMisa> detail { get; set; }
            public List<in_outward> in_outward { get; set; }
            public List<sa_invoice> sa_invoice { get; set; }
            public int voucher_type { get; set; }
            public bool is_get_new_id { get; set; }
            public string org_refid { get; set; }
            public bool is_allow_group { get; set; }
            public string org_refno { get; set; }
            public int org_reftype { get; set; }
            public string org_reftype_name { get; set; }
            public string refno { get; set; }
            public int act_voucher_type { get; set; }
            public string refid { get; set; }
            public string account_object_id { get; set; }
            public string branch_id { get; set; }
            public string employee_id { get; set; }
            public int reason_type_id { get; set; }
            public int display_on_book { get; set; }
            public long reforder { get; set; }
            public string refdate { get; set; }
            public string posted_date { get; set; }
            public bool is_posted_finance { get; set; }
            public bool is_posted_management { get; set; }
            public bool is_posted_cash_book_finance { get; set; }
            public bool is_posted_cash_book_management { get; set; }
            public double exchange_rate { get; set; }
            public double total_amount_oc { get; set; }
            public double total_amount { get; set; }
            public string refno_finance { get; set; }
            public string refno_management { get; set; }
            public string account_object_name { get; set; }
            public string account_object_address { get; set; }
            public string account_object_contact_name { get; set; }
            public string account_object_code { get; set; }
            public string journal_memo { get; set; }
            public string document_included { get; set; }
            public string currency_id { get; set; }
            public string employee_code { get; set; }
            public string employee_name { get; set; }
            public string ca_audit_refid { get; set; }
            public int excel_row_index { get; set; }
            public bool is_valid { get; set; }
            public int reftype { get; set; }
            public string created_date { get; set; }
            public string created_by { get; set; }
            public string modified_date { get; set; }
            public string modified_by { get; set; }
            public bool auto_refno { get; set; }
            public int state { get; set; }
            public string reftype_name { get; internal set; }
            public int unit_price_method { get; internal set; }
            public string in_reforder { get; internal set; }
            public bool is_posted_inventory_book_management { get; internal set; }
            public bool is_posted_inventory_book_finance { get; internal set; }
            public bool is_created_sa_return_last_year { get; internal set; }
            public double total_amount_management { get; internal set; }
            public bool is_return_with_inward { get; internal set; }
            public double total_amount_finance { get; internal set; }
            public bool is_executed { get; internal set; }
            public bool is_adjust_value { get; internal set; }
            public string in_refdate { get; internal set; }
            public bool is_branch_issued { get; internal set; }
            public bool is_sale_with_outward { get; internal set; }
            public bool is_invoice_replace { get; internal set; }
            public string contact_name { get; internal set; }
            public string payment_term_id { get; internal set; }
            public int due_time { get; internal set; }
            public int publish_status { get; internal set; }
            public bool is_invoice_deleted { get; internal set; }
            public bool is_invoice_receipted { get; internal set; }
            public string transporter_id { get; internal set; }
            public string contract_date { get; internal set; }
            public bool is_attach_list { get; internal set; }
            public string contract_code { get; internal set; }
            public string contract_owner { get; internal set; }
            public string company_tax_code { get; internal set; }
            public string transporter_name { get; internal set; }
            public string transport_contract_code { get; internal set; }
            public string transport { get; internal set; }
            public string ref_detail_id { get; internal set; }
            public double quantity { get; internal set; }
            public int send_email_status { get; internal set; }
            public int invoice_status { get; internal set; }
            public int inv_type_id { get; internal set; }
        }
        



        public class DictionaryMisa
        {
            internal string debit_account;

            public int dictionary_type { get; set; }
            public string account_object_id { get; set; }
            public int due_time { get; set; }
            public int account_object_type { get; set; }
            public bool is_vendor { get; set; }
            public bool is_customer { get; set; }
            public bool is_employee { get; set; }
            public bool inactive { get; set; }
            public double agreement_salary { get; set; }
            public double salary_coefficient { get; set; }
            public double insurance_salary { get; set; }
            public double maximize_debt_amount { get; set; }
            public double receiptable_debt_amount { get; set; }
            public string account_object_code { get; set; }
            public string account_object_name { get; set; }
            public string country { get; set; }
            public bool is_same_address { get; set; }
            public string pay_account { get; set; }
            public string receive_account { get; set; }
            public double closing_amount { get; set; }
            public int reftype { get; set; }
            public int reftype_category { get; set; }
            public string branch_id { get; set; }
            public bool is_convert { get; set; }
            public bool is_group { get; set; }
            public bool is_remind_debt { get; set; }
            public int excel_row_index { get; set; }
            public bool is_valid { get; set; }
            public string created_date { get; set; }
            public string created_by { get; set; }
            public string modified_date { get; set; }
            public string modified_by { get; set; }
            public bool auto_refno { get; set; }
            public int state { get; set; }
            public string organization_unit_id { get; set; }
            public int? gender { get; set; }
            public string organization_unit_name { get; set; }
            public int number_of_dependent { get; internal set; }
            public string address { get; internal set; }
            public string account_object_bank_account { get; internal set; }
            public string einvoice_contact_name { get; internal set; }
            public string legal_representative { get; internal set; }
            public string district { get; internal set; }
            public string ward_or_commune { get; internal set; }
            public string prefix { get; internal set; }
            public string contact_name { get; internal set; }
            public string province_or_city { get; internal set; }
            public string company_tax_code { get; internal set; }
            public string tel { get; internal set; }
            public string account_object_group_id_list { get; internal set; }
            public string account_object_group_code_list { get; internal set; }
            public string account_object_group_name_list { get; internal set; }
            public string account_object_group_misa_code_list { get; internal set; }
            public string credit_account { get; internal set; }
        }

        public class SaveMisa_05
        {
            public string app_id { get; set; }
            public string org_company_code { get; set; }
            public List<VoucherMisa> voucher { get; set; }
            public List<DictionaryMisa> dictionary { get; set; }
        }
        public class SaveMisa_13
        {
            public string app_id { get; set; }
            public string org_company_code { get; set; }
            public List<VoucherMisa_13> voucher { get; set; }
            public List<DictionaryMisa> dictionary { get; set; }
        }
    }
}
