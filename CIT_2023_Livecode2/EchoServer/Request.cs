using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EchoServer
{
    internal class Request
    {
        [JsonInclude]
        [JsonPropertyName("Method")]
        public string? Method { get; set; }
        [JsonInclude]
        [JsonPropertyName("Path")]
        public string? Path { get; set; }
        [JsonInclude]
        [JsonPropertyName("Date")]
        public string? Date { get; set; }
        [JsonInclude]
        [JsonPropertyName("Body")]
        public string? Body { get; set; }

    }
}