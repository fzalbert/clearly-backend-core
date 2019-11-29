using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using clearlyApi.Enums;

namespace clearlyApi.Entities
{
    public class Person : PersistantObject
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [Required]
        [DefaultValue(0)]
        public Sex Sex { get; set; }

        public int Age { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
