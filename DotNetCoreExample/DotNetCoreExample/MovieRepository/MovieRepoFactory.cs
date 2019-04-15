using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreExample.MovieRepository
{
    public class MovieRepoFactory
    {
        private static MovieRepoFactory instance = null;
        public enum Repos { OMDB }; 
        public enum Genres { UNKNOWEN, HORROR, DRAMA, MYSTERY, THRILLER, ACTION, ADVENTURE, SCIFI, BIOGRAPHY, HISTORY, WAR };

        public static MovieRepoFactory getInstance()
        {
            if (instance == null)
                instance = new MovieRepoFactory();

            return instance;
        }
        
        public IMovieRepo getRepo(Repos repoEnum)
        {
            IMovieRepo repo = null;
            switch (repoEnum)
            {
                case Repos.OMDB:
                    repo = new OmdbRepo();
                    break;

                default:
                    repo = new OmdbRepo();
                    break;
            }

            return repo;
        }
    }
}
