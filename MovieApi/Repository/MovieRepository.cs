using System;
using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repository.IRepository;

namespace MovieApi.Repository
{
	public class MovieRepository : IMovieRepository
	{
        private readonly ApplicationDbContext _db;

        public MovieRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool MovieExists(string name)
        {
            return _db.Movie.Any(m => m.Name.ToLower().Trim() == name.ToLower().Trim());

        }

        public bool MovieExists(int id)
        {
            return _db.Movie.Any(m => m.Id == id);
        }

        public bool CreateMovie(Movie movie)
        {
            movie.CreationDate = DateTime.Now;
            _db.Movie.Add(movie);
            return Save();
        }

        public bool DeleteMovie(Movie movie)
        {
            _db.Movie.Remove(movie);
            return Save();
        }

        public ICollection<Movie> GetMovies()
        {
            return _db.Movie.OrderBy(c => c.Name).ToList();
        }

        public Movie GetMovie(int movieId)
        {
            return _db.Movie.FirstOrDefault(c => c.Id == movieId);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public bool UpdateMovie(Movie movie)
        {
            movie.CreationDate = DateTime.Now;
            _db.Movie.Update(movie);
            return Save();
        }

        public ICollection<Movie> GetMoviesByName(string name)
        {
            IQueryable<Movie> query = _db.Movie;
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name) || e.Description.Contains(name));
            }
            return query.ToList();
        }

        public ICollection<Movie> GetMoviesInCategory(int categoryId)
        {
            return _db.Movie.Include(ca => ca.Category).Where(ca => ca.CategoryId == categoryId).ToList();
        }
    }
}

