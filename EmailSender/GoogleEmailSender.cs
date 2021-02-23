using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EmailSender
{
    public class GoogleEmailSender : IEmailSender
    {
        private readonly MailAddress AddressFrom;

        private readonly string emailLogin;
        private readonly string emailPassword;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GoogleEmailSender(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException($"{nameof(login)} not set ", nameof(login));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"{nameof(password)} not set ", nameof(password));
            }

            emailLogin = login;
            emailPassword = password;

            AddressFrom = new MailAddress("kbobovskiy@gmail.com", "K. Bobovskiy");
        }

        public bool TrySendEmail(EmailMessage message)
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailLogin, emailPassword)
            };

            using (var email =
                new MailMessage(AddressFrom, message.AddressTo)
                {
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = message.IsBodyInHtml
                })
            {
                try
                {
                    smtp.Send(email);
                    logger.Info($"Sent email to: {email.To} with subject: {email.Subject}");
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to send email with error: {ex.Message}. Mail to: {email.To} with subject: {email.Subject}");
                    return false;
                }
            }
        }
    }
}