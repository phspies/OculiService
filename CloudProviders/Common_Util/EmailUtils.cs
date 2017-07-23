using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;

public class EmailUtils
{
    private Queue<EmailInfo> _queueEmails = new Queue<EmailInfo>();
    private System.Threading.Semaphore _semaphore = new System.Threading.Semaphore(0, 99999);
    private Thread _sendEmailThread;
    private bool _stop;

    public EmailUtils()
    {
        this._sendEmailThread = new Thread(new ThreadStart(this.SendEmailThreadFunc));
        this._sendEmailThread.IsBackground = true;
        this._sendEmailThread.Start();
    }

    public void SendEmail(string EmailRecipients, string subject, string body, EmailServer MailServer)
    {
        if (string.IsNullOrEmpty(EmailRecipients) || string.IsNullOrEmpty(MailServer.SMTPServer))
            return;
        MailMessage mailMsg = new MailMessage(MailServer.FromAddress, EmailRecipients);
        mailMsg.Subject = subject;
        mailMsg.Body = body;
        SmtpClient smtpClient = new SmtpClient(MailServer.SMTPServer);
        if (MailServer.Username != null && MailServer.Password != null)
            smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential(MailServer.Username, (string)MailServer.Password);
        SmtpClient smtp = smtpClient;
        EmailInfo emailInfo = new EmailInfo(mailMsg, smtp);
        lock (this)
            this._queueEmails.Enqueue(emailInfo);
        this._semaphore.Release();
    }

    public void Shutdown()
    {
        this._stop = true;
        this._semaphore.Release();
    }

    private void SendEmailThreadFunc()
    {
        while (true)
        {
            this._semaphore.WaitOne();
            if (!this._stop)
            {
                EmailInfo emailInfo = (EmailInfo)null;
                lock (this)
                    emailInfo = this._queueEmails.Dequeue();
                try
                {
                    emailInfo.Smtp.Send(emailInfo.EmailMsg);
                }
                catch (Exception ex)
                {
                }
            }
            else
                break;
        }
    }
}
