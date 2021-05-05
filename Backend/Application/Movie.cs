using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Movie
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly MovieValidation _movieValidation;

        public Movie(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _movieValidation = new MovieValidation();
        }

        public async Task<AdminMovieModel> Create(AdminMovieModel adminMovieModel)
        {
            if(_movieValidation.CreateMovie(adminMovieModel))
            {
                var movie = new Domain.Movie
                {
                    Companies = adminMovieModel.Companies.Select(c => new Domain.CompanyMovie
                    {
                        CompanyID = c.ID
                    }).ToList(),
                    Crew = adminMovieModel.Crew.Select(c => new Domain.CrewRole
                    {
                        CharacterName = c.CharacterName,
                        PersonID = c.PersonID,
                        Role = (Domain.CrewRole.Roles)c.Role
                    }).ToList(),
                    Description = adminMovieModel.Description,
                    Genres = adminMovieModel.Genres.Select(g => new Domain.GenreMovie
                    {
                        GenreID = g.ID
                    }).ToList(),
                    LanguageID = adminMovieModel.Language.ID,
                    Length = adminMovieModel.Length,
                    ReleaseDate = adminMovieModel.ReleaseDate.ToShortDateString(),
                    Title = adminMovieModel.Title
                };

                _applicationDbContext.Movies.Add(movie);
                await _applicationDbContext.SaveChangesAsync();

                var returnData = new AdminMovieModel
                {
                    ID = movie.ID,
                    Companies = movie.Companies.Select(c => new AdminCompanyModel
                    {
                        ID = c.CompanyID,
                        Name = c.Company.Name,
                        Type = (AdminCompanyModel.Types)c.Company.Type
                    }),
                    Crew = movie.Crew.Select(c => new AdminCrewRoleModel
                    {
                        ID = c.CrewRoleID,
                        CharacterName = c.CharacterName,
                        Person = new AdminPersonModel
                        {
                            ID = c.Person.ID,
                            FirstName = c.Person.FirstName,
                            LastName = c.Person.LastName
                        },
                        Role = (AdminCrewRoleModel.Roles)c.Role
                    }),
                    Description = movie.Description,
                    Genres = movie.Genres.Select(g => new AdminGenreModel
                    {
                        ID = g.GenreID,
                        Name = g.Genre.Name
                    }),
                    Language = new AdminLanguageModel
                    {
                        ID = movie.Language.ID,
                        Name = movie.Language.Name
                    },
                    Length = movie.Length,
                    ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                    Title = movie.Title
                };

                return returnData;
            }

            return null;
        }

        public MovieModel Read(int id)
        {
            return _applicationDbContext.Movies.Select(movie => new MovieModel
            {
                ID = movie.ID,
                Companies = movie.Companies.OrderBy(c => c.Company.ID).Select(c => new CompanyModel
                {
                    ID = c.Company.ID,
                    Name = c.Company.Name,
                    Type = c.Company.Type.ToString()
                }),
                Crew = movie.Crew.OrderBy(c => c.CrewRoleID).Select(c => new CrewRoleModel
                {
                    ID = c.CrewRoleID,
                    CharacterName = c.CharacterName,
                    Person = new PersonModel
                    {
                        ID = c.Person.ID,
                        FirstName = c.Person.FirstName,
                        LastName = c.Person.LastName
                    },
                    Role = c.Role.ToString()
                }),
                Description = movie.Description,
                Genres = movie.Genres.OrderBy(c => c.Genre.ID).Select(g => new GenreModel
                {
                    ID = g.GenreID,
                    Name = g.Genre.Name
                }),
                Language = new LanguageModel
                {
                    ID = movie.Language.ID,
                    Name = movie.Language.Name
                },
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            }).FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<MovieModel> ReadAll()
        {
            return _applicationDbContext.Movies.Select(movie => new MovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            }).ToList();
        }

        public async Task<AdminMovieModel> Update(AdminMovieModel adminMovieModel)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == adminMovieModel.ID);

            movie.Description = adminMovieModel.Description;
            movie.Length = adminMovieModel.Length;
            movie.ReleaseDate = adminMovieModel.ReleaseDate.ToShortDateString();
            movie.Title = adminMovieModel.Title;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminMovieModel
            {
                ID = movie.ID,
                Description = movie.Description,
                Length = movie.Length,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Title = movie.Title
            };
        }

        public async Task<bool> Delete(int id)
        {
            var movie = _applicationDbContext.Movies.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Movies.Remove(movie);

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
