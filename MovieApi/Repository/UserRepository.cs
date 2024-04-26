using System;
using System.Drawing;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;
using XSystem.Security.Cryptography;

namespace MovieApi.Repository
{
	public class UserRepository: IUserRepository
	{
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration config)
		{
            _db = db;
            secretKey = config.GetValue<string>("ApiSettings:secret");
        }

        public User GetUser(int userId)
        {
            return _db.User.FirstOrDefault(u => u.Id == userId);
        }

        public ICollection<User> GetUsers()
        {
            return _db.User.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string user)
        {
            var dbUser = _db.User.FirstOrDefault(u => u.UserName == user);
            return dbUser == null;
        }

        public async Task<User> Register(RegisterUserDTO registerUserDTO)
        {
            var encryptedPassword = GetMd5(registerUserDTO.Password);

            User user = new()
            {
                UserName = registerUserDTO.UserName,
                Password = encryptedPassword,
                Name = registerUserDTO.Name,
                Role = registerUserDTO.Role
            };

            _db.User.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<LoginUserResponseDTO> Login(LoginUserDTO loginUserDTO)
        {
            var encryptedPassword = GetMd5(loginUserDTO.Password);
            var user = _db.User.FirstOrDefault(u =>
                u.UserName.ToLower() == loginUserDTO.UserName.ToLower() &&
                u.Password == encryptedPassword
            );

            if (user == null)
            {
                return new LoginUserResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToLower()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginUserResponseDTO loginUserResponseDTO = new()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };

            return loginUserResponseDTO;
        }

        private static string GetMd5(string value)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}

