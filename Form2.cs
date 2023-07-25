
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ImportXML
{
    public partial class Form2 : Form
    {
        private LibHIS.AccessData m;
        private LibHIS.AccessData d;
        private OracleHelper.OracleSupport oracle = new OracleHelper.OracleSupport();
        private string sql = "";
        public Form2()
        {
            InitializeComponent();
            
            m = new LibHIS.AccessData(oracle);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DataSet ds = m.get_data("select * from dlogin");
        }

        private void btnnoicapbhyt_Click(object sender, EventArgs e)
        {

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    List<LichSuKhamChuaBenh> listdtkcb = new List<LichSuKhamChuaBenh>();
            
        //    listdtkcb = get_nhanLichsuKCB2019("92132_BV", "a66a1c9e85455576c4e2d06d5a996e89", "0791970061", "NGUYỄN NỮ GIA PHỤNG", "06/05/1997", "1997",2,"", "","", "", "002"); // Nữ 2 ; Nam=1
        //}
        public class LichSuKhamChuaBenh
        {
            public string maHoSo { get; set; }
            public string maCSKCB { get; set; }
            public string ngayVao { get; set; }
            public string ngayRa { get; set; }
            public string tenBenh { get; set; }
            public string tinhTrang { get; set; }
            public string keDieuTri { get; set; }
            public string lyDoVV { get; set; }
            public string TEMP1 { get; set; }
            public string TEMP2 { get; set; }
            public string TEMP3 { get; set; }
            public string TEMP4 { get; set; }
            public string TEMP5 { get; set; }


        }
        public class LichSuKT2018
        {
            public string UserKT { get; set; }
            public string thoiGianKT { get; set; }
            public string thongBao { get; set; }
            public string maLoi { get; set; }

        }
        public class nhanLichsuKCB2019_gui
        {
            public string maThe { get; set; }
            public string hoTen { get; set; }
            public string ngaySinh { get; set; }

        }
        public class Thongtuyenbaohiem
        {
            public string maKetQua { get; set; }
            public string ghiChu { get; set; }
            public string maThe { get; set; }
            public string hoTen { get; set; }
            public string ngaySinh { get; set; }
            public string gioiTinh { get; set; }
            public string diaChi { get; set; }
            public string maDKBD { get; set; }
            public string cqBHXH { get; set; }
            public string gtTheTu { get; set; }
            public string gtTheDen { get; set; }
            public string maKV { get; set; }
            public string ngayDu5Nam { get; set; }
            public string maSoBHXH { get; set; }
            public string maTheCu { get; set; }
            public string maTheMoi { get; set; }
            public string gtTheTuMoi { get; set; }
            public string gtTheDenMoi { get; set; }
            public string maDKBDMoi { get; set; }
            public string tenDKBDMoi { get; set; }
            public string thongbao { get; set; }
            public bool thanhcong { get; set; }
            public bool tracuu { get; set; }

            public string makiemtra_mathe { get; set; }
            public string makiemtra_hoten { get; set; }
            public string makiemtra_ngaysinh { get; set; }
            public string makiemtra_namsinh { get; set; }
            public string makiemtra_gioitinh { get; set; }
            public string makiemtra_ngaybd { get; set; }
            public string makiemtra_ngaykt { get; set; }
            public string makiemtra_macskbbd { get; set; }
            public string ghiChuMoi { get; set; }
        }
        public class LoginClass
        {
            public string UserName { get; set; }
            public string Password { get; set; }

        }
        public class Token
        {
            public int maKetQua { get; set; }
            public APIKey APIKey { get; set; }
        }
        public class APIKey
        {
            public string access_token { get; set; }
            public string id_token { get; set; }
            public string token_type { get; set; }
            public string username { get; set; }
            public string expires_in { get; set; }
        }
        public string s_token = "", s_idtoken = "", s_mahoso = "";
        //public void get_token(string _username, string _password)
        //{
        //    try
        //    {
        //        LoginClass login = new LoginClass();
        //        login.UserName = _username;
        //        login.Password = _password;
        //        string data = JsonConvert.SerializeObject(login);
        //       Uri address = new Uri(LibConfig.ConfigManager.UrlGiamdinh + "/api/token/take");
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
        //        request.Method = "post";
        //        request.ContentType = "application/json";
        //        request.Timeout = 20000;
        //        string response = null;
        //        using (Stream s = request.GetRequestStream())
        //        {
        //            using (StreamWriter stw = new StreamWriter(s))
        //                stw.Write(data);
        //        }
        //        using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
        //        {
        //            var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
        //            response = reader.ReadToEnd();
        //        }
        //        Token tok = JsonConvert.DeserializeObject<Token>(response);
        //        s_token = tok.APIKey.access_token;
        //        s_idtoken = tok.APIKey.id_token;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Message.ToString();
        //    }

        //}
        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public string get_MD5(string s)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                s = GetMd5Hash(md5Hash, s);
            }
            return s;
        }
        public class nhanLichsuKCB2019_nhan
        {

            public string maKetQua { get; set; }
            public string ghiChu { get; set; }
            public string maThe { get; set; }
            public string hoTen { get; set; }
            public string ngaySinh { get; set; }
            public string gioiTinh { get; set; }
            public string diaChi { get; set; }
            public string maDKBD { get; set; }
            public string cqBHXH { get; set; }
            public string gtTheTu { get; set; }
            public string gtTheDen { get; set; }
            public string maKV { get; set; }
            public string ngayDu5Nam { get; set; }
            public string maSoBHXH { get; set; }
            public string maTheCu { get; set; }
            public string maTheMoi { get; set; }
            public string gtTheTuMoi { get; set; }
            public string gtTheDenMoi { get; set; }
            public string maDKBDMoi { get; set; }
            public string tenDKBDMoi { get; set; }
            public List<LichSuKhamChuaBenh> dsLichSuKCB2018 { get; set; }
            public List<LichSuKT2018> dsLichSuKT2018 { get; set; }
        }
        
        public List<LichSuKhamChuaBenh> lsLichSuKhamChuaBenh2019 = new List<LichSuKhamChuaBenh>();
        public List<LichSuKT2018> lsLichSuKT2019 = new List<LichSuKT2018>();
        Thongtuyenbaohiem _Thongtuyenbaohiem;
        //public List<LichSuKhamChuaBenh> get_nhanLichsuKCB2019(string _username, string _password, string _mathe, string _hoten, string _ngaysinh, string _namsinh, int _gioitinh, string _ngaybd, string _ngaykt, string _macskbbd, string _mabn, string makp)
        //{
        //    if (LibConfig.ConfigManager.isUserPass_Vetinh && makp != "")
        //    {
        //        sql = "select b.USERGIAMDINH,b.PASSWORDGIAMDINH,b.mabv from btdkp_bv a left join dmbenhvienvetinh b on a.idbenhvienvetinh=b.id where makp='" + makp.ToString() + "'";
        //        foreach (DataRow r in m.get_data(sql).Tables[0].Rows)
        //        {
        //            if (!string.IsNullOrEmpty(r["USERGIAMDINH"].ToString().Trim()) && !string.IsNullOrEmpty(r["PASSWORDGIAMDINH"].ToString().Trim()))
        //            {
        //                _username = r["USERGIAMDINH"].ToString();
        //                _password = r["PASSWORDGIAMDINH"].ToString();
        //                _password = get_MD5(_password);
        //            }
        //        }
        //    }


        //    s_mahoso = "";
        //    List<LichSuKhamChuaBenh> dt1 = new List<LichSuKhamChuaBenh>();
        //    List<LichSuKT2018> dt2 = new List<LichSuKT2018>();
        //    lsLichSuKhamChuaBenh2019 = new List<LichSuKhamChuaBenh>();
        //    lsLichSuKT2019 = new List<LichSuKT2018>();
        //    if (_mathe == "")
        //    {
        //        LibUtility.Utility.MsgBox("Số thẻ không hợp lệ");
        //        return null;
        //    }
        //    if (_hoten == "")
        //    {
        //        LibUtility.Utility.MsgBox("Họ tên không hợp lệ");
        //        return null;
        //    }
        //    //if (_macskbbd == "")
        //    //{
        //    //    LibUtility.Utility.MsgBox("Mã bệnh viện không hợp lệ");
        //    //    return null;
        //    //}

        //    nhanLichsuKCB2019_gui lichsuKCB = new nhanLichsuKCB2019_gui();
        //    lichsuKCB.maThe = _mathe;
        //    lichsuKCB.hoTen = _hoten;
        //    lichsuKCB.ngaySinh = _ngaysinh == "" ? _namsinh : _ngaysinh;
        //    string data = JsonConvert.SerializeObject(lichsuKCB);

        //    get_token(_username, _password);
        //    Uri address = new Uri(LibConfig.ConfigManager.UrlGiamdinh + "/api/egw/KQNhanLichSuKCB2019?token=" + s_token + "&id_token=" + s_idtoken + "&username=" + _username + "&password=" + _password);
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
        //    request.Method = "post";
        //    request.ContentType = "application/json";
        //    request.Timeout = 20000;
        //    string response = null;
        //    using (Stream s = request.GetRequestStream())
        //    {
        //        using (StreamWriter stw = new StreamWriter(s))
        //            stw.Write(data);
        //    }
        //    using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
        //    {
        //        var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
        //        response = reader.ReadToEnd();
        //    }
        //    nhanLichsuKCB2019_nhan tok = JsonConvert.DeserializeObject<nhanLichsuKCB2019_nhan>(response);

        //    string _maketqua = tok.maKetQua;

        //    _Thongtuyenbaohiem = new Thongtuyenbaohiem();
        //    _Thongtuyenbaohiem.maKetQua = tok.maKetQua;
        //    _Thongtuyenbaohiem.ghiChu = tok.ghiChu;
        //    _Thongtuyenbaohiem.maThe = tok.maThe;
        //    _Thongtuyenbaohiem.hoTen = tok.hoTen;
        //    _Thongtuyenbaohiem.ngaySinh = tok.ngaySinh;
        //    _Thongtuyenbaohiem.gioiTinh = tok.gioiTinh;
        //    _Thongtuyenbaohiem.diaChi = tok.diaChi;
        //    _Thongtuyenbaohiem.maDKBD = tok.maDKBD;
        //    _Thongtuyenbaohiem.cqBHXH = tok.cqBHXH;
        //    _Thongtuyenbaohiem.gtTheTu = tok.gtTheTu;
        //    _Thongtuyenbaohiem.gtTheDen = tok.gtTheDen;
        //    _Thongtuyenbaohiem.maKV = tok.maKV;
        //    _Thongtuyenbaohiem.ngayDu5Nam = tok.ngayDu5Nam;
        //    _Thongtuyenbaohiem.maSoBHXH = tok.maSoBHXH;
        //    _Thongtuyenbaohiem.maTheCu = tok.maTheCu;
        //    _Thongtuyenbaohiem.maTheMoi = tok.maTheMoi;
        //    _Thongtuyenbaohiem.gtTheTuMoi = tok.gtTheTuMoi;
        //    _Thongtuyenbaohiem.gtTheDenMoi = tok.gtTheDenMoi;
        //    _Thongtuyenbaohiem.maDKBDMoi = tok.maDKBDMoi;
        //    _Thongtuyenbaohiem.tenDKBDMoi = tok.tenDKBDMoi;

        //    _Thongtuyenbaohiem.makiemtra_mathe = _mathe;
        //    _Thongtuyenbaohiem.makiemtra_hoten = _hoten;
        //    _Thongtuyenbaohiem.makiemtra_ngaysinh = _ngaysinh;
        //    _Thongtuyenbaohiem.makiemtra_namsinh = _namsinh;
        //    _Thongtuyenbaohiem.makiemtra_ngaybd = _ngaybd;
        //    _Thongtuyenbaohiem.makiemtra_ngaykt = _ngaykt;
        //    _Thongtuyenbaohiem.makiemtra_macskbbd = _macskbbd;

        //    _Thongtuyenbaohiem.thanhcong = false;
        //    _Thongtuyenbaohiem.tracuu = false;
        //    StringBuilder stringBuilder = new StringBuilder("");
        //    if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.maTheMoi) && _maketqua != "000")
        //    {
        //        stringBuilder.Append("\r\n- Số thẻ mới: " + _Thongtuyenbaohiem.maTheMoi);
        //    }
        //    if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.maDKBDMoi) && _maketqua != "000") stringBuilder.Append("\r\n- Mã KCBBĐ mới: " + _Thongtuyenbaohiem.maDKBDMoi);
        //    if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.gtTheTuMoi) && _maketqua != "000") stringBuilder.Append("\r\n- Từ ngày mới: " + _Thongtuyenbaohiem.gtTheTuMoi);
        //    if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.gtTheDenMoi) && _maketqua != "000") stringBuilder.Append("\r\n- Đến ngày mới: " + _Thongtuyenbaohiem.gtTheDenMoi);
        //    if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.tenDKBDMoi) && _maketqua != "000") stringBuilder.Append("\r\n- Nơi KCBBĐ mới: " + _Thongtuyenbaohiem.tenDKBDMoi);
        //    if (stringBuilder.ToString().Trim() != "" && _maketqua != "000")
        //    {
        //        _Thongtuyenbaohiem.ghiChuMoi = stringBuilder.ToString().Trim();
        //    }
        //    if (_maketqua == "000")
        //    {
        //        bool isOk = true;
        //        if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.maDKBDMoi) && !string.IsNullOrEmpty(_Thongtuyenbaohiem.gtTheTuMoi))
        //        {
        //            if (LibUtility.Utility.bNgay(m.ngayhienhanh_server, _Thongtuyenbaohiem.gtTheTuMoi))
        //            {
        //                if (_Thongtuyenbaohiem.maDKBDMoi != _Thongtuyenbaohiem.makiemtra_macskbbd)
        //                {
        //                    _Thongtuyenbaohiem.thanhcong = false;
        //                    _Thongtuyenbaohiem.thongbao = "Mã ĐKKCBBĐ " + _macskbbd + " khác so với Mã ĐKKCBBĐ  trên cổng " + _Thongtuyenbaohiem.maDKBDMoi;
        //                    _Thongtuyenbaohiem.ghiChu = _Thongtuyenbaohiem.thongbao + "\r\n----------------------------------------------------------------\r\n" + _Thongtuyenbaohiem.ghiChu;
        //                    isOk = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(_Thongtuyenbaohiem.maDKBD) && !string.IsNullOrEmpty(_Thongtuyenbaohiem.makiemtra_macskbbd) && _Thongtuyenbaohiem.maDKBD != _Thongtuyenbaohiem.makiemtra_macskbbd)
        //            {
        //                _Thongtuyenbaohiem.thanhcong = false;
        //                _Thongtuyenbaohiem.thongbao = "Mã ĐKKCBBĐ " + _macskbbd + " khác so với Mã ĐKKCBBĐ  trên cổng " + _Thongtuyenbaohiem.maDKBD;
        //                _Thongtuyenbaohiem.ghiChu = _Thongtuyenbaohiem.thongbao + "\r\n---------------------------------------------------------------------------------\r\n" + _Thongtuyenbaohiem.ghiChu;
        //                isOk = false;
        //            }
        //        }
        //        if (isOk)
        //        {
        //            if (_Thongtuyenbaohiem.gioiTinh != m.get_tenphai(_mabn, _gioitinh))
        //            {
        //                _Thongtuyenbaohiem.thanhcong = false;
        //                _Thongtuyenbaohiem.thongbao = "Giới tính bệnh nhân đã nhập: " + m.get_tenphai(_mabn, _gioitinh) + ", khác với giới tính khai báo trên cổng giám định: " + _Thongtuyenbaohiem.gioiTinh;
        //                isOk = false;
        //            }
        //        }
        //        if (isOk)
        //        {
        //            _Thongtuyenbaohiem.thanhcong = true;
        //            _Thongtuyenbaohiem.tracuu = true;
        //            _Thongtuyenbaohiem.ghiChu = _Thongtuyenbaohiem.ghiChu;
        //            _Thongtuyenbaohiem.thongbao = "Kiểm tra thành công.";
        //        }

        //    }
        //    else if (_maketqua == "001")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = true;
        //        _Thongtuyenbaohiem.tracuu = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ do BHXH Bộ Quốc Phòng quản lý, đề nghị kiểm tra thẻ và thông tin giấy tờ tuỳ thân.";
        //    }
        //    else if (_maketqua == "002")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = true;
        //        _Thongtuyenbaohiem.tracuu = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ do BHXH Bộ Công An quản lý, đề nghị kiểm tra thẻ và thông tin giấy tờ tuỳ thân.";
        //    }
        //    else if (_maketqua == "003")
        //    {
        //        _Thongtuyenbaohiem.tracuu = true;
        //        _Thongtuyenbaohiem.thanhcong = true;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ cũ hết giá trị sử dụng nhưng được cấp thẻ mới.";

        //    }
        //    else if (_maketqua == "004")
        //    {
        //        _Thongtuyenbaohiem.tracuu = true;
        //        _Thongtuyenbaohiem.thanhcong = true;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ cũ còn giá trị sử dụng nhưng được cấp thẻ mới.";
        //    }
        //    else if (_maketqua == "010")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ hết giá trị sử dụng.";
        //    }
        //    else if (_maketqua == "051")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Mã thẻ không đúng.";
        //    }
        //    else if (_maketqua == "052")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Mã quyền lợi thẻ (ký tự thứ 4,5 của mã thẻ) không đúng.";
        //    }
        //    else if (_maketqua == "053")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Mã quyền lợi thẻ (ký tự thứ 3 của mã thẻ) không đúng.";

        //    }
        //    else if (_maketqua == "050")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Không tìm thấy thông tin thẻ bhyt.";
        //    }
        //    else if (_maketqua == "060")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ sai họ tên.";
        //    }
        //    else if (_maketqua == "061")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ sai họ tên (đúng ký tự đầu).";
        //    }
        //    else if (_maketqua == "070")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ sai ngày sinh.";
        //    }
        //    else if (_maketqua == "100")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Lỗi khi lấy dữ liệu số thẻ.";
        //    }
        //    else if (_maketqua == "101")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Lỗi Server BHXH.";
        //    }
        //    else if (_maketqua == "110")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ đã thu hồi.";
        //    }
        //    else if (_maketqua == "120")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ đã báo giảm.";
        //    }
        //    else if (_maketqua == "121")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ đã báo giảm. Chuyển ngoại tỉnh.";
        //    }
        //    else if (_maketqua == "122")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ đã báo giảm. Chuyển nội tỉnh.";
        //    }
        //    else if (_maketqua == "123")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ đã báo giảm. Thu hồi do tăng cùng đơn vị.";
        //    }
        //    else if (_maketqua == "124")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Thẻ đã báo giảm. Ngừng tham gia.";
        //    }
        //    else if (_maketqua == "130")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = true;
        //        _Thongtuyenbaohiem.tracuu = false;
        //        _Thongtuyenbaohiem.thongbao = "Trẻ em không xuất trình thẻ.";

        //    }
        //    else if (_maketqua == "205")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Lỗi sai tham số truyền vào.";
        //    }
        //    else if (_maketqua == "401")
        //    {
        //        _Thongtuyenbaohiem.thanhcong = false;
        //        _Thongtuyenbaohiem.thongbao = "Lỗi xác thực tài khoản.";
        //    }
        //    dt1 = tok.dsLichSuKCB2018;
        //    lsLichSuKhamChuaBenh2019 = tok.dsLichSuKCB2018;
        //    lsLichSuKT2019 = tok.dsLichSuKT2018;


        //    return dt1;
        //}

    }
}