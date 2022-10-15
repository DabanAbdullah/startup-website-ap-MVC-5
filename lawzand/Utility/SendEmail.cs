using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace lawzand.Utility
{
    public class SendEmail
    {
        public static bool EmailSend(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false)
        {
            bool status = false;
            try
            {
                string HostAddress = ConfigurationManager.AppSettings["Host"].ToString();
                string FormEmailId = ConfigurationManager.AppSettings["MailFrom"].ToString();
                string Password = ConfigurationManager.AppSettings["Password"].ToString();
                string Port = ConfigurationManager.AppSettings["Port"].ToString();
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(FormEmailId);
                mailMessage.Subject = Subject;
                mailMessage.Body = Message;
                mailMessage.IsBodyHtml = IsBodyHtml;
                mailMessage.To.Add(new MailAddress(SenderEmail));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = HostAddress;
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential();
                networkCredential.UserName = mailMessage.From.Address;
                networkCredential.Password = Password;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = Convert.ToInt32(Port);
             smtp.Send(mailMessage);
                status = true;
                return  status;
            }
            catch (Exception ex)
            {

                return status;
            }
        }
    }
}