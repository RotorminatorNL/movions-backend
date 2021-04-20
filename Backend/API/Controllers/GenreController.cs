using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GenreController : Controller
    {
        private readonly Genre Genre;

        public GenreController(IApplicationDbContext applicationDbContext)
        {
            Genre = new Genre(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminGenreModel adminGenreModel)
        {
            return Ok(await Genre.Create(adminGenreModel));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Genre.ReadAll());
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Genre.Read(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(AdminGenreModel adminCompanyModel)
        {
            return Ok(await Genre.Update(adminCompanyModel));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Genre.Delete(id));
        }
    }
}
