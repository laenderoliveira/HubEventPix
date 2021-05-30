using Newtonsoft.Json;
using System;

namespace WebhookPix.Model
{
    public class Pix
    {
        public string EndToEndId { get; set; }

        public string Txid { get; set; }

        public string Chave { get; set; }

        public decimal Valor { get; set; }

        public DateTime Horario { get; set; }
    }
}
