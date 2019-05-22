using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using shortid;


namespace Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public IEnumerable<Url> Urls { get; set; }
    }

    public class Url
    {
        public string CompleteUrl { get; set; }
        public int Id { get; set; }
        public string UrlHash { get; set; }

        public int UserId { get; set; }
    }

    public class UrlContext : DbContext
    {
        private string _conn;
        public UrlContext(string conn)
        {
            _conn = conn;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Url> Urls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conn);
        }
    }

    public class UrlContextFactory : IDesignTimeDbContextFactory<UrlContext>
    {
        public UrlContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}5.20.19"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new UrlContext(config.GetConnectionString("ConStr"));
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        //    {
        //        relationship.DeleteBehavior = DeleteBehavior.Restrict;
        //    }

        //    modelBuilder.Entity<UserLikedJokes>()
        //        .HasKey(ulj => new { ulj.UserId, ulj.JokeId });


        //    modelBuilder.Entity<UserLikedJokes>()
        //        .HasOne(ulj => ulj.User)
        //        .WithMany(u => u.UserLikedJokes)
        //        .HasForeignKey(u => u.UserId);

        //    modelBuilder.Entity<UserLikedJokes>()
        //        .HasOne(ulj => ulj.Joke)
        //        .WithMany(j => j.UserLikedJokes)
        //        .HasForeignKey(ulj => ulj.JokeId);
        //}
    }

    public class UrlRepository
    {
        private string _conn;
        public UrlRepository(string conn)
        {
            _conn = conn;
        }

        public void AddUser(User u)
        {
            using (var context = new UrlContext(_conn))
            {
                var user = new User
                {
                    Name = u.Name,
                    Email = u.Email,
                    Password = HashPassword(u.Password)
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Match(string input, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(input, passwordHash);
        }

        public User GetUserByEmail(string email)
        {
            using (var context = new UrlContext(_conn))
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public string Hash(string urlString)
        {
            string id = ShortId.Generate(8);
            return id;
        }

        public string AddUrl(string completeUrl, int userId)
        {
            using (var context = new UrlContext(_conn))
            {

                string urlHash = Hash(completeUrl);

                while (HashExists(urlHash))
                {
                    urlHash = Hash(urlHash);
                }
                bool exists= context.Urls.Any(u => u.CompleteUrl == completeUrl);
                if (!exists)
                {
                    var url = new Url
                    {
                        CompleteUrl = completeUrl,
                        UrlHash = urlHash,
                        UserId = userId
                    };
                    context.Urls.Add(url);
                    context.SaveChanges();
                }
                                                   
                return urlHash;
            }
        }


        public bool HashExists(string urlHash)
        {
            using (var context = new UrlContext(_conn))
            {
                return context.Urls.Any(u => u.UrlHash == urlHash);
            }
        }

        public IEnumerable<Url> GetUrlsForUser(string email)
        {
            using (var context = new UrlContext(_conn))
            {
                var user = GetUserByEmail(email);
                return context.Urls.Where(u => u.UserId==user.Id).ToList();
            }
        }

        public string GetCompleteUrl(string urlHash)
        {
            using (var context = new UrlContext(_conn))
            {
                Url url = context.Urls.FirstOrDefault(u => u.UrlHash == urlHash);
                string completeUrl=url.CompleteUrl;
                return completeUrl;
            }
        }

    }
}
