using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using LibUtility;
using Newtonsoft.Json;
using LibHIS;
using System.Xml.Serialization;
using System.Net;
using ImportXML;

namespace ImportXML
{
    public partial class Form1 : Form
    {

        private LibHIS.AccessData d;
        private OracleHelper.OracleSupport oracle = new OracleHelper.OracleSupport();

        private string thumuc = @"D:\Source\MySource\Nhathuoc\xmldanhmuc";
        private DataSet ds = new DataSet();
        private DataTable dt, dtTmp;
        private int i_nhommau = 2, iHang, iNuoc;
        private long _id = 0;
        DataRow dr;
        private string _ten = "", _tenTmp = "", user = "", kdau = "", sql = "", mmyy = "";
        private int i_nhomkho = 0;

        public Form1()
        {
            InitializeComponent();
            d = new LibHIS.AccessData(oracle);
        }

        private string getMahc(string s, string hamluong)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            if (s == "")
            {
                s = "000001+";
                return s;
            }
            DataTable dthc = d.get_data("select * from d_dmhoatchat").Tables[0];
            string s2 = s + "+", s3 = "", mahc = "", ma = "", kdau = "";
            int len = s2.Length;
            DataRow r;
            for (int i = 0; i < len; i++)
            {
                if (s2.Substring(i, 1) == "+")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(s3);
                    r = d.getrowbyid(dthc, "khongdau='" + kdau.Trim() + "'");
                    if (r == null)
                    {
                        ma = d.getMabd("d_dmhoatchat", s3, 1);
                        mahc += ma + "+";
                        if (!d.upd_dmhoatchat(ma, s3.Trim(), i_nhom, "", 0, "", ""))
                        {
                            MessageBox.Show("error");
                        }
                        d.execute_data("update d_dmhoatchat set khongdau='" + kdau + "',hamluong='" + hamluong + "' where ma='" + ma + "'");
                    }
                    else
                    {
                        mahc += r["ma"].ToString() + "+";
                    }
                    s3 = "";
                }
                else s3 += s2.Substring(i, 1);
            }
            return mahc;
        }

        private void nhom_Click(object sender, EventArgs e)
        {
            if (!check_column_excel_giavp()) return;
            DataTable dtTmp;
            string _khongdau = "", _ten = "", m_id = "";
            DataRow dr;
            foreach (DataRow r in dt.Rows)
            {
                _ten = r["nhoM"].ToString();
                if (_ten != "")
                {
                    dtTmp = d.get_data("select ma,KHONGDAU from v_nhomvp").Tables[0];

                    _ten = r["nhom"].ToString();
                    _khongdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    dr = d.getrowbyid(dtTmp, "KHONGDAU='" + _khongdau + "'");
                    if (dr == null)
                    {
                        m_id = d.get_id_v_nhomvp.ToString();
                        if (!d.upd_v_nhomvp(decimal.Parse(m_id), decimal.Parse(m_id), _ten.Trim(), m_id, m_id, 1, 0))
                        {
                            LibUtility.Utility.MsgBox("Error");
                        }
                        d.execute_data("update v_nhomvp set KHONGDAU='" + _khongdau + "' where ma='" + m_id + "'");
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private string getMahc(string s, string hamluong, int nhom)
        {
            if (s == "")
            {
                s = "000001+";
                return s;
            }
            DataTable dthc = d.get_data("select * from d_dmhoatchat where nhom=" + nhom).Tables[0];
            string s2 = s + "+", s3 = "", mahc = "", ma = "", kdau = "";
            int len = s2.Length;
            DataRow r;
            for (int i = 0; i < len; i++)
            {
                if (s2.Substring(i, 1) == "+")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(s3);
                    r = d.getrowbyid(dthc, "khongdau='" + kdau.Trim() + "'");
                    if (r == null)
                    {
                        ma = d.getMabd("d_dmhoatchat", s3, nhom);
                        mahc += ma + "+";
                        d.upd_dmhoatchat(ma, s3.Trim(), nhom, "", 0, "", "");
                        d.execute_data("update d_dmhoatchat set khongdau='" + kdau + "',hamluong='" + hamluong + "' where ma='" + ma + "'");
                    }
                    else mahc += r["ma"].ToString() + "+";
                    s3 = "";
                }
                else s3 += s2.Substring(i, 1);
            }
            return mahc;
        }

        private void button118_Click(object sender, EventArgs e)
        {
            if (path.Text.Trim() == "")
            {
                MessageBox.Show("Chọn file import", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dt = get_excel(path.Text).Tables[0];
            try
            {
                if (dataGridView1.DataSource != null)
                    gridView1.Columns.Clear();
            }
            catch { }

            dataGridView1.DataSource = dt;
        }

        private void loaivp_Click(object sender, EventArgs e)
        {
            DataTable dtTmp, dtTmpLoai;
            string _khongdau_nhom = "", _khongdau_loai = "", m_id = "", _loai = "", _idnhom = "", _nhom = "";
            DataRow dr, drloai;
            if (!check_column_excel_giavp())
            {
                dt = new DataTable();
            }
            foreach (DataRow r in dt.Rows)
            {
                _loai = r["LOAI"].ToString().Trim();
                _nhom = r["NHOM"].ToString().Trim();
                if (_loai != "")
                {
                    dtTmpLoai = d.get_data("select id,ten,khongdau from mqhisroot.v_loaivp").Tables[0];

                    _khongdau_nhom = LibUtility.Utility.Hoten_khongdau(_nhom);
                    _khongdau_loai = LibUtility.Utility.Hoten_khongdau(_loai);
                    drloai = d.getrowbyid(dtTmpLoai, "khongdau='" + _khongdau_loai + "'");
                    if (drloai == null)
                    {
                        dtTmp = d.get_data("select ma,ten,khongdau from mqhisroot.v_nhomvp").Tables[0];

                        dr = d.getrowbyid(dtTmp, "khongdau='" + _khongdau_nhom + "'");
                        if (dr != null)
                        {
                            _idnhom = dr["ma"].ToString();
                            m_id = d.get_id_v_loaivp.ToString();
                            if (!d.upd_v_loaivp(decimal.Parse(m_id), decimal.Parse(_idnhom), decimal.Parse(m_id), m_id, _loai, m_id, 1, LibUtility.Utility.getComputername, 0, ""))
                            {
                                MessageBox.Show("Error");
                                return;
                            }
                            d.execute_data("update v_loaivp set KHONGDAU='" + _khongdau_loai + "' where id='" + m_id + "'");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhóm");
                            return;
                        }
                    }



                }
            }
            if (dt.Rows.Count > 0) MessageBox.Show("OK!");
        }

        private void giavp_Click(object sender, EventArgs e)
        {
            if (!check_column_excel_giavp())
            {
                dt = new DataTable();
            }
            try
            {
                decimal _stt = 0, _kythuat = -1, MABH = 0, _chenhlech = 0;
                decimal _id = 0, _gia_th = 0, _gia_bh = 0, _gia_dv = 0, _gia_nn = 0, _gia_cs = 0, _gia_ksk = 0, _nhombaocao=0;
                string _ten = "", _ma = "", _MATD = "", _MATT50 = "", _TENTT50 = "", _IDLOAI = "", _MATT37 = "", _TENTT37 = "", bhyttra = "", _TENTT43, _MATT43, _khongdau_loai = "", MAGIUONG = "",_khongdaunhombaocao="";
                string _MA_GIA = "", _QUYET_DINH = "", _CONG_BO = "";

                DataTable dtTmpLoai = d.get_data("select id,ten,khongdau from mqhisroot.v_loaivp").Tables[0];
                DataTable dtTmpnhombaocao = d.get_data("select id,ten,khongdau from mqhisroot.v_nhombaocao").Tables[0];

                foreach (DataRow r in dt.Rows)
                {
                    _ten = r["ten"].ToString();
                    if (_ten != "")
                    {
                        _khongdau_loai = LibUtility.Utility.Hoten_khongdau(r["loai"].ToString().Trim());
                        dr = d.getrowbyid(dtTmpLoai, "khongdau='" + _khongdau_loai + "'");
                        if (dr != null)
                        {
                            try
                            {
                                _id = decimal.Parse(r["ID"].ToString());
                            }
                            catch { _id = 0; }

                            if (_id == 0) _id = d.get_id_v_giavp;
                            else
                            {
                                d.execute_data("update v_giavp set  hide=1 where id=" + _id);
                            }
                            _IDLOAI = dr["id"].ToString();


                            _khongdaunhombaocao = LibUtility.Utility.Hoten_khongdau(r["nhom_baocao"].ToString().Trim());
                            dr = d.getrowbyid(dtTmpnhombaocao, "khongdau='" + _khongdaunhombaocao + "'");
                            if (dr != null)
                            {
                                try
                                {
                                    _nhombaocao = decimal.Parse(dr["ID"].ToString());
                                }
                                catch { _nhombaocao = 0; }

                            }
                        
                              


                            if (r["stt"].ToString() != "") _stt = decimal.Parse(r["stt"].ToString());
                            else _stt = _id;


                            _TENTT43 = r["TENTT43"].ToString().Trim();

                            _MATD = r["MATUONGDUONG"].ToString();
                            _MATT50 = r["MATT50"].ToString();
                            _TENTT50 = r["TENTT50"].ToString();
                            _MATT37 = r["MATT37"].ToString();
                            _MATT43 = r["MATT43"].ToString();
                            _TENTT37 = r["TENTT37"].ToString();
                           
                            MAGIUONG = r["MAGIUONG"].ToString();
                            _ma = r["MA"].ToString();
                            if(_ma=="") _ma = d.f_get_mavp(_ten.Trim());


                            try
                            {
                                _gia_bh = decimal.Parse(r["GIA_BH"].ToString());
                            }
                            catch { _gia_bh = 0; }
                            try
                            {
                                _gia_th = decimal.Parse(r["GIA_TH"].ToString());
                            }
                            catch { _gia_th = 0; }
                            try
                            {
                                _gia_dv = decimal.Parse(r["GIA_dv"].ToString());
                            }
                            catch { _gia_dv = 0; }

                            try
                            {
                                _gia_nn = decimal.Parse(r["GIA_NN"].ToString());
                            }
                            catch { _gia_nn = 0; }
                            try
                            {
                                _gia_cs = decimal.Parse(r["GIA_CS"].ToString());
                            }
                            catch { _gia_cs = 0; }
                            try
                            {
                                _gia_ksk = decimal.Parse(r["GIA_KSK"].ToString());
                            }
                            catch { _gia_ksk = 0; }
                            try
                            {
                                _kythuat = decimal.Parse(r["KTC"].ToString());
                            }
                            catch { _kythuat = -1; }

                            try
                            {
                                _chenhlech = decimal.Parse(r["chenhlech"].ToString());
                            }
                            catch { _chenhlech = -1; }

                            if (_gia_bh > 0) bhyttra = "100";
                            else bhyttra = "0";

                            _MA_GIA = r["MA_GIA"].ToString();
                            _QUYET_DINH = r["QUYET_DINH"].ToString();
                            _CONG_BO = r["CONG_BO"].ToString();
                            if (_CONG_BO != "")
                            {
                                _CONG_BO = _CONG_BO.Substring(6) + "/" + _CONG_BO.Substring(4, 2) + "/" + _CONG_BO.Substring(0, 4);
                            }
                            try
                            {
                                MABH = decimal.Parse(r["MABH"].ToString());
                            }
                            catch { MABH = 0; }

                            if (!d.upd_v_giavp(_id, decimal.Parse(_IDLOAI), _stt, _ma, _ten, r["DVT"].ToString(), _gia_th, _gia_bh, _gia_dv, _gia_nn, _gia_cs, 0, 0, 0, 0, 0, decimal.Parse(bhyttra), 0, 0, 0, 0, 0, 0, 0, "", 0, 1, 0, _gia_ksk, 0, 0, _kythuat, ""))
                            {
                                LibUtility.Utility.MsgBox("Eror");
                            }
                            d.upd_v_giavp(_id,_nhombaocao,"nhombaocao");
                            d.upd_v_giavp(_id, _MA_GIA, "MA_GIA");
                            d.upd_v_giavp(_id, _QUYET_DINH, "QUYETDINH");
                            d.upd_v_giavp(_id, MAGIUONG, "MAGIUONG");

                            d.upd_v_giavp(_id, _TENTT50, "TENTT50");
                            d.upd_v_giavp(_id, _MATT50, "MATT50");

                            d.upd_v_giavp(_id, _MATT43, "MATT43");
                            d.upd_v_giavp(_id, _TENTT43, "TENTT43");

                            d.upd_v_giavp(_id, _TENTT43, "TEN43");
                            d.upd_v_giavp(_id, _TENTT43, "TENTD");

                            d.upd_v_giavp(_id, _MATT37, "MATT37");
                            d.upd_v_giavp(_id, _TENTT37, "TENTT37");

                            d.upd_v_giavp(_id, _MATD, "ma5084");



                            d.upd_v_giavp(_id, _MATT50, "NGAYQUYETDINH");

                            if (_CONG_BO != "") d.execute_data("update v_giavp set  NGAYQUYETDINH=to_date('" + _CONG_BO + "','dd/mm/yyyy') where id=" + _id);
                            d.execute_data("update v_giavp set  mabh=" + MABH + ",chenhlech=" + _chenhlech + " where id=" + _id);
                            d.execute_data("update v_giavp set  hide=1 where id=" + _id);
                        }


                    }

                }
                LibUtility.Utility.MsgBox("OK");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void nhacc_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            if (!check_column_excel_duoc()) return;
            long _id = 0;
            string _ma = "", _ten = "", _khongdau = "";

            dtTmp = d.get_data("select * from d_dmnx where nhom=" + i_nhom + "").Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                _ten = r["TEN"].ToString().Trim();
                if (_ten != "")
                {
                    _ten = r["NHACC"].ToString().Trim();
                    if (_ten == "") _ten = "Không xác định";
                    _khongdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    DataRow dr = d.getrowbyid(dtTmp, "khongdau='" + _khongdau + "'");
                    if (dr == null)
                    {
                        _id = d.get_id_dmnx;
                        if (!d.upd_dmnx(_id, _id.ToString(), _ten, i_nhom, _id, 1, "", "", "", "", "", "", "", ""))
                        {
                            MessageBox.Show("Không cập nhật thông tin");
                            return;
                        }
                        d.execute_data("update d_dmnx set khongdau='" + _khongdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_dmnx where nhom=" + i_nhom).Tables[0];
                    }

                }
            }
            MessageBox.Show("OK");
        }

        private string getMahcs(string s, string hamluong, string matt)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            if (s == "")
            {
                s = "000001+";
                return s;
            }
            DataTable dthc = d.get_data("select * from d_dmhoatchat").Tables[0];
            string s2 = s + "+", s3 = "", mahc = "", ma = "", kdau = "";
            int len = s2.Length;
            DataRow r;
            for (int i = 0; i < len; i++)
            {
                if (s2.Substring(i, 1) == "+")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(s3);
                    r = d.getrowbyid(dthc, "khongdau='" + kdau.Trim() + "'");
                    if (r == null)
                    {
                        ma = d.getMabd("d_dmhoatchat", s3, 1);
                        mahc += ma + "+";
                        d.upd_dmhoatchat(ma, s3.Trim(), i_nhom, "", 0, "", "");
                        d.execute_data("update d_dmhoatchat set khongdau='" + kdau + "',hamluong='" + hamluong + "',mathongtu='" + matt + "' where ma='" + ma + "'");
                    }
                    else mahc += r["ma"].ToString() + "+";
                    s3 = "";
                }
                else s3 += s2.Substring(i, 1);
            }
            return mahc;
        }

        private void hang_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            long _id = 0;
            bool bOk = false;
            dtTmp = d.get_data("select * from d_dmhang where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "hang"))
            {
                if (r["hang"].ToString().Trim() != "")
                {
                    _ten = r["hang"].ToString().Trim();
                    _ten = _ten.Replace("'", "").Trim();
                    if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                    if (_ten != "")
                    {
                        kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                        dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                        if (dr == null)
                        {
                            _id = d.get_id_dmhang;
                            d.upd_dmhang(_id, _ten, 1, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                            d.execute_data("update d_dmhang set khongdau='" + kdau + "' where id=" + _id);
                            dtTmp = d.get_data("select * from d_dmhang where nhom=" + i_nhom).Tables[0];
                        }

                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void nuoc_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            dtTmp = d.get_data("select * from d_dmnuoc where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nuoc"))
            {
                _ten = r["nuoc"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {

                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        _id = d.get_id_dmnuoc;
                        d.upd_dmnuoc(_id, _ten, 1, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                        d.execute_data("update d_dmnuoc set khongdau='" + kdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_dmnuoc where nhom=" + i_nhom).Tables[0];
                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void nhombd_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "", nhomin = "";
            DataTable dtTmpNhomin = d.get_data("select * from d_nhomin where nhom=" + i_nhom).Tables[0];
            dtTmp = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
            DataRow drnhomin;
            foreach (DataRow r in dt.Select("true", "nhom"))
            {
                _ten = r["nhom"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {
                    nhomin = r["nhomin"].ToString();
                    nhomin = LibUtility.Utility.Hoten_khongdau(nhomin);
                    drnhomin = d.getrowbyid(dtTmpNhomin, "khongdau='" + nhomin + "'");
                    if (drnhomin != null)
                    {

                        kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                        dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                        if (dr == null)
                        {
                            try
                            {
                                _id = long.Parse(d.get_data("select max(id) from " + user + ".d_dmnhom").Tables[0].Rows[0][0].ToString()) + 1;
                            }
                            catch { _id = 1; }
                            d.upd_dmnhom(_id, _ten, 1, i_nhom, int.Parse(drnhomin["id"].ToString()), 0, LibUtility.Utility.get_stt(dtTmp), 0);
                            d.execute_data("update d_dmnhom set khongdau='" + kdau + "' where id=" + _id);
                            dtTmp = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
                        }
                    }
                }
            }
            MessageBox.Show("OK");


        }

        private void loaibd_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            foreach (DataRow r in dt.Select("true", "loai"))
            {
                _ten = r["loai"].ToString().Trim();
                if (_ten == "") _ten = "Không xác định";
                if (_ten != "")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    dtTmp = d.get_data("select * from d_dmloai where nhom=" + i_nhom).Tables[0];
                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        try
                        {
                            _id = long.Parse(d.get_data("select max(id) from " + user + ".d_dmloai").Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        catch { _id = 1; }
                        d.upd_dmloai(_id, _ten, 1, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                        d.execute_data("update d_dmloai set khongdau='" + kdau + "' where id=" + _id);
                    }
                }


            }
            MessageBox.Show("OK");
        }

        private void dmbd_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            if (!check_column_excel_duoc()) return;

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(0, "VẬT TƯ Y TẾ");
            dictionary.Add(1, "TÂN DƯỢC 100%");
            dictionary.Add(2, "CHẾ PHẨM YHCT");
            dictionary.Add(3, "VỊ THUỐC YHCT");
            dictionary.Add(4, "PHÓNG XẠ");
            dictionary.Add(5, "VỊ THUỐC YHCT(566)");
            dictionary.Add(6, "MÁU VÀ CHẾ PHẨM TỪ MÁU");
            dictionary.Add(7, "VẬT TƯ Y TẾ TRONG DANH MỤC BHYT");
            dictionary.Add(8, "VTYT THANH TOÁN THEO TỶ LỆ");
            dictionary.Add(9, "TÂN DƯỢC 50%");
            dictionary.Add(10, "TÂN DƯỢC 70%");
            dictionary.Add(11, "TÂN DƯỢC 30%");

            Dictionary<int, string> dictionary_thau = new Dictionary<int, string>();
            dictionary_thau.Add(0, "THẦU TẬP TRUNG");
            dictionary_thau.Add(1, "THẦU RIÊNG TẠI BỆNH VIỆN");
            dictionary_thau.Add(2, "NGOÀI THẦU");

            Dictionary<int, string> dictionary_congtacduoc = new Dictionary<int, string>();
            dictionary_congtacduoc.Add(4, "Hóa chất, xét nghiệm, thuốc thử(HC, TT)");
            dictionary_congtacduoc.Add(5, "Vật tư y tế tiêu hao(VT)");
            dictionary_congtacduoc.Add(6, "Vắc xin, sinh phẩm");
            dictionary_congtacduoc.Add(8, "Thuốc Kháng sinh(KS)");
            dictionary_congtacduoc.Add(9, "Thuốc Vitamin(Vit.)");
            dictionary_congtacduoc.Add(10, "Dịch truyền(DT)");
            dictionary_congtacduoc.Add(11, "Thuốc Corticoid(Cor.)");
            dictionary_congtacduoc.Add(12, "Thuốc khác");
            dictionary_congtacduoc.Add(23, "khác");



             var TTHAU_QD130 = string.Empty;
            var MA_PP_CHEBIEN = string.Empty;
            var MA_CSKCB_THUOC = string.Empty;
            var BAOCHE = string.Empty;

            string _mahc = "", _mabd = "", _nhom = "1", _loai = "1", _nuoc = "", _hang = "", _dang = "", _tenhc = "", _khongdau = "", _nhomin = "", _tt31 = "", _sodk = "", _bhyttra = "", _nhacckd = "", _STTQD = "", _MABYT = "", _hamluong = "", _DVSD = "", _nhacc = "", STTQD = "", madungchung = "", TTTHAU = "", ma_bv, nhombo = "", sotk = "", _nhomcongtacduoc = "";
            string MADUONGDUNG = "", DUONGDUNG = "", SODK = "", DONGGOI = "", QUYETDINH = "", CONGBO = "", LOAITHUOC = "", LOAITHAU = "", NHOMTHAU = "", MATHAU = "", maatc = "", namthau = "", SOHD = "", ngayhd = "", cacdung = "", nhomdieutri, nhomin;
            string MANHOMVTYT, TENNHOMVTYT;
            int _idnhom = 1, _idloai = 1, _idhang = 0, _idnuoc = 1, _idnhomin = 2, _idnhacc = 0, _nhombo = 0, _sotk = 0, _nhomdt = 0, _phuluc3=0;
            int d_stt = 0,_dmuc=0;
            decimal _tyle = 0, _dongia = 0, SLDONGGOI = 0, GIATHAU = 0, gia_bh = 0, slthau = 0, giaban = 0, giamua, giathau;
            DataTable dtNhom, dtLoai, dtHang, dtBd, dtNuoc, dtnhombo, dtsotk, dtNhomdieutri, dtNhomin;
            dtNhom = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
            dtLoai = d.get_data("select * from d_dmloai where nhom=" + i_nhom).Tables[0];
            dtBd = d.get_data("select * from d_dmbd where nhom=" + i_nhom).Tables[0];
            dtHang = d.get_data("select * from d_dmhang where nhom=" + i_nhom).Tables[0];
            dtNuoc = d.get_data("select * from d_dmnuoc where nhom=" + i_nhom).Tables[0];

            dtnhombo = d.get_data("select * from d_nhombo where nhom=" + i_nhom).Tables[0];
            dtsotk = d.get_data("select * from d_dmnhomkt where nhom=" + i_nhom).Tables[0];
            dtNhomdieutri = d.get_data("select * from d_dmnhomdt where nhom=" + i_nhom).Tables[0];
            dtNhomin = d.get_data("select * from d_nhomin where nhom=" + i_nhom).Tables[0];
            

            DataTable dtnhacc = d.get_data("select * from d_dmnx where nhom=" + i_nhom).Tables[0];
            int _stt = 1, hide = 0;
            decimal TLHAOHUT = 0;
            string NGAYHIEULUCTHAU = "", NGAYHETHIEULUCTHAU = "";
            foreach (DataRow r in dt.Select("true", "ten"))
            {
                if (r["ten"].ToString().Trim() != "")
                {
                    _ten = r["ten"].ToString().Trim();
                    _ten = _ten.Replace("\n", " ");
                    _khongdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    _hamluong = r["hamluong"].ToString().Trim();
                    _tenhc = r["tenhc"].ToString().Trim();
                    _mahc = getMahc(_tenhc, _hamluong);
                    madungchung = r["madungchung"].ToString();
                    cacdung = r["cachdung"].ToString();
                    TTTHAU = r["TTTHAU"].ToString().Trim();
                    _nhom = r["nhom"].ToString().Trim();
                    MANHOMVTYT = r["MANHOMVTYT"].ToString().Trim();
                    TENNHOMVTYT = r["TENNHOMVTYT"].ToString().Trim();
                    ma_bv = r["ma_bv"].ToString().Trim();


                    if (_nhom == "") _nhom = "Không xác định";
                    _nhom = LibUtility.Utility.Hoten_khongdau(_nhom);
                    dr = d.getrowbyid(dtNhom, "khongdau='" + _nhom + "'");
                    if (dr != null) _idnhom = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhóm");
                        return;
                    }

                    _loai = r["loai"].ToString().Trim();
                    if (_loai == "") _loai = "Không xác định";
                    _loai = LibUtility.Utility.Hoten_khongdau(_loai);
                    dr = d.getrowbyid(dtLoai, "khongdau='" + _loai + "'");
                    if (dr != null) _idloai = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Loại ,"+ r["ten"].ToString().Trim());
                        return;
                    }

                    _hang = r["hang"].ToString().Trim();
                    if (_hang == "") _hang = "Không xác định";
                    _hang = LibUtility.Utility.Hoten_khongdau(_hang);
                    dr = d.getrowbyid(dtHang, "khongdau='" + _hang + "'");
                    if (dr != null) _idhang = int.Parse(dr["id"].ToString());
                    else
                    {
                        _idhang = 0;
                        LibUtility.Utility.MsgBox("Hãng ," + r["ten"].ToString().Trim());
                        return;
                    }

                    _nhacc = r["nhacc"].ToString().Trim();
                    if (_nhacc == "") _nhacc = "Không xác định";
                    _nhacc = LibUtility.Utility.Hoten_khongdau(_nhacc);
                    dr = d.getrowbyid(dtnhacc, "khongdau='" + _nhacc + "'");
                    if (dr != null) _idnhacc = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhà cc ," + r["ten"].ToString().Trim());
                        return;
                    }

                    nhombo = r["nhombo"].ToString().Trim();
                    if (nhombo == "") nhombo = "Không xác định";
                    nhombo = LibUtility.Utility.Hoten_khongdau(nhombo);
                    dr = d.getrowbyid(dtnhombo, "khongdau='" + nhombo + "'");
                    if (dr != null) _nhombo = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhom bo ," + r["ten"].ToString().Trim());
                        return;
                    }

                    sotk = r["nhomketoan"].ToString().Trim();
                    if (sotk == "") sotk = "Không xác định";
                    sotk = LibUtility.Utility.Hoten_khongdau(sotk);
                    dr = d.getrowbyid(dtsotk, "khongdau='" + sotk + "'");
                    if (dr != null) _sotk = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhom ke toan ," + r["ten"].ToString().Trim());
                        return;
                    }


                    _nuoc = r["nuoc"].ToString().Trim();
                    if (_nuoc == "") _nuoc = "Không xác định";
                    _nuoc = LibUtility.Utility.Hoten_khongdau(_nuoc);
                    dr = d.getrowbyid(dtNuoc, "khongdau='" + _nuoc + "'");
                    if (dr != null) _idnuoc = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nước ," + r["ten"].ToString().Trim());
                        _idnuoc = 1;
                        return;
                    }

                    nhomdieutri = r["nhomdieutri"].ToString().Trim();
                    if (nhomdieutri == "") nhomdieutri = "Không xác định";
                    nhomdieutri = LibUtility.Utility.Hoten_khongdau(nhomdieutri);
                    dr = d.getrowbyid(dtNhomdieutri, "khongdau='" + nhomdieutri + "'");
                    if (dr != null) _nhomdt = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhóm điều trị ," + r["ten"].ToString().Trim());
                        return;
                    }

                    nhomin = r["nhomin"].ToString().Trim();
                    if (nhomin == "") nhomin = "Không xác định";
                    nhomin = LibUtility.Utility.Hoten_khongdau(nhomin);
                    dr = d.getrowbyid(dtNhomin, "khongdau='" + nhomin + "'");
                    if (dr != null) _idnhomin = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhóm in ," + r["ten"].ToString().Trim());
                        return;
                    }
                    _phuluc3 = 23;
                    _nhomcongtacduoc = r["nhomcongtacduoc"].ToString().Trim();
                    if (_nhomcongtacduoc == "")
                    {
                        _nhomcongtacduoc = "khác";
                        _phuluc3 = 23;
                    }
                    else 
                    {
                        _nhomcongtacduoc = LibUtility.Utility.Hoten_khongdau(_nhomcongtacduoc);

                        foreach (KeyValuePair<int, string> item1 in dictionary_congtacduoc)
                        {
                            if (LibUtility.Utility.Hoten_khongdau(item1.Value.ToUpper()) == _nhomcongtacduoc)
                            {
                                _phuluc3 = item1.Key;
                                break;
                            }
                        }
                    }

                    _dang = r["dvt"].ToString().Trim().ToLower();
                    _DVSD = r["DVSD"].ToString();
                    _bhyttra = "0";
                    _bhyttra = r["bhyt"].ToString().Replace("%", "").Trim();
                    try
                    {
                        _bhyttra = decimal.Parse(_bhyttra).ToString();
                    } catch
                    {
                        _bhyttra = "0";
                    }
                    try
                    {
                        TLHAOHUT = decimal.Parse(r["TLHAOHUT"].ToString());
                    }
                    catch
                    {
                        TLHAOHUT = 0;
                    }

                    try
                    {
                        hide = int.Parse(r["hide"].ToString());
                    }
                    catch
                    {
                        hide = 0;
                    }
                    _STTQD = r["sttqd"].ToString();
                    _MABYT = "";

                    try
                    {
                        _stt = int.Parse(r["stt"].ToString());
                    }
                    catch { _stt = 1; }
                    try
                    {
                        _dmuc = int.Parse(r["DINH_MUC"].ToString());
                    }
                    catch { _dmuc = 0; }
                    

                    MADUONGDUNG = r["MADUONGDUNG"].ToString();
                    DUONGDUNG = r["DUONGDUNG"].ToString();
                    SODK = r["SODK"].ToString();
                    DONGGOI = r["DONGGOI"].ToString();
                    QUYETDINH = r["QUYETDINH"].ToString();
                    CONGBO = r["CONGBO"].ToString();
                    if (CONGBO.Length == 8)
                    {
                        CONGBO = CONGBO.Substring(6) + "/" + CONGBO.Substring(4, 2) + "/" + CONGBO.Substring(0, 4);
                    }

                    NGAYHIEULUCTHAU = r["NGAYHIEULUCTHAU"].ToString();
                    if (NGAYHIEULUCTHAU.Length > 10) NGAYHIEULUCTHAU = NGAYHIEULUCTHAU.Substring(0, 10);
                    if (NGAYHIEULUCTHAU.Length == 8)
                    {
                        NGAYHIEULUCTHAU = NGAYHIEULUCTHAU.Substring(6) + "/" + NGAYHIEULUCTHAU.Substring(4, 2) + "/" + NGAYHIEULUCTHAU.Substring(0, 4);
                    }
                    if (NGAYHIEULUCTHAU != "")
                    {
                        if (NGAYHIEULUCTHAU != "") if (!LibUtility.Utility.bNgay(NGAYHIEULUCTHAU)) NGAYHIEULUCTHAU = "";
                    }

                    NGAYHETHIEULUCTHAU = r["NGAYHETHIEULUCTHAU"].ToString();
                    if (NGAYHETHIEULUCTHAU.Length == 8)
                    {
                        NGAYHETHIEULUCTHAU = NGAYHETHIEULUCTHAU.Substring(6) + "/" + NGAYHETHIEULUCTHAU.Substring(4, 2) + "/" + NGAYHETHIEULUCTHAU.Substring(0, 4);
                    }
                    if (NGAYHETHIEULUCTHAU.Length > 10) NGAYHETHIEULUCTHAU = NGAYHETHIEULUCTHAU.Substring(0, 10);

                    if (NGAYHETHIEULUCTHAU != "")
                    {
                        if (NGAYHETHIEULUCTHAU != "") if (!LibUtility.Utility.bNgay(NGAYHETHIEULUCTHAU)) NGAYHETHIEULUCTHAU = "";
                    }

                    LOAITHUOC = r["LOAITHUOC"].ToString();
                    if (LOAITHUOC == "") LOAITHUOC = "0";
                    else
                    {
                        foreach (KeyValuePair<int, string> item in dictionary)
                        {
                            if (LibUtility.Utility.Hoten_khongdau(item.Value) == LibUtility.Utility.Hoten_khongdau(r["LOAITHUOC"].ToString()).ToUpper().Trim())
                            {
                                LOAITHUOC = item.Key.ToString();
                                break;
                            }
                        }
                    }
                    LOAITHAU = r["LOAITHAU"].ToString();
                    if (LOAITHAU == "") LOAITHAU = "0";
                    else
                    {
                        foreach (KeyValuePair<int, string> item1 in dictionary_thau)
                        {
                            if (LibUtility.Utility.Hoten_khongdau(item1.Value.ToUpper()) == LibUtility.Utility.Hoten_khongdau(r["LOAITHAU"].ToString()).ToUpper().Trim())
                            {
                                LOAITHAU = item1.Key.ToString();
                                break;
                            }
                        }
                    }
                    NHOMTHAU = r["NHOMTHAU"].ToString();
                    try { SLDONGGOI = decimal.Parse(r["SLDONGGOI"].ToString()); } catch { SLDONGGOI = 0; }
                    try { GIATHAU = decimal.Parse(r["GIATHAU"].ToString()); } catch { GIATHAU = 0; }
                    try { slthau = decimal.Parse(r["slthau"].ToString()); } catch { slthau = 0; }
                    MATHAU = r["MATHAU"].ToString();
                    maatc = r["maatc"].ToString();
                    namthau = r["namthau"].ToString();
                    if (namthau.Length > 4)
                    {
                        try {
                            namthau = namthau.Split(' ')[0];
                            namthau = namthau.Split('/')[2];
                        } catch { namthau = ""; }
                    }
                    SOHD = r["SOHD"].ToString();
                    ngayhd = r["ngayhd"].ToString();
                    if (ngayhd.Length == 8)
                    {
                        ngayhd = ngayhd.Substring(6) + "/" + ngayhd.Substring(4, 2) + "/" + ngayhd.Substring(0, 4);
                    }
                    if (ngayhd.Length > 10) ngayhd = ngayhd.Substring(0, 10);
                    if (ngayhd != "")
                    {
                        if (ngayhd != "") if (!LibUtility.Utility.bNgay(ngayhd)) ngayhd = "";
                    }

                    _mabd = d.getMabd("d_dmbd", _ten, i_nhom);
                    _id = 0;
                    try { _id = long.Parse(r["id"].ToString()); } catch { _id = 0; }
                    if (_id == 0) _id = long.Parse(d.get_id_v_giavp.ToString());
                    if (!d.upd_dmbd(_id, _mabd, _ten, _dang, _hamluong, _idnhom, _idloai, _idhang, _idnuoc, _nhombo, int.Parse(_bhyttra), 0, _sotk, _tenhc, _mahc, 1, i_nhom, SODK.Trim(), 0, 0, 0, 0, _tyle, d_stt, "", _idnhomin, 0, "", _idnhacc, DONGGOI.Trim(), SLDONGGOI, 0, 0, 0, _DVSD, 0, "", DUONGDUNG, 0, 0))
                    {
                        MessageBox.Show("error");
                    }
                    try { _dongia = decimal.Parse(r["dongia"].ToString()); } catch { _dongia = 0; }
                    try { gia_bh = decimal.Parse(r["giabh"].ToString()); } catch { gia_bh = 0; }
                    try { giaban = decimal.Parse(r["giaban"].ToString()); } catch { giaban = 0; }
                    try { giamua = decimal.Parse(r["giamua"].ToString()); } catch { giamua = 0; }
                    try { giathau = decimal.Parse(r["giathau"].ToString()); } catch { giathau = 0; }
                    


                    sql = "update d_dmbd set MA2182='" + madungchung + "',qd05='" + _STTQD + "',maatc='" + maatc + "', dongia=" + _dongia + ",gia_bh=" + gia_bh + ",slthau=" + slthau + ",giaban=" + giaban + ",TLHAOHUT=" + TLHAOHUT + ",nhomdt=" + _nhomdt + ",mathau='" + MATHAU + "',namthau='" + namthau + "',GIATHAU=" + GIATHAU + ",hide=" + hide;
                    sql += ",sohd='" + SOHD + "',dmuc=" + _dmuc;
                    sql += ", phuluc3=" + _phuluc3;
                    if (ngayhd != "" && LibUtility.Utility.bNgay(ngayhd)) sql += ", ngayhd=to_date('" + ngayhd + "','dd/mm/yyyy')";
                    sql += " where id=" + _id;
                    if (!d.execute_data(sql))
                    {
                        MessageBox.Show("error");
                    }

                    TTHAU_QD130 = r["TTHAU_QD130"].ToString().Trim();
                    MA_PP_CHEBIEN = r["MA_PP_CHEBIEN"].ToString().Trim();
                    MA_CSKCB_THUOC = r["MA_CSKCB_THUOC"].ToString().Trim();
                    BAOCHE = r["BAOCHE"].ToString().Trim();
                    d.upd_dmbd_thongtu(_id, TTHAU_QD130, "TT_THAU_130");
                    d.upd_dmbd_thongtu(_id, MA_PP_CHEBIEN, "MA_PP_CHEBIEN");
                    d.upd_dmbd_thongtu(_id, MA_CSKCB_THUOC, "MA_CSKCB_THUOC");
                    d.upd_dmbd_thongtu(_id, BAOCHE, "BAOCHE");


                    d.upd_dmbd_col(_id, "MADUONGDUNG", MADUONGDUNG);
                    d.upd_dmbd_col(_id, "DUONGDUNG2182", MADUONGDUNG);
                    d.upd_dmbd_col(_id, "ma_bv", ma_bv);

                    d.upd_dmbd_ten_s(_id, r["tens"].ToString().Trim());
                    d.upd_d_dmbdthongtu(_id, int.Parse(LOAITHUOC), int.Parse(LOAITHAU));
                    d.upd_dmbd_thongtu(_id, QUYETDINH, "QUYETDINH");
                    if (CONGBO != "" && LibUtility.Utility.bNgay(CONGBO))
                    {
                        sql = "update d_dmbdthongtu set NGAYQD=to_date('" + CONGBO + "','dd/mm/yyyy') where id=" + _id;
                        d.execute_data(sql);
                    }
                    if (NGAYHIEULUCTHAU != "" && LibUtility.Utility.bNgay(NGAYHIEULUCTHAU))
                    {
                        sql = "update d_dmbdthongtu set NGAYHIEULUCTHAU=to_date('" + NGAYHIEULUCTHAU + "','dd/mm/yyyy') where id=" + _id;
                        d.execute_data(sql);
                    }
                    if (NGAYHETHIEULUCTHAU != "" && LibUtility.Utility.bNgay(NGAYHETHIEULUCTHAU))
                    {
                        sql = "update d_dmbdthongtu set NGAYHETHIEULUCTHAU=to_date('" + NGAYHETHIEULUCTHAU + "','dd/mm/yyyy') where id=" + _id;
                        d.execute_data(sql);
                    }
                    d.execute_data(sql);
                    try
                    {
                        LOAITHUOC = int.Parse(LOAITHUOC).ToString();
                    } catch { LOAITHUOC = "0"; }
                    try
                    {
                        LOAITHAU = int.Parse(LOAITHAU).ToString();
                    }
                    catch { LOAITHAU = "0"; }



                    if (LOAITHAU != "") d.upd_dmbd_thongtu(_id, LOAITHAU, "LOAITHAU");
                    if (NHOMTHAU != "") d.upd_dmbd_thongtu(_id, NHOMTHAU, "NHOMTHAU");
                    if (NHOMTHAU != "") d.upd_dmbd_thongtu(_id, NHOMTHAU, "NHOMTHAU");
                    if (LOAITHUOC != "") d.upd_dmbd_thongtu(_id, LOAITHUOC, "LOAITHUOC");
                    d.upd_dmbd_thongtu(_id, TTTHAU, "TTTHAU");
                    d.upd_dmbd_thongtu(_id, MANHOMVTYT, "MANHOMVTYT");
                    d.upd_dmbd_thongtu(_id, TENNHOMVTYT, "TENNHOMVTYT");
                    d.upd_dmbd_thongtu(_id, TENNHOMVTYT, "TENNHOMVTYT");
                    d.upd_dmbd_thongtu(_id, r["dang_bao_che"].ToString().Trim(), "baoche");
                    d.upd_dmbd_thongtu(_id, r["DONVITHAU"].ToString().Trim(), "DONVITHUCHIENTHAU");


                }
            }

            MessageBox.Show("Ok");
        }

        private void nhavien_Click(object sender, EventArgs e)
        {
            int stt = 1;
            string hoten = "";
            foreach (DataRow r in dt.Rows)
            {

                hoten = r["hoten"].ToString().Trim();
                if (!d.upd_dmbs(stt.ToString().PadLeft(4, '0'), hoten, "", "", 7, stt.ToString().PadLeft(4,'0'), 1, 1, r["phai"].ToString().ToUpper() == "NAM" ? 0 : 1, 0, "", "", r["ngaysinh"].ToString(),r["diachi"].ToString(), r["CMND"].ToString()))
                {
                    LibUtility.Utility.MsgBox("");
                    return;
                }

                d.execute_data("update " + user + ".dmbs set sochungchi='" + r["sochungchi"].ToString().Trim() + "' where ma='" + stt.ToString().PadLeft(4, '0') + "'");
                stt++;
            }
            MessageBox.Show("OK");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            dtTmp = d.get_data("select * from d_nhomin where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nhomin"))
            {
                _ten = r["nhomin"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {

                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        try
                        {
                            _id = long.Parse(d.get_data("select max(id) from " + user + ".d_nhomin").Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        catch { _id = 1; }
                        d.upd_danhmuc("d_nhomin", _id, _ten, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                        d.execute_data("update d_nhomin set khongdau='" + kdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_nhomin where nhom=" + i_nhom).Tables[0];
                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void butPath_Click(object sender, EventArgs e)
        {
            string sDir = System.Environment.CurrentDirectory.ToString();
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "*.XLSX|*.*";
            of.ShowDialog();
            path.Text = of.FileName.ToString();
            System.Environment.CurrentDirectory = sDir;
            if (path.Text != "") sheet.DataSource = LoadSchemaFromFile(path.Text);
        }
        private DataSet get_excel(string fileName)
        {
            try
            {
                OleDbConnection con = this.ReturnConnection_2003(fileName);
                con.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet.Text + "]", con);
                OleDbDataAdapter dest = new OleDbDataAdapter();
                dest.SelectCommand = cmd;
                DataSet ds = new DataSet();
                dest.Fill(ds);
                cmd.Dispose();
                con.Close();
                return ds;
            }
            catch { return null; }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            sql = "select maatc,tenhc from d_dmbd where maatc is not null and tenhc is not null and hide=0";
            DataSet dsdm = new DataSet();
            dsdm = d.get_data(sql);
            foreach (DataRow r in dsdm.Tables[0].Rows)
            {
                r["tenhc"] = LibUtility.Utility.Hoten_khongdau(r["tenhc"].ToString()).Trim();

            }
            sql = "select * from d_tuongtacdm ";
            string _tenkdau = "";
            DataRow dr;
            foreach (DataRow r in d.get_data_text(sql).Tables[0].Rows)
            {
                _tenkdau = r["tenkdau"].ToString().Trim();
                dr = d.getrowbyid(dsdm.Tables[0], "tenhc='" + _tenkdau + "'");
                if (dr != null)
                {
                    d.upd_tuongtacdmatc(long.Parse(r["id"].ToString()), decimal.Parse(r["stt"].ToString()), dr["maatc"].ToString(), r["ghichu"].ToString(), 0);
                }
            }
            MessageBox.Show("OK");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            sql = "select * from d_tuongtackhac where id in(select id from d_tuongtacdmatc ) and makhac in(select id from d_tuongtacdmatc )";
            DataSet dsdm = d.get_data_text(sql);
            foreach (DataRow r in dsdm.Tables[0].Rows)
            {
                d.upd_tuongtackhacatc(long.Parse(r["id"].ToString()), decimal.Parse(r["stt"].ToString()), long.Parse(r["makhac"].ToString()), int.Parse(r["mucdo"].ToString()), r["ghichu"].ToString(), 0, int.Parse(r["cam"].ToString()), "");

            }
            MessageBox.Show("OK");
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            sql = "select manv from hum_llnv";
            DataSet dsdm = d.get_data_text(sql);
            string manv = "", SOCHUNGCHI = "";
            foreach (DataRow r in dsdm.Tables[0].Rows)
            {
                SOCHUNGCHI = "";
                manv = r["manv"].ToString();
                sql = "select SOCHUNGCHI from dmbs where manhanvien='" + manv + "' and SOCHUNGCHI is not null ";
                foreach (DataRow row in d.get_data(sql).Tables[0].Rows)
                {
                    SOCHUNGCHI = row["SOCHUNGCHI"].ToString();
                    break;
                }
                if (SOCHUNGCHI != "")
                {

                    sql = "update hum_llnv set SOCHUNGCHI='" + SOCHUNGCHI + "' where manv='" + manv + "'";
                    d.execute_data(sql);
                }

            }
            MessageBox.Show("OK");
        }

        private void button12_Click(object sender, EventArgs e)
        {



            int i_userid = 1;
            int makho = 1;
            string soluutru = "", mabn = "", sobenhan = "";
            foreach (DataRow r in dt.Rows)
            {
                soluutru = r["soluutru"].ToString();
                mabn = r["mabn"].ToString();
                if (soluutru != "" && mabn != "")
                {
                    int _vitri = 0;

                    mabn = mabn.PadLeft(8, '0');
                    sobenhan = r["sobenhan"].ToString();
                    DataSet dsVitri = new DataSet();
                    soluutru = soluutru.ToUpper();
                    soluutru = soluutru.Replace("NT", "").Trim();
                    soluutru = soluutru.Replace("/", "").Trim();


                    dsVitri = getVitri(long.Parse(soluutru), 1);
                    if (dsVitri == null || dsVitri.Tables.Count == 0 || dsVitri.Tables[0].Rows.Count != 1)
                    {
                        StreamWriter writer = new StreamWriter("ngoaitru.txt", true, Encoding.UTF8);
                        writer.Write(soluutru);
                        writer.Write("\r\n");
                        writer.Close();
                        writer.Dispose();

                    }
                    else
                    {
                        _vitri = int.Parse(dsVitri.Tables[0].Rows[0][0].ToString());
                        if (_vitri > 0)
                        {
                            if (!d.upd_ba_luutruhoso_ngoai(mabn, d.ngayhienhanh_server, i_userid, makho, _vitri, sobenhan))
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("error");
                        }
                    }
                }
            }
            MessageBox.Show("OK");
        }
        private DataSet getVitri(long soluutru, int loaihs)
        {
            string sql = " select a.id";
            sql += " from ba_dmcot a inner  join ba_dmhang b on a.idhang = b.id";
            sql += " inner join ba_dmke c on b.idke = c.id ";
            sql += " where c.idkho = 1 and c.ngoaitru =  " + loaihs;
            sql += " and c.tu <= " + soluutru + " and c.den >= " + soluutru;
            sql += " and b.tu <= " + soluutru + " and b.den >=" + soluutru;
            sql += " and a.tu <= " + soluutru + " and a.den >=" + soluutru;
            return d.get_data(sql);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string idnhom = "", nhom = "";
            foreach (DataRow r in dt.Rows)
            {
                idnhom = r["idnhom"].ToString();
                nhom = r["nhom"].ToString();
                if (idnhom != "")
                {
                    d.upd_danhmuc("dmnhomphatdo", int.Parse(idnhom), nhom);
                }
            }
            MessageBox.Show("OK");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string idnhom = "", nhom = "", id, ten;
            foreach (DataRow r in dt.Rows)
            {
                id = r["id"].ToString();
                ten = r["ten"].ToString();
                idnhom = r["idnhom"].ToString();
                nhom = r["nhom"].ToString();
                if (idnhom != "")
                {
                    d.upd_dmphatdo(long.Parse(id), int.Parse(idnhom), int.Parse(id), ten);
                }
            }
            MessageBox.Show("OK");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int i_userid = 1;
            int makho = 1;

            string soluutru = "", mabn = "", sobenhan = "";
            foreach (DataRow r in dt.Rows)
            {
                mabn = r["mabn"].ToString();

                soluutru = r["soluutru"].ToString();
                if (soluutru != "" && mabn != "")
                {
                    mabn = mabn.PadLeft(8, '0');
                    int _vitri = 0;

                    sobenhan = r["sobenhan"].ToString();
                    DataSet dsVitri = new DataSet();
                    if (soluutru.IndexOf('/') > -1)
                    {
                        soluutru = soluutru.Split('/')[0].Trim();
                    }


                    dsVitri = getVitri(long.Parse(soluutru), 0);
                    if (dsVitri == null || dsVitri.Tables.Count == 0 || dsVitri.Tables[0].Rows.Count != 1)
                    {
                        StreamWriter writer = new StreamWriter("noitru.txt", true, Encoding.UTF8);
                        writer.Write(soluutru);
                        writer.Write("\r\n");
                        writer.Close();
                        writer.Dispose();

                    }
                    else
                    {
                        _vitri = int.Parse(dsVitri.Tables[0].Rows[0][0].ToString());
                        if (_vitri > 0)
                        {
                            if (!d.upd_ba_luutruhoso_ngoai(mabn, d.ngayhienhanh_server, i_userid, makho, _vitri, sobenhan))
                            {

                            }
                        }

                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string soluutru = "", mabn = "", Val = "";
            foreach (DataRow r in dt.Rows)
            {
                soluutru = r["soluutru"].ToString();
                if (soluutru != "")
                {

                    mabn = r["mabn"].ToString();
                    mabn = mabn.PadLeft(8, '0');

                    DataSet ds = d.f_Get_Hanhchanh(mabn);
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        StreamWriter writer = new StreamWriter("ngoaitru_mabn.txt", true, Encoding.UTF8);
                        Val = mabn + "\t\t" + soluutru + "\t\t" + r["hoten"].ToString();
                        writer.Write(Val);
                        writer.Write("\r\n");
                        writer.Close();
                        writer.Dispose();

                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string soluutru = "", mabn = "", Val = "";
            foreach (DataRow r in dt.Rows)
            {
                soluutru = r["soluutru"].ToString();
                if (soluutru != "")
                {

                    mabn = r["mabn"].ToString();
                    mabn = mabn.PadLeft(8, '0');

                    DataSet ds = d.f_Get_Hanhchanh(mabn);
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        StreamWriter writer = new StreamWriter("noi_mabn.txt", true, Encoding.UTF8);
                        Val = mabn + "\t\t" + soluutru + "\t\t" + r["hoten"].ToString();
                        writer.Write(Val);
                        writer.Write("\r\n");
                        writer.Close();
                        writer.Dispose();

                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void butGiatt39_Click(object sender, EventArgs e)
        {
            decimal dongia = 0;
            decimal id = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = decimal.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                dongia = decimal.Parse(row["gia_bh"].ToString());
                            }
                            catch { dongia = 0; }
                            if (dongia > 0)
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".v_giavp where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_v_giavp_luu(decimal.Parse(r["id"].ToString()), decimal.Parse(r["id_loai"].ToString()), decimal.Parse(r["stt"].ToString()),
                        r["ma"].ToString(), r["ten"].ToString(), r["dvt"].ToString(), decimal.Parse(r["gia_th"].ToString()),
                        decimal.Parse(r["gia_bh"].ToString()), decimal.Parse(r["gia_dv"].ToString()), decimal.Parse(r["gia_nn"].ToString()),
                        decimal.Parse(r["gia_cs"].ToString()), decimal.Parse(r["vattu_th"].ToString()), decimal.Parse(r["vattu_bh"].ToString()),
                        decimal.Parse(r["vattu_dv"].ToString()), decimal.Parse(r["vattu_nn"].ToString()), decimal.Parse(r["vattu_cs"].ToString()),
                        decimal.Parse(r["bhyt"].ToString()), decimal.Parse(r["loaibn"].ToString()), decimal.Parse(r["theobs"].ToString()),
                        decimal.Parse(r["thuong"].ToString()), decimal.Parse(r["trongoi"].ToString()), decimal.Parse(r["loaitrongoi"].ToString()),
                        decimal.Parse(r["chenhlech"].ToString()), decimal.Parse(r["ndm"].ToString()), r["locthe"].ToString(),
                        decimal.Parse(r["readonly"].ToString()), decimal.Parse(r["userid"].ToString()), decimal.Parse(r["tylekhuyenmai"].ToString()),
                        decimal.Parse(r["gia_ksk"].ToString()), decimal.Parse(r["vattu_ksk"].ToString()), decimal.Parse(r["hide"].ToString()),
                        decimal.Parse(r["kythuat"].ToString()), 0,0))
                                    {
                                        MessageBox.Show("error");
                                    }
                                    d.execute_data("update " + user + ".v_giavp_luu set gia_bht=" + r["gia_bh"].ToString() + " where id=" + r["id"].ToString());
                                }
                                sql = "update v_giavp set gia_bht=gia_bh where id=" + id;
                                d.execute_data(sql);
                                sql = "update v_giavp set gia_bh=" + dongia + ",ngayud=sysdate where id=" + id;
                                d.execute_data(sql);
                            }
                        }
                    }
                    catch { id = 0; }

                }
            }
            MessageBox.Show("OK");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            decimal dongia = 0;
            decimal id = 0;
            string cotgia = filedgia.Text; //combogia
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = decimal.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                dongia = decimal.Parse(row[cotgia].ToString());
                            }
                            catch { dongia = 0; }
                            if (dongia > 0)
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".v_giavp where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_v_giavp_luu(decimal.Parse(r["id"].ToString()), decimal.Parse(r["id_loai"].ToString()), decimal.Parse(r["stt"].ToString()),
                        r["ma"].ToString(), r["ten"].ToString(), r["dvt"].ToString(), decimal.Parse(r["gia_th"].ToString()),
                        decimal.Parse(r["gia_bh"].ToString()), decimal.Parse(r["gia_dv"].ToString()), decimal.Parse(r["gia_nn"].ToString()),
                        decimal.Parse(r["gia_cs"].ToString()), decimal.Parse(r["vattu_th"].ToString()), decimal.Parse(r["vattu_bh"].ToString()),
                        decimal.Parse(r["vattu_dv"].ToString()), decimal.Parse(r["vattu_nn"].ToString()), decimal.Parse(r["vattu_cs"].ToString()),
                        decimal.Parse(r["bhyt"].ToString()), decimal.Parse(r["loaibn"].ToString()), decimal.Parse(r["theobs"].ToString()),
                        decimal.Parse(r["thuong"].ToString()), decimal.Parse(r["trongoi"].ToString()), decimal.Parse(r["loaitrongoi"].ToString()),
                        decimal.Parse(r["chenhlech"].ToString()), decimal.Parse(r["ndm"].ToString()), r["locthe"].ToString(),
                        decimal.Parse(r["readonly"].ToString()), decimal.Parse(r["userid"].ToString()), decimal.Parse(r["tylekhuyenmai"].ToString()),
                        decimal.Parse(r["gia_ksk"].ToString()), decimal.Parse(r["vattu_ksk"].ToString()), decimal.Parse(r["hide"].ToString()),
                        decimal.Parse(r["kythuat"].ToString()), 0,0))
                                    {
                                        MessageBox.Show("error");
                                    }
                                    d.execute_data("update " + user + ".v_giavp_luu set gia_bht=" + r["gia_bh"].ToString() + " where id=" + r["id"].ToString());
                                    d.execute_data("update " + user + ".v_giavp set gia_bht=" + r["gia_bh"].ToString() + " where id=" + r["id"].ToString());
                                }

                                sql = "update v_giavp set " + cotgia + "=" + dongia + ",ngayud=sysdate where id=" + id;
                                d.execute_data(sql);
                            }
                        }
                    }
                    catch { id = 0; }

                }
            }
            MessageBox.Show("OK");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            dtTmp = d.get_data("select * from d_dmnhomdt where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nhomdieutri"))
            {
                _ten = r["nhomdieutri"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {

                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        try
                        {
                            _id = long.Parse(d.get_data("select max(id) from " + user + ".d_dmnhomdt").Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        catch { _id = 1; }
                        d.upd_danhmuc("d_dmnhomdt", _id, _ten, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                        d.execute_data("update d_dmnhomdt set khongdau='" + kdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_dmnhomdt where nhom=" + i_nhom).Tables[0];
                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("tinh.xml.xml");
            foreach (DataRow r1 in ds.Tables[0].Rows)
            {
                d.upd_btdtt("btdtt", r1["mavung"].ToString(), r1["matt"].ToString(), r1["tentt"].ToString());
                d.execute_data("update btdtt set ma='" + r1["mattbh"].ToString() + "' where matt='" + r1["matt"].ToString() + "'");
            }
            MessageBox.Show("OK");
        }

        private void button21_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("quan.xml.xml");
            foreach (DataRow r2 in ds.Tables[0].Rows)
            {
                d.upd_btdquan("btdquan", r2["matt"].ToString(), r2["maqu"].ToString(), r2["tenquan"].ToString());
            }
            MessageBox.Show("OK");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("xa.xml.xml");
            foreach (DataRow r3 in ds.Tables[0].Rows)
            {
                d.upd_btdpxa("btdpxa", r3["maqu"].ToString(), r3["maphuongxa"].ToString(), r3["tenpxa"].ToString(), r3["viettat"].ToString());
            }
            MessageBox.Show("OK");
        }

        private void button23_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("btdnn_bv.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.upd_btdnn_bv("btdnn_bv", r["mann"].ToString(), r["tennn"].ToString(), r["mannbo"].ToString());
            }
            MessageBox.Show("OK");
        }

        private void button28_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("btddt.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.upd_btddt(r["MADANTOC"].ToString(), r["DANTOC"].ToString());
            }
            MessageBox.Show("OK");
        }

        private void button25_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("btdkp_bv.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.upd_btdkp_bv("btdkp_bv", r["makp"].ToString() != "99" ? r["makp"].ToString().PadLeft(3, '0') : "999", r["tenkp"].ToString(), int.Parse(r["kehoach"].ToString()), int.Parse(r["thucke"].ToString()), r["makpbo"].ToString(), r["maba"].ToString(), int.Parse(r["loai"].ToString()), int.Parse(r["mavp"].ToString()), r["loaivp"].ToString(), r["mucvp"].ToString(), r["viettat"].ToString(), r["loaicd"].ToString(), r["muccd"].ToString(), 0);

            }
            MessageBox.Show("OK");
        }

        private void button27_Click(object sender, EventArgs e)
        {

            string ma = "";
            foreach (DataRow r in dt.Rows)
            {
                ma = r["ma"].ToString().PadLeft(4, '0');
                d.upd_dmbs(ma, r["hoten"].ToString(), r["makp"].ToString() != "99" ? r["makp"].ToString().PadLeft(3, '0') : "999", r["mapk"].ToString() != "99" ? r["mapk"].ToString().PadLeft(3, '0') : "999",
                        int.Parse(r["nhom"].ToString()), r["viettat"].ToString(), 0, 0, 0, 0, r["bangcap"].ToString(), "", "", "", "");
                sql = "update dmbs set SOCHUNGCHI'" + r["SOCHUNGCHI"].ToString() + "' where ma='" + ma + "'";
                d.execute_data(sql);
            }
            MessageBox.Show("OK");
        }

        private void button29_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("nhomnhanvien.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.upd_danhmuc("nhomnhanvien", int.Parse(r["id"].ToString()), r["ten"].ToString());
            }
            MessageBox.Show("OK");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("d_dmkho.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.upd_dmkho(long.Parse(r["id"].ToString()), r["ten"].ToString(), int.Parse(r["nhom"].ToString()), int.Parse(r["loai"].ToString()),
                    long.Parse(r["stt"].ToString()), int.Parse(r["khott"].ToString()), r["computer"].ToString(), r["matat"].ToString(), int.Parse(r["ketoan"].ToString()),
                    int.Parse(r["thua"].ToString()));

            }
            MessageBox.Show("OK");
        }

        private void button26_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("d_duockp.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.upd_duockp(int.Parse(r["id"].ToString()), r["ma"].ToString(), r["ten"].ToString(), r["nhom"].ToString(), long.Parse(r["stt"].ToString()), r["makp"].ToString().PadLeft(3, '0'),
                       r["makho"].ToString(), int.Parse(r["matutruc"].ToString()), "", r["computer"].ToString(), int.Parse(r["somay"].ToString()));
                d.execute_data("update d_duockp set phieu='" + r["phieu"].ToString() + "',loaiphieu='" + r["loaiphieu"].ToString() + "',tutruc='" + r["tutruc"].ToString() + "' where id=" + long.Parse(r["id"].ToString()));

            }
            MessageBox.Show("OK");
        }

        private void button30_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("d_dmphieu.xml.xml");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                d.execute_data("update d_dmphieu set kho='" + r["KHO"].ToString() + "' where id=" + r["id"].ToString());
            }
            MessageBox.Show("OK");
        }

        private void button31_Click(object sender, EventArgs e)
        {
            //chiphikhambenh _chiphikhambenh = new chiphikhambenh();
            //string ngayvao = "06/09/2019";
            //string ngayra = "06/09/2019";
            //string mabn = "17000002";
            //long mavaovien = 190906170957896987;
            //long maql = 190906170957896987;
            //int madoituong = -1;
            //string makp = "040";
            ////d.VcbGenQrCode(0, mabn, ngayvao, ngayra, mavaovien, maql, madoituong, makp);
            //PAYResponse _PAYResponse = new PAYResponse();
            //_PAYResponse.UserID = "190911000000040";
            //_PAYResponse.Amount = 104000;
            //d.VCBPAY(_PAYResponse, LibHIS.AccessData.AppID_VCB_BILLING);
        }

        private void btnnoicapbhyt_Click(object sender, EventArgs e)
        {
            int stt = 1;
            string mabv = "", tenbv = "", diachi = "";
            oracle.TransactionBegin();
            foreach (DataRow r in dt.Rows)
            {
                tenbv = r["tenbv"].ToString().Trim();
                mabv = r["mabv"].ToString().Trim();
                diachi = r["diachi"].ToString().Trim();
                if (mabv != "" && tenbv != "")
                {
                    if (tenbv.Length > 254)
                    {
                        return;
                        MessageBox.Show(">254");
                        oracle.TransactionRollback();
                    }
                    tenbv = tenbv.Replace("'", "");
                    try
                    {
                        if (diachi != "")
                        {
                            d.execute_data("update " + user + ".dmnoicapbhyt set tenbv='" + tenbv + "',diachi='" + diachi + "' where mabv='" + mabv + "'");
                        }
                        else
                        {
                            if (d.execute_data("update " + user + ".dmnoicapbhyt set tenbv='" + tenbv + "' where mabv='" + mabv + "'"))
                            {

                            }
                            else
                            {
                                oracle.TransactionRollback();
                                MessageBox.Show(tenbv);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("" + ex.ToString());
                        oracle.TransactionRollback();
                    }

                }





            }
            oracle.TransactionCommit();
            MessageBox.Show("OK");
        }

        private void btnupd_filed_Click(object sender, EventArgs e)
        {
            oracle.TransactionBegin();
            string s_tencot = "";
            decimal id = 0;
            string cotgia = cbtenfileld.Text;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = decimal.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                s_tencot = row[cotgia].ToString();
                            }
                            catch (Exception ex)
                            {
                                oracle.TransactionRollback();
                                MessageBox.Show(ex.ToString());
                                
                            }
                            if (s_tencot != "")
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".v_giavp where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_v_giavp_luu(decimal.Parse(r["id"].ToString()), decimal.Parse(r["id_loai"].ToString()), decimal.Parse(r["stt"].ToString()),
                        r["ma"].ToString(), r["ten"].ToString(), r["dvt"].ToString(), decimal.Parse(r["gia_th"].ToString()),
                        decimal.Parse(r["gia_bh"].ToString()), decimal.Parse(r["gia_dv"].ToString()), decimal.Parse(r["gia_nn"].ToString()),
                        decimal.Parse(r["gia_cs"].ToString()), decimal.Parse(r["vattu_th"].ToString()), decimal.Parse(r["vattu_bh"].ToString()),
                        decimal.Parse(r["vattu_dv"].ToString()), decimal.Parse(r["vattu_nn"].ToString()), decimal.Parse(r["vattu_cs"].ToString()),
                        decimal.Parse(r["bhyt"].ToString()), decimal.Parse(r["loaibn"].ToString()), decimal.Parse(r["theobs"].ToString()),
                        decimal.Parse(r["thuong"].ToString()), decimal.Parse(r["trongoi"].ToString()), decimal.Parse(r["loaitrongoi"].ToString()),
                        decimal.Parse(r["chenhlech"].ToString()), decimal.Parse(r["ndm"].ToString()), r["locthe"].ToString(),
                        decimal.Parse(r["readonly"].ToString()), decimal.Parse(r["userid"].ToString()), decimal.Parse(r["tylekhuyenmai"].ToString()),
                        decimal.Parse(r["gia_ksk"].ToString()), decimal.Parse(r["vattu_ksk"].ToString()), decimal.Parse(r["hide"].ToString()),
                        decimal.Parse(r["kythuat"].ToString()), 0,0))
                                    {
                                        oracle.TransactionRollback();
                                        MessageBox.Show("error");
                                        
                                    }

                                }
                                d.upd_v_giavp(id, s_tencot, cotgia);
                              //  sql = "update v_giavp set " + cotgia + "='" + s_tencot + "',ngayud=sysdate where id=" + id;
                                //d.execute_data(sql);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oracle.TransactionRollback();
                        MessageBox.Show(ex.ToString());
                        id = 0;
                        
                    }

                }
            }
            oracle.TransactionCommit();
            MessageBox.Show("OK");
            
        }

        private OleDbConnection ReturnConnection_2003(string fileName)
        {
            try
            {
                return new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;" +
               "Data Source=" + fileName + ";" +
               " Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;\"");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
           
        }

        private OleDbConnection ReturnConnection_2007(string fileName)
        {
            try
            {
                return new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" +
                   "Data Source=" + fileName + ";" +
                   " Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 12.0;\"");

            }           
             catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            d.execute_data("alter table d_dmnx add khongdau varchar2(254) ");
            d.execute_data("alter table d_dmhang add khongdau varchar2(254) ");
            d.execute_data("alter table d_dmnuoc add khongdau varchar2(254) ");
            d.execute_data("alter table d_dmnhomdt add khongdau varchar2(254) ");
            d.execute_data("alter table d_dmnhomkt add khongdau varchar2(254) ");
            d.execute_data("alter table d_nhombo add khongdau varchar2(254) ");
            d.execute_data("alter table d_nhomin add khongdau varchar2(254) ");
            d.execute_data("alter table d_dmnhom add khongdau varchar2(254) ");
            d.execute_data("alter table d_dmloai add khongdau varchar2(254) ");
            d.execute_data("alter table xn_donvi add khongdau varchar2(254) ");
            d.execute_data("alter table v_nhombaocao add khongdau varchar2(254) ");

            string table_name = "d_dmnx";
            string[] arr = new string[] { "d_dmnx", "d_dmhang", "d_dmnuoc", "d_dmnhomdt", "d_dmnhomkt", "d_nhombo", "d_nhomin", "d_dmloai", "v_nhombaocao", "d_dmnhom" };

            string khongdau = "";

            foreach (string s in arr)
            {
                table_name = s;
                sql = "select * from " + table_name + "";
               
                string asql = "";
                foreach (DataRow r in d.get_data(sql).Tables[0].Rows)
                {
                    khongdau = r["ten"].ToString().Trim();
                    khongdau = LibUtility.Utility.Hoten_khongdau(khongdau);
                    khongdau = khongdau.Replace("'", "");
                    asql = " update " + table_name + " set khongdau='" + khongdau + "' where id=" + r["id"].ToString();
                    if (!d.execute_data(asql))
                    {
                        LibUtility.Utility.MsgBox(table_name);
                        return;
                    }
                }

            }



            MessageBox.Show("OK");
        }

        private void button33_Click(object sender, EventArgs e)
        {
            byte[] s = LibUtility.Utility.getBytePDF("C:\\his\\19912782_rPage59.rpt.pdf");
        }

        private void nhomkhoimp_SelectedIndexChanged(object sender, EventArgs e)
        {
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            load();
        }
        private void load()
        {
            makpimp.ComboBox.DisplayMember = "TEN";
            makpimp.ComboBox.ValueMember = "ID";
            makpimp.ComboBox.DataSource = d.get_data("select id,ten||'  -  '||id as ten from d_duockp where nhom  like '%" + i_nhomkho + "%'").Tables[0];

            makhoimp.ComboBox.DataSource = d.get_data("select id,ten||'  -  '||id as ten from d_dmkho where nhom=" + i_nhomkho).Tables[0];

            manguonimp.ComboBox.DisplayMember = "TEN";
            manguonimp.ComboBox.ValueMember = "ID";
            manguonimp.ComboBox.DataSource = d.get_data("select id,ten||'  -  '||id as ten from d_dmnguon where nhom  =" + i_nhomkho + " or nhom=0").Tables[0];

            

        }
        private bool checkdm()
        {
            int _mabd = 0;
            DataRow dr;
            DataTable dtbd = d.get_data("select id,madv from d_dmbd where nhom=" + i_nhomkho).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                if (r["mabd"].ToString().Trim() != "")
                {
                    try
                    {
                        _mabd = int.Parse(r["MABD"].ToString());
                    }
                    catch { _mabd = 0; }
                    if (_mabd > 0)
                    {
                        dr = d.getrowbyid(dtbd, "id=" + _mabd);
                        if (dr == null)
                        {
                            LibUtility.Utility.MsgBox(_mabd.ToString());
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void nhomkhoimp_Click(object sender, EventArgs e)
        {

        }

        private void button35_Click(object sender, EventArgs e)
        {
            int i_sole_giaban = 0;
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            string nhom_tyle2 = get_nhom_tyle2(i_nhomkho);
            int _makho = 0, _makp = 0;
            try
            {
                _makho = int.Parse(makhoimp.ComboBox.SelectedValue.ToString());
            }
            catch { _makho = 0; }
            if (_makho == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kho");
                return;
            }

            try
            {
                _makp = int.Parse(makpimp.ComboBox.SelectedValue.ToString());
            }
            catch { _makp = 0; }
            if (_makp == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kp");
                return;
            }


            if (!checkdm()) return;

            long id = 0;
            if (txtthangton.Text.Trim() == "" || txtthangton.Text.Trim().Length != 4)
            {
                LibUtility.Utility.MsgBox("Tháng tồn không hợp lệ!");
                return;
            }
            mmyy = txtthangton.Text.Trim();
            int _manguon = 0, _mabd = 0, _madv = 0;
            decimal dongia = 0, giamua = 0, giaban = 0, ton = 0;
            string _handung = "";
            DataRow dr;
            DataTable dtbd = d.get_data("select id,madv from d_dmbd where nhom=" + i_nhomkho).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                if (r["mabd"].ToString().Trim() != "")
                {
                    try
                    {
                        _mabd = int.Parse(r["MABD"].ToString());
                    }
                    catch { _mabd = 0; }
                    if (_mabd > 0)
                    {
                        try
                        {
                            ton = decimal.Parse(r["TON"].ToString());
                        }
                        catch { ton = 0; }
                        if (ton > 0)
                        {
                            try
                            {
                                dongia = decimal.Parse(r["dongia"].ToString());
                            }
                            catch { dongia = 0; }
                            try
                            {
                                giaban = decimal.Parse(r["giaban"].ToString());
                            }
                            catch { giaban = 0; }
                            if (chkGiabanTyle.Checked)
                            {
                                decimal tl = d.get_tyleban(_mabd, nhom_tyle2, i_nhomkho, dongia);

                                giaban = LibUtility.Utility.Round(dongia + dongia * tl / 100, i_sole_giaban);

                            }
                            if (giaban > 0)
                            {
                                sql = "update d_dmbd set giaban=" + giaban + " where id=" + _mabd + " and giaban<" + giaban;
                                d.execute_data(sql);
                            }
                            _handung = r["handung"].ToString().Trim();
                            if (_handung.Length >= 10)
                            {
                                _handung = _handung.Split('/')[0] + _handung.Split('/')[1] + _handung.Split('/')[2].Substring(2,2);
                            }
                            else if (_handung.Length == 6)
                            {
                                _handung = _handung.Trim();
                            }
                            else if (_handung.Length == 7)
                            {
                                _handung = _handung.Split('/')[0] + _handung.Split('/')[1].Substring(2, 2);
                            }
                            else   _handung = "";

                            

                                giamua = dongia;

                            //decimal tl = d.get_tyleban(i_nhomkho, giamua);

                            //giaban = LibUtility.Utility.Round(giamua + giamua * tl / 100, i_sole_giaban);

                            _madv = 0;
                            dr = d.getrowbyid(dtbd, "id=" + _mabd);
                            if (dr != null)
                            {
                                _madv = int.Parse(dr["madv"].ToString());
                            }

                             id = d.get_id_tonkho;
                            if (!d.upd_theodoi(mmyy, id, _mabd, _manguon, _madv, _handung, r["losx"].ToString().ToString(), "", "", "", 0, 0, 0, giamua, giaban, 0, 0, dongia, 0, 0))
                            {
                                MessageBox.Show("Không cập nhật được thông tin tồn kho");
                                return;
                            }


                            if (!d.upd_tutrucct(mmyy, _makp, _makho, id, _mabd, ton, 0))
                            {
                                MessageBox.Show("Không cập nhật được thông tin tồn kho");
                                return;
                            }
                        }
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button36_Click(object sender, EventArgs e)
        {
            decimal dongia = 0;
            string exp = "";
            DataRow drow;
            sql = "select a.*,b.tondau ";
            sql += " from mqsoftquan70819.d_theodoi a  inner join mqsoftquan70819.d_tonkhoct b on a.id = b.stt ";
            sql += " where b.makho = 2 and a.giamua = 0";
            DataTable dtTmp = d.get_data(sql).Tables[0];
            foreach (DataRow row in dtTmp.Rows)
            {
                exp = "mabd=" + row["mabd"].ToString() + " and  ton=" + row["tondau"].ToString();
                if (row["handung"].ToString().Trim() != "")
                    exp += " and handung='" + row["handung"].ToString() + "'";
                if (row["losx"].ToString().Trim() != "")
                    exp += " and losx='" + row["losx"].ToString() + "'";
                drow = d.getrowbyid(dt, exp);
                if (drow != null)
                {
                    try
                    {
                        dongia = decimal.Parse(drow["dongia"].ToString());
                    }
                    catch { dongia = 0; }
                    if (dongia > 0)
                    {
                        sql = "update mqsoftquan70819.d_theodoi set giamua=" + dongia + ",  giaban=" + dongia + " where id= " + row["id"].ToString();
                        d.execute_data(sql);
                    }
                }

            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            int stt = 1;
            string Donvi = "", Khongdau = "";
            DataTable dtTmp = d.get_data("select * from  xn_donvi").Tables[0];
            DataRow dr;
            foreach (DataRow r in dt.Rows)
            {

                Donvi = r["Donvi"].ToString().Trim();
                if (Donvi == "") Donvi = ".";
                Khongdau = LibUtility.Utility.Hoten_khongdau(Donvi);
                dr = d.getrowbyid(dtTmp, "khongdau='" + Khongdau + "'");
                if (dr == null)
                {
                    string m_id = d.get_id_xn_donvi.ToString();
                    if (!d.upd_xn_donvi(decimal.Parse(m_id), m_id, Donvi, Donvi, 0, 0))
                    {
                        LibUtility.Utility.MsgBox("");
                        return;
                    }
                    d.execute_data("update xn_donvi set khongdau='" + Khongdau + "' where id=" + m_id);
                    dtTmp = d.get_data("select * from  xn_donvi").Tables[0];
                }

                stt++;
            }
            MessageBox.Show("OK");
        }

        private void button38_Click(object sender, EventArgs e)
        {
            int stt = 1;
            string ten = "", Nhom = "", CSBT_NAM = "", CSBT_NU = "", MaDV = "";
            string Donvi = "", Khongdau = "", CanDuoi = "", CanTren = "", CanDuoiF = "", CanTrenF = "";

            DataTable dtTmpDonvi = d.get_data("select * from  xn_donvi").Tables[0];
            DataTable dtTmpNhom = d.get_data("select * from  xn_nhom").Tables[0];
            DataTable dtTmpLoai = d.get_data("select * from  xn_loai").Tables[0];
            DataRow dr;
            decimal _donvi = 0, _nhom = 0;
            foreach (DataRow r in dt.Rows)
            {
                ten = r["TenDV"].ToString().Trim();
                Nhom = r["Nhom"].ToString().Trim();

                CSBT_NAM = r["CSBT"].ToString().Trim();
                CSBT_NU = r["CSBTF"].ToString().Trim();

                CanDuoi = r["CanDuoi"].ToString().Trim();
                if (CanDuoi == "NULL") CanDuoi = "0";

                CanTren = r["CanTren"].ToString().Trim();
                if (CanTren == "NULL") CanTren = "0";


                CanDuoiF = r["CanDuoiF"].ToString().Trim();
                if (CanDuoiF == "NULL") CanDuoiF = "0";

                CanTrenF = r["CanTrenF"].ToString().Trim();
                if (CanTrenF == "NULL") CanTrenF = "0";


                try { CanDuoi = decimal.Parse(CanDuoi).ToString(); } catch { CanDuoi = "0"; }
                try { CanTren = decimal.Parse(CanTren).ToString(); } catch { CanTren = "0"; }
                try { CanDuoiF = decimal.Parse(CanDuoiF).ToString(); } catch { CanDuoiF = "0"; }
                try { CanTrenF = decimal.Parse(CanTrenF).ToString(); } catch { CanTrenF = "0"; }

                if (CanDuoi != "0" && CanDuoiF == "") CanDuoiF = CanDuoi;
                if (CanTren != "0" && CanTrenF == "") CanTrenF = CanTren;

                MaDV = r["MaDV"].ToString().Trim();

                Donvi = r["Donvi"].ToString().Trim();
                if (Donvi == "NULL") Donvi = ".";
                if (Donvi == "") Donvi = ".";
                Khongdau = LibUtility.Utility.Hoten_khongdau(Donvi);
                dr = d.getrowbyid(dtTmpDonvi, "khongdau='" + Khongdau + "'");
                if (dr == null)
                {
                    _donvi = 0;
                }
                else { _donvi = decimal.Parse(dr["id"].ToString()); }

                dr = d.getrowbyid(dtTmpNhom, "khongdau='" + Nhom + "'");
                if (dr == null)
                {
                    _nhom = 0;
                }
                else { _nhom = decimal.Parse(dr["id"].ToString()); }


                string m_id = d.get_id_xn_ten.ToString();
                if (!d.upd_xn_ten(decimal.Parse(m_id), _nhom, _nhom, decimal.Parse(m_id), MaDV, ten, MaDV, _donvi, CSBT_NU, CSBT_NAM, decimal.Parse(CanDuoi), decimal.Parse(CanTren), decimal.Parse(CanDuoiF), decimal.Parse(CanTrenF), 0, "", 0, "Serum", 0, 0, 0, MaDV, _ten))
                {
                    LibUtility.Utility.MsgBox("");
                    return;
                }


                stt++;
            }
            MessageBox.Show("OK");
        }

        private void Button39_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 1;
            int _stt = 1;
            long _id = 1000000;
            CatShareDetails _Cat;
            foreach (DataRow r in dt.Rows)
            {
                _id++;
                _Cat = new CatShareDetails();
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;
                _Cat.String1 = "";
                _Cat.String2 = "";
                _Cat.String3 = "";
                _Cat.String4 = "";
                _Cat.String5 = "";
                _stt++;
                list.Add(_Cat);
            }
            string json = JsonConvert.SerializeObject(list);
            Insertjson("CatNationals", json);
        }
        private void Insertjson(string name,string json)
        {
            try
            {
                File.Delete(name + ".json");
                StreamWriter writernew = new StreamWriter(name + ".json", true, Encoding.UTF8);
                writernew.Write(json);
                writernew.Close();
                writernew.Dispose();
            }
            catch { }
        }
        private void Button40_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 2;
            int _stt = 1;
            long _id = 2000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from dmvungbv").Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                
                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MA"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatGeographicRegions", json);
        }

        private void Button41_Click(object sender, EventArgs e)
        {
            //string jsonvung = File.ReadAllText("CatGeographicRegions.json");
            //var listVung = JsonConvert.DeserializeObject<List<CatShareDetails>>(jsonvung);

            



            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 3;
            int _stt = 1;
            long _id = 3000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select a.*,2000000+mavung as idvung from btdtt a ").Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                
                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MATT"].ToString());
                _Cat.ID = _id;

                _Cat.ID = _id;
                _Cat.Code = r["MATT"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TENtt"].ToString();
                _Cat.MasterId = _MasterId;

                _Cat.ParentId = long.Parse(r["idvung"].ToString());
                
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;
                _Cat.String1 = r["MA"].ToString().PadLeft(2, '0');
                _Cat.String2= r["MATT"].ToString();

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatCitys", json);
        }

        private void button55_Click(object sender, EventArgs e)
        {
            richGen.Text = "";
            sql = "select column_name, data_type, data_precision, data_scale";
            sql += " from ALL_TAB_COLUMNS ";
            sql += " where lower( TABLE_NAME) = '" + tableName.Text.Trim().ToLower() + "'";
            sql += " and owner = '" + d.user.ToUpper() + "'";
            sql += " ORDER BY COLUMN_ID";
            DataSet ds = d.get_data_text(sql);
            string colName = "", colType = "";
            
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                
                colName = r["column_name"].ToString().ToUpper();
                colType = r["data_type"].ToString().ToUpper();

                if (r["data_type"].ToString().ToLower() == "number")
                {
                    richGen.AppendText("[System.ComponentModel.DefaultValue(0)]\r\n");
                    richGen.AppendText("public ");
                    if (r["data_scale"].ToString().Trim() == "" || r["data_scale"].ToString().Trim() == "0")
                    {
                        if (int.Parse(r["data_precision"].ToString()) > 7)
                        {
                            richGen.AppendText(" long ");
                        }
                        else richGen.AppendText(" int ");
                    }
                    else
                    {
                        richGen.AppendText(" decimal ");
                    }
                }
                else if (r["data_type"].ToString().ToLower() == "blob")
                {
                    richGen.AppendText("public ");
                    richGen.AppendText(" byte[] ");
                }
                else
                {
                    richGen.AppendText("public ");
                    richGen.AppendText(" string ");
                }

                richGen.AppendText(colName);
                richGen.AppendText(" {get;set ;} \r\n");
                
            }

            richGen.AppendText(" public " + tableName.Text + " (");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colName = r["column_name"].ToString().ToUpper();
                colType = r["data_type"].ToString().ToUpper();

                if (r["data_type"].ToString().ToLower() == "number")
                {
                    if (r["data_scale"].ToString().Trim() == "" || r["data_scale"].ToString().Trim() == "0")
                    {
                        if (int.Parse(r["data_precision"].ToString()) > 7)
                        {
                            richGen.AppendText(" long ");
                        }
                        else richGen.AppendText(" int ");
                    }
                    else
                    {
                        richGen.AppendText(" decimal ");
                    }
                }
                else if (r["data_type"].ToString().ToLower() == "blob")
                {
                    richGen.AppendText(" byte[] ");
                }
                else
                {
                    richGen.AppendText(" string ");
                }

                richGen.AppendText(colName.ToLower());
                richGen.AppendText(",");
            }
            richGen.Text = richGen.Text.TrimEnd(',');
            richGen.AppendText(" ) ");
            richGen.AppendText(" {");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colName = r["column_name"].ToString().ToUpper();
                richGen.AppendText("this." + colName.ToUpper() + " = " + colName.ToLower() + ";\r\n");


            }
            richGen.AppendText(" }");
        }

        private void button56_Click(object sender, EventArgs e)
        {
            string colName = "", colType = "";
            string tablename = tableName.Text.Trim().ToUpper();
            
            richGen.Text = "";
            sql = "select column_name, data_type, data_precision, data_scale";
            sql += " from ALL_TAB_COLUMNS ";
            sql += " where lower( TABLE_NAME) = '" + tablename.ToLower() + "'";
            sql += " and owner = '" + schema.Text.ToUpper() + "'";
            sql += " ORDER BY COLUMN_ID";
            DataSet ds = d.get_data_text(sql);


            sql = " select cols.column_name ";
            sql += " FROM all_constraints cons, all_cons_columns cols ";
            sql += " WHERE cols.table_name = '" + tablename.ToUpper() + "'";
            sql += " AND cons.constraint_type = 'P'";
            sql += " AND cons.constraint_name = cols.constraint_name";
            sql += " AND cons.owner = '"+schema.Text.ToUpper()+"'";
            sql += " AND cols.owner = '" + schema.Text.ToUpper() + "'";
            sql += " ORDER BY cols.table_name, cols.position";

            DataSet dsKey = d.get_data_text(sql);
            ArrayList arrKey = new ArrayList();
            foreach (DataRow row in dsKey.Tables[0].Rows)
            {
                arrKey.Add(row["column_name"].ToString().ToUpper());
            }
            if (arrKey.Count == 0) arrKey.Add(ds.Tables[0].Rows[0][0].ToString().ToUpper());

            StringBuilder sbHeader = new StringBuilder("");

            #region header
            
            #region insert_upd
            sbHeader.Append("procedure insert_update_" + tablename.ToLower()+"( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append("p_" + colName);

                if (r["data_type"].ToString().ToLower() == "date")
                {
                    sbHeader.Append(" varchar2 ");
                }
                else sbHeader.Append(" in " + tablename + "." + colName + " % type ");
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(");\r\n ");
            #endregion insert_upd

            #region upd
            sbHeader.Append("procedure update_" + tablename.ToLower() + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append("p_" + colName);

                if (r["data_type"].ToString().ToLower() == "date")
                {
                    sbHeader.Append(" varchar2 ");
                }
                else sbHeader.Append(" in " + tablename + "." + colName + " % type ");
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(");\r\n ");
            #endregion upd

            #region insert 
            sbHeader.Append("procedure insert_" + tablename.ToLower() + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append("p_" + colName);

                if (r["data_type"].ToString().ToLower() == "date")
                {
                    sbHeader.Append(" varchar2 ");
                }
                else sbHeader.Append(" in " + tablename + "." + colName + " % type ");
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(");\r\n ");
            #endregion insert

            #region  get 
            sbHeader.Append("procedure get_" + tablename.ToLower() + "_all( io_cursor in out t_cursor);\r\n ");
            sbHeader.Append("procedure get_" + tablename.ToLower() + "( io_cursor in out t_cursor, ");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {
                    sbHeader.Append("p_" + _keyname);

                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(" varchar2 ");
                    }
                    else sbHeader.Append(" in " + tablename + "." + _keyname + " % type ");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" );\r\n");
            #endregion  get 

            #region delete
            sbHeader.Append("procedure del_" + tablename.ToLower() + "(");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {
                    sbHeader.Append("p_" + _keyname);

                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(" varchar2 ");
                    }
                    else sbHeader.Append(" in " + tablename + "." + _keyname + " % type ");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" );\r\n");
            #endregion delete

            #endregion header


            #region body

            #region insert_update
            sbHeader.Append("procedure insert_update_" + tablename.ToLower() + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append("p_" + colName);

                if (r["data_type"].ToString().ToLower() == "date")
                {
                    sbHeader.Append(" varchar2 ");
                }
                else sbHeader.Append(" in " + tablename + "." + colName + " % type ");
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(")\r\n ");
            sbHeader.Append(" is \r\n ");
            sbHeader.Append(" begin \r\n ");
            sbHeader.Append("           update set ");

            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                if (!isKey(arrKey, colName))
                {
                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(colName + " =to_date(p_" + colName + "','dd/mm/yyyy hh24:mi'),");
                    }
                    else sbHeader.Append(colName + " =p_" + colName + ",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" where " );
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {


                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(_keyname + " =to_char(p_" + _keyname + "','dd/mm/yyyy hh24:mi')");
                    }
                    else sbHeader.Append(_keyname + " =p_" + _keyname + "");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(";\r\n ");
            sbHeader.Append("     if sql % rowcount = 0 then \r\n");
            sbHeader.Append("           insert into  " + tablename + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append(colName );
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(") values( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                if (colType.ToLower() == "date")
                {
                    sbHeader.Append("to_date(p_" + colName + "','dd/mm/yyyy hh24:mi'),");
                }
                else sbHeader.Append(" p_" + colName + ",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" );\r\n ");
            sbHeader.Append(" end if;\r\n ");
            sbHeader.Append(" end;\r\n ");
            #endregion insert_update

            #region update
            sbHeader.Append("procedure update_" + tablename.ToLower() + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append("p_" + colName);

                if (r["data_type"].ToString().ToLower() == "date")
                {
                    sbHeader.Append(" varchar2 ");
                }
                else sbHeader.Append(" in " + tablename + "." + colName + " % type ");
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(")\r\n ");
            sbHeader.Append(" is \r\n ");
            sbHeader.Append(" begin \r\n ");
            sbHeader.Append("           update set ");

            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                if (!isKey(arrKey, colName))
                {
                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(colName + " =to_date(p_" + colName + "','dd/mm/yyyy hh24:mi'),");
                    }
                    else sbHeader.Append(colName + " =p_" + colName + ",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" where ");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {


                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(_keyname + " =to_char(p_" + _keyname + "','dd/mm/yyyy hh24:mi')");
                    }
                    else sbHeader.Append(_keyname + " =p_" + _keyname + "");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(";\r\n ");
            sbHeader.Append(" end;\r\n ");
            #endregion update

            #region insert
            sbHeader.Append("procedure insert_" + tablename.ToLower() + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append("p_" + colName);

                if (r["data_type"].ToString().ToLower() == "date")
                {
                    sbHeader.Append(" varchar2 ");
                }
                else sbHeader.Append(" in " + tablename + "." + colName + " % type ");
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(")\r\n ");
            sbHeader.Append(" is \r\n ");
            sbHeader.Append(" begin \r\n ");
            sbHeader.Append("           insert into  " + tablename + "( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                colName = r["column_name"].ToString().ToUpper();
                sbHeader.Append(colName);
                sbHeader.Append(",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(") values( ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                if (colType.ToLower() == "date")
                {
                    sbHeader.Append("to_date(p_" + colName + "','dd/mm/yyyy hh24:mi'),");
                }
                else sbHeader.Append(" p_" + colName + ",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" );\r\n ");
            sbHeader.Append(" end;\r\n ");
            #endregion insert

            #region get
            sbHeader.Append("procedure get_" + tablename.ToLower() + "_all( io_cursor in out t_cursor)\r\n ");
            sbHeader.Append("    is\r\n ");
            sbHeader.Append("begin\r\n ");
            sbHeader.Append("open io_cursor for\r\n ");
            sbHeader.Append(" select ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                if (colType.ToLower() == "date")
                {
                    sbHeader.Append("to_char(" + colName + "','dd/mm/yyyy hh24:mi') as " + colName + ",");
                }
                else sbHeader.Append( colName + ",");
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append("; \r\n");
            sbHeader.Append(" end; \r\n");

            sbHeader.Append("procedure get_" + tablename.ToLower() + "( io_cursor in out t_cursor, ");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {
                    sbHeader.Append("p_" + _keyname);

                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(" varchar2 ");
                    }
                    else sbHeader.Append(" in " + tablename + "." + _keyname + " % type ");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" )\r\n");
            sbHeader.Append("    is\r\n ");
            sbHeader.Append("begin\r\n ");
            sbHeader.Append("open io_cursor for\r\n ");
            sbHeader.Append(" select ");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                colType = r["data_type"].ToString().ToUpper();
                colName = r["column_name"].ToString().ToUpper();
                if (!isKey(arrKey, colName))
                {
                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(" to_char(" + colName + "','dd/mm/yyyy hh24:mi') as " + colName + ",");
                    }
                    else sbHeader.Append(colName + ",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" where ");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {


                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(_keyname + " =to_char(p_" + _keyname + "','dd/mm/yyyy hh24:mi')");
                    }
                    else sbHeader.Append(_keyname + " =p_" + _keyname + "");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append("; \r\n");
            sbHeader.Append(" end; \r\n");
            #endregion get

            #region delete
            sbHeader.Append("procedure del_" + tablename.ToLower() + "(");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {
                    sbHeader.Append("p_" + _keyname);

                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(" varchar2 ");
                    }
                    else sbHeader.Append(" in " + tablename + "." + _keyname + " % type ");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append(" )\r\n");
            sbHeader.Append("    is\r\n ");
            sbHeader.Append("begin\r\n ");
            sbHeader.Append("    delete from "+tablename+" where  ");
            foreach (string _keyname in arrKey)
            {
                colType = "";
                DataRow dr = d.getrowbyid(ds.Tables[0], "column_name='" + _keyname.ToUpper() + "'");
                if (dr != null) colType = dr["data_type"].ToString().ToLower();
                if (colType != "")
                {


                    if (colType.ToLower() == "date")
                    {
                        sbHeader.Append(_keyname + " =to_char(p_" + _keyname + "','dd/mm/yyyy hh24:mi')");
                    }
                    else sbHeader.Append(_keyname + " =p_" + _keyname + "");
                    sbHeader.Append(",");
                }
            }
            sbHeader = new StringBuilder(sbHeader.ToString().TrimEnd(','));
            sbHeader.Append("; \r\n");
            sbHeader.Append(" end; \r\n");
            #endregion delete


            #endregion body
            richGen.Text = sbHeader.ToString();

        }
        private bool isKey(ArrayList key,string col)
        {
            foreach (string keyname in key)
            {
                if (col.ToUpper() == keyname.ToUpper()) return true;
            }
            return false;
        }

        private void button57_Click(object sender, EventArgs e)
        {
            Thread threadStart = new Thread(getPatient);
            threadStart.Start();
        }
        private void getPatient()
        {
            //sql = "select distinct mabn from(";
            //sql += " select mabn from btdbn where instr('0119+',nam) > 0 and length(mabn)= 8 and mabn_old is null ";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0219+',nam) > 0 and length(mabn)= 8 and mabn_old is null";

            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0319+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0419+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0519+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0619+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0719+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0819+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('0919+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('1019+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('1119+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " union all";
            //sql += " select mabn from btdbn where instr('1219+',nam) > 0 and length(mabn)= 8 and mabn_old is null";
            //sql += " )";

            //DataSet dsData = d.get_data(sql);
            //long tong = dsData.Tables[0].Rows.Count;
            //long tt = 1;
            //foreach (DataRow _r in dsData.Tables[0].Rows)
            //{
            //    //lblSo.Refresh();
            //    //lblSo.Text = tt.ToString() + "/" + tong.ToString();
            //    //tt++;
            //    //lblSo.Refresh();

            //    DataSet dsPatient = d.getPatient(_r["mabn"].ToString());
            //    foreach (DataRow r in dsPatient.Tables[0].Rows)
            //    {
            //        Patient patient = new Patient();
            //        patient.id = r["mabn"].ToString();
            //        var hoten = LibUtility.Utility.tach_holot_ten(r["hoten"].ToString());
            //        patient.name = hoten.Split('~')[1];
            //        patient.surname = hoten.Split('~')[0];
            //        patient.sex = int.Parse(r["phai"].ToString());
            //        patient.birthyear = int.Parse(r["namsinh"].ToString());
            //        patient.birthdate = r["ngaysinh"].ToString();
            //        patient.mobile = r["didong"].ToString();
            //        patient.social_id = r["socmnd"].ToString();
            //        #region Country
            //        Country country = new Country();
            //        country.id = string.IsNullOrEmpty(r["manuoc"].ToString()) ? "VN" : r["manuoc"].ToString();
            //        country.code = country.id;
            //        country.name = string.IsNullOrEmpty(r["manuoc"].ToString()) ? "Việt Nam" : r["manuoc"].ToString();
            //        patient.country = country is null ? null : country;
            //        patient.country_code = country.id;
            //        #endregion Country

            //        #region City
            //        City city = new City();
            //        city.id = int.Parse(r["matt"].ToString());
            //        city.name = r["tentt"].ToString();
            //        city.country_code = "VN";
            //        patient.city = city is null ? null : city;
            //        #endregion City

            //        #region City
            //        District district = new District();
            //        district.id = r["maqu"].ToString();
            //        district.name = r["tenquan"].ToString();
            //        district.city_id = r["matt"].ToString();
            //        patient.district = district is null ? null : district;
            //        #endregion City

            //        #region Ward
            //        Ward ward = new Ward();
            //        ward.id = r["maphuongxa"].ToString();
            //        ward.name = r["tenpxa"].ToString();
            //        ward.district_id = r["maqu"].ToString();
            //        patient.ward = ward is null ? null : ward;
            //        #endregion Ward
            //        patient.city_id = r["matt"].ToString();
            //        patient.district_id = r["maqu"].ToString();
            //        patient.ward_id = r["maphuongxa"].ToString();
            //        patient.address = r["diachi"].ToString();



            //        #region get so the
            //        if (!string.IsNullOrEmpty(r["nam"].ToString()))
            //        {
            //            var nam = r["nam"].ToString();
            //            patient.bhyt_code = "";
            //            PatientSothe patientSothe = new PatientSothe();
            //            if (nam.Length >= 5)
            //            {
            //                nam = nam.Substring(nam.Length - 5, 4);
            //                if (!d.bMmyy(nam)) nam = d.s_curmmyy;
            //                DataSet dsSothe = d.get_sothebhyt(r["mabn"].ToString(), nam);

            //                List<PatientSothe> list = new List<PatientSothe>();
            //                foreach (DataRow row in dsSothe.Tables[0].Rows)
            //                {
            //                    patientSothe = new PatientSothe();
            //                    patientSothe.SOTHE = row["sothe"].ToString();
            //                    patientSothe.DENNGAY = row["DENNGAY"].ToString();
            //                    list.Add(patientSothe);
            //                }
            //                list.OrderByDescending(x => x.DENNGAY);
            //                foreach (PatientSothe item in list)
            //                {
            //                    patientSothe = item;
            //                    break;
            //                }

            //                if (patientSothe != null)
            //                {
            //                    patient.bhyt_code = patientSothe.SOTHE;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            patient.bhyt_code = "";
            //        }
            //        #endregion get so the

            //        string data = JsonConvert.SerializeObject(patient);

            //        try
            //        {
            //            StreamWriter writernew = new StreamWriter("Patient\\" + r["mabn"].ToString() + ".json", true, Encoding.UTF8);
            //            writernew.Write(data);
            //            writernew.Close();
            //            writernew.Dispose();
            //        }
            //        catch { }
            //        sql = "update btdbn set mabn_old=mabn where mabn='" + r["mabn"].ToString() + "'";
            //        d.execute_data(sql);
            //    }
            //}
        }


        private void button58_Click(object sender, EventArgs e)
        {
            int i_sole_giaban = 0;
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            int _makho = 0;
            try
            {
                _makho = int.Parse(makhoimp.ComboBox.SelectedValue.ToString());
            }
            catch { _makho = 0; }
            if (_makho == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kho");
                return;
            }
        
            long id = 0;
            mmyy = "1019";
            int _manguon = 1, _mabd = 0, _madv = 0;
            decimal dongia = 0, giamua = 0, giaban = 0, ton = 10000;
            DataRow dr;
            DataTable dtbd = d.get_data("select * from d_dmbd where nhom=" + i_nhomkho).Tables[0];

            foreach (DataRow r in dtbd.Rows)
            {
                try
                {
                    _mabd = int.Parse(r["id"].ToString());
                }
                catch { _mabd = 0; }
                if (_mabd > 0)
                {
                    
                    if (ton > 0)
                    {
                        try
                        {
                            dongia = decimal.Parse(r["DONGIA"].ToString());
                        }
                        catch { dongia = 0; }
                        giamua = dongia;


                        decimal tl = 0;


                        giaban = LibUtility.Utility.Round(giamua + giamua * tl / 100, i_sole_giaban);

                        _madv = 0;
                        dr = d.getrowbyid(dtbd, "id=" + _mabd);
                        if (dr != null)
                        {
                            _madv = int.Parse(dr["madv"].ToString());
                        }
                        id = d.get_id_tonkho;
                        if (!d.upd_theodoi(mmyy, id, _mabd, _manguon, _madv, "1222","LO"+_mabd.ToString(), "", "", "", 0, 0, 0, giamua, giaban, 0, 0, dongia, 0, 0))
                        {
                            MessageBox.Show("Không cập nhật được thông tin tồn kho");
                            return;
                        }

                        if (!d.upd_tonkhoct(mmyy, _makho, id, _mabd, ton, 0))
                        {
                            MessageBox.Show("Không cập nhật được thông tin tồn kho");
                            return;
                        }
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DataSet ds = d.get_data("select * from dmnoicapbhyt");
            string _ma = "", _ten = "", _diachi = "",_matuyen="",_maloai="",_mahang="",_mavung="",_matinh="";
            foreach (DataRow r in dt.Rows)
            {
                _ma = r["ma"].ToString().Trim().PadLeft(5, '0');
                _ten = r["ten"].ToString().Trim();
                _diachi = r["diachi"].ToString().Trim();

                DataRow row = d.getrowbyid(ds.Tables[0], "mabv='" + _ma + "'");
                if (row == null)
                {
                    _matuyen = dr["matuyen"].ToString();
                    _maloai = dr["maloai"].ToString();
                    _mahang = dr["mahang"].ToString();
                    _mavung = dr["mavung"].ToString();
                    _matinh = dr["matinh"].ToString();
                }
                else
                {
                    _matuyen = "0";
                    _maloai = "1";
                    _mahang = "4";
                    _mavung = "7";
                    _matinh = "719";
                }
                d.upd_tenvien("dmnoicapbhyt", _ma, _ten, r["diachi"].ToString(), "", _matuyen, _maloai, _mahang, _mavung, _matinh);
            }
            
        }

        private void button59_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(richText1.getText);
            //MessageBox.Show(richText1.getText);
        }

        private void button60_Click(object sender, EventArgs e)
        {
            //richText1.setText = "122";
        }

        private void button42_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 4;
            int _stt = 1;
            long _id = 4000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select a.*,3000000+matt as idtt from btdquan a").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MAQU"].ToString());
                _Cat.ID = _id;

                
                _Cat.Code = r["MAQU"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TENQUAN"].ToString();
                _Cat.MasterId = _MasterId;

                _Cat.ParentId = long.Parse(r["IDTT"].ToString());

                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;
                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatDistricts", json);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 5;
            int _stt = 1;
            long _id = 50000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select a.*,4000000+maQU as idQU from BTDPXA a ").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                string _strid = "5" + r["MAphuongxa"].ToString().PadLeft(7, '0');

                _id = long.Parse(_strid);
                _Cat.ID = _id;


                _Cat.Code = r["MAphuongxa"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TENpxa"].ToString();
                _Cat.MasterId = _MasterId;

                _Cat.ParentId = long.Parse(r["idQU"].ToString());

                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;
                _Cat.String2=r["viettat"].ToString();
                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatWards", json);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 6;
            int _stt = 1;
            long _id = 6000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from dmhangbv").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MA"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatHospitalGrades", json);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 7;
            int _stt = 1;
            long _id = 7000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from dmtuyenbv").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MA"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatHospitalRoutes", json);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 8;
            int _stt = 1;
            long _id = 8000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from dmloaibv").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MA"].ToString().Replace(".", ""));
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatHospitalTypes", json);
        }

        private void button47_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId =10;
            int _stt = 1;
            long _id = 1000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from btdnn").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MANN"].ToString().Replace(".", ""));
                _Cat.ID = _id;
                _Cat.Code = r["MANN"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TENNN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatCareerStandards", json);
        }

        private void button48_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 11;
            int _stt = 1;
            long _id = 11000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from btdkp").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MAKP"].ToString().Replace(".", ""));
                _Cat.ID = _id;
                _Cat.Code = r["MAKP"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TENKP"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;
                _Cat.String1 = r["MABH"].ToString();

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatDepartmentStandards", json);
        }

        private void button49_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 12;
            int _stt = 1;
            long _id = 1200000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from nhombhxh").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = int.Parse(r["stt"].ToString());
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatPriceGroupInsurances", json);
        }

        private void button50_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 13;
            int _stt = 1;
            long _id = 1300000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from icd_chapter").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID_chapter"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID_chapter"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["chapter"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatChapterICD10s", json);
        }

        private void button51_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 14;
            int _stt = 1;
            long _id = 1400000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select a.id_nhom as id,1300000+id_chapter as id_chapter,nhom as ten from icd_nhom a").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["id"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["id"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["ten"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = long.Parse(r["id_chapter"].ToString());
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatGroupICD10s", json);
        }

        private void button52_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId =15;
            int _stt = 1;
            long _id = 1500000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from phanloaipttt").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MA"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatSurgicalClassifications", json);
        }

        private void button53_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 16;
            int _stt = 1;
            long _id = 1600000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from loaipttt").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["MA"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["MA"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatSurgeryTypes", json);
        }

        private void button54_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 17;
            
            long _id = 1700000;
            long _idmaster = _id;
            CatShareDetails _Cat;



            _Cat = new CatShareDetails();
            _id = 1700000;
            _Cat.ID = _id;
            _Cat.Code = "0";

            _Cat.SortNumber = 0;
            _Cat.Name = "Khám sức khỏe";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 1700001;
            _Cat.ID = _id;
            _Cat.Code = "1";

            _Cat.SortNumber = 1;
            _Cat.Name = "Khám nội";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 1700002;
            _Cat.ID = _id;
            _Cat.Code = "2";

            _Cat.SortNumber = 2;
            _Cat.Name = "Khám ngoại";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 1700003;
            _Cat.ID = _id;
            _Cat.Code = "3";

            _Cat.SortNumber = 3;
            _Cat.Name = "Khám cơ xương khớp";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 1700004;
            _Cat.ID = _id;
            _Cat.Code = "4";

            _Cat.SortNumber = 4;
            _Cat.Name = "Khám da liễu";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);



            _Cat = new CatShareDetails();
            _id = 1700005;
            _Cat.ID = _id;
            _Cat.Code = "5";

            _Cat.SortNumber = 5;
            _Cat.Name = "Khám mắt";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 1700006;
            _Cat.ID = _id;
            _Cat.Code = "6";

            _Cat.SortNumber =6;
            _Cat.Name = "Khám mắt";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);


            _Cat = new CatShareDetails();
            _id = 1700007;
            _Cat.ID = _id;
            _Cat.Code = "7";

            _Cat.SortNumber = 7;
            _Cat.Name = "Khám tai mũi họng";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);


            _Cat = new CatShareDetails();
            _id = 1700008;
            _Cat.ID = _id;
            _Cat.Code = "8";

            _Cat.SortNumber = 8;
            _Cat.Name = "Khám sản phụ khoa";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);


            _Cat = new CatShareDetails();
            _id = 1700009;
            _Cat.ID = _id;
            _Cat.Code = "9";

            _Cat.SortNumber = 9;
            _Cat.Name = "Kết luận";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);


            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatHealthClinics", json);
        }

        private void button58_Click_1(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 18;
            int _stt = 1;
            long _id = 1800000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from v_loaibn").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatPatientTypes", json);
        }

        private void button77_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 19;
            int _stt = 1;
            long _id = 1900000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from dmtraituyen").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatOfflines", json);
        }

        private void button76_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 20;
            int _stt = 1;
            long _id = 2000000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select madantoc as id,dantoc as ten from btddt").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatFolk", json);
        }

        private void button75_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 22;

            long _id = 2200000;
            long _idmaster = _id;
            CatShareDetails _Cat;



            _Cat = new CatShareDetails();
            _id = 2200000;
            _Cat.ID = _id;
            _Cat.Code = "0";

            _Cat.SortNumber = 0;
            _Cat.Name = "Độc thân";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 2200001;
            _Cat.ID = _id;
            _Cat.Code = "1";

            _Cat.SortNumber = 1;
            _Cat.Name = "Lập gia đình";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

          

            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatMarriages", json);
        }

        private void button74_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 24;
            int _stt = 1;
            long _id = 2400000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select * from nhomnhanvien").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatEmployeeGroups", json);
        }

        private void button73_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 23;
            int _stt = 1;
            long _id = 2300000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select ma as id, a.* from nhantu a").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatReceives", json);
        }

        private void button72_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 25;
            int _stt = 1;
            long _id = 2500000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select a.ma as id, a.* from dm_11 a ").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;
                _Cat.String1 = r["STT"].ToString();
                _Cat.String2 = r["icd10"].ToString();

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatForm15s", json);
        }

        private void button71_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 27;
            int _stt = 1;
            long _id = 2700000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select ma as id, a.* from dentu a").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatPlaceOfIntroductions", json);
        }

        private void button70_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 29;

            long _id = 2900000;
            long _idmaster = _id;
            CatShareDetails _Cat;



            _Cat = new CatShareDetails();
            _id = 2900000;
            _Cat.ID = _id;
            _Cat.Code = "0";

            _Cat.SortNumber = 0;
            _Cat.Name = "Không";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 2900001;
            _Cat.ID = _id;
            _Cat.Code = "1";

            _Cat.SortNumber = 1;
            _Cat.Name = "Biến chứng";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);



            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatTreatmentComplications", json);
        }

        private void button69_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 30;

            long _id = 3000000;
            long _idmaster = _id;
            CatShareDetails _Cat;



            _Cat = new CatShareDetails();
            _id = 3000000;
            _Cat.ID = _id;
            _Cat.Code = "0";

            _Cat.SortNumber = 0;
            _Cat.Name = "Không";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);

            _Cat = new CatShareDetails();
            _id = 3000001;
            _Cat.ID = _id;
            _Cat.Code = "1";

            _Cat.SortNumber = 1;
            _Cat.Name = "Tai biến";
            _Cat.MasterId = _MasterId;
            _Cat.ParentId = 0;
            _Cat.Number1 = 0;
            _Cat.Number2 = 0;
            _Cat.Number3 = 0;
            _Cat.Number4 = 0;
            _Cat.Number5 = 0;
            list.Add(_Cat);



            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatCatastrophes", json);
        }

        private void button68_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 31;
            int _stt = 1;
            long _id = 3100000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select ma as id, a.* from mqsoftvl.gphaubenh a").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatPathologicalAnatomys", json);
        }

        private void button67_Click(object sender, EventArgs e)
        {
            List<CatShareDetails> list = new List<CatShareDetails>();
            int _MasterId = 32;
            int _stt = 1;
            long _id = 3200000;
            long _idmaster = _id;
            CatShareDetails _Cat;
            dt = d.get_data("select ma as id, a.* from mqsoftvl.chetdo a").Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatShareDetails();
                _id = _idmaster + long.Parse(r["ID"].ToString());
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.MasterId = _MasterId;
                _Cat.ParentId = 0;
                _Cat.Number1 = 0;
                _Cat.Number2 = 0;
                _Cat.Number3 = 0;
                _Cat.Number4 = 0;
                _Cat.Number5 = 0;

                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatCauseOfDeath", json);
        }

        private void button66_Click(object sender, EventArgs e)
        {
            List<CatHospitals> list = new List<CatHospitals>();
            
            int _stt = 1;
            long _id = 1;
            
            CatHospitals _Cat;
            sql = "select mabv as id,tenbv as ten,diachi,null as fax,7000000+matuyen as matuyen";
            sql += " ,nvl(maloai, 1) + 8000000 as maloai,2000000 +case when mavung = '0' then '1' else mavung end as mavung,3000000 + matinh as matinh ";
            sql += " ,decode(nvl(mahang, 1), 0, 1, nvl(mahang, 1)) + 6000000 as mahang ";

            sql += " from dmnoicapbhyt";
            dt = d.get_data(sql).Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatHospitals();
               
                _Cat.ID = _id;
                _Cat.Code = r["ID"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["TEN"].ToString();
                _Cat.HospitalTypeId = long.Parse(r["maloai"].ToString());
                _Cat.GeographicRegionId = long.Parse(r["mavung"].ToString());
                _Cat.HospitalRouteId = long.Parse(r["matuyen"].ToString());
                _Cat.CityId = long.Parse(r["matinh"].ToString());
                _Cat.HospitalGradeId = long.Parse(r["mahang"].ToString());

                _id++;
                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatHospitals", json);
        }

        private void button65_Click(object sender, EventArgs e)
        {
            List<CatICD10s> list = new List<CatICD10s>();

            int _stt = 1;
            long _id = 1;

            CatICD10s _Cat;
            sql = "select  2500000+nvl(b.ma,1)  as Form15Id,1400000+id_chapter as id_chapter";
            sql += " ,a.* from icd10 a left      join dm_11 b on a.stt = b.stt";
            dt = d.get_data(sql).Tables[0];
            foreach (DataRow r in dt.Rows)
            {

                _Cat = new CatICD10s();

                _Cat.ID = _id;
                _Cat.Code = r["cicd10"].ToString();

                _Cat.SortNumber = _stt;
                _Cat.Name = r["VVIET"].ToString();

                _Cat.Form15Id = long.Parse(r["Form15Id"].ToString());
                _Cat.GroupId = long.Parse(r["id_chapter"].ToString());
                _Cat.EquivalentCode = r["MATD"].ToString();
                _Cat.EquivalentName = r["GIAMDINH"].ToString();
                _Cat.IsChronic = r["BENHMANTINH"].ToString() == "1";
                _Cat.InsuranceCode = r["cicd10"].ToString();


                _id++;
                _stt++;
                list.Add(_Cat);
            }
            var json = JsonConvert.SerializeObject(list);
            Insertjson("CatICD10s", json);
        }

        private void button78_Click(object sender, EventArgs e)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (!d.upd_tenvien("tenvien", r["ma"].ToString(), r["ten"].ToString(), r["diachi"].ToString(), "", r["tuyen"].ToString(), "1", r["hang"].ToString(), "", r["ma"].ToString().Substring(0, 3)))
                {
                    LibUtility.Utility.MsgBox("");
                    return;
                }
                if (!d.upd_tenvien("dmnoicapbhyt", r["ma"].ToString(), r["ten"].ToString(), r["diachi"].ToString(), "", r["tuyen"].ToString(), "1", r["hang"].ToString(), "", r["ma"].ToString().Substring(0, 3)))
                    {
                    LibUtility.Utility.MsgBox("");
                    return;
                }
                if (!d.upd_tenvien("dstt", r["ma"].ToString(), r["ten"].ToString(), r["diachi"].ToString(), "", r["tuyen"].ToString(), "1", r["hang"].ToString(), "", r["ma"].ToString().Substring(0, 3)))
                    {
                    LibUtility.Utility.MsgBox("");
                    return;
                }

            }
            MessageBox.Show("OK");
        }

        private void button79_Click(object sender, EventArgs e)
        {
            string _ma = "",_viettat="";
            foreach (DataRow r in dt.Rows)
            {
                _ma = r["ma"].ToString();
                _viettat = r["viettat"].ToString();
                _viettat = LibUtility.Utility.Hoten_khongdau(_viettat);
                sql = "update btdpxa set viettat='" + _viettat + "' where maphuongxa_yk='" + _ma + "'";
                d.execute_data(sql);

            }
            MessageBox.Show("OK");
        }

        private void button80_Click(object sender, EventArgs e)
        {
            d.upd_btdtt("btdtt", "1", "000", ".");
            dt = d.get_data("select * from btdtt ").Tables[0];
            string maquan = "";
            foreach (DataRow r in dt.Rows)
            {
                maquan = r["matt"].ToString().PadLeft(3, '0') + "00";
                if (!d.upd_btdquan("btdquan", r["matt"].ToString(), maquan, "."))
                {
                    MessageBox.Show("Error");
                    return;
                }
                sql = "update btdquan set ma='" + r["ma"].ToString() + "' where maqu='" + maquan + "' ";
                d.execute_data(sql);

            }
            MessageBox.Show("OK");
        }

        private void button81_Click(object sender, EventArgs e)
        {
            
            dt = d.get_data("select * from btdquan ").Tables[0];
            string maphuongxa = "";
            foreach (DataRow r in dt.Rows)
            {
                maphuongxa = r["maqu"].ToString() + "00";
                if (!d.upd_btdpxa("btdpxa", r["maqu"].ToString(), maphuongxa, "."))
                {
                    MessageBox.Show("Error");
                    return;
                }
                sql = "update btdpxa set ma='" + r["ma"].ToString() + "' where maphuongxa='" + maphuongxa + "' ";
                d.execute_data(sql);

            }
            MessageBox.Show("OK");
        }

        private void button82_Click(object sender, EventArgs e)
        {
            int bhyt = 0;
            foreach (DataRow r in dt.Rows)
            {
                if (r["ten"].ToString() != "")
                {
                    try
                    {
                        bhyt = int.Parse(r["BHYT"].ToString());
                    }
                    catch {
                        bhyt = 0;
                    }
                    if (bhyt > 0)
                    {
                        sql = "update d_dmbd set bhyt=" + bhyt + " where id=" + r["id"].ToString();
                        d.execute_data(sql);
                    }
                }
                
            }
            MessageBox.Show("OK");
        }

        private void button83_Click(object sender, EventArgs e)
        {
            int bhyt = 0;
            foreach (DataRow r in dt.Rows)
            {
                if (r["ten"].ToString() != "")
                {
                    try
                    {
                        bhyt = int.Parse(r["hide"].ToString());
                    }
                    catch
                    {
                        bhyt = 0;
                    }
                    if (bhyt > 0)
                    {
                        sql = "update v_giavp set hide=" + bhyt + " where id=" + r["id"].ToString();
                        d.execute_data(sql);
                    }
                }

            }
            MessageBox.Show("OK");
        }

        private void CHIDINH_Click(object sender, EventArgs e)
        {
            dt.WriteXml("chidinh.xml", XmlWriteMode.WriteSchema);
        }

        private void button84_Click(object sender, EventArgs e)
        {
            dt.WriteXml("maxn.xml", XmlWriteMode.WriteSchema);
        }

        private void button85_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("chidinh.xml");
            gridControl1.DataSource = ds.Tables[0];

            DataSet ds1 = new DataSet();
            ds1.ReadXml("maxn.xml");
            gridControl2.DataSource = ds1.Tables[0];

            string macu = "";
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                macu = r["MACU"].ToString();
                if (macu != "")
                {
                    DataRow dataRow = d.getrowbyid(ds1.Tables[0], "MaDV='" + macu + "'");
                    if (dataRow != null)
                    {
                        r["IDYKHOA"] = dataRow["MaXNCha"];
                        sql = "update xn_bv_ten set hosttestcode='" + dataRow["MaXNCha"].ToString().Trim() + "' where id=" + r["IDHIS"].ToString();
                        d.execute_data(sql);
                    }
                }
            }
            LibUtility.Utility.MsgBox("Ok");
        }

        private void button86_Click(object sender, EventArgs e)
        {
            string TestCodeHIS = "", TestName = "", Testcode="";
            decimal id;
            foreach (DataRow r in dt.Rows)
            {
                id = d.get_id_xn_ten;
                TestCodeHIS = r["TestCodeHIS"].ToString();
                TestName = r["TestName"].ToString();
                Testcode = r["Testcode"].ToString();
                if (!d.upd_xn_ten(id, 1, 1, id, TestCodeHIS, TestName, TestCodeHIS, 0, "", "", 0, 0, 0, 0, 0, "", 0, "", 0, 0, 0, Testcode, TestName))
                {
                    LibUtility.Utility.MsgBox("");
                }
            }
            LibUtility.Utility.MsgBox("ok");
        }

        private void button87_Click(object sender, EventArgs e)
        {
            DataSet dsbvten = new DataSet();
            dsbvten = d.get_data("select * from xn_bv_ten ");

            DataSet dsten = new DataSet();
            dsten = d.get_data("select * from xn_ten ");

            string MaXNCha = "", MaXN = "";
            decimal id, id_bv_ten = 0, id_ten = 0;
            decimal stt = 1;
            foreach (DataRow r in dt.Rows)
            {
                MaXNCha = r["MaXNCha"].ToString();
                MaXN  = r["MaXN"].ToString();
                DataRow dataRow = d.getrowbyid(dsbvten.Tables[0], "hosttestcode='" + MaXNCha + "'");
                if (dataRow != null)
                {
                    id_bv_ten = decimal.Parse(dataRow["id"].ToString());
                    id_bv_ten = decimal.Parse(dataRow["id"].ToString());
                    id_bv_ten = decimal.Parse(dataRow["id"].ToString());
                    dataRow = d.getrowbyid(dsten.Tables[0], "ma='" + MaXN + "'");
                    if (dataRow != null)
                    {
                        id_ten = decimal.Parse(dataRow["id"].ToString());
                        id = d.get_id_xn_bv_chitiet;
                        if (!d.upd_xn_bv_chitiet(id, id_bv_ten, id_ten, stt, 0, MaXN, 0))
                        {
                            LibUtility.Utility.MsgBox("");
                        }
                        sql = "update xn_bv_chitiet set hosttestcode='" + MaXN + "' where id=" + id;
                        d.execute_data(sql);
                    }
                }
                stt++;
            }
            LibUtility.Utility.MsgBox("ok");
        }

        private void button88_Click(object sender, EventArgs e)
        {
            
            DataSet dsten = new DataSet();
            dsten = d.get_data("select * from xn_ten ");
            decimal id = 0;
            foreach (DataRow r in dsten.Tables[0].Rows)
            {
                id = decimal.Parse(r["id"].ToString());
                DataRow dataRow = d.getrowbyid(dt, "TestCode='" + r["machiso"].ToString() + "'");
                if (dataRow != null)
                {
                    if (!d.upd_xn_ten(id, dataRow["TestName"].ToString(), dataRow["TestName"].ToString())) 
                    {
                        LibUtility.Utility.MsgBox("");
                    }
                }
            }
            LibUtility.Utility.MsgBox("ok");
        }

        private void button89_Click(object sender, EventArgs e)
        {
            string MaXNCha = "", HOSTTESTCODE="", MauOng="";
            DataSet dsten = new DataSet();
            dsten = d.get_data("select * from xn_bv_ten ");
            decimal id = 0;
            DataSet dsMauong = new DataSet();
            dsMauong = d.get_data("select id,ten from dmmauong");
            foreach (DataRow r in dsMauong.Tables[0].Rows)
            {
                r["ten"] = LibUtility.Utility.Hoten_khongdau(r["ten"].ToString());
            }
                foreach (DataRow r in dsten.Tables[0].Rows)
            {
                id = decimal.Parse(r["id"].ToString());
                HOSTTESTCODE = r["HOSTTESTCODE"].ToString();
                DataRow dataRow = d.getrowbyid(dt, "MaXNCha='" + r["HOSTTESTCODE"].ToString() + "'");
                if (dataRow != null)
                {
                    MauOng = dataRow["MauOng"].ToString();
                    MauOng = LibUtility.Utility.Hoten_khongdau(MauOng);
                    DataRow dataRow1 = d.getrowbyid(dsMauong.Tables[0], "ten='" + MauOng + "'");
                    if (dataRow1 != null)
                    {
                        sql = "update xn_bv_ten set idong="+ dataRow1 ["id"].ToString()+ " where id="+id;
                        d.execute_data(sql);
                    }
                }
            }
            LibUtility.Utility.MsgBox("ok");
        }

        private void button90_Click(object sender, EventArgs e)
        {
            string  MauOng = "";
            decimal id = 0;
            DataSet dsMauong = new DataSet();
            dsMauong = d.get_data("select id,ten from dmmauong");
            foreach (DataRow r in dsMauong.Tables[0].Rows)
            {
                r["ten"] = LibUtility.Utility.Hoten_khongdau(r["ten"].ToString());
            }
            foreach (DataRow r in dt.Rows)
            {
                id = decimal.Parse(r["id"].ToString());
                MauOng = r["MauOng"].ToString();
                MauOng = LibUtility.Utility.Hoten_khongdau(MauOng);
                DataRow dataRow1 = d.getrowbyid(dsMauong.Tables[0], "ten='" + MauOng + "'");
                if (dataRow1 != null)
                {
                    sql = "update xn_bv_ten set idong=" + dataRow1["id"].ToString() + " where id=" + id;
                    d.execute_data(sql);
                }
            }
            LibUtility.Utility.MsgBox("ok");
        }


        private void pttt_Click(object sender, EventArgs e)
        {
         DataTable dtmuc = new DataTable();
            dtmuc = d.get_data("select * from " + user + ".muc order by id_muc").Tables[0];

            int stt = 1,id_muc=0;
            int idvp = 0;
            string mapt = "", mapttt = "", tenpttt = "", mamuc = "", manhom = "";
            foreach (DataRow r in dt.Rows)
            {
                manhom = r["manhom"].ToString().Trim();
                mamuc = r["MUC"].ToString().Trim();
                tenpttt = r["ten"].ToString().Trim();
                try
                {
                    idvp = int.Parse(r["mavp"].ToString());
                }
                catch { idvp = 0; }
                mapt = mamuc + r["stt"].ToString().PadLeft(3, '0');

                if (manhom == "P") mapttt = r["stt"].ToString().PadLeft(3, '0') + ".PT";
                else mapttt = r["stt"].ToString().PadLeft(3, '0') + ".TT";
                mapttt = mapttt + ".";
                mapttt = mapttt + mamuc.Replace("T", "").Replace("P", "").Trim();
                id_muc = int.Parse(mamuc.Replace("T", "").Replace("P", ""));

                if (!d.upd_dmpttt(mapt, mapttt, tenpttt, "", "", "", "", int.Parse(r["PHANLOAI"].ToString()), int.Parse(r["LOAI"].ToString()), id_muc, idvp, ""))
                {
                    LibUtility.Utility.MsgBox("");
                    return;
                }
                if (idvp > 0 && r["NHOM"].ToString() == "KTC")
                {
                    d.execute_data("update " + user + ".v_giavp set kythuat=0 where id=" + idvp);
                }
                stt++;
            }
            MessageBox.Show("OK");
        }


        private void btnnhombaocao_Click(object sender, EventArgs e)
        {
            DataTable dtTmp;
            string _khongdau_nhom  = "", m_id = "", _loai = "", _idnhom = "", _nhom = "";
            DataRow dr;
            if (!check_column_excel_giavp())
            {
                dt = new DataTable();
            }
            foreach (DataRow r in dt.Rows)
            {
                _loai = r["nhom_baocao"].ToString().Trim();
               
                if (_loai != "")
                {
                    

                    _khongdau_nhom = LibUtility.Utility.Hoten_khongdau(_loai);
                    
                    
                    
                        dtTmp = d.get_data("select id,ten,khongdau from mqhisroot.v_nhombaocao").Tables[0];

                        dr = d.getrowbyid(dtTmp, "khongdau='" + _khongdau_nhom + "'");
                        if (dr == null)
                        {
                            
                            m_id = d.get_id_v_nhombaocao.ToString();
                            if (!d.upd_v_nhombaocao(decimal.Parse(m_id),_loai,_khongdau_nhom))
                            {
                                MessageBox.Show("Error");
                                return;
                            }
                            d.execute_data("update v_nhombaocao set KHONGDAU='" + _khongdau_nhom + "' where id='" + m_id + "'");
                        }
                        
                    



                }
            }
            if (dt.Rows.Count > 0) MessageBox.Show("OK!");
        }
        private bool bExist_Col(string username, string table_name, string col_name)
        {
            return d.get_data_text(" select column_name from ALL_TAB_COLUMNS   where   owner='" + username.ToUpper() + "' and  table_name='" + table_name.ToUpper() + "' and column_name ='" + col_name.ToUpper() + "' ").Tables[0].Rows.Count > 0;
        }
        private bool bExist_table(DataSet dsTable, string tablename)
        {
            StringBuilder exp = new StringBuilder("table_name='" + tablename.ToUpper() + "'");
            DataRow r;
            r = Utility.getrowbyid(dsTable.Tables[0], exp.ToString());
            return r != null;
        }
        private bool f_kiemtramabndainser(string mabn_old)
        {
            bool kq = false;
            DataSet ds = new DataSet();
            string sql = "select mabn from "+user+".btdbn where mabn_old='"+ mabn_old + "' ";
            ds=d.get_data_text(sql);
            return ds.Tables[0].Rows.Count > 0;
        }
        private void btnthongtinhanhchinh_Click(object sender, EventArgs e)
        {
            //Thread thread = new Thread(chuyenbtdbn);
            //thread.Start();

            chuyenbtdbn();
        }
        private void chuyenbtdbn()
        {
            string m_matt = "", m_matt_yk = "", m_qu = "", m_qu_yk = "", mpx = "", mpx_yk = "", vt = "", s_madt = "25", s_tuoi = "", xa_kxd = "0000000", quan_kxd = "00000", tinh_kxd = "000", _ngaysinh = "", _namsinh = "", _mann = "99";
            string m_matt_thanhuy = "", m_qu_thanhuy = "", mpx_thanhuy="";
            DataSet dsTable = d.get_data_text("select table_name from dba_tables where owner='" + user.ToUpper() + "'");

            

            string sql = string.Empty;
            DataTable dt_tinhthanh = d.get_data("select * from " + user + ".btdtt ").Tables[0];
            DataTable dt_qu = d.get_data("select * from " + user + ".btdquan ").Tables[0];
            DataTable dt_tpx = d.get_data("select * from " + user + ".btdpxa ").Tables[0];
            DataTable dt_vungbv = d.get_data("select * from " + user + ".dmvungbv ").Tables[0];
            DataTable dt_btdnn = d.get_data("select * from " + user + ".btdnn_bv ").Tables[0];

            DataTable dtpxfull = d.get_data("select a.maqu,a.maphuongxa,b.matt from btdpxa a left join btdquan b on a.maqu=b.maqu").Tables[0];
            int i_phai = 0, i_docthan = 0;
            if (!check_column_excel_thongtinhanhchinh())
            {
                dt = new DataTable();
            }
            long tong = dt.Rows.Count;
            long tt = 1;
            foreach (DataRow r in dt.Rows)
            {
                lbl.Refresh();
                lbl.Text = tt.ToString() + " / " + tong.ToString();
                lbl.Refresh();

                if (!f_kiemtramabndainser(r["mabn"].ToString().Trim()))
                {
                    string mabn2 = d.ngayhienhanh_server.Substring(8, 2);
                    long stt = d.get_mabn(int.Parse(mabn2), 1, 1, true);
                    string mabn3 = stt.ToString().PadLeft(6, '0');
                    string s_mabn = mabn2 + mabn3;

                    if (r["gioitinh"].ToString() == "Nam") i_phai = 0;
                    else
                    {
                        if (r["gioitinh"].ToString() == "Nữ") i_phai = 0;
                        else i_phai = Int32.Parse(r["gioitinh"].ToString());
                    }
                        

                    if (r["tinhtranghonnhan"].ToString() != "")
                    {
                        i_docthan = Int32.Parse(r["tinhtranghonnhan"].ToString());
                        if (i_docthan == 1) i_docthan = 0;
                        else i_docthan = 1;
                    }
                    else i_docthan = 0;


                    if (r["madantoc"].ToString() == "" || r["madantoc"].ToString() == "1") s_madt = "25";
                    else if (r["madantoc"].ToString() == "4") s_madt = "20";
                    else if (r["madantoc"].ToString() == "5") s_madt = "23";
                    else if (r["madantoc"].ToString() == "7") s_madt = "37";
                    else if (r["madantoc"].ToString() == "17") s_madt = "4";
                    else if (r["madantoc"].ToString() == "37") s_madt = "29";
                    else if (r["madantoc"].ToString() == "55") s_madt = "55";
                    else s_madt = "25";




                    m_matt_yk = r["tinhthanh"].ToString().Trim().PadLeft(2, '0');
                    m_matt_thanhuy = r["tinhthanh"].ToString().Trim();
                    DataRow row = dt_tinhthanh.Select("matt_yk = '" + m_matt_yk + "' ").FirstOrDefault();
                    if (row != null)
                    {

                        m_matt = row["MATT"].ToString().Trim().PadLeft(3, '0');
                    }
                    else
                    {
                        m_matt = "000";
                        
                    }

                    _mann = r["MaNN"].ToString().Trim();
                    if (string.IsNullOrEmpty(_mann))
                    {
                        _mann = "99";
                    }
                    else
                    {
                        row = dt_btdnn.Select("mann = '" + _mann + "' ").FirstOrDefault();
                        if (row != null)
                        {
                            _mann = "99";
                        }
                    }


                    m_qu_yk = r["huyenquan"].ToString().Trim().PadLeft(3, '0');
                    m_qu_thanhuy = r["huyenquan"].ToString().Trim().PadLeft(3, '0');
                    row = dt_qu.Select("maqu_yk = '" + m_qu_yk + "' ").FirstOrDefault();
                    if (row != null)
                    {
                        m_qu = row["MAQU"].ToString().Trim().PadLeft(5, '0');

                    }
                    else
                    {
                        row = dt_qu.Select("matt = '" + m_matt + "' and tenquan='.' ").FirstOrDefault();
                        if (row != null) m_qu = row["MAQU"].ToString().Trim().PadLeft(5, '0');
                    }

                    mpx_yk = r["xaphuong"].ToString().Trim().PadLeft(5, '0');
                    mpx_thanhuy = r["xaphuong"].ToString().Trim();
                    row = dt_tpx.Select("maphuongxa_yk = '" + mpx_yk + "' ").FirstOrDefault();
                    if (row != null)
                    {
                        mpx = row["maphuongxa"].ToString().Trim().PadLeft(7, '0');
                    }
                    else
                    {
                        row = dt_tpx.Select("maqu = '" + m_qu + "' and tenpxa='.' ").FirstOrDefault();
                        if (row != null) mpx = row["maphuongxa"].ToString().Trim().PadLeft(7, '0');
                    }

                    row = dtpxfull.Select("maphuongxa='" + mpx + "'").FirstOrDefault();
                    if (row != null)
                    {
                        mpx = row["maphuongxa"].ToString().Trim();
                        m_qu = row["MAQU"].ToString().Trim();
                        m_matt = row["MATT"].ToString().Trim();
                    }
                    else
                    {
                        mpx = xa_kxd;
                        m_qu = quan_kxd;
                        m_matt = tinh_kxd;
                    }
                    _ngaysinh = r["ngaysinh"].ToString().Trim();
                    if (LibUtility.Utility.bNgay(_ngaysinh))
                    {
                        _namsinh = _ngaysinh.Split('/')[2];
                    }
                    else
                    {
                        _ngaysinh = "";
                        _namsinh = r["namsinh"].ToString().Trim();
                        if (_namsinh.Length != 4)
                        {
                            _namsinh = d.cur_yyyy;
                        }
                    }

                    if (!d.upd_btdbn(s_mabn, r["hoten"].ToString().Trim(), _ngaysinh, _namsinh, (i_phai == 1 ? 0 : 1), _mann, s_madt, r["sonha"].ToString().Trim(), r["thonpho"].ToString().Trim(), r["diachi"].ToString().Trim(), m_matt, m_qu, mpx, 0))
                    {
                     //   LibUtility.Utility.MsgBox("error");
                       // return;
                    }

                    sql = "update btdbn set nam='1120+', docthan=" + i_docthan + ", mabn_old='" + r["mabn"].ToString().Trim() + "' where mabn='" + s_mabn + "'"; //,cholam='"+mpx_thanhuy+","+m_qu_thanhuy+","+m_matt_thanhuy+"'
                    if (!d.execute_data(sql))
                    {
                        LibUtility.Utility.MsgBox("error");
                      //  return;
                    }
                    if (!string.IsNullOrEmpty(r["quanhe"].ToString().Trim()) || !string.IsNullOrEmpty(r["hotennguoinha"].ToString().Trim()) || !string.IsNullOrEmpty(r["diachinguoinha"].ToString().Trim()))
                    {
                        //if (!d.upd_benhnhanquanhe(s_mabn, r["quanhe"].ToString().Trim(), r["hotennguoinha"].ToString().Trim(), r["diachinguoinha"].ToString().Trim(), r["dienthoainguoinha"].ToString().Trim()))
                        //{
                        //    LibUtility.Utility.MsgBox("error");
                        //    return;
                        //}
                        if (!d.upd_quanhe(long.Parse(s_mabn), r["quanhe"].ToString().Trim(), r["hotennguoinha"].ToString().Trim(), r["diachinguoinha"].ToString().Trim(), r["dienthoainguoinha"].ToString().Trim()))
                        {
                        //    LibUtility.Utility.MsgBox("error");
                          //  return;
                        }
                    }
                    if (!string.IsNullOrEmpty(r["dienthoai"].ToString().Trim()))
                    {
                        if (!d.upd_dienthoai(s_mabn, r["dienthoai"].ToString().Trim()))
                        {
                         //   LibUtility.Utility.MsgBox("error");
                          //  return;
                        }
                    }
                    m_qu = m_matt = mpx = m_qu_yk = m_matt_yk = mpx_yk = "";
                }
                tt++;
            }

            MessageBox.Show("OK");
        }
        private void chuyenbtdbn_mabncu()
        {
            string m_matt = "", m_matt_yk = "", m_qu = "", m_qu_yk = "", mpx = "", mpx_yk = "", vt = "", s_madt = "25", s_tuoi = "", xa_kxd = "0000000", quan_kxd = "00000", tinh_kxd = "000", _ngaysinh = "", _namsinh = "", _mann = "99";

            string sql = string.Empty;
            DataTable dt_tinhthanh = d.get_data("select * from " + user + ".btdtt ").Tables[0];
            DataTable dt_qu = d.get_data("select * from " + user + ".btdquan ").Tables[0];
            DataTable dt_tpx = d.get_data("select * from " + user + ".btdpxa ").Tables[0];
            DataTable dt_vungbv = d.get_data("select * from " + user + ".dmvungbv ").Tables[0];
            DataTable dt_btdnn = d.get_data("select * from " + user + ".btdnn_bv ").Tables[0];

            DataTable dtpxfull = d.get_data("select a.maqu,a.maphuongxa,b.matt from btdpxa a left join btdquan b on a.maqu=b.maqu").Tables[0];
            int i_phai = 0, i_docthan = 0;
            //if (!check_column_excel_thongtinhanhchinh())
            //{
            //    dt = new DataTable();
            //}
            long tong = dt.Rows.Count;
            long tt = 1;
            foreach (DataRow r in dt.Rows)
            {
                //lbl.Refresh();
                //lbl.Text = tt.ToString() + " / " + tong.ToString();
                //lbl.Refresh();

                if (f_kiemtramabndainser(r["mabn"].ToString().Trim()))
                {
                    DataSet _dsTmp = d.get_hanhchanh_mabn_old(r["mabn"].ToString().Trim());
                    if(_dsTmp.Tables[0].Rows.Count>0)
                    {
                        string s_mabn = _dsTmp.Tables[0].Rows[0][0].ToString();
                        s_madt = _dsTmp.Tables[0].Rows[0]["madantoc"].ToString();

                        i_phai = Int32.Parse(r["gioitinh"].ToString());

                        if (r["tinhtranghonnhan"].ToString() != "")
                        {
                            i_docthan = Int32.Parse(r["tinhtranghonnhan"].ToString());
                            if (i_docthan == 1) i_docthan = 0;
                            else i_docthan = 1;
                        }
                        else i_docthan = 0;
                        DataRow row;


                        _mann = r["MaNN"].ToString().Trim();
                        if (string.IsNullOrEmpty(_mann))
                        {
                            _mann = "99";
                        }
                        else
                        {
                            row = dt_btdnn.Select("mann = '" + _mann + "' ").FirstOrDefault();
                            if (row != null)
                            {
                                _mann = "99";
                            }
                        }


                        //m_qu_yk = r["huyenquan"].ToString().Trim().PadLeft(3, '0');
                        //row = dt_qu.Select("maqu_yk = '" + m_qu_yk + "' ").FirstOrDefault();
                        //if (row != null)
                        //{
                        //    m_qu = row["MAQU"].ToString().Trim().PadLeft(5, '0');

                        //}
                        //else
                        //{
                        //    row = dt_qu.Select("matt = '" + m_matt + "' and tenquan='.' ").FirstOrDefault();
                        //    if (row != null) m_qu = row["MAQU"].ToString().Trim().PadLeft(5, '0');
                        //}

                        mpx_yk = r["xaphuong"].ToString().Trim().PadLeft(5, '0');
                        row = dt_tpx.Select("maphuongxa_yk = '" + mpx_yk + "' ").FirstOrDefault();
                        if (row != null)
                        {
                            mpx = row["maphuongxa"].ToString().Trim().PadLeft(7, '0');
                        }
                        else
                        {
                            row = dt_tpx.Select("maqu = '" + m_qu + "' and tenpxa='.' ").FirstOrDefault();
                            if (row != null) mpx = row["maphuongxa"].ToString().Trim().PadLeft(7, '0');
                        }

                        row = dtpxfull.Select("maphuongxa='" + mpx + "'").FirstOrDefault();
                        if (row != null)
                        {
                            mpx = row["maphuongxa"].ToString().Trim();
                            m_qu = row["MAQU"].ToString().Trim();
                            m_matt = row["MATT"].ToString().Trim();
                        }
                        else
                        {
                            mpx = xa_kxd;
                            m_qu = quan_kxd;
                            m_matt = tinh_kxd;
                        }
                        _ngaysinh = r["ngaysinh"].ToString().Trim();
                        if (LibUtility.Utility.bNgay(_ngaysinh))
                        {
                            _namsinh = _ngaysinh.Split('/')[2];
                        }
                        else
                        {
                            _ngaysinh = "";
                            _namsinh = r["namsinh"].ToString().Trim();
                            if (_namsinh.Length != 4)
                            {
                                _namsinh = d.cur_yyyy;
                            }
                        }

                        if (!d.upd_btdbn(s_mabn, r["hoten"].ToString().Trim(), _ngaysinh, _namsinh, (i_phai == 1 ? 0 : 1), _mann, s_madt, r["sonha"].ToString().Trim(), r["thonpho"].ToString().Trim(), r["diachi"].ToString().Trim(), m_matt, m_qu, mpx, 0))
                        {
                            //   LibUtility.Utility.MsgBox("error");
                            // return;
                        }

                        sql = "update btdbn set  docthan=" + i_docthan + ", mabn_old='" + r["mabn"].ToString().Trim() + "' where mabn='" + s_mabn + "'";
                        if (!d.execute_data(sql))
                        {
                            LibUtility.Utility.MsgBox("error");
                            //  return;
                        }
                        if (!string.IsNullOrEmpty(r["quanhe"].ToString().Trim()) || !string.IsNullOrEmpty(r["hotennguoinha"].ToString().Trim()) || !string.IsNullOrEmpty(r["diachinguoinha"].ToString().Trim()))
                        {
                                if (!d.upd_quanhe(long.Parse(s_mabn), r["quanhe"].ToString().Trim(), r["hotennguoinha"].ToString().Trim(), r["diachinguoinha"].ToString().Trim(), r["dienthoainguoinha"].ToString().Trim()))
                            {
                             }
                        }
                        if (!string.IsNullOrEmpty(r["dienthoai"].ToString().Trim()))
                        {
                            if (!d.upd_dienthoai(s_mabn, r["dienthoai"].ToString().Trim()))
                            {
                               
                            }
                        }

                    }

                   
                    m_qu = m_matt = mpx = m_qu_yk = m_matt_yk = mpx_yk = "";
                }
                tt++;
            }

            MessageBox.Show("OK");
        }

        private void chuyenbtdbn_mabncuS()
        {

            long tong = dt.Rows.Count;
            long tt = 1;
            string _ngaysinh = "", _namsinh = "", _dd = "", _mm = "",_yyyy="";
            foreach (DataRow r in dt.Rows)
            {
                lbl.Refresh();
                lbl.Text = tt.ToString() + " / " + tong.ToString();
                lbl.Refresh();

                if (f_kiemtramabndainser(r["mabn"].ToString().Trim()))
                {
                    _ngaysinh = "";
                    DataSet _dsTmp = d.get_hanhchanh_mabn_old(r["mabn"].ToString().Trim());
                    if (_dsTmp.Tables[0].Rows.Count > 0)
                    {
                        string s_mabn = _dsTmp.Tables[0].Rows[0][0].ToString();
                        _dd = r["ngay"].ToString().Trim();
                        _mm = r["thang"].ToString().Trim();
                        _namsinh = r["namsinh"].ToString().Trim();
                        if (_namsinh.Length != 4)
                        {
                            _namsinh = d.cur_yyyy;
                        }
                        if (_dd != "" && _mm != "")
                        {
                            _ngaysinh = _dd.PadLeft(2,'0') + "/" + _mm.PadLeft(2, '0') + "/" + _namsinh;
                        }
                        sql = "update btdbn set  namsinh='" + _namsinh + "'";
                        if (_ngaysinh != "") sql += ", ngaysinh=to_date('" + _ngaysinh + "','dd/mm/yyyy') ";
                        sql += " where mabn='" + s_mabn + "'";
                        if (!d.execute_data(sql))
                        {
                            LibUtility.Utility.MsgBox("error");
                            //  return;
                        }
                    }
                    
                }
                tt++;
            }

            MessageBox.Show("OK");
        }
        private void button90_Click_1(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            DataTable  dtNhomctduoc;
             dtNhomctduoc = d.get_data("select distinct id23 as id,nvl(ten1,ten) as ten,khongdau from bieu_07_23 where ma in (2,3,4) and ten1 is not null order by id23").Tables[0];
            string MADUONGDUNG = "", _nhomcongtacduoc="";
            int _phuluc3 = 0;
            foreach (DataRow r in dt.Select("true", "ten"))
            {

                if (r["id"].ToString().Trim() != "")
                {
                    try { _id = long.Parse(r["id"].ToString()); } catch { _id = 0; }
                    if (_id > 0)
                    {
                        MADUONGDUNG = r["MADUONGDUNG"].ToString();
                        d.upd_dmbd_col(_id, "MADUONGDUNG", MADUONGDUNG);
                        _nhomcongtacduoc = r["nhomcongtacduoc"].ToString().Trim();
                        if (_nhomcongtacduoc == "") _nhomcongtacduoc = "Không xác định";
                        _nhomcongtacduoc = LibUtility.Utility.Hoten_khongdau(_nhomcongtacduoc);
                        dr = d.getrowbyid(dtNhomctduoc, "khongdau='" + _nhomcongtacduoc + "'");
                        if (dr != null) _phuluc3 = int.Parse(dr["id"].ToString());
                        else
                        {
                            _phuluc3 = 0;
                        }
                        //sql = "update d_dmbd set phuluc3=" + _phuluc3 + " where id=" + _id;
                        //d.execute_data(sql);
                    }
                }
            }
            MessageBox.Show("Ok");
         
        }

        private void button92_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            
            int _phuluc3 = 0;
            foreach (DataRow r in dt.Select("true", "ten"))
            {

                if (r["id"].ToString().Trim() != "")
                {
                    try { _id = long.Parse(r["id"].ToString()); } catch { _id = 0; }
                    if (_id > 0)
                    {
                        _phuluc3 = 0;
                        try {
                            _phuluc3 = int.Parse(r["nhomcongtacduoc"].ToString().Trim());
                        } catch { }
                        
                        sql = "update d_dmbd set phuluc3=" + _phuluc3 + " where id=" + _id;
                        d.execute_data(sql);
                    }
                }
            }
            MessageBox.Show("Ok");
        }

        private void button93_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(chuyenbtdbn_mabncu);
            thread.Start();

            //chuyenbtdbn_mabncu();
        }

        private void button94_Click(object sender, EventArgs e)
        {

            foreach (DataRow r in dt.Rows)
            {
                sql = "update btdbn set mabn_old ='" + r["mabncu"].ToString() + "' where mabn='" + r["mabn"].ToString() + "'";
                d.execute_data(sql);
            }
            MessageBox.Show("Ok");
        }

        private void button95_Click(object sender, EventArgs e)
        {
            //Thread thread = new Thread(chuyenbtdbn_mabncuS);
            //thread.Start();

           chuyenbtdbn_mabncuS();
        }

        private void button96_Click(object sender, EventArgs e)
        {
            int i_sole_giaban = 0;
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            int _makho = 0, _makp = 0;
            try
            {
                _makho = int.Parse(makhoimp.ComboBox.SelectedValue.ToString());
            }
            catch { _makho = 0; }
            if (_makho == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kho");
                return;
            }

            try
            {
                _makp = int.Parse(makpimp.ComboBox.SelectedValue.ToString());
            }
            catch { _makp = 0; }
            if (_makp == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kp");
                return;
            }


          

            long id = 0;
            if (txtthangton.Text.Trim() == "" || txtthangton.Text.Trim().Length != 4)
            {
                LibUtility.Utility.MsgBox("Tháng tồn không hợp lệ!");
                return;
            }
            mmyy = txtthangton.Text.Trim();
            int _manguon = 0, _mabd = 0, _madv = 0;
            decimal dongia = 0, giamua = 0, giaban = 0, ton = 0;
            string _handung = "";
            DataRow dr;
            DataTable dtbd = d.get_data("select id,madv from d_dmbd where nhom=" + i_nhomkho).Tables[0];
            DataTable dtTheodoi = new DataTable();
            sql = "select a.*,b.* from " + user + mmyy + ".d_tutrucct a inner join " + user + mmyy + ".d_theodoi b on a.stt = b.id where ";
            sql += "  a.makp=" + _makp + " and a.makho=" + _makho;

            dtTheodoi = d.get_data(sql).Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                if (r["mabd"].ToString().Trim() != "")
                {
                    try
                    {
                        _mabd = int.Parse(r["MABD"].ToString());
                    }
                    catch { _mabd = 0; }
                    if (_mabd > 0)
                    {
                        try
                        {
                            dongia = decimal.Parse(r["dongia"].ToString());
                        }
                        catch { dongia = 0; }
                        try
                        {
                            giaban = decimal.Parse(r["giaban"].ToString());
                        }
                        catch { giaban = 0; }
                        if (giaban > 0)
                        {
                            sql = "update d_dmbd set dongia=" + dongia + " where id=" + _mabd;
                            d.execute_data(sql);
                        }

                        dr = d.getrowbyid(dtTheodoi, "mabd=" + _mabd);
                        if (dr != null)
                        {
                            id = long.Parse(dr["ID"].ToString());
                            sql = "update " + user + mmyy + ".d_theodoi set giamua=" + dongia + ",giaban=" + giaban + " where id=" + id+" and giamua=0 and mabd="+_mabd;
                            d.execute_data(sql);
                        }
                    }
                }
            }
            MessageBox.Show("OK");
        }


        private void button97_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "", nhomin = "";
            DataTable dtTmpNhomin = d.get_data("select * from d_nhomin where nhom=" + i_nhom).Tables[0];
            dtTmp = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
            DataRow drnhomin;
            foreach (DataRow r in dt.Select("true", "nhom"))
            {
                _ten = r["nhom"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
                       dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        try
                        {
                            _id = long.Parse(d.get_data("select max(id) from " + user + ".d_dmnhom").Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        catch { _id = 1; }
                        d.upd_dmnhom(_id, _ten, 1, i_nhom, 0, 0, LibUtility.Utility.get_stt(dtTmp), 0);
                        d.execute_data("update d_dmnhom set khongdau='" + kdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button98_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            

            string _mahc = "", _mabd = "", _nhom = "1", _loai = "1", _nuoc = "", _hang = "", _dang = "", _tenhc = "", _khongdau = "", _nhomin = "", _tt31 = "", _sodk = "", _bhyttra = "", _nhacckd = "", _STTQD = "", _MABYT = "", _hamluong = "", _DVSD = "", _nhacc = "", STTQD = "", madungchung = "", TTTHAU = "", ma_bv, nhombo = "", sotk = "", _nhomcongtacduoc = "";
            string MADUONGDUNG = "", DUONGDUNG = "", SODK = "", DONGGOI = "", QUYETDINH = "", CONGBO = "", LOAITHUOC = "", LOAITHAU = "", NHOMTHAU = "", MATHAU = "", maatc = "", namthau = "", SOHD = "", ngayhd = "", cacdung = "", nhomdieutri, nhomin;
            string MANHOMVTYT, TENNHOMVTYT;
            int _idnhom = 1, _idloai = 1, _idhang = 0, _idnuoc = 1, _idnhomin = 2, _idnhacc = 0, _nhombo = 0, _sotk = 0, _nhomdt = 0, _phuluc3 = 0;
            int d_stt = 0, _dmuc = 0;
            decimal _tyle = 0, _dongia = 0, SLDONGGOI = 0, GIATHAU = 0, gia_bh = 0, slthau = 0, giaban = 0, giamua, giathau;
            DataTable dtNhom, dtLoai, dtHang, dtBd, dtNuoc, dtnhombo, dtsotk, dtNhomdieutri, dtNhomin, dtNhomctduoc;
            dtNhom = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
            dtLoai = d.get_data("select * from d_dmloai where nhom=" + i_nhom).Tables[0];
            
            dtHang = d.get_data("select * from d_dmhang where nhom=" + i_nhom).Tables[0];
            dtNuoc = d.get_data("select * from d_dmnuoc where nhom=" + i_nhom).Tables[0];

            dtnhombo = d.get_data("select * from d_nhombo where nhom=" + i_nhom).Tables[0];
            dtsotk = d.get_data("select * from d_dmnhomkt where nhom=" + i_nhom).Tables[0];
            dtNhomdieutri = d.get_data("select * from d_dmnhomdt where nhom=" + i_nhom).Tables[0];
            dtNhomin = d.get_data("select * from d_nhomin where nhom=" + i_nhom).Tables[0];
            dtNhomctduoc = d.get_data("select distinct id23 as id,nvl(ten1,ten) as ten from bieu_07_23 where ma in (2,3,4) and ten1 is not null order by id23").Tables[0];

            DataTable dtnhacc = d.get_data("select * from d_dmnx where nhom=" + i_nhom).Tables[0];
            int _stt = 1, hide = 0;
            decimal TLHAOHUT = 0;
            string NGAYHIEULUCTHAU = "", NGAYHETHIEULUCTHAU = "";
            foreach (DataRow r in dt.Select("true", "ten"))
            {
                if (r["ten"].ToString().Trim() != "")
                {
                    madungchung = r["madungchung"].ToString();
                    _ten = r["TEN"].ToString().Trim();
                    _nhom = r["nhom"].ToString().Trim();
                    if (_nhom == "") _nhom = "Không xác định";
                    _nhom = LibUtility.Utility.Hoten_khongdau(_nhom);
                    dr = d.getrowbyid(dtNhom, "khongdau='" + _nhom + "'");
                    if (dr != null) _idnhom = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhóm");
                        return;
                    }

                    _hang = r["hang"].ToString().Trim();
                    if (_hang == "") _hang = "Không xác định";
                    _hang = LibUtility.Utility.Hoten_khongdau(_hang);
                    dr = d.getrowbyid(dtHang, "khongdau='" + _hang + "'");
                    if (dr != null) _idhang = int.Parse(dr["id"].ToString());
                    else
                    {
                        _idhang = 1;
                        LibUtility.Utility.MsgBox("Hảng");
                        return;
                    }

                    
                    MADUONGDUNG = r["MADUONGDUNG"].ToString();
                    DUONGDUNG = r["DUONGDUNG"].ToString();
                    
                    LOAITHAU = r["LOAITHAU"].ToString();
                    if (LOAITHAU == "") LOAITHAU = "0";
                    
                    _id = 0;
                    try { _id = long.Parse(r["id"].ToString()); } catch { _id = 0; }

                    sql = "update d_dmbd set manhom=" + _idnhom + ",mahang=" + _idhang + " where id=" + _id;
                    d.execute_data(sql);

                    d.upd_dmbd_col(_id, "TEN", _ten);
                    d.upd_dmbd_col(_id, "MADUONGDUNG", MADUONGDUNG);
                    d.upd_dmbd_col(_id, "DUONGDUNG2182", MADUONGDUNG);
                    d.upd_dmbd_col(_id, "MA2182", madungchung);
                    
                    d.upd_dmbd_col(_id, "DUONGDUNG", DUONGDUNG);

                    d.upd_dmbd_ten_s(_id, r["ten_s"].ToString().Trim());
                    sql = "update " + user + ".d_dmbdthongtu set loaithau=" + LOAITHAU + " where id=" + _id;

                    d.execute_data(sql);
                }
            }

            MessageBox.Show("Ok");
        }

        private void btnimpcotduoc_Click(object sender, EventArgs e)
        {
            oracle.TransactionBegin();
            string s_tencot = "";
            long id = 0;
            string cotgia = cbdanhmucduoc.Text;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = long.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                s_tencot = row[cotgia].ToString();
                            }
                            catch (Exception ex)
                            {
                                oracle.TransactionRollback();
                                MessageBox.Show(ex.ToString());

                            }
                            if (s_tencot != "")
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".d_dmbd where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_dmbd_luu(id))
                                    {
                                        oracle.TransactionRollback();
                                        MessageBox.Show("error");

                                    }
                                    else
                                    {
                                        sql = "update " + user + ".d_dmbd set " + cotgia + "='" + s_tencot + "',ngayud=sysdate where id=" + id;
                                        d.execute_data(sql);
                                    }

                                }
                                
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oracle.TransactionRollback();
                        MessageBox.Show(ex.ToString());
                        id = 0;

                    }

                }
            }
            oracle.TransactionCommit();
            MessageBox.Show("OK");
        }

        //private void button97_Click(object sender, EventArgs e)
        //{
        //   int i_nhomkho = int.Parse(cboNhomkho.SelectedValue.ToString());
        //    long _id = 0;
        //    bool bOk = false;
        //    dtTmp = d.get_data("select * from d_dmhang where nhom=" + i_nhomkho).Tables[0];
        //    foreach (DataRow r in dt.Rows)
        //    {
        //        if (r["id"].ToString() != "") _id = long.Parse(r["id"].ToString());
        //        {
        //            if (r["tenhang"].ToString().Trim() != "")
        //            {
        //                _ten = r["tenhang"].ToString().Trim();
        //                _ten = _ten.Replace("'", "").Trim();
        //                if (_ten != "")
        //                {
        //                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
        //                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
        //                    if (dr != null)
        //                    {
        //                        d.upd_dmbd_luu(_id);
        //                        d.execute_data("update d_dmbd set mahang=" + long.Parse(dr["id"].ToString()) + " where id=" + _id);

        //                    }

        //                }

        //            }
        //        }
        //    }
        //    MessageBox.Show("OK!");
        //}

        private void button4_Click(object sender, EventArgs e)
        {

        }
        private string get_nhom_tyle2(int nhomkho)
        {
            sql = "select ten from d_thongso where id=276 and nhom=" + nhomkho;
            foreach (DataRow r in d.get_data(sql).Tables[0].Rows)
            {
                return r["ten"].ToString();
            }
            return "";
        }

        private void button34_Click(object sender, EventArgs e)
        {
            int i_sole_giaban = 0;
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            string nhom_tyle2 = get_nhom_tyle2(i_nhomkho);

            int _makho = 0;
            try
            {
                _makho = int.Parse(makhoimp.ComboBox.SelectedValue.ToString());
            } catch { _makho = 0; }
            if (_makho == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kho");
                return;
            }
            if (!checkdm()) return;

            long id = 0;
            if(txtthangton.Text.Trim()==""|| txtthangton.Text.Trim().Length!=4)
            {
                LibUtility.Utility.MsgBox("Tháng tồn không hợp lệ!");
                return;
            }
            mmyy =txtthangton.Text.Trim();
            int _manguon = 0, _mabd = 0, _madv = 0;
            _manguon = int.Parse(manguonimp.ComboBox.SelectedValue.ToString());
            decimal dongia = 0, giamua = 0, giaban = 0, ton = 0;
            string _handung = "";
            DataRow dr;
            DataTable dtbd = d.get_data("select id,madv from d_dmbd where nhom=" + i_nhomkho).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                if (r["mabd"].ToString().Trim() != "")
                {
                    try
                    {
                        _mabd = int.Parse(r["MABD"].ToString());
                    }
                    catch { _mabd = 0; }
                    if (_mabd > 0)
                    {
                        try
                        {
                            ton = decimal.Parse(r["TON"].ToString());
                        }
                        catch { ton = 0; }
                        if (ton > 0)
                        {
                            try
                            {
                                dongia = decimal.Parse(r["DONGIA"].ToString());
                            }
                            catch { dongia = 0; }
                            giamua = dongia;
                            _handung = r["handung"].ToString().Trim();
                            if (_handung.Length >= 10)
                            {
                                _handung = _handung.Split('/')[1] + _handung.Split('/')[0] + _handung.Split('/')[2].Substring(2,2);
                            }
                            else if (_handung.Length == 6)
                            {
                                _handung = _handung.Trim();
                            }
                            else if (_handung.Length == 7)
                            {
                                _handung = _handung.Split('/')[0] + _handung.Split('/')[1].Substring(2, 2);
                            }
                            else 
                            if (_handung.Length == 4&&_handung.IndexOf("/")!=-1)
                            {
                                _handung ="00"+_handung.Split('/')[0] + _handung.Split('/')[1];
                            }
                            if (_handung.Length == 4 && _handung.IndexOf("/") == -1)
                            {
                                _handung = "00" + _handung;
                            }
                          

                            try
                            {
                                dongia = decimal.Parse(r["dongia"].ToString());
                            }
                            catch { dongia = 0; }
                            try
                            {
                                giaban = decimal.Parse(r["giaban"].ToString());
                            }
                            catch { giaban = 0; }
                            if (chkGiabanTyle.Checked)
                            {
                                decimal tl = d.get_tyleban(_mabd, nhom_tyle2, i_nhomkho, dongia);

                                giaban = LibUtility.Utility.Round(dongia + dongia * tl / 100, i_sole_giaban);
                                
                            }
                            if (giaban > 0)
                            {
                                sql = "update d_dmbd set giaban=" + giaban + " where id=" + _mabd + " and giaban<" + giaban;
                                d.execute_data(sql);
                            }
                            giamua = dongia;
                            _madv = 0;
                            dr = d.getrowbyid(dtbd, "id=" + _mabd);
                            if (dr != null)
                            {
                                _madv = int.Parse(dr["madv"].ToString());
                            }
                            id = d.get_id_tonkho;
                            if (!d.upd_theodoi(mmyy, id, _mabd, _manguon, _madv, _handung.Trim(), r["losx"].ToString().Trim(), "", "", "", 0, 0, 0, giamua, giaban, 0, 0, dongia, 0, 0))
                            {
                                MessageBox.Show("Không cập nhật được thông tin tồn kho");
                                return;
                            }

                            if (!d.upd_tonkhoct(mmyy, _makho, id, _mabd, ton, 0))
                            {
                                MessageBox.Show("Không cập nhật được thông tin tồn kho");
                                return;
                            }
                        }
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button110_Click(object sender, EventArgs e)
        {
            if (colGiavp.SelectedIndex == -1) return;
            if (colExcel.Text == "") return;
            decimal id = 0;
            string cotgia = colGiavp.Text, _colExcel = colExcel.Text;
            string value = "";
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    id = decimal.Parse(r["ID"].ToString());
                }
                catch { id = 0; }
                if (id > 0)
                {
                    try
                    {
                        value = r[_colExcel].ToString().Trim();
                    }
                    catch { value = ""; }
                    if (value != "")
                    {
                        d.upd_v_giavp(id, value, cotgia);
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button111_Click(object sender, EventArgs e)
        {
            if (colGiavp.SelectedIndex == -1) return;
            if (colExcel.Text == "") return;
            decimal id = 0;
            string cotgia = colGiavp.Text, _colExcel = colExcel.Text;
            decimal value = 0;
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    id = decimal.Parse(r["ID"].ToString());
                }
                catch { id = 0; }
                if (id > 0)
                {
                    try
                    {
                        value = decimal.Parse(r[_colExcel].ToString().Trim());
                        d.upd_v_giavp(id, value, cotgia);
                    }
                    catch { value = 0; }

                }
            }
            MessageBox.Show("OK");
        }

        private void button100_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            DataTable dtTmpNhomin = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nhomdieutri"))
            {
                _ten = r["nhomdieutri"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    dr = d.getrowbyid(dtTmpNhomin, "khongdau='" + kdau + "'");
                    if (dr != null)
                    {


                        d.execute_data("update d_dmbd set manhom=" + long.Parse(dr["id"].ToString()) + " where id=" + _id);

                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button99_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            DataTable dtTmpNhomin = d.get_data("select * from d_dmnx where nhom=" + i_nhom).Tables[0];
            
           
            foreach (DataRow r in dt.Select("true", "nhacc"))
            {
                _ten = r["nhacc"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    dr = d.getrowbyid(dtTmpNhomin, "khongdau='" + kdau + "'");
                    if (dr != null)
                    {
                        
                        
                        d.execute_data("update d_dmbd set madv="+long.Parse(dr["id"].ToString())+" where id=" + _id);
                       
                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button101_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            DataTable dtTmpNhomin = d.get_data("select * from d_dmnuoc where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nuoc"))
            {
                _ten = r["nuoc"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    dr = d.getrowbyid(dtTmpNhomin, "khongdau='" + kdau + "'");
                    if (dr != null)
                    {


                        d.execute_data("update d_dmbd set manuoc=" + long.Parse(dr["id"].ToString()) + " where id=" + _id);

                    }
                }
            }
            MessageBox.Show("OK");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnkiemtraloaivp_Click(object sender, EventArgs e)
        {
            DataTable dtTmp, dtTmpLoai;
            string _khongdau_nhom = "", _khongdau_loai = "", m_id = "", _loai = "", _idnhom = "", _nhom = "";
            DataRow dr, drloai;
            if (!check_column_excel_giavp())
            {
                dt = new DataTable();
            }
            foreach (DataRow r in dt.Rows)
            {
                _loai = r["LOAI"].ToString().Trim();
                _nhom = r["NHOM"].ToString().Trim();
                if (_loai != "")
                {
                    dtTmpLoai = d.get_data("select id,ten,khongdau from mqhisroot.v_loaivp").Tables[0];

                    _khongdau_nhom = LibUtility.Utility.Hoten_khongdau(_nhom);
                    _khongdau_loai = LibUtility.Utility.Hoten_khongdau(_loai);
                    drloai = d.getrowbyid(dtTmpLoai, "khongdau='" + _khongdau_loai + "'");
                    if (drloai == null)
                    {
                        dtTmp = d.get_data("select ma,ten,khongdau from mqhisroot.v_nhomvp").Tables[0];

                        dr = d.getrowbyid(dtTmp, "khongdau='" + _khongdau_nhom + "'");
                        if (dr != null)
                        {
                            _idnhom = dr["ma"].ToString();
                            m_id = d.get_id_v_loaivp.ToString();
                            if (!d.upd_v_loaivp(decimal.Parse(m_id), decimal.Parse(_idnhom), decimal.Parse(m_id), m_id, _loai, m_id, 1, LibUtility.Utility.getComputername, 0, ""))
                            {
                                MessageBox.Show("Error");
                                return;
                            }
                            d.execute_data("update v_loaivp set KHONGDAU='" + _khongdau_loai + "' where id='" + m_id + "'");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhóm");
                            return;
                        }
                    }



                }
            }
            if (dt.Rows.Count > 0) MessageBox.Show("OK!");
        }

        private void btncapnhatloaivp_Click(object sender, EventArgs e)
        {
            DataTable dtTmp, dtTmpLoai;
            string _khongdau_nhom = "", _khongdau_loai = "", m_id = "", _loai = "", _idnhom = "", _nhom = "";
            DataRow dr, drloai;
            decimal dongia = 0;
            decimal id = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = decimal.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            _loai = row["TEN_LOAI"].ToString().Trim();
                            _nhom = row["TEN_NHOM"].ToString().Trim();
                            dtTmpLoai = d.get_data("select id,ten,khongdau from mqhisroot.v_loaivp").Tables[0];                          
                            _khongdau_loai = LibUtility.Utility.Hoten_khongdau(_loai);
                            _khongdau_nhom = LibUtility.Utility.Hoten_khongdau(_nhom);
                            drloai = d.getrowbyid(dtTmpLoai, "khongdau='" + _khongdau_loai + "'");
                            if (drloai != null)
                            {
                                dongia =decimal.Parse(drloai["ID"].ToString());
                            }
                            else
                            {
                                dtTmp = d.get_data("select ma,ten,khongdau from mqhisroot.v_nhomvp").Tables[0];

                                dr = d.getrowbyid(dtTmp, "khongdau='" + _khongdau_nhom + "'");
                                if (dr != null)
                                {
                                    _idnhom = dr["ma"].ToString();
                                    m_id = d.get_id_v_loaivp.ToString();
                                    dongia = decimal.Parse(m_id);
                                    if (!d.upd_v_loaivp(decimal.Parse(m_id), decimal.Parse(_idnhom), decimal.Parse(m_id), m_id, _loai, m_id, 1, LibUtility.Utility.getComputername, 0, ""))
                                    {
                                        MessageBox.Show("Error");
                                        return;
                                    }
                                    d.execute_data("update v_loaivp set KHONGDAU='" + _khongdau_loai + "' where id='" + m_id + "'");
                                }
                            }

                                if (dongia > 0)
                            {
                                
                                
                                sql = "update v_giavp set id_loai="+dongia+" where id=" + id;
                                d.execute_data(sql);
                                
                            }
                        }
                    }
                    catch { id = 0; }

                }
            }
            MessageBox.Show("OK");
        }

        private void button99_Click_1(object sender, EventArgs e)
        {
            string[] arr = { "BHYTTHUOC", "D_HUYBANCT", "D_NGTRUCT", "D_NHAPBBCT", "D_NHAPCT", "D_NHAPCT_TRACK", "D_THEODOI", "D_THEODOIGIA", "D_THUCBUCSTT", "D_THUCXUAT", "D_THUCXUAT2", "D_THUOCBHYTCT", "D_THUOCBHYTNG", "D_TIENTHUOC", "D_TOATHUOCCT", "D_TONKHOCT", "D_TONKHOKEMTHEO", "D_TONKHOTH", "D_TUTRUCCT", "D_TUTRUCKEMTHEO", "D_TUTRUCTH", "D_XTUTRUCCT", "D_XUATCT", "D_XUATCT_DQG", "D_XUATSDCT" };
            long _id = 0, _idmedi = 0;

            foreach (DataRow r in dt.Rows)
            {
                _id = 0;
                _idmedi = 0;
                try
                {
                    _id = long.Parse(r["ID"].ToString());
                    _idmedi = long.Parse(r["idmedi"].ToString());
                }
                catch
                {
                }
                if (_id > 0 && _idmedi > 0)
                {
                    sql = "select mmyy from tables";
                    foreach (DataRow dr in d.get_data(sql).Tables[0].Rows)
                    {
                        foreach (string _tb in arr)
                        {
                            string xxx = user + dr["mmyy"].ToString();
                            sql = "update " + xxx + "." + _tb;
                            sql += " set mabd=" + _idmedi;
                            sql += " where mabd=" + _id;
                            if (!d.execute_data(sql))
                            {
                                LibUtility.Utility.MsgBox(sql);
                            }
                        }
                    }
                }
            }

            LibUtility.Utility.MsgBox("OK");

        }

        private void button100_Click_1(object sender, EventArgs e)
        {
            string _tenhc = "", _mahc="";
            foreach (DataRow row in dt.Rows)
            {
                _tenhc = row["tenhc"].ToString().Trim();
                if (!string.IsNullOrEmpty(_tenhc))
                {
                    _mahc = getMahc(_tenhc, "");
                }
            }
            LibUtility.Utility.MsgBox("Ok");
        }

        private void button102_Click(object sender, EventArgs e)
        {
            long _id;
            string _tenhc = "", _mahc = "";
            foreach (DataRow row in dt.Rows)
            {
                _id = long.Parse(row["ID"].ToString());
                if (_id > 0)
                {
                    _tenhc = row["tenhc"].ToString().Trim();
                    d.upd_dmbd_col(_id, "tenhc", _tenhc);
                    d.upd_dmbd_col(_id, "mahc", getmahc(_tenhc));
                }
            }
            LibUtility.Utility.MsgBox("Ok");
        }
        private string getmahc(string tenhc)
        {
            string str = "";
            string[] arr = tenhc.Split('+');
            foreach (string strtenhc in arr)
            {
                if (strtenhc != "")
                {
                    string kdau = LibUtility.Utility.Hoten_khongdau(strtenhc);
                    if (kdau != "")
                    {
                        sql = "select ma from d_dmhoatchat where khongdau='" + kdau + "'";
                        DataSet dsTmp = d.get_data(sql);
                        if (dsTmp.Tables[0].Rows.Count == 0)
                        {
                            getMahc(tenhc, "");
                            return "";
                        }
                        else
                        {
                            foreach (DataRow r in dsTmp.Tables[0].Rows)
                            {
                                str += r["MA"].ToString() + "+";
                            }
                        }
                    }
                }
            }
            return str;
        }

        private void button103_Click(object sender, EventArgs e)
        {
            string _ten = "";
            foreach (DataRow r in dt.Rows)
            {
                _ten = r["ten"].ToString();
                if (_ten != "")
                {
                    string m_id = d.get_id_xn_khangsinh.ToString();
                    if (!d.upd_xn_khangsinh(decimal.Parse(m_id), decimal.Parse(m_id), r["ma"].ToString().Trim(), r["ma"].ToString().Trim(), _ten, r["ma"].ToString().Trim(), 0))
                    {
                        MessageBox.Show("");
                    }
                }

            }
            MessageBox.Show("OK");
        }

        private void button104_Click(object sender, EventArgs e)
        {
            string _ten = "";
            foreach (DataRow r in dt.Rows)
            {
                _ten = r["ten"].ToString();
                if (_ten != "")
                {
                    string m_id = d.get_id_xn_vitrung.ToString();
                    if (!d.upd_xn_vitrung(decimal.Parse(m_id), decimal.Parse(m_id), r["ma"].ToString().Trim(), _ten, "1","1", r["ma"].ToString().Trim()))
                    {
                        MessageBox.Show("");
                    }
                }

            }
            MessageBox.Show("OK");
        }

        private void button105_Click(object sender, EventArgs e)
        {
            dt.WriteXml("1.xml", XmlWriteMode.WriteSchema);
        }

        private void button106_Click(object sender, EventArgs e)
        {
            TUVANDINHDUONG _tuvandinhduong = new TUVANDINHDUONG();
            decimal bmi = decimal.Parse(txtBMI.Text);
            decimal chieucao = decimal.Parse(txtChieucao.Text);
            int phai = int.Parse(txtPhai.Text);
            string tuoivao = txtTuoivao.Text;
            _tuvandinhduong = Tuvandinhduong(bmi, chieucao, phai, tuoivao);
            if (_tuvandinhduong != null) txtResult.Text = _tuvandinhduong.TINHTRANG + "\r\n" + _tuvandinhduong.TUVAN;
        }
        private TUVANDINHDUONG Tuvandinhduong(decimal bmi,decimal chieucao,int phai,string tuoivao)
        {
            TUVANDINHDUONG _tuvandinhduong = new TUVANDINHDUONG();
            string strTuvandinhduong = string.Empty;
            if (string.IsNullOrEmpty(tuoivao))
            {
                return null;
            }
            string _loaituoi = tuoivao.Substring(0, 1);
            int _tuoi = int.Parse(tuoivao.Substring(1));
            int _loai = 0;//0: >=19     1:6-18          2:0-4
            if (_loaituoi == "0")// tuoi
            {
                if (_tuoi <= 4) _loai = 2;
                else if (_tuoi >= 6 && _tuoi <= 18) _loai = 1;
                else _loai = 0;
            }
            else if (_loaituoi == "1")// thang
            {
                if (_tuoi <= 60) _loai = 2;
                else _loai = 1;
            }
            else if (_loaituoi == "2" || _loaituoi == "3")// ngay + gio
            {
                _loai = 2;
            }
            string _fileBMI = "TVDD-0_4_BMI.xml";
            string _fileHEIGHT = "TVDD-0_4_HEIGHT.xml";
            string _fileTVDD = "TVDD-0_4TVDD.xml";
            string _file = "";

            decimal bmi_sd3 = 0, bmi_sd2 = 0, bmi_Median = 0, bmi_sd_3 = 0, bmi_sd_2 = 0;
            decimal height_sd3 = 0, height_sd2 = 0, height_Median = 0, height_sd_3 = 0, height_sd_2 = 0;
            string _bmi_tvdd = "", _height_tvdd = "";

            if (_loai == 0)
            {
                #region >=19
                _fileTVDD = "TVDD-19-999.xml";
                _file = "..\\..\\..\\xml\\" + _fileTVDD;
                if (!System.IO.File.Exists(_file)) return null;

                _fileTVDD = "TVDD-19-999.xml";
                DataSet dsTvdd = new DataSet();
                dsTvdd.ReadXml(_file);
                foreach (DataRow r in dsTvdd.Tables[0].Rows)
                {
                    if (bmi >= decimal.Parse(r["BMI_TU"].ToString()) && bmi <= decimal.Parse(r["BMI_DEN"].ToString()))
                    {
                        _tuvandinhduong.TINHTRANG = r["TINHTRANG"].ToString().Trim();
                        _tuvandinhduong.TUVAN = r["TUVAN"].ToString().Trim();
                        break;
                    }
                }
                #endregion >=19
            }
            else if (_loai == 1)
            {
                #region 5-18
                DataSet dsBmi = new DataSet();
                DataSet dsHeight = new DataSet();
                DataSet dsTvdd = new DataSet();

                _fileBMI = "TVDD-5-18-BMI.xml";
                _file = "..\\..\\..\\xml\\" + _fileBMI;
                if (!System.IO.File.Exists(_file)) return null;
                dsBmi.ReadXml(_file);

                _fileHEIGHT = "TVDD-5-18-HEIGHT.xml";
                _file = "..\\..\\..\\xml\\" + _fileHEIGHT;
                if (!System.IO.File.Exists(_file)) return null;
                dsHeight.ReadXml(_file);

                _fileTVDD = "TVDD-5-18-TVDD.xml";
                _file = "..\\..\\..\\xml\\" + _fileTVDD;
                if (!System.IO.File.Exists(_file)) return null;
                dsTvdd.ReadXml(_file);

                LibUtility.Utility.RemoveAllSpaceDataset(dsTvdd, "BMI");
                LibUtility.Utility.RemoveAllSpaceDataset(dsTvdd, "CHIEUCAO");
                // bmi
                DataRow drBmi = d.getrowbyid(dsBmi.Tables[0], "TUOI=" + _tuoi);
                if (drBmi == null) return null;
                if (phai == 0)
                {
                    bmi_sd3 = decimal.Parse(drBmi["NAM3SD"].ToString());
                    bmi_sd2 = decimal.Parse(drBmi["NAM2SD"].ToString());
                    bmi_Median = decimal.Parse(drBmi["NAMMedian"].ToString());
                    bmi_sd_3 = decimal.Parse(drBmi["NAM-3SD"].ToString());
                    bmi_sd_2 = decimal.Parse(drBmi["NAM-2SD"].ToString());
                }
                else
                {
                    bmi_sd3 = decimal.Parse(drBmi["NU3SD"].ToString());
                    bmi_sd2 = decimal.Parse(drBmi["NU2SD"].ToString());
                    bmi_Median = decimal.Parse(drBmi["NUMedian"].ToString());
                    bmi_sd_3 = decimal.Parse(drBmi["NU-3SD"].ToString());
                    bmi_sd_2 = decimal.Parse(drBmi["NU-2SD"].ToString());
                }
                if (bmi < bmi_sd_3) _bmi_tvdd = "BMI<-3SD";
                else if (bmi >= bmi_sd_3 && bmi < bmi_sd_2) _bmi_tvdd = "-3SD≤ BMI<-2SD";
                else if (bmi >= bmi_sd_2 && bmi <= bmi_sd2) _bmi_tvdd = "-2SD≤BMI≤2SD";
                else if (bmi > bmi_sd3) _bmi_tvdd = "BMI>3SD";
                else if (bmi > bmi_sd2) _bmi_tvdd = "BMI>2SD";

                // height
                DataRow drHeight = d.getrowbyid(dsHeight.Tables[0], "TUOI=" + _tuoi);
                if (drHeight == null) return null;
                if (phai == 0)
                {
                    height_sd3 = decimal.Parse(drHeight["NAM3SD"].ToString());
                    height_sd2 = decimal.Parse(drHeight["NAM2SD"].ToString());
                    height_Median = decimal.Parse(drHeight["NAMMedian"].ToString());
                    height_sd_3 = decimal.Parse(drHeight["NAM-3SD"].ToString());
                    height_sd_2 = decimal.Parse(drHeight["NAM-2SD"].ToString());
                }
                else
                {
                    height_sd3 = decimal.Parse(drHeight["NU3SD"].ToString());
                    height_sd2 = decimal.Parse(drHeight["NU2SD"].ToString());
                    height_Median = decimal.Parse(drHeight["NUMedian"].ToString());
                    height_sd_3 = decimal.Parse(drHeight["NU-3SD"].ToString());
                    height_sd_2 = decimal.Parse(drHeight["NU-2SD"].ToString());
                }
                if (chieucao < height_sd_3) _height_tvdd = "CC<-3SD";
                else if (chieucao >= height_sd_3 && chieucao < height_sd_2) _height_tvdd = "-3SD≤CC<-2SD";
                else if (chieucao >= height_sd_2 && chieucao <= height_sd2) _height_tvdd = "-2SD≤CC≤2SD";
                else if (chieucao > height_sd2) _height_tvdd = "CC>2SD";
                if (string.IsNullOrEmpty(_bmi_tvdd) || string.IsNullOrEmpty(_height_tvdd)) return null;

                _bmi_tvdd = LibUtility.Utility.RemoveAllSpace(_bmi_tvdd);
                _height_tvdd = LibUtility.Utility.RemoveAllSpace(_height_tvdd);

                DataRow drTuvan = d.getrowbyid(dsTvdd.Tables[0], "BMI='" + _bmi_tvdd + "' and CHIEUCAO='" + _height_tvdd + "'");
                if (drTuvan != null)
                {
                    _tuvandinhduong.TINHTRANG = drTuvan["TINHTRANG"].ToString().Trim();
                    _tuvandinhduong.TUVAN = drTuvan["TUVAN"].ToString().Trim();
                }
                else return null;

                #endregion 5-18
            }
            else
            {
                #region 0-4
                DataSet dsBmi = new DataSet();
                DataSet dsHeight = new DataSet();
                DataSet dsTvdd = new DataSet();

                _fileBMI = "TVDD-0-4-BMI.xml";
                _file = "..\\..\\..\\xml\\" + _fileBMI;
                if (!System.IO.File.Exists(_file)) return null;
                dsBmi.ReadXml(_file);

                _fileHEIGHT = "TVDD-0-4-HEIGHT.xml";
                _file = "..\\..\\..\\xml\\" + _fileHEIGHT;
                if (!System.IO.File.Exists(_file)) return null;
                dsHeight.ReadXml(_file);

                _fileTVDD = "TVDD-0-4-TVDD.xml";
                _file = "..\\..\\..\\xml\\" + _fileTVDD;
                if (!System.IO.File.Exists(_file)) return null;
                dsTvdd.ReadXml(_file);

                LibUtility.Utility.RemoveAllSpaceDataset(dsTvdd,"BMI");
                LibUtility.Utility.RemoveAllSpaceDataset(dsTvdd, "CHIEUCAO");
                // bmi
                DataRow drBmi = d.getrowbyid(dsBmi.Tables[0], "Months=" + _tuoi);
                if (drBmi == null) return null;
                if (phai == 0)
                {
                    bmi_sd3 = decimal.Parse(drBmi["NAM3SD"].ToString());
                    bmi_sd2 = decimal.Parse(drBmi["NAM2SD"].ToString());
                    bmi_Median = decimal.Parse(drBmi["NAMMedian"].ToString());
                    bmi_sd_3 = decimal.Parse(drBmi["NAM-3SD"].ToString());
                    bmi_sd_2 = decimal.Parse(drBmi["NAM-2SD"].ToString());
                }
                else
                {
                    bmi_sd3 = decimal.Parse(drBmi["NU3SD"].ToString());
                    bmi_sd2 = decimal.Parse(drBmi["NU2SD"].ToString());
                    bmi_Median = decimal.Parse(drBmi["NUMedian"].ToString());
                    bmi_sd_3 = decimal.Parse(drBmi["NU-3SD"].ToString());
                    bmi_sd_2 = decimal.Parse(drBmi["NU-2SD"].ToString());
                }
                if (bmi < bmi_sd_3) _bmi_tvdd = "BMI<-3SD";
                else if (bmi >= bmi_sd_3 && bmi < bmi_sd_2) _bmi_tvdd = "-3SD≤BMI<-2SD";
                else if (bmi >= bmi_sd_2 && bmi <= bmi_sd2) _bmi_tvdd = "-2SD≤BMI≤2SD";
                else if (bmi > bmi_sd3) _bmi_tvdd = "BMI>3SD";
                else if (bmi > bmi_sd2) _bmi_tvdd = "BMI>2SD";

                // height
                DataRow drHeight = d.getrowbyid(dsHeight.Tables[0], "Months=" + _tuoi);
                if (drHeight == null) return null;
                if (phai == 0)
                {
                    height_sd3 = decimal.Parse(drHeight["NAM3SD"].ToString());
                    height_sd2 = decimal.Parse(drHeight["NAM2SD"].ToString());
                    height_Median = decimal.Parse(drHeight["NAMMedian"].ToString());
                    height_sd_3 = decimal.Parse(drHeight["NAM-3SD"].ToString());
                    height_sd_2 = decimal.Parse(drHeight["NAM-2SD"].ToString());
                }
                else
                {
                    height_sd3 = decimal.Parse(drHeight["NU3SD"].ToString());
                    height_sd2 = decimal.Parse(drHeight["NU2SD"].ToString());
                    height_Median = decimal.Parse(drHeight["NUMedian"].ToString());
                    height_sd_3 = decimal.Parse(drHeight["NU-3SD"].ToString());
                    height_sd_2 = decimal.Parse(drHeight["NU-2SD"].ToString());
                }
                if (chieucao < height_sd_3) _height_tvdd = "CC<-3SD";
                else if (chieucao >= height_sd_3 && chieucao < height_sd_2) _height_tvdd = "-3SD≤CC<-2SD";
                else if (chieucao >= height_sd_2 && chieucao <= height_sd2) _height_tvdd = "-2SD≤CC≤2SD";
                else if (chieucao > height_sd2) _height_tvdd = "CC>2SD";
                if (string.IsNullOrEmpty(_bmi_tvdd) || string.IsNullOrEmpty(_height_tvdd)) return null;


                _bmi_tvdd = LibUtility.Utility.RemoveAllSpace(_bmi_tvdd);
                _height_tvdd = LibUtility.Utility.RemoveAllSpace(_height_tvdd);

                string exp = "BMI='" + _bmi_tvdd + "'";
                exp += " AND CHIEUCAO='" + _height_tvdd + "'";
                DataRow drTuvan = d.getrowbyid(dsTvdd.Tables[0], exp);
                if (drTuvan != null)
                {
                    _tuvandinhduong.TINHTRANG = drTuvan["TINHTRANG"].ToString().Trim();
                    _tuvandinhduong.TUVAN = drTuvan["TUVAN"].ToString().Trim();
                }
                else return null;

                #endregion 0-4
            }



            return _tuvandinhduong;
        }
        private OleDbConnection ReturnConnection(string fileName)
        {
            return new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + fileName + ";" +
                " Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;\"");
        }

        private void button107_Click(object sender, EventArgs e)
        {
            string qrcode = "0204287018|486fc3a06e67205875c3a26e2048e1baa56e|30/07/1981|1|-|79 - 034|01/02/2021|-|20/02/2021|79020204287018|-|4| 01/01/2015|15e89ac07ee8517f-7102|4|5175e1baad6e2031322c205468c3a06e68207068e1bb912048e1bb93204368c3ad204d696e68|$";

            LibUtility.Utility.QRCode(qrcode);
            
        }

        private void button108_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            
            long _id = 0;
            string _ma = "", _ten = "", _khongdau = "";

            dtTmp = d.get_data("select * from d_dmnx where nhom=" + i_nhom + "").Tables[0];
            foreach (DataRow r in dtTmp.Rows)
            {
                _ten = r["TEN"].ToString().Trim();
                if (_ten != "")
                {
                   
                    _khongdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    
                   
                        d.execute_data("update d_dmnx set khongdau='" + _khongdau + "' where id=" + _id);
                   

                }
            }
            MessageBox.Show("OK");
        }

        private void button109_Click(object sender, EventArgs e)
        {
            DataTable dtTmpLoai;           
            DataRow  drloai;
            long _id_loai = 0,_l_id=0;
            
            foreach (DataRow r in dt.Rows)
            {
                _id_loai =long.Parse(r["ID_LOAI"].ToString().Trim());
                _l_id = long.Parse(r["ID"].ToString().Trim());
                if (_id_loai>0&&_l_id>0)
                {
                    dtTmpLoai = d.get_data("select id,ten,khongdau from mqhisroot.v_loaivp").Tables[0];
                    drloai = d.getrowbyid(dtTmpLoai, "id=" + _id_loai + "");
                    if (drloai == null)
                    {

                        MessageBox.Show("Không tìm thấy loại viện phí của mã viện phí : ", r["ID"].ToString());
                        return;
                    }
                    else
                    {
                        
                        sql = "update v_giavp set id_loai=" + _id_loai + ",ngayud=sysdate where id=" + _l_id;
                        d.execute_data(sql);
                    }



                }
            }
            if (dt.Rows.Count > 0) MessageBox.Show("OK!");
        }

        private void button112_Click(object sender, EventArgs e)
        {
            oracle.TransactionBegin();
            decimal s_tencot = 0;
            decimal id = 0;
            string cotgia = cbocotgiavp_so.Text;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = decimal.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                s_tencot =decimal.Parse(row[cotgia].ToString());
                            }
                            catch (Exception ex)
                            {
                                oracle.TransactionRollback();
                                MessageBox.Show(ex.ToString());

                            }
                            if (s_tencot.ToString() != "")
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".v_giavp where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_v_giavp_luu(decimal.Parse(r["id"].ToString()), decimal.Parse(r["id_loai"].ToString()), decimal.Parse(r["stt"].ToString()),
                        r["ma"].ToString(), r["ten"].ToString(), r["dvt"].ToString(), decimal.Parse(r["gia_th"].ToString()),
                        decimal.Parse(r["gia_bh"].ToString()), decimal.Parse(r["gia_dv"].ToString()), decimal.Parse(r["gia_nn"].ToString()),
                        decimal.Parse(r["gia_cs"].ToString()), decimal.Parse(r["vattu_th"].ToString()), decimal.Parse(r["vattu_bh"].ToString()),
                        decimal.Parse(r["vattu_dv"].ToString()), decimal.Parse(r["vattu_nn"].ToString()), decimal.Parse(r["vattu_cs"].ToString()),
                        decimal.Parse(r["bhyt"].ToString()), decimal.Parse(r["loaibn"].ToString()), decimal.Parse(r["theobs"].ToString()),
                        decimal.Parse(r["thuong"].ToString()), decimal.Parse(r["trongoi"].ToString()), decimal.Parse(r["loaitrongoi"].ToString()),
                        decimal.Parse(r["chenhlech"].ToString()), decimal.Parse(r["ndm"].ToString()), r["locthe"].ToString(),
                        decimal.Parse(r["readonly"].ToString()), decimal.Parse(r["userid"].ToString()), decimal.Parse(r["tylekhuyenmai"].ToString()),
                        decimal.Parse(r["gia_ksk"].ToString()), decimal.Parse(r["vattu_ksk"].ToString()), decimal.Parse(r["hide"].ToString()),
                        decimal.Parse(r["kythuat"].ToString()), 0, 0))
                                    {
                                        oracle.TransactionRollback();
                                        MessageBox.Show("error");

                                    }

                                }
                                d.upd_v_giavp(id, s_tencot, cotgia);
                                //  sql = "update v_giavp set " + cotgia + "='" + s_tencot + "',ngayud=sysdate where id=" + id;
                                //d.execute_data(sql);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oracle.TransactionRollback();
                        MessageBox.Show(ex.ToString());
                        id = 0;

                    }

                }
            }
            oracle.TransactionCommit();
            MessageBox.Show("OK");
        }

        private void button113_Click(object sender, EventArgs e)
        {
            DataTable dtTmp, dtTmpLoai;
            long _id_loai = 0, _l_id = 0;
            string _khongdau_nhom = "", _khongdau_loai = "", m_id = "", _loai = "", _idnhom = "", _nhom = "";
            DataRow dr, drloai;
            if (!check_column_excel_giavp())
            {
                dt = new DataTable();
            }
            foreach (DataRow r in dt.Rows)
            {
                _l_id = long.Parse(r["ID"].ToString().Trim());
                _loai = r["LOAI"].ToString().Trim();
                _nhom = r["NHOM"].ToString().Trim();
                if (_loai != "")
                {
                    dtTmpLoai = d.get_data("select id,ten,khongdau from mqhisroot.v_loaivp").Tables[0];

                    _khongdau_nhom = LibUtility.Utility.Hoten_khongdau(_nhom);
                    _khongdau_loai = LibUtility.Utility.Hoten_khongdau(_loai);
                    drloai = d.getrowbyid(dtTmpLoai, "khongdau='" + _khongdau_loai + "'");
                    if (drloai == null)
                    {
                        dtTmp = d.get_data("select ma,ten,khongdau from mqhisroot.v_nhomvp").Tables[0];

                        dr = d.getrowbyid(dtTmp, "khongdau='" + _khongdau_nhom + "'");
                        if (dr != null)
                        {
                            _idnhom = dr["ma"].ToString();
                            m_id = d.get_id_v_loaivp.ToString();
                            if (!d.upd_v_loaivp(decimal.Parse(m_id), decimal.Parse(_idnhom), decimal.Parse(m_id), m_id, _loai, m_id, 1, LibUtility.Utility.getComputername, 0, ""))
                            {
                                MessageBox.Show("Error");
                                return;
                            }
                            d.execute_data("update v_loaivp set KHONGDAU='" + _khongdau_loai + "' where id='" + m_id + "'");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhóm");
                            return;
                        }
                    }
                    else
                    {
                        _id_loai = long.Parse(drloai["ID"].ToString().Trim());
                        sql = "update v_giavp set id_loai=" + _id_loai + ",ngayud=sysdate where id=" + _l_id;
                        d.execute_data(sql);
                    }



                }
            }
            if (dt.Rows.Count > 0) MessageBox.Show("OK!");
        }

        private void btndmbdthongtu_Click(object sender, EventArgs e)
        {
            oracle.TransactionBegin();
            string s_tencot = "";
            long id = 0;
            string cotgia = cbdmbdthongtu.Text;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = long.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                s_tencot = row[cotgia].ToString();
                            }
                            catch (Exception ex)
                            {
                                oracle.TransactionRollback();
                                MessageBox.Show(ex.ToString());

                            }
                            if (s_tencot != "")
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".d_dmbdthongtu where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_dmbd_luu(id))
                                    {
                                        oracle.TransactionRollback();
                                        MessageBox.Show("error");

                                    }
                                    else
                                    {
                                        sql = "update " + user + ".d_dmbdthongtu set " + cotgia + "='" + s_tencot + "',ngayud=sysdate where id=" + id;
                                        d.execute_data(sql);
                                    }

                                }


                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oracle.TransactionRollback();
                        MessageBox.Show(ex.ToString());
                        id = 0;

                    }

                }
            }
            oracle.TransactionCommit();
            MessageBox.Show("OK");
        }

        private void btndmbdthongtuso_Click(object sender, EventArgs e)
        {
            oracle.TransactionBegin();
            string s_tencot = "";
            long id = 0;
            string cotgia = cbdmbdthongtuso.Text;
            foreach (DataRow row in dt.Rows)
            {
                if (row["ID"].ToString().Trim() != "")
                {
                    try
                    {
                        id = long.Parse(row["ID"].ToString().Trim());
                        if (id > 0)
                        {
                            try
                            {
                                s_tencot = row[cotgia].ToString();
                            }
                            catch (Exception ex)
                            {
                                oracle.TransactionRollback();
                                MessageBox.Show(ex.ToString());

                            }
                            if (s_tencot != "")
                            {
                                foreach (DataRow r in d.get_data("select * from " + user + ".d_dmbdthongtu where id=" + id).Tables[0].Rows)
                                {
                                    if (!d.upd_dmbd_luu(id))
                                    {
                                        oracle.TransactionRollback();
                                        MessageBox.Show("error");

                                    }
                                    else
                                    {
                                        sql = "update " + user + ".d_dmbdthongtu set " + cotgia + "=" + s_tencot + ",ngayud=sysdate where id=" + id;
                                        d.execute_data(sql);
                                    }

                                }


                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oracle.TransactionRollback();
                        MessageBox.Show(ex.ToString());
                        id = 0;

                    }

                }
            }
            oracle.TransactionCommit();
            MessageBox.Show("OK");
        }

        private void button114_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string _tenhc = "";
            
            foreach (DataRow r in dt.Select("true", "MAATC"))
            {

                _tenhc = r["tenhc"].ToString().Trim();
                if (_tenhc != "")
                {
                 d.execute_data("update d_dmbd set maatc='" + r["maatc"].ToString().Trim() + "' where tenhc='" + r["tenhc"].ToString().Trim()+"'");
                 d.execute_data("update d_dmbd set maatc='" + r["maatc"].ToString().Trim() + "' where tenhc||' '||hamluong='" + r["tenhc"].ToString().Trim() + "'");
                }
            }
            MessageBox.Show("OK");
        }

        private void button115_Click(object sender, EventArgs e)
        {
            int i_sole_giaban = 0;
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            string nhom_tyle2 = get_nhom_tyle2(i_nhomkho);
            long _id = 0;
            int _mabd = 0, _stt = 0;
            decimal giamua = 0;
            decimal giaban = 0;
            //sql = "select id,mabd,giamua from mqsoftbmt0222.d_theodoi ";
            //dt = d.get_data(sql).Tables[0];
            //foreach (DataRow r in dt.Rows)
            //{
            //    _id = long.Parse(r["id"].ToString());
            //    _mabd = int.Parse(r["mabd"].ToString());
            //    giamua = decimal.Parse(r["giamua"].ToString());
            //    decimal tl = d.get_tyleban(_mabd, nhom_tyle2, i_nhomkho, giamua);

            //    giaban = LibUtility.Utility.Round(giamua + giamua * tl / 100, i_sole_giaban);
            //    sql = "update mqsoftbmt0222.d_theodoi set giaban=" + giaban + " where id=" + _id + " and mabd=" + _mabd;
            //    d.execute_data(sql);
            //}
            //sql = "select id,stt,mabd,giamua from mqsoftbmt0222.d_nhapct ";
            //dt = d.get_data(sql).Tables[0];
            //foreach (DataRow r in dt.Rows)
            //{
            //    _id = long.Parse(r["id"].ToString());
            //    _mabd = int.Parse(r["mabd"].ToString());
            //    _stt = int.Parse(r["stt"].ToString());
            //    giamua = decimal.Parse(r["giamua"].ToString());
            //    decimal tl = d.get_tyleban(_mabd, nhom_tyle2, i_nhomkho, giamua);

            //    giaban = LibUtility.Utility.Round(giamua + giamua * tl / 100, i_sole_giaban);
            //    sql = "update mqsoftbmt0222.d_nhapct set giaban=" + giaban + " where id=" + _id + " and mabd=" + _mabd+" and stt="+_stt;
            //    d.execute_data(sql);
            //}
            sql = "select id,id as mabd from d_dmbd where id in(select mabd from mqsoftbmt0222.d_theodoi where giaban>0)";
            dt = d.get_data(sql).Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                _id = long.Parse(r["mabd"].ToString());
                _mabd = int.Parse(r["mabd"].ToString());
                sql = "select max(giaban)  as giaban from from mqsoftbmt0222.d_theodoi where mabd=" + _mabd + " ";
                foreach (DataRow r1 in   d.get_data(sql).Tables[0].Rows)
                {
                    giaban = decimal.Parse(r["giaban"].ToString());
                    break;
                }
                sql = "update d_dmbd set giaban=" + giaban + " where id=" + _id;
                d.execute_data(sql);
            }
            MessageBox.Show("OK");
        }

        private void button116_Click(object sender, EventArgs e)
        {
    
            foreach (DataRow r in dt.Rows)
            {
                if (r["MA"].ToString() != "")
                {

                    sql = "update dmnoicapbhyt  set matuyen=" + r["TUYEN"].ToString() + " WHERE mabv='" + r["MA"].ToString().PadLeft(5, '0') + "'";
                        d.execute_data(sql);
                   
                }

            }
            MessageBox.Show("OK");
        }


        private void button117_Click(object sender, EventArgs e)
        {
            string _right = "";
            sql = "select id,right_ from d_dlogin ";
            dt = d.get_data(sql).Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                _id = long.Parse(r["id"].ToString());
                _right = r["right_"].ToString();
                string[] arr = _right.Split('+');
                _right = string.Empty;
                foreach (string s in arr)
                {
                    _right += s.PadLeft(4, '0') + "+";
                }
                sql = "update d_dlogin set right_='" + _right + "' where id=" + _id;
                d.execute_data(sql);
            }
            MessageBox.Show("OK");
        }


        //private void button117_Click(object sender, EventArgs e)
        //{
        //    //frmChuyenPhieuMisa frm = new frmChuyenPhieuMisa(int.Parse("0"), 5);
        //    //frm.Show();
        //}

        private void button119_Click(object sender, EventArgs e)
        {
            frmLienThongDonThuoc frm = new frmLienThongDonThuoc();
            frm.Show();
        }

        private void button120_Click(object sender, EventArgs e)
        {
            string _mmyy = txtthangton.Text;
            string _mmyytruoc = LibUtility.Utility.Mmyy_truoc(_mmyy);
            string _mmyytruoc2 = LibUtility.Utility.Mmyy_truoc(_mmyytruoc);


            sql = "  select a.id,b.idttrv,b.mmyy,a.idtamung from " + user + _mmyy + ".v_ttrvtamung a inner join ";
              sql+= "    (";
            sql += "    select id, idttrv,'"+ _mmyy + "' as mmyy from " + user + _mmyy + ".v_tamung";
            sql += "     union all";
            sql += "     select id, idttrv,'" + _mmyytruoc + "' as mmyy from " + user + _mmyytruoc + ".v_tamung";
            if (d.bMmyy(_mmyytruoc2))
            {
                sql += "     union all";
                sql += "     select id, idttrv,'" + _mmyytruoc2 + "' as mmyy from " + user + _mmyytruoc2 + ".v_tamung";
            }
            sql += "     ) b on a.idtamung = b.id";
            sql += " inner join " + user + _mmyy + ".v_ttrvct c on a.id=c.id";
            sql += " where a.id<> b.idttrv";
            sql += " and c.tra=0 ";

            sql += " group by a.id,b.idttrv,b.mmyy,a.idtamung";

            foreach (DataRow r in d.get_data(sql).Tables[0].Rows)
            {
                sql = "update " + user + r["mmyy"].ToString() + ".v_tamung set idttrv=" + r["id"].ToString() + " where id=" + r["idtamung"].ToString();
                d.execute_data(sql);
            }
            MessageBox.Show("OK");
        }

        private void btnupd_losx_hd_Click(object sender, EventArgs e)
        {
            int i_sole_giaban = 0;
            i_nhomkho = int.Parse(nhomkhoimp.ComboBox.SelectedValue.ToString());
            string nhom_tyle2 = get_nhom_tyle2(i_nhomkho);

            int _makho = 0;
            try
            {
                _makho = int.Parse(makhoimp.ComboBox.SelectedValue.ToString());
            }
            catch { _makho = 0; }
            if (_makho == 0)
            {
                LibUtility.Utility.MsgBox("Chọn kho");
                return;
            }
            if (!checkdm()) return;

            long id = 0;
            if (txtthangton.Text.Trim() == "" || txtthangton.Text.Trim().Length != 4)
            {
                LibUtility.Utility.MsgBox("Tháng tồn không hợp lệ!");
                return;
            }
            mmyy = txtthangton.Text.Trim();
            int _manguon = 0, _mabd = 0;
            _manguon = int.Parse(manguonimp.ComboBox.SelectedValue.ToString());
            decimal dongia = 0, ton = 0;
            string _handung = "";
            DataRow dr;
            DataTable dtbd = d.get_data("select id,madv from d_dmbd where nhom=" + i_nhomkho).Tables[0];

            foreach (DataRow r in dt.Rows)
            {
                if (r["mabd"].ToString().Trim() != "")
                {
                    try
                    {
                        _mabd = int.Parse(r["MABD"].ToString());
                    }
                    catch { _mabd = 0; }
                    if (_mabd > 0)
                    {
                        try
                        {
                            ton = decimal.Parse(r["TON"].ToString());
                        }
                        catch { ton = 0; }
                        if (ton > 0)
                        {
                            
                            
                            _handung = r["handung"].ToString().Trim();
                            if (_handung.Length >= 10)
                            {
                                _handung = _handung.Split('/')[1] + _handung.Split('/')[0] + _handung.Split('/')[2].Substring(2, 2);
                            }
                            else if (_handung.Length == 6)
                            {
                                _handung = _handung.Trim();
                            }
                            else if (_handung.Length == 7)
                            {
                                _handung = _handung.Split('/')[0] + _handung.Split('/')[1].Substring(2, 2);
                            }
                            else
                            if (_handung.Length == 4 && _handung.IndexOf("/") != -1)
                            {
                                _handung = "00" + _handung.Split('/')[0] + _handung.Split('/')[1];
                            }
                            else
                            if (_handung.Length == 4 && _handung.IndexOf("/") == -1)
                            {
                                _handung = "00" + _handung;
                            }
                            else _handung = "";

                            // update lại handung

                            
                            sql = "select a.*,b.tondau ";
                            sql += " from " + user + mmyy + ".d_theodoi a  inner join " + user + mmyy + ".d_tonkhoct b on a.id = b.stt ";
                            sql += " where b.makho ="+ _makho + " and a.handung is null and a.mabd="+ _mabd + " ";
                            DataTable dtTmp = d.get_data(sql).Tables[0];
                            foreach (DataRow row in dtTmp.Rows)
                            {
                                
                               
                                   
                                    if (_handung != "")
                                    {
                                        sql = "update " + user + mmyy + ".d_theodoi set handung='" + _handung + "' where id= " + row["id"].ToString();
                                        d.execute_data(sql);
                                    }
                                }

                            }
                            // ket thuc update handung



                        }
                    }
                }
            
            MessageBox.Show("OK");

            
            
        }

        private void button121_Click(object sender, EventArgs e)
        {
            var pass = "TBDtp@2021";
            pass = LibUtility.Utility.encode_MD5(pass);
        }

        private void button122_Click(object sender, EventArgs e)
        {
            SignParam signParam = new SignParam();
            signParam.IDSIGN = Guid.NewGuid().ToString();
            signParam.IdSignPdf = Guid.NewGuid().ToString();
            signParam.mabssign = "0666";
            var file = "D:\\ksk_LaiXe.XML";
            signParam.SourceFile = file;
            string filer = d.SignEFYXmlGiaypheplaixe(signParam, "");
        }

        //private void button123_Click(object sender, EventArgs e)
        //{
        //    get_gksk(LibConfig.ConfigManager.stendangnhapVAS, LibConfig.ConfigManager.spasdangnhapVAS, "DƯƠNG QUỐC BẢO", "09/07/1988", 0, "01002/GKSKLX/79039/23", "Tổ 5, ấp Phú Thuận, Xã Phú Mỹ Hưng, Huyện Củ Chi, TP. Hồ Chí Minh Phú Mỹ Hưng H. Củ Chi Tp. HCM", "", "", "", "211872582", "30/04/2017", "Công an bình định", "79039", "Bệnh viện Huyện Củ Chi", "123", "0", "0", "16/03/2023", "Bs.Trần Chánh Xuân", "A0-1", "A2", "16/03/2023", "ssss", "", "1");
        //}
        public class root
        {
            public string UUID { get; set; }
            public string CREATEDDATE { get; set; }
            public string USERCREATE { get; set; }
            public string ACTION { get; set; }
            public string SO { get; set; }
            public string HOTEN { get; set; }
            public string GIOITINHVAL { get; set; }
            public string NGAYSINH { get; set; }
            public string DIACHITHUONGTRU { get; set; }
            public string MATINH_THUONGTRU { get; set; }
            public string MAHUYEN_THUONGTRU { get; set; }
            public string MAXA_THUONGTRU { get; set; }
            public string SOCMND_PASSPORT { get; set; }
            public string NGAYTHANGNAMCAPCMD { get; set; }
            public string NOICAP { get; set; }
            public string ECITIZENCODE { get; set; }
            public string MOBILE { get; set; }
            public string EMAIL { get; set; }
            public string IDBENHVIEN { get; set; }
            public string BENHVIEN { get; set; }
            public string NONGDOCON { get; set; }
            public string DVINONGDOCON { get; set; }
            public string MATUY { get; set; }
            public string NGAYKETLUAN { get; set; }
            public string BACSYKETLUAN { get; set; }
            public string KETLUAN { get; set; }
            public string HANGBANGLAI { get; set; }
            public string NGAYKHAMLAI { get; set; }
            public string LYDO { get; set; }
            public string TINHTRANGBENH { get; set; }
            public string STATE { get; set; }
            //public string SIGNDATA { get; set; }

        }
        //
        public class guiksk
        {
            public string UUID { get; set; }
            public string CREATEDDATE { get; set; }
            public string USERCREATE { get; set; }
            public string ACTION { get; set; }
            public string SO { get; set; }
            public string HOTEN { get; set; }
            public string GIOITINHVAL { get; set; }
            public string NGAYSINH { get; set; }
            public string DIACHITHUONGTRU { get; set; }
            public string MATINH_THUONGTRU { get; set; }
            public string MAHUYEN_THUONGTRU { get; set; }
            public string MAXA_THUONGTRU { get; set; }
            public string SOCMND_PASSPORT { get; set; }
            public string NGAYTHANGNAMCAPCMND { get; set; }
            public string NOICAP { get; set; }
            public string ECITIZENCODE { get; set; }
            public string MOBILE { get; set; }
            public string EMAIL { get; set; }
            public string IDBENHVIEN { get; set; }
            public string BENHVIEN { get; set; }
            public string NONGDOCON { get; set; }
            public string DVINONGDOCON { get; set; }
            public string MATUY { get; set; }
            public string NGAYKETLUAN { get; set; }
            public string BACSYKETLUAN { get; set; }
            public string KETLUAN { get; set; }
            public string HANGBANGLAI { get; set; }
            public string NGAYKHAMLAI { get; set; }
            public string LYDO { get; set; }
            public string TINHTRANGBENH { get; set; }
            public string STATE { get; set; }
            public string SIGNDATA { get; set; }
        }
        //

        public class nhanKSK
        {
            public string MSG_TEXT { get; set; }
            public string MSG_STATE { get; set; }
            public string IDBENHVIEN { get; set; }
            public string SO { get; set; }
            public string UUID { get; set; }
            public string BENHVIEN { get; set; }

        }
        //public bool get_gksk(string _username, string _password, string _hoten, string _ngaysinh, int _gioitinh, string _SO, string _DIACHITHUONGTRU, string _MATINH_THUONGTRU, string _MAHUYEN_THUONGTRU, string _MAXA_THUONGTRU, string _SOCMND_PASSPORT, string _NGAYTHANGNAMCAPCMND, string _NOICAP, string _macskbbd, string _BENHVIEN, string _NONGDOCON, string _DVINONGDOCON, string _MATUY, string _NGAYKETLUAN, string _BACSYKETLUAN, string _KETLUAN, string _HANGBANGLAI, string _NGAYKHAMLAI, string _LYDO, string _TINHTRANGBENH, string _STATE)
        //{
        //    DataSet dsxmldaky = new DataSet();
        //    var xmlDoc = new System.Xml.XmlDocument();
        //    if (_hoten == "")
        //    {
        //        LibUtility.Utility.MsgBox("Họ tên không hợp lệ!");
        //        return false;
        //    }
        //    if (_ngaysinh == "")
        //    {
        //        LibUtility.Utility.MsgBox("Ngày tháng năm sinh không được để trống!");
        //        return false;
        //    }
        //    root lichsuKSK = new root();
        //    lichsuKSK.UUID = "";
        //    lichsuKSK.CREATEDDATE = "";
        //    lichsuKSK.USERCREATE = "";
        //    lichsuKSK.ACTION = "";
        //    lichsuKSK.SO = _SO;
        //    lichsuKSK.HOTEN = _hoten;
        //    lichsuKSK.GIOITINHVAL = _gioitinh.ToString();
        //    lichsuKSK.NGAYSINH = _ngaysinh;
        //    lichsuKSK.DIACHITHUONGTRU = _DIACHITHUONGTRU;
        //    lichsuKSK.MATINH_THUONGTRU = _MATINH_THUONGTRU;
        //    lichsuKSK.MAHUYEN_THUONGTRU = _MAHUYEN_THUONGTRU;
        //    lichsuKSK.MAXA_THUONGTRU = _MAXA_THUONGTRU;
        //    lichsuKSK.SOCMND_PASSPORT = _SOCMND_PASSPORT;
        //    lichsuKSK.NGAYTHANGNAMCAPCMD = _NGAYTHANGNAMCAPCMND;
        //    //lichsuKSK.NGAYTHANGNAMCAPCMND = _NGAYTHANGNAMCAPCMND;
        //    lichsuKSK.NOICAP = _NOICAP;
        //    lichsuKSK.IDBENHVIEN = _macskbbd;
        //    lichsuKSK.BENHVIEN = _BENHVIEN;
        //    lichsuKSK.NONGDOCON = _NONGDOCON;
        //    lichsuKSK.DVINONGDOCON = _DVINONGDOCON;
        //    lichsuKSK.MATUY = _MATUY;
        //    lichsuKSK.NGAYKETLUAN = _NGAYKETLUAN;
        //    lichsuKSK.BACSYKETLUAN = _BACSYKETLUAN;
        //    lichsuKSK.KETLUAN = _KETLUAN;
        //    lichsuKSK.HANGBANGLAI = _HANGBANGLAI;
        //    lichsuKSK.NGAYKHAMLAI = _NGAYKHAMLAI;
        //    lichsuKSK.LYDO = _LYDO;
        //    lichsuKSK.TINHTRANGBENH = _TINHTRANGBENH;
        //    lichsuKSK.STATE = _STATE;

        //    var serializer = new XmlSerializer(typeof(root));
        //    if (!System.IO.Directory.Exists("..\\..\\..\\xml")) System.IO.Directory.CreateDirectory("..\\..\\..\\xml");
        //    if (File.Exists("..\\..\\..\\xml\\lichsuKSK.xml")) File.Delete("..\\..\\..\\xml\\lichsuKSK.xml");
        //    if (File.Exists("..\\..\\..\\xml\\lichsuKSK_DaKy.xml")) File.Delete("..\\..\\..\\xml\\lichsuKSK_DaKy.xml");
        //    using (var writer = new StreamWriter("..\\..\\..\\xml\\lichsuKSK.xml")) //write file xml
        //    {
        //        serializer.Serialize(writer, lichsuKSK);
        //    }
        //    // ky so file xml
        //    //KyXML_USB("..\\..\\..\\xml\\lichsuKSK.xml", "..\\..\\..\\xml\\lichsuKSK_DaKy.xml");
        //    // read file xml da ky
        //    xmlDoc.Load("..\\..\\..\\xml\\lichsuKSK_DaKy.xml"); //sample xml content
        //    var xmlDocString = xmlDoc.InnerXml;
        //    guiksk guiKSK = new guiksk();
        //    guiKSK.UUID = "";
        //    guiKSK.CREATEDDATE = "";
        //    guiKSK.USERCREATE = "";
        //    guiKSK.ACTION = "";
        //    guiKSK.SO = _SO;
        //    guiKSK.HOTEN = _hoten;
        //    guiKSK.GIOITINHVAL = _gioitinh.ToString();
        //    guiKSK.NGAYSINH = _ngaysinh;
        //    guiKSK.DIACHITHUONGTRU = _DIACHITHUONGTRU;
        //    guiKSK.MATINH_THUONGTRU = _MATINH_THUONGTRU;
        //    guiKSK.MAHUYEN_THUONGTRU = _MAHUYEN_THUONGTRU;
        //    guiKSK.MAXA_THUONGTRU = _MAXA_THUONGTRU;
        //    guiKSK.SOCMND_PASSPORT = _SOCMND_PASSPORT;
        //    //guiKSK.NGAYTHANGNAMCAPCMD = _NGAYTHANGNAMCAPCMND;
        //    guiKSK.NGAYTHANGNAMCAPCMND = _NGAYTHANGNAMCAPCMND;
        //    guiKSK.NOICAP = _NOICAP;
        //    guiKSK.IDBENHVIEN = _macskbbd;
        //    guiKSK.BENHVIEN = _BENHVIEN;
        //    guiKSK.NONGDOCON = _NONGDOCON;
        //    guiKSK.DVINONGDOCON = _DVINONGDOCON;
        //    guiKSK.MATUY = _MATUY;
        //    guiKSK.NGAYKETLUAN = _NGAYKETLUAN;
        //    guiKSK.BACSYKETLUAN = _BACSYKETLUAN;
        //    guiKSK.KETLUAN = _KETLUAN;
        //    guiKSK.HANGBANGLAI = _HANGBANGLAI;
        //    guiKSK.NGAYKHAMLAI = _NGAYKHAMLAI;
        //    guiKSK.LYDO = _LYDO;
        //    guiKSK.TINHTRANGBENH = _TINHTRANGBENH;
        //    guiKSK.STATE = _STATE;
        //    guiKSK.SIGNDATA = LibUtility.Utility.Base64Encode(xmlDocString);// copy nội dung file xml đã ký vào thẻ xml
        //    //
        //    d.s_mahoso = "";

        //    string data = JsonConvert.SerializeObject(guiKSK);
        //   Uri address = new Uri(LibConfig.ConfigManager.UrlGiamdinh + "/api/hssk/gksk");
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
        //    request.Method = "post";
        //    request.ContentType = "application/json";
        //    request.Headers.Add("Username", _username);
        //    request.Headers.Add("Password", d.GetPass(_password));
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
        //    nhanKSK tok = JsonConvert.DeserializeObject<nhanKSK>(response);
        //    string _maketqua = tok.MSG_STATE;
        //    if (_maketqua == "1")
        //    {

        //    }
        //    if (_maketqua == "0")
        //    {
        //        LibUtility.Utility.MsgBox(tok.MSG_TEXT);
        //        return false;
        //    }

        //    return _maketqua == "1";
        //}

        private void button124_Click(object sender, EventArgs e)
        {
            var MA = string.Empty;
            var MA_QUOCTICH = string.Empty;

            foreach (DataRow r in dt.Rows)
            {
                MA = r["MA"].ToString().Trim();
                MA_QUOCTICH = r["MA_QUOCTICH"].ToString().Trim();
                sql = "update dmquocgia set MA_QUOCTICH='" + MA_QUOCTICH + "' where ma='" + MA + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void dantoc_Click(object sender, EventArgs e)
        {
            var MADANTOC = string.Empty;
            var MA_DANTOC = string.Empty;


            foreach (DataRow r in dt.Rows)
            {
                MADANTOC = r["MADANTOC"].ToString().Trim();
                MA_DANTOC = r["MA_DANTOC"].ToString().Trim();
                sql = "update btddt set MA_DANTOC='" + MA_DANTOC + "' where MADANTOC='" + MADANTOC + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void nghenghiep_Click(object sender, EventArgs e)
        {
            var mann = string.Empty;
            var MA_NGHE_NGHIEP = string.Empty;


            foreach (DataRow r in dt.Rows)
            {
                mann = r["mann"].ToString().Trim();
                MA_NGHE_NGHIEP = r["MA_NGHE_NGHIEP"].ToString().Trim();
                sql = "update btdnn_bv set MA_NGHE_NGHIEP='" + MA_NGHE_NGHIEP + "' where mann='" + mann.PadLeft(2,'0') + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void icd9chuong_Click(object sender, EventArgs e)
        {
            var CHUONG = string.Empty;
            var IDCHUONG = string.Empty;



            foreach (DataRow r in dt.Rows)
            {
                CHUONG = r["CHUONG"].ToString().Trim();
                IDCHUONG = r["IDCHUONG"].ToString().Trim();
                d.upd_icd9chuong(int.Parse(IDCHUONG), CHUONG);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void icd9nhom_Click(object sender, EventArgs e)
        {
            var NHOM = string.Empty;
            var IDNHOM = string.Empty;



            foreach (DataRow r in dt.Rows)
            {
                NHOM = r["NHOM"].ToString().Trim();
                IDNHOM = r["IDNHOM"].ToString().Trim();
                d.upd_icd9nhom(int.Parse(IDNHOM), NHOM);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void icd9_Click(object sender, EventArgs e)
        {
            var IDNHOM = string.Empty;
            var IDCHUONG = string.Empty;


            foreach (DataRow r in dt.Rows)
            {
                IDNHOM = r["IDNHOM"].ToString().Trim();
                IDCHUONG = r["IDCHUONG"].ToString().Trim();
                d.upd_icd9(r["MAICD9"].ToString().Trim(), r["ICD9"].ToString().Trim(), int.Parse(IDNHOM), int.Parse(IDCHUONG));
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void tinh_Click(object sender, EventArgs e)
        {
            var MATT = string.Empty;
            var MATINH_CU_TRU = string.Empty;


            foreach (DataRow r in dt.Rows)
            {
                MATT = r["MATT"].ToString().Trim();
                MATINH_CU_TRU = r["MATINH_CU_TRU"].ToString().Trim();
                sql = "update BTDTT set MATT_YK='" + MATINH_CU_TRU + "' where matt='" + MATT.PadLeft(3, '0') + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void quan_Click(object sender, EventArgs e)
        {
            var MAQU = string.Empty;
            var MAHUYEN_CU_TRU = string.Empty;


            foreach (DataRow r in dt.Rows)
            {
                MAQU = r["MAQU"].ToString().Trim();
                MAHUYEN_CU_TRU = r["MAHUYEN_CU_TRU"].ToString().Trim();
                sql = "update btdquan set MAQU_YK='" + MAHUYEN_CU_TRU + "' where MAQU='" + MAQU.PadLeft(5, '0') + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void button125_Click(object sender, EventArgs e)
        {
            var MAPHUONGXA = string.Empty;
            var  MAXA_CU_TRU = string.Empty;

        
            foreach (DataRow r in dt.Rows)
            {
                MAPHUONGXA = r["MAPHUONGXA"].ToString().Trim();
                MAXA_CU_TRU = r["MAXA_CU_TRU"].ToString().Trim();
                sql = "update btdpxa  set MAPHUONGXA_YK='" + MAXA_CU_TRU + "' where MAPHUONGXA='" + MAPHUONGXA.PadLeft(7, '0') + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void button126_Click(object sender, EventArgs e)
        {
            load_all();
        }
        private void load_all()
        {
            MQLIB.Splasher.ShowScreenWaiting(this, true);
            sql = "select a.maphuongxa,a.MAPHUONGXA_YK as MAXA_CU_TRU , b.maqu,b.MAQU_YK as MAHUYEN_CU_TRU,c.matt,c.matt_yk as MATINH_CU_TRU ";
            sql += " ,a.tenpxa||' '||b.tenquan || ' ' || c.tentt as ten,null as khongdau ";
            sql += " from btdpxa a inner join btdquan b on a.maqu = b.maqu inner join btdtt c on b.matt = c.matt";
            DataSet dsXa = new DataSet();
            dsXa = d.get_data(sql);
            foreach (DataRow r in dsXa.Tables[0].Rows)
            {
                r["ten"] = edit_string(r["ten"].ToString().Trim());
                r["ten"] = r["ten"].ToString().Replace("'","").Trim();
                r["khongdau"] = LibUtility.Utility.khongdau(r["ten"].ToString());
            }
            dgXa.DataSource = dsXa.Tables[0];

            DataTable dtCoppy = dt.Copy();
            dtCoppy.Columns.Add("TEN");
            dtCoppy.Columns.Add("KHONGDAU");
            foreach (DataRow r in dtCoppy.Rows)
            {
                var ten = r["XA"].ToString().Trim() + " " + r["TENQUAN"].ToString().Trim() + " " + r["TENTT"].ToString().Trim();
                r["ten"] = edit_string(ten);
                r["ten"] = r["ten"].ToString().Replace("'", "").Trim();
                r["khongdau"] = LibUtility.Utility.khongdau(r["ten"].ToString());


            }
            dgXaCucdanso.DataSource = dtCoppy;
            MQLIB.Splasher.ShowScreenWaiting(this, false);
        }
        private string edit_string(string value)
        {
            string temp = value;
            temp = temp.Replace("  ", " ");
            temp = temp.Replace("Tỉnh ", "");
            temp = temp.Replace(" Quận 2 (Thành phố Thủ Đức)", " thủ đức");
            temp = temp.Replace(" Quận Thủ Đức  (Thành phố Thủ Đức)", " thủ đức");
            temp = temp.Replace(" Quận 9 (Thành phố Thủ Đức)", " thủ đức");
            temp = temp.Replace(" Thủ Đức", " thủ đức");
            temp = temp.Replace(" Thủ Đức (Thành phố Thủ Đức)", " thủ đức");
            temp = temp.Replace("Thành phố ", "");
            temp = temp.Replace("Huyện ", "");
            temp = temp.Replace("Thị xã ", "");
            temp = temp.Replace("Quận ", "");
            temp = temp.Replace("Thị trấn ", "");

            temp = temp.Replace("Phường 0", "");
            temp = temp.Replace("Phường ", "");

            temp = temp.Replace("Xã ", "");
            return temp.ToLower();
        }

        private void button127_Click(object sender, EventArgs e)
        {
            MQLIB.Splasher.ShowScreenWaiting(this, true);
            DataTable dtXa =(DataTable) dgXa.DataSource;
            dtXa.AcceptChanges();

            DataTable dtXaCucdanso = (DataTable)dgXaCucdanso.DataSource;
            dtXaCucdanso.AcceptChanges();
            var khongdau = string.Empty;
            int i = 1;
            foreach (DataRow r in dtXa.Rows)
            {
                khongdau = r["khongdau"].ToString().Trim();
                DataRow dr = d.getrowbyid(dtXaCucdanso, "khongdau='" + khongdau + "'");
                if (dr != null)
                {
                    r["MAXA_CU_TRU"] = dr["MAPHUONGXA"];
                    r["MAHUYEN_CU_TRU"] = dr["MAQUAN"];
                    r["MATINH_CU_TRU"] = dr["MATT"];

                    sql = "update BTDTT set MATT_YK='" + dr["MATT"].ToString().Trim() + "' where matt='" + r["matt"].ToString() + "'";
                    d.execute_data(sql);

                    sql = "update btdquan set MAQU_YK='" + dr["MAQUAN"].ToString().Trim() + "' where MAQU='" + r["maqu"].ToString().Trim() + "'";
                    d.execute_data(sql);


                    sql = "update btdpxa  set MAPHUONGXA_YK='" + dr["MAPHUONGXA"].ToString().Trim() + "' where MAPHUONGXA='" + r["MAPHUONGXA"].ToString() + "'";
                    d.execute_data(sql);

                   

                }
                i++;
                butMapAll.Refresh();
                butMapAll.Text = i.ToString() + " / " + dtXa.Rows.Count.ToString();
                butMapAll.Refresh();
            }
            butMapAll.Text = "butMapAll";
            dtXa.AcceptChanges();
            MQLIB.Splasher.ShowScreenWaiting(this, false);
            load_all();
        }

        private void butMap1_Click(object sender, EventArgs e)
        {
            try
            {
                gridView4.FocusedColumn = gridView4.VisibleColumns[0];
                gridView4.FocusedColumn = gridView4.VisibleColumns[1];
                gridView4.FocusedColumn = gridView4.VisibleColumns[0];
                gridView4.FocusedRowHandle = 0;
                gridView4.Focus();
                gridView4.ShowEditor();
            }
            catch { }
            try
            {
                gridView5.FocusedColumn = gridView5.VisibleColumns[0];
                gridView5.FocusedColumn = gridView5.VisibleColumns[1];
                gridView5.FocusedColumn = gridView5.VisibleColumns[0];
                gridView5.FocusedRowHandle = 0;
                gridView5.Focus();
                gridView5.ShowEditor();
            }
            catch { }

            System.Data.DataRow rowsel = gridView4.GetDataRow(gridView4.FocusedRowHandle);
            if (rowsel != null)
            {
                System.Data.DataRow row = gridView5.GetDataRow(gridView5.FocusedRowHandle);
                if (rowsel != null)
                {
                    sql = "update BTDTT set MATT_YK='" + row["MATT"].ToString().Trim() + "' where matt='" + rowsel["matt"].ToString() + "'";
                    d.execute_data(sql);

                    sql = "update btdquan set MAQU_YK='" + row["MAQUAN"].ToString().Trim() + "' where MAQU='" + rowsel["maqu"].ToString().Trim() + "'";
                    d.execute_data(sql);


                    sql = "update btdpxa  set MAPHUONGXA_YK='" + row["MAPHUONGXA"].ToString().Trim() + "' where MAPHUONGXA='" + rowsel["MAPHUONGXA"].ToString() + "'";
                    d.execute_data(sql);

                    rowsel["MAXA_CU_TRU"] = row["MAPHUONGXA"];
                    rowsel["MAHUYEN_CU_TRU"] = row["MAQUAN"];
                    rowsel["MATINH_CU_TRU"] = row["MATT"];
                }
            }
        }

        private void button127_Click_1(object sender, EventArgs e)
        {
            var ma = string.Empty;
            var MABHXH = string.Empty;
           foreach (DataRow r in dt.Rows)
            {
                ma = r["ma"].ToString().Trim();
                MABHXH = r["MABHXH"].ToString().Trim();
                sql = "update dmbs  set MABHXH='" + MABHXH + "' where ma='" + ma.PadLeft(4, '0') + "'";
                d.execute_data(sql);
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private void button128_Click(object sender, EventArgs e)
        {
            var ID = string.Empty;
            var TTHAU_QD130 = string.Empty;
            var MA_PP_CHEBIEN = string.Empty;
            var MA_CSKCB_THUOC = string.Empty;
            var BAOCHE = string.Empty;
                foreach (DataRow r in dt.Rows)
            {
                ID = r["id"].ToString().Trim();
                TTHAU_QD130 = r["TTHAU_QD130"].ToString().Trim();
                MA_PP_CHEBIEN = r["MA_PP_CHEBIEN"].ToString().Trim();
                MA_CSKCB_THUOC = r["MA_CSKCB_THUOC"].ToString().Trim();
                BAOCHE = r["BAOCHE"].ToString().Trim();
                d.upd_dmbd_thongtu(long.Parse(ID.ToString()), TTHAU_QD130, "TT_THAU_130");
                d.upd_dmbd_thongtu(long.Parse(ID.ToString()), MA_PP_CHEBIEN, "MA_PP_CHEBIEN");
                d.upd_dmbd_thongtu(long.Parse(ID.ToString()), MA_CSKCB_THUOC, "MA_CSKCB_THUOC");
                d.upd_dmbd_thongtu(long.Parse(ID.ToString()), BAOCHE, "BAOCHE");
            }
            LibUtility.Utility.MsgBox("Xong");
        }

        private string[] LoadSchemaFromFile(string fileName)
        {
            string[] SheetNames = null;
            OleDbConnection conn = this.ReturnConnection(fileName);
            try
            {
                conn.Open();

                DataTable SchemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null });
                if (SchemaTable.Rows.Count > 0)
                {
                    SheetNames = new string[SchemaTable.Rows.Count];
                    int i = 0;
                    foreach (DataRow TmpRow in SchemaTable.Rows)
                    {
                        SheetNames[i] = TmpRow["TABLE_NAME"].ToString();
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                

            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return SheetNames;
        }

   

        private void Form1_Load(object sender, EventArgs e)
        {
            user = d.user;
            txtthangton.Text = d.s_curmmyy;
            schema.Text = user;
            cboNhomkho.DisplayMember = "TEN";
            cboNhomkho.ValueMember = "ID";
            cboNhomkho.DataSource = d.get_data("select id,ten from d_dmnhomkho order by id").Tables[0];
            //string pass = d.get_MD5("admin");
            DataSet ds = new DataSet();
            ds.ReadXml("..\\..\\..\\xml\\m_field_gia.xml");
            filedgia.DisplayMember = "MA";
            filedgia.ValueMember = "MA";
            filedgia.DataSource = ds.Tables[0];
            filedgia.SelectedIndex = 0;

            DataSet ds1 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'V_GIAVP' and DATA_TYPE<>'NUMBER' and DATA_TYPE<>'DATE'  ";
            ds1 = d.get_data_text(sql);
            cbtenfileld.DisplayMember = "MA";
            cbtenfileld.ValueMember = "MA";
            cbtenfileld.DataSource = ds1.Tables[0];
            cbtenfileld.SelectedIndex = -1;
            //cbtenfileld

            DataSet ds2 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'V_GIAVP' and DATA_TYPE='NUMBER' and DATA_TYPE<>'DATE'  ";
            ds2 = d.get_data_text(sql);
            cbocotgiavp_so.DisplayMember = "MA";
            cbocotgiavp_so.ValueMember = "MA";
            cbocotgiavp_so.DataSource = ds2.Tables[0];
            cbocotgiavp_so.SelectedIndex = -1;
            //cbtenfileld

            DataSet ds12 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'D_DMBD' and DATA_TYPE<>'NUMBER' and DATA_TYPE<>'DATE'  ";
            ds12 = d.get_data_text(sql);
            cbdanhmucduoc.DisplayMember = "MA";
            cbdanhmucduoc.ValueMember = "MA";
            cbdanhmucduoc.DataSource = ds12.Tables[0];
            cbdanhmucduoc.SelectedIndex = -1;

            DataSet ds13 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'D_DMBDTHONGTU' and DATA_TYPE<>'NUMBER' and DATA_TYPE<>'DATE'  ";
            ds13 = d.get_data_text(sql);
            cbdmbdthongtu.DisplayMember = "MA";
            cbdmbdthongtu.ValueMember = "MA";
            cbdmbdthongtu.DataSource = ds13.Tables[0];
            cbdmbdthongtu.SelectedIndex = -1;

            DataSet ds14 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'D_DMBDTHONGTU' and DATA_TYPE='NUMBER'   ";
            ds14 = d.get_data_text(sql);
            cbdmbdthongtuso.DisplayMember = "MA";
            cbdmbdthongtuso.ValueMember = "MA";
            cbdmbdthongtuso.DataSource = ds14.Tables[0];
            cbdmbdthongtuso.SelectedIndex = -1;

            nhomkhoimp.ComboBox.DisplayMember = "TEN";
            nhomkhoimp.ComboBox.ValueMember = "ID";
            nhomkhoimp.ComboBox.DataSource = d.get_data("select id,ten from d_dmnhomkho").Tables[0];

            makhoimp.ComboBox.DisplayMember = "TEN";
            makhoimp.ComboBox.ValueMember = "ID";

            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;

            colGiavp.DisplayMember = "ID";
            colGiavp.ValueMember = "ID";
            colGiavp.DataSource = d.get_data("select COLUMN_name as ID from USER_TAB_COLUMNS  where table_name='V_GIAVP' order by COLUMN_name").Tables[0];
        }

        private bool check_column_excel_thongtinhanhchinh()
        {
            ArrayList arr = new ArrayList();
            arr.Add("mabn");
            arr.Add("hoten");
            arr.Add("diachi");
            arr.Add("dienthoai");
            arr.Add("ngaysinh");
            arr.Add("namsinh");
            arr.Add("gioitinh");
            arr.Add("xaphuong");
            arr.Add("huyenquan");
            arr.Add("tinhthanh");
            arr.Add("mann");
            arr.Add("tinhtranghonnhan");
            arr.Add("hotennguoinha");
            arr.Add("quanhe");
            arr.Add("dienthoainguoinha");
            arr.Add("diachinguoinha");
            arr.Add("madantoc");
            arr.Add("mangoaikieu");
            arr.Add("thonpho");
            foreach (string str in arr)
            {
                if (!LibUtility.Utility.IsAColumnExist(dt, str))
                {
                    MessageBox.Show("Thiếu cột " + str);
                    return false;
                }
            }

            return true;
        }
        private bool check_column_excel_giavp()
        {
            ArrayList arr = new ArrayList();
            arr.Add("nhom");
            arr.Add("loai");
            arr.Add("ten");
            arr.Add("stt");

            arr.Add("MATUONGDUONG");
            arr.Add("MATT50");
            arr.Add("TENTT50");
            arr.Add("MATT37");
            arr.Add("TENTT37");
            arr.Add("GIA_BH");
            arr.Add("GIA_TH");
            arr.Add("GIA_DV");
            arr.Add("GIA_NN");
            arr.Add("GIA_CS");
            arr.Add("GIA_KSK");
            arr.Add("MA_GIA");
            arr.Add("QUYET_DINH");
            arr.Add("CONG_BO");
            arr.Add("MADUNGCHUNG");
            arr.Add("DVT");
            arr.Add("TENTT43");
            arr.Add("MATT43");
            arr.Add("MAGIUONG");
            arr.Add("MABH");
            arr.Add("CHENHLECH");



            foreach (string str in arr)
            {
                if (!LibUtility.Utility.IsAColumnExist(dt, str))
                {
                    MessageBox.Show("Thiếu cột " + str);
                    return false;
                }
            }

            return true;
        }
        private bool check_column_excel_duoc()
        {
            ArrayList arr = new ArrayList();

            arr.Add("loai");
            arr.Add("ten");
            arr.Add("stt");
            arr.Add("tenhc");
            arr.Add("maduongdung");
            arr.Add("duongdung");
            arr.Add("hamluong");
            arr.Add("sodk");
            arr.Add("donggoi");
            arr.Add("dvt");
            arr.Add("giathau");
            arr.Add("slthau");
            arr.Add("hang");
            arr.Add("nuoc");
            arr.Add("nhacc");
            arr.Add("quyetdinh");
            arr.Add("congbo");
            arr.Add("ma_bv");
            arr.Add("loaithuoc");
            arr.Add("loaithau");
            arr.Add("nhomthau");
            arr.Add("ttthau");
            arr.Add("goithau");
            arr.Add("nhom");
            arr.Add("madungchung");
            arr.Add("cachdung");
            arr.Add("SLDONGGOI");
            arr.Add("maatc");
            arr.Add("namthau");
            arr.Add("sohd");
            arr.Add("ngayhd");
            arr.Add("DVSD");
            arr.Add("sttqd");
            arr.Add("MATHAU");
            arr.Add("dongia");
            arr.Add("giabh");
            arr.Add("giaban");
            arr.Add("giamua");
            arr.Add("MANHOMVTYT");
            arr.Add("TENNHOMVTYT");
            arr.Add("NHOMIN");
            arr.Add("TENS");// ten bh
            arr.Add("dang_bao_che");
            arr.Add("hide");
            arr.Add("NGAYHIEULUCTHAU");
            arr.Add("NGAYHETHIEULUCTHAU");
            arr.Add("DONVITHAU");
            arr.Add("DINH_MUC");
            arr.Add("nhomcongtacduoc");
            arr.Add("TTHAU_QD130");
            arr.Add("MA_PP_CHEBIEN");
            arr.Add("MA_CSKCB_THUOC");
            arr.Add("BAOCHE");





            foreach (string str in arr)
            {
                if (!LibUtility.Utility.IsAColumnExist(dt, str))
                {
                    MessageBox.Show("Thiếu cột " + str);
                    return false;
                }
            }

            return true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            dtTmp = d.get_data("select * from d_nhombo where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nhombo"))
            {
                _ten = r["nhombo"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {

                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        try
                        {
                            _id = long.Parse(d.get_data("select max(id) from " + user + ".d_nhombo").Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        catch { _id = 1; }
                        d.upd_danhmuc("d_nhombo", _id, _ten, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                        d.execute_data("update d_nhombo set khongdau='" + kdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_nhombo where nhom=" + i_nhom).Tables[0];
                    }

                }
            }
            MessageBox.Show("OK");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            dtTmp = d.get_data("select * from d_dmnhomkt where nhom=" + i_nhom).Tables[0];
            foreach (DataRow r in dt.Select("true", "nhomketoan"))
            {
                _ten = r["nhomketoan"].ToString().Trim();
                _ten = _ten.Replace("'", "");
                if (_ten == "" || _ten == "NULL") _ten = "Không xác định";
                if (_ten != "")
                {

                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);

                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr == null)
                    {
                        try
                        {
                            _id = long.Parse(d.get_data("select max(id) from " + user + ".d_dmnhomkt").Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        catch { _id = 1; }
                        d.upd_danhmuc("d_dmnhomkt", _id, _ten, i_nhom, LibUtility.Utility.get_stt(dtTmp));
                        d.execute_data("update d_dmnhomkt set khongdau='" + kdau + "' where id=" + _id);
                        dtTmp = d.get_data("select * from d_dmnhomkt where nhom=" + i_nhom).Tables[0];
                    }

                }
            }
            MessageBox.Show("OK");
        }

        private string ConvertHexStrToUnicode(string hexString)
        {
            int length = hexString.Length;
            byte[] bytes = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return Encoding.UTF8.GetString(bytes);
        }



    }
    public class TUVANDINHDUONG
    {
        public string  TINHTRANG { get; set; }
        public string TUVAN { get; set; }
    }
    public class CatShareDetails
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public int SortNumber { get; set; }
        public string Name { get; set; }
        public int MasterId { get; set; }
        public long ParentId { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public int Number3 { get; set; }
        public int Number4 { get; set; }
        public int Number5 { get; set; }
        public string String1 { get; set; }
        public string String2 { get; set; }
        public string String3 { get; set; }
        public string String4 { get; set; }
        public string String5 { get; set; }
    }

    public class CatHospitals
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public int SortNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public Boolean IsHospital { get; set; }
        public long HospitalTypeId { get; set; }
        public long GeographicRegionId { get; set; }
        public long HospitalRouteId { get; set; }
        public long CityId { get; set; }
        public long HospitalGradeId { get; set; }
        public string Extension { get; set; }
        public string Location { get; set; }

    }

    public class CatICD10s
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public int SortNumber { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public long Form15Id { get; set; }
        public long GroupId { get; set; }
        public Boolean IsChronic { get; set; }
        public string InsuranceCode { get; set; }
        public string EquivalentCode { get; set; }
        public string EquivalentName { get; set; }

     

    }
    public class test
    {


    }
}