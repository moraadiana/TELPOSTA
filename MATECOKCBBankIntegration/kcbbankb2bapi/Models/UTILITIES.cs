using System;
using System.Data.SqlClient;
using System.IO;

namespace b2b
{
	internal class UTILITIES
	{
		public static string User
		{
			get
			{
				return "UserCoopBank";
			}
		}
		public static string Password
		{
			get
			{
				return "rfrfg#f_yKPd-ZvG2$Lx5w";
			}
		}

		public static string DebugFileName
		{
			get
			{
				return "C:\\b2b-API-Logs\\Debug - " + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt";
			}
		}
        public static string LogFilePath
        {
            get
            {
                return "C:\\b2b-API-Logs\\b2b-API-" + DateTime.Now.ToString ("yyyy-MM-dd-HH") + ".txt";
            }
        }

		
		public static string TitleCase(string sentence)
		{
			try
			{
				string[] array = sentence.Split(" ".ToCharArray());
				sentence = "";
				string[] array2 = array;
				foreach (string text in array2)
				{
					sentence = sentence + text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower() + " ";
				}
			}
			catch (Exception ex)
			{
				Logexception(ex);
				ex.Data.Clear();
			}
			return sentence.Trim();
		}

		public static void Logexception(Exception ex)
		{
            LogDebugOnFile(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"\n"+  ex.Source + "\n" +ex.Message + "\n" +  ex.StackTrace);
			//LogDebugOnFile(ex.StackTrace);
		}

		public static void LogDebugOnFile(string clientRequest)
		{
			try
			{
				if (!File.Exists(DebugFileName))
				{
					File.Create(DebugFileName).Dispose();
				}
				File.AppendAllText(DebugFileName, clientRequest + "\n");
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
			}
		}
        public static void WriteLogOnFile(string jsonRequest)
        {
            try
            {
                if (!File.Exists(LogFilePath))
                {
                    File.Create(LogFilePath).Dispose();
                }
                File.AppendAllText(LogFilePath,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")+"\t"+ jsonRequest + "\n");
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        public static void LogEntryOnFile(string fileName, string clientRequest)
		{
			try
			{
				if (!File.Exists(fileName))
				{
					File.Create(fileName).Dispose();
				}
				File.AppendAllText(fileName, clientRequest);
			}
			catch (Exception ex)
			{
				ex.Data.Clear();
			}
		}

	}
}
