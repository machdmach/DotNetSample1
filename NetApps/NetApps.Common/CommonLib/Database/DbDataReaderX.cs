using System.Data.Common;

namespace System
{
    public class DbDataReaderX
    {
        //===================================================================================================

        //public static void ReadBLOB_cm(DbCommand cmd)
        //{
        //    cmd.CommandText = "select id, logo from tab1";
        //    DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
        //    while (rdr.Read())
        //    {
        //        var col1 = rdr[0];
        //    }

        //    string certFile = HttpContext.Current.Server.MapPath("~/App_Data/vip_cert.p12");
        //    FileStream fs = File.Open(certFile, FileMode.Open, FileAccess.Read); //thread having fs can only read (not write)
        //    byte[] buffer = new byte[fs.Length];
        //    int count = fs.Read(buffer, 0, buffer.Length);
        //    fs.Close();
        //    X509Certificate2 cert = new X509Certificate2(buffer, "xxxxxxx");
        //}
        //===================================================================================================

    }
}
