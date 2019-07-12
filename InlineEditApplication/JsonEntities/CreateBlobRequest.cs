using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEditApplication.JsonEntities
{
    public class CreateBlobRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("encoding")]
        public string Encoding { get; set; }

    }
}
