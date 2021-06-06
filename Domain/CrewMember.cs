using Domain.Enums;

namespace Domain
{
    public class CrewMember
    {
        public int ID { get; set; }
        public string CharacterName { get; set; }
        public CrewRoles Role { get; set; }
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
        public int PersonID { get; set; }
        public Person Person { get; set; }
    }
}
