using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCoreExample.Models;
using DotNetCoreExample.Utils;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using static DotNetCoreExample.MovieRepository.MovieRepoFactory;

namespace DotNetCoreExample.MovieRepository
{
    public class OmdbRepo : IMovieRepo
    {
        private HttpClient client = new HttpClient();

        public OmdbRepo()
        {
            client.BaseAddress = new Uri(ProjectVariables.omdbURL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public MovieClass getMovie(string title)
        {
            MovieClass returnObj = null;
            string keyword = title.Replace(' ', '+');
            string apiText = string.Format("?t={0}&apikey={1}", keyword, ProjectVariables.omdbApiKey); 
            
            HttpResponseMessage response = client.GetAsync(apiText).Result;
            if(response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                OmdbClass omdbObj = JsonConvert.DeserializeObject<OmdbClass>(result);

                if (omdbObj == null || omdbObj.Title == null)
                    return null;

                returnObj = parseToMovieClass(omdbObj);
            }

            return returnObj;
        }

        private MovieClass parseToMovieClass(OmdbClass omdbObj)
        {
            MovieClass returnObj = new MovieClass();
            returnObj.t = omdbObj.Title;
            returnObj.rd = getReleasedDate(omdbObj.Released);
            returnObj.g = getGenre(omdbObj.Genre);
            returnObj.s = omdbObj.Plot;
            returnObj.md = Functions.stringToList(omdbObj.Director, ", ");
            returnObj.ma = Functions.stringToList(omdbObj.Actors, ", ");

            return returnObj;
        }

        private int getGenre(string omdbGenre)
        {
            List<string> tmp = Functions.stringToList(omdbGenre, ", ");
            switch (tmp[0])
            {
                case "Horror":
                    return (int)Genres.HORROR;

                case "Drama":
                    return (int)Genres.DRAMA;

                case "Mystery":
                    return (int)Genres.MYSTERY;

                case "Thriller":
                    return (int)Genres.THRILLER;

                case "Action":
                    return (int)Genres.ACTION;

                case "Adventure":
                    return (int)Genres.ADVENTURE;

                case "Sci-Fi":
                    return (int)Genres.SCIFI;

                case "Biography":
                    return (int)Genres.BIOGRAPHY;

                case "History":
                    return (int)Genres.HISTORY;

                case "War":
                    return (int)Genres.WAR;

                default:
                    return (int)Genres.UNKNOWEN;
            }
        }

        private string getReleasedDate(string omdbReleaseDate)
        {
            DateTime parsedDate = DateTime.Parse(omdbReleaseDate);
            return DBTypesEvaluator.GetTimeFullString(parsedDate);
        }
    }
}
