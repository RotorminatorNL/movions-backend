﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class MovieModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Title { get; set; }
        public IEnumerable<CompanyModel> Companies { get; set; }
        public IEnumerable<CrewRoleModel> Crew { get; set; }
        public IEnumerable<GenreModel> Genres { get; set; }
        public LanguageModel Language { get; set; }
    }
}