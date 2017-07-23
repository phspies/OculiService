using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.Common.Interfaces
{
    public interface IEngineSettings
    {
        bool BypassProxyForLocal { get; }
        int HttpsPort { get; }
        string EngineID { get; }
        string AccessKey { get; }
        string SecretKey { get; }
        int ApiTimeout { get; }
        bool UseWebProxy { get; }
        string WebProxyUsername { get; }
        string WebProxyPassword { get; }
    }
}
