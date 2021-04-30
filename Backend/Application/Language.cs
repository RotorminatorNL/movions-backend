using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Language
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public Language(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminLanguageModel> Create(AdminLanguageModel adminLanguageModel)
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

        public IEnumerable<LanguageModel> ReadAll()
        {
            return _applicationDbContext.Languages.ToList().Select(language => new LanguageModel
            {
                ID = language.ID,
                Name = language.Name,
            });
        }

        public LanguageModel Read(int id)
        {
            return _applicationDbContext.Languages.ToList().Select(language => new LanguageModel
            {
                ID = language.ID,
                Name = language.Name,
            }).FirstOrDefault(x => x.ID == id);
        }

        public async Task<AdminLanguageModel> Update(AdminLanguageModel adminLanguageModel) 
        {
            var language = _applicationDbContext.Languages.FirstOrDefault(x => x.ID == adminLanguageModel.ID);

            language.Name = adminLanguageModel.Name;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminLanguageModel
            {
                ID = language.ID,
                Name = language.Name
            };
        }

        public async Task<bool> Delete(int id) 
        {
            var language = _applicationDbContext.Languages.FirstOrDefault(x => x.ID == id);

            _applicationDbContext.Languages.Remove(language);
 
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
