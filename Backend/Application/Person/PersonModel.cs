using System;
using System.Collections.Generic;

namespace Application
{
    public class PersonModel
    {
        public int ID { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<CrewRoleModel> Roles { get; set; }
    }
}
