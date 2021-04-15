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
    public class LanguageController : Controller
    {
        private readonly Language Language;

        public LanguageController(ApplicationDbContext applicationDbContext)
        {
            Language = new Language(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminLanguageModel adminLanguageModel)
        {
            return Ok(await Language.Create(adminLanguageModel));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Language.ReadAll());
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Language.Read(id));
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
