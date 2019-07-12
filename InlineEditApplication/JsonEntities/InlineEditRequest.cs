using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InlineEditApplication.JsonEntities
{
    public class InlineEditRequest
    {
        [JsonProperty("fraginfo")]
        public FragInfo[] Fraginfo { get; set; }
    }
}
