using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using clearlyApi.Enums;

namespace clearlyApi.Entities
{
    public class User : PersistantObject
    {
        public string Login { get; set; }

        public LoginType LoginType { get; set; }

        [Required]
        [DefaultValue(UserType.User)]
        public UserType UserType { get; set; }

        [DefaultValue(false)]
        public Boolean IsActive { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        public Person Person { get; set; }

        public List<Message> UserMessages { get; set; }

        public List<Message> AdminMessages { get; set; }
    }
}
