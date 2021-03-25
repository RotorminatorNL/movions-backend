using DataAccessLayer;
using BusinessContractLayer;

namespace BusinessLogicLayer
{
    public class ReadCompany
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReadCompany(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Company Do(int id)
        {
            Company company = null;

            if (id == 1) return new Company { ID = id, Name = "New Republic Pictures" };
            if (id == 2) return new Company { ID = id, Name = "Paramount Pictures" };

            return company;
        }
    }
}
