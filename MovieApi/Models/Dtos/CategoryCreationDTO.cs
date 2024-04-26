using System;
using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models.Dtos
{
	public class CategoryCreationDTO
	{
        [Required(ErrorMessage = "Name field if required")]
        [MaxLength(60, ErrorMessage = "Max characters for the name is 60!")]
        public string Name { get; set; }
    }
}

