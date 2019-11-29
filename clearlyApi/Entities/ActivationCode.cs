using System;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Entities
{
    public class ActivationCode : PersistantObject
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public String Code { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
    }
}
