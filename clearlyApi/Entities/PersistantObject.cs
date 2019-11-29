using System;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Entities
{
    public abstract class PersistantObject
    {
        [Key]
        public int Id { get; set; }
    }
}
