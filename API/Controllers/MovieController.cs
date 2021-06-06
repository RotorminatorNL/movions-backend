using Application;
using Application.AdminModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
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
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");

            movie = new Movie(applicationDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminMovieModel adminMovieModel)
        {
            if (await movie.Create(adminMovieModel) is MovieModel result && result != null)
            {
                return CreatedAtAction(nameof(Read), new { id = result.ID }, result);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
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
            if (await movie.Update(adminMovieModel) is MovieModel result && result != null)
            {
                return Ok(result);
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

        [HttpPost("{id}/genres/{genreID}")]
        public async Task<IActionResult> DisconnectGenre(int id, int genreID)
        {
            var result = await movie.DisconnectGenre(new AdminGenreMovieModel { GenreID = genreID, MovieID = id });

            if (result != null)
            {
                if (result.ID > 0)
                {
                    return Ok(result);
                }

                return GetCustomNotFound(result.ID, "disconnect");
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
