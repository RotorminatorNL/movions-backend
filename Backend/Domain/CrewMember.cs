using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class CrewMember
    {
        public int CrewMemberID { get; set; }
        public string CharacterName { get; set; }
        public CrewRoles Role { get; set; }
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
        public int PersonID { get; set; }
        public Person Person { get; set; }
    }
}
