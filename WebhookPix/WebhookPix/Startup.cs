using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using WebhookPix.Model;

namespace WebhookPix
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            var connectionFactory = Configuration.BindTo("RABBITMQ", new ConnectionFactory());

            var webhookConfiguration = Configuration.BindTo("RABBITMQ", new WebhookConfiguration());

            services.AddSingleton(connectionFactory);

            services.AddSingleton(webhookConfiguration);
            
            services.AddSingleton(sp => sp.GetRequiredService<ConnectionFactory>().CreateConnection());

            services.AddTransient(sp => sp.GetRequiredService<IConnection>().CreateModel());

            services.AddScoped<Publisher>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebhookPix", Version = "v1" });
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebhookPix v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            InitializeMessageBroker(app, logger);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
