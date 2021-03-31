using DataAccessLayer;
using BusinessContractLayer;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer
{
    public class ReadAllCompanies
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReadAllCompanies(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Company> Do()
        {
            return _applicationDbContext.Companies.ToList();
        }
    }
}
