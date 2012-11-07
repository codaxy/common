using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace Codaxy.Common.Logging
{
    public class MailLogAppender : ILogAppender
    {
        public String To { get; set; }
        public String Bcc { get; set; }
        public String From { get; set; }

        public bool AppendLogLevelToMessageSubject { get; set; }

        public class SmtpConfiguration
        {
            public String Host { get; set; }
            public int Port { get; set; }
            public bool SSL { get; set; }

            public string Username { get; set; }
            public String Password { get; set; }
        }

        public SmtpConfiguration Smtp { get; set; }

        public String Subject { get; set; }

        public MailLogAppender()
        {
            
        }

        public void Log(LogEntry entry)
        {
            //if (Smtp == null || Smtp.Host == null || From == null)
            //{
            //    Debug.WriteLine("Mail appender not configured properly.");
            //    return;
            //}

            try
            {
                StringBuilder mb = new StringBuilder();

                mb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff} {1}: {2}", entry.Message.Time, entry.Message.Level, entry.LoggerName).AppendLine();
                mb.AppendLine();
                mb.AppendLine(entry.Message.Message);

                if (entry.Message.StackTrace != null)
                {
                    mb.AppendLine();
                    mb.AppendLine(entry.Message.StackTrace);
                }

                using (var msg = new MailMessage())
                {
                    if (!String.IsNullOrWhiteSpace(Bcc))
                        msg.Bcc.Add(PrepareAddresses(Bcc));
                    
                    if (!String.IsNullOrWhiteSpace(To))
                        msg.To.Add(PrepareAddresses(To));
                    
                    if (!String.IsNullOrEmpty(From))
                        msg.From = new MailAddress(From);

                    msg.Body = mb.ToString();
                    
                    msg.Subject = Subject;
                    if (AppendLogLevelToMessageSubject)
                        msg.Subject += " " + entry.Message.Level.ToString();

                    using (var smtp = GetSmtpClient())
                    {
                        smtp.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MailLogAppender.Log exception: " + ex);
            }
        }

        public SmtpClient GetSmtpClient()
        {
            if (Smtp == null)
                return new SmtpClient();

            var client = new SmtpClient(Smtp.Host, Smtp.Port);

            if (Smtp.Password != null)
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(Smtp.Username, Smtp.Password);
            }
            else
                client.UseDefaultCredentials = true;

            client.EnableSsl = Smtp.SSL;
            return client;
        }

        private string PrepareAddresses(string a)
        {
            if (a == null)
                a = String.Empty;
            return a.Replace(" ", ",").Replace(";", ",").Replace(",,", ",");
        }
    }
}
