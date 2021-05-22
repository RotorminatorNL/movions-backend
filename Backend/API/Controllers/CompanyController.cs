﻿using Application;
using Microsoft.AspNetCore.Mvc;
using PersistenceInterface;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CompanyController : Controller
    {
        private readonly Company Company;

        public CompanyController(IApplicationDbContext applicationDbContext)
        {
            Company = new Company(applicationDbContext);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(AdminCompanyModel adminCompanyModel)
        {
            return Ok(await Company.Create(adminCompanyModel));
        }

        [HttpGet("{id}")]
        public IActionResult Read(int id)
        {
            return Ok(Company.Read(id));
        }

        [HttpGet()]
        public IActionResult ReadAll()
        {
            return Ok(Company.ReadAll());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(AdminCompanyModel adminCompanyModel)
        {
            return Ok(await Company.Update(adminCompanyModel));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Company.Delete(id));
        }
    }
}
