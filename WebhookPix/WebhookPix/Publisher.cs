using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using WebhookPix.Model;
using WebhookPix.Model.Events;

namespace WebhookPix
{
    public class Publisher : IDisposable
    {

        private readonly ILogger<Publisher> logger;
        private readonly IModel model;
        private readonly WebhookConfiguration webhookConfiguration;


        public Publisher(ILogger<Publisher> logger, IModel model, WebhookConfiguration webhookConfiguration)
        {
            this.logger = logger;
            this.model = model;
            this.webhookConfiguration = webhookConfiguration;
        }

        public void PublishPixReceived(HttpRequest request, List<Pix> listPix)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            IBasicProperties basicProperties = BuildProperties(this.model, request);

            foreach (var pix in listPix)
            {
                logger.LogInformation("Pix received: chave: {0}, valor: {1}, txid: {2}", pix.Chave, pix.Valor, pix.Txid);

                byte[] bodyBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pix, serializeOptions));
                model.BasicPublish(webhookConfiguration.Exchange, $"pix.{pix.Chave}", basicProperties, bodyBytes);
            }
        }

        private static IBasicProperties BuildProperties(IModel model, HttpRequest request)
        {
            var basicProperties = model.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            basicProperties.ContentType = "application/json";

            basicProperties.Headers = new Dictionary<string, object>
            {
                { "Received-At", DateTime.UtcNow.ToString("u") },
                { "Received-At-BrTime", DateTime.UtcNow.ToLocalTime().ToString("R") }
            };

            foreach (var httpHeader in request.Headers)
            {
                basicProperties.Headers.Add(httpHeader.Key, httpHeader.Value.ToString());
            }

            return basicProperties;
        }

        public void Dispose()
        {
            model.Dispose();
        }
    }
}
