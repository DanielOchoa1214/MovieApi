using System;
namespace MovieApi.Models.Dtos
{
	public class LoginUserResponseDTO
	{
		public User User { get; set; }
		public string Token { get; set; }
	}
}

