using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektas
{
    public class Movies: BaseModel
    {
        
        public int id { get; set; }
        public string imdbID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string imdbRating { get; set; }
        public string Genre { get; set; }
        public string Plot { get; set; }
        public string Actors { get; set; }
        public string imdbLink { get; set; }
        public string Poster { get; set; }
    }
}
