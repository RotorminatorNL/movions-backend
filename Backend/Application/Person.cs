using Application.AdminModels;
using Application.Validation;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;
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

        public async Task<PersonModel> Create(AdminPersonModel adminPersonModel)
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

                return await Read(person.ID);
            }

            return null;
        }

        public async Task<PersonModel> Read(int id)
        {
            return await _applicationDbContext.Persons.Select(person => new PersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName,
            }).FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<PersonModel>> ReadAll()
        {
            return await _applicationDbContext.Persons.Select(person => new PersonModel
            {
                ID = person.ID,
                BirthDate = DateTime.Parse(person.BirthDate),
                BirthPlace = person.BirthPlace,
                Description = person.Description,
                FirstName = person.FirstName,
                LastName = person.LastName,
            }).ToListAsync();
        }

        public async Task<PersonModel> Update(AdminPersonModel adminPersonModel)
        {
            var person = _applicationDbContext.Persons.FirstOrDefault(x => x.ID == adminPersonModel.ID);

            if (person != null && _personValidation.IsInputValid(adminPersonModel))
            {
                person.BirthDate = adminPersonModel.BirthDate.ToString("dd-MM-yyyy");
                person.BirthPlace = adminPersonModel.BirthPlace;
                person.Description = adminPersonModel.Description;
                person.FirstName = adminPersonModel.FirstName;
                person.LastName = adminPersonModel.LastName;

                await _applicationDbContext.SaveChangesAsync();

                return await Read(person.ID);
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
