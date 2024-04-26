using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MovieApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController: ControllerBase
	{
        private readonly IUserRepository _usrRepo;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public UsersController(IUserRepository usrRepo, IMapper mapper)
        {
            _usrRepo = usrRepo;
            _apiResponse = new();
            _mapper = mapper;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetUsers()
        {
            var usersList = _usrRepo.GetUsers();
            var usersListDTO = new List<UserDTO>();

            foreach (var user in usersList)
            {
                usersListDTO.Add(_mapper.Map<UserDTO>(user));
            }
            return Ok(usersListDTO);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{userId:int}", Name = "GetUser")]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int userId)
        {
            var userItem = _usrRepo.GetUser(userId);

            if (userItem == null) return NotFound();

            var userItemDTO = _mapper.Map<UserDTO>(userItem);
            return Ok(userItemDTO);
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO registerUserDTO)
        {
            bool validateUniqueUserName = _usrRepo.IsUniqueUser(registerUserDTO.UserName);
            if(!validateUniqueUserName)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("The username already exists");
                return BadRequest(_apiResponse);
            }

            var user = await _usrRepo.Register(registerUserDTO);
            if(user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Error registering user");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }

        [AllowAnonymous]
        [HttpGet("auth")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromQuery(Name = "username")] string username, [FromQuery(Name = "password")] string password)
        {
            LoginUserDTO loginUserDTO = new()
            {
                UserName = username,
                Password = password
            };

            var loginResponse = await _usrRepo.Login(loginUserDTO);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username or password are incorrect :(");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = loginResponse;
            return Ok(_apiResponse);
        }
    }
}

