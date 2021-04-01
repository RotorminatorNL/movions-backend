﻿using System;
using System.Collections.Generic;

namespace BusinessLogicLayer
{
    public class PersonModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderModel Gender { get; set; }
        public string Description { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public IEnumerable<CrewRoleModel> Roles { get; set; }
    }
}
