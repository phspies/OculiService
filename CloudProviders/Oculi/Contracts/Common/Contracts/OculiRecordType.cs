using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Common.Contracts
{
    public class OculiRecordType : OculiTimestampType, IOculiErrorType
    {
        [JsonProperty("errors")]
        public List<string> errors { get; set; }
    }
}
