using System;
using System.Threading.Tasks;

namespace Zorra
{
    public interface IMessageProvider
    {
        public Task<bool> SendSMS(String to, String text);
    }
}
