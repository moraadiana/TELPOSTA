using System;
using System.IO;
using System.Globalization;
using b2b.Models;
using Newtonsoft.Json;
using System.Configuration;

namespace b2b.Respository
{
    public class ErrorHandler
    {
        //public static string LogErrorToDb(Exception ex)
        //{
        //    string error = "";
        //    try
        //    {
        //        GetExceptionMessage(ex);
        //        return error = JsonConvert.SerializeObject(new ApiException { error = true, message = ex.Message.ToString(), data = null });
        //    }
        //    catch (Exception EX)
        //    {
        //        return error = JsonConvert.SerializeObject(new ApiException { error = true, message = ex.Message.ToString(), data = null });
        //    }
           
        //}
        public static string GetExceptionMessage(Exception ex)
        {
            if (ex.InnerException != null)
            {

                WriteLog("Error", ex.Message.ToString() + " : : " + ex.InnerException.Message.ToString() + " : : " + ex.StackTrace.ToString());
                return ex.InnerException.Message;// +" : " + ex.StackTrace.ToString();

            }
            else
            {
                WriteLog("Error", ex.Message.ToString() + " : : " + ex.StackTrace.ToString());
                return ex.Message;// +" : " + ex.StackTrace.ToString(); 
            }
        }

         public static void WriteLog(string ErrType,string Message)
        {
            string Path = ConfigurationManager.AppSettings["Path"];
            try
            {
                string loc = Path + "/" + DateTime.Today.ToString("dd-MM-yy");
                if (!Directory.Exists(loc))
                    Directory.CreateDirectory(loc);
                string path = loc + "/" + ErrType + ".txt";
                if (!File.Exists(path))
                    File.Create(path).Dispose();
                if (File.Exists(path))
                {
                    using (StreamWriter w = File.AppendText(path))
                    {
                        w.Write("\r\nLog Entry :");
                        w.Write("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                        w.WriteLine("-:-" + Message);
                        w.Flush();
                        w.Close();
                    }
                }
            }
            catch (Exception ex) { WriteLog("Exception", ex.InnerException.ToString()); }
    }
    }
}