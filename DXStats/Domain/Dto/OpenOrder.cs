﻿using Newtonsoft.Json;

namespace DXStats.Domain.Dto
{
    public class OpenOrder
    {
        public string Id { get; set; }
        public string Maker { get; set; }
        [JsonProperty("maker_size", NullValueHandling = NullValueHandling.Ignore)]
        public string MakerSize { get; set; }
        public string Taker { get; set; }
        [JsonProperty("taker_size", NullValueHandling = NullValueHandling.Ignore)]
        public string TakerSize { get; set; }
        [JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
        public string UpdatedAt { get; set; }
        [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
        public string CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
