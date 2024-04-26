using System;
using AutoMapper;
using MovieApi.Models;
using MovieApi.Models.Dtos;

namespace MovieApi.MovieMapper
{
	public class MovieMapper: Profile
	{
		public MovieMapper()
		{
			CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryCreationDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, LoginUserDTO>().ReverseMap();
            CreateMap<User, LoginUserResponseDTO>().ReverseMap();
            CreateMap<User, RegisterUserDTO>().ReverseMap();
        }
	}
}

