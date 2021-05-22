using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PersonController : Controller
    {
        private readonly Person Person;

        public PersonController(IApplicationDbContext applicationDbContext)
        {
            Person = new Person(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminPersonModel adminPersonModel)
        {
            return Ok(await Person.Create(adminPersonModel));
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Person.Read(id));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Person.ReadAll());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(AdminPersonModel adminCompanyModel)
        {
            return Ok(await Person.Update(adminCompanyModel));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Person.Delete(id));
        }
    }
}
