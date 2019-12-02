using System;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Dto.Request
{
    public class MessageRequest
    {
        [Required]
        public string Text { get; set; }

        public int UserId { get; set; }
    }
}
