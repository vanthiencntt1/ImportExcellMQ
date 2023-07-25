using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ImportXML
{
    public partial class frmLienThongDonThuoc : XtraForm
    {
        private static LibHIS.AccessData m;
        public static OracleHelper.OracleSupport Oracle;
        private string xxx, user;
    
        private string apiLogin = "http://api-beta.donthuocquocgia.vn";
        private string apiGuidon = "http://api-beta.donthuocquocgia.vn";


        //private string macskb = "25366";
        private string ma_lien_thong_co_so_kham_chua_benh = "0125366";
        private string pass_lien_thong_co_so_kham_chua_benh = "0125366";
        private string ma_lien_thong_bac_si = "01002245HNO-CCHNT";
        private string pass_lien_thong_bac_si = "01002245HNO-CCHNT";

        private int Recorddone = 0;
        private string  token = "";
        private long l_id = 0;
        private bool connect = false;
        private bool send_auto = false;
        public frmLienThongDonThuoc()
        {
            InitializeComponent();

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");
            Control.CheckForIllegalCrossThreadCalls = false;
            

            m = Program.dal;
            Oracle = Program.Oracle;
            if (Oracle == null) Oracle = new OracleHelper.OracleSupport();
            if (m == null) m = new LibHIS.AccessData(Oracle);
            user = m.user;
            xxx = user + m.mmyy(m.ngayhienhanh_server);
            lblNofication.Text = "";


            apiLogin = LibConfig.ConfigManager.DonthuocQuocgia_Api_Login;
            apiGuidon = LibConfig.ConfigManager.DonthuocQuocgia_Api_Guidon;
            ma_lien_thong_co_so_kham_chua_benh = LibConfig.ConfigManager.DonthuocQuocgia_Api_Userid;
            pass_lien_thong_co_so_kham_chua_benh = LibConfig.ConfigManager.DonthuocQuocgia_Api_Password;
        }

        private void frmTestApi_DonThuoc_Load(object sender, EventArgs e)
        {
            try
            {
                nSoPhutTimer.Value = decimal.Parse(LibUtility.Utility.Thongso("thongso", this.Name + nSoPhutTimer.Name));
            }
            catch { }
            string tungay = "01/" + DateTime.Now.Date.ToString("MM") + "/" + DateTime.Now.Date.ToString("yyyy");
            string denngay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "/" + DateTime.Now.Date.ToString("MM") + "/" + DateTime.Now.Date.ToString("yyyy");
            d_tungay.EditValue = DateTime.ParseExact(tungay, "dd/MM/yyyy", null);
            d_denngay.EditValue = DateTime.ParseExact(denngay, "dd/MM/yyyy", null);
            load_danhsach(tungay, denngay, 3);
            load_loai();
        }
        private void gui_don_thuoc_tu_dong(bool isAuto)
        {
            System.Data.DataTable dtTmp = (System.Data.DataTable)grList.DataSource;
            dtTmp.AcceptChanges();
            Recorddone = 0;
            foreach (DataRow r in dtTmp.Select("chon=true"))
            {

                string s_thong_tin_nguoi_giam_ho = "";
                string s_ngay_tai_kham = "";
                string s_can_nang = "";
                string so_dien_thoai_nguoi_kham_benh = r["didong"].ToString();
                long l_maql = long.Parse(r["maql"].ToString());
                foreach (DataRow r1 in m.get_data("select a.hoten,a.dienthoai from " + user + ".quanhe a where a.maql=" + l_maql+ " union all select a.hoten,a.dienthoai  from " + user + m.mmyy(r["ngaycaptoa"].ToString())  + ".quanhe a where a.maql=" + l_maql + "").Tables[0].Rows)
                {
                    if(so_dien_thoai_nguoi_kham_benh==""&& r1["dienthoai"].ToString()!="") so_dien_thoai_nguoi_kham_benh= r1["dienthoai"].ToString();
                    s_thong_tin_nguoi_giam_ho = r1["hoten"].ToString();
                }
                try
                {
                    s_ngay_tai_kham = m.get_ngay_hen(r["ngay"].ToString(), r["mabn"].ToString(), long.Parse(r["mavaovien"].ToString()));
                }
                catch
                {
                    s_ngay_tai_kham = "";
                }

                string sql = "select cannang from " + user + m.mmyy(r["ngaycaptoa"].ToString()) + ".dausinhton where maql =" + l_maql;
                foreach (DataRow _r in m.get_data(sql).Tables[0].Rows)
                {
                     s_can_nang = string.IsNullOrEmpty(_r["cannang"].ToString()) ? "0" : _r["cannang"].ToString();
                 }

                l_id = long.Parse(r["id"].ToString());
                string s_mabn = r["mabn"].ToString();
                string ten_benh_nhan = r["hoten"].ToString();
                string ngay_sinh_benh_nhan = string.IsNullOrEmpty(r["ngaysinh"].ToString()) ? "01/01/" + r["namsinh"].ToString() : r["ngaysinh"].ToString().Substring(0, 10);
                string s_namsinh = r["namsinh"].ToString();
                int tuoi_benh_nhan = !string.IsNullOrWhiteSpace(s_namsinh) ? DateTime.Now.Date.Year - int.Parse(s_namsinh) : 0;
                string gioi_tinh = r["igioitinh"].ToString();
                string ma_so_the_bao_hiem_y_te = r["sothe"].ToString();
                string thong_tin_nguoi_giam_ho = s_thong_tin_nguoi_giam_ho;
                string dia_chi = r["diachi"].ToString();
                string socmnd = r["socmnd"].ToString();
                string ma_chan_doan = r["maicd"].ToString();
                string chan_doan = r["chandoan"].ToString();
                string ket_luan = r["chandoan"].ToString();
                string luu_y = r["ghichu"].ToString();
                string hinh_thuc_dieu_tri = "2";
                string loi_dan = r["ghichu"].ToString();
                string bacsiky = r["bacsiky"].ToString();
                string tenbs = r["tenbs"].ToString();
                
                if (string.IsNullOrEmpty(so_dien_thoai_nguoi_kham_benh)) so_dien_thoai_nguoi_kham_benh = LibConfig.ConfigManager.Dienthoai;
                string ngay_gio_ke_don = "", tu_ngay = "", den_ngay = "";
                ngay_gio_ke_don = r["ngay"].ToString();
                tu_ngay = r["toatu"].ToString();
                den_ngay = r["toaden"].ToString();

                int songaytk = int.Parse(r["songaytaikham"].ToString());
                string can_nang = s_can_nang;
                int dot_dung_thuoc = 1;
                string loai_don_thuoc_hangdoi = r["loai_don_thuoc_hangdoi"].ToString();
                string malienthong_hangdoi = r["malienthong_hangdoi"].ToString();
                ma_lien_thong_bac_si = r["ma_lien_thong_bac_si"].ToString();
                pass_lien_thong_bac_si = r["password_lienthong_bs"].ToString();
                if (string.IsNullOrEmpty(ma_lien_thong_bac_si) || string.IsNullOrEmpty(pass_lien_thong_bac_si))
                {
                    LibUtility.Utility.showPopup(bacsiky + " Chưa khai báo mã liên thông!");
                }
                else
                {
                    guidonthuoc(l_id, s_mabn, loai_don_thuoc_hangdoi, malienthong_hangdoi, ten_benh_nhan, tuoi_benh_nhan.ToString(), ngay_sinh_benh_nhan,
                         can_nang, gioi_tinh, ma_so_the_bao_hiem_y_te, thong_tin_nguoi_giam_ho, dia_chi, socmnd, ma_chan_doan, chan_doan, ket_luan, luu_y,
                          hinh_thuc_dieu_tri, dot_dung_thuoc.ToString(), loi_dan, so_dien_thoai_nguoi_kham_benh, s_ngay_tai_kham, ngay_gio_ke_don, tu_ngay, den_ngay, bacsiky, tenbs,r["ngaycaptoa"].ToString());
                }
                lblNofication.Text = string.Format("Đã thực hiện : " + Recorddone.ToString() + " toa!");
            }
            if (!string.IsNullOrEmpty(lblNofication.Text))
                LibUtility.Utility.showPopup(lblNofication.Text);

            reload_data(isAuto);
        }
        private void load_loai()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));

            dt.Rows.Add(1, "Tất cả");
            dt.Rows.Add(2, "Đã chuyển");
            dt.Rows.Add(3, "Chưa chuyển");

            lookup_loai.DataSource = dt;
            lookup_loai.DisplayMember = "name";
            lookup_loai.ValueMember = "id";

            cb_loai.EditValue = 3;
        }
        private void btnLoadData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            reload_data(false);
        }

        private void reload_data(bool isAuto)
        {
            if (isAuto)
            {
                var ngayserver = m.ngayhienhanh_server.Substring(0, 10);
                 d_tungay.EditValue = DateTime.ParseExact(ngayserver, "dd/MM/yyyy", null);
                d_denngay.EditValue = DateTime.ParseExact(ngayserver, "dd/MM/yyyy", null);
            }

            string tungay = string.Format("{0:dd/MM/yyyy}", d_tungay.EditValue);
            string denngay = string.Format("{0:dd/MM/yyyy}", d_denngay.EditValue);
            
            int loai = int.Parse(cb_loai.EditValue.ToString());
            load_danhsach(tungay, denngay, loai);
            btnSend.Enabled = true;
            btnDel.Enabled = true;
        }

        private void load_danhsach(string tungay, string denngay, int loai)
        {
            try
            {
                string sql = "select distinct to_char(a.ngay,'dd/mm/yyyy') as ngaycaptoa, a.id,a.mabn,b.hoten,a.mavaovien,a.maql,to_char(a.ngay,'yyyy-mm-dd hh24:mi:ss') as ngay,a.done,";
                sql += "trim(b.sonha)||' '||trim(b.thon)||' '||trim(z3.tenpxa)||', '||trim(z2.tenquan)||', '||z1.tentt as diachi, ";
                sql += "case when b.phai=0 then 'Nam' else 'Nữ' end as phai, case when b.phai=0 then '2' else '3' end as igioitinh,";
                sql += " b.namsinh,to_char(b.ngaysinh,'dd/mm/yyyy') as ngaysinh,nvl(d.sothe,' ') as sothe,a.chandoan,a.maicd,e.tenkp,nvl(f.hoten,' ') as tenbs,";
                sql += "a.songay,a.ghichu,a.makp,a.mabs,a.ketluan, a.malienthong ";
                sql += ", nvl(a.daky,0) as daky,a.mabsky,bsk.hoten as bacsiky,to_char(a.ngayky,'dd/mm/yyyy hh24:mi') as ngayky,a.fileid,nvl(a.SONGAYNGHIOM,0) as SONGAYNGHIOM,nvl(a.SONGAYTAIKHAM,0) as SONGAYTAIKHAM,a.tuvandinhduong, p.didong ";
                sql += ", nvl(f.ma_lien_thong_bac_si,'') ma_lien_thong_bac_si, nvl(f.password_lienthong_bs,'') password_lienthong_bs,b.socmnd,a.malienthong_hangdoi,a.loai_don_thuoc_hangdoi,to_char(a.ngay,'dd/mm/yyyy') toatu, to_char(a.ngay+a.songay,'dd/mm/yyyy')  toaden ";
                sql += " from xxx.d_thuocbhytll a inner join " + user + ".btdbn b on a.mabn=b.mabn ";
                sql += " inner join xxx.d_thuocbhytct g on a.id=g.id";
                sql += " left join xxx.bhyt d on a.maql=d.maql ";
                sql += " inner join " + user + ".btdkp_bv e on a.makp=e.makp";
                sql += " left join " + user + ".dmbs f on a.mabs=f.ma";
                sql += " left join " + user + ".dmbs bsk on a.mabsky=bsk.ma ";
                sql += " left join " + user + ".btdtt z1 on b.matt=z1.matt";
                sql += " left join " + user + ".btdquan z2 on b.maqu=z2.maqu ";
                sql += " left join " + user + ".btdpxa z3 on b.maphuongxa=z3.maphuongxa ";
                sql += " left join " + user + ".dienthoai p on b.mabn=p.mabn ";
                if (loai == 1) sql += "where   1=1 and "; // all
                if (loai == 2) sql += "where a.malienthong is not null  and "; // đã chuyển
                if (loai == 3) sql += "where  a.malienthong_hangdoi is not  null and a.malienthong is   null and (f.ma_lien_thong_bac_si is not null and f.password_lienthong_bs is not null) and "; // chưa chuyển
                sql += "     a.ngay between to_date('" + tungay.Substring(0, 10) +" 00:00"+ "','dd/mm/yyyy hh24:mi') and to_date('" + denngay.Substring(0, 10) + " 23:59"+ "','dd/mm/yyyy hh24:mi')";
                DataTable dt = m.get_data_mmyy(sql, tungay, denngay, false).Tables[0];
                dt.Columns.Add("chon", typeof(bool));
                foreach (DataRow r in dt.Rows) r["chon"] = true;
                grList.DataSource = dt;
                if (dt.Rows.Count > 0) lblRecord.Text = "Record: " + dt.Rows.Count.ToString();
                else
                    lblRecord.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "lỗi");
            }
        }
        private void btnSend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(ma_lien_thong_co_so_kham_chua_benh) || string.IsNullOrEmpty(pass_lien_thong_co_so_kham_chua_benh))
            {
                MessageBox.Show("Chưa khai báo mã liên thông KCB!");
                return;
            }
            gui_don_thuoc_tu_dong(false);
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int[] selectedBasketTradeRows = this.gvList.GetSelectedRows();

            if (selectedBasketTradeRows.Length == 0)
            {
                LibUtility.Utility.MsgBox("Vui lòng chọn dòng cần hủy liên thông !");
                return;
            }



            if (selectedBasketTradeRows.Length < 1)
            {
                LibUtility.Utility.MsgBox("Vui lòng chọn dòng cần hủy !");
                return;
            }
            else if (MessageBox.Show("Bạn muốn hủy liên thông những dòng được chọn ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int i = 0; i < selectedBasketTradeRows.Length; i++)
                {
                    DataRow r = this.gvList.GetDataRow(selectedBasketTradeRows[i]);
                    try
                    {

                        m.executedata("update " + user + m.mmyy(r["ngaycaptoa"].ToString()) + ".d_thuocbhytll set malienthong is null where id=" + r["id"].ToString());

                        reload_data(false);
                    }
                    catch
                    {
                        LibUtility.Utility.MsgBox("Xóa lỗi !");
                    }
                }
            }
        }
        private void butGuiDonThuoc_Click(object sender, EventArgs e)
        {
           
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private bool guidonthuoc(long id,string s_mabn, string loai_don_thuoc, string ma_don_thuoc, string ten_benh_nhan,
            string tuoi_benh_nhan, string ngay_sinh_benh_nhan, string can_nang, string gioi_tinh,
            string ma_so_the_bao_hiem_y_te, string thong_tin_nguoi_giam_ho, string dia_chi,string socmnd, string ma_chan_doan, string chan_doan, 
            string ket_luan, string luu_y, string hinh_thuc_dieu_tri, string dot_dung_thuoc, 
            string loi_dan, string so_dien_thoai_nguoi_kham_benh, string ngay_tai_kham, string ngay_gio_ke_don, string tu_ngay, string den_ngay,string bacsiky,string tenbs,string ngaycaptoa)
        {
            bool done = false;
            string sothebhyt = "";
            
            don_thuoc_lien_thong item = new don_thuoc_lien_thong();
            try
            {
                try
                {
                    ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, true);

                    string p_where = " and a0.mabn='" + s_mabn + "'";

                    p_where += " and a.id=" + id;
                    DataTable dtct = m.getToathuoc(m.mmyy(ngaycaptoa), p_where).Tables[0];

                    if (dtct.Rows.Count > 0)
                    {
                        if (ma_so_the_bao_hiem_y_te.Trim() != "" && ma_so_the_bao_hiem_y_te.Trim() != null) sothebhyt = ma_so_the_bao_hiem_y_te.Substring(0, 15);
                        Random r = new Random();
                        if (loai_don_thuoc == "")
                        {
                            loai_don_thuoc = "c";
                        }                       
                        if(ma_don_thuoc=="") ma_don_thuoc = ma_lien_thong_co_so_kham_chua_benh.Substring(ma_lien_thong_co_so_kham_chua_benh.Length - 5, 5) + RandomString(7) + "-" + loai_don_thuoc;
                        item.loai_don_thuoc = loai_don_thuoc;
                        item.ma_don_thuoc = ma_don_thuoc;
                        item.ten_benh_nhan = ten_benh_nhan;
                        item.tuoi_benh_nhan = tuoi_benh_nhan;
                        item.can_nang = can_nang;
                        item.ngay_sinh_benh_nhan = ngay_sinh_benh_nhan;
                        item.gioi_tinh = gioi_tinh;
                        item.ma_so_the_bao_hiem_y_te = sothebhyt;
                        item.thong_tin_nguoi_giam_ho = thong_tin_nguoi_giam_ho;
                        item.dia_chi = dia_chi;
                        item.socmnd = socmnd;
                        item.ma_chan_doan = ma_chan_doan;
                        item.chan_doan = chan_doan;
                        item.ket_luan = ket_luan;
                        item.luu_y = luu_y;
                        item.hinh_thuc_dieu_tri = hinh_thuc_dieu_tri;
                        item.dot_dung_thuoc = dot_dung_thuoc;
                        item.loi_dan =LibUtility.Utility.RemoveSpecialChars(loi_dan);
                        item.so_dien_thoai_nguoi_kham_benh =so_dien_thoai_nguoi_kham_benh;
                        item.ngay_tai_kham = ngay_tai_kham;
                        item.ngay_gio_ke_don = ngay_gio_ke_don;
                        item.tu_ngay = tu_ngay;
                        item.den_ngay = den_ngay;
                        item.bacsiky = bacsiky;
                        item.tenbs = tenbs;
                        chitiet_thuoc ct;
                        List<chitiet_thuoc> lst_ct = new List<chitiet_thuoc>();

                        foreach (DataRow dr in dtct.Rows)
                        {
                            ct = new chitiet_thuoc();
                            ct.biet_duoc = dr["ma"].ToString();
                            ct.ten_thuoc = dr["ten"].ToString();
                            ct.don_vi_tinh = dr["dang"].ToString();
                            ct.ma_thuoc = dr["mabd"].ToString();
                            ct.so_luong = dr["soluong"].ToString();
                            ct.cach_dung = (dr["cachdung"].ToString()==""? dr["ghichu"].ToString(): dr["cachdung"].ToString()) ;
                            lst_ct.Add(ct);
                        }
                        item.thongtindonthuoc = lst_ct;
                        done = api_post_donthuoc(item);
                        return done;
                    }
                }
                catch (Exception ex)
                {
                    LibUtility.Utility.showPopup(ex.Message);
                    ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                    return done;
                }
            }
            catch (Exception ex)
            {
                LibUtility.Utility.MsgBox(ex.Message);
                return done;
            }
            return done;
        }
        private void get_token_Bacsi(don_thuoc_lien_thong item)
        {

            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(apiLogin + "/api/auth/dang-nhap-bac-si");

            // Tạo StringContent
            string jsoncontent = "{";
            jsoncontent += "\"ma_lien_thong_bac_si\": \"" + ma_lien_thong_bac_si + "\", ";
            jsoncontent += "\"ma_lien_thong_co_so_kham_chua_benh\": \"" + ma_lien_thong_co_so_kham_chua_benh + "\", ";
            jsoncontent += "\"password\": \"" + pass_lien_thong_bac_si + "\" ";
            jsoncontent += "}";

            var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = httpClient.SendAsync(httpRequestMessage).Result;

            if (response.StatusCode.ToString() == "OK")
            {
                var contents = response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<DataTable>("[" + contents.Result + "]");
                token = data.Rows[0]["token"].ToString();
            }
            else
            {
                token = "";

                string _path = "..\\jsoncontent_toadientu\\";
                if (!System.IO.Directory.Exists(_path)) System.IO.Directory.CreateDirectory(_path);
                System.IO.File.WriteAllText(_path + "jsoncontent" + item.ma_don_thuoc + "_" + item.ten_benh_nhan + ".json", jsoncontent);

            }

        }
        private bool api_post_donthuoc( don_thuoc_lien_thong item)
        {
            bool done= false;
            string jsoncontent = "";
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            try
            {
                //ref data
                jsoncontent = TaoJon_GuiDonThuoc(item);

                string _path = "..\\jsoncontent_toadientu\\";
                if (!System.IO.Directory.Exists(_path)) System.IO.Directory.CreateDirectory(_path);
                System.IO.File.WriteAllText(_path + "jsoncontent" + item.ma_don_thuoc + "_" + item.ten_benh_nhan + ".json", jsoncontent);
                get_token_Bacsi(item);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpRequestMessage.Method = HttpMethod.Post;
                    httpRequestMessage.RequestUri = new Uri(apiGuidon + "/api/v1/gui-don-thuoc");
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    if (item != null)
                    {
                        StringContent httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
                        httpRequestMessage.Content = httpContent;
                        string errorbad = "";
                        try
                        {
                            var response = httpClient.SendAsync(httpRequestMessage).Result;
                            var contents = response.Content.ReadAsStringAsync();
                            errorbad = contents.Result.ToString();
                            var data = JsonConvert.DeserializeObject<DataTable>("[" + contents.Result + "]");

                            if (data.Columns[0].ToString() == "success")
                            {
                                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                                m.executedata("update " + xxx + ".d_thuocbhytll set malienthong='" + item.ma_don_thuoc + "' where id=" + l_id);
                                Recorddone = Recorddone + 1;
                            }
                            else
                            {

                                File.WriteAllText("error.txt", jsoncontent, Encoding.UTF8);
                                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                                LibUtility.Utility.showPopup(data.Rows[0][0].ToString());
                            }
                        }
                        catch
                        {
                          LibUtility.Utility.showPopup (errorbad);
                            return false;
                        }

                    }
                    else
                    {
                        LibUtility.Utility.showPopup("Dữ liệu không hợp lệ");
                        return done;
                    }

                }
                else
                {
                    LibUtility.Utility.showPopup("Bác sĩ khám  chưa đăng ký tài khoản liên thông: [" + item.tenbs + "].  Vui lòng đăng ký cổng liên thông toa thuốc quốc gia!");
                    return done;
                }
            }
            catch (Exception ex)
            {
                LibUtility.Utility.showPopup(ex.Message);
                ReportDynamic.clsCreateReportDynamic.ShowScreenWaiting(this, false);
                return done;
            }
            return done;
        }
       
        private string TaoJon_GuiDonThuoc(don_thuoc_lien_thong item)
        {
            string jsoncontent = "";

            // Tạo StringContent
            jsoncontent = "{";
            jsoncontent += "\"ma_don_thuoc\": \"" + item.ma_don_thuoc + "\", ";
            jsoncontent += "\"ho_ten_benh_nhan\": \"" + item.ten_benh_nhan + "\", ";
            jsoncontent += "\"ngay_sinh_benh_nhan\": \"" + item.ngay_sinh_benh_nhan.Substring(0, 10) + "\", ";
            jsoncontent += "\"loai_don_thuoc\": \"" + item.loai_don_thuoc + "\", ";
            jsoncontent += " \"chan_doan\": [ {\"ma_chan_doan\": \"" + item.ma_chan_doan + "\", \"ten_chan_doan\": \"" + item.chan_doan + "\",\"ket_luan\": \"" + item.ket_luan + "\"}     ], ";
            jsoncontent += "\"hinh_thuc_dieu_tri\": " + item.hinh_thuc_dieu_tri + ", ";
            jsoncontent += "\"dia_chi\": \"" + item.dia_chi + "\", ";
            jsoncontent += "\"gioi_tinh\": " + item.gioi_tinh + ", ";
            jsoncontent += "\"so_dien_thoai_nguoi_kham_benh\": \"" + item.so_dien_thoai_nguoi_kham_benh.ToString() + "\", ";
            jsoncontent += "\"can_nang\": " + item.can_nang + ", ";
            jsoncontent += "\"ngay_tai_kham\": \"" + item.ngay_tai_kham + "\", ";
            jsoncontent += "\"ma_so_the_bao_hiem_y_te\": \"" + item.ma_so_the_bao_hiem_y_te + "\", ";
            jsoncontent += "\"thong_tin_nguoi_giam_ho\": \"" + (string.IsNullOrEmpty(item.thong_tin_nguoi_giam_ho) ? item.ten_benh_nhan : item.thong_tin_nguoi_giam_ho) + "\", ";
            jsoncontent += "\"ma_so_chung_minh_thu\": \"" + item.socmnd + "\", ";
            jsoncontent += "\"thong_tin_don_thuoc\": ";

            jsoncontent += "[ ";
            foreach (chitiet_thuoc ct in item.thongtindonthuoc)
            {
                jsoncontent += "{ ";
                jsoncontent += "\"biet_duoc\": \"" + ct.biet_duoc + "\", ";
                jsoncontent += "\"ten_thuoc\": \"" + ct.ten_thuoc + "\", ";
                jsoncontent += "\"don_vi_tinh\": \"" + ct.don_vi_tinh + "\", ";
                jsoncontent += "\"cach_dung\": \"" + ct.cach_dung + "\", ";
                jsoncontent += "\"ma_thuoc\": \"" + ct.ma_thuoc + "\", ";
                jsoncontent += "\"so_luong\": \"" + ct.so_luong + "\" ";
                jsoncontent += "},";
            }
            jsoncontent = jsoncontent.Remove(jsoncontent.Length - 1);
            jsoncontent += "],";

            jsoncontent += "\"loi_dan\": \"" + item.loi_dan.ToString().Replace("\r\n", " ") + "\", ";
            jsoncontent += "    \"dot_dung_thuoc\": {\"dot\": " + 1 + ", \"tu_ngay\": \"" + item.tu_ngay + "\", \"den_ngay\": \"" + item.den_ngay + "\" }     , ";
            jsoncontent += "\"ngay_gio_ke_don\": \"" + item.ngay_gio_ke_don + "\" ";
            jsoncontent += "}";

            return jsoncontent;
        }
  
        private void gvlist_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                string quantity = gvList.GetRowCellValue(e.RowHandle, "MALIENTHONG").ToString();

                if (string.IsNullOrEmpty(quantity))
                {
                    e.Appearance.ForeColor = Color.Black;
                    e.HighPriority = true;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                    e.HighPriority = true;
                }
            }
        }
        private void btnConfig_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmDbConfig_LienThong f = new frmDbConfig_LienThong();
        //    f.ShowDialog();
            //api = f.api;
            //ma_lien_thong_bac_si = f.malienthong;
            //ma_lien_thong_co_so_kham_chua_benh = f.cskb;
            //ma_lien_thong_co_so_kham_chua_benh = f.cskb.Substring(2);
            //pass_lien_thong_co_so_kham_chua_benh = f.matkhau;
        }
        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string path = "Excel\\danhsachlienthong.xlsx";
            if (!Directory.Exists("Excel")) Directory.CreateDirectory("Excel");
            if (File.Exists(path)) File.Delete(path);
            gvList.ExportToXlsx(path);
            System.Diagnostics.Process.Start(path);
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LibUtility.Utility.writeXml("thongso", this.Name + nSoPhutTimer.Name, nSoPhutTimer.Value.ToString());
            this.Close();
        }

        private class chitiet_thuoc
        {
            public object biet_duoc { get; set; }
            public object ten_thuoc { get; set; }
            public object don_vi_tinh { get; set; }
            public object ma_thuoc { get; set; }
            public object so_luong { get; set; }
            public object cach_dung { get; set; }
        }

        private void chkAll_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)grList.DataSource;
            foreach (DataRow r in dt.Rows)
                r["chon"] = chkAll.Checked;
        }

        private class don_thuoc_lien_thong
        {
            public string loai_don_thuoc { get; set; }
            public string ma_don_thuoc { get; set; }
            public string ten_benh_nhan { get; set; }
            public string tuoi_benh_nhan { get; set; }
            public string ngay_sinh_benh_nhan { get; set; }
            public string can_nang { get; set; }
            public string gioi_tinh { get; set; }
            public string ma_so_the_bao_hiem_y_te { get; set; }
            public string thong_tin_nguoi_giam_ho { get; set; }
            public string dia_chi { get; set; }
            public string socmnd { get; set; }
            public string ma_chan_doan { get; set; }
            public string chan_doan { get; set; }
            public string ket_luan { get; set; }
            public string luu_y { get; set; }
            public string hinh_thuc_dieu_tri { get; set; }
            public string dot_dung_thuoc { get; set; }
            public List<chitiet_thuoc> thongtindonthuoc { get; set; }
            public string loi_dan { get; set; }
            public string so_dien_thoai_nguoi_kham_benh { get; set; }
            public string ngay_tai_kham { get; set; }
            public string ngay_gio_ke_don { get; set; }
            public string tu_ngay { get; set; }
            public string den_ngay { get; set; }
            public string bacsiky { get; set; }
            public string tenbs { get; set; }

        }

        private void nSoPhutTimer_ValueChanged(object sender, EventArgs e)
        {
            if (nSoPhutTimer.Value>=5)
            {
                timer1.Start();
                timer1.Interval =  (int)nSoPhutTimer.Value * 60000;
            }
            else if(nSoPhutTimer.Value <5 && nSoPhutTimer.Value > 0)
            {
                MessageBox.Show("Vui lòng chọn 5 phút trở lên");
                nSoPhutTimer.Focus();
            }
            else
            {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (nSoPhutTimer.Value>=5)
            {
              
                if (string.IsNullOrEmpty(ma_lien_thong_co_so_kham_chua_benh) || string.IsNullOrEmpty(pass_lien_thong_co_so_kham_chua_benh))
                {
                    LibUtility.Utility.showPopup("Chưa khai báo mã liên thông KCB!");
                    return;
                }
                reload_data(true);
                gui_don_thuoc_tu_dong(true);
            }
        }

        private class Lienthongbs_info
        {
            public string cskb { get; set; }
            public string ma_lien_thong_bac_si { get; set; }
            public string ma_lien_thong_cskcb { get; set; }
            public string password_lienthong_bs { get; set; }
            public string password_lienthong_cs { get; set; }
        }
    }
}
