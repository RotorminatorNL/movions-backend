using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieLanguage
    {
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
        public int LanguageID { get; set; }
        public Language Language { get; set; }
    }
}
