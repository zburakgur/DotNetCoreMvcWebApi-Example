using DotNetCoreExample.Models;
using DotNetCoreExample.MovieRepository;
using DotNetCoreExample.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCoreExample.BackgroundTasks
{
    public class MovieDataRefreshService : HostedService
    {
       // private readonly RandomStringProvider _randomStringProvider;

        public MovieDataRefreshService()
        {
            //_randomStringProvider = randomStringProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await TaskImplementation(cancellationToken);
                await Task.Delay(TimeSpan.FromMinutes(ProjectVariables.serviceSleepTimeMinute), cancellationToken);
            }
        }

        protected Task<bool> TaskImplementation(CancellationToken cancellationToken)
        {
            SqlConnection connection = new SqlConnection(ProjectVariables.dbConnectionString);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                /* read id of the last movie which it's data refreshed */
                int lastRefreshedId = DBFunctions.getLastRefreshedId(command);

                /* these movies will be refreshed from remote repository */
                List<MovieClass> movieList = DBFunctions.getMovieTitles(command, lastRefreshedId);

                MovieRepoFactory repoFactory = MovieRepoFactory.getInstance();
                IMovieRepo repo = repoFactory.getRepo(MovieRepoFactory.Repos.OMDB);

                int i = 0;
                for(i=0; i < movieList.Count; i++)
                {
                    MovieClass oldMovie = movieList[i];
                    MovieClass newMovie = repo.getMovie(oldMovie.t);
                    newMovie.i = oldMovie.i;

                    DBFunctions.updateMovie(command, newMovie);
                    DBFunctions.updateLastRefreshedId(command, newMovie.i);
                }

                /* if all movies are refreshed, then start from the first one */
                if(i == 0)
                    DBFunctions.updateLastRefreshedId(command, 0);
            }
            catch (Exception ex)
            {
                Functions.LogWebMethodError(this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return Task.FromResult<bool>(true);
        }
    }
}
