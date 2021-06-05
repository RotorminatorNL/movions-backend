using Application.AdminModels;
using Application.Validation;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using PersistenceInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Company
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly CompanyValidation _companyValidation;

        private CompanyModel GetCompanyModelWithErrorID(object company, object movie)
        {
            if (company != null && movie == null)
            {
                return new CompanyModel
                {
                    ID = -1
                };
            }

            if (company == null && movie != null)
            {
                return new CompanyModel
                {
                    ID = -2
                };
            }

            if (company == null && movie == null)
            {
                return new CompanyModel
                {
                    ID = -3
                };
            }

            return new CompanyModel
            {
                ID = -4
            };
        }

        public Company(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _companyValidation = new CompanyValidation();
        }

        public async Task<CompanyModel> Create(AdminCompanyModel adminCompanyModel)
        {
            if (_companyValidation.IsInputValid(adminCompanyModel))
            {
                var company = new Domain.Company
                {
                    Name = adminCompanyModel.Name,
                    Type = adminCompanyModel.Type
                };

                _applicationDbContext.Companies.Add(company);
                await _applicationDbContext.SaveChangesAsync();

                return await Read(company.ID);
            }

            return null;
        }

        public async Task<CompanyModel> ConnectMovie(AdminCompanyMovieModel adminCompanyMovieModel)
        {
            var company = await _applicationDbContext.Companies.FirstOrDefaultAsync(c => c.ID == adminCompanyMovieModel.CompanyID);
            var movie = await _applicationDbContext.Movies.FirstOrDefaultAsync(c => c.ID == adminCompanyMovieModel.MovieID);

            if (company != null && movie != null)
            {
                var companyMovie = new Domain.CompanyMovie
                {
                    CompanyID = adminCompanyMovieModel.CompanyID,
                    MovieID = adminCompanyMovieModel.MovieID
                };

                _applicationDbContext.CompanyMovies.Add(companyMovie);
                await _applicationDbContext.SaveChangesAsync();

                return await Read(adminCompanyMovieModel.CompanyID);
            }

            return GetCompanyModelWithErrorID(company, movie);
        }

        public async Task<CompanyModel> Read(int id)
        {
            return await _applicationDbContext.Companies.Select(c => new CompanyModel
            {
                ID = c.ID,
                Name = c.Name,
                Type = c.Type.ToString(),
                Movies = c.Movies.Select(m => new MovieModel
                {
                    ID = m.MovieID,
                    Title = m.Movie.Title
                })
            }).FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<IEnumerable<CompanyModel>> ReadAll()
        {
            return await _applicationDbContext.Companies.Select(c => new CompanyModel
            {
                ID = c.ID,
                Name = c.Name,
                Type = c.Type.ToString()
            }).ToListAsync();
        }

        public async Task<CompanyModel> Update(AdminCompanyModel adminCompanyModel)
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(c => c.ID == adminCompanyModel.ID);

            if (company != null && _companyValidation.IsInputValid(adminCompanyModel))
            {
                company.Name = adminCompanyModel.Name;
                company.Type = adminCompanyModel.Type;

                await _applicationDbContext.SaveChangesAsync();

                return await Read(adminCompanyModel.ID);
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var company = _applicationDbContext.Companies.FirstOrDefault(c => c.ID == id);

            if (company != null)
            {
                _applicationDbContext.Companies.Remove(company);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }
                
            return false;
        }

        public async Task<CompanyModel> DisconnectMovie(AdminCompanyMovieModel adminCompanyMovieModel)
        {
            var company = await _applicationDbContext.Companies.FirstOrDefaultAsync(c => c.ID == adminCompanyMovieModel.CompanyID);
            var movie = await _applicationDbContext.Movies.FirstOrDefaultAsync(c => c.ID == adminCompanyMovieModel.MovieID);

            if (company != null && movie != null)
            {
                var companyMovie = _applicationDbContext
                                    .CompanyMovies
                                    .FirstOrDefault(c => c.CompanyID == company.ID && c.MovieID == movie.ID);

                if (companyMovie != null)
                {
                    _applicationDbContext.CompanyMovies.Remove(companyMovie);
                    await _applicationDbContext.SaveChangesAsync();

                    return new CompanyModel();
                }
            }

            return GetCompanyModelWithErrorID(company, movie);
        }
    }
}
