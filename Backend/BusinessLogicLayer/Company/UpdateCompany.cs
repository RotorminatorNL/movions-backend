using DataAccessLayer;
using Domain;

namespace BusinessLogicLayer
{
    public class UpdateCompany
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UpdateCompany(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Do(int id, string name)
        {
            _applicationDbContext.Companies.Update(new Company
            {
                ID = id,
                Name = name
            });
        }
    }
}
