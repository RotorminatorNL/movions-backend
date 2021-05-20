using Domain.Enums;
using System.Collections.Generic;


namespace Application
{
    public class AdminCompanyModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public CompanyTypes Type { get; set; }
        public IEnumerable<AdminMovieModel> Movies { get; set; }
    }
}
