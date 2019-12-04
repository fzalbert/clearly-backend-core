using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Entities
{
    public class Message : PersistantObject
    {
        public Enums.MessageType Type { get; set; }

        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DefaultValue(false)]
        public bool IsFromAdmin { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int AdminId { get; set; }
        public User Admin { get; set; }

        public List<Package> Packages { get; set; }
    }
}
