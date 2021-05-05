using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class CrewRole
    {
        public int CrewRoleID { get; set; }
        public string CharacterName { get; set; }
        public Roles Role { get; set; }
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
        public int PersonID { get; set; }
        public Person Person { get; set; }

        public enum Roles
        {
            Actor,
            Director,
            Editor,
            Producer,
            Writer
        }
    }
}
