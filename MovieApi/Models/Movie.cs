using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApi.Models
{
	public class Movie
	{
        [Key]
        public int Id { get; set; }
		public string Name { get; set; }
		public string ImageRoute { get; set; }
		public string Description { get; set; }
		public int Duration { get; set; }
		public enum ClasificationType { SevenPlus, ThirteenPlus, SixteenPlus, EighteenPlus }
		public ClasificationType Clasification { get; set; }
		public DateTime CreationDate { get; set; }
		[ForeignKey("categoryId")]
		public int CategoryId { get; set; }
		public Category Category { get; set; }
	}
}

