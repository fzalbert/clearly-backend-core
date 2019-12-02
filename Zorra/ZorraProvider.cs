using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utils;
using Zorra.DTO;

namespace Zorra
{
    public class ZorraProvider : IMessageProvider
    {

        public async Task<bool> SendSMS(string to, string text)
        {

            var requestBody = new SingleSMSRequest()
            {
                Type = "sms",
                Sender = "Dostavim",
                Recipient = to,
                Body = text
            };

            var result = await HttpHelper.ServicePostRequestJSON(
                ApiConfiguration.SINGLE_SMS_MESSAGE,
                JsonConvert.SerializeObject(requestBody),
                $"Bearer {ApiConfiguration.Token}"
            );
            if (result.Status != 200)
                return false;

            var status = (bool)JObject.Parse(result.Result)["success"];

            return status == true;

        }

        private static class ApiConfiguration
        {
            public static readonly string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwOlwvXC9s" +
                "b2NhbGhvc3Q6ODAwMFwvYXBpXC9hdXRoXC9sb2dpbiIsImlhdCI6MTU1NjM2MDc2OCwiZXhwIjoxNTU2Mzk" +
                "2NzY4LCJuYmYiOjE1NTYzNjA3NjgsImp0aSI6ImdlcUxLZ2VxVnQyRGVwWFciLCJzdWIiOjI2MiwicHJ2IjoiMjN" +
                "iZDVjODk0OWY2MDBhZGIzOWU3MDFjNDAwODcyZGI3YTU5NzZmNyJ9.wDeXQrZeWBDuDk1IJ7ncwmy-Msg2srhv-JdOqnbrzYw";

            static readonly string URL = "https://my.zorra.com/api/v2";

            public readonly static string SINGLE_SMS_MESSAGE = $"{URL}/mailing/single/send";
        }
    }
}
