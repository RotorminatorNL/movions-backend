using System.Collections.Generic;

namespace Domain
{
    public class Language
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
    }
}
