﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class AdminCrewRoleModel
    {
        public int ID { get; set; }
        public Roles Role { get; set; }
        public string CharacterName { get; set; }

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