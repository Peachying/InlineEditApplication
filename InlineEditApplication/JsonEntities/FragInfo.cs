using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEditApplication.JsonEntities
{
    public class FragInfo
    {
        [JsonProperty("startline")]
        public string StartLine { get; set; }
        [JsonProperty("endline")]
        public string EndLine { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("origin_url")]
        public string Origin_url { get; set; }
    }
}
