using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class AdminCompanyModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Types Type { get; set; }
        public IEnumerable<AdminMovieModel> Movies { get; set; }

        public enum Types
        {
            Distributor,
            Producer
        }
    }
}
