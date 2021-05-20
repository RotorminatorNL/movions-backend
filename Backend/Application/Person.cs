using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Person
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public Person(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminPersonModel> Create(AdminPersonModel adminPersonModel)
        {
            var person = new Domain.Person
            {
                BirthDate = adminPersonModel.BirthDate.ToShortDateString(),
                BirthPlace = adminPersonModel.BirthPlace,
                Description = adminPersonModel.Description,
                FirstName = adminPersonModel.FirstName,
                LastName = adminPersonModel.LastName
            };

            _applicationDbContext.Persons.Add(person);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminPersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName
            };
        }

        public PersonModel Read(int id)
        {
            return _applicationDbContext.Persons.ToList().Select(person => new PersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName,
            }).FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<PersonModel> ReadAll()
        {
            return _applicationDbContext.Persons.ToList().Select(person => new PersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName,
            });
        }

        public async Task<AdminPersonModel> Update(AdminPersonModel adminPersonModel)
        {
            var person = _applicationDbContext.Persons.FirstOrDefault(x => x.ID == adminPersonModel.ID);

            person.BirthDate = adminPersonModel.BirthDate.ToShortDateString();
            person.BirthPlace = adminPersonModel.BirthPlace;
            person.Description = adminPersonModel.Description;
            person.FirstName = adminPersonModel.FirstName;
            person.LastName = adminPersonModel.LastName;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminPersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName
            };
        }

        public async Task<bool> Delete(int id)
        {
            var person = _applicationDbContext.Persons.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Persons.Remove(person);

            try
            {
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
