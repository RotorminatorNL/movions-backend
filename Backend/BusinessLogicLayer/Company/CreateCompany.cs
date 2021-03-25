using DataAccessLayer;
using BusinessContractLayer;

namespace BusinessLogicLayer
{
    public class CreateCompany
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CreateCompany(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Do(int id, string name)
        {
            _applicationDbContext.Companies.Add(new BusinessContractLayer.Company
            {
                ID = id,
                Name = name
            });
        }
    }
}
