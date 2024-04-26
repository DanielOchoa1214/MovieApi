using System;
using MovieApi.Models;
using MovieApi.Models.Dtos;

namespace MovieApi.Repository.IRepository
{
	public interface IUserRepository
	{
        ICollection<User> GetUsers();
        User GetUser(int userId);
        bool IsUniqueUser(string user);
        Task<LoginUserResponseDTO> Login(LoginUserDTO loginUserDTO);
        Task<User> Register(RegisterUserDTO registerUserDTO);
    }
}

