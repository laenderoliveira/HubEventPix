using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebhookPix.Model;

namespace WebhookPix.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly Publisher _publisher;

        public WebhookController(ILogger<WebhookController> logger, Publisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Server online");
        }

        [HttpPost]
        public IActionResult Test([FromBody] PixEventTest pixEventTest)
        {
            _logger.LogInformation("Event test received: evento: {0}, data_criacao: {1} ", 
                                        pixEventTest.Evento, pixEventTest.DataCriacao.ToString("R"));
            return Ok();
        }

        [HttpPost("pix")]
        public IActionResult PixReceived([FromBody] PixEvent pixEvent)
        {
            _publisher.PublishPixReceived(Request, pixEvent.Pix);

            return Ok();
        }
    }
}
