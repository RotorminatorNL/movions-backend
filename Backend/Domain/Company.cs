using Domain.Enums;
using System.Collections.Generic;

namespace Domain
{
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public CompanyTypes Type { get; set; }
        public IEnumerable<CompanyMovie> Movies { get; set; }
    }
}
