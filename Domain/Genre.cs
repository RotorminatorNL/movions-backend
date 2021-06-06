using System.Collections.Generic;

namespace Domain
{
    public class Genre
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<GenreMovie> Movies { get; set; }
    }
}
