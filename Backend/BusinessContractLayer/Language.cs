﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessContractLayer
{
    public class Language
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
