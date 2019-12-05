using System.Collections.Generic;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;

namespace clearlyApi.Services.Chat
{
    public class MessageDTO<T>
    {
        public MessageDTO(Message message)
        {
            Type = message.Type;
            IsAdmin = message.IsFromAdmin;
        }

        public bool IsAdmin { get; set; }
        public MessageType Type { get; set; }
        public T Data { get; set; }
    }

    public class PackagesList
    {
        public List<PackageDTOResponse> Packages { get; set; }
        public int OrderId { get; set; }
    }
}
