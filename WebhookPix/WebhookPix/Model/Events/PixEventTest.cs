using Newtonsoft.Json;
using System;

namespace WebhookPix.Model.Events
{
    public class PixEventTest
    {
        public string Evento { get; set; }

        [JsonProperty(PropertyName = "data_criacao")]
        public DateTime DataCriacao { get; set; }
    }
}
