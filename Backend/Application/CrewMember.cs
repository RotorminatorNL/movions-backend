using Application.Validation;
using PersistenceInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class CrewMember
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly CrewMemberValidation _crewMemberValidation;

        public CrewMember(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _crewMemberValidation = new CrewMemberValidation();
        }

        public async Task<AdminCrewMemberModel> Create(AdminCrewMemberModel adminCrewRoleModel)
        {
            if (_crewMemberValidation.IsInputValid(adminCrewRoleModel))
            {
                var crewRole = new Domain.CrewMember
                {
                    CharacterName = adminCrewRoleModel.CharacterName,
                    Role = adminCrewRoleModel.Role
                };

                _applicationDbContext.CrewMembers.Add(crewRole);

                await _applicationDbContext.SaveChangesAsync();

                return new AdminCrewMemberModel
                {
                    ID = crewRole.CrewMemberID,
                    CharacterName = crewRole.CharacterName,
                    Role = crewRole.Role
                };
            }

            return null;
        }

        public CrewMemberModel Read(int id)
        {
            return _applicationDbContext.CrewMembers.ToList().Select(crewRole => new CrewMemberModel
            {
                ID = crewRole.CrewMemberID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString(),
                MovieID = crewRole.MovieID,
            }).FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<CrewMemberModel> ReadAll()
        {
            return _applicationDbContext.CrewMembers.ToList().Select(crewRole => new CrewMemberModel
            {
                ID = crewRole.CrewMemberID,
                CharacterName = crewRole.CharacterName,
                Role = crewRole.Role.ToString(),
                MovieID = crewRole.MovieID,
            });
        }

        public async Task<AdminCrewMemberModel> Update(AdminCrewMemberModel adminCrewRoleModel)
        {
            var crewRole = _applicationDbContext.CrewMembers.FirstOrDefault(x => x.CrewMemberID == adminCrewRoleModel.ID);

            if (crewRole != null && _crewMemberValidation.IsInputValid(adminCrewRoleModel))
            {
                if (_crewMemberValidation.IsInputDifferent(crewRole, adminCrewRoleModel))
                {
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

                return new AdminCrewMemberModel();
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var crewRole = _applicationDbContext.CrewMembers.FirstOrDefault(x => x.CrewMemberID == id);

            if (crewRole != null)
            {
                _applicationDbContext.CrewMembers.Remove(crewRole);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
