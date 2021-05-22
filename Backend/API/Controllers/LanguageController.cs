using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LanguageController : Controller
    {
        private readonly Language Language;

        public LanguageController(IApplicationDbContext applicationDbContext)
        {
            Language = new Language(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminLanguageModel adminLanguageModel)
        {
            return Ok(await Language.Create(adminLanguageModel));
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Language.Read(id));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Language.ReadAll());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(AdminLanguageModel adminCompanyModel)
        {
            return Ok(await Language.Update(adminCompanyModel));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Language.Delete(id));
        }
    }
}
