using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using WebhookPix.Model;

namespace WebhookPix
{
    public static class Extensions
    {
        public static T BindTo<T>(this IConfiguration configuration, string key, T target)
        {
            configuration.GetSection(key).Bind(target);
            return target;
        }
    }
}
