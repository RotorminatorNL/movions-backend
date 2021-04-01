using DataAccessLayer;
using Domain;

namespace BusinessLogicLayer
{
    public class DeleteCompany
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DeleteCompany(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Do(int id)
        {
            _applicationDbContext.Companies.Remove(new ReadCompany(_applicationDbContext).Do(id));
        }
    }
}
