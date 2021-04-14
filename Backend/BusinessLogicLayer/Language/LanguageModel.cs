using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class LanguageModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<MovieModel> Movies { get; set; }
    }
}
