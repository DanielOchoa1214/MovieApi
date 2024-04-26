using System;
using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models.Dtos
{
	public class RegisterUserDTO
	{
        [Required(ErrorMessage = "UserName field is mandatory! >:(")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name field is mandatory! >:(")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password field is mandatory! >:(")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

