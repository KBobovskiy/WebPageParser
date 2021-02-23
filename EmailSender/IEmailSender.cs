using System.Net;
using System.Net.Mail;
using System;

namespace EmailSender
{
    public interface IEmailSender
    {
        bool TrySendEmail(EmailMessage message);
    }
}