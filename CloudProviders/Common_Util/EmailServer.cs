using System;
using System.Runtime.Serialization;

[DataContract]
[Serializable]
public class EmailServer
{
    [DataMember]
    public string SMTPServer;
    [DataMember]
    public string FromAddress;
    [DataMember]
    public string Username;
    [DataMember]
    public Password Password;

    public EmailServer()
    {
    }

    public EmailServer(string smtpServer, string fromAddress, string username, Password password)
    {
        this.SMTPServer = smtpServer;
        this.FromAddress = fromAddress;
        this.Username = username;
        this.Password = password;
    }
}
