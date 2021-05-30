using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WebhookPix.Model;

namespace WebhookPix
{
    public partial class Startup
    {
        public void InitializeMessageBroker(IApplicationBuilder app, ILogger logger)
        {

            logger.LogInformation("Trying to create RabbitMQ exchange ... ");
            var model = app.ApplicationServices.GetRequiredService<IModel>();
            var webhookConfiguration = app.ApplicationServices.GetRequiredService<WebhookConfiguration>();

            model.ExchangeDeclare(webhookConfiguration.Exchange, webhookConfiguration.ExchangeType, true, false);

            logger.LogInformation("Created ok. Exchange: {0} ExchangeType: {1}.", webhookConfiguration.Exchange, webhookConfiguration.ExchangeType);
        }
    }
}
