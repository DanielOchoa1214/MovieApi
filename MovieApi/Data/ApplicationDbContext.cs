using System;
using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Data
{
	public class ApplicationDbContext: DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		// Add models here (They are the tables in the DB)
		public DbSet<Category> Category { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<User> User { get; set; }
    }
}

