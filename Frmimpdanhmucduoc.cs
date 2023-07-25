using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using LibHIS;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using LibUtility;

namespace ImportXML
{
    public partial class Frmimpdanhmucduoc : Form
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

        public Frmimpdanhmucduoc()
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
                        d.upd_dmhoatchat(ma, s3.Trim(), i_nhom, "", 0, "", "");
                        d.execute_data("update d_dmhoatchat set khongdau='" + kdau + "',hamluong='" + hamluong + "' where ma='" + ma + "'");
                    }
                    else mahc += r["ma"].ToString() + "+";
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
                                d.execute_data("update v_giavp set  hide=0 where id=" + _id);
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
                        }


                    }

                }
                LibUtility.Utility.MsgBox("OK");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void nhacc_Click(object sender, EventArgs e)
        {
            if(!bExist_Col(d.user, "d_dmnx","KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
            if (!bExist_Col(d.user, "d_dmhang", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
            if (!bExist_Col(d.user, "d_dmnuoc", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            string kdau = "";
            dtTmp = d.get_data("select * from d_dmhang where nhom=" + i_nhom).Tables[0];
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
            if (!bExist_Col(d.user, "d_dmnhom", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
            if (!bExist_Col(d.user, "d_dmloai", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
            



            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            if (!check_column_excel_duoc()) return;

            string _mahc = "", _mabd = "", _nhom = "1", _loai = "1", _nuoc = "", _hang = "", _dang = "", _tenhc = "", _khongdau = "", _nhomin = "", _tt31 = "", _sodk = "", _bhyttra = "", _nhacckd = "", _STTQD = "", _MABYT = "", _hamluong = "", _DVSD = "", _nhacc = "", STTQD = "", madungchung = "", TTTHAU = "", ma_bv, nhombo = "", sotk = "", _nhomcongtacduoc = "";
            string MADUONGDUNG = "", DUONGDUNG = "", SODK = "", DONGGOI = "", QUYETDINH = "", CONGBO = "", LOAITHUOC = "", LOAITHAU = "", NHOMTHAU = "", MATHAU = "", maatc = "", namthau = "", SOHD = "", ngayhd = "", cacdung = "", nhomdieutri, nhomin;
            string MANHOMVTYT, TENNHOMVTYT;
            int _idnhom = 1, _idloai = 1, _idhang = 0, _idnuoc = 1, _idnhomin = 2, _idnhacc = 0, _nhombo = 0, _sotk = 0, _nhomdt = 0, _phuluc3=0;
            int d_stt = 0,_dmuc=0;
            decimal _tyle = 0, _dongia = 0, SLDONGGOI = 0, GIATHAU = 0, gia_bh = 0, slthau = 0, giaban = 0, giamua, giathau;
            DataTable dtNhom, dtLoai, dtHang, dtBd, dtNuoc, dtnhombo, dtsotk, dtNhomdieutri, dtNhomin, dtNhomctduoc;
            dtNhom = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
            dtLoai = d.get_data("select * from d_dmloai where nhom=" + i_nhom).Tables[0];
            dtBd = d.get_data("select * from d_dmbd where nhom=" + i_nhom).Tables[0];
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
                        LibUtility.Utility.MsgBox("Loại");
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

                    _nhacc = r["nhacc"].ToString().Trim();
                    if (_nhacc == "") _nhacc = "Không xác định";
                    _nhacc = LibUtility.Utility.Hoten_khongdau(_nhacc);
                    dr = d.getrowbyid(dtnhacc, "khongdau='" + _nhacc + "'");
                    if (dr != null) _idnhacc = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhà cc");
                        return;
                    }

                    nhombo = r["nhombo"].ToString().Trim();
                    if (nhombo == "") nhombo = "Không xác định";
                    nhombo = LibUtility.Utility.Hoten_khongdau(nhombo);
                    dr = d.getrowbyid(dtnhombo, "khongdau='" + nhombo + "'");
                    if (dr != null) _nhombo = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhom bo");
                        return;
                    }

                    sotk = r["nhomketoan"].ToString().Trim();
                    if (sotk == "") sotk = "Không xác định";
                    sotk = LibUtility.Utility.Hoten_khongdau(sotk);
                    dr = d.getrowbyid(dtsotk, "khongdau='" + sotk + "'");
                    if (dr != null) _sotk = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhom ke toan");
                        return;
                    }



                    _nuoc = r["nuoc"].ToString().Trim();
                    if (_nuoc == "") _nuoc = "Không xác định";
                    _nuoc = LibUtility.Utility.Hoten_khongdau(_nuoc);
                    dr = d.getrowbyid(dtNuoc, "khongdau='" + _nuoc + "'");
                    if (dr != null) _idnuoc = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nước");
                        _idnuoc = 96;
                        return;
                    }

                    nhomdieutri = r["nhomdieutri"].ToString().Trim();
                    if (nhomdieutri == "") nhomdieutri = "Không xác định";
                    nhomdieutri = LibUtility.Utility.Hoten_khongdau(nhomdieutri);
                    dr = d.getrowbyid(dtNhomdieutri, "khongdau='" + nhomdieutri + "'");
                    if (dr != null) _nhomdt = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhóm điều trị");
                        return;
                    }

                    nhomin = r["nhomin"].ToString().Trim();
                    if (nhomin == "") nhomin = "Không xác định";
                    nhomin = LibUtility.Utility.Hoten_khongdau(nhomin);
                    dr = d.getrowbyid(dtNhomin, "khongdau='" + nhomin + "'");
                    if (dr != null) _idnhomin = int.Parse(dr["id"].ToString());
                    else
                    {
                        LibUtility.Utility.MsgBox("Nhóm in");
                        return;
                    }
                    _nhomcongtacduoc = r["nhomcongtacduoc"].ToString().Trim();
                    if (_nhomcongtacduoc == "") _nhomcongtacduoc = "Không xác định";
                    _nhomcongtacduoc = LibUtility.Utility.Hoten_khongdau(_nhomcongtacduoc);
                    dr = d.getrowbyid(dtNhomctduoc, "khongdau='" + _nhomcongtacduoc + "'");
                    if (dr != null) _phuluc3 = int.Parse(dr["id"].ToString());
                    else
                    {
                        _phuluc3 = 0;
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

                    LOAITHUOC = r["LOAITHUOC"].ToString().ToUpper();
                    if (LOAITHUOC == "") LOAITHUOC = "0";
                    else
                    {
                        foreach (KeyValuePair<int, string> item in dictionary)
                        {
                            if(item.Value== r["LOAITHUOC"].ToString().ToUpper().Trim())
                            {
                                LOAITHUOC = item.Key.ToString();
                                break;
                            }
                        }
                    }
                    LOAITHAU = r["LOAITHAU"].ToString().ToUpper();
                    if (LOAITHAU == "") LOAITHAU = "0";
                    else
                    {
                        foreach (KeyValuePair<int, string> item1 in dictionary_thau)
                        {
                            if (item1.Value.ToUpper() == r["LOAITHAU"].ToString().ToUpper().Trim())
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
            if (!bExist_Col(d.user, "d_nhomin", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
                OleDbConnection con = this.ReturnConnection(fileName);
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
                d.upd_tuongtackhacatc(long.Parse(r["id"].ToString()), decimal.Parse(r["stt"].ToString()), long.Parse(r["makhac"].ToString()), int.Parse(r["mucdo"].ToString()), r["ghichu"].ToString(), 0, int.Parse(r["cam"].ToString()));

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
                        decimal.Parse(r["kythuat"].ToString()), 0))
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

       

        private void button19_Click(object sender, EventArgs e)
        {
            if (!bExist_Col(d.user, "d_dmnhomdt", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
                    else
                    {
                        d.execute_data("update d_dmnhomdt set khongdau='" + kdau + "' where id=" + _id);
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
            chiphikhambenh _chiphikhambenh = new chiphikhambenh();
            string ngayvao = "06/09/2019";
            string ngayra = "06/09/2019";
            string mabn = "17000002";
            long mavaovien = 190906170957896987;
            long maql = 190906170957896987;
            int madoituong = -1;
            string makp = "040";
            //d.VcbGenQrCode(0, mabn, ngayvao, ngayra, mavaovien, maql, madoituong, makp);
            PAYResponse _PAYResponse = new PAYResponse();
            _PAYResponse.UserID = "190911000000040";
            _PAYResponse.Amount = 104000;
            d.VCBPAY(_PAYResponse, LibHIS.AccessData.AppID_VCB_BILLING);
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

        

        //private OleDbConnection ReturnConnection(string fileName)
        //{
        //    return new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;" +
        //        "Data Source=" + fileName + ";" +
        //        " Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 8.0;\"");
        //}

        private OleDbConnection ReturnConnection(string fileName)
        {
            return new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=" + fileName + ";" +
                " Jet OLEDB:Engine Type=5;Extended Properties=\"Excel 12.0;\"");
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
            string[] arr = new string[] { "d_dmnx", "d_dmhang", "d_dmnuoc", "d_dmnhomdt", "d_dmnhomkt", "d_nhombo", "d_nhomin", "d_dmloai","v_nhombaocao", "bieu_07_23" ,"d_dmnhom"};

            string khongdau = "";

            foreach (string s in arr)
            {
                table_name = s;
                sql = "select * from " + table_name + "";
               
                string asql = "";
                foreach (DataRow r in d.get_data(sql).Tables[0].Rows)
                {
                    if (table_name == "bieu_07_23")
                    {
                        asql = " update " + table_name + " set khongdau='" + khongdau + "' where id23=" + r["id"].ToString();
                    }
                    else asql = " update " + table_name + " set khongdau='" + khongdau + "' where id=" + r["id"].ToString();

                    khongdau = r["ten"].ToString().Trim();
                    khongdau = LibUtility.Utility.Hoten_khongdau(khongdau);
                    khongdau = khongdau.Replace("'", "");
                    
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
        

        

        

        
        private bool isKey(ArrayList key,string col)
        {
            foreach (string keyname in key)
            {
                if (col.ToUpper() == keyname.ToUpper()) return true;
            }
            return false;
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

                                }
                                //  d.upd_d_dmbd(id, s_tencot, cotgia,"");
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

        private void button2_Click(object sender, EventArgs e)
        {
            oracle.TransactionBegin();
            string s_tencot = "";
            long id = 0;
            string cotgia = cbdanhmucduoc_kieuso.Text;
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

                                }
                                  d.upd_d_dmbd(id,long.Parse(s_tencot), cotgia,"");
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

        private void btncapnhatnhomdieutri_Click(object sender, EventArgs e)
        {
            if (!bExist_Col(d.user, "d_dmnhom", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
            int i_nhom = int.Parse(cboNhomkho.SelectedValue.ToString());
            foreach (DataRow r in dt.Select("true", "nhom"))
            {

            _id = long.Parse(r["id"].ToString());           
            _ten = r["nhom"].ToString().Trim();
                if (_ten == "") _ten = "Không xác định";
                if (_ten != "")
                {
                    kdau = LibUtility.Utility.Hoten_khongdau(_ten);
                    dtTmp = d.get_data("select * from d_dmnhom where nhom=" + i_nhom).Tables[0];
                    dr = d.getrowbyid(dtTmp, "khongdau='" + kdau + "'");
                    if (dr != null)
                    {
                        
                        d.execute_data("update d_dmbd set manhom=" + long.Parse(dr["id"].ToString()) + " where id=" + _id);
                    }
                    else
                    {
                        MessageBox.Show("Không tồn tại giá trị TENNHOM !");
                        return;
                    }
                }


            }
            MessageBox.Show("OK");
        }

        private void button4_Click(object sender, EventArgs e)
        {

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
            
            cboNhomkho.DisplayMember = "TEN";
            cboNhomkho.ValueMember = "ID";
            cboNhomkho.DataSource = d.get_data("select id,ten from d_dmnhomkho order by id").Tables[0];
           

          

            DataSet ds12 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'D_DMBD' and DATA_TYPE<>'NUMBER' and DATA_TYPE<>'DATE'  ";
            ds12 = d.get_data_text(sql);
            cbdanhmucduoc.DisplayMember = "MA";
            cbdanhmucduoc.ValueMember = "MA";
            cbdanhmucduoc.DataSource = ds12.Tables[0];
            cbdanhmucduoc.SelectedIndex = -1;


            DataSet ds13 = new DataSet();
            sql = "select UPPER(column_name) as MA from dba_tab_columns where owner = '" + d.user.ToUpper() + "' AND TABLE_NAME = 'D_DMBD' and DATA_TYPE='NUMBER'  ";
            ds13 = d.get_data_text(sql);
            cbdanhmucduoc_kieuso.DisplayMember = "MA";
            cbdanhmucduoc_kieuso.ValueMember = "MA";
            cbdanhmucduoc_kieuso.DataSource = ds13.Tables[0];
            cbdanhmucduoc_kieuso.SelectedIndex = -1;



            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;


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
            if (!bExist_Col(d.user, "d_nhombo", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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
            if (!bExist_Col(d.user, "d_dmnhomkt", "KHONGDAU"))
            {
                MessageBox.Show("Không tồn tại cột, KHONGDAU !");
                return;

            }
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




    }
   

   

    
   
}