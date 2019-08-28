using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.WinAppSendEmail
{
    public static class Libraries
    {
        public static void WriteLog(string str)
        {
            try
            {
                bool exists = System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/error/");

                if (!exists)
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/error/");
                using (var writer = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/error/" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "error.log", true))
                {
                    StringBuilder err = new StringBuilder();
                    err.Append("\n--------------------------------------------\nDate: " + DateTime.Now.ToString("dd/MM/yyyy HH:ss") + "\n");
                    err.Append("\nError Message:" + str);
                    writer.WriteLine(err);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
