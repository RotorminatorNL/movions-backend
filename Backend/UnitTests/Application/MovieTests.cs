using Application;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class MovieTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public MovieTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "movions_movies")
                .Options;
        }

        [Fact]
        public async Task ReadAll()
        {
            // Arrange 
            var dbContext = new ApplicationDbContext(_dbContextOptions);

            dbContext.Movies.AddRange(
                Enumerable.Range(1, 5).Select(m => new Domain.Movie { ID = m, Description = $"Description {m}" })
            );

            await dbContext.SaveChangesAsync();

            var movie = new Movie(dbContext);

            // Act
            var result = movie.ReadAll();

            // Assert
            var movieModel = Assert.IsAssignableFrom<IEnumerable<MovieModel>>(result);
            Assert.Equal(5, movieModel.Count());
        }
    }
}
