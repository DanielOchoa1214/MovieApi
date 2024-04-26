using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApi.Models.Dtos
{
	public class MovieDTO
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Name field if required")]
        public string Name { get; set; }

        public string ImageRoute { get; set; }

        [Required(ErrorMessage = "Description field if required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration field if required")]
        public int Duration { get; set; }

        public enum ClasificationType { SevenPlus, ThirteenPlus, SixteenPlus, EighteenPlus }
        public ClasificationType Clasification { get; set; }

        public DateTime CreationDate { get; set; }

        public int CategoryId { get; set; }
    }
}

