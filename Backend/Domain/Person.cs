﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Person
    {
        public int ID { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<CrewRole> Roles { get; set; }
    }
}
