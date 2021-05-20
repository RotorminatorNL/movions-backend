using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class CrewMemberModel
    {
        public int ID { get; set; }
        public string Role { get; set; }
        public string CharacterName { get; set; }
        public int MovieID { get; set; }
        public MovieModel Movie { get; set; }
        public int PersonID { get; set; }
        public PersonModel Person { get; set; }
    }
}
