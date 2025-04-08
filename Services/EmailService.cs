using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Tcc_DbTracker_API.Models;

namespace Tcc_DbTracker_API.Services
{
    public class EmailService
    {
        private readonly EmailModel _emailModel;

        public EmailService(IOptions<EmailModel> emailOptions)
        {
            _emailModel = emailOptions.Value;
        }

        public void SendEmail(string assunto, string corpo)
        {
            var fromAddress = new MailAddress(_emailModel.Remetente, _emailModel.NomeRemetente);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailModel.Remetente, _emailModel.SenhaApp)
            };

            var message = new MailMessage
            {
                From = fromAddress,
                Subject = assunto,
                Body = corpo
            };

            foreach (var destinatario in _emailModel.Destinatarios)
            {
                message.To.Add(destinatario);
            }

            smtp.Send(message);
        }
    }
}