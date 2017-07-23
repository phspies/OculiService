using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts.Common
{
    public interface IOculiErrorType
    {
        List<string> errors { get; set; }
    }
}
