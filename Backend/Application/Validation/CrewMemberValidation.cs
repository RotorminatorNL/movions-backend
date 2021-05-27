using Application.AdminModels;
using Domain.Enums;
using System;

namespace Application.Validation
{
    public class CrewMemberValidation
    {
        public bool IsInputValid(AdminCrewMemberModel adminCrewMemberModel)
        {
            bool isCharacterNameOk = !(adminCrewMemberModel.CharacterName == null || adminCrewMemberModel.CharacterName == "");
            bool isRoleOk = Enum.IsDefined(typeof(CrewRoles), adminCrewMemberModel.Role);

            return isCharacterNameOk && isRoleOk;
        }

        public bool IsInputDifferent(Domain.CrewMember crewMember, AdminCrewMemberModel adminCrewMemberModel)
        {
            bool isCharacterNameOk = crewMember.CharacterName != adminCrewMemberModel.CharacterName;
            bool isRoleOk = crewMember.Role != adminCrewMemberModel.Role;

            return isCharacterNameOk || isRoleOk;
        }
    }
}
