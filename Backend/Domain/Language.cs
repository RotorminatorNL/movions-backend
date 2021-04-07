using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Language
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<MovieLanguage> Movies { get; set; }
    }
}
