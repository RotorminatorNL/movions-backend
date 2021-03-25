using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessContractLayer
{
    public class CrewRole
    {
        public int ID { get; set; }
        public Roles Role { get; set; }
        public string CharacterName { get; set; }
    }

    public enum Roles
    {
        Actor,
        Director,
        Writer
    }
}
