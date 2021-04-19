using DataAccessLayer;
using DataAccessLayerInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class CrewRole
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CrewRole(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminCrewRoleModel> Create(AdminCrewRoleModel adminCrewRoleModel)
        {
            var crewRole = new Domain.CrewRole
            {
                CharacterName = adminCrewRoleModel.CharacterName,
                Role = (Domain.CrewRole.Roles)adminCrewRoleModel.Role
            };

            _applicationDbContext.CrewRoles.Add(crewRole);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCrewRoleModel
            {
                ID = crewRole.ID,
                CharacterName = crewRole.CharacterName,
                Role = (AdminCrewRoleModel.Roles)crewRole.Role
            };
        }

        public IEnumerable<CrewRoleModel> ReadAll()
        {
            return _applicationDbContext.CrewRoles
            .Include(crewRole => crewRole.Movie)
            .Include(crewRole => crewRole.Person)
            .ToList().Select(crewRole => new CrewRoleModel
            {
                ID = crewRole.ID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString(),
                MovieID = crewRole.MovieID,
                Movie = new MovieModel
                {
                    ID = crewRole.Movie.ID,
                    Description = crewRole.Movie.Description,
                    Length = crewRole.Movie.Length,
                    ReleaseDate = crewRole.Movie.ReleaseDate,
                    Title = crewRole.Movie.Title,
                    //Companies = crewRole.Movie.Companies.Select(company => new CompanyModel
                    //{
                    //    ID = company.ID,
                    //    Name = company.Name,
                    //    Type = company.Type.ToString()
                    //}),
                    //Crew = null, // already displayed
                    //Genres = crewRole.Movie.Genres.Select(genre => new GenreModel
                    //{
                    //    ID = genre.ID,
                    //    Name = genre.Name
                    //}),
                    //Language = new LanguageModel
                    //{
                    //    ID = crewRole.Movie.Language.ID,
                    //    Name = crewRole.Movie.Language.Name
                    //}
                },
                PersonID = crewRole.PersonID,
                Person = new PersonModel
                {
                    ID = crewRole.Person.ID,
                    BirthDate = crewRole.Person.BirthDate,
                    BirthPlace = crewRole.Person.BirthPlace,
                    Description = crewRole.Person.Description,
                    FirstName = crewRole.Person.FirstName,
                    LastName = crewRole.Person.LastName
                }
            });
        }

        public CrewRoleModel Read(int id)
        {
            return _applicationDbContext.CrewRoles
            .Include(crewRole => crewRole.Movie)
            .Include(crewRole => crewRole.Person)
            .ToList()
            .Select(crewRole => new CrewRoleModel
            {
                ID = crewRole.ID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString(),
                MovieID = crewRole.MovieID,
                Movie = new MovieModel
                {
                    ID = crewRole.Movie.ID,
                    Description = crewRole.Movie.Description,
                    Length = crewRole.Movie.Length,
                    ReleaseDate = crewRole.Movie.ReleaseDate,
                    Title = crewRole.Movie.Title,
                    //Companies = crewRole.Movie.Companies.Select(company => new CompanyModel
                    //{
                    //    ID = company.ID,
                    //    Name = company.Name,
                    //    Type = company.Type.ToString()
                    //}),
                    //Crew = null, // already displayed
                    //Genres = crewRole.Movie.Genres.Select(genre => new GenreModel
                    //{
                    //    ID = genre.ID,
                    //    Name = genre.Name
                    //}),
                    //Language = new LanguageModel
                    //{
                    //    ID = crewRole.Movie.Language.ID,
                    //    Name = crewRole.Movie.Language.Name
                    //}
                },
                PersonID = crewRole.PersonID,
                Person = new PersonModel
                {
                    ID = crewRole.Person.ID,
                    BirthDate = crewRole.Person.BirthDate,
                    BirthPlace = crewRole.Person.BirthPlace,
                    Description = crewRole.Person.Description,
                    FirstName = crewRole.Person.FirstName,
                    LastName = crewRole.Person.LastName
                }
            }).FirstOrDefault(x => x.ID == id);
        }

        public async Task<AdminCrewRoleModel> Update(AdminCrewRoleModel adminCrewRoleModel) 
        {
            var crewRole = _applicationDbContext.CrewRoles.FirstOrDefault(x => x.ID == adminCrewRoleModel.ID);

            crewRole.CharacterName = adminCrewRoleModel.CharacterName;
            crewRole.Role = (Domain.CrewRole.Roles)adminCrewRoleModel.Role;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCrewRoleModel
            {
                ID = crewRole.ID,
                CharacterName = crewRole.CharacterName,
                Role = (AdminCrewRoleModel.Roles)crewRole.Role
            };
        }

        public async Task<bool> Delete(int id) 
        {
            var crewRole = _applicationDbContext.CrewRoles.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.CrewRoles.Remove(crewRole);
 
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
