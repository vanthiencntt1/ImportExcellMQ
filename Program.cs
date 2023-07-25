using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ImportXML
{
    static class Program
    {
        public static OracleHelper.OracleSupport Oracle = new OracleHelper.OracleSupport();
        public static LibHIS.AccessData dal;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            dal = new LibHIS.AccessData(Oracle);

            if (!dal.isCheckLicense())
            {
                //MQLicense.FrmLicense f = new MQLicense.FrmLicense();
                //f.ShowDialog();
            }
            else
            {
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            
        }
    }
}