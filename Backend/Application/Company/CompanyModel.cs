using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class CompanyModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IEnumerable<MovieModel> Movies { get; set; }
    }
}
