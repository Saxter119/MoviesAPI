using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using System.Security.Claims;

namespace MoviesAPI
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesGenders>().HasKey(x=> new {x.MovieId, x.GenderId});

            modelBuilder.Entity<MoviesActors>().HasKey(x=> new {x.MovieId, x.ActorId});

            modelBuilder.Entity<MoviesRoomMovies>(x=> x.HasKey(x=> new { x.MovieId, x.RoomMovieId}));

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Gender> Genders { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<RoomMovie> RoomMovies { get; set; }
        public DbSet<MoviesGenders> MoviesGenders { get; set; }
        public DbSet<MoviesActors> MoviesActors { set; get; }
        public DbSet<MoviesRoomMovies> MoviesRoomMovies { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
