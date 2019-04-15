using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreExample.Models
{
    public class LoginClass
    {
        public string uN = "";                          /* user name */
        public string p = "";                           /* password */
    }

    public class MovieClass
    {
        public int i = 0;                               /* id */
        public string t = "";                           /* title */
        public string rd = "";                          /* release date */
        public int g = 0;                               /* genre */
        public string s = "";                           /* summary */
        public List<string> md = new List<string>();    /* movie director list */
        public List<string> ma = new List<string>();    /* movie actor list */
    }

    public class GetMovieResult
    {
        public MovieClass m = new MovieClass();
        public bool isErr = false;
    }

    /* OMDB json classes */

    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }
    }

    public class OmdbClass
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public List<Rating> Ratings { get; set; }
        public string Metascore { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string DVD { get; set; }
        public string BoxOffice { get; set; }
        public string Production { get; set; }
        public string Website { get; set; }
        public string Response { get; set; }
    }
}
