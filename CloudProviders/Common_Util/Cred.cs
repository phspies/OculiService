using System.Runtime.Serialization;

[DataContract]
public class Cred
{
    [DataMember]
    public string HostName;
    [DataMember]
    public string UserName;
    [DataMember]
    public string Password;
    [DataMember]
    public string HostAlias;
}
