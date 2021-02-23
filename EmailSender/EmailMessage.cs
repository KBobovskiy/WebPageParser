using System.Net.Mail;

namespace EmailSender
{
    public class EmailMessage
    {
        public MailAddress AddressTo { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyInHtml { get; set; }
    }
}