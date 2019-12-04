using System.Collections.Generic;
using System.Linq;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;
using Newtonsoft.Json;

namespace clearlyApi.Services.Chat
{
    public class MessageDTO<T>
    {
        public MessageDTO(Message message)
        {
            Type = message.Type;

            if (message.Type == MessageType.Text || message.Type == MessageType.Photo)
                Data = message.Content;
            else
                Data = null;

            IsAdmin = message.AdminId == 0 ? false : true;
        }

        public MessageDTO(Message messge, List<Package> packages) : this(messge)
        {
            
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
