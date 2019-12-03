using clearlyApi.Entities;
using clearlyApi.Enums;

namespace clearlyApi.Services.Chat
{
    public class MessageDTO
    {
        public MessageDTO(Message message)
        {
            Type = message.Type;
            Text = message.Content;

            IsAdmin = message.AdminId == 0 ? false : true;
        }

        public bool IsAdmin { get; set; }
        public MessageType Type { get; set; }
        public string Text { get; set; }
    }
}
