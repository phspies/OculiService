
namespace OculiService.Common.Interfaces
{
    public interface ICoreEngineSettings
    {
        bool BypassProxyForLocal { get; }
        int HttpsPort { get; }
        string CoreEngineID { get; set; }
        string OrganizationID { get; }
        string AccessKey { get; set; }
        string SecretKey { get; set; }
        string TokenAccessCode { get; set; }
        long TokenExpiry { get; set; }
        int ApiTimeout { get; }
        bool UseWebProxy { get; }
        string WebProxyUsername { get; }
        string WebProxyPassword { get; }
    }
}
