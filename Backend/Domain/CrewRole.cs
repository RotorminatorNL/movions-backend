using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class CrewRole
    {
        public int ID { get; set; }
        public string CharacterName { get; set; }
        public Roles Role { get; set; }
    }

    public enum Roles
    {
        Actor,
        Director,
        Editor,
        Producer,
        Writer
    }
}
