﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Movie
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<CrewRole> Crew { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public Language Language { get; set; }
        public int LanguageID { get; set; }
    }
}
