using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;

namespace MovieApi.Controllers
{
	[Route("api/movie")]
	[ApiController]
	public class MoviesController: ControllerBase
	{
        private readonly IMovieRepository _movRepo;
        private readonly IMapper _mapper;

        public MoviesController(IMovieRepository movRepo, IMapper mapper)
        {
            _movRepo = movRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMovies()
        {
            var moviesList = _movRepo.GetMovies();
            var moviesListDTO = new List<MovieDTO>();

            foreach (var movie in moviesList)
            {
                moviesListDTO.Add(_mapper.Map<MovieDTO>(movie));
            }
            return Ok(moviesListDTO);
        }

        [AllowAnonymous]
        [HttpGet("{movieId:int}", Name = "GetMovie")]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMovie(int movieId)
        {
            var movieItem = _movRepo.GetMovie(movieId);

            if (movieItem == null) return NotFound();

            var movieItemDTO = _mapper.Map<MovieDTO>(movieItem);
            return Ok(movieItemDTO);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(MovieDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie([FromBody] MovieDTO movieDTO)
        {
            if (!ModelState.IsValid || movieDTO == null) return BadRequest(ModelState);

            if (_movRepo.MovieExists(movieDTO.Name))
            {
                ModelState.AddModelError("", "Movie already exists");
                return StatusCode(404, ModelState);
            }

            var movie = _mapper.Map<Movie>(movieDTO);
            if (!_movRepo.CreateMovie(movie))
            {
                ModelState.AddModelError("", $"Something went wrong, I broke :( {movie.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetMovie", new { movieId = movie.Id }, movie);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{movieId:int}", Name = "UpdatePatchMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult UpdatePatchMovie(int movieId, [FromBody] MovieDTO movieDTO)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var movie = _mapper.Map<Movie>(movieDTO);

            if (!_movRepo.UpdateMovie(movie))
            {
                ModelState.AddModelError("", $"Something went wrong updating, I broke :( {movie.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{movieId:int}", Name = "DeleteMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteMovie(int movieId)
        {

            if (!_movRepo.MovieExists(movieId)) return NotFound();

            var movie = _movRepo.GetMovie(movieId);


            if (!_movRepo.DeleteMovie(movie))
            {
                ModelState.AddModelError("", $"Something went wrong deleting, I broke :( {movie.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("name")]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMoviesByName(string movieName)
        {
            try
            {
                var result = _movRepo.GetMoviesByName(movieName.Trim());
                if (result.Any()) return Ok(result);

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error finding the movie, sorry :(");
            }
            
        }

        [AllowAnonymous]
        [HttpGet("category/{categoryId:int}")]
        [ResponseCache(CacheProfileName = "20SecondsDefect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMoviesInCategory(int categoryId)
        {
            var moviesList = _movRepo.GetMoviesInCategory(categoryId);

            if (moviesList == null) return NotFound();

            var moviesListDTO = new List<MovieDTO>();
            foreach (var movie in moviesList)
            {
                moviesListDTO.Add(_mapper.Map<MovieDTO>(movie));
            }
            return Ok(moviesListDTO);
        }


    }
}

