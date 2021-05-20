using PersistenceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class CrewRole
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public CrewRole(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AdminCrewMemberModel> Create(AdminCrewMemberModel adminCrewRoleModel)
        {
            var crewRole = new Domain.CrewMember
            {
                CharacterName = adminCrewRoleModel.CharacterName,
                Role = adminCrewRoleModel.Role
            };

            _applicationDbContext.CrewRoles.Add(crewRole);

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCrewMemberModel
            {
                ID = crewRole.CrewMemberID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role
            };
        }

        public IEnumerable<CrewMemberModel> ReadAll()
        {
            return _applicationDbContext.CrewRoles.ToList().Select(crewRole => new CrewMemberModel
            {
                ID = crewRole.CrewMemberID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString(),
                MovieID = crewRole.MovieID,
            });
        }

        public CrewMemberModel Read(int id)
        {
            return _applicationDbContext.CrewRoles.ToList().Select(crewRole => new CrewMemberModel
            {
                ID = crewRole.CrewMemberID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString(),
                MovieID = crewRole.MovieID,
            }).FirstOrDefault(x => x.ID == id);
        }

        public async Task<AdminCrewMemberModel> Update(AdminCrewMemberModel adminCrewRoleModel) 
        {
            var crewRole = _applicationDbContext.CrewRoles.FirstOrDefault(x => x.CrewMemberID == adminCrewRoleModel.ID);

            crewRole.CharacterName = adminCrewRoleModel.CharacterName;
            crewRole.Role = adminCrewRoleModel.Role;

            await _applicationDbContext.SaveChangesAsync();

            return new AdminCrewMemberModel
            {
                ID = crewRole.CrewMemberID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role
            };
        }

        public async Task<bool> Delete(int id) 
        {
            var crewRole = _applicationDbContext.CrewRoles.FirstOrDefault(x => x.CrewMemberID == id);

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
