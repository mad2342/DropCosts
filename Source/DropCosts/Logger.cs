using System;
using System.IO;



namespace DropCosts {
    public class Logger
    {
        static string filePath = $"{DropCosts.ModDirectory}/DropCosts.log";
        public static void LogError(Exception ex)
        {
            if (DropCosts.DebugLevel >= 1)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    var prefix = "[DropCosts @ " + DateTime.Now.ToString() + "]";
                    writer.WriteLine("Message: " + ex.Message + "<br/>" + Environment.NewLine + "StackTrace: " + ex.StackTrace + "" + Environment.NewLine);
                    writer.WriteLine("----------------------------------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        public static void LogLine(String line)
        {
            if (DropCosts.DebugLevel >= 2)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    var prefix = "[DropCosts @ " + DateTime.Now.ToString() + "]";
                    writer.WriteLine(prefix + line);
                }
            }
        }
    }
}