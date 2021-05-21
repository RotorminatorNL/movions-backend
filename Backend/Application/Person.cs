using Application.Validation;
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
        private readonly PersonValidation _personValidation;

        public Person(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _personValidation = new PersonValidation();
        }

        public async Task<AdminPersonModel> Create(AdminPersonModel adminPersonModel)
        {
            if (_personValidation.IsInputValid(adminPersonModel))
            {
                var person = new Domain.Person
                {
                    BirthDate = adminPersonModel.BirthDate.ToString("dd-MM-yyyy"),
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

            return null;
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

            if (person != null && _personValidation.IsInputValid(adminPersonModel))
            {
                if (_personValidation.IsInputDifferent(person, adminPersonModel))
                {
                    person.BirthDate = adminPersonModel.BirthDate.ToString("dd-MM-yyyy");
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

                return new AdminPersonModel();
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var person = _applicationDbContext.Persons.FirstOrDefault(x => x.ID == id);

            if (person != null)
            {
                _applicationDbContext.Persons.Remove(person);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
