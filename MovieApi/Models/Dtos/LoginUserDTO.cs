using System;
using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models.Dtos
{
	public class LoginUserDTO
	{
        [Required(ErrorMessage = "UserName field is mandatory! >:(")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password field is mandatory! >:(")]
        public string Password { get; set; }
    }
}

