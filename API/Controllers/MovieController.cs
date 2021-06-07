using Application;
using Application.AdminModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {
        private readonly Movie movie;

        private NotFoundObjectResult GetCustomNotFound(int id, string function)
        {
            switch (id)
            {
                case -1:
                    {
                        ModelState.AddModelError("GenreID", "Does not exist.");
                        break;
                    }
                case -2:
                    {
                        ModelState.AddModelError("MovieID", "Does not exist.");
                        break;
                    }
                case -3:
                    {
                        ModelState.AddModelError("GenreID", "Does not exist.");
                        ModelState.AddModelError("MovieID", "Does not exist.");
                        break;
                    }
                default:
                    if (function == "connect")
                    {
                        ModelState.AddModelError("GenreMovieID", "Does already exist.");
                    }
                    else
                    {
                        ModelState.AddModelError("GenreMovieID", "Does not exist.");
                    }
                    break;
            }

            return NotFound(new ValidationProblemDetails(ModelState));
        }

        public MovieController(IApplicationDbContext applicationDbContext)
        {
            movie = new Movie(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminMovieModel adminMovieModel)
        {
            var result = await movie.Create(adminMovieModel);

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
                }

                ModelState.AddModelError("LanguageID", "Does not exist.");
                return NotFound(new ValidationProblemDetails(ModelState));
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost("{id}/genres")]
        public async Task<IActionResult> ConnectGenre(int id, [FromBody] int genreID)
        {
            var result = await movie.ConnectGenre(new AdminGenreMovieModel { GenreID = genreID, MovieID = id });

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return Ok(result);
                }

                return GetCustomNotFound(result.ID, "connect");
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            if (await movie.Read(id) is MovieModel result && result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            if (await movie.ReadAll() is ICollection<MovieModel> result && result.Count > 0)
            {
                return Ok(result);
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AdminMovieModel adminMovieModel)
        {
            var result = await movie.Update(adminMovieModel);

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return Ok(result);
                }

                ModelState.AddModelError("LanguageID", "Does not exist.");
                return NotFound(new ValidationProblemDetails(ModelState));
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await movie.Delete(id))
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}/genres/{genreID}")]
        public async Task<IActionResult> DisconnectGenre(int id, int GenreID)
        {
            var result = await movie.DisconnectGenre(new AdminGenreMovieModel { GenreID = GenreID, MovieID = id });

            if (result != null)
            {
                if (result.ID == 0)
                {
                    return Ok(result);
                }

                return GetCustomNotFound(result.ID, "disconnect");
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
