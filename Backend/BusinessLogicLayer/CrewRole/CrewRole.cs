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
            return _applicationDbContext.CrewRoles.ToList().Select(crewRole => new CrewRoleModel
            {
                ID = crewRole.ID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString()
            });
        }

        public CrewRoleModel Read(int id)
        {
            return _applicationDbContext.CrewRoles.ToList().Select(crewRole => new CrewRoleModel
            {
                ID = crewRole.ID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString()
            }).FirstOrDefault(c => c.ID == id);
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
