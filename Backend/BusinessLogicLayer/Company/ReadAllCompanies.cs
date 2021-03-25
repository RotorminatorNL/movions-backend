using DataAccessLayer;
using BusinessContractLayer;
using System.Collections.Generic;

namespace BusinessLogicLayer
{
    public class ReadAllCompanies
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReadAllCompanies(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public List<Company> Do()
        {
            List<Company> companies = null;

            companies = new List<Company> {
                new Company { ID = 1, Name = "New Republic Pictures" },
                new Company { ID = 2, Name = "Paramount Pictures"}
            };

            return companies;
        }
    }
}
