using System.Net.Mail;

internal class EmailInfo
{
    public MailMessage EmailMsg;
    public SmtpClient Smtp;

    public EmailInfo(MailMessage mailMsg, SmtpClient smtp)
    {
        this.EmailMsg = mailMsg;
        this.Smtp = smtp;
    }
}
