namespace WebhookPix.Model.InputModels
{
    public class BRCodeStaticInputModel
    {
        public double Amount { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCity { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public string TxId { get; set; }
    }
}
