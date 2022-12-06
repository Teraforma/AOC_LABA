using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerV2
{
    internal class LogMachine
    {
        private readonly string _defaultName = "serverLog.txt";
        private string FullPath;
        private readonly string _TxtBreakPoing = "< / >";//BreAkPoiNt$£
        FileStream stream = null;


        public LogMachine(string fullPath = @"D:\test\log.txt")
        {
            if (!fullPath.ToLower().Contains(".txt"))
            {
                char last = fullPath[fullPath.Length - 1];
                if (!last.Equals(@"\"))
                {
                    fullPath += @"\";
                }
                fullPath += _defaultName;
            }
            FullPath = fullPath;

            stream = new FileStream(FullPath, FileMode.OpenOrCreate);
            // Create a StreamWriter from FileStream  
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.WriteLine("Start");
            }
            //stream.Close();
            //stream = null;
            //stream = new FileStream(FullPath, FileMode.Append);
        }
        public void ToLog(string ToSaveCommand)
        {
            ToLog(ToSaveCommand, "null");
        }
        public void ToLog(string ToSaveCommand, int ToSave)
        {
            ToLog(ToSaveCommand, ToSave.ToString());
        }
        public void ToLog(string ToSaveCommand, string ToSave)
        {
            ToSave = ToSaveCommand + _TxtBreakPoing + @ToSave + _TxtBreakPoing + DateTime.Now.ToString("o") + _TxtBreakPoing;


            try
            {
                // Create a FileStream with mode CreateNew  
                stream = new FileStream(FullPath, FileMode.Append);
                // Create a StreamWriter from FileStream  
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(ToSave);
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
            // Read a file  

        }

    }
}
