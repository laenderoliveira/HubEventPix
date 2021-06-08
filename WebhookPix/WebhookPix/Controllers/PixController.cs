using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebhookPix.BRCode;
using WebhookPix.Model.InputModels;

namespace WebhookPix.Controllers
{
    [ApiController]
    [Route("pix")]
    public class PixController : ControllerBase
    {
        private readonly ILogger<PixController> _logger;

        public PixController(ILogger<PixController> logger)
        {
            _logger = logger;
        }

        [HttpPost("static")]
        public IActionResult Static([FromBody] BRCodeStaticInputModel brCode)
        {
            var payload = new Payload()
                                .SetAmout(brCode.Amount)
                                .SetDescription(brCode.Description)
                                .SetMerchantCity(brCode.MerchantCity)
                                .SetMerchantName(brCode.MerchantName)
                                .SetPixKey(brCode.Key)
                                .SetTxid(brCode.TxId)
                                .GetPayload();

            return Ok(payload);
        }

        [HttpPost("dynamic")]
        public IActionResult Dynamic([FromBody] BRCodeDynamicInputModel brCode)
        {
            var payload = new Payload()
                                .SetUniquePayment(true)
                                .SetUrl(brCode.Url)
                                .SetMerchantCity(brCode.MerchantCity)
                                .SetMerchantName(brCode.MerchantName)
                                .GetPayload();

            return Ok(payload);
        }
    }
}
