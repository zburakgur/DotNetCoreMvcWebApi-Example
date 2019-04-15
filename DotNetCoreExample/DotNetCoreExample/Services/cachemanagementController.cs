using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreExample.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetCoreExample.Services
{
    [Route("api/cachemanagementController/[action]")]
    [ApiController]
    public class cachemanagementController : ControllerBase
    {
        private IMemoryCache cache;

        public cachemanagementController(IMemoryCache cache)
        {
            this.cache = cache;
        }

        [HttpGet]
        public void clear()
        {
            cache.Remove(ProjectVariables.MOVIE_CACHE_TAG);
        }
    }
}