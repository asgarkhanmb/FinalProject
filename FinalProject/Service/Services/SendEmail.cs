using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class SendEmail :ISendEmail
    {
        public void Send(string from, string displayName, string to, string messageBody, string subject)
        {
            MailMessage mailMessage = new();
            mailMessage.From = new MailAddress(from, displayName);
            mailMessage.To.Add(new MailAddress(to));
            mailMessage.Subject = subject;
            mailMessage.Body = messageBody;
            mailMessage.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("asgarkhanmb@code.edu.az", "vobc mioe slye ibpz");
            smtpClient.Send(mailMessage);
        }
    }
}
