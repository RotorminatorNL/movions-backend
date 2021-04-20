using System;
using System.Collections.Generic;

namespace Application
{
    public class AdminPersonModel
    {
        public int ID { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<AdminCrewRoleModel> Roles { get; set; }
    }
}
