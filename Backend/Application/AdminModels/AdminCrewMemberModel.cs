using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class AdminCrewMemberModel
    {
        public int ID { get; set; }
        public CrewRoles Role { get; set; }
        public string CharacterName { get; set; }
        public int MovieID { get; set; }
        public AdminMovieModel Movie { get; set; }
        public int PersonID { get; set; }
        public AdminPersonModel Person { get; set; }
    }
}
