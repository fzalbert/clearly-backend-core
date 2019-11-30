using System;
using System.ComponentModel.DataAnnotations;

namespace clearlyApi.Dto.Request
{
    public class AuthRequest
    {
        [Required]
        public string Login { get; set; }

        public Enums.LoginType Type { get; set; }

        public string Code { get; set; }
    }
}
