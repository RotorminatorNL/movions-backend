using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieCompany
    {
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; }
    }
}
