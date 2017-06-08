using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("addresslist")]
        public string addresslist { get; set; }
        [JsonProperty("deploymentpolicy_id")]
        public string deploymentpolicy_id { get; set; }
        [JsonProperty("platformtemplate_id")]
        public string platformtemplate_id { get; set; }
        [JsonProperty("application_id")]
        public string application_id { get; set; }
        [JsonProperty("cpu")]
        public int cpu { get; set; }
        [JsonProperty("cpu_speed")]
        public int cpu_speed { get; set; }
        [JsonProperty("cores")]
        public int cores { get; set; }
        [JsonProperty("memory")]
        public int memory { get; set; }
        [JsonProperty("ostype")]
        public string ostype { get; set; }
        [JsonProperty("osedition")]
        public string osedition { get; set; }
        [JsonProperty("serialnumber")]
        public string serialnumber { get; set; }
        [JsonProperty("id")]
        public string model { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("moid1")]
        public string moid1 { get; set; }
        [JsonProperty("moid2")]
        public string moid2 { get; set; }
        [JsonProperty("moid3")]
        public string moid3 { get; set; }
        [JsonProperty("hardwaretype")]
        public int hardwaretype { get; set; }
        [JsonProperty("perf_collection_enabled")]
        public bool perf_collection_enabled { get; set; }
        [JsonProperty("perf_collection_status")]
        public bool perf_collection_status { get; set; }
        [JsonProperty("perf_collection_message")]
        public string perf_collection_message { get; set; }
        [JsonProperty("perf_contact_error_count")]
        public int perf_contact_error_count { get; set; }
        [JsonProperty("perf_last_contact")]
        public DateTime perf_last_contact { get; set; }
        [JsonProperty("os_collection_enabled")]
        public bool os_collection_enabled { get; set; }
        [JsonProperty("os_collection_status")]
        public bool os_collection_status { get; set; }
        [JsonProperty("os_collection_message")]
        public string os_collection_message { get; set; }
        [JsonProperty("os_contact_error_count")]
        public int os_contact_error_count { get; set; }
        [JsonProperty("os_last_contact")]
        public DateTime os_last_contact { get; set; }
        [JsonProperty("netstat_collection_enabled")]
        public bool netstat_collection_enabled { get; set; }
        [JsonProperty("netstat_collection_status")]
        public bool netstat_collection_status { get; set; }
        [JsonProperty("netstat_collection_message")]
        public string netstat_collection_message { get; set; }
        [JsonProperty("netstat_contact_error_count")]
        public int netstat_contact_error_count { get; set; }
        [JsonProperty("netstat_last_contact")]
        public DateTime netstat_last_contact { get; set; }
        [JsonProperty("dm_collection_enabled")]
        public bool dm_collection_enabled { get; set; }
        [JsonProperty("dm_collection_status")]
        public bool dm_collection_status { get; set; }
        [JsonProperty("dm_collection_message")]
        public string dm_collection_message { get; set; }
        [JsonProperty("dm_contact_error_count")]
        public int dm_contact_error_count { get; set; }
        [JsonProperty("dm_last_contact")]
        public DateTime dm_last_contact { get; set; }
        [JsonProperty("last_dm_event_id")]
        public long last_dm_event_id { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("primary_dns")]
        public string primary_dns { get; set; }
        [JsonProperty("secondary_dns")]
        public string secondary_dns { get; set; }
        [JsonProperty("timezone")]
        public string timezone { get; set; }
        [JsonProperty("sync_fw_rules")]
        public bool sync_fw_rules { get; set; }
        [JsonProperty("sync_tag_rules")]
        public bool sync_tag_rules { get; set; }
        [JsonProperty("sync_affinity_rules")]
        public bool sync_affinity_rules { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty("credential")]
        public OculiWorkloadPackageType credential { get; set; }
        [JsonProperty("platform")]
        public OculiPlatformType platform { get; set; }
        [JsonProperty("workloaddisks")]
        public List<OculiWorkloadDiskType> workloaddisks { get; set; }
        [JsonProperty("workloadvolumes")]
        public List<OculiWorkloadVolumeType> workloadvolumes { get; set; }
        [JsonProperty("workloadinterfaces")]
        public List<OculiWorkloadInterfaceType> workloadinterfaces { get; set; }
        [JsonProperty("workloadpackages")]
        public List<OculiWorkloadProcessType> workloadpackages { get; set; }
        [JsonProperty("workloadprocesses")]
        public List<OculiWorkloadPackageType> workloadprocesses { get; set; }

        [JsonProperty("datamoverdeploymentpolicy")]
        public List<OculiDatamoverDeploymentPolicyType> datamoverdeploymentpolicy { get; set; }
        [JsonProperty("datamovetpolicy")]
        public List<OculiDatamoverPolicyType> datamovetpolicy { get; set; }
    }
}
