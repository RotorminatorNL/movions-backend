using DataAccessLayer;
using Domain;
using System.Collections.Generic;

namespace BusinessLogicLayer
{
    public class CreateCompany
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CreateCompany(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public bool Do(string name, List<Movie> movies)
        {
            bool status = false;

            try
            {
                _applicationDbContext.Companies.Add(new Company
                {
                    Name = name,
                    Movies = movies
                });
                status = true;
            }
            catch (System.Exception)
            {

            }

            return status;
        }
    }
}
