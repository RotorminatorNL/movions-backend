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
    public class Language
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly LanguageValidation _languageValidation;

        public Language(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _languageValidation = new LanguageValidation();
        }

        public async Task<AdminLanguageModel> Create(AdminLanguageModel adminLanguageModel)
        {
            if (_languageValidation.IsInputValid(adminLanguageModel))
            {
                var language = new Domain.Language
                {
                    Name = adminLanguageModel.Name
                };

                _applicationDbContext.Languages.Add(language);

                await _applicationDbContext.SaveChangesAsync();

                return new AdminLanguageModel
                {
                    ID = language.ID,
                    Name = language.Name
                };
            }

            return null;
        }

        public async Task<LanguageModel> Read(int id)
        {
            return await _applicationDbContext.Languages.Select(language => new LanguageModel
            {
                ID = language.ID,
                Name = language.Name,
            }).FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<LanguageModel>> ReadAll()
        {
            return await _applicationDbContext.Languages.Select(language => new LanguageModel
            {
                ID = language.ID,
                Name = language.Name,
            }).ToListAsync();
        }

        public async Task<AdminLanguageModel> Update(AdminLanguageModel adminLanguageModel) 
        {
            var language = _applicationDbContext.Languages.FirstOrDefault(x => x.ID == adminLanguageModel.ID);

            if (language != null && _languageValidation.IsInputValid(adminLanguageModel))
            {
                language.Name = adminLanguageModel.Name;

                await _applicationDbContext.SaveChangesAsync();

                return new AdminLanguageModel { 
                    ID = language.ID, 
                    Name = language.Name 
                };
            }

            return null;
        }

        public async Task<bool> Delete(int id) 
        {
            var language = _applicationDbContext.Languages.FirstOrDefault(x => x.ID == id);

            if (language != null)
            {
                _applicationDbContext.Languages.Remove(language);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }
 
            return false;
        }
    }
}
