using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiDatamoverDeploymentPolicyType : OculiRecordType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("_default")]
        public bool _default { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("policy")]
        public string policy { get; set; }
        [JsonProperty("dm_installpath")]
        public string dm_installpath { get; set; }
        [JsonProperty("dm_windows_temppath")]
        public string dm_windows_temppath { get; set; }
        [JsonProperty("dm_linux_temppath")]
        public string dm_linux_temppath { get; set; }
        [JsonProperty("dm_inifile")]
        public string dm_inifile { get; set; }
        [JsonProperty("dm_max_memory")]
        public int dm_max_memory { get; set; }
        [JsonProperty("dm_windows_queue_folder")]
        public string dm_windows_queue_folder { get; set; }
        [JsonProperty("dm_linux_queue_folder")]
        public string dm_linux_queue_folder { get; set; }
        [JsonProperty("dm_queue_limit_disk_size")]
        public int dm_queue_limit_disk_size { get; set; }
        [JsonProperty("dm_queue_min_disk_free_size")]
        public int dm_queue_min_disk_free_size { get; set; }
        [JsonProperty("dm_queue_scheme")]
        public int dm_queue_scheme { get; set; }
        [JsonProperty("dm_source_activation_code")]
        public string dm_source_activation_code { get; set; }
        [JsonProperty("dm_target_activation_code")]
        public string dm_target_activation_code { get; set; }
    }
}
