using System;
using MovieApi.Models;

namespace MovieApi.Repository.IRepository
{
	public interface IMovieRepository
	{
        ICollection<Movie> GetMovies();
        Movie GetMovie(int movieId);
        bool MovieExists(string name);
        bool MovieExists(int id);
        bool CreateMovie(Movie movie);
        bool UpdateMovie(Movie movie);
        bool DeleteMovie(Movie movie);

        ICollection<Movie> GetMoviesByName(string name);
        ICollection<Movie> GetMoviesInCategory(int categoryId);

        bool Save();
    }
}

