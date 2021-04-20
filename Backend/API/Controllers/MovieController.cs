﻿using BusinessLogicLayer;
using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MovieController : Controller
    {
        private readonly Movie Movie;

        public MovieController(IApplicationDbContext applicationDbContext)
        {
            Movie = new Movie(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminMovieModel adminMovieModel)
        {
            return Ok(await Movie.Create(adminMovieModel));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Movie.ReadAll());
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Movie.Read(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(AdminMovieModel adminMovieModel)
        {
            return Ok(await Movie.Update(adminMovieModel));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Movie.Delete(id));
        }
    }
}
