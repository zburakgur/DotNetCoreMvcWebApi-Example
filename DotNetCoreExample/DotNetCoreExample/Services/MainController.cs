using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreExample.Models;
using DotNetCoreExample.MovieRepository;
using DotNetCoreExample.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace DotNetCoreExample.Services
{
    [Route("api/MainController/[action]")]
    [ApiController]
    public class MainController : Controller
    {
        private IMemoryCache cache;

        public MainController(IMemoryCache cache)
        {
            this.cache = cache;
        }

        /**
         * Parameters:
         *  title: movie title 
         * 
         * Returns:
         *  MovieClass JSON object, look at JSONClasses.js
         *  
         * Not: Returns only one result
         **/
         
        [HttpPost("{userName}/{password}/{title}")]
        public JsonResult GetMovie(string userName, string password, string title)
        {
            GetMovieResult result = new GetMovieResult();
            SqlConnection connection = new SqlConnection(ProjectVariables.dbConnectionString);

            try
            {
                Functions.EnsureNotNullCredentials(userName, password);

                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                bool flag = DBFunctions.checkLogin(command, userName, password);
                if (!flag)                
                    result.isErr = true;
                else
                {
                    // get movie from cache if it is cached
                    MovieClass cacheMovie = null;
                    MovieClass movie = null;       
                    if (Functions.getCache<MovieClass>(cache, ProjectVariables.MOVIE_CACHE_TAG, out cacheMovie))
                    {
                        if(cacheMovie.t.Contains(title))
                            movie = cacheMovie;
                    }
                        
                    // read movie from database
                    if(movie == null)
                        movie = DBFunctions.getMovie(command, title);

                    // get the movie from remote repositories
                    if (movie == null)
                    {
                        MovieRepoFactory repoFactory = MovieRepoFactory.getInstance();
                        IMovieRepo repo = repoFactory.getRepo(MovieRepoFactory.Repos.OMDB);

                        movie = repo.getMovie(title);
                        if(movie != null)
                            DBFunctions.insertMovie(command, movie);
                    }

                    // add to cache
                    if(movie != null)
                        Functions.setCache<MovieClass>(cache, ProjectVariables.MOVIE_CACHE_TAG, movie);

                    result.m = movie;
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                result.isErr = true;
                Functions.LogWebMethodError(this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return Json(result);
        }
    }
}