using DotNetCoreExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreExample.MovieRepository
{
    public interface IMovieRepo
    {
        MovieClass getMovie(string title);
    }
}
