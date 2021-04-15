using BusinessLogicLayer;
using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CrewRoleController : Controller
    {
        private readonly CrewRole Genre;

        public CrewRoleController(ApplicationDbContext applicationDbContext)
        {
            Genre = new CrewRole(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminCrewRoleModel adminCrewRoleModel)
        {
            return Ok(await Genre.Create(adminCrewRoleModel));
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
        public async Task<IActionResult> Update(AdminCrewRoleModel adminCompanyModel)
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
