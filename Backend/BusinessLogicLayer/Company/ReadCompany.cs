using DataAccessLayer;
using Domain;
using System.Linq;

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
            return _applicationDbContext.Companies.Where(c => c.ID == id).FirstOrDefault();
        }
    }
}
