using System.Globalization;

namespace WebhookPix.BRCode
{
    public class Payload
    {
        public string PixKey { get; private set; }

        public string Description { get; private set; }

        public string MerchantName { get; private set; }

        public string MerchantCity { get; private set; }

        public string Txid { get; private set; }

        public string Amount { get; private set; }

        public string Url { get; private set; }

        public bool UniquePayment { get; private set; }

        public Payload SetTxid(string txtid)
        {
            Txid = txtid;
            return this;
        }

        public Payload SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public Payload SetMerchantName(string merchantName)
        {
            MerchantName = merchantName;
            return this;
        }

        public Payload SetMerchantCity(string merchantCity)
        {
            MerchantCity = merchantCity;
            return this;
        }

        public Payload SetPixKey(string pixKey)
        {
            PixKey = pixKey;
            return this;
        }

        public Payload SetAmout(double amount)
        {
            if (amount != 0)
            {
                var nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "" };
                Amount = amount.ToString("#0.00", nfi);
            }
            return this;
        }

        public Payload SetUniquePayment(bool uniquePayment)
        {
            UniquePayment = uniquePayment;
            return this;
        }

        public Payload SetUrl(string url)
        {
            Url = url;
            return this;
        }

        private string GetValue(string id, string value)
        {
            return $"{id}{value.Length.ToString().PadLeft(2, '0')}{value}";
        }

        private string GetMechantInformation()
        {

            var gui = GetValue(PayloadId.ID_MERCHANT_ACCOUNT_INFORMATION_GUI, "br.gov.bcb.pix");
            var key = string.IsNullOrEmpty(PixKey) ? string.Empty : GetValue(PayloadId.ID_MERCHANT_ACCOUNT_INFORMATION_KEY, PixKey);
            var description = string.IsNullOrEmpty(Description) ? string.Empty : GetValue(PayloadId.ID_MERCHANT_ACCOUNT_INFORMATION_DESCRIPTION, Description);
            var url = string.IsNullOrEmpty(Url) ? string.Empty : GetValue(PayloadId.ID_MERCHANT_ACCOUNT_INFORMATION_URL, Url);

            return GetValue(PayloadId.ID_MERCHANT_ACCOUNT_INFORMATION, gui + key + description + url);
        }

        private string GetAdditionalDataField()
        {
            if (string.IsNullOrEmpty(Txid))
            {
                return string.Empty;
            }

            var txid = GetValue(PayloadId.ID_ADDITIONAL_DATA_FIELD_TEMPLATE_TXID, Txid);

            return GetValue(PayloadId.ID_ADDITIONAL_DATA_FIELD_TEMPLATE, txid);
        }

        private string GetAmount()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                return string.Empty;
            }
            return GetValue(PayloadId.ID_TRANSACTION_AMOUNT, Amount);
        }

        private string GetUniquePayment()
        {
            if (!UniquePayment)
            {
                return string.Empty;
            }
            return GetValue(PayloadId.ID_POINT_OF_INITIATION_METHOD, "12");
        }

        private string GetCrc16(string payload)
        {
            payload = payload + PayloadId.ID_CRC16 + "04";
            var crc = payload.CalcCRC16();
            return payload + crc;
        }

        public string GetPayload()
        {
            var payload = GetValue(PayloadId.ID_PAYLOAD_FORMAT_INDICATOR, "01") +
                          GetUniquePayment() +
                          GetMechantInformation() +
                          GetValue(PayloadId.ID_MERCHANT_CATEGORY_CODE, "0000") +
                          GetValue(PayloadId.ID_TRANSACTION_CURRENCY, "986") +
                          GetAmount() +
                          GetValue(PayloadId.ID_COUNTRY_CODE, "BR") +
                          GetValue(PayloadId.ID_MERCHANT_NAME, MerchantName) +
                          GetValue(PayloadId.ID_MERCHANT_CITY, MerchantCity) +
                          GetAdditionalDataField();


            return GetCrc16(payload);
        }

    }
}
