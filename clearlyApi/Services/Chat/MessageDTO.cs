using System;
using System.Collections.Generic;
using System.Linq;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;
using Newtonsoft.Json;

namespace clearlyApi.Services.Chat
{
    public class MessageDTO
    {
        public MessageDTO(Message message)
        {
            Type = message.Type;
            IsAdmin = message.IsFromAdmin;
            Created = message.Created;
            Data = message.Content;
        }

        public MessageDTO(Message message, List<Package> packages)
        {
            Type = message.Type;
            IsAdmin = message.IsFromAdmin;
            Created = message.Created;

            Int32.TryParse(message.Content, out int orderId);

            Data = JsonConvert.SerializeObject(
                    new PackagesList()
                    {
                        OrderId = orderId,
                        Packages = packages.Select(x => new PackageDTOResponse(x)).ToList()
                    });

        }


        public bool IsAdmin { get; set; }
        public MessageType Type { get; set; }
        public string Data { get; set; }

        public DateTime Created { get; set; }
    }

    public class PackagesList
    {
        public List<PackageDTOResponse> Packages { get; set; }
        public int OrderId { get; set; }
    }
}
