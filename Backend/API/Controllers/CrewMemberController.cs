using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CrewMemberController : Controller
    {
        private readonly CrewMember Genre;

        public CrewMemberController(IApplicationDbContext applicationDbContext)
        {
            Genre = new CrewMember(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminCrewMemberModel adminCrewRoleModel)
        {
            return Ok(await Genre.Create(adminCrewRoleModel));
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Genre.Read(id));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Genre.ReadAll());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(AdminCrewMemberModel adminCompanyModel)
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
