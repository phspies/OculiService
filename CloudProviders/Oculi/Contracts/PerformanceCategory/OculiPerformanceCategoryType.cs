using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts
{

    public class OculiPerformanceCategoryType : OculiRecordType
    {
        public OculiPerformanceCategoryType()
        {
            performancecounters = new List<OculiPerformanceCounterType>();
        }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("category")]
        public string category { get; set; }
        [JsonProperty("counter")]
        public string counter { get; set; }
        [JsonProperty("instance")]
        public string instance { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty("max")]
        public float max { get; set; }
        [JsonProperty("avg")]
        public float avg { get; set; }
        [JsonProperty("_00")]
        public float _00 { get; set; }
        [JsonProperty("_01")]
        public float _01 { get; set; }
        [JsonProperty("_02")]
        public float _02 { get; set; }
        [JsonProperty("_03")]
        public float _03 { get; set; }
        [JsonProperty("_04")]
        public float _04 { get; set; }
        [JsonProperty("_05")]
        public float _05 { get; set; }
        [JsonProperty("_06")]
        public float _06 { get; set; }
        [JsonProperty("_07")]
        public float _07 { get; set; }
        [JsonProperty("_08")]
        public float _08 { get; set; }
        [JsonProperty("_09")]
        public float _09 { get; set; }
        [JsonProperty("_10")]
        public float _10 { get; set; }
        [JsonProperty("_11")]
        public float _11 { get; set; }
        [JsonProperty("_12")]
        public float _12 { get; set; }
        [JsonProperty("_13")]
        public float _13 { get; set; }
        [JsonProperty("_14")]
        public float _14 { get; set; }
        [JsonProperty("_15")]
        public float _15 { get; set; }
        [JsonProperty("_16")]
        public float _16 { get; set; }
        [JsonProperty("_17")]
        public float _17 { get; set; }
        [JsonProperty("_18")]
        public float _18 { get; set; }
        [JsonProperty("_19")]
        public float _19 { get; set; }
        [JsonProperty("_20")]
        public float _20 { get; set; }
        [JsonProperty("_21")]
        public float _21 { get; set; }
        [JsonProperty("_22")]
        public float _22 { get; set; }
        [JsonProperty("_23")]
        public float _23 { get; set; }
        [JsonProperty("performancecounters_attributes")]
        public List<OculiPerformanceCounterType> performancecounters { get; set; }
    }
}
