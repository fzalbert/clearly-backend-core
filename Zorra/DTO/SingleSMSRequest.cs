using System;
namespace Zorra.DTO
{
    public class SingleSMSRequest
    {
        public string Type { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public string Recipient { get; set; }
    }
}
