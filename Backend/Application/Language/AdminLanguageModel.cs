﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class AdminLanguageModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<AdminMovieModel> Movies { get; set; }
    }
}