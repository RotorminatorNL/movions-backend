using System.Collections.Generic;

namespace Domain
{
    public class Person
    {
        public int ID { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<CrewMember> Roles { get; set; }
    }
}
