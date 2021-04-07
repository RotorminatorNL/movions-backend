using DataAccessLayer;
using DataAccessLayerInterface;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer
{
    public class Company
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public Company(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public bool Create(string name, List<Domain.MovieCompany> movies)
        {
            bool status = false;

            try
            {
                _applicationDbContext.Companies.Add(new Domain.Company
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

        public IEnumerable<Domain.Company> ReadAll()
        {
            return _applicationDbContext.Companies.ToList();
        }

        public Domain.Company Read(int id)
        {
            return _applicationDbContext.Companies.Where(c => c.ID == id).FirstOrDefault();
        }

        public void Update(int id, string name)
        {
            _applicationDbContext.Companies.Update(new Domain.Company
            {
                ID = id,
                Name = name
            });
        }

        public void Delete(int id)
        {
            _applicationDbContext.Companies.Remove(Read(id));
        }
    }
}
