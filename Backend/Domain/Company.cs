using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Types Type { get; set; }
        public IEnumerable<Movie> Movies { get; set; }

        public enum Types
        {
            Distributor,
            Producer
        }
    }
}
