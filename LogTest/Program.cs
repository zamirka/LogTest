using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using NLog;
using System.IO;
using System.Runtime.Serialization;
using System.Globalization;

namespace LogTest
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                var logEntryObj = GetLogEntry();
                var logEntry = GetJson(logEntryObj);
                logger.Info(logEntry);
                var fileName = logEntryObj.FileName;
                logEntry = GetJson(GetLogEntry(true, fileName));
                logger.Info(logEntry);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static LogEntry GetLogEntry(Boolean repeat = false, String lastFileName = "")
        {
            var fileName = repeat ? lastFileName : String.Format("AVM_{1}_{0}.pdf", Guid.NewGuid(), DateTime.Now.ToShortDateString());
            var op = repeat ? "DELE" : "RETR";

            return new LogEntry
            {
                date = DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US")),
                daemon = "ftp proftpd[42439]",
                path = String.Format("/ftp/public/OUT/biit/123/{0}", fileName),
                user = "biit",
                operation = String.Format("{0} /123/{1}", op, fileName),
                field1 = "226",
                field2 = "3799937",
                ip = "213.79.88.19",
                FileName = fileName
            };
        }
        static String GetJson(LogEntry entry)
        {
            var serializer = new DataContractJsonSerializer(typeof(LogEntry));
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    serializer.WriteObject(stream, entry);
                }
                catch(Exception)
                {
                    serializer.WriteObject(stream, new LogEntry());
                }
                return Encoding.Default.GetString(stream.GetBuffer());
            }
        }
    }

    [DataContract]
    class LogEntry
    {
        [DataMember(Order = 0)]
        public String date { get; set; }
        [DataMember(Order = 1)]
        public String daemon { get; set; }
        [DataMember(Order = 2)]
        public String path { get; set; }
        [DataMember(Order = 3)]
        public String user { get; set; }
        [DataMember(Order = 4)]
        public String operation { get; set; }
        [DataMember(Order = 5)]
        public String field1 { get; set; }
        [DataMember(Order = 6)]
        public String field2 { get; set; }
        [DataMember(Order = 7)]
        public String ip { get; set; }

        public String FileName { get; set; }
        public LogEntry()
        {
            date = daemon = path = user = operation = field1 = field2 = ip = FileName = String.Empty;
        }
    }
}
